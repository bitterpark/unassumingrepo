using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryScreenHandler : MonoBehaviour 
{
	public PartyMember selectedMember;
	//public ItemImageHandler itemPrefab;
	public TraitUIHandler traitPrefab;
	public RelationTextHandler relationPrefab;

	public Image memberPortrait;
	public Text memberNameText;
	public Text classNameText;
	//Deprecated
	public Text meleeDamageText;
	public Text rangedDamageText;

	public Text healthValueText;
	public Text startingArmorValueText;
	public Text startingStaminaValueText;
	public Text startingAmmoValueText;

	public Button kickMemberButton;
	public Button closeButton;
	
	public Text skillpointCounterText;
	
	public static InventoryScreenHandler mainISHandler;
	public bool inventoryShown=false;
	bool interactive = true;

	//PREFABS
	public InventorySlot inventorySlotPrefab;
	public MemberInventorySlot memberSlotPrefab;
	public EquipmentSlot equipmentSlotPrefab;
	public MeleeSlot meleeSlotPrefab;
	public RangedSlot rangedSlotPrefab;
	public SlotItem slotItemPrefab;
	public CombatCardGraphic cardPrefab;
	
	
	//GROUPS
	//public Transform weaponsGroup;
	public Transform meleeSlotPlacement;
	public Transform rangedSlotPlacement;
	public Transform equipmentGroup;
	public Transform inventoryGroup;
	public Transform memberInventoryGroup;
	public Transform traitGroup;
	public Transform skillGroup;
	public Transform deckGroup;
	
	/*
	public void InventoryChangeHandler() 
	{
		if (GetComponent<Canvas>().enabled==true) {RefreshInventoryItems();}
	}*/

	public void ToggleSelectedMember(PartyMember newMember)
	{
		if (selectedMember!=newMember)
		{
			//Determine whether or not inventory screen is allowed to be opened at this time
			EncounterCanvasHandler encounterManager=EncounterCanvasHandler.main;
			if (!GameEventManager.mainEventManager.drawingEvent)
			{
				OpenScreen(newMember);
				if (PartyManager.mainPartyManager.partyMembers.Contains(newMember) && !CardsScreen.missionOngoing)
					kickMemberButton.gameObject.SetActive(true);
				else 
					kickMemberButton.gameObject.SetActive(false);
			}
		} 
		else 
			CloseScreen();
	}

	public void EncounterToggleNewSelectedMember(PartyMember newMember)
	{
		if (selectedMember!=newMember) 
			OpenScreen(newMember);
		else 
			CloseScreen();
	}


	void OpenScreen(PartyMember newMember)
	{
		selectedMember=newMember;
		if (PartyManager.mainPartyManager.partyMembers.Contains(selectedMember) && !CardsScreen.missionOngoing)
			interactive = true;
		else
			interactive = false;

		memberPortrait.color=newMember.color;
		GetComponent<Canvas>().enabled=true;
		inventoryShown=true;

		memberNameText.text=newMember.name;
		classNameText.text = newMember.myClass.ToString();
		RefreshInventoryItems();
		RecipeMakeButton.EItemMade += RefreshInventoryItems;
		InventorySlot.EItemDropped += RefreshInventoryItems;
		InventorySlot.EItemUsed += RefreshInventoryItems;
	}
	/*
	void InventoryCloseEncounterEffect()
	{
		EncounterCanvasHandler encounterManager=EncounterCanvasHandler.main;
		if (selectedMember!=null && encounterManager.encounterOngoing)
		{
			if (IsMemberInBattle(selectedMember))
			{
				bool emptyBool;
				encounterManager.memberTokens[selectedMember].TryMove(out emptyBool);
			}
		}
	}

	bool AllowToggleInventoryFromEncounter(PartyMember newMember)
	{
		bool allowToggle=false;
		EncounterCanvasHandler encounterManager=EncounterCanvasHandler.main;
		//make sure guys outside of encounter can't interact with guys within an encounter and members currently in combat can't open inv
		if(encounterManager.encounterMembers.Contains(newMember) 
		&& (!IsMemberInBattle(newMember) || encounterManager.memberTokens[newMember].CanMove()))
		{
			//Only allow to open the member's inventory while in battle, do not allow to switch from it
			if (selectedMember==null) allowToggle=true;
			else if (!IsMemberInBattle(selectedMember)) allowToggle=true;
		}
		return allowToggle;
	}

	bool IsMemberInBattle(PartyMember checkedMember)
	{
		EncounterCanvasHandler encounterManager=EncounterCanvasHandler.main;
		if (encounterManager.encounterOngoing)
		{
			if (!encounterManager.roomButtons[encounterManager.memberCoords[checkedMember]].RoomHasEnemies())
			{
				return true;
			}
		}
		return false;
	}
	*/
	public void GameEndHandler()
	{
		CloseScreen();
	}
	
	public void CloseScreen()
	{
		
		selectedMember=null;
		GetComponent<Canvas>().enabled=false;
		inventoryShown=false;
		RecipeMakeButton.EItemMade -= RefreshInventoryItems;
		InventorySlot.EItemDropped -= RefreshInventoryItems;
		InventorySlot.EItemUsed -= RefreshInventoryItems;
	}
	
	public void RefreshInventoryItems()
	{
		if (inventoryShown)
		{
			RefreshLeftCornerText();
			if (interactive)
			{
				rangedSlotPlacement.gameObject.SetActive(true);
				meleeSlotPlacement.gameObject.SetActive(true);
				equipmentGroup.parent.gameObject.SetActive(true);
				inventoryGroup.parent.gameObject.SetActive(true);
				memberInventoryGroup.parent.gameObject.SetActive(true);
				RefreshWeapons();
				RefreshEquipment();
				RefreshMemberInventory();
				RefreshLocalInventory();
			}
			else
			{
				rangedSlotPlacement.gameObject.SetActive(false);
				meleeSlotPlacement.gameObject.SetActive(false);
				equipmentGroup.parent.gameObject.SetActive(false);
				inventoryGroup.parent.gameObject.SetActive(false);
				memberInventoryGroup.parent.gameObject.SetActive(false);
			}
			RefreshTraits();
			RefreshMercDeck();
		}
	}

	void RefreshLeftCornerText()
	{
		
		healthValueText.text=selectedMember.GetHealth().ToString();
		startingArmorValueText.text = selectedMember.GetStartArmor().ToString();
		startingStaminaValueText.text = selectedMember.GetMaxStamina().ToString();
		startingAmmoValueText.text = selectedMember.GetStartAmmo().ToString();



		//Update weapon damage
		/*
		string meleeText = "Melee:";
		meleeText += selectedMember.GetMeleeDamageString();
		meleeDamageText.text = meleeText;

		string rangedText = "Ranged:";
		rangedText += selectedMember.GetRangedDamage();
		rangedDamageText.text = rangedText;
		 */
	}

	void RefreshWeapons()
	{
		//Remove old weapons
		Image oldMeleeSlot = meleeSlotPlacement.GetComponentInChildren<Image>();
		Image oldRangedSlot = rangedSlotPlacement.GetComponentInChildren<Image>();
		
		if (oldMeleeSlot != null) 
			GameObject.Destroy(oldMeleeSlot.gameObject);
		if (oldRangedSlot != null) 
			GameObject.Destroy(oldRangedSlot.gameObject);

		//Refresh melee slot
		MeleeSlot newMeleeSlot = Instantiate(meleeSlotPrefab);
		newMeleeSlot.transform.SetParent(meleeSlotPlacement, false);
		if (selectedMember.equippedMeleeWeapon != null)
		{
			SlotItem meleeWeapon = Instantiate(slotItemPrefab);
			meleeWeapon.AssignItem(selectedMember.equippedMeleeWeapon);
			newMeleeSlot.AssignItem(meleeWeapon);
			//!!NO ONCLICK LISTENER ASSIGNED!!
		}

		//Refresh ranged slot
		RangedSlot newRangedSlot = Instantiate(rangedSlotPrefab);
		newRangedSlot.transform.SetParent(rangedSlotPlacement, false);
		if (selectedMember.equippedRangedWeapon != null)
		{
			SlotItem rangedWeapon = Instantiate(slotItemPrefab);
			rangedWeapon.AssignItem(selectedMember.equippedRangedWeapon);
			newRangedSlot.AssignItem(rangedWeapon);
			//!!NO ONCLICK LISTENER ASSIGNED!!
		}
	}

	void RefreshEquipment()
	{
		//Refresh current equipment
		foreach (Image oldItemSlot in equipmentGroup.GetComponentsInChildren<Image>())//.GetComponentsInChildren<Button>())
		{
			GameObject.Destroy(oldItemSlot.gameObject);
		}
		int equipmentSlotCount = 4;
		for (int i = 0; i < equipmentSlotCount; i++)
		{
			EquipmentSlot newSlot = Instantiate(equipmentSlotPrefab);
			newSlot.transform.SetParent(equipmentGroup, false);
			if (i < selectedMember.equippedItems.Count)
			{
				SlotItem newItem = Instantiate(slotItemPrefab);
				newItem.AssignItem(selectedMember.equippedItems[i]);
				newSlot.AssignItem(newItem);
				//!!NO ONCLICK LISTENER ASSIGNED!!
			}
		}
	}

	void RefreshMemberInventory()
	{
		//REFRESH MEMBER INVENTORY
		foreach (Image oldItemSlot in memberInventoryGroup.GetComponentsInChildren<Image>())//Button>())
		{
			GameObject.Destroy(oldItemSlot.gameObject);
		}
		int memberSlotCount = selectedMember.currentCarryCapacity;
		for (int i = 0; i < memberSlotCount; i++)
		{
			InventorySlot newSlot = Instantiate(memberSlotPrefab);
			newSlot.transform.SetParent(memberInventoryGroup, false);
			if (i < selectedMember.carriedItems.Count)
			{
				SlotItem newItem = Instantiate(slotItemPrefab);
				newItem.AssignItem(selectedMember.carriedItems[i]);
				newSlot.AssignItem(newItem);
				//newItem.GetComponent<Button>().onClick.AddListener(() => InventoryItemClicked(newItem.assignedItem, newItem.currentSlot));
			}
		}
	}

	void RefreshLocalInventory()
	{
		//REFRESH INVENTORY
		foreach (Image oldItemSlot in inventoryGroup.GetComponentsInChildren<Image>())//Button>())
		{
			GameObject.Destroy(oldItemSlot.gameObject);
		}
		int slotCount = 40;
		List<InventoryItem> activeList;
		//select the right list for in-encounter and out-of-encounter
		if (EncounterCanvasHandler.main.encounterOngoing)
		{
			EncounterCanvasHandler encounterManager = EncounterCanvasHandler.main;
			activeList = encounterManager.currentEncounter.encounterMap[encounterManager.memberCoords[selectedMember]].floorItems;
		}
		else { activeList = selectedMember.currentRegion.GetStashedItems(); }//PartyManager.mainPartyManager.GetPartyInventory();}
		//Use floor or inventory list
		for (int i = 0; i < slotCount; i++)
		{
			InventorySlot newSlot = Instantiate(inventorySlotPrefab);
			newSlot.transform.SetParent(inventoryGroup, false);
			if (i < activeList.Count)
			{
				SlotItem newItem = Instantiate(slotItemPrefab);
				newItem.AssignItem(activeList[i]);
				newSlot.AssignItem(newItem);
				//newItem.GetComponent<Button>().onClick.AddListener(() => InventoryItemClicked(newItem.assignedItem, newItem.currentSlot));
			}
		}
	}

	void RefreshTraits()
	{
		//Refresh traits
		//Refresh skillpoint counter
		skillpointCounterText.text = "Skillpoints:" + selectedMember.skillpoints;
		//Delete old traits
		foreach (Image oldTraitImage in traitGroup.GetComponentsInChildren<Image>())
		{
			GameObject.Destroy(oldTraitImage.gameObject);
		}
		//Delete old skills
		foreach (Image oldSkillImage in skillGroup.GetComponentsInChildren<Image>())
		{
			GameObject.Destroy(oldSkillImage.gameObject);
		}
		//Repopulate skills and traits	
		foreach (Trait memberTrait in selectedMember.traits)
		{
			TraitUIHandler newPerkImage = Instantiate(traitPrefab);
			newPerkImage.AssignTrait(memberTrait);
			if (memberTrait.GetType().BaseType == typeof(Trait)) newPerkImage.transform.SetParent(traitGroup, false);
			else newPerkImage.transform.SetParent(skillGroup, false);
		}
	}

	void RefreshMercDeck()
	{
		ClearMercDeck();
		DisplayMercDeck();
	}
	void DisplayMercDeck()
	{
		foreach (CombatCard card in selectedMember.GetAllUsedCards())
		{
			CombatCardGraphic newGraphic = Instantiate(cardPrefab);
			newGraphic.AssignCard(card);
			newGraphic.transform.SetParent(deckGroup,false);
		}
	}
	void ClearMercDeck()
	{
		foreach (CombatCardGraphic graphic in deckGroup.GetComponentsInChildren<CombatCardGraphic>())
		{
			GameObject.Destroy(graphic.gameObject);
		}
	}
	/*
	public void InventoryItemClicked(InventoryItem clickedItem,InventorySlot originSlot)
	{
		if (clickedItem.UseAction(selectedMember)) 
		{
			//if item came from common inventory/encounter floor
			if (originSlot.GetType()==typeof(InventorySlot))
			{
				selectedMember.currentRegion.TakeStashItem(clickedItem);
				//{PartyManager.mainPartyManager.RemoveItems(clickedItem);}
				
			}
			//if item came from member inventory
			if (originSlot.GetType()==typeof(MemberInventorySlot))
			{
				//selectedMember.carriedItems.Remove(clickedItem);
				selectedMember.RemoveCarriedItem(clickedItem);
			}
			RefreshInventoryItems();
		} 
	}*/

	public void KickMemberPressed()
	{
		PartyMember removedMember=selectedMember;
		CloseScreen();
		PartyManager.mainPartyManager.RemovePartyMember(removedMember,false);
	}

	void Start() 
	{
		mainISHandler=this;
		//PartyManager.InventoryChanged+=InventoryChangeHandler;
		GameManager.GameOver+=GameEndHandler;
		selectedMember=null;
		closeButton.onClick.AddListener(()=>CloseScreen());
	}
	
	void OnDestroy() 
	{
		//PartyManager.InventoryChanged-=InventoryChangeHandler;
		GameManager.GameOver-=GameEndHandler;
	}
	
	void Update()
	{	
		/*
		if (Input.GetKeyDown(KeyCode.I))
		{
			if (GameManager.main.gameStarted && EncounterCanvasHandler.main.GetComponent<CanvasGroup>().interactable 
				&& PartyManager.mainPartyManager.partyMembers.Count>0)
			{
				//IF INSIDE AN ENCOUNTER DO NOTHING (The toggle will be handled from within EncounterCanvasHandler instead
				if (EncounterCanvasHandler.main.encounterOngoing)
				{
					//ToggleNewSelectedMember(EncounterCanvasHandler.main.selectedMember);
				}
				else 
				{
					//IF ON WORLD MAP
					if (PartyManager.mainPartyManager.selectedMembers.Count>0) ToggleSelectedMember(PartyManager.mainPartyManager.selectedMembers[0]);
					else ToggleSelectedMember(PartyManager.mainPartyManager.partyMembers[0]);
				}
			}
		}
		//DUPLICATES Q FUNCTIONALITY FROM ENCOUNTER SELECTION
		if (Input.GetKeyDown(KeyCode.Q) && inventoryShown)
		{
			if (GameManager.main.gameStarted && EncounterCanvasHandler.main.GetComponent<CanvasGroup>().interactable)
			{
				//IF INSIDE AN ENCOUNTER
				if (EncounterCanvasHandler.main.encounterOngoing)
				{
					//ToggleNewSelectedMember(EncounterCanvasHandler.main.selectedMember);
				}
				else 
				{
					{
						int selectedIndex=PartyManager.mainPartyManager.partyMembers.IndexOf(selectedMember);
						int  nextIndex=(int)Mathf.Repeat(selectedIndex+1,PartyManager.mainPartyManager.partyMembers.Count);
						if (selectedIndex!=nextIndex) ToggleSelectedMember(PartyManager.mainPartyManager.partyMembers[nextIndex]);
					}
				}
			}
		}
		 * */
	}
}
