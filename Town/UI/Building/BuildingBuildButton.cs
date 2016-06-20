using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingBuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	public delegate void BuildingBuiltDeleg();
	public static event BuildingBuiltDeleg EBuildingBuilt;

	IBuildable assignedBuilding;

	public void AssignBuilding(IBuildable newBuilding)
	{
		assignedBuilding = newBuilding;
		GetComponent<Image>().sprite = assignedBuilding.GetIcon();
		if (!newBuilding.IsBuilt())
		{
			bool canBuild = false;


			List<InventoryItem> usedLocalItems = new List<InventoryItem>();

			//if (currentSelectedMember.CheckEnoughFatigue(totalBuildFatigueCost))
			{
				//Prep used item list and required item count dict
				Dictionary<InventoryItem.LootItems, int> availableIngredients 
					= new Dictionary<InventoryItem.LootItems, int>(assignedBuilding.GetMaterials());
				//Run a check through local inventory and member personal inventory, to see if 
				foreach (InventoryItem.LootItems ingredientKey in assignedBuilding.GetMaterials().Keys)
				{
					foreach (InventoryItem localItem in MapManager.main.GetTown().GetStashedItems())//InventoryScreenHandler.mainISHandler.selectedMember.currentRegion.GetStashedItems())
					{
						if (localItem.GetType() == InventoryItem.GetLootingItem(ingredientKey).GetType())
						{
							availableIngredients[ingredientKey] = availableIngredients[ingredientKey] - 1;
							usedLocalItems.Add(localItem);
							if (availableIngredients[ingredientKey] == 0) break;
						}
					}
				}
				bool enoughIngredients = true;
				foreach (int remainingRequiredCount in availableIngredients.Values)
				{
					if (remainingRequiredCount > 0)
					{
						enoughIngredients = false;
						break;
					}
				}
				canBuild = (enoughIngredients && TownManager.main.money >= assignedBuilding.GetBuildCost());
			}
			//Refresh button
			if (canBuild)
			{
				GetComponent<Button>().interactable = true;
				//Add button effect
				GetComponent<Button>().onClick.RemoveAllListeners();
				GetComponent<Button>().onClick.AddListener(
				() => ButtonClicked(usedLocalItems));
			}
			else GetComponent<Button>().interactable = false;
		}
		else
		{
			GetComponent<Button>().onClick.RemoveAllListeners();
			GetComponent<Button>().interactable = true;
		}
	}

	void ButtonClicked(List<InventoryItem> usedLocalItems)
	{
		//PartyMember creator=InventoryScreenHandler.mainISHandler.selectedMember;
		//creator.ChangeFatigue(totalBuildFatigueCost);	
		MapRegion centralRegion = MapManager.main.GetTown();
		foreach (InventoryItem usedLocalItem in usedLocalItems) centralRegion.TakeStashItem(usedLocalItem);//creator.currentRegion.TakeStashItem(usedLocalItem);
		TownManager.main.money -= assignedBuilding.GetBuildCost();
		assignedBuilding.BuildingFinished();
		if (BuildingBuildButton.EBuildingBuilt != null) 
			BuildingBuildButton.EBuildingBuilt();
	}

	#region IPointerEnterHandler implementation
	public void OnPointerEnter(PointerEventData eventData)
	{
		TooltipManager.main.CreateTooltip(assignedBuilding.GetDescription(), this.transform);
	}
	#endregion

	#region IPointerExitHandler implementation
	public void OnPointerExit(PointerEventData eventData) { TooltipManager.main.StopAllTooltips(); }
	#endregion

}
