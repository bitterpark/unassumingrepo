using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MarketTab : MonoBehaviour,TownTab 
{

	public const int genericItemCost = 300;

	public MarketItem itemPrefab;

	public Transform buyGroup;
	public Transform sellGroup;

	public GameObject buyMenuWrapper;
	public GameObject sellMenuWrapper;

	public void OpenTab()
	{
		buyMenuWrapper.SetActive(true);
		sellMenuWrapper.SetActive(true);
		transform.SetAsLastSibling();
		RefreshAllItems();
		MarketItem.ETransactionMade+=RefreshAllItems;
	}

	void RefreshAllItems()
	{
		RefreshSellList();
		RefreshBuyList();
	}

	void RefreshBuyList()
	{
		CleanupOldBuyList();
		
		foreach (InventoryItem item in TownManager.main.itemsOnSale)
		{
			MarketItem newItem = Instantiate(itemPrefab);
			newItem.AssignItem(item, true);
			newItem.transform.SetParent(buyGroup, false);
		}
	}

	void CleanupOldBuyList()
	{
		foreach (Image oldItemSlot in buyGroup.GetComponentsInChildren<Image>())GameObject.Destroy(oldItemSlot.gameObject);
		
	}

	void RefreshSellList()
	{
		CleanupOldSellList();
		
		foreach (InventoryItem item in MapManager.main.mapRegions[0].GetStashedItems())
		{
			MarketItem newItem = Instantiate(itemPrefab);
			newItem.AssignItem(item,false);
			newItem.transform.SetParent(sellGroup,false);
		}
	}

	void CleanupOldSellList()
	{
		foreach (Image oldItemSlot in sellGroup.GetComponentsInChildren<Image>())GameObject.Destroy(oldItemSlot.gameObject);
	}


	public void CloseTab()
	{
		CleanupOldBuyList();
		CleanupOldSellList();
		MarketItem.ETransactionMade -= RefreshAllItems;
		buyMenuWrapper.SetActive(false);
		sellMenuWrapper.SetActive(false);
	}
}
