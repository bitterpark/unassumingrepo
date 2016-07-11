using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StorageTab : MonoBehaviour, TownTab {

	public InventorySlot inventorySlotPrefab;
	public SlotItem slotItemPrefab;
	public Transform inventoryGroup;

	public void OpenTab()
	{
		gameObject.SetActive(true);
		transform.SetAsLastSibling();
		RefreshInventoryItems();
		InventorySlot.EItemDropped += RefreshInventoryItems;
		InventorySlot.EItemUsed += RefreshInventoryItems;
	}

	void RefreshInventoryItems()
	{
		CleanupOldInventorySlots();

		int slotCount = 40;
		List<InventoryItem> activeList = MapManager.main.GetTown().GetStashedItems();//PartyManager.mainPartyManager.GetPartyInventory();}
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


	void CleanupOldInventorySlots()
	{
		foreach (Image oldItemSlot in inventoryGroup.GetComponentsInChildren<Image>())//Button>())
		{
			GameObject.Destroy(oldItemSlot.gameObject);
		}
	}

	public void CloseTab()
	{
		CleanupOldInventorySlots();
		InventorySlot.EItemDropped -= RefreshInventoryItems;
		InventorySlot.EItemUsed -= RefreshInventoryItems;
		gameObject.SetActive(false);
	}
}
