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

	IBuildable assignedBuilding;

	public void AssignBuilding(BuildingUpgrade newBuilding)
	{
		assignedBuilding = newBuilding;
		descriptionText.text = assignedBuilding.GetName();

		if (!newBuilding.IsBuilt())
		{
			//Add required ingredient icons
			foreach (InventoryItem.LootItems ingredientKey in assignedBuilding.GetMaterials().Keys)
			{
				RecipeIngredient newIngredientImage = Instantiate(ingredientPrefab);
				newIngredientImage.AssignIngredientItem(ingredientKey, assignedBuilding.GetMaterials()[ingredientKey]);
				newIngredientImage.transform.SetParent(ingredientsGroup, false);
			}
			moneyCostText.text = "Build cost: $" + newBuilding.GetBuildCost();
		}

		//Refresh make button
		buildButton.AssignBuilding(newBuilding);
	}



}
