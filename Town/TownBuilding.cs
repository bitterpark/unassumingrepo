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

public abstract class BuildingUpgrade : BuildableObject
{
	TownBuilding mainBuilding;

	public BuildingUpgrade(TownBuilding myMainBuilding)
	{
		mainBuilding = myMainBuilding;
	}

	public bool MainBuildingIsActive()
	{
		return mainBuilding.BuildingIsActive();
	}

	public override void BuildingFinished()
	{
		base.BuildingFinished();
		if (mainBuilding.BuildingIsActive()) EnableActiveEffect();
	}

	public sealed override void EnableActiveEffect()
	{
		if (built) 
			EffectActivation();
	}

	protected abstract void EffectActivation();

	public sealed override void DisableActiveEffect()
	{
		if (built) 
			EffectDeactivation();
	}
	protected abstract void EffectDeactivation();
}


public class Habitation : TownBuilding
{
	int mercCountIncrease = 1;
	
	public Habitation()
	{
		name = "Habitation";
		icon = SpriteBase.mainSpriteBase.rest;
		requiredMaterials.Add(InventoryItem.LootItems.Scrap,1);
		buildMoneyCost = 800;

		description="Cabins, bathrooms and the mess hall, which currently houses the local dive. The mercs congregate here, so this is a great place to mobilize them.";
		description += "\n\nIncreases the maximum number of hireable mercenaries by " + mercCountIncrease;
		upgrades.Add(new Canteen(this));
	}

	public override void BuildingFinished()
	{
		base.BuildingFinished();
	}

	public override void EnableActiveEffect()
	{
		TownManager.main.IncrementHireableMercsPoolSize(mercCountIncrease);
		base.EnableActiveEffect();
	}
	public override void DisableActiveEffect()
	{
		base.DisableActiveEffect();
		TownManager.main.IncrementHireableMercsPoolSize(-mercCountIncrease);
	}
}

public class Canteen : BuildingUpgrade
{
	int mercCountIncrease = 1;
	public Canteen(TownBuilding myMainBuilding)
		: base(myMainBuilding)
	{
		name = "Canteen";
		icon = SpriteBase.mainSpriteBase.droplet;
		requiredMaterials.Add(InventoryItem.LootItems.Scrap, 2);
		buildMoneyCost = 800;

		description = "Upgrade our canteen (and bar) with fancy, hi-tech habitation systems, such as plumbing and refrigeration.";
		description += "\n\nIncreases the maximum number of hireable mercenaries by " + mercCountIncrease;
	}

	protected override void EffectActivation()
	{
		TownManager.main.IncrementHireableMercsPoolSize(mercCountIncrease);
	}
	protected override void EffectDeactivation()
	{
		TownManager.main.IncrementHireableMercsPoolSize(-mercCountIncrease);
	}
}

public class CommCenter : TownBuilding
{
	public CommCenter()
	{
		name = "Comms Center";
		icon = SpriteBase.mainSpriteBase.radio;
		requiredMaterials.Add(InventoryItem.LootItems.Scrap, 1);
		requiredMaterials.Add(InventoryItem.LootItems.ComputerParts, 1);
		buildMoneyCost = 800;

		description = "The heart of our operation, this is the only way to locate the parts necessary for rebuilding this scrapheap and steal them";
		description += "\n\nReveals the location of the Fusion Module";
	}

