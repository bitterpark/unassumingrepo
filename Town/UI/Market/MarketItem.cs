using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MarketItem : MonoBehaviour, IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{

	public delegate void TransactionDeleg();
	public static event TransactionDeleg ETransactionMade;

	public Image itemIcon;
	public Text costText;
	int cost;
	bool buySlot;
	InventoryItem assignedItem;

	public void AssignItem(InventoryItem item, bool buy)
	{
		buySlot = buy;
		assignedItem = item;
		itemIcon.sprite = item.GetItemSprite();
		if (buy)
			cost = Mathf.RoundToInt(MarketTab.genericItemCost * TownManager.main.GetMarketPriceMult());
		else
			cost = MarketTab.genericItemCost;
		costText.text = "$" + cost;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (buySlot)
		{
			if (TownManager.main.money >= cost)
			{
				TownManager.main.money -= cost;
				TownManager.main.itemsOnSale.Remove(assignedItem);
				MapManager.main.GetTown().StashItem(assignedItem);
				if (ETransactionMade != null) ETransactionMade();
			}
		}
		else
		{
			TownManager.main.money += MarketTab.genericItemCost;
			MapManager.main.GetTown().TakeStashItem(assignedItem);
			if (ETransactionMade != null) ETransactionMade();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (assignedItem != null) TooltipManager.main.CreateItemTooltip(assignedItem, transform);
		//TooltipManager.main.CreateTooltip(assignedItem.GetMouseoverDescription(), this.transform);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		TooltipManager.main.StopAllTooltips();
	}
}
