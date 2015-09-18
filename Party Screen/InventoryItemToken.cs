using UnityEngine;
using System.Collections;

public class InventoryItemToken : MonoBehaviour {

	public bool isWeaponSlot=false;
	
	public InventoryItem myItem
	{
		get {return _myItem;}
		set 
		{
			_myItem=value;
			if (myItem!=null) gameObject.GetComponent<SpriteRenderer>().sprite=_myItem.GetItemSprite();
		}
	}
	InventoryItem _myItem;
	
	bool drawMouseoverText=false;
	
	void OnMouseDown()
	{
		
		PartyScreenManager.mainPSManager.InventoryItemClicked(this);
		//if (!isWeaponSlot) {PartyScreenManager.mainPSManager.InventoryItemClicked(this);}
		//else {}
		//print ("item clicked!");
	}
	
	void OnMouseOver()
	{
		drawMouseoverText=true;
	}
	
	void FixedUpdate() {drawMouseoverText=false;}
	
	//void OnMouseExit() {drawMouseoverText=false;}
	
	void OnGUI()
	{
		if (drawMouseoverText && _myItem!=null)
		{
			Vector3 myScreenPos=Camera.main.WorldToScreenPoint(transform.position);
			Rect textRect=new Rect(myScreenPos.x+20,Screen.height-myScreenPos.y-20,200,40);
			GUI.Box(textRect,_myItem.GetMouseoverDescription());
		}
		//drawMouseoverText=false;
	}
}
