using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class EncounterRoomDrawer : MonoBehaviour 
{
	public Material normalMat;
	public Material exitMat;
	public Material entranceMat;
	public Material lootMat;
	
	public int presetX;
	public int presetY;
	
	public bool isExit;
	public bool isEntrance;
	public bool hasLoot;
	bool cachedIsExit=false;
	bool cachedIsEntrance=false;
	bool cachedHasLoot=false;
	
	void Start() 
	{
		cachedIsExit=isExit;
		cachedIsEntrance=isEntrance;
		cachedHasLoot=hasLoot;
		SetColor();
	}
	
	void SetColor()
	{
		if (isExit) {GetComponent<Renderer>().material=exitMat;}
		if (isEntrance) {GetComponent<Renderer>().material=entranceMat;}
		if (hasLoot) {GetComponent<Renderer>().material=lootMat;}
		if (!isExit && !isEntrance && !hasLoot) {GetComponent<Renderer>().material=normalMat;}
	}
	
	void Update() 
	{
		if (cachedIsExit!=isExit) {cachedIsExit=isExit; SetColor();}
		if (cachedIsEntrance!=isEntrance) {cachedIsEntrance=isEntrance; SetColor();}
		if (cachedHasLoot!=hasLoot) {cachedHasLoot=hasLoot; SetColor();}
	}
	
	//public EncounterRoom roomInfo=null;
	/*
	public void SetEncounterRoomInfo(EncounterRoom newInfo)
	{
		roomInfo=newInfo;
		//because unassigned bool==false, this is necessary
		isWall=!roomInfo.isWall;
		isExit=!roomInfo.isExit;
		hasEnemies=!roomInfo.hasEnemies;
		hasLoot=!roomInfo.hasLoot;
		isVisible=!roomInfo.isVisible;
	}*/
	/*
	void SetColor()
	{
		if (isWall)
		{
			GetComponent<Renderer>().material.color=Color.black;
		}
		else
		{
		if (isExit) {GetComponent<Renderer>().material.color=Color.blue;}
		else 
		{
			if (!isVisible) {GetComponent<Renderer>().material.color=Color.gray;}
			else
			{
				if (hasEnemies) {GetComponent<Renderer>().material.color=Color.red;}
				else {if (!hasLoot) {GetComponent<Renderer>().material.color=Color.white;} else {GetComponent<Renderer>().material.color=Color.yellow;}}
			}
		
		}
		}
	}*/
	/*
	void Update()
	{
		if (roomInfo!=null) 
		{
			//print ("updating");
			isWall=roomInfo.isWall;
			isExit=roomInfo.isExit;
			hasEnemies=roomInfo.hasEnemies;
			hasLoot=roomInfo.hasLoot;
			isVisible=roomInfo.isVisible;
			
		}
	}*/
	/*
	void OnMouseDown()
	{
		if (!isWall) {EncounterManager.mainEncounterManager.RoomClicked(roomInfo);}
		//print (hasLoot);
	}*/
}
