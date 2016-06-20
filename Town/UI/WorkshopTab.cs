using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WorkshopTab : MonoBehaviour, TownTab {

	public InventorySlot inventorySlotPrefab;
	public SlotItem slotItemPrefab;
	public Transform inventoryGroup;

	public Transform recipeGroup;
	public RecipeGroup recipePrefab;
	public List<CraftRecipe> availableRecipes = new List<CraftRecipe>();

	public void OpenTab()
	{
		gameObject.SetActive(true);
		transform.SetAsLastSibling();
		RefreshAllItems();
		RecipeMakeButton.EItemMade += RefreshAllItems;
		InventorySlot.EItemDropped += RefreshAllItems;
	}

	void RefreshAllItems()
	{
		RefreshInventoryItems();
		RefreshCraftingList();
	}

	void RefreshInventoryItems()
	{
		CleanupOldInventorySlots();

		int slotCount = 48;
		List<InventoryItem> activeList=MapManager.main.GetTown().GetStashedItems();//PartyManager.mainPartyManager.GetPartyInventory();}
		//Use floor or inventory list
		for (int i = 0; i < slotCount; i++)
		{
			InventorySlot newSlot = Instantiate(inventorySlotPrefab);
			newSlot.transform.SetParent(inventoryGroup, false);
			if (i < activeList.Count)
			{
				SlotItem newItem = Instantiate(slotItemPrefab);
				newItem.AssignItem(activeList[i]);
				newSlot.AssignItem(newItem);
				//newItem.GetComponent<Button>().onClick.AddListener(() => InventoryItemClicked(newItem.assignedItem, newItem.currentSlot));
			}
		}
	}

	

	void RefreshCraftingList()
	{
		CleanupOldCraftingList();
		//Refresh crafting recipes
		availableRecipes = CraftRecipe.GetGenericRecipes(false);
		foreach (CraftRecipe recipe in availableRecipes)
		{
			RecipeGroup newRecipeDisplay = Instantiate(recipePrefab);
			newRecipeDisplay.transform.SetParent(recipeGroup);
			newRecipeDisplay.AssignRecipe(recipe);
		}
	}

	void CleanupOldInventorySlots()
	{
		foreach (Image oldItemSlot in inventoryGroup.GetComponentsInChildren<Image>())//Button>())
		{
			GameObject.Destroy(oldItemSlot.gameObject);
		}
	}

	void CleanupOldCraftingList()
	{
		foreach (HorizontalLayoutGroup oldRecipeGroup in recipeGroup.GetComponentsInChildren<HorizontalLayoutGroup>())
		{ GameObject.Destroy(oldRecipeGroup.gameObject); }
	}

	public void CloseTab()
	{
		CleanupOldInventorySlots();
		CleanupOldCraftingList();
		RecipeMakeButton.EItemMade -= RefreshAllItems;
		InventorySlot.EItemDropped -= RefreshAllItems;
		gameObject.SetActive(false);
	}
}
