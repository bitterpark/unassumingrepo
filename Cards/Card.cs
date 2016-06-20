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
		//possibleRoomCards.Add(new HardCover());
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
		CardsScreen.main.DamageAllMercs(damagePerTurn,true);
		CardsScreen.main.DamageAllEnemies(damagePerTurn, true);
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
	public virtual void PlayCard()
	{
		foreach (InventoryItem item in rewardItems)
		{
			MapManager.main.GetTown().StashItem(item);
		}
	}
	public List<InventoryItem> rewardItems = new List<InventoryItem>();

	protected string FormItemRewardDescription()
	{
		return "Gain";
	}
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
		effectDescription=FormItemRewardDescription();
		rewardItems.Add(new AmmoBox());
	}
}

public abstract class CombatCard: Card
{
	public int healthDamage=0;
	public int staminaDamage=0;

	public int ammoCost=0;
	public int staminaCost=0;

	public enum TargetType { None, Select,Weakest,Strongest,Random,All, Card };
	public TargetType targetType=TargetType.None;

	public enum CardType {Melee_Attack,Ranged_Attack,Effect};
	public CardType cardType=CardType.Melee_Attack;

	//public CharacterGraphic targetCharGraphic;
	public List<CharacterGraphic> targetChars=new List<CharacterGraphic>();
	public CharacterGraphic userCharGraphic;

	public Deck<CombatCard> originDeck;

	public virtual void PlayCard()
	{
		CardPlayEffects();
		targetChars.Clear();
		userCharGraphic = null;
	}

	protected virtual void CardPlayEffects()
	{
		userCharGraphic.IncrementAmmo(-ammoCost);
		userCharGraphic.IncrementStamina(-staminaCost);
		if (targetType != TargetType.None)
		{
			foreach (CharacterGraphic targetCharGraphic in targetChars)
			{
				targetCharGraphic.TakeDamage(healthDamage);
				targetCharGraphic.IncrementStamina(-staminaDamage);
			}
		}
	}


	//public CombatCard() { originDeck = deck; }
	public void SetOriginDeck(Deck<CombatCard> deck)
	{
		originDeck=deck;
	}

	public static CombatCard[] GetMultipleCards(System.Type cardType, int cardCount)
	{
		CombatCard[] cards = new CombatCard[cardCount];
		for (int i = 0; i < cardCount; i++) cards[i] = (CombatCard)System.Activator.CreateInstance(cardType);
		return cards;
	}
}

public class RangedCard : CombatCard
{
	public RangedCard()
		: base()
	{
		cardType = CardType.Ranged_Attack;
	}
}

public class FullAuto : RangedCard
{
	public FullAuto(): base()
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
	public Hipfire()
		: base()
	{
		name = "Hipfire";
		description = "Aiming is for nerds";
		image = SpriteBase.mainSpriteBase.bulletSprite;

		healthDamage = 10;
		staminaCost = 0;
		ammoCost = 1;
		targetType = TargetType.Select;
	}
}

public class MeleeCard : CombatCard
{
	public MeleeCard()
		: base()
	{
		cardType = CardType.Melee_Attack;
	}
}

public class Effect : CombatCard
{
	public Effect()
		: base()
	{
		cardType=CardType.Effect;
	}
}

public class Breather : Effect
{
	int staminaRestore = 2;
	public Breather()
		: base()
	{
		name = "Breather";
		description = "Back in a sec (restore " + staminaRestore + " stamina)";
		image = SpriteBase.mainSpriteBase.restSprite;
		targetType = TargetType.None;
	}


	protected override void CardPlayEffects()
	{
		userCharGraphic.IncrementStamina(staminaRestore);
	}
}

//SOLDIER (TANK) CARDS

public class TakeCover : Effect
{
	int armorGain = 10;
	public TakeCover()
		: base()
	{
		name = "Take Cover";
		description = "Hit the deck (gain " + armorGain + " armor)";
		image = SpriteBase.mainSpriteBase.cover;
		targetType = TargetType.None;
		staminaCost = 3;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.IncrementArmor(armorGain);
	}
}

public class Reposition : Effect
{
	int armorGain = 20;
	int healthLoss = 10;
	public Reposition()
		: base()
	{
		name = "Reposition";
		description = "Changing cover (gain " + armorGain + " armor, lose "+healthLoss+" health)";
		image = SpriteBase.mainSpriteBase.lateralArrows;
		targetType = TargetType.None;
		staminaCost = 1;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.IncrementArmor(armorGain);
		userCharGraphic.TakeHealthDamage(healthLoss);
	}
}

