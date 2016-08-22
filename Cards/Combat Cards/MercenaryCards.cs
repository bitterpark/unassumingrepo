using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MercenaryCards
{
	public static CombatDeck GetClassCards()
	{
		CombatDeck result = new CombatDeck();

		result.AddCards(typeof(TakeCover));
		result.AddCards(typeof(Hipfire));
		result.AddCards(typeof(BurstFire));
		result.AddCards(typeof(Overconfidence));
		result.AddCards(typeof(RunAndGun));
		result.AddCards(typeof(LastStand));
		result.AddCards(typeof(Gambit));

		return result;
	}

	public static List<PrepCard> GetClassPrepCards()
	{
		List<PrepCard> result = new List<PrepCard>();
		result.Add(new Deathwish());
		result.Add(new Unbreakable());
		result.Add(new Challenger());


		return result;
	}

	public class Deathwish : PrepCard
	{
		public Deathwish()
		{
			name = "Deathwish";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.skull;

			addedCombatCards.Add(new SuicideCharge());
		}
	}
	public class Unbreakable : PrepCard
	{
		public Unbreakable()
		{
			name = "Unbreakable";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.armor;

			addedCombatCards.Add(new PlanB());
		}
	}
	public class Challenger : PrepCard
	{
		public Challenger()
		{
			name = "Challenger";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.arm;

			addedCombatCards.Add(new ComeAtMe());
		}
	}
}



public class PlanB : EffectCard
{
	int bonusArmorPerStaminaPoint = 25;
	
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;

		name = "Plan B";
		description = "Spend all stamina, gain " + bonusArmorPerStaminaPoint+" armor per point of stamina";
		image = SpriteBase.mainSpriteBase.lateralArrows;
	}

	protected override void ApplyPlayCosts()
	{
		staminaCost = userCharGraphic.GetStamina();
		userArmorGain = bonusArmorPerStaminaPoint * staminaCost;
		base.ApplyPlayCosts();
	}
}

public class Gambit : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		staminaCost = 1;
		userArmorGain = 70;
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
		staminaCost = 2;
		userArmorGain = 40;
		
		name = "Take Cover";
		description = "Hit the deck (gain " + userArmorGain + " armor)";
		image = SpriteBase.mainSpriteBase.cover;
	}
}



public class LastStand : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		takeDamageCost = 20;
		userStaminaGain = 2;
		
		name = "Last Stand";
		description = "Restore " + userStaminaGain + " stamina, take " + takeDamageCost + " damage";
		image = SpriteBase.mainSpriteBase.restSprite;
	}
}

public class ComeAtMe : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		addedStipulationCard = new Brawler();
		staminaCost = 2;

		name = "Come At Me";
		description = "Play a Come At Me card to the character";
		image = SpriteBase.mainSpriteBase.skull;

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
}

public class RunAndGun : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		damage = 60;
		ammoCost = 1;
		staminaCost = 2;
		takeDamageCost=10;
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
		staminaCost = 2;
		takeDamageCost = 10;
		damage = 40;

		name = "Overconfidence";
		description = "You can't die twice (targets strongest enemy, mercenary takes "+takeDamageCost+" damage)";
		image = SpriteBase.mainSpriteBase.skull;
		
	}
}
public class SuicideCharge : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		damage = 10;
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
