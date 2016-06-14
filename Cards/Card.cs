using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Card
{
	public string name;
	public Sprite image;
	public string description;
}

public abstract class EncounterCard : Card
{
	//public Deck<RoomCard> possibleRoomCards=new Deck<RoomCard>();
	protected List<RoomCard> possibleRoomCards = new List<RoomCard>();
	public RoomCard[] GetRoomCards()
	{
		return possibleRoomCards.ToArray();
	}
}

public class EngineRoom : EncounterCard
{
	public EngineRoom()
	{
		name = "Engine Room";
		image = SpriteBase.mainSpriteBase.scrapSprite;
		description = "Gas hazards";
		possibleRoomCards.Add(new HardCover());
		possibleRoomCards.Add(new Radiation());
	}
}

public class Hallway : EncounterCard
{
	public Hallway()
	{
		name = "Hallway";
		image = SpriteBase.mainSpriteBase.skull;
		description = "Tight spaces";
		possibleRoomCards.Add(new Radiation());
	}
}

public abstract class RoomCard : Card
{
	public abstract void ActivateCard();
	public abstract void DeactivateCard();
}

public class HardCover : RoomCard
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

public class Radiation : RoomCard
{
	int damagePerTurn = 5;
	public Radiation()
	{
		name = "Radiation";
		image = SpriteBase.mainSpriteBase.skull;
		description = "Everyone takes "+damagePerTurn+" damage per turn";
	}

	void DamageAllCharactersInRoom()
	{
		CardsScreen.main.DamageAllEnemies(damagePerTurn);
		CardsScreen.main.DamageAllMercs(damagePerTurn);
	}

	public override void ActivateCard()
	{
		CardsScreen.ERoundIsOver += DamageAllCharactersInRoom;
	}
	public override void DeactivateCard()
	{
		CardsScreen.ERoundIsOver -= DamageAllCharactersInRoom;
	}
}

public abstract class RewardCard : Card
{
	public string effectDescription;
	public abstract void PlayCard();
}

public class CashStash : RewardCard
{
	int cashReward = 250;
	
	public CashStash()
	{
		name = "Cash Stash";
		description = "Stacks of Federation and Free Worlds currency, and a few Gate coins";
		image = SpriteBase.mainSpriteBase.pillsSprite;
		effectDescription="+$"+cashReward;
	}

	public override void PlayCard()
	{
		TownManager.main.money += cashReward;
	}

}

public class AmmoStash : RewardCard
{
	int ammoReward = 5;
	public AmmoStash()
	{
		name = "Ammo Stash";
		image = SpriteBase.mainSpriteBase.ammoBoxSprite;
		description = "Neatly stacked boxes full of powder bullets";
		effectDescription="+"+ammoReward+" ammo";
	}

	public override void PlayCard()
	{
		foreach (CharacterGraphic merc in CardsScreen.main.mercGraphics) 
		{
			merc.IncrementAmmo(ammoReward);
		}
	}
}

public abstract class CombatCard: Card
{
	public int healthDamage=0;
	public int staminaDamage=0;

	public int ammoCost=0;
	public int staminaCost=0;

	public enum TargetType { None, Select,Weakest,Strongest,All, Card };
	public TargetType targetType=TargetType.None;

	public enum CardType {Melee_Attack,Ranged_Attack,Effect};
	public CardType cardType=CardType.Melee_Attack;

	//public CharacterGraphic targetCharGraphic;
	public List<CharacterGraphic> targetChars=new List<CharacterGraphic>();
	public CharacterGraphic userCharGraphic;

	public Deck<CombatCard> originDeck;

	public virtual void PlayCard()
	{
		userCharGraphic.IncrementAmmo(-ammoCost);
		userCharGraphic.IncrementStamina(-staminaCost);
		if (targetType != TargetType.None)
		{
			foreach (CharacterGraphic targetCharGraphic in targetChars)
			{
				targetCharGraphic.TakeHealthDamage(healthDamage);
				targetCharGraphic.IncrementStamina(-staminaDamage);
			}
			targetChars.Clear();
		}
	}

	public CombatCard(Deck<CombatCard> deck) { originDeck = deck; }

