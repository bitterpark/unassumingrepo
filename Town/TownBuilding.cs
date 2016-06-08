using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TownBuilding
{
	public Sprite icon;

	public bool built=false;

	public int buildMoneyCost;
	public string name;
	public string description;
	public Dictionary<InventoryItem.LootItems, int> requiredMaterials=new Dictionary<InventoryItem.LootItems,int>();

	public List<TownBuilding> upgrades=new List<TownBuilding>();

	public abstract void ActivateBuildEffect();
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

		upgrades.Add(new Padlocks());
	}

	public override void ActivateBuildEffect()
	{
		built = true;
	}
}

public class Padlocks : TownBuilding
{
	public Padlocks()
	{
		name = "Padlocks";
		icon = SpriteBase.mainSpriteBase.padlockIcon;
		requiredMaterials.Add(InventoryItem.LootItems.Scrap, 2);
		buildMoneyCost = 100;

		description = "Protect from thieves";
	}

	public override void ActivateBuildEffect()
	{
		built = true;
	}
}
