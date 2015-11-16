using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Encounter 
{
	public int minX=0;
	public int minY=0;
	public int maxX=0;
	public int maxY=0;
	
	protected const int safeDistanceFromEntrance=2;
	
	public string lootDescription="";
	public string enemyDescription="";
	
	public enum LootTypes {Apartment,Warehouse,Store,Police,Hospital,Endgame,Horde};
	public LootTypes encounterLootType;
	
	//key is dropchance percentage
	public Dictionary<float,InventoryItem.LootItems> lootChances;
	
	//Used by Horde
	public static Dictionary<float,InventoryItem.LootItems> GetLootChancesList(LootTypes areaType)
	{
		string emptyString=null;
		return GetLootChancesList(areaType,out emptyString);
	}
	//Not used by Horde
	public static Dictionary<float,InventoryItem.LootItems> GetLootChancesList(LootTypes areaType, out string areaDescription)
	{
		Dictionary<float, InventoryItem.LootItems>lootChances=new Dictionary<float, InventoryItem.LootItems>();
		areaDescription="";
		switch (areaType)
		{
			case LootTypes.Apartment:
			{
				areaDescription="An apartment building";
				//Add largest number first	
				lootChances.Add (0.35f,InventoryItem.LootItems.Shotgun);
				lootChances.Add (0.3f,InventoryItem.LootItems.Knife);
				//one per level - 0.2-0.4
				//one per level - 0-0.2
				lootChances.Add (0.2f,InventoryItem.LootItems.Food);
				lootChances.Add (0.1f, InventoryItem.LootItems.PerishableFood);
				break;
			}
			case LootTypes.Hospital:
			{
				areaDescription="An empty clinic";
				lootChances.Add (0.5f,InventoryItem.LootItems.Medkits);
				lootChances.Add (0.2f, InventoryItem.LootItems.Bandages);
				break;
			}
			case LootTypes.Store:
			{
				areaDescription="An abandoned store";
				lootChances.Add (0.6f,InventoryItem.LootItems.Food);
				lootChances.Add (0.4f,InventoryItem.LootItems.PerishableFood);
				break;
			}
			case LootTypes.Warehouse:
			{
				areaDescription="A warehouse";
				lootChances.Add (0.85f,InventoryItem.LootItems.ArmorVest);
				lootChances.Add (0.75f,InventoryItem.LootItems.Axe);
				lootChances.Add (0.65f,InventoryItem.LootItems.Knife);
				lootChances.Add (0.5f,InventoryItem.LootItems.Flashlight);
				lootChances.Add (0.3f,InventoryItem.LootItems.Food);
				lootChances.Add (0.1f,InventoryItem.LootItems.PerishableFood);
				break;
			}
			case LootTypes.Police:
			{
				areaDescription="A deserted police station";
				lootChances.Add (0.5f,InventoryItem.LootItems.Ammo);
				lootChances.Add (0.2f,InventoryItem.LootItems.NineM);
				lootChances.Add (0.05f,InventoryItem.LootItems.AssaultRifle);
				break;
			}
			case LootTypes.Endgame:
			{
				areaDescription="An overrun radio station";
				lootChances.Add (1f,InventoryItem.LootItems.Radio);
				break;
			}
			//case LootTypes.Horde: {break;}
		}
		return lootChances;
	}
	
	//public enum EnemyTypes {Flesh,Quick,Slime,Muscle,Transient,Gasser,Spindler};
	public EncounterEnemy.EnemyTypes encounterEnemyType;  
	
	public Dictionary<Vector2,EncounterRoom> encounterMap=new Dictionary<Vector2,EncounterRoom>();
	
	//empty constructor for Horde (it will always call at least one base constructor from the derived class constructor)
	public Encounter (int emptyInt) {}
	
	//for endgame
	public Encounter (bool isEndgame)
	{
		encounterLootType=LootTypes.Endgame;
		lootChances=Encounter.GetLootChancesList(encounterLootType,out lootDescription);
		encounterEnemyType=EncounterEnemy.EnemyTypes.Muscle; enemyDescription="muscle masses";
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.SetupEncounterMap(this,encounterLootType),1.5f);
	}
	//regular constructor
	public Encounter ()
	{
		//determine area loot type
		float lootRoll=Random.Range(0f,5f);
		if (lootRoll<=5){encounterLootType=LootTypes.Hospital;}
		if (lootRoll<=4){encounterLootType=LootTypes.Police;}
		if (lootRoll<=3) {encounterLootType=LootTypes.Apartment;}
		if (lootRoll<=2) {encounterLootType=LootTypes.Store;}
		if (lootRoll<=1) {encounterLootType=LootTypes.Warehouse;}
		
		lootChances=Encounter.GetLootChancesList(encounterLootType,out lootDescription);
		
		//determine area enemy type
		float enemiesRoll=Random.Range(0f,7f);
		if (enemiesRoll<=7) {encounterEnemyType=EncounterEnemy.EnemyTypes.Spindler;}
		if (enemiesRoll<=6) {encounterEnemyType=EncounterEnemy.EnemyTypes.Gasser;}
		if (enemiesRoll<=5) {encounterEnemyType=EncounterEnemy.EnemyTypes.Transient;}
		if (enemiesRoll<=4) {encounterEnemyType=EncounterEnemy.EnemyTypes.Muscle;}
		if (enemiesRoll<=3) {encounterEnemyType=EncounterEnemy.EnemyTypes.Flesh;}
		if (enemiesRoll<=2) {encounterEnemyType=EncounterEnemy.EnemyTypes.Quick;}
		if (enemiesRoll<=1) {encounterEnemyType=EncounterEnemy.EnemyTypes.Slime;}
		enemyDescription=EncounterEnemy.GetMapDescription(encounterEnemyType);
		//GenerateEncounter();
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.SetupEncounterMap(this,encounterLootType),1f);//0.15f);//0.3f);
		//GameManager.DebugPrint("New encounter added, maxX:"+maxX);
	}
	
	protected void GenerateEncounterFromPrefabMap(List<EncounterRoom> prefabMap, float enemyCountModifier)
	{
		
		encounterMap.Clear();
		List<EncounterRoom> roomsEligibleForEnemyPlacement=new List<EncounterRoom>();
		List<EncounterRoom> roomsEligibleForLootPlacement=new List<EncounterRoom>();
		EncounterRoom entranceRoom=null;
		foreach (EncounterRoom room in prefabMap)
		{
			encounterMap.Add (new Vector2(room.xCoord,room.yCoord),room);
			//find coordinate range within map
			minX=Mathf.Min(minX,room.xCoord);
			minY=Mathf.Min(minY,room.yCoord);
			maxX=Mathf.Max(maxX,room.xCoord);
			maxY=Mathf.Max(maxY,room.yCoord);
			
			
			if (!room.isWall)
			{
				if (room.isEntrance) {entranceRoom=room;}
				else {roomsEligibleForEnemyPlacement.Add (room);}
				if (room.hasLoot) 
				{
					room.hasLoot=false;
					roomsEligibleForLootPlacement.Add (room); 
				}
			}
		}
		
		//Generate loot placement
		int requiredLootCount=5;
		int currentLootCount=0;
		while (currentLootCount<requiredLootCount && roomsEligibleForLootPlacement.Count>0)
		{
			EncounterRoom randomlySelectedRoom=roomsEligibleForLootPlacement[Random.Range(0,roomsEligibleForLootPlacement.Count)];
			randomlySelectedRoom.hasLoot=true;
			currentLootCount++;
			roomsEligibleForLootPlacement.Remove(randomlySelectedRoom);//
		}
		
		//Generate enemy placement
		List<EncounterRoom> cachedList=new List<EncounterRoom> (roomsEligibleForEnemyPlacement);
		foreach (EncounterRoom room in cachedList)
		{
			if (Mathf.Abs(entranceRoom.xCoord-room.xCoord)+Mathf.Abs(entranceRoom.yCoord-room.yCoord)<=safeDistanceFromEntrance)
			roomsEligibleForEnemyPlacement.Remove(room);
		}
		
		int currentEnemyCount=0;
		while (currentEnemyCount<EncounterEnemy.GetEnemyCount(encounterEnemyType)*enemyCountModifier && roomsEligibleForEnemyPlacement.Count>0)
		{
			EncounterRoom randomlySelectedRoom=roomsEligibleForEnemyPlacement[Random.Range(0,roomsEligibleForEnemyPlacement.Count)];
			randomlySelectedRoom.GenerateEnemy(encounterEnemyType);
			currentEnemyCount++;
			roomsEligibleForEnemyPlacement.Remove(randomlySelectedRoom);
		}
		
	}
	//Called after an encounter finishes to reshuffle enemies and simulate the passing of time
	public void RandomizeEnemyPositions()
	{
		List<EncounterRoom> emptyRooms=new List<EncounterRoom>();
		List<EncounterRoom> roomsWithEnemies=new List<EncounterRoom>();
		
		EncounterRoom entranceRoom=null;
		foreach (EncounterRoom room in encounterMap.Values)
		{
			if (room.isEntrance) {entranceRoom=room;}
			if (room.hasEnemies) {roomsWithEnemies.Add(room);}
			else emptyRooms.Add(room);
		}
		
		
		foreach (EncounterRoom emptyRoom in new List<EncounterRoom>(emptyRooms))
		{
			if (Mathf.Abs(emptyRoom.xCoord-entranceRoom.xCoord)+Mathf.Abs(emptyRoom.yCoord-entranceRoom.yCoord)<=safeDistanceFromEntrance)
			{
				emptyRooms.Remove(emptyRoom);
			}
		}
		
		while (emptyRooms.Count>0 && roomsWithEnemies.Count>0)
		{
			EncounterRoom startRoom=roomsWithEnemies[Random.Range(0,roomsWithEnemies.Count)];
			EncounterRoom endRoom=emptyRooms[Random.Range(0,emptyRooms.Count)];
			
			endRoom.MoveEnemyIn(startRoom.enemiesInRoom[0]);
			startRoom.MoveEnemyOut(startRoom.enemiesInRoom[0]);
			endRoom.enemiesInRoom[0].SetCoords(endRoom.GetCoords());
			
			roomsWithEnemies.Remove(startRoom);
			emptyRooms.Remove(endRoom);
		}
	}
}

