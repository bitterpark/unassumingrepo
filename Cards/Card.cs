using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Card
{
	public string name;
	public Sprite image;
	public string description;
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
	int cashReward = 325;
	
	public CashStash()
	{
		name = "Cash Stash";
		description = "Stacks of Federation and Inner Worlds currency, and a few Gate coins";
		image = SpriteBase.mainSpriteBase.pillsSprite;
		effectDescription="+$"+cashReward;
	}

	public override void PlayCard()
	{
		TownManager.main.money += cashReward;
	}

}

public class CashVault : RewardCard
{
	int cashReward = 650;

	public CashVault()
	{
		name = "Cash Vault";
		description = "A safe stacked with bills of several currencies";
		image = SpriteBase.mainSpriteBase.pillsSprite;
		effectDescription = "+$" + cashReward;
	}

	public override void PlayCard()
	{
		TownManager.main.money += cashReward;
	}
}

public class AmmoStash : RewardCard
{
	public AmmoStash()
	{
		name = "Ammo Stash";
		image = SpriteBase.mainSpriteBase.ammoBoxSprite;
		description = "Neatly stacked boxes full of powder bullets";
		effectDescription="Everyone gains full ammo";
		//rewardItems.Add(new AmmoBox());
	}
	public override void PlayCard()
	{
		CardsScreen.main.ResetAllMercsResource(CharacterGraphic.Resource.Ammo);
	}
}

public class ArmorStash : RewardCard
{
	int armorReward = 30;
	public ArmorStash()
	{
		name = "Armor Stash";
		image = SpriteBase.mainSpriteBase.armor;
		description = "Piles of helmets and armor vests (+" + armorReward + " armor for everyone)";
		effectDescription = "+" + armorReward + " armor for everyone";
		//rewardItems.Add(new AmmoBox());
	}
	public override void PlayCard()
	{
		CardsScreen.main.IncrementAllMercsResource(CharacterGraphic.Resource.Armor, armorReward);
	}
}

public class StoryRewardOne : RewardCard
{
	public StoryRewardOne()
	{
		name = "Fusion Core";
		image = SpriteBase.mainSpriteBase.lightning;
		description = "The key to our dream";
		rewardItems.Add(new FusionCore());
	}
}

public class ScrapReward : RewardCard
{
	public ScrapReward()
	{
		name = "Scrapheap";
		image = SpriteBase.mainSpriteBase.wrench;
		description = "A pile of salvageable metal";
		rewardItems.Add(new Scrap());
		rewardItems.Add(new Scrap());
	}
}

public class ComputerPartsReward : RewardCard
{
	public ComputerPartsReward()
	{
		name = "Computers";
		image = SpriteBase.mainSpriteBase.computerParts;
		description = "Circuits lined with hundreds of thousands of transistors";
		rewardItems.Add(new ComputerParts());
	}
}

public class CrewReward : RewardCard
{
	int crewIncrease = 10;
	
	public CrewReward()
	{
		name = "Crew";
		image = SpriteBase.mainSpriteBase.mercPortrait;
		description = "People capable and/or crazy enough to join us";
		effectDescription = "Increase crew by " + crewIncrease;
	}

	public override void PlayCard()
	{
		TownManager.main.IncrementCrew(crewIncrease);
	}
}

public class IncomeReward : RewardCard
{
	//int cashReward = 1300;

	public IncomeReward()
	{
		name = "Fake Cash";
		description = "Middling quality";
		image = SpriteBase.mainSpriteBase.money;
		//effectDescription = "+$" + cashReward;
		rewardItems.Add(new FakeMoney());
	}
	/*
	public override void PlayCard()
	{
		TownManager.main.money += cashReward;
	}*/
}

public abstract class CombatCard: Card
{
	public int damage=0;
	public int staminaDamage=0;
	public int unarmoredBonusDamage = 0;

	public int ammoCost=0;
	public int staminaCost=0;
	public int removeHealthCost = 0;
	public int takeDamageCost = 0;

	public bool ignoresArmor = false;

	public StipulationCard addedStipulationCard = null;

	public enum TargetType { None, SelectEnemy, SelectFriendly,Weakest,Strongest,Random,AllEnemies,AllFriendlies};
	public TargetType targetType=TargetType.None;

