using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MarketItem : MonoBehaviour, IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{

	public delegate void TransactionDeleg();
	public static event TransactionDeleg ETransactionMade;

	public Image itemIcon;
	public Text cost;
	bool buySlot;
	InventoryItem assignedItem;

	public void AssignItem(InventoryItem item, bool buy)
	{
		buySlot = buy;
		assignedItem = item;
		itemIcon.sprite = item.GetItemSprite();
		cost.text = "$"+MarketTab.genericItemCost;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (buySlot)
		{
			if (TownManager.main.money >= MarketTab.genericItemCost)
			{
				TownManager.main.money -= MarketTab.genericItemCost;
				TownManager.main.itemsOnSale.Remove(assignedItem);
				MapManager.main.mapRegions[0].StashItem(assignedItem);
				if (ETransactionMade != null) ETransactionMade();
			}
		}
		else
		{
			TownManager.main.money += MarketTab.genericItemCost;
			MapManager.main.mapRegions[0].TakeStashItem(assignedItem);
			if (ETransactionMade != null) ETransactionMade();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (assignedItem != null) TooltipManager.main.CreateTooltip(assignedItem.GetMouseoverDescription(), this.transform);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		TooltipManager.main.StopAllTooltips();
	}
}