public class Kick : MeleeCard
{
	public Kick():base()
	{
		name = "Kick";
		description = "Guard break (ignores armor)";
		image = SpriteBase.mainSpriteBase.brokenLegsSprite;
		targetType = TargetType.Select;

		staminaCost = 3;
		healthDamage = 10;
	}

	protected override void CardPlayEffects()
	{
		userCharGraphic.IncrementStamina(-staminaCost);
		targetChars[0].TakeHealthDamage(healthDamage);
	}
}

public class Smash : MeleeCard
{
	public Smash()
		: base()
	{
		name = "Smash";
		description = "When in doubt - punch things";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;

		healthDamage = 10;
		staminaCost = 2;
		targetType = TargetType.Select;
	}

}

public class Throw : RangedCard
{
	public Throw()
		: base()
	{
		name = "Throw";
		description = "Everything is fair";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;
		targetType = TargetType.Select;

		staminaCost = 1;
		healthDamage = 5;
	}
}

public class Grenade : RangedCard
{
	public Grenade()
		: base()
	{
		name = "Grenade";
		description = "Fire in the hole (damages all enemies)";
		image = SpriteBase.mainSpriteBase.fire;
		targetType = TargetType.All;

		ammoCost = 3;
		staminaCost = 1;
		healthDamage = 10;
	}
}

public class SecondWind : Effect
{
	int staminaRestore = 3;
	int healthPenalty = 10;
	public SecondWind()
		: base()
	{
		name = "Second Wind";
		description = "Restore " + staminaRestore + " stamina, lose " + healthPenalty + " health";
		image = SpriteBase.mainSpriteBase.restSprite;
		targetType = TargetType.None;
	}

	protected override void CardPlayEffects()
	{
		userCharGraphic.IncrementStamina(staminaRestore);
		userCharGraphic.TakeDamage(healthPenalty, true);
	}
}
//Doubletap

//BANDIT CARDS

public class SuckerPunch : MeleeCard
{
	public SuckerPunch()
	{
		name = "Sucker Punch";
		description = "Won't know what hit them";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;
		targetType = TargetType.Select;

		staminaCost = 2;
		healthDamage = 15;
	}

}

public class Knee : MeleeCard
{
	public Knee()
		: base()
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

public class Jab : MeleeCard
{
	public Jab()
		: base()
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
	public Roundhouse()
		: base()
	{
		name = "Roundhouse kick";
		description = "Badass";
		image = SpriteBase.mainSpriteBase.brokenLegsSprite;
		targetType = TargetType.Select;

		staminaCost = 5;
		healthDamage = 20;
	}
}

public class NoMercy : MeleeCard
{
	public NoMercy()
		: base()
	{
		name = "No Mercy";
		description = "Targets the weakest enemy";
		image = SpriteBase.mainSpriteBase.skull;
		targetType = TargetType.Weakest;

		staminaCost = 4;
		healthDamage = 20;
	}
}
//Throw
//Snap shot
public class SprayNPray:RangedCard
{
	public SprayNPray()
	{
		name = "Spray'n'Pray";
		description = "Quantity over quality (Targets random enemy)";
		image = SpriteBase.mainSpriteBase.bullets;
		targetType = TargetType.Random;

		ammoCost = 5;
		healthDamage = 20;
	}
}
//No mercy x2

//GUNMAN CARDS
public class Sidearm : RangedCard
{
	public Sidearm()
		: base() //x2
	{
		name = "Sidearm";
		description = "Trick gun";
		image = SpriteBase.mainSpriteBase.nineMSprite;
		targetType = TargetType.Select;
		healthDamage = 5;
	}
}
public class Pistolwhip : MeleeCard
{
	public Pistolwhip(): base() //x2
	{
		name = "Pistol Whip";
		description = "Guns don't kill people";
		image = SpriteBase.mainSpriteBase.nineMSprite;
		targetType = TargetType.Select;

		staminaCost = 4;
		healthDamage = 10;
	}
}

public class Doubletap : RangedCard
{
	public Doubletap()
		: base() //x2
	{
		name = "Double Tap";
		description = "Always";
		image = SpriteBase.mainSpriteBase.bulletSprite;

		healthDamage = 20;
		staminaCost = 0;
		ammoCost = 2;
		targetType = TargetType.Select;
	}

}

public class BurstFire : RangedCard
{
	public BurstFire()
		: base()
	{
		name = "Burst Fire";
		description = "Walk the shots";
		image = SpriteBase.mainSpriteBase.bullets;

		healthDamage = 30;
		ammoCost = 4;
		targetType = TargetType.Select;
	}
}

public class HighGround : Effect
{
	int armorGain = 10;
	public HighGround()
		: base()
	{
		name = "High Ground";
		description = "Gain " + armorGain + " armor";
		image = SpriteBase.mainSpriteBase.arrow;
		targetType = TargetType.None;
		staminaCost = 4;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.IncrementArmor(armorGain);
	}
}

public class ScopeIn : RangedCard
{
	public ScopeIn()
		: base()
	{
		name = "Scope In";
		description = "Whites of their eyes (ignores armor)";
		image = SpriteBase.mainSpriteBase.crosshair;

		healthDamage = 25;
		ammoCost = 2;
		targetType = TargetType.Select;
	}

