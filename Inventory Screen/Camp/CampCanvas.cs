using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CampCanvas : MonoBehaviour {

	public bool campShown=false;
	public SlotItem slotItemPrefab;
	
	public CookingSlot cookingSlotPrefab;
	public Transform cookingSlotPlacement;
	
	public BedSlot bedSlotPrefab;
	public Transform bedSlotGroup;
	public Text freeBedsCount;
	
	public Camp assignedCamp;
	
	public void AssignCamp(Camp newCamp, bool regionHasCamp)
	{
		if (regionHasCamp)
		{
			campShown=true;
			assignedCamp=newCamp;
			GetComponent<Canvas>().enabled=true;
			RefreshSlots();
		}
		else CloseScreen();
	}
	
	public void RefreshSlots()
	{
		
		//Remove old cooking slot
		Image oldCookingSlot=cookingSlotPlacement.GetComponentInChildren<Image>();
		if (oldCookingSlot!=null) GameObject.Destroy(oldCookingSlot.gameObject);
		
		//Remove old bed slots
		foreach (Image oldBedSlot in bedSlotGroup.GetComponentsInChildren<Image>())//.GetComponentsInChildren<Button>())
		{
			GameObject.Destroy(oldBedSlot.gameObject);
		}
		if (assignedCamp!=null)
		{
			//Refresh cooking slot
			CookingSlot newCookingSlot=Instantiate(cookingSlotPrefab);
			newCookingSlot.transform.SetParent(cookingSlotPlacement,false);
			if (assignedCamp.cookingImplement!=null)
			{
				SlotItem cookingImplement=Instantiate(slotItemPrefab);
				cookingImplement.AssignItem(assignedCamp.cookingImplement);
				newCookingSlot.AssignItem(cookingImplement);
				//!!NO ONCLICK LISTENER ASSIGNED!!
			}
			
			//Refresh bed slots
			int bedSlotCount=assignedCamp.beds.Count+1;
			int occupiedBedsCount=assignedCamp.beds.Count-assignedCamp.freeBeds;
			for (int i=0; i<bedSlotCount; i++)
			{
				BedSlot newBedSlot=Instantiate(bedSlotPrefab);
				newBedSlot.transform.SetParent(bedSlotGroup,false);
				if (i<assignedCamp.beds.Count)
				{
					SlotItem newBed=Instantiate(slotItemPrefab);
					newBed.AssignItem(assignedCamp.beds[i]);
					newBedSlot.AssignItem(newBed);
					if (occupiedBedsCount>0)
					{
						newBed.draggable=false;
						occupiedBedsCount-=1;
					}
					//!!NO ONCLICK LISTENER ASSIGNED!!
				}
			}
			freeBedsCount.text="Free beds:"+assignedCamp.freeBeds;
			
		} else {print ("Assigned camp is null");}
	}
	
	public void CloseScreen()
	{
		campShown=false;
		assignedCamp=null;
		GetComponent<Canvas>().enabled=false;
	}
}
