using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeMakeButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
	
	public delegate void ItemMadeDeleg();
	public static event ItemMadeDeleg EItemMade;

	public Text resultItemCountText;
	
	InventoryItem.LootItems madeItemType;
	
	public void AssignMadeItem(CraftRecipe newRecipe)
	{
		madeItemType=newRecipe.resultItem;
		//This is very badly written
		GetComponent<Image>().sprite=InventoryItem.GetLootingItem(madeItemType).GetItemSprite();
		resultItemCountText.text="x"+newRecipe.resultItemCount;
		//Find out if item can be made
		bool canMakeItem=false;
		List<InventoryItem> usedLocalItems=new List<InventoryItem>();
		//List<InventoryItem> usedCarriedItems=new List<InventoryItem>();

		//PartyMember currentSelectedMember=InventoryScreenHandler.mainISHandler.selectedMember;
		//int totalBuildFatigueCost=newRecipe.requiredFatigue+currentSelectedMember.currentFatigueCraftPenalty;

		//if (currentSelectedMember.CheckEnoughFatigue(totalBuildFatigueCost))
		{
			//Prep used item list and required item count dict
			
			Dictionary<InventoryItem.LootItems,int> availableIngredients=new Dictionary<InventoryItem.LootItems, int>(newRecipe.requiredIngredients);
			//Run a check through local inventory and member personal inventory, to see if 
			foreach (InventoryItem.LootItems ingredientKey in newRecipe.requiredIngredients.Keys)
			{
				foreach (InventoryItem localItem in MapManager.main.GetTown().GetStashedItems())//InventoryScreenHandler.mainISHandler.selectedMember.currentRegion.GetStashedItems())
				{
					if (localItem.GetType()==InventoryItem.GetLootingItem(ingredientKey).GetType())
					{
						availableIngredients[ingredientKey]=availableIngredients[ingredientKey]-1;
						usedLocalItems.Add(localItem);
						if (availableIngredients[ingredientKey]==0) break;
					}
				}
				/*
				foreach (InventoryItem carriedItem in InventoryScreenHandler.mainISHandler.selectedMember.carriedItems)
				{
					if (carriedItem.GetType()==InventoryItem.GetLootingItem(ingredientKey).GetType())
					{
						availableIngredients[ingredientKey]=availableIngredients[ingredientKey]-1;
						usedCarriedItems.Add(carriedItem);
						if (availableIngredients[ingredientKey]==0) break;
					} 
				} */
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
		}
		
		//Refresh button
		if (canMakeItem) 
		{
			GetComponent<Button>().interactable=true;
			//Add button effect
			GetComponent<Button>().onClick.RemoveAllListeners();
			GetComponent<Button>().onClick.AddListener(
			()=>
			{
				//PartyMember creator=InventoryScreenHandler.mainISHandler.selectedMember;
				//creator.ChangeFatigue(totalBuildFatigueCost);	
				MapRegion centralRegion = MapManager.main.mapRegions[0];
				foreach (InventoryItem usedLocalItem in usedLocalItems) centralRegion.TakeStashItem(usedLocalItem);//creator.currentRegion.TakeStashItem(usedLocalItem);
				//foreach(InventoryItem usedCarriedItem in usedCarriedItems) creator.RemoveCarriedItem(usedCarriedItem);
				for(int i=0; i<newRecipe.resultItemCount; i++) centralRegion.StashItem(InventoryItem.GetLootingItem(madeItemType));
				if (RecipeMakeButton.EItemMade != null) RecipeMakeButton.EItemMade();
			});
		}
		else GetComponent<Button>().interactable=false;
		
	}
	
	#region IPointerEnterHandler implementation
	public void OnPointerEnter (PointerEventData eventData)
	{
		TooltipManager.main.CreateTooltip(InventoryItem.GetLootingItem(madeItemType).GetMouseoverDescription(),this.transform); 
	}
	#endregion

	#region IPointerExitHandler implementation
	public void OnPointerExit (PointerEventData eventData){TooltipManager.main.StopAllTooltips();}
	#endregion

}
