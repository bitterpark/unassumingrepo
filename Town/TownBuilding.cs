using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IBuildable
{
	bool IsBuilt();
	int GetBuildCost();
	string GetName();
	string GetDescription();
	Sprite GetIcon();
	Dictionary<InventoryItem.LootItems, int> GetMaterials();

	void BuildingFinished();
}

public abstract class BuildableObject: IBuildable
{
	protected Sprite icon;

	protected bool built=false;

	protected int buildMoneyCost;
	
	protected string name;
	protected string description;
	protected Dictionary<InventoryItem.LootItems, int> requiredMaterials=new Dictionary<InventoryItem.LootItems,int>();


	public virtual void BuildingFinished()
	{
		built = true;
	}
	public virtual void EnableActiveEffect() { }
	public virtual void DisableActiveEffect() { }
	
	public bool IsBuilt()
	{
		return built;
	}

	public Sprite GetIcon()
	{
		return icon;
	}

	public int GetBuildCost()
	{
		return buildMoneyCost;
	}

	public string GetName()
	{
		return name;
	}

	public string GetDescription()
	{
		return description;
	}

	public Dictionary<InventoryItem.LootItems, int> GetMaterials()
	{
		return requiredMaterials;
	}
}

public abstract class TownBuilding : BuildableObject
{
	protected int crewRequirement = 0;

	protected static int currentCrewReq = 10;
	protected const int crewReqIncreaseWithEachBuild = 10;

	public List<BuildingUpgrade> upgrades = new List<BuildingUpgrade>();

	public override void BuildingFinished()
	{
		base.BuildingFinished();
		currentCrewReq += crewReqIncreaseWithEachBuild;
		if (BuildingIsActive()) 
			EnableActiveEffect();
	}

	public override void EnableActiveEffect()
	{
		foreach (BuildingUpgrade upgrade in upgrades)
		{
			upgrade.EnableActiveEffect();
		}
	}

	public override void DisableActiveEffect()
	{
		foreach (BuildingUpgrade upgrade in upgrades)
		{
			upgrade.DisableActiveEffect();
		}
	}

	public void UpdateActiveStatus()
	{
		if (built)
		{
			if (BuildingIsActive()) EnableActiveEffect();
			else DisableActiveEffect();
		}
	}

	public bool BuildingIsActive()
	{
		if (built && TownManager.main.GetCrew() >= crewRequirement) return true;
		else return false;
	}

	public int GetCrewRequirement()
	{
		if (!built) crewRequirement = currentCrewReq;
		return crewRequirement;
	}
}



public class Bar : TownBuilding
{
	public Bar()
	{
		name = "Bar";
		icon = SpriteBase.mainSpriteBase.barIcon;
		requiredMaterials.Add(InventoryItem.LootItems.Scrap,3);
		buildMoneyCost = 500;

		description="Serves drinks";

		upgrades.Add(new Padlocks(this));
	}

	public override void BuildingFinished()
	{
		base.BuildingFinished();
		MapManager.main.UnlockStoryEncounterOne();
		//TownManager.main.BarBuilt();
	}

	public override void EnableActiveEffect()
	{
		base.EnableActiveEffect();
	}
	public override void DisableActiveEffect()
	{
		base.DisableActiveEffect();
	}
}

public abstract class BuildingUpgrade : BuildableObject
{
	TownBuilding mainBuilding;

	public BuildingUpgrade(TownBuilding myMainBuilding)
	{
		mainBuilding = myMainBuilding;
	}
	
	public override void BuildingFinished()
	{
		base.BuildingFinished();
		if (mainBuilding.BuildingIsActive()) EnableActiveEffect();
	}

	sealed public override void EnableActiveEffect()
	{
		if (built) EffectActivation();
	}

	protected abstract void EffectActivation();

	sealed public override void DisableActiveEffect()
	{
		if (built) EffectDeactivation();
	}
	protected abstract void EffectDeactivation();
}

public class Padlocks : BuildingUpgrade
{
	public Padlocks(TownBuilding myMainBuilding):base(myMainBuilding)
	{
		name = "Padlocks";
		icon = SpriteBase.mainSpriteBase.padlockIcon;
		requiredMaterials.Add(InventoryItem.LootItems.Scrap, 2);
		buildMoneyCost = 100;

		description = "Protect from thieves";
	}

	protected override void EffectActivation()
	{

	}
	protected override void EffectDeactivation()
	{

	}
}
