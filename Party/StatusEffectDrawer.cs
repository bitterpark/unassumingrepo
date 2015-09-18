using UnityEngine;
using System.Collections;

public class StatusEffectDrawer : MonoBehaviour 
{
	
	public StatusEffect assignedEffect
	{
		get {return _assignedEffect;}
		set 
		{
			_assignedEffect=value;
			if (_assignedEffect!=null) {GetComponent<SpriteRenderer>().sprite=_assignedEffect.effectSprite;}
		}
	}
	StatusEffect _assignedEffect;
	
	bool drawMouseoverText=false;
	
	void OnMouseOver()
	{
		drawMouseoverText=true;
	}
	
	void FixedUpdate() {drawMouseoverText=false;}
	
	//void OnMouseExit() {drawMouseoverText=false;}
	
	void OnGUI()
	{
		if (drawMouseoverText)
		{
			Vector3 myScreenPos=Camera.main.WorldToScreenPoint(transform.position);
			Rect textRect=new Rect(myScreenPos.x+20,Screen.height-myScreenPos.y-20,200,40);
			GUI.Box(textRect,_assignedEffect.GetMouseoverDescription());
		}
		//drawMouseoverText=false;
	}
}
