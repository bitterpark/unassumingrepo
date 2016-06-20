using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingInfo : MonoBehaviour {

	public Transform ingredientsGroup;
	public BuildingBuildButton buildButton;
	public Text descriptionText;
	public Text crewRequirementText;
	public Text moneyCostText;
	
	public RecipeIngredient ingredientPrefab;

	public Transform upgradesGroup;
	public BuildingUpgradeInfo upgradePrefab;

	TownBuilding assignedBuilding;

	public void AssignBuilding(TownBuilding newBuilding)
	{
		assignedBuilding = newBuilding;

		ShowGeneralInfo();
		if (!newBuilding.IsBuilt())
		{
			ShowIncompletedBuildingInfo();
		}
		else
		{
			ShowCompletedBuildingInfo();	
		}
		//Refresh make button
		buildButton.AssignBuilding(newBuilding);
	}

	void ShowGeneralInfo()
	{
		descriptionText.text = assignedBuilding.GetName();
		crewRequirementText.text = "Requires " + assignedBuilding.GetCrewRequirement() + " crew";
	}

	void ShowIncompletedBuildingInfo()
	{
		if (assignedBuilding.GetCrewRequirement() <= TownManager.main.GetCrew())
		{
			crewRequirementText.color = Color.black;
		}
		else
		{
			crewRequirementText.color = Color.red;
		}
		//Add required ingredient icons
		foreach (InventoryItem.LootItems ingredientKey in assignedBuilding.GetMaterials().Keys)
		{
			RecipeIngredient newIngredientImage = Instantiate(ingredientPrefab);
			newIngredientImage.AssignIngredientItem(ingredientKey, assignedBuilding.GetMaterials()[ingredientKey]);
			newIngredientImage.transform.SetParent(ingredientsGroup, false);
		}
		moneyCostText.text = "Build cost: $" + assignedBuilding.GetBuildCost();
	}

	void ShowCompletedBuildingInfo()
	{
		moneyCostText.gameObject.SetActive(false);
		Color textColor;
		if (assignedBuilding.BuildingIsActive())
		{
			textColor = Color.black;
		}
		else
		{
			textColor=Color.red;
		}
		crewRequirementText.color = textColor;
		descriptionText.color = textColor;

		foreach (BuildingUpgrade upgrade in assignedBuilding.upgrades)
		{
			BuildingUpgradeInfo upgradeInfo = Instantiate(upgradePrefab);
			upgradeInfo.AssignBuilding(upgrade);
			upgradeInfo.transform.SetParent(upgradesGroup, false);
		}
	}

	
	
}
