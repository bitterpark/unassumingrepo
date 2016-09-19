using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class RoomCard : Card
{
	
	//public Deck<RoomCard> possibleRoomCards=new Deck<RoomCard>();
	protected List<RoomStipulationCard> possibleRoomStipulationCards = new List<RoomStipulationCard>();

	const int maxEnemyCountInRoom = 4;
	const int minEnemyCountInRoom = 3;
	int enemyCountInRoom;
	Encounter parentMission;

	public static List<System.Type> GetAllPossibleRoomCards(Encounter.EncounterTypes encounterType)
	{
		List<System.Type> possibleCards = new List<System.Type>();
		if (encounterType == Encounter.EncounterTypes.Wreckage)
		{
			possibleCards.Add(typeof(EngineRoom));
			possibleCards.Add(typeof(CoolantRoom));
			possibleCards.Add(typeof(Airlock));
			possibleCards.Add(typeof(UpperDecks));
			possibleCards.Add(typeof(LowerDecks));
		}
		if (encounterType == Encounter.EncounterTypes.Ruins)
		{
			possibleCards.Add(typeof(Courtyard));
			possibleCards.Add(typeof(Rooftop));
			possibleCards.Add(typeof(RuinedHall));
			possibleCards.Add(typeof(Armory));
			possibleCards.Add(typeof(StoreRoom));
			possibleCards.Add(typeof(Backdoor));
			possibleCards.Add(typeof(Courtyard));
			possibleCards.Add(typeof(GuardPost));
		}
		return possibleCards;
	}
	public static RoomCard GetRoomCardByType(System.Type roomCardType, Encounter parentMission)
	{
		RoomCard newRoomCard=(RoomCard)System.Activator.CreateInstance(roomCardType);
		newRoomCard.SetParentMission(parentMission);
		return newRoomCard;
	}

	public RoomCard()
	{
		enemyCountInRoom = 0;
		if (Random.value < 0.25f)
			enemyCountInRoom = minEnemyCountInRoom;
		else
			enemyCountInRoom = maxEnemyCountInRoom;
		ExtenderConstructor();
	}

	public void SetParentMission(Encounter parentMission)
	{
		this.parentMission = parentMission;
	}

	protected virtual void ExtenderConstructor()
	{

	}

	public EncounterEnemy[] GetEnemies()
	{
		return parentMission.GenerateEnemies(GetEnemyCount());
	}


	public int GetEnemyCount()
	{
		return enemyCountInRoom;
	}

	public bool TryGetRespectiveStipulationCard(out RoomStipulationCard card)
	{
		if (possibleRoomStipulationCards.Count > 0)
		{
			card = possibleRoomStipulationCards[0];
			return true;
		}
		else
		{
			card = null;
			return false;
		}
	}

	public RoomStipulationCard[] GetAllRoomStipulationCards()
	{
		return possibleRoomStipulationCards.ToArray();
	}
	public List<RoomStipulationCard> GetRandomRoomStipulationCards(int cardNumber)
	{
		List<RoomStipulationCard> resultList=new List<RoomStipulationCard>();
		List<RoomStipulationCard> selectionBufferList=new List<RoomStipulationCard>(possibleRoomStipulationCards);
		while (resultList.Count < cardNumber && selectionBufferList.Count > 0)
		{
			int randomCardIndex=Random.Range(0,selectionBufferList.Count);
			resultList.Add(selectionBufferList[randomCardIndex]);
			selectionBufferList.RemoveAt(randomCardIndex);
		}
		
		return possibleRoomStipulationCards;
	}
}

public abstract class StipulationCard : Card
{
	
}

public abstract class CharacterStipulationCard : StipulationCard
{
	public CharacterGraphic appliedToCharacter;
	public List<CombatCard> addedCombatCards=new List<CombatCard>();

	bool lastsIndefinitely = true;
	int maxRoundsActive;
	int currentRoundsActive;
	//CharacterGraphic countRoundsFromCharacter;

	protected void SetLastsForRounds(int rounds)
	{
		lastsIndefinitely = false;
		maxRoundsActive = rounds;
	}

	public void ActivateCard(CharacterGraphic applyCardTo)
	{
		if (!lastsIndefinitely)
		{
			CombatManager.ERoundIsOver += RoundOver;
			currentRoundsActive = 0;
		}
		appliedToCharacter = applyCardTo;

		if (addedCombatCards.Count > 0)
			appliedToCharacter.AddCardsToCurrentDeck(addedCombatCards.ToArray());
		ExtenderSpecificActivation();
	}

