using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class BuildingUpgradeInfo : MonoBehaviour
{
	public Transform ingredientsGroup;
	public BuildingBuildButton buildButton;
	public Text descriptionText;
	public Text moneyCostText;

	public RecipeIngredient ingredientPrefab;

	BuildingUpgrade assignedBuilding;

	public void AssignBuilding(BuildingUpgrade newBuilding)
	{
		assignedBuilding = newBuilding;
		descriptionText.text = assignedBuilding.GetName();

		if (newBuilding.IsBuilt())
		{
			ShowCompletedUpgradeInfo();
		}
		else
		{
			ShowIncompleteUpgradeInfo();
		}

		//Refresh make button
		buildButton.AssignBuilding(newBuilding);
	}

	void ShowIncompleteUpgradeInfo()
	{
		//Add required ingredient icons
		foreach (InventoryItem.LootItems ingredientKey in assignedBuilding.GetMaterials().Keys)
		{
			RecipeIngredient newIngredientImage = Instantiate(ingredientPrefab);
			newIngredientImage.AssignIngredientItem(ingredientKey, assignedBuilding.GetMaterials()[ingredientKey]);
			newIngredientImage.transform.SetParent(ingredientsGroup, false);
		}
		moneyCostText.text = "Build cost: $" + assignedBuilding.GetBuildCost();
	}

	void ShowCompletedUpgradeInfo()
	{
		moneyCostText.gameObject.SetActive(false);
		Color textColor;
		if (assignedBuilding.MainBuildingIsActive())
		{
			textColor = Color.black;
		}
		else
		{
			textColor = Color.red;
		}
		descriptionText.color = textColor;
	}


}
