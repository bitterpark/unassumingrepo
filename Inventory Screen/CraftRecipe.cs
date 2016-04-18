using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CraftRecipe
{
	public string description;
	public Dictionary<InventoryItem.LootItems,int> requiredIngredients;
	public int requiredFatigue;
	
	//public List<InventoryItem.LootItems> resultItems=new List<InventoryItem.LootItems>();
	public InventoryItem.LootItems resultItem;
	public int resultItemCount=1;
	
	public static List<CraftRecipe> GetGenericRecipes(bool withToolbox)
	{
		List<CraftRecipe> recipeList=new List<CraftRecipe>();
		recipeList.Add(new JunkCookingRecipe());
		recipeList.Add(new CampUpgradeRecipe());
		recipeList.Add(new TrapRecipe());
		recipeList.Add(new BulletRecipe());
		recipeList.Add(new MeleeWeaponRecipe());
		recipeList.Add(new RangedWeaponRecipe());

		if (withToolbox)
		{
			recipeList.Add(new FuelToScrap());
			recipeList.Add(new PipegunToScrap());
			recipeList.Add(new PipeToScrap());
			recipeList.Add(new KnifeToScrap());

			recipeList.Add(new AxeToScrap());
		}
		return recipeList;
	}
	
	protected void SetResultItem(InventoryItem.LootItems item, int count)
	{
		resultItem=item;
		resultItemCount=count;
	}
	protected void SetResultItem(InventoryItem.LootItems item)
	{
		SetResultItem(item,1);
	}
}
/*
public class FoodRecipe: CraftRecipe
{
	public FoodRecipe()
	{
		requiredFatigue=10;
		description="Build bullets";
		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Junkfood,2);
		resultItems.Add(InventoryItem.LootItems.Food);
	}
}*/

public class BulletRecipe: CraftRecipe
{
	public  BulletRecipe()
	{
		requiredFatigue=1;
		description="Make bullets";
		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap,1);
		requiredIngredients.Add(InventoryItem.LootItems.Gunpowder,1);
		SetResultItem(InventoryItem.LootItems.Ammo,5);
	}
}

public class TrapRecipe: CraftRecipe
{
	public  TrapRecipe()
	{
		requiredFatigue=2;
		description="Make traps";
		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap,1);
		SetResultItem(InventoryItem.LootItems.SettableTrap,2);
		
	}
}

public class CampUpgradeRecipe: CraftRecipe
{
	public  CampUpgradeRecipe()
	{
		requiredFatigue=5;
		description="Make camp barricade";
		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap,3);
		SetResultItem(InventoryItem.LootItems.CampBarricade);
		
	}
}

public class MeleeWeaponRecipe: CraftRecipe
{
	public MeleeWeaponRecipe()
	{
		requiredFatigue=3;
		description="Make pipe";
		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap,1);
		SetResultItem(InventoryItem.LootItems.Pipe);
	}
}

public class RangedWeaponRecipe: CraftRecipe
{
	public RangedWeaponRecipe()
	{
		requiredFatigue=3;
		description="Make pipegun";
		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap,1);
		SetResultItem(InventoryItem.LootItems.Pipegun);
	}
}

public class JunkCookingRecipe: CraftRecipe
{
	public JunkCookingRecipe()
	{
		requiredFatigue=5;
		description="Cook food";

		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Junkfood,2);
		requiredIngredients.Add(InventoryItem.LootItems.Fuel,1);
		SetResultItem(InventoryItem.LootItems.Cookedfood,2);
	}
}

public class FuelToScrap :CraftRecipe
{
	public FuelToScrap()
	{
		requiredFatigue=2;
		description="Make scrap";

		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Fuel,1);
		SetResultItem(InventoryItem.LootItems.Scrap,1);
	}
}

public class PipegunToScrap :CraftRecipe
{
	public PipegunToScrap()
	{
		requiredFatigue=2;
		description="Make scrap";

		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Pipegun,1);
		SetResultItem(InventoryItem.LootItems.Scrap,1);
	}
}

public class PipeToScrap :CraftRecipe
{
	public PipeToScrap()
	{
		requiredFatigue=2;
		description="Make scrap";

		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Pipe,1);
		SetResultItem(InventoryItem.LootItems.Scrap,1);
	}
}
public class AxeToScrap :CraftRecipe
{
	public AxeToScrap()
	{
		requiredFatigue=2;
		description="Make scrap";

		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Axe,1);
		SetResultItem(InventoryItem.LootItems.Scrap,1);
	}
}
public class KnifeToScrap :CraftRecipe
{
	public KnifeToScrap()
	{
		requiredFatigue=2;
		description="Make scrap";

		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Knife,1);
		SetResultItem(InventoryItem.LootItems.Scrap,1);
	}
}