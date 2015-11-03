using UnityEngine;
using System.Collections;

public class MemberInventorySlot :InventorySlot
{
	protected override void RegisterNewItem (SlotItem newSlotItem)
	{
		AssignItem(newSlotItem);
		InventoryScreenHandler.mainISHandler.selectedMember.carriedItems.Add(filledItem.assignedItem);
	}
	
	public override bool CheckItem (SlotItem newSlotItem)
	{
		bool assignItem=false;

		if (newSlotItem.currentSlot.GetType()!=this.GetType()) 
		{
			//The PickUpItem check adds the item to the party member on True by itself
			if (InventoryScreenHandler.mainISHandler.selectedMember.CanPickUpItem())
			{assignItem=true;}
		}
		return assignItem;
	}
	
	public override void EmptySlot ()
	{
		if (filledItem!=null) {InventoryScreenHandler.mainISHandler.selectedMember.carriedItems.Remove(filledItem.assignedItem);}
		filledItem=null;
	}
}
