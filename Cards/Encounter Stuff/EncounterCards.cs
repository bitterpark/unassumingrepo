using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class RoomCard : Card
{
	//public Deck<RoomCard> possibleRoomCards=new Deck<RoomCard>();
	protected List<RoomStipulationCard> possibleRoomCards = new List<RoomStipulationCard>();
	public RoomStipulationCard[] GetRoomCards()
	{
		return possibleRoomCards.ToArray();
	}
	public List<RoomStipulationCard> GetRandomRoomStipulationCards(int cardNumber)
	{
		List<RoomStipulationCard> resultList=new List<RoomStipulationCard>();
		List<RoomStipulationCard> selectionBufferList=new List<RoomStipulationCard>(possibleRoomCards);
		while (resultList.Count < cardNumber && selectionBufferList.Count > 0)
		{
			int randomCardIndex=Random.Range(0,selectionBufferList.Count);
			resultList.Add(selectionBufferList[randomCardIndex]);
			selectionBufferList.RemoveAt(randomCardIndex);
		}
		
		return possibleRoomCards;
	}

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
	public static RoomCard GetRoomCardByType(System.Type roomCardType)
	{
		return (RoomCard)System.Activator.CreateInstance(roomCardType);
	}
}

public abstract class StipulationCard : Card
{
	
}

public abstract class CharacterStipulationCard : StipulationCard
{
	public abstract void ActivateCard(CharacterGraphic user);
	public abstract void DeactivateCard();

	public CharacterGraphic characterGraphic;
}

public class MeleeDefence :CharacterStipulationCard
{
	int damage=20;

	public MeleeDefence()
	{
		name = "Melee Defence";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;

		description = "When a melee attack is played, the attacker takes "+damage+" damage";
	}

