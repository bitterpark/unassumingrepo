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
	
	public static List<CraftRecipe> GetAllRecipes()
	{
		List<CraftRecipe> recipeList=new List<CraftRecipe>();
		recipeList.Add(new BulletRecipe());
		recipeList.Add(new TrapRecipe());
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
		requiredFatigue=10;
		description="Build bullets";
		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap,3);
		requiredIngredients.Add(InventoryItem.LootItems.Gunpowder,1);
		SetResultItem(InventoryItem.LootItems.Ammo,2);
	}
}

public class TrapRecipe: CraftRecipe
{
	public  TrapRecipe()
	{
		requiredFatigue=10;
		description="Build trap";
		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Scrap,1);
		SetResultItem(InventoryItem.LootItems.SettableTrap);
		
	}
}