	public static CombatCard[] GetMultipleCards(System.Type cardType ,Deck<CombatCard> originDeck, int cardCount)
	{
		CombatCard[] cards = new CombatCard[cardCount];
		for (int i = 0; i < cardCount; i++) cards[i] = (CombatCard)System.Activator.CreateInstance(cardType, originDeck);
		return cards;
	}
}

public class RangedCard : CombatCard
{
	public RangedCard(Deck<CombatCard> deck)
		: base(deck)
	{
		cardType = CardType.Ranged_Attack;
	}
}

public class FullAuto : RangedCard
{
	public FullAuto(Deck<CombatCard> deck):base(deck)
	{
		name = "Full Auto";
		description = "Targets all enemies";
		image=SpriteBase.mainSpriteBase.assaultRifleSprite;

		healthDamage = 10;
		staminaCost = 0;
		ammoCost = 4;
		targetType = TargetType.All;
	}
}

public class Hipfire : RangedCard
{
	public Hipfire(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Hipfire";
		description = "Aiming is for nerds";
		image = SpriteBase.mainSpriteBase.crosshair;

		healthDamage = 10;
		staminaCost = 0;
		ammoCost = 1;
		targetType = TargetType.Select;
	}

}

public class Doubletap : RangedCard
{
	public Doubletap(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Double Tap";
		description = "Always";
		image = SpriteBase.mainSpriteBase.crosshair;

		healthDamage = 20;
		staminaCost = 0;
		ammoCost = 2;
		targetType = TargetType.Select;
	}

}

public class MeleeCard : CombatCard
{
	public MeleeCard(Deck<CombatCard> deck)
		: base(deck)
	{
		cardType = CardType.Melee_Attack;
	}
}

public class Smash : MeleeCard
{
	public Smash(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Smash";
		description = "When in doubt - punch things";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;

		healthDamage = 10;
		staminaCost = 2;
		targetType = TargetType.Select;
	}

}

public class Jab : MeleeCard
{
	public Jab(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Jab";
		description = "Left hand is enough";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;

		healthDamage = 5;
		staminaCost = 0;
		targetType = TargetType.Select;
	}

}

public class Roundhouse : MeleeCard
{
	public Roundhouse(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Roundhouse kick";
		description = "Badass";
		image = SpriteBase.mainSpriteBase.brokenLegsSprite;
		targetType = TargetType.Select;

		staminaCost = 5;
		healthDamage = 200;
	}
}

public class NoMercy : MeleeCard
{
	public NoMercy(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "No Mercy";
		description = "Targets the weakest enemy";
		image = SpriteBase.mainSpriteBase.skull;
		targetType = TargetType.Weakest;

		staminaCost = 4;
		healthDamage = 20;
	}
}

public class DonQuixote : MeleeCard
{
	public DonQuixote(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Don Quixote";
		description = "Targets the strongest enemy";
		image = SpriteBase.mainSpriteBase.skull;
		targetType = TargetType.Strongest;

		staminaCost = 4;
		healthDamage = 20;
	}
}

public class Knee : MeleeCard
{
	public Knee(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Knee";
		description = "Knock the wind out";
		image = SpriteBase.mainSpriteBase.brokenLegsSprite;
		targetType = TargetType.Select;

		staminaCost = 1;
		staminaDamage = 1;
		healthDamage = 5;
	}
}

public class Effect : CombatCard
{
	public Effect(Deck<CombatCard> deck)
		: base(deck)
	{
		cardType=CardType.Effect;
	}
}

public class Breather : Effect
{
	int staminaRestore = 2;
	public Breather(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Breather";
		description = "Back in a sec (restore " + staminaRestore + " stamina";
		image = SpriteBase.mainSpriteBase.restSprite;
		targetType = TargetType.None;
	}

	public override void PlayCard()
	{
		userCharGraphic.IncrementStamina(staminaRestore);
	}
}

public class LastStand : Effect
{
	int staminaRestore = 2;
	int healthPenalty = 5;
	public LastStand(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Last Stand";
		description = "Restore " + staminaRestore + " stamina, lose " + healthPenalty + " health";
		image = SpriteBase.mainSpriteBase.skull;
		targetType = TargetType.None;
	}

	public override void PlayCard()
	{
		userCharGraphic.IncrementStamina(staminaRestore);
		userCharGraphic.TakeHealthDamage(healthPenalty);
	}
}