public class Hive:Encounter
{
	public MapRegion hiveRegion;
	int enemyCount=0;
	public void DeadEnemyReport() 
	{
		enemyCount-=1;
		if (enemyCount<=0) 
		{
			lootDescription=lootDescription.Remove(lootDescription.IndexOf(" hive"));
			PartyStatusCanvasHandler.main.NewNotification("Hive cleared!");
			hiveRegion.isHive=false;
		}
	}
	
	public Hive(MapRegion myRegion,LootTypes lootType, EncounterEnemy.EnemyTypes enemyType):base(0)
	{
		hiveRegion=myRegion;
		encounterLootType=lootType;
		lootChances=Encounter.GetLootChancesList(lootType,out lootDescription);
		lootDescription+=" hive";
		
		encounterEnemyType=enemyType;
		enemyDescription=EncounterEnemy.GetMapDescription(encounterEnemyType);
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.SetupEncounterMap(this,lootType),1.5f);
		foreach (EncounterRoom room in encounterMap.Values) 
		{if (room.hasEnemies) {enemyCount++;}}
	}
}

public class Horde:Encounter
{
	public int mapX;
	public int mapY;
	public int lifespan=3;
	public HordeTokenDrawer token;
	
	int enemyCount=0;
	
	public void ReduceLifespan()
	{
		lifespan--;
		if (lifespan<=0) {MapManager.mainMapManager.RemoveHorde(this);}
	}
	
	public void DeadEnemyReport() 
	{
		enemyCount-=1;
		if (enemyCount<=0) {MapManager.mainMapManager.RemoveHorde(this);}
	}
	//for hordes
	public Horde (EncounterEnemy.EnemyTypes enemyType, int mapCoordX, int mapCoordY) : base(0)
	{
		mapX=mapCoordX;
		mapY=mapCoordY;
		//lootChances=new Dictionary<float, InventoryItem.LootItems>();
		encounterEnemyType=enemyType;
		encounterLootType=LootTypes.Horde;
		lootChances=Encounter.GetLootChancesList(encounterLootType);
		string enemyDesc=EncounterEnemy.GetMapDescription(encounterEnemyType);
		lootDescription="A horde of "+enemyDesc+" !";
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.SetupEncounterMap(this,encounterLootType),1.5f);
		//assign enemy count
		foreach (EncounterRoom room in encounterMap.Values) 
		{if (room.hasEnemies) {enemyCount++;}}
	}

}
