using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryScreenHandler : MonoBehaviour 
{
	public PartyMember selectedMember;
	//public ItemImageHandler itemPrefab;
	public PerkTextHandler perkPrefab;
	public RelationTextHandler relationPrefab;
	
	public Text memberNameText;
	public Text meleeDamageText;
	public Text rangedDamageText;
	public Text armorValueText;
	public CampCanvas campingCanvas;
	public static InventoryScreenHandler mainISHandler;
	public bool inventoryShown=false;
	
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
	
	
	/*
	public void InventoryChangeHandler() 
	{
		if (GetComponent<Canvas>().enabled==true) {RefreshInventoryItems();}
	}*/
	
	public void AssignSelectedMember(PartyMember newMember)
	{
		if (selectedMember!=newMember)
		{
			bool allow=false;
			EncounterCanvasHandler encounterManager=EncounterCanvasHandler.main;
			if (!encounterManager.encounterOngoing) {allow=true;}
			else 
			{
				//make sure guys outside of encounter can't interact with guys within an encounter and members currently in combat can't open inv
				if(encounterManager.encounterMembers.Contains(newMember) 
				 && !encounterManager.currentEncounter.encounterMap[encounterManager.memberCoords[newMember]].hasEnemies)
				{allow=true;}
			}
			if (allow)
			{
				selectedMember=newMember;
				GetComponent<Canvas>().enabled=true;
				inventoryShown=true;
				memberNameText.text=newMember.name;		
			
				if (!EncounterCanvasHandler.main.encounterOngoing)
				{
					campingCanvas.AssignCamp(selectedMember.currentRegion.campInRegion
					,selectedMember.currentRegion.hasCamp);
				}
				RefreshInventoryItems();
			}
		} 
		else 
		{
			CloseScreen();
		}
	}
	
	public void GameEndHandler()
	{
		CloseScreen();
	}
	
	void CloseScreen()
	{
		selectedMember=null;
		campingCanvas.CloseScreen();
		GetComponent<Canvas>().enabled=false;
		inventoryShown=false;
	}
	
	public void RefreshInventoryItems()
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
		
		//Refresh perks
		foreach (Image oldPerkImage in transform.FindChild("Inventory Panel").FindChild("Perks Group").GetComponentsInChildren<Image>())
		{
			GameObject.Destroy(oldPerkImage.gameObject);
		}	
		foreach (Perk memberPerk in selectedMember.perks)
		{
			PerkTextHandler newPerkImage=Instantiate(perkPrefab);
			newPerkImage.AssignPerk(memberPerk);
			newPerkImage.transform.SetParent(transform.FindChild("Inventory Panel").FindChild("Perks Group"),false);
		}
		
		//Refresh relations
		foreach (Image oldRelImage in transform.FindChild("Inventory Panel").FindChild("Relations Group").GetComponentsInChildren<Image>())
		{
			GameObject.Destroy(oldRelImage.gameObject);
		}	
		foreach (Relationship memberRelation in selectedMember.relationships.Values)
		{
			RelationTextHandler newRelImage=Instantiate(relationPrefab);
			newRelImage.AssignRelation(memberRelation);
			newRelImage.transform.SetParent(transform.FindChild("Inventory Panel").FindChild("Relations Group"),false);
		}
		
		//Refresh camp
		campingCanvas.RefreshSlots();
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
				if (EncounterCanvasHandler.main.encounterOngoing)
				{
					AssignSelectedMember(EncounterCanvasHandler.main.selectedMember);
				}
				else {AssignSelectedMember(PartyManager.mainPartyManager.partyMembers[0]);}
			}
		}
	}
}
