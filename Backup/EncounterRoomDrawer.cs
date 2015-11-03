using UnityEngine;
using System.Collections;

public class EncounterRoomDrawer : MonoBehaviour 
{
	
	public int presetX;
	public int presetY;
	
	public bool isWall
	{
		get {return _isWall;}
		set 
		{
			_isWall=value;
			//SetColor();
		}
	}
	bool _isWall;
	
	public bool isExit
	{
		get {return _isExit;}
		set 
		{
			if (_isExit!=value)
			{
				_isExit=value;
				//SetColor();
			}
		}
	}
	
	public bool _isExit;
	
	public bool isEntrance;
	
	public bool isVisible
	{
		get {return _isVisible;}
		set
		{
			if (_isVisible!=value)
			{
				_isVisible=value;
				//SetColor();
			}
		}
	}
	
	public bool _isVisible;
	
	public bool hasEnemies
	{
		get {return _hasEnemies;}
		set 
		{
			if (_hasEnemies!=value)
			{
					_hasEnemies=value;
					//SetColor();
			}
		}
		
	}
	public bool _hasEnemies;
	
	public bool hasLoot
	{
		get {return _hasLoot;}
		set 
		{
			if (_hasLoot!=value)
			{
				_hasLoot=value;
				//SetColor();
			}
		}
		
	}
	public bool _hasLoot;
	
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