	void RoundOver()
	{
		currentRoundsActive++;
		if (currentRoundsActive >= maxRoundsActive)
			appliedToCharacter.RemoveCharacterStipulationCard(this);
	}

	protected virtual void ExtenderSpecificActivation() { }

	public void DeactivateCard()
	{
		if (!lastsIndefinitely)
			CombatManager.ERoundIsOver -= RoundOver;

		if (addedCombatCards.Count > 0)
			appliedToCharacter.RemoveCardsFromCurrentDeck(addedCombatCards.ToArray());

		ExtenderSpecificDeactivation();
	}

	protected virtual void ExtenderSpecificDeactivation() { }

	
}

public abstract class EnemyVariationCard : CharacterStipulationCard
{
	public void AddEnemysBasicCombatDeck(CombatCard[] basicDeckCards)
	{
		addedCombatCards.AddRange(basicDeckCards);
	}
}

public abstract class RoomStipulationCard : StipulationCard
{
	public abstract void ActivateCard();
	public abstract void DeactivateCard();
}


//Used by enemies
public class TripmineEnemy : RoomStipulationCard
{
	int damage;

	public TripmineEnemy(int assignedDamage)
	{
		name = "Booby Trap";
		image = SpriteBase.mainSpriteBase.bomb;

		damage = assignedDamage;
		description = "When any friendly character plays a melee attack, they take " + damage + " damage and this card is removed";
		
		
	}

	public override void ActivateCard()
	{
		MeleeCard.EMeleeCardPlayed += Detonate;
	}
	void Detonate(CharacterGraphic cardPlayer, MeleeCard playedCard)
	{
		if (cardPlayer.GetType() == typeof(MercGraphic))
		{
			CombatManager.main.RemoveRoomStipulationCard(this);
			cardPlayer.TakeDamage(damage);
		}

	}
	public override void DeactivateCard()
	{
		MeleeCard.EMeleeCardPlayed -= Detonate;
	}
}

//Used by friendly mercs
public class TripmineFriendly : RoomStipulationCard
{
	int damage;

	public TripmineFriendly(int assignedDamage)
	{
		damage = assignedDamage;
		name = "Trip Mine";
		image = SpriteBase.mainSpriteBase.bomb;
		description = "Melee block, attacker takes "+damage+" damage";
	}

	public override void ActivateCard()
	{
		MeleeCard.EMeleeCardPlayed += Detonate;
	}
	void Detonate(CharacterGraphic cardPlayer, MeleeCard playedCard)
	{
		if (cardPlayer.GetType() != typeof(MercGraphic))
		{
			CombatManager.main.RemoveRoomStipulationCard(this);
			cardPlayer.TakeDamage(damage);
		}
	}
	public override void DeactivateCard()
	{
		MeleeCard.EMeleeCardPlayed -= Detonate;
	}
}



//SHIP ROOMS
public class EngineRoom : RoomCard
{
	protected override void ExtenderConstructor()
	{
		name = "Engine Room";
		image = SpriteBase.mainSpriteBase.wrench;
		description = "Radiation leaks";
		possibleRoomStipulationCards.Add(new Radiation());
	}

	public class Radiation : RoomStipulationCard
	{
		int damagePerTurn = 5;
		public Radiation()
		{
			name = "Radiation";
			image = SpriteBase.mainSpriteBase.skull;
			description = "Everyone loses " + damagePerTurn + " health per round";
		}

		public override void ActivateCard()
		{
			CombatManager.ERoundIsOver += DamageAllCharactersInRoom;
		}
		void DamageAllCharactersInRoom()
		{
			MissionCharacterManager.main.DamageAllMercs(damagePerTurn, true);
			MissionCharacterManager.main.DamageAllEnemies(damagePerTurn, true);
		}
		public override void DeactivateCard()
		{
			CombatManager.ERoundIsOver -= DamageAllCharactersInRoom;
		}
	}
}

public class CoolantRoom : RoomCard
{
	protected override void ExtenderConstructor()
	{
		name = "Coolant Room";
		image = SpriteBase.mainSpriteBase.snowflake;
		description = "Freezing hazards";
		possibleRoomStipulationCards.Add(new Freeze());
	}

	public class Freeze : RoomStipulationCard
	{
		int staminaReduction = 4;

		public Freeze()
		{
			name = "Freeze";
			image = SpriteBase.mainSpriteBase.snowflake;
			description = "Starting stamina reduced by " + staminaReduction + " for everyone";
		}

