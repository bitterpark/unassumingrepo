using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Encounter 
{
	public int minX=0;
	public int minY=0;
	public int maxX=0;
	public int maxY=0;
	
	protected const int safeDistanceFromEntrance=1;
	const float barricadeChance=0.15f;//0.08f;
	const int normalChestCountMin=2;
	const int normalChestCountMax=3;
	const float baselineEnemyPerRoomRatio=0.1f;
	readonly public int requiredMembers=2;
	
	public string lootDescription="";
	public string enemyDescription="";
	
	public enum AreaTypes {Apartment,Warehouse,Store,Police,Hospital,Endgame,Horde};
	public AreaTypes encounterAreaType;
	
	//key is dropchance percentage
	//public Dictionary<float,InventoryItem.LootItems> lootChances;
	public Dictionary<float,InventoryItem.LootMetatypes> possibleLootTypes;
	
	//Used by Horde
	public static Dictionary<float,InventoryItem.LootMetatypes> GetLootTypesList(AreaTypes areaType)
	{
		string emptyString=null;
		return GetLootTypesList(areaType,out emptyString);
	}
	
	public static InventoryItem.LootMetatypes GetChestType(AreaTypes areaType)
	{
		float roll=Random.value;
		InventoryItem.LootMetatypes chestType=InventoryItem.LootMetatypes.FoodItems;//not null because value type
		switch (areaType)
		{
		case AreaTypes.Apartment:
		{
			//Add largest number first	
			chestType=InventoryItem.LootMetatypes.Melee;
			if(roll<0.8f)chestType=InventoryItem.LootMetatypes.FoodItems;
			break;
		}
		case AreaTypes.Hospital:
		{
			chestType=InventoryItem.LootMetatypes.Medical;
			break;
		}
		case AreaTypes.Store:
		{
			chestType=InventoryItem.LootMetatypes.FoodItems;
			break;
		}
		case AreaTypes.Warehouse:
		{
			chestType=InventoryItem.LootMetatypes.Equipment;
			if(roll<0.4f)chestType=InventoryItem.LootMetatypes.Melee;
			if(roll<0.2f)chestType=InventoryItem.LootMetatypes.FoodItems;
			break;
		}
		case AreaTypes.Police:
		{
			chestType=InventoryItem.LootMetatypes.Guns;
			break;
		}
		case AreaTypes.Endgame:
		{
			chestType=InventoryItem.LootMetatypes.Radio;
			break;
		}
		}
		return chestType;
	}
	//Not used by Horde
	public static Dictionary<float,InventoryItem.LootMetatypes> GetLootTypesList(AreaTypes areaType, out string areaDescription)
	{
		//Dictionary<float, InventoryItem.LootItems>lootChances=new Dictionary<float, InventoryItem.LootItems>();
		Dictionary<float, InventoryItem.LootMetatypes> lootTypes=new Dictionary<float, InventoryItem.LootMetatypes>();
		areaDescription="";
		switch (areaType)
		{
		case AreaTypes.Apartment:
		{
			areaDescription="An apartment building";
			//Add largest number first	
			lootTypes.Add(1f,InventoryItem.LootMetatypes.Melee);
			lootTypes.Add(0.8f, InventoryItem.LootMetatypes.FoodItems);
			break;
		}
		case AreaTypes.Hospital:
		{
			areaDescription="An empty clinic";
			lootTypes.Add(1f,InventoryItem.LootMetatypes.Medical);
			break;
		}
		case AreaTypes.Store:
		{
			areaDescription="An abandoned store";
			lootTypes.Add(1f,InventoryItem.LootMetatypes.FoodItems);
			break;
		}
		case AreaTypes.Warehouse:
		{
			areaDescription="A warehouse";
			lootTypes.Add(1f,InventoryItem.LootMetatypes.Equipment);
			lootTypes.Add(0.4f,InventoryItem.LootMetatypes.Melee);
			lootTypes.Add(0.2f,InventoryItem.LootMetatypes.FoodItems);
			break;
		}
		case AreaTypes.Police:
		{
			areaDescription="A deserted police station";
			lootTypes.Add(1f,InventoryItem.LootMetatypes.Guns);
			break;
		}
		case AreaTypes.Endgame:
		{
			areaDescription="An overrun radio station";
			lootTypes.Add(1f,InventoryItem.LootMetatypes.Radio);
			break;
		}
				/*
			case AreaTypes.Apartment:
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
			case AreaTypes.Hospital:
			{
				areaDescription="An empty clinic";
				lootChances.Add (0.5f,InventoryItem.LootItems.Medkits);
				lootChances.Add (0.2f, InventoryItem.LootItems.Bandages);
				break;
			}
			case AreaTypes.Store:
			{
				areaDescription="An abandoned store";
				lootChances.Add (0.6f,InventoryItem.LootItems.Food);
				lootChances.Add (0.4f,InventoryItem.LootItems.PerishableFood);
				break;
			}
			case AreaTypes.Warehouse:
			{
				areaDescription="A warehouse";
				lootChances.Add (0.85f,InventoryItem.LootItems.ArmorVest);
				lootChances.Add (0.75f,InventoryItem.LootItems.Axe);
				lootChances.Add (0.65f,InventoryItem.LootItems.Knife);
				lootChances.Add (0.5f,InventoryItem.LootItems.SettableTrap);
				lootChances.Add (0.3f,InventoryItem.LootItems.Food);
				lootChances.Add (0.1f,InventoryItem.LootItems.PerishableFood);
				break;
			}
			case AreaTypes.Police:
			{
				areaDescription="A deserted police station";
				lootChances.Add (0.5f,InventoryItem.LootItems.Ammo);
				lootChances.Add (0.2f,InventoryItem.LootItems.NineM);
				lootChances.Add (0.05f,InventoryItem.LootItems.AssaultRifle);
				break;
			}
			case AreaTypes.Endgame:
			{
				areaDescription="An overrun radio station";
				lootChances.Add (1f,InventoryItem.LootItems.Radio);
				break;
			}*/
			//case LootTypes.Horde: {break;}
		}
		return lootTypes;
	}
	
	//public enum EnemyTypes {Flesh,Quick,Slime,Muscle,Transient,Gasser,Spindler};
	public EncounterEnemy.EnemyTypes encounterEnemyType;  
	
	public Dictionary<Vector2,EncounterRoom> encounterMap=new Dictionary<Vector2,EncounterRoom>();
	
	//empty constructor for Horde (it will always call at least one base constructor from the derived class constructor)
	public Encounter (int emptyInt) {}
	
	//for endgame
	public Encounter (bool isEndgame)
	{
		encounterAreaType=AreaTypes.Endgame;
		possibleLootTypes=Encounter.GetLootTypesList(encounterAreaType,out lootDescription);
		encounterEnemyType=EncounterEnemy.EnemyTypes.Muscle; enemyDescription="muscle masses";
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.SetupEncounterMap(this,encounterAreaType),1.5f);
	}
	//regular constructor
	public Encounter ()
	{
		//determine area loot type
		float lootRoll=Random.Range(0f,5f);
		if (lootRoll<=5){encounterAreaType=AreaTypes.Hospital;}
		if (lootRoll<=4){encounterAreaType=AreaTypes.Police;}
		if (lootRoll<=3) {encounterAreaType=AreaTypes.Apartment;}
		if (lootRoll<=2) {encounterAreaType=AreaTypes.Store;}
		if (lootRoll<=1) {encounterAreaType=AreaTypes.Warehouse;}
		
		possibleLootTypes=Encounter.GetLootTypesList(encounterAreaType,out lootDescription);
		
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
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.SetupEncounterMap(this,encounterAreaType),1f);//0.15f);//0.3f);
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
				else 
				{
					roomsEligibleForEnemyPlacement.Add (room);
					if (!room.isExit && Random.value<barricadeChance) room.canBarricade=true;//barricadeInRoom=new Barricade(); 
				}
				if (room.hasLoot) 
				{
					room.hasLoot=false;
					roomsEligibleForLootPlacement.Add (room); 
				}
			}
		}
		
		//Generate loot placement
		int requiredChestCount=Random.Range(normalChestCountMin,normalChestCountMax+1);
		int currentChestCount=0;
		while (currentChestCount<requiredChestCount && roomsEligibleForLootPlacement.Count>0)
		{
			/*
			float randomRes=Random.value;
			
			InventoryItem.LootItems item=InventoryItem.LootItems.Ammo; //the compiler made me assign this, it should be unassigned/null
			bool lootFound=false;
			foreach(float chance in lootChances.Keys)
			{
				if (randomRes<=chance) {item=lootChances[chance]; lootFound=true;}
			}
			*/
			
			//if (lootFound)
			//{
				InventoryItem.LootMetatypes chestType=GetChestType(encounterAreaType);
				EncounterRoom randomlySelectedRoom=roomsEligibleForLootPlacement[Random.Range(0,roomsEligibleForLootPlacement.Count)];
				foreach (InventoryItem lootItem in InventoryItem.GenerateLootSet(chestType)) randomlySelectedRoom.AddLootItem(lootItem);
				currentChestCount++;
				roomsEligibleForLootPlacement.Remove(randomlySelectedRoom);//
			//}
		}
		
		//Generate enemy placement
		List<EncounterRoom> cachedList=new List<EncounterRoom> (roomsEligibleForEnemyPlacement);
		//Requires a second pass, when the exit is known
		foreach (EncounterRoom room in cachedList)
		{
			if (Mathf.Abs(entranceRoom.xCoord-room.xCoord)+Mathf.Abs(entranceRoom.yCoord-room.yCoord)<=safeDistanceFromEntrance
			|| room.barricadeInRoom!=null)
			roomsEligibleForEnemyPlacement.Remove(room);
		}
		
		int currentEnemyCount=0;
		//while (currentEnemyCount<EncounterEnemy.GetEnemyCount(encounterEnemyType)*enemyCountModifier && roomsEligibleForEnemyPlacement.Count>0)
		//Approximately between 1/3 and 1/2 of the rooms should have enemies (that's the baseline)
		int desiredEnemyCount=
		Mathf.RoundToInt(roomsEligibleForEnemyPlacement.Count*EncounterEnemy.GetEnemyCountModifier(encounterEnemyType)*baselineEnemyPerRoomRatio);
		while (currentEnemyCount<desiredEnemyCount && roomsEligibleForEnemyPlacement.Count>0)
		{
			EncounterRoom randomlySelectedRoom=roomsEligibleForEnemyPlacement[Random.Range(0,roomsEligibleForEnemyPlacement.Count)];
			randomlySelectedRoom.GenerateEnemy(encounterEnemyType);
			currentEnemyCount++;
			//roomsEligibleForEnemyPlacement.Remove(randomlySelectedRoom);
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
			else 
			{
				if (!room.isWall) {emptyRooms.Add(room);}
			}
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
		
			for (int i=0; i<startRoom.enemiesInRoom.Count; i++)
			{
				endRoom.MoveEnemyIn(startRoom.enemiesInRoom[i]);
				startRoom.MoveEnemyOut(startRoom.enemiesInRoom[i]);
				endRoom.enemiesInRoom[i].SetCoords(endRoom.GetCoords());
			}	
			roomsWithEnemies.Remove(startRoom);
			emptyRooms.Remove(endRoom);
		}
	}
}

