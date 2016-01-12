using UnityEngine;
using System.Collections;

public class BedSlot:InventorySlot 
{
	protected override void RegisterNewItem (SlotItem newSlotItem)
	{
		AssignItem(newSlotItem);
		InventoryScreenHandler.mainISHandler.campingCanvas.assignedCamp.AddBed(newSlotItem.assignedItem as Bed);
		//Required to update displayed rest type
		PartyStatusCanvasHandler.main.RefreshAssignmentButtons();
	}
	
	public override bool CheckItem (SlotItem newSlotItem)
	{
		bool allowItem=false;
		if (newSlotItem.currentSlot.GetType()!=this.GetType()) 
		{
			//check if type is appropriate
			if (newSlotItem.assignedItem.GetType()==typeof(Bed))
			{
				//check if party member can equip (prevents equpping multiple flashlights or armor vests
				allowItem=true;
			}
		}
		return allowItem;
	}
	
	public override void EmptySlot ()
	{
		if (filledItem!=null) InventoryScreenHandler.mainISHandler.campingCanvas.assignedCamp.RemoveBed(filledItem.assignedItem as Bed);
		filledItem=null;
		//Required to update displayed rest type
		PartyStatusCanvasHandler.main.RefreshAssignmentButtons();
	}
}
