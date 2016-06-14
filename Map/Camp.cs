using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Camp
{
/*
	public Camp()
	{
		PartyManager.ETimePassedEnd+=TimePassCold;
	}
	public void TimePassCold(int emptyInt)
	{
		//int temperatureChange=Mathf.Clamp(MapManager.GetDailyTemperatureRating(),0,1);
		//IncrementTemperature(-1);
		temperatureNumber=MapManager.mapTemperatureNumber;
		GameManager.DebugPrint("Temperature set to:"+MapManager.mapTemperatureNumber);
	}	*/

	public Pot cookingImplement=new Pot();
	public List<InventoryItem> beds=new List<InventoryItem>();
	public int freeBeds=0;

	//TEMPERATURE
	/*
	public static string GetTemperatureDescription(int temperatureNumber)
	{
		string description="";
		switch(temperatureNumber)
		{
			case 0:{description="Freezing"; break;}
			case 1:{description="Cold"; break;}
			case 2:{description="Okay"; break;}
		}
		return description;
	}
	public enum TemperatureRating {Freezing,Cold,Okay};
	public TemperatureRating GetTemperature()
	{
		//Cannot be <0 or >2
		TemperatureRating rating=TemperatureRating.Okay;
		switch(temperatureNumber)
		{
			case 0:{rating=TemperatureRating.Freezing; break;}
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
	*/
	//THREAT
	public MapRegion.ThreatLevels GetThreatLevel()
	{
		//Cannot be >2, okay for all results <0
		MapRegion.ThreatLevels level=MapRegion.ThreatLevels.None;
		switch(threatLevelNumber)
		{
			default:{level=MapRegion.ThreatLevels.None; break;}
			case 1:{level=MapRegion.ThreatLevels.Low; break;}
			case 2:{level=MapRegion.ThreatLevels.Medium; break;}
		}
		return level;
	}
	public int threatLevelNumber=2;

	public void EquipCookingImplement(Pot newImplement)
	{
		cookingImplement=newImplement;
	}
	
	public void UnequipCurrentCookingImplement()
	{
		cookingImplement=null;
	}
}

