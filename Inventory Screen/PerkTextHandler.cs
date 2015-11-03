using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PerkTextHandler : MonoBehaviour {

	Perk assignedPerk;
	public Text nameText;
	bool drawMouseoverText=false;
	
	public void AssignPerk(Perk newPerk)
	{
		assignedPerk=newPerk;
		nameText.text=newPerk.name;
	}
	
	public void DrawMouseoverText() {TooltipManager.main.CreateTooltip(assignedPerk.GetMouseoverDescription(),transform);}
	public void StopMouseoverText() {TooltipManager.main.StopAllTooltips();}//drawMouseoverText=false;}
	
	/*
	void OnGUI()
	{
		if (drawMouseoverText && assignedPerk!=null)
		{
			float height=60;
			Vector3 myScreenPos=transform.position;//Camera.main.WorldToScreenPoint(transform.position);
			Rect textRect=new Rect(myScreenPos.x+20,Screen.height-myScreenPos.y-height*0.5f,200,height);
			GUI.Box(textRect,assignedPerk.GetMouseoverDescription());
		}
		//drawMouseoverText=false;
	}*/
}