	public enum CardType {Melee_Attack,Ranged_Attack,Effect};
	public CardType cardType=CardType.Melee_Attack;

	//public CharacterGraphic targetCharGraphic;
	public List<CharacterGraphic> targetChars=new List<CharacterGraphic>();
	public CharacterGraphic userCharGraphic;

	public Deck<CombatCard> originDeck;

	public void PlayCard()
	{
		CardPlayEvents();
		if (userCharGraphic.GetHealth()>0)
			CardPlayEffects();
		ClearTargetsAfterPlay();
	}

	protected virtual void CardPlayEffects()
	{
		ApplyPlayCosts();
		TryAddStipulationCards();
		ApplyEffects();
	}

	protected virtual void ApplyPlayCosts()
	{
		userCharGraphic.IncrementAmmo(-ammoCost);
		userCharGraphic.IncrementStamina(-staminaCost);
		userCharGraphic.TakeDamage(takeDamageCost);
		userCharGraphic.TakeDamage(removeHealthCost, true);
	}

	protected virtual void ApplyEffects()
	{
		if (targetType != TargetType.None)
		{
			DamageTargets();
		}
	}

	protected virtual void DamageTargets()
	{
		foreach (CharacterGraphic targetCharGraphic in targetChars)
		{
			int totalDamage = damage;
			if (targetCharGraphic.GetArmor() <= 0)
				totalDamage += unarmoredBonusDamage;
			targetCharGraphic.TakeDamage(totalDamage, ignoresArmor);
			targetCharGraphic.IncrementStamina(-staminaDamage);
		}
	}

	protected void TryAddStipulationCards()
	{
		if (addedStipulationCard != null)
		{
			if (addedStipulationCard.GetType().BaseType == typeof(RoomStipulationCard))
			{
				RoomStipulationCard card = addedStipulationCard as RoomStipulationCard;
				CardsScreen.main.PlaceRoomCard(card);
			}
			if (addedStipulationCard.GetType().BaseType == typeof(CharacterStipulationCard))
			{
				CharacterStipulationCard card = addedStipulationCard as CharacterStipulationCard;
				userCharGraphic.PlaceCharacterCard(card);
			}
		}
	}

	protected virtual void CardPlayEvents()
	{

	}

	void ClearTargetsAfterPlay()
	{
		targetChars.Clear();
		userCharGraphic = null;
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
	public delegate void RangedCardDeleg(CharacterGraphic cardPlayer);
	public static event RangedCardDeleg ERangedCardPlayed;
	
	public RangedCard()
		: base()
	{
		cardType = CardType.Ranged_Attack;
	}

	protected override void CardPlayEvents()
	{
		if (ERangedCardPlayed != null) ERangedCardPlayed(userCharGraphic);
	}
}

public class MeleeCard : CombatCard
{
	public MeleeCard()
		: base()
	{
		cardType = CardType.Melee_Attack;
	}

	public delegate void MeleeCardDeleg(CharacterGraphic cardPlayer);
	public static event MeleeCardDeleg EMeleeCardPlayed;

	protected override void CardPlayEvents()
	{
		if (EMeleeCardPlayed != null) EMeleeCardPlayed(userCharGraphic);
	}
}

public class EffectCard : CombatCard
{
	protected int userStaminaGain = 0;
	protected int userAmmoGain = 0;
	protected int userArmorGain = 0;

	protected int targetStaminaGain = 0;
	protected int targetAmmoGain = 0;
	protected int targetArmorGain = 0;

	
	public EffectCard()
		: base()
	{
		cardType=CardType.Effect;
	}

	protected override void ApplyEffects()
	{
		userCharGraphic.IncrementStamina(userStaminaGain);
		userCharGraphic.IncrementAmmo(userAmmoGain);
		userCharGraphic.IncrementArmor(userArmorGain);
		if (targetType != TargetType.None)
		{
			foreach (CharacterGraphic targetChar in targetChars)
			{
				targetChar.IncrementStamina(targetStaminaGain);
				targetChar.IncrementAmmo(targetAmmoGain);
				targetChar.IncrementArmor(targetArmorGain);
			}
		}
	}

}





