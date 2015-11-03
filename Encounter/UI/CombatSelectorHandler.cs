using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class CombatSelectorHandler : MonoBehaviour 
{
	public PartyMember assignedMember;
	public GameObject selectArrow;
	MemberTokenHandler memberMapToken;
	
	public bool selected
	{
		get {return _selected;}
		set 
		{
			_selected=value;
			DetermineColor();
		}
	}	
	bool _selected=false;
	
	public bool actionTaken
	{
		get {return _actionTaken;}
		set 
		{
			_actionTaken=value; 
			if (_actionTaken) {Deselect();}
			DetermineColor();
		}
	}
	bool _actionTaken=false;
	
	bool drawMouseoverText=false;
	
	public void Clicked()
	{
		//if (!actionTaken) EncounterCanvasHandler.main.SelectMember(this); - selectors are deprecated for now
	}
	
	public void Select()
	{
		selected=true;
	}
	
	public void Deselect()
	{
		selected=false;
	}
	
	void DetermineColor()
	{
		if (!actionTaken)
		{
			GetComponent<Button>().interactable=true;
			if (_selected) 
			{
				
				//GetComponent<Button>().image.color=Color.blue;
				selectArrow.SetActive(true);
				memberMapToken.Select();
			}
			else 
			{
				GetComponent<Button>().image.color=assignedMember.color;
				selectArrow.SetActive(false);
				memberMapToken.Deselect();
			}
		}
		else 
		{
			GetComponent<Button>().interactable=false; selectArrow.SetActive(false);
			memberMapToken.Deselect();
		}//GetComponent<Button>().image.color=Color.gray;}
	}
	
	public void AssignMember(PartyMember member, MemberTokenHandler token) 
	{
		assignedMember=member;
		GetComponent<Button>().image.color=member.color;
		memberMapToken=token;
	}
	
	//public void Selected() {selected=!selected;}
	
	public void DrawMouseoverText()
	{
		drawMouseoverText=true;
	}
	public void StopMouseoverText() {drawMouseoverText=false;}
	
	void OnGUI()
	{
		if (drawMouseoverText && assignedMember!=null)
		{
			float lineHeight=20;
			string text=assignedMember.name;
			float height=lineHeight;
			foreach (Perk perk in assignedMember.perks)
			{
				text+="\n-"+perk.name;
				height+=lineHeight;
			}
			
			Vector3 myScreenPos=Camera.main.WorldToScreenPoint(transform.position);
			Rect textRect=new Rect(myScreenPos.x+20,Screen.height-myScreenPos.y-height*0.5f,100,height);
			//Camera.main.transform.position=myScreenPos;//Camera.main.ScreenToWorldPoint(new Vector2(myScreenPos.x,myScreenPos.y));
			
			GUI.Box(textRect,text);
		}
		//drawMouseoverText=false;
	}
	
}
