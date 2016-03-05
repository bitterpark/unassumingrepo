using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class RecipeGroup : MonoBehaviour 
{
	public Transform ingredientsGroup;
	public RecipeMakeButton makeButton;
	public Text descriptionText;
	public Text fatigueCostText;
	
	public RecipeIngredient ingredientPrefab;
	
	CraftRecipe assignedRecipe;
	
	public void AssignRecipe(CraftRecipe newRecipe)
	{
		assignedRecipe=newRecipe;
		//Add required ingredient icons
		foreach (InventoryItem.LootItems ingredientKey in newRecipe.requiredIngredients.Keys)
		{
			RecipeIngredient newIngredientImage=Instantiate(ingredientPrefab);
			newIngredientImage.AssignIngredientItem(ingredientKey,newRecipe.requiredIngredients[ingredientKey]);
			newIngredientImage.transform.SetParent(ingredientsGroup,false);
		}
		descriptionText.text=newRecipe.description;
		fatigueCostText.text=newRecipe.requiredFatigue+" fatigue";
		/*
		//Find out if item can be made
		bool canMakeItem=false;
		if (InventoryScreenHandler.mainISHandler.selectedMember.GetFatigue()<=100-newRecipe.requiredFatigue)
		{
			//Prep used item list and required item count dict
			List<InventoryItem> usedLocalItems=new List<InventoryItem>();
			List<InventoryItem> usedCarriedItems=new List<InventoryItem>();
			Dictionary<InventoryItem.LootItems,int> availableIngredients=new Dictionary<InventoryItem.LootItems, int>(newRecipe.requiredIngredients);
			//Run a check through local inventory and member personal inventory, to see if 
			foreach (InventoryItem.LootItems ingredientKey in newRecipe.requiredIngredients.Keys)
			{
				foreach (InventoryItem localItem in InventoryScreenHandler.mainISHandler.selectedMember.currentRegion.GetStashedItems())
				{
					if (localItem.GetType()==InventoryItem.GetLootingItem(ingredientKey).GetType())
					{
						availableIngredients[ingredientKey]=availableIngredients[ingredientKey]-1;
						usedLocalItems.Add(localItem);
						if (availableIngredients[ingredientKey]==0) break;
					}
				}
				foreach (InventoryItem carriedItem in InventoryScreenHandler.mainISHandler.selectedMember.carriedItems)
				{
					if (carriedItem.GetType()==InventoryItem.GetLootingItem(ingredientKey).GetType())
					{
						availableIngredients[ingredientKey]=availableIngredients[ingredientKey]-1;
						usedCarriedItems.Add(carriedItem);
						if (availableIngredients[ingredientKey]==0) break;
					} 
				} 
			}
			bool enoughIngredients=true;
			foreach (int remainingRequiredCount in availableIngredients.Values)
			{
				if (remainingRequiredCount>0)
				{
					enoughIngredients=false;
					break;
				}
			}
			canMakeItem=enoughIngredients;
		}*/
		//Refres make button
		makeButton.AssignMadeItem(assignedRecipe.resultItem,newRecipe);
		//if (canMakeItem) makeButton.GetComponent<Button>().interactable=true;
		//else makeButton.GetComponent<Button>().interactable=false;
	}
	
}
