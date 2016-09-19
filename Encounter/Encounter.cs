using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Encounter 
{
	//NEW STUFF
	protected List<System.Type> enemyTypes=new List<System.Type>();
	protected List<RoomCard> rooms=new List<RoomCard>();
	protected Deck<RewardCard> rewards = new Deck<RewardCard>();

	protected RoomCard lastRoom;
	protected RewardCard[] missionEndRewards;

	protected bool finished = false;
	int roomsToGo = 2;

	public enum EncounterTypes {Wreckage,Ruins};
	public EncounterTypes encounterType;

	public enum Difficulty {Normal, Hard, Very_Hard};
	protected Difficulty difficulty;

	static List<System.Type> GetEnemyTypesForEncounterType(EncounterTypes typeArgument)
	{
		List<System.Type> enemyTypesList = new List<System.Type>();
		if (typeArgument == EncounterTypes.Wreckage)
		{
			//enemyTypesList.Add(typeof(Bugzilla));
			//enemyTypesList.Add(typeof(Skitter));
			//enemyTypesList.Add(typeof(Stinger));
			//enemyTypesList.Add(typeof(Hardshell));
			enemyTypesList.Add(typeof(Puffer));
		}
		if (typeArgument == EncounterTypes.Ruins)
		{
			enemyTypesList.Add(typeof(Puffer));
			//enemyTypesList.Add(typeof(ShockTrooper));
			//enemyTypesList.Add(typeof(Commander));
			//enemyTypesList.Add(typeof(HeavyGunner));
			//enemyTypesList.Add(typeof(Marksman));
		}

		return enemyTypesList;
	}

	public Encounter() { }

	public Encounter(Difficulty difficulty)
	{
		this.difficulty = difficulty;
		PickRandomEncounterType();
		GenerateEncounter();
	}

	void PickRandomEncounterType()
	{
		if (Random.value < 0.5f)
			encounterType = EncounterTypes.Wreckage;
		else
			encounterType = EncounterTypes.Ruins;
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
		lastRoom = RoomCard.GetRoomCardByType(randomLastRoomType,this);

		int requiredRoomsCount = Mathf.Min(3, possibleRoomTypes.Count);
		for (int i = 0; i < requiredRoomsCount; i++)
		{
			System.Type randomRoomCardType = possibleRoomTypes[Random.Range(0, possibleRoomTypes.Count)];
			rooms.Add(RoomCard.GetRoomCardByType(randomRoomCardType,this));
			rooms.Add(RoomCard.GetRoomCardByType(randomRoomCardType,this));
			possibleRoomTypes.Remove(randomRoomCardType);
		}
	}

	protected virtual void GenerateRewards()
	{
		rewards.AddCards(new CashStash(),new CashVault());
		rewards.Shuffle();

		float diceRoll = Random.value;

		missionEndRewards = new RewardCard[2];
		if (encounterType == EncounterTypes.Ruins)
		{
			if (diceRoll <= 1)
			{
				missionEndRewards[0] = new IncomeReward();
				missionEndRewards[1] = new IncomeReward();
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
			System.Type randomEnemyType = enemyTypes[Random.Range(0, enemyTypes.Count)];

			EncounterEnemy.PowerLevel enemyPowerLevel = EncounterEnemy.PowerLevel.Normal;
			if (difficulty == Difficulty.Normal)
				enemyPowerLevel = EncounterEnemy.PowerLevel.Normal;
			if (difficulty == Difficulty.Hard)
				enemyPowerLevel = EncounterEnemy.PowerLevel.Tough;

			//!!
			//randomEnemyType = typeof(Bugzilla);
			//!!

			resultList[i] = EncounterEnemy.CreateEnemyOfSetPowerLevel(randomEnemyType, enemyPowerLevel);
		}
		return resultList;
	}

	public bool IsFinished()
	{
		return finished;
	}

	public RoomCard[] GetRoomSelection(int selectCount)
	{
		if (rooms.Count > 0 && roomsToGo>0)
		{
			int modifiedCount = Mathf.Min(selectCount, rooms.Count);
			RoomCard[] roomsToSelectFrom = new RoomCard[modifiedCount];
			for (int i = 0; i < modifiedCount; i++)
			{
				int randomRoomIndex = Random.Range(0,rooms.Count);
				roomsToSelectFrom[i] = rooms[randomRoomIndex];
				rooms.RemoveAt(randomRoomIndex);
			}
			roomsToGo--;
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
			RewardCard card;
			if (rewards.DrawCard(out card))
				rewardSelection[i] = card;
			else
				throw new System.Exception("Could not find a reward card to draw from reward deck!");
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
		result += "\n"+difficulty;

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