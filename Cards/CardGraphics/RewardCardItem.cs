using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class RewardCardItem : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {

	public Image itemIcon;
	InventoryItem assignedItem;

	public void AssignItem(InventoryItem item)
	{
		assignedItem = item;
		itemIcon.sprite = item.GetItemSprite();
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