	public override void BuildingFinished()
	{
		base.BuildingFinished();
		MapManager.main.UnlockStoryEncounterOne();
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

public class Engines : TownBuilding
{
	public Engines()
	{
		name = "Engines";
		icon = SpriteBase.mainSpriteBase.flamingBullet;
		requiredMaterials.Add(InventoryItem.LootItems.Scrap, 1);
		requiredMaterials.Add(InventoryItem.LootItems.ComputerParts, 1);
		requiredMaterials.Add(InventoryItem.LootItems.FusionModule, 1);
		buildMoneyCost = 800;

		description = "Aside from all the fancy radiation shielding and life support, this is what truly separates a spaceship from any old pile of metal in the middle of nowhere. Once we build this, we can finally get off this planet.";
	}

	public override void BuildingFinished()
	{
		base.BuildingFinished();
		TownManager.main.EnginesBuilt();
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

public class EngineeringBay: TownBuilding
{
	public EngineeringBay()
	{
		name = "Engineering Bay";
		icon = SpriteBase.mainSpriteBase.wrench;
		//requiredMaterials.Add(InventoryItem.LootItems.Scrap, 1);
		buildMoneyCost = 400;

		description = "The former center for repair and maintenance. If we can bring the power tools back online, we could use them to build equipment for our guys.";
		description += "\n\nUnlocks the Workshops";
	}

	public override void BuildingFinished()
	{
		base.BuildingFinished();
	}

	public override void EnableActiveEffect()
	{
		base.EnableActiveEffect();
		TownManager.main.UnlockWorkshops();
	}
	public override void DisableActiveEffect()
	{
		base.DisableActiveEffect();
		TownManager.main.LockWorkshops();
	}
}

public class CargoBay : TownBuilding
{
	public CargoBay()
	{
		name = "Cargo Bay";
		icon = SpriteBase.mainSpriteBase.sack;
		requiredMaterials.Add(InventoryItem.LootItems.Scrap, 1);
		buildMoneyCost = 400;

		description = "A part of the future ship, for now we can use the space and equipment to open a market terminal for passing truckers.";
		description += "\n\nUnlocks the Market";

		upgrades.Add(new Loader(this));
	}

	public override void BuildingFinished()
	{
		base.BuildingFinished();
	}

	public override void EnableActiveEffect()
	{
		base.EnableActiveEffect();
		TownManager.main.UnlockMarket();
	}
	public override void DisableActiveEffect()
	{
		base.DisableActiveEffect();
		TownManager.main.LockMarket();
	}

	public class Loader : BuildingUpgrade
	{
		float marketPriceReduction = 0.2f;
		public Loader(TownBuilding myMainBuilding)
			: base(myMainBuilding)
		{
			name = "Loader";
			icon = SpriteBase.mainSpriteBase.wrench;
			requiredMaterials.Add(InventoryItem.LootItems.Scrap, 2);
			buildMoneyCost = 100;

			description = "Repairing the old cargo loader will allow us to offload cargo ourselves and save on teamster fees";
			description += "\n\nReduces market purchasing prices by " + (marketPriceReduction * 100) + "%";
		}

		protected override void EffectActivation()
		{
			TownManager.main.IncrementMarketPriceMult(-marketPriceReduction);
		}
		protected override void EffectDeactivation()
		{
			TownManager.main.IncrementMarketPriceMult(marketPriceReduction);
		}
	}
}



public class SickBay : TownBuilding
{
	int baseDailyMercHealRate = 20;
	
	public SickBay()
	{
		name = "Sick Bay";
		icon = SpriteBase.mainSpriteBase.medicalCross;
		requiredMaterials.Add(InventoryItem.LootItems.Scrap, 1);
		requiredMaterials.Add(InventoryItem.LootItems.ComputerParts, 1);
		buildMoneyCost = 400;

		description = "The ship requires a treatment center capable of dealing with spacefaring injuries and diseases. We can also use it to patch up our wounded mercs.";
		description += "\n\nIncreases daily mercenary heal rate by "+baseDailyMercHealRate;

		upgrades.Add(new HullRepair(this));
		upgrades.Add(new AirFiltration(this));
	}

	public override void BuildingFinished()
	{
		base.BuildingFinished();
	}

	public override void EnableActiveEffect()
	{
		TownManager.main.SetDailyMercHealRate(baseDailyMercHealRate);
		base.EnableActiveEffect();
	}
	public override void DisableActiveEffect()
	{
		base.DisableActiveEffect();
		TownManager.main.SetDailyMercHealRate(0);
	}

	public class HullRepair : BuildingUpgrade
	{
		int healRateIncrease = 10;
		public HullRepair(TownBuilding myMainBuilding)
			: base(myMainBuilding)
		{
			name = "Hull Repair";
			icon = SpriteBase.mainSpriteBase.padlockIcon;
			requiredMaterials.Add(InventoryItem.LootItems.ComputerParts, 1);
			buildMoneyCost = 400;

			description = "Restoring hull integrity will improve medical conditions, and also stop people from throwing garbage into the holes in walls.";
			description += "\n\nIncreases daily mercenary heal rate by " + healRateIncrease;
		}

		protected override void EffectActivation()
		{
			TownManager.main.IncrementDailyMercHealRate(healRateIncrease);
		}
		protected override void EffectDeactivation()
		{
			TownManager.main.IncrementDailyMercHealRate(-healRateIncrease);
		}
	}

	public class AirFiltration : BuildingUpgrade
	{
		int healRateIncrease = 10;
		public AirFiltration(TownBuilding myMainBuilding)
			: base(myMainBuilding)
		{
			name = "Air Filtration";
			icon = SpriteBase.mainSpriteBase.padlockIcon;
			requiredMaterials.Add(InventoryItem.LootItems.ComputerParts, 1);
			buildMoneyCost = 400;

			description = "Our docs insist this is necessary for proper quarantine procedures, but I think they're just trying to get air conditioning.";
			description += "\n\nIncreases daily mercenary heal rate by " + healRateIncrease;
		}

		protected override void EffectActivation()
		{
			TownManager.main.IncrementDailyMercHealRate(healRateIncrease);
		}
		protected override void EffectDeactivation()
		{
			TownManager.main.IncrementDailyMercHealRate(-healRateIncrease);
		}
	}
}


