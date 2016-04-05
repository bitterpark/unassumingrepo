using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Camp
{
	public Camp()
	{
		PartyManager.ETimePassedEnd+=TimePassCold;
	}
	public void TimePassCold(int emptyInt)
	{
		IncrementTemperature(-1);
	}	

	public Pot cookingImplement=new Pot();
	public List<InventoryItem> beds=new List<InventoryItem>();
	public int freeBeds=0;
	public enum TemperatureRating {Very_Cold,Cold,Okay};
	public TemperatureRating GetTemperature()
	{
		//Cannot be <0 or >2
		TemperatureRating rating=TemperatureRating.Okay;
		switch(temperatureNumber)
		{
			case 0:{rating=TemperatureRating.Very_Cold; break;}
			case 1:{rating=TemperatureRating.Cold; break;}
			case 2:{rating=TemperatureRating.Okay; break;}
		}
		return rating;
	}
	public void IncrementTemperature(int delta)
	{
		temperatureNumber+=delta;
		temperatureNumber=Mathf.Clamp(temperatureNumber,0,2);//
	}
	int temperatureNumber;

	public MapRegion.ThreatLevels GetThreatLevel()
	{
		//Cannot be >3, okay for all results <0
		MapRegion.ThreatLevels level=MapRegion.ThreatLevels.None;
		switch(threatLevelNumber)
		{
			default:{level=MapRegion.ThreatLevels.None; break;}
			case 1:{level=MapRegion.ThreatLevels.Low; break;}
			case 2:{level=MapRegion.ThreatLevels.Medium; break;}
			case 3:{level=MapRegion.ThreatLevels.High; break;}
		}
		return level;
	}
	public int threatLevelNumber=3;

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