	protected override void CardPlayEffects()
	{
		targetChars[0].TakeDamage(healthDamage, true);
	}
}

public class LessLethal : RangedCard
{
	public LessLethal()
		: base()
	{
		name = "Less Lethal";
		description = "Let them suffer";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;

		healthDamage = 5;
		staminaDamage = 3;
		ammoCost = 2;
		targetType = TargetType.Select;
	}
}

public class Suppression : RangedCard
{
	public Suppression()
		: base()
	{
		name = "Suppressive Fire";
		description = "Keep them pinned (targets all enemies)";
		image = SpriteBase.mainSpriteBase.bullets;

		healthDamage = 5;
		staminaDamage = 2;
		ammoCost = 4;
		targetType = TargetType.All;
	}
}

public class DetonateAmmo : RangedCard
{
	int bonusDamagePerAmmo = 2;
	
	public DetonateAmmo()
		:base()
	{
		name = "Detonate Ammo";
		description = "Remove target ammo. Deal damage equal to removed ammo x"+bonusDamagePerAmmo;
		image = SpriteBase.mainSpriteBase.flamingBullet;

		healthDamage = 5;
		ammoCost = 1;
		targetType = TargetType.Select;
	}

	protected override void CardPlayEffects()
	{
		userCharGraphic.IncrementStamina(-staminaCost);
		int totalDamage = healthDamage + targetChars[0].GetAmmo();
		targetChars[0].TakeDamage(totalDamage);
		targetChars[0].SetAmmo(0);
		
	}
}

//MERCENARY CARDS

//Burst Fire
//Supression
public class RunAndGun:MeleeCard
{
	int takeDamageCost = 5;
	public RunAndGun()
		:base()
	{
		name = "Run And Gun";
		description = "Bread and butter (Take "+takeDamageCost+" damage)";
		image = SpriteBase.mainSpriteBase.lateralArrows;

		healthDamage = 25;
		ammoCost = 3;
		staminaCost = 2;
		targetType = TargetType.Select;
	}

	protected override void CardPlayEffects()
	{
		userCharGraphic.TakeDamage(takeDamageCost);
		base.CardPlayEffects();
	}
}
//Smash
//Jab
//Kick
//Second Wind
public class LegIt : Effect
{
	int armorGain = 10;
	public LegIt()
		: base()
	{
		name = "Leg It";
		description = "Live to fight another day (gain "+armorGain+" armor)";
		image = SpriteBase.mainSpriteBase.brokenLegsSprite;
		targetType = TargetType.None;
		staminaCost = 4;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.IncrementArmor(armorGain);
	}
}
public class DonQuixote : MeleeCard
{
	public DonQuixote()
		: base()
	{
		name = "Don Quixote";
		description = "Targets the strongest enemy";
		image = SpriteBase.mainSpriteBase.skull;
		targetType = TargetType.Strongest;

		staminaCost = 4;
		healthDamage = 20;
	}
}
public class SuicideCharge:MeleeCard
{
	int damageTakenCost = 10;
	public SuicideCharge()
		:base()
	{
		name = "Suicide Charge";
		description = "Take " + damageTakenCost + " damage. Targets all enemies.";
		image = SpriteBase.mainSpriteBase.skull;

		healthDamage = 15;
		ammoCost = 2;
		staminaCost = 2;
		targetType = TargetType.All;
	}

	protected override void CardPlayEffects()
	{
		userCharGraphic.IncrementStamina(-staminaCost);
		userCharGraphic.TakeDamage(damageTakenCost);
		foreach (CharacterGraphic targetChar in targetChars)
		{
			targetChar.TakeDamage(healthDamage);
		}
	}
}



