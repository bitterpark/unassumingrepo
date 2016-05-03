using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Encounter 
{
	public int minX=int.MaxValue;
	public int minY=int.MaxValue;
	public int maxX=int.MinValue;
	public int maxY=int.MinValue;
	
	protected const int safeDistanceFromEntrance=1;
	const float barricadeChance=0.35f;//0.08f;
	const int normalChestCountMin=2;
	const int normalChestCountMax=3;
	const float baselineEnemyPerRoomRatio=0.1f;
	readonly public int minRequiredMembers=1;
	readonly public int maxRequiredMembers=20;
	
	public string lootDescription="";
	public string enemyDescription="";
	
	public enum AreaTypes {Apartment,Warehouse,Store,Police,Hospital,Endgame,Horde};
	public AreaTypes encounterAreaType;
	
	//key is dropchance percentage
	//public Dictionary<float,InventoryItem.LootItems> lootChances;
	//public Dictionary<float,InventoryItem.LootMetatypes> possibleLootTypes;
	public ProbabilityListValueTypes<InventoryItem.LootMetatypes> chestTypes;

	public static InventoryItem.LootMetatypes GetChestType(ProbabilityListValueTypes<InventoryItem.LootMetatypes> chestRandomizer)
	{
		float roll=Random.value;
		InventoryItem.LootMetatypes chestType=InventoryItem.LootMetatypes.FoodItems;//not null because value type
		if (!chestRandomizer.RollProbability(out chestType)) throw new System.Exception("Unable to roll a positive result on chest probability!");
		return chestType;
	}

	public static ProbabilityListValueTypes<InventoryItem.LootMetatypes> GetLootTypesList(AreaTypes areaType, out string areaDescription)
	{
		//Dictionary<float, InventoryItem.LootItems>lootChances=new Dictionary<float, InventoryItem.LootItems>();
		ProbabilityListValueTypes<InventoryItem.LootMetatypes> lootTypes=new ProbabilityListValueTypes<InventoryItem.LootMetatypes>();
		areaDescription="";
		switch (areaType)
		{
		case AreaTypes.Apartment:
		{
			areaDescription="An apartment building";

			//Add largest number first	
			lootTypes.AddProbability(InventoryItem.LootMetatypes.ApartmentSalvage,0.6f);
			lootTypes.AddProbability(InventoryItem.LootMetatypes.FoodItems,0.25f);
			lootTypes.AddProbability(InventoryItem.LootMetatypes.Melee,0.15f);
			break;
		}
		case AreaTypes.Hospital:
		{
			areaDescription="An empty clinic";
			lootTypes.AddProbability(InventoryItem.LootMetatypes.Medical,1f);
			break;
		}
		case AreaTypes.Store:
		{
			areaDescription="An abandoned store";
			lootTypes.AddProbability(InventoryItem.LootMetatypes.FoodItems,1f);
			break;
		}
		case AreaTypes.Warehouse:
		{
			areaDescription="A warehouse";
			//Add largest number first	

			lootTypes.AddProbability(InventoryItem.LootMetatypes.WarehouseSalvage,0.6f);
			lootTypes.AddProbability(InventoryItem.LootMetatypes.Equipment,0.3f);
			lootTypes.AddProbability(InventoryItem.LootMetatypes.Melee,0.1f);
			break;
		}
		case AreaTypes.Police:
		{
			areaDescription="A deserted police station";
			lootTypes.AddProbability(InventoryItem.LootMetatypes.Guns,0.5f);
			lootTypes.AddProbability(InventoryItem.LootMetatypes.Gear,0.3f);
			lootTypes.AddProbability(InventoryItem.LootMetatypes.ApartmentSalvage,0.2f);
			break;
		}
		case AreaTypes.Endgame:
		{
			areaDescription="An overrun radio station";
			lootTypes.AddProbability(InventoryItem.LootMetatypes.Radio,1f);
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
		chestTypes=Encounter.GetLootTypesList(encounterAreaType,out lootDescription);
		encounterEnemyType=EncounterEnemy.EnemyTypes.Muscle; enemyDescription="muscle masses";
		List<EncounterRoom> nonSegmentRooms=null;
		maxRequiredMembers=4;
		Dictionary<Vector2,Dictionary<Vector2,EncounterRoom>> newEncounterSegments
		=PrefabAssembler.assembler.GenerateEncounterMap(this,encounterAreaType,maxRequiredMembers,out nonSegmentRooms);
		GenerateEncounterFromPrefabMap(newEncounterSegments,nonSegmentRooms);
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
		
		chestTypes=Encounter.GetLootTypesList(encounterAreaType,out lootDescription);
		
		//determine area enemy type
		List<EncounterEnemy.EnemyTypes> potentialEnemyTypes=new List<EncounterEnemy.EnemyTypes>();
		potentialEnemyTypes.Add(EncounterEnemy.EnemyTypes.Spindler);
		potentialEnemyTypes.Add(EncounterEnemy.EnemyTypes.Gasser);
		potentialEnemyTypes.Add(EncounterEnemy.EnemyTypes.Muscle);
		potentialEnemyTypes.Add(EncounterEnemy.EnemyTypes.Slime);
		potentialEnemyTypes.Add(EncounterEnemy.EnemyTypes.Flesh);
		//potentialEnemyTypes.Add(EncounterEnemy.EnemyTypes.Flesh);

		encounterEnemyType=potentialEnemyTypes[Random.Range(0,potentialEnemyTypes.Count)];

		encounterEnemyType=EncounterEnemy.EnemyTypes.Muscle;
		enemyDescription=EncounterEnemy.GetMapDescription(encounterEnemyType);
		
		//Determine member requirement
		minRequiredMembers=1;
		maxRequiredMembers=0;
		float sizeRoll=Random.value;
		if (sizeRoll<1f) maxRequiredMembers=1;
		if (sizeRoll<0.5f) maxRequiredMembers=2;
		if (sizeRoll<0.25f) maxRequiredMembers=3;
		
		//GenerateEncounter();
		List<EncounterRoom> nonSegmentRooms=null;
		Dictionary<Vector2,Dictionary<Vector2,EncounterRoom>> newEncounterSegments
		=PrefabAssembler.assembler.GenerateEncounterMap(this,encounterAreaType,1,out nonSegmentRooms);
		GenerateEncounterFromPrefabMap(newEncounterSegments,nonSegmentRooms);//.SetupEncounterMap(this,encounterAreaType),1f);//0.15f);//0.3f);
		//GameManager.DebugPrint("New encounter added, maxX:"+maxX);
	}
	//enemy count modifier is no longer used
	protected void GenerateEncounterFromPrefabMap(Dictionary<Vector2,Dictionary<Vector2,EncounterRoom>> prefabMap, List<EncounterRoom> nonSegmentRooms)
	{
		int segmentCount=0;
		//Dictionary<Vector2,List<EncounterRoom>> segmentRoomsEligibleForEnemyPlacement=new Dictionary<Vector2,List<EncounterRoom>>();
		List<List<EncounterRoom>> lootRoomsBySegment=new List<List<EncounterRoom>>();
		encounterMap.Clear();
		List<EncounterRoom> roomsEligibleForEnemyPlacement=new List<EncounterRoom>();
		//List<EncounterRoom> roomsEligibleForLootPlacement=new List<EncounterRoom>();
		EncounterRoom entranceRoom=null;
		foreach (Vector2 segmentKey in prefabMap.Keys)
		{
			//segmentRoomsEligibleForEnemyPlacement.Add(segmentKey,new List<EncounterRoom>(prefabMap[segmentKey].Values));
			List<EncounterRoom> segmentLootRooms=new List<EncounterRoom>();//(prefabMap[segmentKey].Values);
			lootRoomsBySegment.Add(segmentLootRooms);
			segmentCount++;
			foreach (Vector2 roomCoord in prefabMap[segmentKey].Keys)
			{
				EncounterRoom room=prefabMap[segmentKey][roomCoord];
				encounterMap.Add (roomCoord,room);
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
						//segmentRoomsEligibleForEnemyPlacement[segmentKey].Add(room);
						if (!room.isExit && Random.value<barricadeChance) room.canBarricade=true;//barricadeInRoom=new Barricade(); 
					}
					if (room.hasLoot) 
					{
						room.hasLoot=false;
						segmentLootRooms.Add(room);//roomsEligibleForLootPlacement.Add (room); 
					}
				}
			}
		}
		foreach (EncounterRoom room in nonSegmentRooms)
		{
			encounterMap.Add (room.GetCoords(),room);
			//find coordinate range within map
			minX=Mathf.Min(minX,room.xCoord);
			minY=Mathf.Min(minY,room.yCoord);
			maxX=Mathf.Max(maxX,room.xCoord);
			maxY=Mathf.Max(maxY,room.yCoord);
			//Non-segment rooms cannot be eligible for enemy or treasure placement
		}
		
		//Generate loot placement
		foreach (List<EncounterRoom> eligibleSegmentRooms in lootRoomsBySegment)
		{
			int requiredChestCount=Random.Range(normalChestCountMin,normalChestCountMax+1);
			int currentChestCount=0;
			while (currentChestCount<requiredChestCount && eligibleSegmentRooms.Count>0)
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
				InventoryItem.LootMetatypes chestType=GetChestType(chestTypes);
				EncounterRoom randomlySelectedRoom=eligibleSegmentRooms[Random.Range(0,eligibleSegmentRooms.Count)];
				foreach (InventoryItem lootItem in InventoryItem.GenerateLootSet(chestType)) randomlySelectedRoom.AddLootItem(lootItem);
				currentChestCount++;
				eligibleSegmentRooms.Remove(randomlySelectedRoom);//
				//}
			}
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
		//Disabled for the new system
		/*
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
		}*/
		
	}
	//Called after an encounter finishes to reshuffle enemies, respawn dead enemies and simulate the passing of time
	/*
	public void RandomizeEnemyPositions()
	{
		List<EncounterRoom> emptyRooms=new List<EncounterRoom>();
		List<EncounterRoom> roomsWithEnemies=new List<EncounterRoom>();
		
		EncounterRoom entranceRoom=null;
		foreach (EncounterRoom room in encounterMap.Values)
		{
			if (room.isEntrance) entranceRoom=room;
			if (room.hasEnemies) 
			{
				roomsWithEnemies.Add(room);
			}
			else 
			{
				if (!room.isWall) {emptyRooms.Add(room);}
			}
		}
		
		//Remove rooms that are too close to the entrance from consideration
		foreach (EncounterRoom emptyRoom in new List<EncounterRoom>(emptyRooms))
		{
			if (Mathf.Abs(emptyRoom.xCoord-entranceRoom.xCoord)+Mathf.Abs(emptyRoom.yCoord-entranceRoom.yCoord)<=safeDistanceFromEntrance)
			{
				emptyRooms.Remove(emptyRoom);
			}
		}
		//Reshuffle enemies around different rooms
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
			if (Mathf.Abs(startRoom.xCoord-entranceRoom.xCoord)+Mathf.Abs(startRoom.yCoord-entranceRoom.yCoord)>safeDistanceFromEntrance)
			{
				emptyRooms.Add(startRoom);
			}
			emptyRooms.Remove(endRoom);
		}
		//Determine the number of additional enemies required
		int currentEnemyCount=0;
		int desiredEnemyCount=
		Mathf.RoundToInt(emptyRooms.Count*EncounterEnemy.GetEnemyCountModifier(encounterEnemyType)*baselineEnemyPerRoomRatio);
		
		//Respawn missing enemies
		for (int i=currentEnemyCount; (i<desiredEnemyCount && emptyRooms.Count>0);i++)
		{
			EncounterRoom randomlySelectedRoom=emptyRooms[Random.Range(0,emptyRooms.Count)];
			randomlySelectedRoom.GenerateEnemy(encounterEnemyType);
			emptyRooms.Remove(randomlySelectedRoom);
		}
		
	}*/
}
/*
public class RandomAttack:Encounter
{
	public RandomAttack(EncounterEnemy.EnemyTypes enemyType):base(0)
	{
		encounterAreaType=AreaTypes.Horde;
		possibleLootTypes=Encounter.GetLootTypesList(encounterAreaType);
		encounterEnemyType=enemyType; enemyDescription=EncounterEnemy.GetMapDescription(enemyType);
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.GenerateEncounterMap(this,encounterAreaType),1f);
	}
}*/
/*
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
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.GenerateEncounterMap(this,encounterAreaType),1.5f);
		foreach (EncounterRoom room in encounterMap.Values) 
		{if (room.hasEnemies) {enemyCount++;}}
	}
}
*/
/*
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
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.GenerateEncounterMap(this,encounterAreaType),1.5f);
		//assign enemy count
		foreach (EncounterRoom room in encounterMap.Values) 
		{if (room.hasEnemies) {enemyCount++;}}
	}

}*/
