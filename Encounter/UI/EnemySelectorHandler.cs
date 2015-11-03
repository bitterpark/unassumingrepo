using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemySelectorHandler : MonoBehaviour {

	public EncounterEnemy assignedEnemy;
	
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
	/*
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
	bool _actionTaken=false;*/
	
	bool drawMouseoverText=false;
	
	public void Clicked()
	{
		//EncounterCanvasHandler.main.EnemySelectorClicked(this);
	}
	
	public void Select()
	{
		//if (!actionTaken)
		//{
			selected=true;
		//}
	}
	
	public void Deselect()
	{
		selected=false;
	}
	
	void DetermineColor()
	{
		//if (!actionTaken)
		//{
			if (_selected) 
			{
				//GetComponent<Button>().Select();
				GetComponent<Button>().image.color=Color.blue;
			}
			else 
			{
				//GetComponent<Button>().
				GetComponent<Button>().image.color=Color.black;
			}
		//}
		//else {GetComponent<Button>().image.color=Color.gray;}
	}
	
	public void AssignEnemy(EncounterEnemy enemy) 
	{
		assignedEnemy=enemy;
		GetComponent<Image>().sprite=enemy.GetSprite();
	}
	
	//public void Selected() {selected=!selected;}
	
	public void StartMouseoverText()
	{
		drawMouseoverText=true;
	}
	public void StopMouseoverText() {drawMouseoverText=false;}
	
	void OnGUI()
	{
		if (drawMouseoverText && assignedEnemy!=null)
		{
			float height=25;
			Vector3 myScreenPos=transform.position;//Camera.main.WorldToScreenPoint(transform.position);
			Rect textRect=new Rect(myScreenPos.x+20,Screen.height-myScreenPos.y-height*0.5f,100,height);
			string text=assignedEnemy.name;
			GUI.Box(textRect,text);
		}
		//drawMouseoverText=false;
	}
}
