using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class CraftRecipe
{
	public string description = "Build";
	public Dictionary<CraftableItems, int> requiredIngredients;
	//public int requiredFatigue;
	
	//public List<CraftableItems> resultItems=new List<CraftableItems>();
	public CraftableItems resultItem;
	public int resultItemCount=1;

	public enum CraftableItems
	{
		Scrap, ComputerParts,FusionModule
			, AssaultRifle, Shotgun, NineM
			, Pipe, Knife,Axe
			, ArmorVest,Stim,Ammopack
	}
	public static InventoryItem GetItemInstance(CraftableItems itemType)
	{
		InventoryItem itemInstance = null;
		switch (itemType)
		{
			//MELEE WEAPONS
			case CraftableItems.Pipe: { itemInstance = new Pipe(); break; }
			case CraftableItems.Knife: { itemInstance = new Knife(); break; }
			case CraftableItems.Axe: { itemInstance = new Axe(); break; }
			//RANGED WEAPONS
			case CraftableItems.NineM: { itemInstance = new NineM(); break; }
			case CraftableItems.Shotgun: { itemInstance = new Shotgun(); break; }
			case CraftableItems.AssaultRifle: { itemInstance = new AssaultRifle(); break; }
			//EQUIPMENT
			case CraftableItems.Stim: { itemInstance = new Stim(); break; }
			case CraftableItems.ArmorVest: { itemInstance = new ArmorVest(); break; }
			case CraftableItems.Ammopack: { itemInstance = new AmmoPouch(); break; }
			//MISC
			//INGREDIENTS
			case CraftableItems.Scrap: { itemInstance = new Scrap(); break; }
			case CraftableItems.ComputerParts: { itemInstance = new ComputerParts(); break; }
			case CraftableItems.FusionModule: { itemInstance = new FusionCore(); break; }
		}
		return itemInstance;
	}

	public static List<CraftRecipe> GetGenericRecipes()
	{
		List<CraftRecipe> recipeList=new List<CraftRecipe>();

		recipeList.Add(new NineMRecipe());
		recipeList.Add(new AssaultRifleRecipe());
		recipeList.Add(new ShotgunRecipe());

		recipeList.Add(new PipeRecipe());
		recipeList.Add(new AxeRecipe());
		recipeList.Add(new KnifeRecipe());

		recipeList.Add(new StimRecipe());
		recipeList.Add(new ArmorVestRecipe());
		recipeList.Add(new AmmopouchRecipe());

		return recipeList;
	}

	public CraftRecipe()
	{
		requiredIngredients = new Dictionary<CraftableItems, int>();
		requiredIngredients.Add(CraftableItems.Scrap, 1);
		ExtenderConstructor();
	}
	protected abstract void ExtenderConstructor();

	protected void SetResultItem(CraftableItems item, int count)
	{
		resultItem=item;
		resultItemCount=count;
	}
	protected void SetResultItem(CraftableItems item)
	{
		SetResultItem(item,1);
	}
}

public class NineMRecipe : CraftRecipe
{
	protected override void ExtenderConstructor()
	{
		
		SetResultItem(CraftableItems.NineM);
	}
}

public class ShotgunRecipe : CraftRecipe
{
	protected override void ExtenderConstructor()
	{
		SetResultItem(CraftableItems.Shotgun);
	}
}

public class AssaultRifleRecipe : CraftRecipe
{
	protected override void ExtenderConstructor()
	{
		SetResultItem(CraftableItems.AssaultRifle);
	}
}

public class PipeRecipe : CraftRecipe
{
	protected override void ExtenderConstructor()
	{
		SetResultItem(CraftableItems.Pipe);
	}
}
public class AxeRecipe : CraftRecipe
{
	protected override void ExtenderConstructor()
	{
		SetResultItem(CraftableItems.Axe);
	}
}
public class KnifeRecipe : CraftRecipe
{
	protected override void ExtenderConstructor()
	{
		SetResultItem(CraftableItems.Knife);
	}
}
public class StimRecipe : CraftRecipe
{
	protected override void ExtenderConstructor()
	{
		SetResultItem(CraftableItems.Stim);
	}
}
public class  AmmopouchRecipe : CraftRecipe
{
	protected override void ExtenderConstructor()
	{
		SetResultItem(CraftableItems.Ammopack);
	}
}
public class ArmorVestRecipe : CraftRecipe
{
	protected override void ExtenderConstructor()
	{
		SetResultItem(CraftableItems.ArmorVest);
	}
}