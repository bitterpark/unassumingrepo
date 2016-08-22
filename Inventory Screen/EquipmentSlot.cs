using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class EquipmentSlot :InventorySlot

{
	
	protected override void RegisterNewItem (SlotItem newSlotItem)
	{
		AssignItem(newSlotItem);
		InventoryScreenHandler.mainISHandler.selectedMember.EquipItem(newSlotItem.assignedItem as EquippableItem);
	}

	public override bool CheckItem (SlotItem newSlotItem)
	{
		bool allowItem=false;
		if (newSlotItem.currentSlot.GetType()!=this.GetType()) 
		{
			//check if type is appropriate
			if (newSlotItem.assignedItem.GetType().BaseType==typeof(EquippableItem))
				allowItem=true;
		}	
		return allowItem;
	}
	
	public override void EmptySlot ()
	{
		if (filledItem!=null) InventoryScreenHandler.mainISHandler.selectedMember.UnequipItem(filledItem.assignedItem as EquippableItem);
		filledItem=null;
	}
	
}

