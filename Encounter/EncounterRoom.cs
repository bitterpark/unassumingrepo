using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Collections.Generic;

public class Barricade
{
	public int health=10;
}

public class EncounterRoom
{
	public Encounter parentEncounter;
	public List<InventoryItem> floorItems=new List<InventoryItem>();
	
	//public int currentCost=0;
	
	public bool isWall;
	
	public bool isExit
	{
		get {return _isExit;}
		set 
		{
			_isExit=value;
			if (_isExit) 
			{
				hasEnemies=false;
				hasLoot=false;
				description="This room leads outside";
			}
		}
	}
	public bool _isExit=false;
	
	public bool isEntrance=false;
	public bool isDiscovered=false;
	
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
	
	//List<EncounterEnemy> enemiesInRoom
	//public EncounterEnemy enemyInRoom
	public List<EncounterEnemy> enemiesInRoom=new List<EncounterEnemy>();
	
	public bool hasLoot
	{
		get {return _hasLoot;}
		set 
		{
			_hasLoot=value;
			if (_hasLoot) 
			{
				if (Random.value<0.5f) {lootIsLocked=true;}
			}
			else {lootIsLocked=false;}
		}
	
	}
	public bool _hasLoot=false;
	public bool lootIsLocked
	{
		get {return _lootIsLocked;}
		set 
		{
			_lootIsLocked=value;
			if (_lootIsLocked) {lockStrength=3;}
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
	
	public Trap trapInRoom=null;
	public Barricade barricadeInRoom=null;
	//public bool isVisible=false;
	
	public string description="";
	
	public int xCoord=0;
	public int yCoord=0;
	
	public Vector2 GetCoords() {return new Vector2(xCoord,yCoord);}
	
	public EncounterRoom(Encounter parent, int x, int y)
	{
		xCoord=x;
		yCoord=y;
		parentEncounter=parent;
	}
	
	public EncounterRoom(Encounter parent) {parentEncounter=parent;}
	
	//ENEMY METHODS
	
	public void GenerateEnemy(EncounterEnemy.EnemyTypes enemyType)
	{
		enemiesInRoom.Add(EncounterEnemy.GetEnemy(enemyType, new Vector2(xCoord,yCoord)));
		hasEnemies=true;
	}
	
	//These two methods are called from the corresponding RoomButton, the button is called first, and then it updates the assignedRoom
	public void MoveEnemyIn(EncounterEnemy newEnemy)
	{
		enemiesInRoom.Add(newEnemy);
		hasEnemies=true;
	}
	public void MoveEnemyOut(EncounterEnemy movingEnemy)
	{
		//GameManager.DebugPrint("enemy moved out of:"+new Vector2(xCoord,yCoord));
		enemiesInRoom.Remove(movingEnemy);
		if (enemiesInRoom.Count==0) {hasEnemies=false;}
		//GameManager.DebugPrint("enemies remaining in"+new Vector2(xCoord,yCoord)+":"+enemiesInRoom.Count);
	}
	
	public int DamageEnemy(int damage,EncounterEnemy damagedEnemy, bool isRanged)
	{
		if (enemiesInRoom.Contains(damagedEnemy))
		{
			//enemyInRoom.health-=damage;
			int enemyIndex=enemiesInRoom.IndexOf(damagedEnemy);
			int actualDmgTaken=damagedEnemy.TakeDamage(damage,isRanged);
			if (damagedEnemy.health<=0) 
			{
				//hasEnemies=false;
				//EncounterCanvasHandler.main.DisplayNewMessage(damagedEnemy.name+" killed!");
				if (parentEncounter.GetType()==typeof(Horde)) 
				{
					Horde parentHorde=parentEncounter as Horde;
					parentHorde.DeadEnemyReport();
				}
				if (parentEncounter.GetType()==typeof(Hive))
				{
					Hive parentHive=parentEncounter as Hive;
					parentHive.DeadEnemyReport();
				}
				//check if any more enemies remain in room
				enemiesInRoom.Remove(damagedEnemy);
				if (enemiesInRoom.Count==0) {hasEnemies=false;}
				//GameManager.DebugPrint(enemiesInRoom.Count+" enemies left");
			}
			return actualDmgTaken;
		}
		else {throw new System.Exception("Trying to damage an enemy that does not exist in the room!");}
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
	}
	
	public void BashBarricade(int bashStrength)
	{
		//Currently, strength is uniformly 1
		if (barricadeInRoom==null) throw new System.Exception("Trying to bash a barricade that doesn't exist in room!");
		else
		{
			barricadeInRoom.health-=bashStrength;
			if (barricadeInRoom.health<=0) {barricadeInRoom=null;}
		}
	}
	
}
