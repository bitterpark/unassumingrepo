using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingInfo : MonoBehaviour {

	public Transform ingredientsGroup;
	public BuildingBuildButton buildButton;
	public Text descriptionText;
	public Text moneyCostText;
	
	public RecipeIngredient ingredientPrefab;

	public Transform upgradesGroup;
	public BuildingInfo upgradePrefab;

	TownBuilding assignedBuilding;

	public void AssignBuilding(TownBuilding newBuilding)
	{
		assignedBuilding = newBuilding;
		descriptionText.text = assignedBuilding.name;

		if (!newBuilding.built)
		{
			//Add required ingredient icons
			foreach (InventoryItem.LootItems ingredientKey in assignedBuilding.requiredMaterials.Keys)
			{
				RecipeIngredient newIngredientImage = Instantiate(ingredientPrefab);
				newIngredientImage.AssignIngredientItem(ingredientKey, assignedBuilding.requiredMaterials[ingredientKey]);
				newIngredientImage.transform.SetParent(ingredientsGroup, false);
			}
			moneyCostText.text = "$" + newBuilding.buildMoneyCost;
		}
		else
		{
			foreach (TownBuilding upgrade in newBuilding.upgrades)
			{
				BuildingInfo upgradeInfo = Instantiate(upgradePrefab);
				upgradeInfo.AssignBuilding(upgrade);
				upgradeInfo.transform.SetParent(upgradesGroup,false);
			}
		}
		//Refresh make button
		buildButton.AssignBuilding(newBuilding);
	}
	
}
