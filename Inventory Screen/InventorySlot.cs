using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
	public delegate void ItemDroppedDeleg();
	public static event ItemDroppedDeleg EItemDropped;
	public delegate void ItemUsedDeleg();
	public static event ItemUsedDeleg EItemUsed;
	
	protected SlotItem filledItem;
		
	//unconditional adding of items, for start init (visual assignment only, without duplicating items over into partyInventory)
	public void AssignItem(SlotItem newSlotItem)
	{
		filledItem=newSlotItem;
		newSlotItem.SetInSlot(this);
	}
	
	protected virtual void RegisterNewItem(SlotItem newSlotItem)
	{
		AssignItem(newSlotItem);
		InventoryScreenHandler.mainISHandler.selectedMember.currentRegion.StashItem(newSlotItem.assignedItem);
	}
	
	//see if the new item can currently be filled into this slot, if yes - return true
	public virtual bool CheckItem(SlotItem newSlotItem)
	{
		bool assignItem=false;
		//Check if item has no slot from which it came, or the slot is of a different type from Inventory
		if (newSlotItem.currentSlot==null) {assignItem=true;}
		else {if (newSlotItem.currentSlot.GetType()!=this.GetType()) {assignItem=true;}}
		/*
		if (assignItem) 
		{
			
			//this makes common inventory slots act as both common party inventory outside encounters and encounter floor within
			if (EncounterCanvasHandler.main.encounterOngoing)
			{
				EncounterCanvasHandler encounterManager=EncounterCanvasHandler.main;
				EncounterRoom affectedRoom=EncounterCanvasHandler.main.currentEncounter
				.encounterMap[EncounterCanvasHandler.main.memberCoords[InventoryScreenHandler.mainISHandler.selectedMember]];
				affectedRoom.AddFloorItem(newSlotItem.assignedItem);
				//.encounterMap[encounterManager.memberCoords[InventoryScreenHandler.mainISHandler.selectedMember]].AddFloorItem(newSlotItem.assignedItem);//floorItems.Add(newSlotItem.assignedItem);
			}//.encounterFloor.Add(newSlotItem.assignedItem);} 
			else {PartyManager.mainPartyManager.partyInventory.Add(newSlotItem.assignedItem);}
		}*/
		return assignItem;
	}

	//for items being dropped in with the mouse (public to allow overlaid SlotItems to pass it on if they block the raycast
	public void ItemDroppedIn(SlotItem newSlotItem)
	{
		//check if item applies to this slot
		if (CheckItem(newSlotItem)) 
		{
			SlotItem oldItem=filledItem;
			InventorySlot oldSlot=newSlotItem.currentSlot;
			oldSlot.EmptySlot();
			//remove current item and fill in new item
			EmptySlot();
			RegisterNewItem(newSlotItem);
			//If slot previously had an item in it - swap to other slot
			if (oldItem!=null) {oldSlot.ItemSwapped(oldItem);}
			//Update armor/damage/other stats on inventory screen
			//InventoryScreenHandler.mainISHandler.RefreshInventoryItems();
			if (EItemDropped != null) 
				EItemDropped();
		}
	}
	
	//for when this slot's item gets put into another slot, the other slot's item is swapped to this slot
	protected void ItemSwapped(SlotItem swapItem)
	{
		if (CheckItem(swapItem)) {RegisterNewItem(swapItem);}
		else 
		{
			foreach (InventorySlot slot in InventoryScreenHandler.mainISHandler.inventoryGroup.GetComponentsInChildren<InventorySlot>())
			{
				if (slot.filledItem==null && slot!=this) 
				{
					slot.RegisterNewItem(swapItem); 
					break;
				}
			}
		}
		//InventoryScreenHandler.mainISHandler.RefreshInventoryItems();
		
	}

	public virtual void LclickAction()
	{
		if (GetType() == typeof(InventorySlot))
		{
			if (filledItem.assignedItem.UseAction())
			{
				EmptySlot();
				if (EItemUsed != null)
					EItemUsed();
			}
		}
	}

	//Only works for inheriting classes, does nothing for this class
	public virtual void RclickAction()
	{
		//Drop item from current slot to local inventory
		if (GetType()!=typeof(InventorySlot))
		{
			foreach (InventorySlot slot in InventoryScreenHandler.mainISHandler.inventoryGroup.GetComponentsInChildren<InventorySlot>())
			{
				if (slot.filledItem==null && slot!=this) 
				{
					slot.ItemDroppedIn(filledItem);
					break;
				}
			} 
		}
		else
		{
			if (InventoryScreenHandler.mainISHandler.selectedMember.CanPickUpItem())
			{
				foreach (MemberInventorySlot slot in InventoryScreenHandler.mainISHandler.memberInventoryGroup.GetComponentsInChildren<MemberInventorySlot>())
				{
					if (slot.filledItem==null) 
					{
						slot.ItemDroppedIn(filledItem);
						break;
					}
				}
			}
		}
	}

	//unconditional emptying out
	public virtual void EmptySlot()
	{
		if (filledItem!=null)
		{
			MapManager.main.GetTown().TakeStashItem(filledItem.assignedItem);
			//{PartyManager.mainPartyManager.RemoveItems(filledItem.assignedItem);}
		}
		filledItem=null;
	}
	
	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		//SlotItem.itemBeingDragged.currentSlot.EmptySlot;
		//print ("Drop fired");
		if (SlotItem.itemBeingDragged!=null) ItemDroppedIn(SlotItem.itemBeingDragged);
	}
	#endregion
}
