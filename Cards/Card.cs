using UnityEngine;
using System.Collections;

public abstract class Card
{
	public string name;
	public Sprite image;
	public string description;
}

public abstract class EncounterCard : Card
{

}

public class EngineRoom : EncounterCard
{
	public EngineRoom()
	{
		name = "Engine Room";
		image = SpriteBase.mainSpriteBase.scrapSprite;
		description = "Gas hazards";
	}
}

public class Hallway : EncounterCard
{
	public Hallway()
	{
		name = "Hallway";
		image = SpriteBase.mainSpriteBase.genericEnemySprite;
		description = "Tight spaces";
	}
}

public abstract class RewardCard : Card
{
	public string effectDescription;
	public abstract void PlayCard();
}

public class CashStash : RewardCard
{
	int cashReward = 500;
	
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
	int ammoReward = 10;
	public AmmoStash()
	{
		name = "Ammo Stash";
		image = SpriteBase.mainSpriteBase.ammoBoxSprite;
		description = "Neatly stacked ammo boxes full of powder bullets";
		effectDescription="+"+ammoReward+" ammo";
	}

	public override void PlayCard()
	{
		TownManager.main.money += ammoReward;
	}
}

public abstract class CombatCard: Card
{
	public int healthDamage=0;
	public int staminaDamage=0;

	public int ammoCost=0;
	public int staminaCost=0;

	public enum TargetType { None, Character, Card };
	public TargetType targetType=TargetType.None;
	public CharacterGraphic targetCharGraphic;
	public CharacterGraphic userCharGraphic;

	public Deck<CombatCard> originDeck;

	public virtual void PlayCard()
	{
		userCharGraphic.IncrementAmmo(-ammoCost);
		userCharGraphic.IncrementStamina(-staminaCost);
		if (targetType == TargetType.Character)
		{
			targetCharGraphic.TakeHealthDamage(healthDamage);
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

public class FullAuto : CombatCard
{
	public FullAuto(Deck<CombatCard> deck):base(deck)
	{
		name = "Full Auto";
		description = "Full auto blast";
		image=SpriteBase.mainSpriteBase.assaultRifleSprite;

		healthDamage = 20;
		staminaCost = 0;
		ammoCost = 3;
		targetType = TargetType.Character;
	}

}

public class Smash : CombatCard
{
	public Smash(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Smash";
		description = "When in doubt - punch things";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;

		healthDamage = 10;
		staminaCost = 2;
		targetType = TargetType.Character;
	}

}

public class Breather : CombatCard
{
	int staminaRestore = 2;
	public Breather(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Breather";
		description = "Back in a sec (restore "+staminaRestore+" stamina";
		image = SpriteBase.mainSpriteBase.restSprite;
		targetType = TargetType.None;
	}

	public override void PlayCard()
	{
		userCharGraphic.IncrementStamina(staminaRestore);
	}
}

public class Roundhouse : CombatCard
{
	public Roundhouse(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Roundhouse kick";
		description = "Badass";
		image = SpriteBase.mainSpriteBase.brokenLegsSprite;
		targetType = TargetType.Character;

		staminaCost = 4;
		healthDamage = 20;
	}
}

public class Knee : CombatCard
{
	public Knee(Deck<CombatCard> deck)
		: base(deck)
	{
		name = "Knee";
		description = "Knock the wind out";
		image = SpriteBase.mainSpriteBase.brokenLegsSprite;
		targetType = TargetType.Character;

		staminaCost = 1;
		staminaDamage = 2;
		healthDamage = 5;
	}
}
