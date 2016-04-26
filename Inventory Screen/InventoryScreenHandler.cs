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
	public Text meleeDamageText;
	public Text rangedDamageText;
	public Text armorValueText;

	public Button kickMemberButton;
	
	public Text skillpointCounterText;
	
	public CampCanvas campingCanvas;
	public static InventoryScreenHandler mainISHandler;
	public bool inventoryShown=false;

	public Text campThreatText;
	public Text campTemperatureText;

	//PREFABS
	public InventorySlot inventorySlotPrefab;
	public MemberInventorySlot memberSlotPrefab;
	public EquipmentSlot equipmentSlotPrefab;
	public MeleeSlot meleeSlotPrefab;
	public RangedSlot rangedSlotPrefab;
	public SlotItem slotItemPrefab;
	
	//GROUPS
	//public Transform weaponsGroup;
	public Transform meleeSlotPlacement;
	public Transform rangedSlotPlacement;
	public Transform equipmentGroup;
	public Transform inventoryGroup;
	public Transform memberInventoryGroup;
	public Transform traitGroup;
	public Transform skillGroup;
	public Transform relationsGroup;
	
	/*
	public void InventoryChangeHandler() 
	{
		if (GetComponent<Canvas>().enabled==true) {RefreshInventoryItems();}
	}*/
	
	public void AssignSelectedMember(PartyMember newMember)
	{
		if (selectedMember!=newMember)
		{
			//Determine whether or not inventory screen is allowed to be opened at this time
			bool allow=false;
			EncounterCanvasHandler encounterManager=EncounterCanvasHandler.main;
			if (!GameEventManager.mainEventManager.drawingEvent)
			{
				if (!encounterManager.encounterOngoing) 
				{
					allow=true;
					if (PartyManager.mainPartyManager.partyMembers.Count>1) kickMemberButton.gameObject.SetActive(true);
					else kickMemberButton.gameObject.SetActive(false);
				}
				else 
				{
					kickMemberButton.gameObject.SetActive(false);
					//make sure guys outside of encounter can't interact with guys within an encounter and members currently in combat can't open inv
					if(encounterManager.encounterMembers.Contains(newMember) 
					 && !encounterManager.currentEncounter.encounterMap[encounterManager.memberCoords[newMember]].hasEnemies)
					{allow=true;}
				}
			}
			//If inventory is allowed to be opened
			if (allow)
			{
				selectedMember=newMember;
				memberPortrait.color=newMember.color;
				GetComponent<Canvas>().enabled=true;
				inventoryShown=true;
				memberNameText.text=newMember.name;		
				/*
				if (!EncounterCanvasHandler.main.encounterOngoing)
				{
					campingCanvas.AssignCampRegion(selectedMember.currentRegion);
				}*/
				RefreshInventoryItems();
			}
		} 
		else CloseScreen();
	}
	
	public void GameEndHandler()
	{
		CloseScreen();
	}
	
	public void CloseScreen()
	{
		selectedMember=null;
		campingCanvas.CloseScreen();
		GetComponent<Canvas>().enabled=false;
		inventoryShown=false;
	}
	
	public void RefreshInventoryItems()
	{
		//This check is done to make sure inventory screen won't stay open for members who bled out during a timeskip with inventory open
		if (selectedMember!=null) 
		{
			if (!PartyManager.mainPartyManager.partyMembers.Contains(selectedMember))
			{
				CloseScreen();
			}
		}

		if (inventoryShown)
		{
			
			//Update armor value
			armorValueText.text="Armor:"+selectedMember.armorValue;
			
			//Update weapon damage
			string meleeText="Melee:";
			meleeText+=selectedMember.GetMeleeDamageString();
			meleeDamageText.text=meleeText;
			
			string rangedText="Ranged:";
			rangedText+=selectedMember.GetRangedDamage();
			rangedDamageText.text=rangedText;

			//Update camp info
			//Temperature
			campTemperatureText.text="Temperature: "+MapRegion.GetTemperatureDescription(selectedMember.currentRegion.localTemperature);
			//Threat
			campThreatText.text="Camp security:"+selectedMember.currentRegion.GetCampSecurityDescription();

			//Remove old weapons
			Image oldMeleeSlot=meleeSlotPlacement.GetComponentInChildren<Image>();
			Image oldRangedSlot=rangedSlotPlacement.GetComponentInChildren<Image>();
			if (oldMeleeSlot!=null) GameObject.Destroy(oldMeleeSlot.gameObject);
			if (oldRangedSlot!=null) GameObject.Destroy(oldRangedSlot.gameObject);
		
			//Refresh melee slot
			MeleeSlot newMeleeSlot=Instantiate(meleeSlotPrefab);
			newMeleeSlot.transform.SetParent(meleeSlotPlacement,false);
			if (selectedMember.equippedMeleeWeapon!=null)
			{
				SlotItem meleeWeapon=Instantiate(slotItemPrefab);
				meleeWeapon.AssignItem(selectedMember.equippedMeleeWeapon);
				newMeleeSlot.AssignItem(meleeWeapon);
				//!!NO ONCLICK LISTENER ASSIGNED!!
			}
			
			//Refresh ranged slot
			RangedSlot newRangedSlot=Instantiate(rangedSlotPrefab);
			newRangedSlot.transform.SetParent(rangedSlotPlacement,false);
			if (selectedMember.equippedRangedWeapon!=null)
			{
				SlotItem rangedWeapon=Instantiate(slotItemPrefab);
				rangedWeapon.AssignItem(selectedMember.equippedRangedWeapon);
				newRangedSlot.AssignItem(rangedWeapon);
				//!!NO ONCLICK LISTENER ASSIGNED!!
			}
			
			//Refresh current equipment
			foreach (Image oldItemSlot in equipmentGroup.GetComponentsInChildren<Image>())//.GetComponentsInChildren<Button>())
			{
				GameObject.Destroy(oldItemSlot.gameObject);
			}	
			int equipmentSlotCount=4;
			for (int i=0; i<equipmentSlotCount; i++)
			{
				EquipmentSlot newSlot=Instantiate(equipmentSlotPrefab);
				newSlot.transform.SetParent(equipmentGroup,false);
				if (i<selectedMember.equippedItems.Count)
				{
					SlotItem newItem=Instantiate(slotItemPrefab);
					newItem.AssignItem(selectedMember.equippedItems[i]);
					newSlot.AssignItem(newItem);
					//!!NO ONCLICK LISTENER ASSIGNED!!
				}
			}
			
			//REFRESH MEMBER INVENTORY
			foreach (Image oldItemSlot in memberInventoryGroup.GetComponentsInChildren<Image>())//Button>())
			{
				GameObject.Destroy(oldItemSlot.gameObject);
			}
			int memberSlotCount=selectedMember.currentCarryCapacity;
			for (int i=0; i<memberSlotCount; i++)
			{
				InventorySlot newSlot=Instantiate(memberSlotPrefab);
				newSlot.transform.SetParent(memberInventoryGroup,false);
				if (i<selectedMember.carriedItems.Count)
				{
					SlotItem newItem=Instantiate(slotItemPrefab);
					newItem.AssignItem(selectedMember.carriedItems[i]);
					newSlot.AssignItem(newItem);
					newItem.GetComponent<Button>().onClick.AddListener(()=>InventoryItemClicked(newItem.assignedItem,newItem.currentSlot));
				}
			}
			
			//REFRESH INVENTORY
			foreach (Image oldItemSlot in inventoryGroup.GetComponentsInChildren<Image>())//Button>())
			{
				GameObject.Destroy(oldItemSlot.gameObject);
			}
			int slotCount=48;
			List<InventoryItem> activeList;
			//select the right list for in-encounter and out-of-encounter
			if (EncounterCanvasHandler.main.encounterOngoing) 
			{
				EncounterCanvasHandler encounterManager=EncounterCanvasHandler.main;
				activeList=encounterManager.currentEncounter.encounterMap[encounterManager.memberCoords[selectedMember]].floorItems;
			}
			else {activeList=selectedMember.currentRegion.GetStashedItems();}//PartyManager.mainPartyManager.GetPartyInventory();}
			//Use floor or inventory list
			for (int i=0; i<slotCount; i++)
			{
				InventorySlot newSlot=Instantiate(inventorySlotPrefab);
				newSlot.transform.SetParent(inventoryGroup,false);
				if (i<activeList.Count)
				{
					SlotItem newItem=Instantiate(slotItemPrefab);
					newItem.AssignItem(activeList[i]);
					newSlot.AssignItem(newItem);
					newItem.GetComponent<Button>().onClick.AddListener(()=>InventoryItemClicked(newItem.assignedItem,newItem.currentSlot));
				}
			}
			
			//Refresh traits
			//Refresh skillpoint counter
			skillpointCounterText.text="Skillpoints:"+selectedMember.skillpoints;
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
				TraitUIHandler newPerkImage=Instantiate(traitPrefab);
				newPerkImage.AssignTrait(memberTrait);
				if (memberTrait.GetType().BaseType==typeof(Trait)) newPerkImage.transform.SetParent(traitGroup,false);
				else newPerkImage.transform.SetParent(skillGroup,false);
			}
			
			//Refresh relations
			foreach (Image oldRelImage in relationsGroup.GetComponentsInChildren<Image>())
			{
				GameObject.Destroy(oldRelImage.gameObject);
			}	
			foreach (Relationship memberRelation in selectedMember.relationships.Values)
			{
				RelationTextHandler newRelImage=Instantiate(relationPrefab);
				newRelImage.AssignRelation(memberRelation);
				newRelImage.transform.SetParent(relationsGroup,false);
			}
			if (!EncounterCanvasHandler.main.encounterOngoing) campingCanvas.RefreshSlots();
			else campingCanvas.CloseScreen();
		}
	}
	
	public void InventoryItemClicked(InventoryItem clickedItem,InventorySlot originSlot)
	{
		if (clickedItem.UseAction(selectedMember)) 
		{
			//if item came from common inventory/encounter floor
			if (originSlot.GetType()==typeof(InventorySlot))
			{
				EncounterCanvasHandler encounterHandler=EncounterCanvasHandler.main;
				if (encounterHandler.encounterOngoing) 
				{
					//EncounterCanvasHandler.main.displayedRoom.RemoveFloorItem(clickedItem);
					encounterHandler.roomButtons[encounterHandler.memberCoords[selectedMember]].PickUpFloorItem(clickedItem);
				}
				else selectedMember.currentRegion.TakeStashItem(clickedItem);
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
	}

	public void KickMemberPressed()
	{
		PartyMember removedMember=selectedMember;
		CloseScreen();
		PartyManager.mainPartyManager.RemovePartyMember(removedMember);
	}

	void Start() 
	{
		mainISHandler=this;
		//PartyManager.InventoryChanged+=InventoryChangeHandler;
		GameManager.GameOver+=GameEndHandler;
		selectedMember=null;
	}
	
	void OnDestroy() 
	{
		//PartyManager.InventoryChanged-=InventoryChangeHandler;
		GameManager.GameOver-=GameEndHandler;
	}
	
	void Update()
	{	
		if (Input.GetKeyDown(KeyCode.I))
		{
			if (GameManager.main.gameStarted && EncounterCanvasHandler.main.GetComponent<CanvasGroup>().interactable)
			{
				//IF INSIDE AN ENCOUNTER
				if (EncounterCanvasHandler.main.encounterOngoing)
				{
					AssignSelectedMember(EncounterCanvasHandler.main.selectedMember);
				}
				else 
				{
					//IF ON WORLD MAP
					if (PartyManager.mainPartyManager.selectedMembers.Count>0) AssignSelectedMember(PartyManager.mainPartyManager.selectedMembers[0]);
					else AssignSelectedMember(PartyManager.mainPartyManager.partyMembers[0]);
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
					AssignSelectedMember(EncounterCanvasHandler.main.selectedMember);
				}
				else 
				{
					//IF ON WORLD MAP
					/*
					if (PartyManager.mainPartyManager.selectedMembers.Count>0) 
					{
						int selectedIndex=PartyManager.mainPartyManager.selectedMembers.IndexOf(selectedMember);
						int nextIndex=(int)Mathf.Repeat(selectedIndex+1,PartyManager.mainPartyManager.selectedMembers.Count);
						if (selectedIndex!=nextIndex) AssignSelectedMember(PartyManager.mainPartyManager.selectedMembers[nextIndex]);
					}

					else*/
					{
						int selectedIndex=PartyManager.mainPartyManager.partyMembers.IndexOf(selectedMember);
						int  nextIndex=(int)Mathf.Repeat(selectedIndex+1,PartyManager.mainPartyManager.partyMembers.Count);
						if (selectedIndex!=nextIndex) AssignSelectedMember(PartyManager.mainPartyManager.partyMembers[nextIndex]);
					}
				}
			}
		}
		/*
		if (Input.GetKeyDown(KeyCode.T))
		{
			int fucktest=0;
			fucktest=(int)Mathf.Repeat(fucktest+1,1);
			print ("Final fucktest="+fucktest.ToString());
		}*/
	}
}
