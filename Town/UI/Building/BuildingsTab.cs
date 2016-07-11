using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BuildingsTab : MonoBehaviour,TownTab {

	public InventorySlot inventorySlotPrefab;
	public SlotItem slotItemPrefab;
	public Transform inventoryGroup;

	public BuildingInfo buildingInfoPrefab;
	public Transform buildingsGroup;


	public void OpenTab()
	{
		gameObject.SetActive(true);
		transform.SetAsLastSibling();
		RefreshBuildingList();
		RefreshInventoryItems();
		BuildingBuildButton.EBuildingBuilt += RefreshBuildingList;
		InventorySlot.EItemDropped += RefreshInventoryItems;
		InventorySlot.EItemUsed += RefreshInventoryItems;
	}

	void RefreshBuildingList()
	{
		CleanupOldBuildingList();
		foreach (TownBuilding building in TownManager.main.buildings)
		{
			BuildingInfo newBuildingInfo = Instantiate(buildingInfoPrefab);
			newBuildingInfo.AssignBuilding(building);
			newBuildingInfo.transform.SetParent(buildingsGroup,false);
		}
	}

	void CleanupOldBuildingList()
	{
		foreach (BuildingInfo oldBuilding in buildingsGroup.GetComponentsInChildren<BuildingInfo>())//Button>())
		{
			GameObject.Destroy(oldBuilding.gameObject);
		}
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
		CleanupOldBuildingList();
		InventorySlot.EItemDropped -= RefreshInventoryItems;
		InventorySlot.EItemUsed -= RefreshInventoryItems;
		BuildingBuildButton.EBuildingBuilt -= RefreshBuildingList;
		gameObject.SetActive(false);
	}
}