		public override void ActivateCard()
		{
			ReduceStaminaForAll();
		}
		void ReduceStaminaForAll()
		{
			MissionCharacterManager.main.IncrementAllCharactersResource(CharacterGraphic.Resource.Stamina, -staminaReduction);
		}
		public override void DeactivateCard()
		{

		}
	}
}

public class Airlock : RoomCard
{
	protected override void ExtenderConstructor()
	{
		name = "Airlock";
		image = SpriteBase.mainSpriteBase.door;
		description = "Tight Quarters";
		possibleRoomStipulationCards.Add(new TightQuarters());
	}

	public class TightQuarters : RoomStipulationCard
	{
		int rangedAttackDamagePenalty = 30;

		public TightQuarters()
		{
			name = "Tight Quarters";
			image = SpriteBase.mainSpriteBase.cover;
			description = "Any character playing a ranged attack takes " + rangedAttackDamagePenalty + " damage";
		}

		public override void ActivateCard()
		{
			RangedCard.ERangedCardPlayed += Trigger;
		}
		void Trigger(CharacterGraphic cardPlayer, RangedCard playedCard)
		{
			//CardsScreen.main.RemoveRoomCard(this);
			cardPlayer.TakeDamage(rangedAttackDamagePenalty);
		}
		public override void DeactivateCard()
		{
			RangedCard.ERangedCardPlayed -= Trigger;
		}
	}
}

public class LowerDecks : RoomCard
{
	protected override void ExtenderConstructor()
	{
		name = "Lower Decks";
		image = SpriteBase.mainSpriteBase.anchor;
		description = "Filled with gas";

		possibleRoomStipulationCards.Add(new VolatileGas());
	}

	public class VolatileGas : RoomStipulationCard
	{
		int damageToEveryone = 20;

		public VolatileGas()
		{
			name = "Volatile Gas";
			image = SpriteBase.mainSpriteBase.cloud;
			description = "If a ranged attack is played, everyone takes " + damageToEveryone + " damage and this card is removed";
		}
		public override void ActivateCard()
		{
			RangedCard.ERangedCardPlayed += ExplosionEffect;
		}

		void ExplosionEffect(CharacterGraphic cardPlayer, RangedCard playedCard)
		{
			CombatManager.main.RemoveRoomStipulationCard(this);
			MissionCharacterManager.main.DamageAllCharacters(damageToEveryone);
		}

		public override void DeactivateCard()
		{
			RangedCard.ERangedCardPlayed -= ExplosionEffect;
		}
	}
}

public class UpperDecks : RoomCard
{
	protected override void ExtenderConstructor()
	{
		name = "Upper Decks";
		image = SpriteBase.mainSpriteBase.arrow;
		description = "";
	}
}

//RUINS
public class Backdoor : RoomCard
{
	protected override void ExtenderConstructor()
	{
		name = "Backdoor";
		image = SpriteBase.mainSpriteBase.lockIcon;
		description = "Usually mined";
		possibleRoomStipulationCards.Add(new Mine());
	}

	public class Mine : RoomStipulationCard
	{
		int damage = 30;

		public Mine()
		{
			name = "Mine";
			image = SpriteBase.mainSpriteBase.bomb;
			description = "When any character plays a melee attack, they take " + damage + " damage and this card is removed";
		}

		public override void ActivateCard()
		{
			MeleeCard.EMeleeCardPlayed += Detonate;
		}
		void Detonate(CharacterGraphic cardPlayer, MeleeCard playedCard)
		{
			CombatManager.main.RemoveRoomStipulationCard(this);
			cardPlayer.TakeDamage(damage);

		}
		public override void DeactivateCard()
		{
			MeleeCard.EMeleeCardPlayed -= Detonate;
		}
	}
}

public class GuardPost : RoomCard
{
	int mineDamage = 30;

	protected override void ExtenderConstructor()
	{
		name = "Guard Post";
		image = SpriteBase.mainSpriteBase.flag;
		description = "Complete with booby traps";
		possibleRoomStipulationCards.Add(new TripmineEnemy(mineDamage));
	}
}

public class Rooftop : RoomCard
{
	protected override void ExtenderConstructor()
	{
		name = "Rooftop";
		image = SpriteBase.mainSpriteBase.cloud;
		description = "Little to no cover";
		possibleRoomStipulationCards.Add(new NoCover());
	}

	public class NoCover : RoomStipulationCard
	{
		int armorPenalty = 30;

