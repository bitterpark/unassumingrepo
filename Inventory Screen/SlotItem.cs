using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotItem : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler, IDropHandler, IPointerClickHandler
{
	public static SlotItem itemBeingDragged;
	
	public InventorySlot currentSlot;
	public InventoryItem assignedItem;
	
	public bool draggable=true;
	
	bool drawMouseoverText=false;
	
	public void AssignItem(InventoryItem newItem)
	{
		GetComponent<Image>().sprite=newItem.GetItemSprite();
		assignedItem=newItem;
	}
	
	public void SetInSlot(InventorySlot slot)
	{	
		currentSlot=slot;
		transform.SetParent(slot.transform,false);
		//transform.position=transform.parent.position;
	}
	
	#region IBeginDragHandler implementation
	public void OnBeginDrag (PointerEventData eventData)
	{
		if (draggable)
		{
			itemBeingDragged=this;
			//makes sure OnDrop fires correctly for all objects
			GetComponent<CanvasGroup>().blocksRaycasts=false;
			transform.SetParent(transform.GetComponentInParent<Canvas>().transform,false);//PartyStatusCanvasHandler.main.transform);
		}
	}
	#endregion
	
	
	#region IDragHandler implementation
	public void OnDrag (PointerEventData eventData)
	{
		if (draggable)
		{
			Vector3 mousePositionInWorldCoords=Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 newItemPosition=new Vector3(mousePositionInWorldCoords.x,mousePositionInWorldCoords.y,transform.position.z);
			transform.position = newItemPosition;
		}
	}
	#endregion

	#region IEndDragHandler implementation
	
	public void OnEndDrag (PointerEventData eventData)
	{
		
		if (draggable)
		{
			GetComponent<CanvasGroup>().blocksRaycasts=true;
			transform.SetParent(currentSlot.transform,false);
			transform.position=transform.parent.position;
			itemBeingDragged=null;
		}
	}
	#endregion
	
	public void StartMouseoverText()
	{
		if (assignedItem != null)
		{
			TooltipManager.main.CreateItemTooltip(assignedItem, transform);
		}
	}
	public void StopMouseoverText() 
	{
		TooltipManager.main.StopAllTooltips();
	}
	
	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		//SlotItem.itemBeingDragged.currentSlot.EmptySlot;
		//print ("Drop fired");
		currentSlot.ItemDroppedIn(SlotItem.itemBeingDragged);
	}
	#endregion

	#region IPointerClickHandler implementation

	public void OnPointerClick(PointerEventData eventData)
	{
		//if (eventData.button == PointerEventData.InputButton.Right && InventoryScreenHandler.mainISHandler.inventoryShown)
			//currentSlot.RclickAction();
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			currentSlot.LclickAction();
		}
	}

	#endregion
}
