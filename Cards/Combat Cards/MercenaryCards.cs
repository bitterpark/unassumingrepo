using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MercenaryCards
{
	public static CombatDeck GetClassCards()
	{
		CombatDeck result = new CombatDeck();
		/*
		result.AddCards(typeof(TakeCover),2);
		result.AddCards(typeof(Hipfire));
		result.AddCards(typeof(BurstFire));
		result.AddCards(typeof(Overconfidence),2);
		result.AddCards(typeof(RunAndGun),2);
		result.AddCards(typeof(LastStand));
		result.AddCards(typeof(Gambit));
		*/
		result.AddCards(typeof(BurstFire));
		result.AddCards(typeof(SuicideCharge));
		//result.AddCards(typeof(LastStand));
		//result.AddCards(typeof(ComeAtMe));
		result.AddCards(typeof(RunAndGun));
		result.AddCards(typeof(TakeCover));

		return result;
	}

	public static List<PrepCard> GetClassPrepCards()
	{
		List<PrepCard> result = new List<PrepCard>();
		result.Add(new Deathwish());
		result.Add(new LastStand());
		result.Add(new Challenger());
		//result.Add(new RangeBlockerPrep());


		return result;
	}

	public class Deathwish : PrepCard
	{
		public Deathwish()
		{
			placedStipulationCard = new AdrenalineJunkie();
			
			name = "Deathwish";
			description = placedStipulationCard.description;
			image = SpriteBase.mainSpriteBase.skull;
		}
	}

	public class AdrenalineJunkie : CharacterStipulationCard
	{	
		public AdrenalineJunkie()
		{
			name = "Adrenaline Junkie";
			image = SpriteBase.mainSpriteBase.lightning;
			description = "At the start of every round, if armor is 0 gain Full Block";
		}

		protected override void ExtenderSpecificActivation()
		{
			CombatManager.ENewRoundStarted += TryAddFullBlock;
		}

		protected override void ExtenderSpecificDeactivation()
		{
			CombatManager.ENewRoundStarted -= TryAddFullBlock;
		}

		void TryAddFullBlock()
		{
			if (appliedToCharacter.GetArmor()==0)
				appliedToCharacter.TryPlaceCharacterStipulationCard(new FullBlock());
		}
	}

	public class LastStand : PrepCard
	{
		public LastStand()
		{
			placedStipulationCard = new DigIn();
			
			name = "Last Stand";
			description = placedStipulationCard.description;
			image = SpriteBase.mainSpriteBase.cover;
		}
	}

	public class DigIn : CharacterStipulationCard
	{
		int armorPerPointOfMaxStamina = 40;
		public DigIn()
		{
			name = "Dig In";
			image = SpriteBase.mainSpriteBase.cover;
			description = "Set max stamina to 0, gain " + armorPerPointOfMaxStamina + " armor per point of max stamina";
		}

		protected override void ExtenderSpecificActivation()
		{
			int mercsMaxStamina = appliedToCharacter.GetMaxStamina();
			int armorIncrement = mercsMaxStamina * armorPerPointOfMaxStamina;
			appliedToCharacter.SetMaxStamina(0);
			appliedToCharacter.IncrementArmor(armorIncrement);
		}
	}

	public class Challenger : PrepCard
	{
		public Challenger()
		{
			placedStipulationCard = new ComeAtMe();
			
			name = "Challenger";
			description = placedStipulationCard.description;
			image = SpriteBase.mainSpriteBase.arm;
		}
	}
}

public class ComeAtMe : CharacterStipulationCard
{
	public ComeAtMe()
	{
		name = "Come At Me";
		image = SpriteBase.mainSpriteBase.skull;
		description = "At the start of every round, if armor is above 0 become Brawler";
	}

	protected override void ExtenderSpecificActivation()
	{
		CombatManager.ENewRoundStarted += TryAddBrawler;
	}

	protected override void ExtenderSpecificDeactivation()
	{
		CombatManager.ENewRoundStarted -= TryAddBrawler;
	}

	void TryAddBrawler()
	{
		if (appliedToCharacter.GetArmor() > 0)
			appliedToCharacter.TryPlaceCharacterStipulationCard(new Brawler());
	}
}

