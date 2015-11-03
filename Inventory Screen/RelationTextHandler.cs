using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RelationTextHandler : MonoBehaviour 
{
	//Relationship assignedRelation;
	public Text relationText;
	//bool drawMouseoverText=false;
	
	public void AssignRelation(Relationship relation)
	{
		//assignedRelation=relation;
		relationText.text=relation.GetText();
	}
	/*
	public void DrawMouseoverText()
	{
		drawMouseoverText=true;
	}
	public void StopMouseoverText() {drawMouseoverText=false;}
	
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
