﻿using UnityEngine;
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
		image = SpriteBase.mainSpriteBase.ammoBox;
		description = "Neatly stacked boxes full of powder bullets";
		effectDescription="Everyone gains full ammo";
		//rewardItems.Add(new AmmoBox());
	}
	public override void PlayCard()
	{
		MissionCharacterManager.main.ResetAllMercsResource(CharacterGraphic.Resource.Ammo);
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
		MissionCharacterManager.main.IncrementAllMercsResource(CharacterGraphic.Resource.Armor, armorReward);
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
		image = SpriteBase.mainSpriteBase.screwdriver;
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

public abstract class PrepCard : Card
{
	protected List<CombatCard> addedCombatCards=new List<CombatCard>();
	protected CharacterStipulationCard placedStipulationCard;

	public void PlayCard(CharacterGraphic playToCharacter)
	{
		if (addedCombatCards.Count>0)
			playToCharacter.AddCardsToCurrentDeck(addedCombatCards.ToArray());
		if (placedStipulationCard != null)
			playToCharacter.TryPlaceCharacterStipulationCard(placedStipulationCard);
	}

	public List<CombatCard> GetAddedCombatCards()
	{
		return addedCombatCards;
	}
	public CharacterStipulationCard GetPlacedStipulationCard()
	{
		return placedStipulationCard;
	}

}

public class CustomPrepCard : PrepCard
{
	public CustomPrepCard(string name, string description, Sprite image, List<CombatCard> addedCombatCards)
	{
		this.name = name;
		this.description = description;
		this.image = image;
		this.addedCombatCards = addedCombatCards;
	}
}

public class DefaultMeleePrepCard : PrepCard {

	public DefaultMeleePrepCard()
	{
		name = "Bare Hands";
		description = "Add melee cards to your deck";
		image = SpriteBase.mainSpriteBase.arm;

		addedCombatCards.Add(new Jab());
		addedCombatCards.Add(new Jab());
		addedCombatCards.Add(new Smash());
		addedCombatCards.Add(new Smash());
		addedCombatCards.Add(new Roundhouse());
	}
}

public class DefaultRangedPrepCard : PrepCard
{
	public DefaultRangedPrepCard()
	{
		name = "Sidearm";
		description = "Add ranged cards to your deck";
		image = SpriteBase.mainSpriteBase.pipegun;

		addedCombatCards.Add(new Hipfire());
		addedCombatCards.Add(new Hipfire());
		addedCombatCards.Add(new Sidearm());
		addedCombatCards.Add(new Sidearm());
		addedCombatCards.Add(new DoubleTap());
	}
}

public abstract class CombatCard: Card
{
	public int damage=0;
	public int damagePerStaminaPoint = 0;
	public int staminaDamage=0;
	public int ammoDamage = 0;
	public int unarmoredBonusDamage = 0;
		
	public int ammoCost=0;
	public int staminaCost=0;
	public int maxStaminaCost = 0;
	public bool useUpAllStamina = false;

	protected int usedUpStaminaPoints = 0;

	public int removeHealthCost = 0;
	public int takeDamageCost = 0;

	public bool ignoresArmor = false;
	public bool ignoresBlocks = false;

	public StipulationCard addedStipulationCard = null;

	public enum TargetType { None, SelectEnemy, SelectFriendly,SelectFriendlyOther,Weakest,Strongest,Random,AllEnemies,AllFriendlies};
	public TargetType targetType=TargetType.None;

	public enum CardType {Melee_Attack,Ranged_Attack,Effect};
	public CardType cardType=CardType.Melee_Attack;

	//public CharacterGraphic targetCharGraphic;
	public List<CharacterGraphic> targetChars=new List<CharacterGraphic>();
	public CharacterGraphic userCharGraphic;

	public Deck<CombatCard> originDeck;

	public bool SpecialPrerequisitesMet(CharacterGraphic user)
	{
		bool prerequisitesMet = true;

		if (targetType == TargetType.SelectFriendlyOther)
		{
			if (user.GetType() == typeof(EnemyGraphic))
				prerequisitesMet = CardsScreen.main.GetCurrentEnemyCount() > 1;
			if (user.GetType() == typeof(MercGraphic))
				prerequisitesMet = CardsScreen.main.GetCurrentMercCount() > 1;
		}
		if (!ExtenderPrerequisitesMet(user))
			prerequisitesMet = false;

		return prerequisitesMet;
	}

	protected virtual bool ExtenderPrerequisitesMet(CharacterGraphic user)
	{
		return true;
	}

	public void PlayCard()
	{
		if (useUpAllStamina)
			usedUpStaminaPoints = damagePerStaminaPoint * userCharGraphic.GetStamina();
		ApplyPlayCosts();
		//bool cardIsBlocked = IsCardBlocked();
		CardPlayEvents();
		if (userCharGraphic.GetHealth() > 0)
			ApplyCardPlayEffects();
	}

	public void SetIgnoresArmor(bool ignoresArmor)
	{
		this.ignoresArmor = ignoresArmor;
	}

	public void SetIgnoresBlocks(bool ignoresBlocks)
	{
		this.ignoresBlocks = ignoresBlocks;
	}

	public void SetUserChar(CharacterGraphic userChar)
	{
		userCharGraphic = userChar;
	}
	public CharacterGraphic GetUserChar()
	{
		return userCharGraphic;
	}

	public CombatCard()
	{
		ExtenderConstructor();
	}

	public void SetDefaultState()
	{
		damage=0;
		staminaDamage=0;
		ammoDamage = 0;
		damagePerStaminaPoint = 0;
		unarmoredBonusDamage = 0;
		usedUpStaminaPoints = 0;

		ammoCost=0;
		staminaCost=0;
		maxStaminaCost = 0;
		removeHealthCost = 0;
		takeDamageCost = 0;

		ignoresArmor = false;
		ignoresBlocks = false;
		userCharGraphic = null;
		targetChars.Clear();
		ExtenderConstructor();
	}
	protected abstract void ExtenderConstructor();
	/*
	bool IsCardBlocked()
	{
		System.Type playedCardType = GetType();
		if (playedCardType == typeof(MeleeCard) | playedCardType == typeof(RangedCard))
		{

			if (ignoresBlocks)
				return false;
			else
				return CheckIfAllTargetsBlocked();
		}
		else 
			return false;
	}

	bool CheckIfAllTargetsBlocked()
	{
		foreach (CharacterGraphic target in new List<CharacterGraphic>(targetChars))
		{
			if (target.IsCardBlocked(this))
				targetChars.Remove(target);
		}
		if (targetChars.Count > 0)
			return false;
		else
			return true;
	}
	*/
	protected virtual void ApplyCardPlayEffects()
	{
		TryAddStipulationCards();
		ApplyStatEffects();
	}

	protected virtual void ApplyPlayCosts()
	{
		if (useUpAllStamina)
			usedUpStaminaPoints = userCharGraphic.GetStamina();
		userCharGraphic.SubtractCardCostsFromResources(useUpAllStamina,ammoCost,staminaCost,maxStaminaCost,takeDamageCost,removeHealthCost);
	}

	protected virtual void ApplyStatEffects()
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
			totalDamage += damagePerStaminaPoint * usedUpStaminaPoints;

			if (targetCharGraphic.GetArmor() <= 0)
				totalDamage += unarmoredBonusDamage;
			targetCharGraphic.TakeDamage(totalDamage, ignoresArmor);
			targetCharGraphic.IncrementStamina(-staminaDamage);
			targetCharGraphic.IncrementAmmo(-ammoDamage);
		}
	}

	protected void TryAddStipulationCards()
	{
		if (addedStipulationCard != null)
		{
			if (addedStipulationCard.GetType().BaseType == typeof(RoomStipulationCard))
			{
				RoomStipulationCard card = addedStipulationCard as RoomStipulationCard;
				CombatManager.main.PlaceRoomStipulationCard(card);
			}
			if (addedStipulationCard.GetType().BaseType == typeof(CharacterStipulationCard))
			{
				CharacterStipulationCard card = addedStipulationCard as CharacterStipulationCard;
				if (targetType == TargetType.None)
					userCharGraphic.TryPlaceCharacterStipulationCard(card);
				else
				{
					foreach (CharacterGraphic targetChar in targetChars)
						targetChar.TryPlaceCharacterStipulationCard(card);
				}
			}
		}
	}

	protected virtual void CardPlayEvents()
	{

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
	public delegate void RangedCardDeleg(CharacterGraphic cardPlayer, RangedCard playedCard);
	public static event RangedCardDeleg ERangedCardPlayed;
	
	public RangedCard()
		: base()
	{
		cardType = CardType.Ranged_Attack;
	}

	protected override void ExtenderConstructor()
	{
	
	}

	protected override void CardPlayEvents()
	{
		if (ERangedCardPlayed != null) 
			ERangedCardPlayed(userCharGraphic,this);
	}
}

public class MeleeCard : CombatCard
{
	public MeleeCard()
		: base()
	{
		cardType = CardType.Melee_Attack;
	}

	public delegate void MeleeCardDeleg(CharacterGraphic cardPlayer, MeleeCard playedCard);
	public static event MeleeCardDeleg EMeleeCardPlayed;

	protected override void ExtenderConstructor()
	{

	}

	protected override void CardPlayEvents()
	{
		if (EMeleeCardPlayed != null) 
			EMeleeCardPlayed(userCharGraphic,this);
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

	protected override void ExtenderConstructor()
	{

	}

	protected override void ApplyStatEffects()
	{
		userCharGraphic.IncrementStamina(userStaminaGain);
		userCharGraphic.IncrementAmmo(userAmmoGain);
		userCharGraphic.IncrementArmor(userArmorGain);
		if (targetType != TargetType.None)
		{
			base.ApplyStatEffects();
			foreach (CharacterGraphic targetChar in targetChars)
			{
				targetChar.IncrementStamina(targetStaminaGain);
				targetChar.IncrementAmmo(targetAmmoGain);
				targetChar.IncrementArmor(targetArmorGain);
			}
		}
	}

}





