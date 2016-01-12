using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Camp
{
	public Pot cookingImplement=new Pot();
	public List<InventoryItem> beds=new List<InventoryItem>();
	public int freeBeds=0;

	public void EquipCookingImplement(Pot newImplement)
	{
		cookingImplement=newImplement;
	}
	
	public void UnequipCurrentCookingImplement()
	{
		cookingImplement=null;
	}
	
	public void AddBed(Bed newBed)
	{
		beds.Add(newBed);
		freeBeds+=1;
		//Consider moving to bed slot instead
		if (InventoryScreenHandler.mainISHandler.campingCanvas.campShown) InventoryScreenHandler.mainISHandler.campingCanvas.RefreshSlots();
	}
	public void RemoveBed(Bed removedBed)
	{
		beds.Remove(removedBed);
		freeBeds-=1;
		//Consider moving to bed slot instead
		if (InventoryScreenHandler.mainISHandler.campingCanvas.campShown) InventoryScreenHandler.mainISHandler.campingCanvas.RefreshSlots();
	}
}