public class RandomAttack:Encounter
{
	public RandomAttack(EncounterEnemy.EnemyTypes enemyType):base(0)
	{
		encounterAreaType=AreaTypes.Horde;
		possibleLootTypes=Encounter.GetLootTypesList(encounterAreaType);
		encounterEnemyType=enemyType; enemyDescription=EncounterEnemy.GetMapDescription(enemyType);
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.SetupEncounterMap(this,encounterAreaType),1f);
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
	
	public Hive(MapRegion myRegion,AreaTypes lootType, EncounterEnemy.EnemyTypes enemyType):base(0)
	{
		hiveRegion=myRegion;
		encounterAreaType=lootType;
		possibleLootTypes=Encounter.GetLootTypesList(lootType,out lootDescription);
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
		if (lifespan<=0) {MapManager.main.RemoveHorde(this);}
	}
	
	public void DeadEnemyReport() 
	{
		enemyCount-=1;
		if (enemyCount<=0) {MapManager.main.RemoveHorde(this);}
	}
	//for hordes
	public Horde (EncounterEnemy.EnemyTypes enemyType, int mapCoordX, int mapCoordY) : base(0)
	{
		mapX=mapCoordX;
		mapY=mapCoordY;
		//lootChances=new Dictionary<float, InventoryItem.LootItems>();
		encounterEnemyType=enemyType;
		encounterAreaType=AreaTypes.Horde;
		possibleLootTypes=Encounter.GetLootTypesList(encounterAreaType);
		string enemyDesc=EncounterEnemy.GetMapDescription(encounterEnemyType);
		lootDescription="A horde of "+enemyDesc+" !";
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.SetupEncounterMap(this,encounterAreaType),1.5f);
		//assign enemy count
		foreach (EncounterRoom room in encounterMap.Values) 
		{if (room.hasEnemies) {enemyCount++;}}
	}

}
