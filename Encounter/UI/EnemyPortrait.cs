using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyPortrait : MonoBehaviour {

	public EncounterEnemy assignedEnemy;
	
	bool drawMouseoverText=false;
	
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
