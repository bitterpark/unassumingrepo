using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CampCanvas : MonoBehaviour {

	public bool campShown=false;
	public SlotItem slotItemPrefab;
	
	public CookingSlot cookingSlotPrefab;
	public Transform cookingSlotPlacement;
	
	public BedSlot bedSlotPrefab;
	public Transform bedSlotGroup;
	public Text freeBedsCount;
	
	public Transform craftGroup;
	public RecipeGroup recipePrefab;
	public List<CraftRecipe> availableRecipes=new List<CraftRecipe>();
	
	public Camp assignedCamp;
	
	public void AssignCamp(Camp newCamp, bool regionHasCamp)
	{
		if (regionHasCamp)
		{
			campShown=true;
			assignedCamp=newCamp;
			availableRecipes.Clear();
			availableRecipes.Add(new FoodRecipe());
			availableRecipes.Add(new TrapRecipe());
			
			GetComponent<Canvas>().enabled=true;
			RefreshSlots();
		}
		else CloseScreen();
	}
	
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
		
		if (assignedCamp!=null)
		{
			//Refresh cooking slot
			//print ("New cooking slot created!");
			CookingSlot newCookingSlot=Instantiate(cookingSlotPrefab);
			newCookingSlot.transform.SetParent(cookingSlotPlacement,false);
			if (assignedCamp.cookingImplement!=null)
			{
				
				SlotItem cookingImplement=Instantiate(slotItemPrefab);
				cookingImplement.AssignItem(assignedCamp.cookingImplement);
				newCookingSlot.AssignItem(cookingImplement);
				//!!NO ONCLICK LISTENER ASSIGNED!!
			}
			
			//Refresh bed slots
			int bedSlotCount=assignedCamp.beds.Count+1;
			int occupiedBedsCount=assignedCamp.beds.Count-assignedCamp.freeBeds;
			for (int i=0; i<bedSlotCount; i++)
			{
				BedSlot newBedSlot=Instantiate(bedSlotPrefab);
				newBedSlot.transform.SetParent(bedSlotGroup,false);
				if (i<assignedCamp.beds.Count)
				{
					SlotItem newBed=Instantiate(slotItemPrefab);
					newBed.AssignItem(assignedCamp.beds[i]);
					newBedSlot.AssignItem(newBed);
					if (occupiedBedsCount>0)
					{
						newBed.draggable=false;
						occupiedBedsCount-=1;
					}
					//!!NO ONCLICK LISTENER ASSIGNED!!
				}
			}
			freeBedsCount.text="Free beds:"+assignedCamp.freeBeds;
			
			//Refresh crafting recipes
			foreach (CraftRecipe recipe in availableRecipes)
			{
				RecipeGroup newRecipeDisplay=Instantiate(recipePrefab);
				newRecipeDisplay.transform.SetParent(craftGroup);
				newRecipeDisplay.AssignRecipe(recipe);
				
			}
			
		}
	}
	
	public void CloseScreen()
	{
		campShown=false;
		assignedCamp=null;
		GetComponent<Canvas>().enabled=false;
	}
}
