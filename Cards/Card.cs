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
	int cashReward = 250;
	
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
	int cashReward = 500;

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
	int ammoReward = 5;
	public AmmoStash()
	{
		name = "Ammo Stash";
		image = SpriteBase.mainSpriteBase.ammoBoxSprite;
		description = "Neatly stacked boxes full of powder bullets";
		effectDescription="+"+ammoReward+" ammo for everyone";
		//rewardItems.Add(new AmmoBox());
	}
	public override void PlayCard()
	{
		CardsScreen.main.IncrementAllMercsResource(CharacterGraphic.Resource.Ammo,ammoReward);
	}
}

public class ArmorStash : RewardCard
{
	int armorReward = 5;
	public ArmorStash()
	{
		name = "Armor Stash";
		image = SpriteBase.mainSpriteBase.cover;
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
	}
}

public class ComputerPartsReward : RewardCard
{
	public ComputerPartsReward()
	{
		name = "Computers";
		image = SpriteBase.mainSpriteBase.wrench;
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
		image = SpriteBase.mainSpriteBase.wrench;
		description = "People capable and/or crazy enough to join us";
		effectDescription = "Increase crew by " + crewIncrease;
	}

	public override void PlayCard()
	{
		TownManager.main.IncrementCrew(crewIncrease);
	}
}

public abstract class CombatCard: Card
{
	public int healthDamage=0;
	public int staminaDamage=0;

	public int ammoCost=0;
	public int staminaCost=0;

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
		CardPlayEffects();
		ClearTargetsAfterPlay();
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

public class Effect : CombatCard
{
	public Effect()
		: base()
	{
		cardType=CardType.Effect;
	}
}





