using UnityEngine;
using System.Collections;

public class RangedSlot :InventorySlot
{
	protected override void RegisterNewItem (SlotItem newSlotItem)
	{
		AssignItem(newSlotItem);
		InventoryScreenHandler.mainISHandler.selectedMember.EquipWeapon(newSlotItem.assignedItem as Weapon);
	}
	
	public override bool CheckItem (SlotItem newSlotItem)
	{
		bool allowItem=false;
		if (newSlotItem.currentSlot.GetType()!=this.GetType()) 
		{
			//check if type is appropriate
			if (newSlotItem.assignedItem.GetType().BaseType==typeof(RangedWeapon))
			{
				//check if party member can equip (prevents equpping multiple flashlights or armor vests
				//InventoryScreenHandler.mainISHandler.selectedMember.EquipWeapon(newSlotItem.assignedItem as Weapon);
				allowItem=true;
			}
		}	
		return allowItem;
	}
	
	public override void EmptySlot ()
	{
		if (filledItem!=null) InventoryScreenHandler.mainISHandler.selectedMember.UnequipWeapon(filledItem.assignedItem as Weapon);
		filledItem=null;
	}
	
}
