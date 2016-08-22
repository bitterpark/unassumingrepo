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
		foreach (CraftRecipe.CraftableItems ingredientKey in newRecipe.requiredIngredients.Keys)
		{
			RecipeIngredient newIngredientImage=Instantiate(ingredientPrefab);
			newIngredientImage.AssignIngredientItem(ingredientKey,newRecipe.requiredIngredients[ingredientKey]);
			newIngredientImage.transform.SetParent(ingredientsGroup,false);
		}
		descriptionText.text=newRecipe.description;
		//fatigueCostText.text=(newRecipe.requiredFatigue+InventoryScreenHandler.mainISHandler.selectedMember.currentFatigueCraftPenalty)+" fatigue";

		//Refresh make button
		makeButton.AssignMadeItem(assignedRecipe);
		//if (canMakeItem) makeButton.GetComponent<Button>().interactable=true;
		//else makeButton.GetComponent<Button>().interactable=false;
	}
	
}
