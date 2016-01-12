using UnityEngine;
using System.Collections;

public class CookingSlot:InventorySlot 
{
	protected override void RegisterNewItem (SlotItem newSlotItem)
	{
		AssignItem(newSlotItem);
		InventoryScreenHandler.mainISHandler.campingCanvas.assignedCamp.EquipCookingImplement(newSlotItem.assignedItem as Pot);//.selectedMember.EquipWeapon(newSlotItem.assignedItem as Weapon);
		print("Cooking equipped");
	}
	
	public override bool CheckItem (SlotItem newSlotItem)
	{
		bool allowItem=false;
		print("Begin approving cooking");
		if (newSlotItem.currentSlot.GetType()!=this.GetType()) 
		{
			//check if type is appropriate
			if (newSlotItem.assignedItem.GetType()==typeof(Pot))
			{
				//check if party member can equip (prevents equpping multiple flashlights or armor vests
				allowItem=true;
				print("Cooking approved");
			}
		}
		
		return allowItem;
	}
	
	public override void EmptySlot ()
	{
		if (filledItem!=null) InventoryScreenHandler.mainISHandler.campingCanvas.assignedCamp.UnequipCurrentCookingImplement();
		filledItem=null;
	}
}
