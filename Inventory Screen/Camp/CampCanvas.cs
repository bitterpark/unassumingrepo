using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CampCanvas : MonoBehaviour {

	public bool campShown=false;
	public SlotItem slotItemPrefab;
	
	public Transform cookingSlotPlacement;
	
	public Transform bedSlotGroup;
	public Text freeBedsCount;

	public Transform craftGroup;
	public RecipeGroup recipePrefab;
	public List<CraftRecipe> availableRecipes=new List<CraftRecipe>();
	
	//public Camp assignedCamp;
	public Camp assignedCamp=null;
	/*
	public void AssignCampRegion(MapRegion campRegion)
	{
		assignedCampRegion=campRegion;
		if (assignedCampRegion.hasCamp)
		{
			campShown=true;
			availableRecipes.Clear();
			availableRecipes=CraftRecipe.GetAllRecipes();
			
			GetComponent<Canvas>().enabled=true;
			RefreshSlots();
		}
		else CloseScreen();
	}*/
	
	public void RefreshSlots()
	{
		
		//Remove old cooking slot
		//Image oldCookingSlot=cookingSlotPlacement.GetComponentInChildren<Image>();
		//if (oldCookingSlot!=null) {GameObject.Destroy(oldCookingSlot.gameObject); print ("Old cooking slot deleted!");}
		// {print ("Old cooking slot not found!");}
		foreach (Image oldCookingSlot in cookingSlotPlacement.GetComponentsInChildren<Image>())//.GetComponentsInChildren<Button>())
		{
			//print ("Old cooking slot deleted!");
			GameObject.Destroy(oldCookingSlot.gameObject);
		}
		
		//Remove old bed slots
		foreach (Image oldBedSlot in bedSlotGroup.GetComponentsInChildren<Image>())//.GetComponentsInChildren<Button>())
		{
			GameObject.Destroy(oldBedSlot.gameObject);
		}
		
		//Remove old crafting recipes
		foreach (HorizontalLayoutGroup oldRecipeGroup in craftGroup.GetComponentsInChildren<HorizontalLayoutGroup>()) 
		{GameObject.Destroy(oldRecipeGroup.gameObject);}

		if (InventoryScreenHandler.mainISHandler.selectedMember.currentRegion.hasCamp)
		{
			campShown=true;
			availableRecipes.Clear();
			//Search local inventory and all present member inventories for tools
			bool partyHasTools=false;
			List<InventoryItem> sumLocalInventory=new List<InventoryItem>();
			sumLocalInventory.AddRange(InventoryScreenHandler.mainISHandler.selectedMember.currentRegion.GetStashedItems());
			foreach (PartyMember member in InventoryScreenHandler.mainISHandler.selectedMember.currentRegion.localPartyMembers)
			{
				sumLocalInventory.AddRange(member.carriedItems);
			}
			foreach (InventoryItem item in sumLocalInventory)
			{
				if (item.GetType()==typeof(Toolbox)) 
				{
					partyHasTools=true;
					break;
				}
			}

			availableRecipes=CraftRecipe.GetGenericRecipes(partyHasTools);
			GetComponent<Canvas>().enabled=true;

			assignedCamp=InventoryScreenHandler.mainISHandler.selectedMember.currentRegion.campInRegion;


			//Refresh crafting recipes
			foreach (CraftRecipe recipe in availableRecipes)
			{
				RecipeGroup newRecipeDisplay=Instantiate(recipePrefab);
				newRecipeDisplay.transform.SetParent(craftGroup);
				newRecipeDisplay.AssignRecipe(recipe);

			}
		}
		else CloseScreen();
	}
	
	public void CloseScreen()
	{
		campShown=false;
		assignedCamp=null;
		GetComponent<Canvas>().enabled=false;
	}
}
