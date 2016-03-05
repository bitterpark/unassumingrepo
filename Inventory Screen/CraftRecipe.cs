using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CraftRecipe
{
	public string description;
	public Dictionary<InventoryItem.LootItems,int> requiredIngredients;
	public int requiredFatigue;
	
	public InventoryItem.LootItems resultItem;
}

public class FoodRecipe: CraftRecipe
{
	public FoodRecipe()
	{
		requiredFatigue=10;
		description="Prepare food";
		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Junkfood,2);
		resultItem=InventoryItem.LootItems.Food;
	}
}

public class TrapRecipe: CraftRecipe
{
	public  TrapRecipe()
	{
		requiredFatigue=10;
		description="Build trap";
		requiredIngredients=new Dictionary<InventoryItem.LootItems, int>();
		requiredIngredients.Add(InventoryItem.LootItems.Pipe,1);
		resultItem=InventoryItem.LootItems.SettableTrap;
	}
	
}
