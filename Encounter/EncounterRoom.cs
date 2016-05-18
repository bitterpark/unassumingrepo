using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Collections.Generic;

public class Barricade
{
	public int health=3;
}

public class EncounterRoom
{
	int barricadeBuildHealth=25;
	
	public Encounter parentEncounter;
	public List<InventoryItem> floorItems=new List<InventoryItem>();
	
	//public int currentCost=0;
	
	public bool isWall;

	public bool isExit=false;
	
	public bool isEntrance=false;
	public bool isDiscovered=false;
	/*
	public bool hasEnemies
	{
		get {return _hasEnemies;}
		set 
		{
			_hasEnemies=value;
			if (_hasEnemies) 
			{
				description="There are enemies in this room!";
				//GenerateEnemy();
			}
			else 
			{
				description="This room is peaceful and has no enemies";
			}
		}
	}
	public bool _hasEnemies;
	*/

	//List<EncounterEnemy> enemiesInRoom
	//public EncounterEnemy enemyInRoom
	//public List<EncounterEnemy> enemiesInRoom=new List<EncounterEnemy>();
	
	/////LOOT
	public bool hasLoot=false;
	public List<InventoryItem> lootInRoom=new List<InventoryItem>();
	//LOCK
	public const int maxLockStrength=180;
	public void ResetLockStrength() {lockStrength=maxLockStrength;}
	public bool lootIsLocked
	{
		get {return _lootIsLocked;}
		set 
		{
			_lootIsLocked=value;
			if (_lootIsLocked) {lockStrength=maxLockStrength;}
		}
	}
	bool _lootIsLocked=false;
	public int lockStrength
	{
		get {return _lockStrength;}
		set 
		{
			_lockStrength=value;
			if (_lockStrength<=0)
			{
				_lockStrength=0;
				lootIsLocked=false;
			}
		}
	}
	int _lockStrength=0;
	//TRAP
	public Trap trapInRoom=null;
	//BARRICADE
	public bool canBarricade
	{
		get {return _canBarricade;}
		set 
		{
			_canBarricade=value;
			if (_canBarricade) {barricadeMaterials=5;}
		}
	}
	bool _canBarricade=false;
	public int barricadeMaterials
	{
		get {return _barricadeMaterials;}
		set 
		{
			_barricadeMaterials=value;
			if (_barricadeMaterials<=0)
			{
				_barricadeMaterials=0;
				canBarricade=false;
			}
		}
	}
	int _barricadeMaterials=0;
	public Barricade barricadeInRoom=null;
	//public bool isVisible=false;
	
	public bool hideImage=false;
	
	public string description="";
	
	public int xCoord=0;
	public int yCoord=0;
	
	public Vector2 GetCoords() {return new Vector2(xCoord,yCoord);}
	public void SetCoords(Vector2 newCoords) {xCoord=(int)newCoords.x; yCoord=(int)newCoords.y;}
	
	public EncounterRoom(Encounter parent, int x, int y)
	{
		xCoord=x;
		yCoord=y;
		parentEncounter=parent;
	}
	
	public EncounterRoom(Encounter parent, Vector2 coords )
	{
		xCoord=(int)coords.x;
		yCoord=(int)coords.y;
		parentEncounter=parent;	
	}
	
	public EncounterRoom(Encounter parent) {parentEncounter=parent;}

	public void GenerateUnlockedRoomLoot()
	{
		InventoryItem.LootMetatypes lootType=default(InventoryItem.LootMetatypes);
		parentEncounter.chestTypes.RollProbability(out lootType);
		lootInRoom.Clear();
		lootInRoom.AddRange(InventoryItem.GenerateUnlockedRoomLoot(lootType));
		hasLoot=true;
	}

	public void GenerateRoomLockedLoot()
	{
		InventoryItem.LootMetatypes lootType=default(InventoryItem.LootMetatypes);
		parentEncounter.chestTypes.RollProbability(out lootType);
		lootInRoom.Clear();
		lootInRoom.AddRange(InventoryItem.GenerateLockedRoomLoot(lootType));
		hasLoot=true;
		lootIsLocked=true;
	}

	//LOOT/ITEM METHODS
	public void AddFloorItem(InventoryItem item)
	{
		floorItems.Add(item);
		//EncounterCanvasHandler.main.roomButtons[new Vector2(xCoord,yCoord)].AddFloorItem(item);
	}
	public void RemoveFloorItem(InventoryItem item)
	{
		if (floorItems.Contains(item))
		{
			floorItems.Remove(item);
			//EncounterCanvasHandler.main.roomButtons[new Vector2(xCoord,yCoord)].RemoveFloorItem(item);
		} else {throw new System.Exception("Trying to remove floor item that doesn't exist in the EncounterRoom!");}
	}
	
	public void BashLock(int bashStrength)
	{
		lockStrength-=bashStrength;
	}
	
	//This method is only called from the corresponding RoomButton, the button is called first, and then it updates the assignedRoom
	/*
	public bool LootRoom(out InventoryItem.LootItems item)
	{
		hasLoot=false;
		float randomRes=Random.value;
		item=InventoryItem.LootItems.Ammo; //the compiler made me assign this, it should be unassigned/null
		bool lootFound=false;
		foreach(float chance in parentEncounter.lootChances.Keys)
		{
			if (randomRes<=chance) {item=parentEncounter.lootChances[chance]; lootFound=true;}
		}
		return lootFound;
	}*/
	//This method is only called from the corresponding RoomButton, the button is called first, and then it updates the assignedRoom
	
	public List<InventoryItem> LootRoom()
	{
		hasLoot=false;
		List<InventoryItem> foundLoot=new List<InventoryItem>(lootInRoom);
		lootInRoom.Clear();
		return foundLoot;
	}
	
	public void BashBarricade(int bashStrength)
	{
		//Currently, strength is uniformly 1
		if (barricadeInRoom==null) throw new System.Exception("Trying to bash a barricade that doesn't exist in room!");
		else
		{
			barricadeInRoom.health-=bashStrength;
			if (barricadeInRoom.health<=0) 
			{
				barricadeInRoom=null;
				EncounterCanvasHandler.main.AddNewLogMessage("Barricade is broken!");
			}
		}
	}
	
	public void BuildBarricade()
	{
		if (!canBarricade) throw new System.Exception("Trying to barricade an unbarricadeable room!");
		if (barricadeInRoom==null) 
		{
			barricadeInRoom=new Barricade();
			barricadeInRoom.health=barricadeBuildHealth;
		}
		else
		{
			barricadeInRoom.health+=barricadeBuildHealth;
		}
		barricadeMaterials-=1;
	}
	
}
