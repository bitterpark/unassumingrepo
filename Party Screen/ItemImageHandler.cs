using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemImageHandler : MonoBehaviour 
{
	public InventoryItem assignedItem
	{
		get {return _assignedItem;}
	}
	InventoryItem _assignedItem;
	public Sprite emptySlotSprite;
	bool drawMouseoverText=false;
	
	public void AssignItem(InventoryItem newItem)
	{
		_assignedItem=newItem;
		if (newItem!=null) 
		{
			GetComponent<Image>().sprite=newItem.GetItemSprite();
		} else {GetComponent<Image>().sprite=emptySlotSprite;}
	}
	
	public void DrawMouseoverText()
	{
		drawMouseoverText=true;
	}
	public void StopMouseoverText() {drawMouseoverText=false;}
	
	void OnGUI()
	{
		if (drawMouseoverText && assignedItem!=null)
		{
			float height=60;
			Vector3 myScreenPos=transform.position;//Camera.main.WorldToScreenPoint(transform.position);
			Rect textRect=new Rect(myScreenPos.x+20,Screen.height-myScreenPos.y-height*0.5f,200,height);
			GUI.Box(textRect,assignedItem.GetMouseoverDescription());
		}
		//drawMouseoverText=false;
	}
}
