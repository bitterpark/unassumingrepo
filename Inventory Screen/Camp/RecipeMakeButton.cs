using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeMakeButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
	
	InventoryItem madeItem=null;
	
	public void AssignMadeItem(InventoryItem.LootItems itemType, CraftRecipe newRecipe)
	{
		madeItem=InventoryItem.GetLootingItem(itemType);
		GetComponent<Image>().sprite=madeItem.GetItemSprite();
		//Find out if item can be made
		bool canMakeItem=false;
		List<InventoryItem> usedLocalItems=new List<InventoryItem>();
		List<InventoryItem> usedCarriedItems=new List<InventoryItem>();
		
		if (InventoryScreenHandler.mainISHandler.selectedMember.GetFatigue()<=100-newRecipe.requiredFatigue)
		{
			//Prep used item list and required item count dict
			
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
				PartyMember creator=InventoryScreenHandler.mainISHandler.selectedMember;
				creator.ChangeFatigue(newRecipe.requiredFatigue);	
				foreach(InventoryItem usedLocalItem in usedLocalItems) creator.currentRegion.TakeStashItem(usedLocalItem);
				foreach(InventoryItem usedCarriedItem in creator.carriedItems) creator.RemoveCarriedItem(usedCarriedItem);
				creator.currentRegion.StashItem(madeItem);
				InventoryScreenHandler.mainISHandler.RefreshInventoryItems();
			});
		}
		else GetComponent<Button>().interactable=false;
		
	}
	
	#region IPointerEnterHandler implementation
	public void OnPointerEnter (PointerEventData eventData)
	{
		if (madeItem!=null) TooltipManager.main.CreateTooltip(madeItem.GetMouseoverDescription(),this.transform); 
	}
	#endregion

	#region IPointerExitHandler implementation
	public void OnPointerExit (PointerEventData eventData){TooltipManager.main.StopAllTooltips();}
	#endregion

}
