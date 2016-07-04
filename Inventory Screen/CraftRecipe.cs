using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CraftRecipe
{
	public string description = "Build";
	public Dictionary<InventoryItem.LootItems,int> requiredIngredients;
	//public int requiredFatigue;
	
	//public List<InventoryItem.LootItems> resultItems=new List<InventoryItem.LootItems>();
	public InventoryItem.LootItems resultItem;
	public int resultItemCount=1;
	
	public static List<CraftRecipe> GetGenericRecipes()
	{
		List<CraftRecipe> recipeList=new List<CraftRecipe>();

		recipeList.Add(new NineMRecipe());
		recipeList.Add(new AssaultRifleRecipe());
		recipeList.Add(new ShotgunRecipe());

		recipeList.Add(new PipeRecipe());
		recipeList.Add(new AxeRecipe());
		recipeList.Add(new KnifeRecipe());

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

public class NineMRecipe : CraftRecipe
{
	public NineMRecipe()
	{
		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap,1);
		SetResultItem(InventoryItem.LootItems.NineM);
	}
}

public class ShotgunRecipe : CraftRecipe
{
	public ShotgunRecipe()
	{
		requiredIngredients = new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap, 1);
		SetResultItem(InventoryItem.LootItems.Shotgun);
	}
}

public class AssaultRifleRecipe : CraftRecipe
{
	public AssaultRifleRecipe()
	{
		requiredIngredients = new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap, 1);
		SetResultItem(InventoryItem.LootItems.AssaultRifle);
	}
}

public class PipeRecipe : CraftRecipe
{
	public PipeRecipe()
	{
		requiredIngredients = new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap, 1);
		SetResultItem(InventoryItem.LootItems.Pipe);
	}
}
public class AxeRecipe : CraftRecipe
{
	public AxeRecipe()
	{
		requiredIngredients = new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap, 1);
		SetResultItem(InventoryItem.LootItems.Axe);
	}
}
public class KnifeRecipe : CraftRecipe
{
	public KnifeRecipe()
	{
		requiredIngredients = new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap, 1);
		SetResultItem(InventoryItem.LootItems.Knife);
	}
}