	public override void ActivateCard(CharacterGraphic user)
	{
		characterGraphic = user;
		MeleeCard.EMeleeCardPlayed += Detonate;
	}
	void Detonate(CharacterGraphic cardPlayer)
	{
		if (cardPlayer.GetType() != typeof(MercGraphic))
		{
			//characterGraphic.RemoveCharacterCard(this);
			cardPlayer.TakeDamage(damage);
		}

	}
	public override void DeactivateCard()
	{
		MeleeCard.EMeleeCardPlayed -= Detonate;
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
	void Detonate(CharacterGraphic cardPlayer)
	{
		if (cardPlayer.GetType() == typeof(MercGraphic))
		{
			CardsScreen.main.RemoveRoomCard(this);
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
		description = "When any enemy character plays a melee attack, they take " + damage + " damage and this card is removed";
	}

	public override void ActivateCard()
	{
		MeleeCard.EMeleeCardPlayed += Detonate;
	}
	void Detonate(CharacterGraphic cardPlayer)
	{
		if (cardPlayer.GetType() != typeof(MercGraphic))
		{
			CardsScreen.main.RemoveRoomCard(this);
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
	public EngineRoom()
	{
		name = "Engine Room";
		image = SpriteBase.mainSpriteBase.wrench;
		description = "Radiation leaks";
		possibleRoomCards.Add(new Radiation());
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
			CardsScreen.ERoundIsOver += DamageAllCharactersInRoom;
		}
		void DamageAllCharactersInRoom()
		{
			CardsScreen.main.DamageAllMercs(damagePerTurn, true);
			CardsScreen.main.DamageAllEnemies(damagePerTurn, true);
		}
		public override void DeactivateCard()
		{
			CardsScreen.ERoundIsOver -= DamageAllCharactersInRoom;
		}
	}
}

public class CoolantRoom : RoomCard
{
	public CoolantRoom()
	{
		name = "Coolant Room";
		image = SpriteBase.mainSpriteBase.snowflake;
		description = "Freezing hazards";
		possibleRoomCards.Add(new Freeze());
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
			CardsScreen.main.IncrementAllCharactersResource(CharacterGraphic.Resource.Stamina, -staminaReduction);
		}
		public override void DeactivateCard()
		{

		}
	}
}

public class Airlock : RoomCard
{
	public Airlock()
	{
		name = "Airlock";
		image = SpriteBase.mainSpriteBase.door;
		description = "Tight Quarters";
		possibleRoomCards.Add(new TightQuarters());
	}

	public class TightQuarters : RoomStipulationCard
	{
		int rangedAttackDamagePenalty = 15;

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
		void Trigger(CharacterGraphic cardPlayer)
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
	public LowerDecks()
	{
		name = "Lower Decks";
		image = SpriteBase.mainSpriteBase.anchor;
		description = "Filled with gas";

		possibleRoomCards.Add(new VolatileGas());
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

		void ExplosionEffect(CharacterGraphic cardPlayer)
		{
			CardsScreen.main.RemoveRoomCard(this);
			CardsScreen.main.DamageAllCharacters(damageToEveryone);
		}

		public override void DeactivateCard()
		{
			RangedCard.ERangedCardPlayed -= ExplosionEffect;
		}
	}
}

public class UpperDecks : RoomCard
{
	public UpperDecks()
	{
		name = "Upper Decks";
		image = SpriteBase.mainSpriteBase.arrow;
		description = "";
	}
}

//RUINS
public class Backdoor : RoomCard
{
	public Backdoor()
	{
		name = "Backdoor";
		image = SpriteBase.mainSpriteBase.lockIcon;
		description = "Usually mined";
		possibleRoomCards.Add(new Mine());
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
		void Detonate(CharacterGraphic cardPlayer)
		{
			CardsScreen.main.RemoveRoomCard(this);
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
	
	public GuardPost()
	{
		name = "Guard Post";
		image = SpriteBase.mainSpriteBase.flag;
		description = "Complete with booby traps";
		possibleRoomCards.Add(new TripmineEnemy(mineDamage));
	}
}

public class Rooftop : RoomCard
{
	public Rooftop()
	{
		name = "Rooftop";
		image = SpriteBase.mainSpriteBase.cloud;
		description = "Little to no cover";
		possibleRoomCards.Add(new NoCover());
	}

	public class NoCover : RoomStipulationCard
	{
		int armorPenalty = 15;

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
			CardsScreen.main.IncrementAllCharactersResource(CharacterGraphic.Resource.Armor, -armorPenalty);
		}
		public override void DeactivateCard()
		{

		}
	}
}

public class Armory : RoomCard
{
	public Armory()
	{
		name = "Armory";
		image = SpriteBase.mainSpriteBase.armor;
		description = "Volatile materials";
		possibleRoomCards.Add(new ExplosiveBarrel());
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

		void ExplosionEffect(CharacterGraphic cardPlayer)
		{
			CardsScreen.main.RemoveRoomCard(this);
			CardsScreen.main.DamageOpposingTeam(damageToOpposingTeam);
		}

		public override void DeactivateCard()
		{
			RangedCard.ERangedCardPlayed -= ExplosionEffect;
		}
	}
}

public class StoreRoom : RoomCard
{
	public StoreRoom()
	{
		name = "Storage Room";
		image = SpriteBase.mainSpriteBase.sack;
		description = "Shelves provide heavy cover";
		possibleRoomCards.Add(new HardCover());
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
			CardsScreen.main.SetRangedAttacksRestriction(false);
		}
		public override void DeactivateCard()
		{
			CardsScreen.main.SetRangedAttacksRestriction(true);
		}
	}

}

public class RuinedHall : RoomCard
{
	public RuinedHall()
	{
		name = "Ruined Hall";
		image = SpriteBase.mainSpriteBase.rock;
		description = "Crumbled architecture provides light cover";
		possibleRoomCards.Add(new LightCover());
	}

	public class LightCover : RoomStipulationCard
	{
		int armorBonus = 15;

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
			CardsScreen.main.IncrementAllCharactersResource(CharacterGraphic.Resource.Armor, armorBonus);
		}
		public override void DeactivateCard()
		{

		}
	}
}

public class Courtyard : RoomCard
{
	public Courtyard()
	{
		name = "Courtyard";
		image = SpriteBase.mainSpriteBase.roseOfWinds;
		description = "Open space for snipers";
		possibleRoomCards.Add(new OpenSpace());
	}

	public class OpenSpace : RoomStipulationCard
	{
		int meleeAttackDamagePenalty = 15;

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
		void Trigger(CharacterGraphic cardPlayer)
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