		public NoCover()
		{
			name = "No Cover";
			image = SpriteBase.mainSpriteBase.roseOfWinds;
			description = "Everyone starts with -" + armorPenalty + " armor";
		}

		public override void ActivateCard()
		{
			DecreaseArmorForAll();
		}
		void DecreaseArmorForAll()
		{
			MissionCharacterManager.main.IncrementAllCharactersResource(CharacterGraphic.Resource.Armor, -armorPenalty);
		}
		public override void DeactivateCard()
		{

		}
	}
}

public class Armory : RoomCard
{
	protected override void ExtenderConstructor()
	{
		name = "Armory";
		image = SpriteBase.mainSpriteBase.armor;
		description = "Volatile materials";
		possibleRoomStipulationCards.Add(new ExplosiveBarrel());
	}

	public class ExplosiveBarrel : RoomStipulationCard
	{
		int damageToOpposingTeam = 20;

		public ExplosiveBarrel()
		{
			name = "Explosive Barrel";
			image = SpriteBase.mainSpriteBase.fire;
			description = "If a ranged attack is played, the opposing team takes " + damageToOpposingTeam + " damage and this card is removed";
		}
		public override void ActivateCard()
		{
			RangedCard.ERangedCardPlayed += ExplosionEffect;
		}

		void ExplosionEffect(CharacterGraphic cardPlayer, RangedCard playedCard)
		{
			CombatManager.main.RemoveRoomStipulationCard(this);
			MissionCharacterManager.main.DamageOpposingTeam(damageToOpposingTeam);
		}

		public override void DeactivateCard()
		{
			RangedCard.ERangedCardPlayed -= ExplosionEffect;
		}
	}
}

public class StoreRoom : RoomCard
{
	protected override void ExtenderConstructor()
	{
		name = "Storage Room";
		image = SpriteBase.mainSpriteBase.sack;
		description = "Shelves provide heavy cover";
		possibleRoomStipulationCards.Add(new HardCover());
	}

	public class HardCover : RoomStipulationCard
	{
		public HardCover()
		{
			name = "Hard Cover";
			image = SpriteBase.mainSpriteBase.cover;
			description = "Ranged attacks cannot be played";
		}

		public override void ActivateCard()
		{
			CombatManager.rulesHandler.SetRangedAttacksRestriction(false);
		}
		public override void DeactivateCard()
		{
			CombatManager.rulesHandler.SetRangedAttacksRestriction(true);
		}
	}

}

public class RuinedHall : RoomCard
{
	protected override void ExtenderConstructor()
	{
		name = "Ruined Hall";
		image = SpriteBase.mainSpriteBase.rock;
		description = "Crumbled architecture provides light cover";
		possibleRoomStipulationCards.Add(new LightCover());
	}

	public class LightCover : RoomStipulationCard
	{
		int armorBonus = 30;

		public LightCover()
		{
			name = "Light Cover";
			image = SpriteBase.mainSpriteBase.cover;
			description = "Everyone starts with +" + armorBonus + " armor";
		}

		public override void ActivateCard()
		{
			IncreaseArmorForAll();
		}
		void IncreaseArmorForAll()
		{
			MissionCharacterManager.main.IncrementAllCharactersResource(CharacterGraphic.Resource.Armor, armorBonus);
		}
		public override void DeactivateCard()
		{

		}
	}
}

public class Courtyard : RoomCard
{
	protected override void ExtenderConstructor()
	{
		name = "Courtyard";
		image = SpriteBase.mainSpriteBase.roseOfWinds;
		description = "Open space for snipers";
		possibleRoomStipulationCards.Add(new OpenSpace());
	}

	public class OpenSpace : RoomStipulationCard
	{
		int meleeAttackDamagePenalty = 30;

		public OpenSpace()
		{
			name = "Open Space";
			image = SpriteBase.mainSpriteBase.lateralArrows;
			description = "Any character playing a melee attack takes " + meleeAttackDamagePenalty + " damage";
		}

		public override void ActivateCard()
		{
			MeleeCard.EMeleeCardPlayed += Trigger;
		}
		void Trigger(CharacterGraphic cardPlayer, MeleeCard playedCard)
		{
			//CardsScreen.main.RemoveRoomCard(this);
			cardPlayer.TakeDamage(meleeAttackDamagePenalty);
		}
		public override void DeactivateCard()
		{
			MeleeCard.EMeleeCardPlayed -= Trigger;
		}
	}
}
