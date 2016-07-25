using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MissionSelectorHandler : MonoBehaviour {
	
	public PartyMember assignedMember;
	public bool selected
	{
		get {return _selected;}
		set 
		{
			_selected=value;
			if (_selected) 
			{
				
				GetComponent<Button>().image.color=Color.blue;
				//GetComponent<Button>().image.color=Color.blue;
			}
			else 
			{
				GetComponent<Button>().image.color=assignedMember.color;
				//GetComponent<Button>().image.color=Color.black;
			}
		}
	}	
	bool _selected=false;
	bool drawMouseoverText=false;
	
	public void AssignMember(PartyMember member) 
	{
		assignedMember=member;
		GetComponent<Button>().image.color=assignedMember.color;
	}
	
	//public void Selected() {selected=!selected;}
	
	public void DrawMouseoverText()
	{
		string text=assignedMember.name;
		TooltipManager.main.CreateTooltip(text,this.transform);
	}
	public void StopMouseoverText() {TooltipManager.main.StopAllTooltips();}
	
	/*
	void OnGUI()
	{
		if (drawMouseoverText && assignedMember!=null)
		{
			float lineHeight=20;
			float height=lineHeight;
			string text=assignedMember.name;
			foreach (Relationship relation in assignedMember.relationships.Values)
			{
				text+="\n"+relation.GetText();
				height+=lineHeight;
			}
			
			Vector3 myScreenPos=transform.position;//Camera.main.WorldToScreenPoint(transform.position);
			Rect textRect=new Rect(myScreenPos.x+20,Screen.height-myScreenPos.y-height*0.5f,200,height);
			GUI.Box(textRect,text);
		}
		//drawMouseoverText=false;
	}*/
}
