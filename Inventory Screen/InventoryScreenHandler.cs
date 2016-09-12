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
	public EquipmentSlot equipmentSlotPrefab;
	public WeaponSlot weaponSlotPrefab;

	public SlotItem slotItemPrefab;
	public CombatCardGraphic combatCardPrefab;
	public PrepCardGraphic prepCardPrefab;
	
	
	//GROUPS
	//public Transform weaponsGroup;
	public Transform weaponsGroup;
	public Transform equipmentGroup;
	public Transform inventoryGroup;
	public Transform traitGroup;
	public Transform skillGroup;
	public Transform prepCardsGroup;
	public Transform combatDeckGroup;

	const int equipmentSlotCount = 3;
	const int weaponSlotCount = 3;
	
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
			if (!GameEventManager.mainEventManager.drawingEvent)
			{
				OpenScreen(newMember);
				if (PartyManager.mainPartyManager.partyMembers.Contains(newMember) && !CardsScreen.IsMissionOngoing())
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
		if (PartyManager.mainPartyManager.partyMembers.Contains(selectedMember) && !CardsScreen.IsMissionOngoing())
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
				equipmentGroup.parent.gameObject.SetActive(true);
				weaponsGroup.parent.gameObject.SetActive(true);
				inventoryGroup.parent.gameObject.SetActive(true);
				RefreshWeapons();
				RefreshEquipment();
				RefreshLocalInventory();
			}
			else
			{
				equipmentGroup.parent.gameObject.SetActive(false);
				weaponsGroup.parent.gameObject.SetActive(false);
				inventoryGroup.parent.gameObject.SetActive(false);
			}
			RefreshTraits();
			RefreshMercDeck();
			RefreshMercPrepCards();
		}
	}

	void RefreshLeftCornerText()
	{
		
		healthValueText.text=selectedMember.GetHealth().ToString();
		startingArmorValueText.text = selectedMember.GetStartArmor().ToString();
		startingStaminaValueText.text = selectedMember.GetMaxStamina().ToString();
		startingAmmoValueText.text = selectedMember.GetStartAmmo().ToString();
	}

	void RefreshWeapons()
	{
		//Refresh current weapons
		foreach (Image oldItemSlot in weaponsGroup.GetComponentsInChildren<Image>())//.GetComponentsInChildren<Button>())
		{
			GameObject.Destroy(oldItemSlot.gameObject);
		}

		for (int i = 0; i < weaponSlotCount; i++)
		{
			WeaponSlot newSlot = Instantiate(weaponSlotPrefab);
			newSlot.transform.SetParent(weaponsGroup, false);

			List<IEquipmentItem> membersWeapons=selectedMember.GetEquippedWeapons();
			if (i < membersWeapons.Count)
			{
				SlotItem newItem = Instantiate(slotItemPrefab);
				InventoryItem assignedItem = membersWeapons[i] as Weapon;
				newItem.AssignItem(assignedItem);
				newSlot.AssignItem(newItem);
				//!!NO ONCLICK LISTENER ASSIGNED!!
			}
		}
	}

	void RefreshEquipment()
	{
		//Refresh current equipment
		foreach (Image oldItemSlot in equipmentGroup.GetComponentsInChildren<Image>())//.GetComponentsInChildren<Button>())
		{
			GameObject.Destroy(oldItemSlot.gameObject);
		}

		for (int i = 0; i < equipmentSlotCount; i++)
		{
			EquipmentSlot newSlot = Instantiate(equipmentSlotPrefab);
			newSlot.transform.SetParent(equipmentGroup, false);

			List<IEquipmentItem> membersEquipment = selectedMember.GetEquippedItems();
			if (i < membersEquipment.Count)
			{
				SlotItem newItem = Instantiate(slotItemPrefab);
				InventoryItem assignedItem = membersEquipment[i] as InventoryItem;
				newItem.AssignItem(assignedItem);
				newSlot.AssignItem(newItem);
				//!!NO ONCLICK LISTENER ASSIGNED!!
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

		activeList = selectedMember.currentRegion.GetStashedItems();
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

	void RefreshMercPrepCards()
	{
		ClearMercPrepCards();
		DisplayMercPrepCards();
	}
	void DisplayMercPrepCards()
	{
		foreach (PrepCard card in selectedMember.GetClassPrepCards())
		{
			PrepCardGraphic newGraphic = Instantiate(prepCardPrefab);
			newGraphic.AssignCard(card,true);
			newGraphic.transform.SetParent(prepCardsGroup, false);
		}
	}
	void ClearMercPrepCards()
	{
		foreach (PrepCardGraphic graphic in prepCardsGroup.GetComponentsInChildren<PrepCardGraphic>())
		{
			GameObject.Destroy(graphic.gameObject);
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
			CombatCardGraphic newGraphic = Instantiate(combatCardPrefab);
			newGraphic.AssignCard(card,true);
			newGraphic.transform.SetParent(combatDeckGroup,false);
		}
	}
	void ClearMercDeck()
	{
		foreach (CombatCardGraphic graphic in combatDeckGroup.GetComponentsInChildren<CombatCardGraphic>())
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
