using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Encounter 
{
	public int minX=int.MaxValue;
	public int minY=int.MaxValue;
	public int maxX=int.MinValue;
	public int maxY=int.MinValue;

	const float barricadeChance=0.35f;//0.08f;
	const float baselineEnemyPerRoomRatio=0.1f;
	readonly public int minRequiredMembers=1;
	readonly public int maxRequiredMembers=20;
	
	public string lootDescription="";
	public string enemyDescription="";
	
	public enum AreaTypes {Apartment,Warehouse,Store,Police,Hospital,Endgame,Horde};
	public AreaTypes encounterAreaType;

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
		}
		return lootTypes;
	}
	
	//public enum EnemyTypes {Flesh,Quick,Slime,Muscle,Transient,Gasser,Spindler};
	public EncounterEnemy.EnemyTypes encounterEnemyType;  
	
	public Dictionary<Vector2,EncounterRoom> encounterMap=new Dictionary<Vector2,EncounterRoom>();
	
	//empty constructor for Horde (it will always call at least one base constructor from the derived class constructor)
	public Encounter (int emptyInt) {}

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

		//encounterEnemyType=EncounterEnemy.EnemyTypes.Spindler;
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

	protected void GenerateEncounterFromPrefabMap(Dictionary<Vector2,Dictionary<Vector2,EncounterRoom>> prefabMap, List<EncounterRoom> nonSegmentRooms)
	{
		//Dictionary<Vector2,List<EncounterRoom>> segmentRoomsEligibleForEnemyPlacement=new Dictionary<Vector2,List<EncounterRoom>>();
		encounterMap.Clear();
		TurnPrefabToEncounterMap(prefabMap,nonSegmentRooms);
		AddLootToEncounterRooms(new List<EncounterRoom>(encounterMap.Values));
		AddBarricadesToEncounterRooms(new List<EncounterRoom>(encounterMap.Values));
		AddSpawnersToEncounterRooms(new List<EncounterRoom>(encounterMap.Values));
	}

	void TurnPrefabToEncounterMap(Dictionary<Vector2,Dictionary<Vector2,EncounterRoom>> prefabMap, List<EncounterRoom> nonSegmentRooms)
	{
		foreach (Vector2 segmentKey in prefabMap.Keys)
		{
			List<EncounterRoom> segmentLootRooms=new List<EncounterRoom>();
			foreach (Vector2 roomCoord in prefabMap[segmentKey].Keys)
			{
				EncounterRoom room=prefabMap[segmentKey][roomCoord];
				encounterMap.Add (roomCoord,room);
				//find coordinate range within map
				minX=Mathf.Min(minX,room.xCoord);
				minY=Mathf.Min(minY,room.yCoord);
				maxX=Mathf.Max(maxX,room.xCoord);
				maxY=Mathf.Max(maxY,room.yCoord);
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
		}
	}

	void AddBarricadesToEncounterRooms(List<EncounterRoom> allEncounterRooms)
	{
		foreach (EncounterRoom room in allEncounterRooms) {if (Random.value<barricadeChance) room.canBarricade=true;}
	}

	void AddSpawnersToEncounterRooms(List<EncounterRoom> allEncounterRooms)
	{
		int requiredSpawnerCount = 2;
		List<EncounterRoom> potentialSpawners = FindRoomsEligibleForSpawners(allEncounterRooms);
		int createdSpawnerCount=0;
		while (createdSpawnerCount < requiredSpawnerCount && potentialSpawners.Count > 0)
		{
			EncounterRoom randomlySelectedPotentialRoom=potentialSpawners[Random.Range(0,potentialSpawners.Count)];
			randomlySelectedPotentialRoom.isSpawner = true;
			potentialSpawners.Remove(randomlySelectedPotentialRoom);
			createdSpawnerCount++;
		}
		//Remove spawners from the rooms that were not selected
		foreach (EncounterRoom room in potentialSpawners) room.isSpawner = false;

	}

	List<EncounterRoom> FindRoomsEligibleForSpawners(List<EncounterRoom> allEncounterRooms)
	{
		List<EncounterRoom> potentialSpawnerRooms = new List<EncounterRoom>();
		foreach (EncounterRoom room in allEncounterRooms)
		{
			if (room.isSpawner) potentialSpawnerRooms.Add(room);
		}
		return potentialSpawnerRooms;
	}


	void AddLootToEncounterRooms(List<EncounterRoom> allEncounterRooms)
	{
		List<EncounterRoom> roomsEligibleForLoot=FindRoomsEligibleForLoot(allEncounterRooms);
		List<EncounterRoom> roomsEligibleForLockedLoot=FindRoomsEligibleForLockedLoot(allEncounterRooms);
		allEncounterRooms=RemoveAllPrefabLootFlags(allEncounterRooms);

		foreach (EncounterRoom room in roomsEligibleForLoot) room.GenerateUnlockedRoomLoot();
		int lockedLootRooms=0;
		foreach (EncounterRoom room in roomsEligibleForLockedLoot)
		{
			room.GenerateRoomLockedLoot();
			lockedLootRooms++;
			if (lockedLootRooms==maxRequiredMembers) break;
		}
	}

	List<EncounterRoom> FindRoomsEligibleForLoot(List<EncounterRoom> allEncounterRooms)
	{
		return allEncounterRooms;
	}

	List<EncounterRoom> FindRoomsEligibleForLockedLoot(List<EncounterRoom> allEncounterRooms)
	{
		List<EncounterRoom> lockedLootRooms=new List<EncounterRoom>();
		foreach (EncounterRoom room in allEncounterRooms)
		{
			if (room.hasLoot) lockedLootRooms.Add(room);
		}
		return lockedLootRooms;
	}
	List<EncounterRoom> RemoveAllPrefabLootFlags(List<EncounterRoom> allEncounterRooms)
	{
		foreach (EncounterRoom room in allEncounterRooms)
		{
			room.hasLoot=false;
		}
		return allEncounterRooms;
	}

	//NEW STUFF
	protected List<System.Type> enemyTypes=new List<System.Type>();
	protected List<RoomCard> rooms=new List<RoomCard>();
	protected Deck<RewardCard> rewards = new Deck<RewardCard>();

	protected RoomCard lastRoom;
	protected RewardCard[] missionEndRewards;

	protected bool finished = false;

	public enum EncounterTypes {Wreckage,Ruins};
	protected EncounterTypes encounterType;

	static List<System.Type> GetEnemyTypesForEncounterType(EncounterTypes typeArgument)
	{
		List<System.Type> enemyTypesList = new List<System.Type>();
		if (typeArgument == EncounterTypes.Wreckage)
		{
			enemyTypesList.Add(typeof(Bugzilla));
			enemyTypesList.Add(typeof(Skitter));
			enemyTypesList.Add(typeof(Stinger));
			enemyTypesList.Add(typeof(Hardshell));
			enemyTypesList.Add(typeof(Puffer));
		}
		if (typeArgument == EncounterTypes.Ruins)
		{
			enemyTypesList.Add(typeof(ShockTrooper));
			enemyTypesList.Add(typeof(FieldCommander));
			enemyTypesList.Add(typeof(HeavyGunner));
			enemyTypesList.Add(typeof(Marksman));
		}

		return enemyTypesList;
	}

	public Encounter(bool newKind)
	{
		if (Random.value < 0.5f)
			encounterType = EncounterTypes.Wreckage;
		else
			encounterType = EncounterTypes.Ruins;

		GenerateEncounter();
	}

	protected void GenerateEncounter()
	{
		GenerateRoomCards();
		GenerateRewards();
		GenerateEnemyTypes();
	}

	void GenerateRoomCards()
	{
		List<System.Type> possibleRoomTypes = RoomCard.GetAllPossibleRoomCards(encounterType);
		//Order is important
		System.Type randomLastRoomType = possibleRoomTypes[Random.Range(0, possibleRoomTypes.Count)];
		lastRoom = RoomCard.GetRoomCardByType(randomLastRoomType);

		int requiredRoomsCount = Mathf.Min(3, possibleRoomTypes.Count);
		for (int i = 0; i < requiredRoomsCount; i++)
		{
			System.Type randomRoomCardType = possibleRoomTypes[Random.Range(0, possibleRoomTypes.Count)];
			rooms.Add(RoomCard.GetRoomCardByType(randomRoomCardType));
			rooms.Add(RoomCard.GetRoomCardByType(randomRoomCardType));
			possibleRoomTypes.Remove(randomRoomCardType);
		}
	}

	protected virtual void GenerateRewards()
	{
		rewards.AddCards(new CashStash(), new AmmoStash(), new AmmoStash(), new ArmorStash(), new ArmorStash());
		rewards.Shuffle();

		float diceRoll = Random.value;

		missionEndRewards = new RewardCard[2];
		if (encounterType == EncounterTypes.Ruins)
		{
			if (diceRoll <= 1)
			{
				missionEndRewards[0] = new CashStash();
				missionEndRewards[1] = new CashStash();
			}
			if (diceRoll < 0.5f)
			{
				missionEndRewards[0] = new CrewReward();
				missionEndRewards[1] = new CrewReward();
			}
		}
		if (encounterType == EncounterTypes.Wreckage)
		{
			if (diceRoll <= 1)
			{
				missionEndRewards[0] = new ScrapReward();
				missionEndRewards[1] = new ScrapReward();
			}
			if (diceRoll < 0.5f)
			{
				missionEndRewards[0] = new ComputerPartsReward();
				missionEndRewards[1] = new ComputerPartsReward();
			}
		}
	}

	void GenerateEnemyTypes()
	{
		enemyTypes = GetEnemyTypesForEncounterType(encounterType);
	}

	public EncounterEnemy[] GenerateEnemies(int enemiesCount)
	{
		EncounterEnemy[] resultList = new EncounterEnemy[enemiesCount];
		for (int i = 0; i < enemiesCount; i++)
		{
			resultList[i] = (EncounterEnemy)System.Activator.CreateInstance(enemyTypes[Random.Range(0,enemyTypes.Count)]);
		}
		return resultList;
	}

	public bool IsFinished()
	{
		return finished;
	}

	public RoomCard[] GetRoomSelection(int selectCount)
	{
		if (rooms.Count > 0)
		{
			int modifiedCount = Mathf.Min(selectCount, rooms.Count);
			RoomCard[] roomsToSelectFrom = new RoomCard[modifiedCount];
			for (int i = 0; i < modifiedCount; i++)
			{
				int randomRoomIndex = Random.Range(0,rooms.Count);
				roomsToSelectFrom[i] = rooms[randomRoomIndex];
				rooms.RemoveAt(randomRoomIndex);
			}
			return roomsToSelectFrom;
		}
		else 
		{
			finished = true;
			RoomCard[] oneItemAr = new RoomCard[1];
			oneItemAr[0] = lastRoom;
			return oneItemAr;
		}
	}

	public RewardCard[] GetRewardCardSelection(int selectCount)
	{
		RewardCard[] rewardSelection=new RewardCard[selectCount];
		for (int i = 0; i < selectCount; i++)
		{
			rewardSelection[i] = rewards.DrawCard();
		}
		return rewardSelection;
	}
	public void DiscardRewardCards(params RewardCard[] discarded)
	{
		rewards.DiscardCards(discarded);
	}

	public RewardCard[] GetMissionEndRewards()
	{
		return missionEndRewards;
	}

	public virtual string GetScoutingDescription()
	{
		string result=null;
		if (encounterType == EncounterTypes.Wreckage)
		{
			result = "We've managed to locate another pristine alien ship, designation JXZ-795.";
			result += "\nNot the biggest one we've ever seen, but should contain some very useful stuff.";
			result += "\nSend in the mercs, and watch out for the infestation.";
		}
		if (encounterType == EncounterTypes.Ruins)
		{
			result = "An unidentified group of armed thugs has set up in some industrial ruins nearby.";
			result += "\nThey're probably sent by the Corp to mess with the scavengers. Or sent by the Scavs to mess with corp outposts.";
			result += "\nWhoever is pulling their strings clearly doesn't want to be associated with them, so they won't be too pissed";
			result += "\nif we clear the place out and take all the hardware for ourselves.";
		}

		if (result == null)
			throw new System.Exception("Could not get a scouting description for map node!");
		return result;
	}
	public virtual string GetTooltipDescription()
	{
		string result = null;
		if (encounterType == EncounterTypes.Wreckage)
		{
			result = "Wreckage";
		}
		if (encounterType == EncounterTypes.Ruins)
		{
			result = "Ruins";
		}

		if (result == null)
			throw new System.Exception("Could not get a tooltip description for map node!");
		return result;
	}

	//NEW STUFF END

}

public class StoryMissionOne : Encounter
{
	public StoryMissionOne()
	{
		encounterType = EncounterTypes.Wreckage;
		GenerateEncounter();
	}

	protected override void GenerateRewards()
	{
		base.GenerateRewards();
		missionEndRewards=new RewardCard[1];
		missionEndRewards[0] = new StoryRewardOne();
	}

	public override string GetTooltipDescription()
	{
		return "Fusion Generator";
	}

	public override string GetScoutingDescription()
	{
		string result;
		result = "After some searching, scouting, bribing and threatening, we finally got a lead on a working Fusion Generator.";
		result += "\nGibraltar, formerly a United Federation battlecruiser, now a pile of smashed metal.";
		result += "\nThe wreck is huge, and the infestation there has grown pretty bad. Go in, take every functioning piece of tech,";
		result += "\nAnd find that generator.";
		return result;
	}
}