public class Brawler : CharacterStipulationCard
{
	public Brawler()
	{
		SetLastsForRounds(1);

		name = "Brawler";
		image = SpriteBase.mainSpriteBase.arm;
		description = "For one round: all melee attacks played by enemies will only target this character";
	}

	protected override void ExtenderSpecificActivation()
	{
		CombatCardTargeter.main.SetNewMeleeTargetMerc(appliedToCharacter);
		CombatCardTargeter.ENewMeleeTargetMercSet += RemoveCard;
	}
	void RemoveCard()
	{
		appliedToCharacter.RemoveCharacterStipulationCard(this);
	}

	protected override void ExtenderSpecificDeactivation()
	{
		CombatCardTargeter.main.ClearMeleeTargetMerc();
		CombatCardTargeter.ENewMeleeTargetMercSet -= RemoveCard;
	}
}


public class SuicideCharge : MeleeCard
{
	protected override bool ExtenderPrerequisitesMet(CharacterGraphic user)
	{
		return user.GetArmor() > 0;
	}

	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectEnemy;

		name = "Suicide Charge";
		description = "Spend all armor, do damage equal to armor spent";
		image = SpriteBase.mainSpriteBase.skull;
	}

	protected override void ApplyPlayCosts()
	{
		takeDamageCost = userCharGraphic.GetArmor();
		damage += takeDamageCost;
		base.ApplyPlayCosts();
	}
}

/*
public class PlanB : EffectCard
{
	int bonusArmorPerStaminaPoint = 25;
	
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		usesUpAllStamina = true;

		name = "Plan B";
		description = "Spend all stamina, gain " + bonusArmorPerStaminaPoint+" armor per point of stamina";
		image = SpriteBase.mainSpriteBase.lateralArrows;
	}

	protected override void ApplyEffects()
	{
		userArmorGain = usedUpStaminaPoints * bonusArmorPerStaminaPoint;
		base.ApplyEffects();
	}
}*/

public class Gambit : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		userArmorGain = 50;
		removeHealthCost = 10;
		
		name = "Gambit";
		description = "High stakes (gain " + userArmorGain + " armor, lose " + removeHealthCost + " health)";
		image = SpriteBase.mainSpriteBase.lateralArrows;
		
	}
}

public class TakeCover : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		staminaCost = 1;
		userArmorGain = 20;
		
		name = "Take Cover";
		description = "Hit the deck (gain " + userArmorGain + " armor)";
		image = SpriteBase.mainSpriteBase.cover;
	}
}

/*
public class LastStand : EffectCard
{
	int maxStaminaReduction = 2;
	
	protected override bool ExtenderPrerequisitesMet(CharacterGraphic user)
	{
		return user.GetMaxStamina() >= maxStaminaReduction;
	}
	
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		maxStaminaCost = maxStaminaReduction;
		userArmorGain = 80;
		
		name = "Last Stand";
		description = "Remove "+maxStaminaCost+" max stamina, gain "+userArmorGain+" armor";
		image = SpriteBase.mainSpriteBase.restSprite;
	}
}*/
/*
public class ComeAtMe : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		addedStipulationCard = new Brawler();

		name = "Come At Me";
		description = addedStipulationCard.description;
		image = SpriteBase.mainSpriteBase.skull;

	}
}*/

public class RunAndGun : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		damage = 30;
		ammoCost = 1;
		takeDamageCost = 10;
		targetType = TargetType.SelectEnemy;
		
		name = "Run And Gun";
		description = "Bread and butter (Take " + takeDamageCost + " damage)";
		image = SpriteBase.mainSpriteBase.lateralArrows;
	}
}

//Smash
//Jab
//Kick
//Second Wind

public class Overconfidence : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.Strongest;
		staminaCost = 1;
		takeDamageCost = 10;
		damage = 30;

		name = "Overconfidence";
		description = "You can't die twice (targets toughest enemy, mercenary takes "+takeDamageCost+" damage)";
		image = SpriteBase.mainSpriteBase.skull;
		
	}
}

