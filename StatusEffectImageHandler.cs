using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StatusEffectImageHandler : MonoBehaviour 
{

	StatusEffect assignedEffect;
	bool drawMouseoverText=false;
	
	public void AssignStatusEffect(StatusEffect effect) 
	{
		assignedEffect=effect;
		GetComponent<Image>().sprite=assignedEffect.effectSprite;
	}	
	
	public void DrawMouseoverText()
	{
		drawMouseoverText=true;
	}
	public void StopMouseoverText() {drawMouseoverText=false;}
	
	void OnGUI()
	{
		if (drawMouseoverText)
		{
			float height=60;
			Vector3 myScreenPos=transform.position;//Camera.main.WorldToScreenPoint(transform.position);
			Rect textRect=new Rect(myScreenPos.x+20,Screen.height-myScreenPos.y-height*0.5f,200,height);
			GUI.Box(textRect,assignedEffect.GetMouseoverDescription());
		}
		//drawMouseoverText=false;
	}
}
