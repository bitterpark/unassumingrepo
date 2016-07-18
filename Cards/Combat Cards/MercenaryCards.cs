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
		result.AddCards(typeof(SuicideCharge));
		result.AddCards(typeof(Jab));
		result.AddCards(typeof(Smash),2);
		result.AddCards(typeof(ComeAtMe));
		result.AddCards(typeof(ControlledBursts));

		return result;
	}

	public static List<PrepCard> GetClassPrepCards()
	{
		List<PrepCard> result = new List<PrepCard>();
		result.Add(new Deathwish());
		result.Add(new Unbreakable());
		result.Add(new HiredGun());


		return result;
	}

	public class Deathwish : PrepCard
	{
		public Deathwish()
		{
			name = "Deathwish";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.skull;

			addedCombatCards.Add(new Overconfidence());
			addedCombatCards.Add(new SuicideCharge());
			addedCombatCards.Add(new ComeAtMe());
			addedCombatCards.Add(new RunAndGun());
		}
	}
	public class Unbreakable : PrepCard
	{
		public Unbreakable()
		{
			name = "Unbreakable";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.armor;

			addedCombatCards.Add(new TakeCover());
			addedCombatCards.Add(new LastStand());
			addedCombatCards.Add(new PlanB());
			addedCombatCards.Add(new PlanB());
		}
	}
	public class HiredGun : PrepCard
	{
		public HiredGun()
		{
			name = "Hired Gun";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.assaultRifleSprite;

			addedCombatCards.Add(new BurstFire());
			addedCombatCards.Add(new BurstFire());
			addedCombatCards.Add(new Hipfire());
			addedCombatCards.Add(new ControlledBursts());
		}
	}
}

public class ControlledBursts : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		addedStipulationCard = new TriggerDiscipline();
		staminaCost = 1;

		name = "Controlled Bursts";
		description = "Play a Trigger Discipline card to the character";
		image = SpriteBase.mainSpriteBase.bullets;

	}

	public class TriggerDiscipline : CharacterStipulationCard
	{
		public TriggerDiscipline()
		{
			name = "Trigger Discipline";
			image = SpriteBase.mainSpriteBase.crosshair;
			description = "Character's next ranged attack costs stamina instead of ammo";
		}

		protected override void ExtenderSpecificActivation()
		{
			appliedToCharacter.SetRangedAttacksCostStamina(true);
			RangedCard.ERangedCardPlayed += TriggerEffect;
		}
		void TriggerEffect(CharacterGraphic cardPlayer, RangedCard playedCard)
		{
			if (cardPlayer==appliedToCharacter)
				appliedToCharacter.RemoveCharacterStipulationCard(this);
		}

		protected override void ExtenderSpecificDeactivation()
		{
			appliedToCharacter.SetRangedAttacksCostStamina(false);
			RangedCard.ERangedCardPlayed -= TriggerEffect;
		}
	}
}

public class PlanB : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		userArmorGain = 50;

		name = "Plan B";
		description = "Gain "+userArmorGain+" armor, lose all stamina";
		image = SpriteBase.mainSpriteBase.lateralArrows;
	}
	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.IncrementStamina(-userCharGraphic.GetStamina());
	}
}

public class Gambit : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		staminaCost = 1;
		userArmorGain = 35;
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
		userArmorGain = 45;
		
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
		takeDamageCost = 40;
		userStaminaGain = 3;
		
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
			image = SpriteBase.mainSpriteBase.brokenArmsSprite;
			description = "For one round: all melee attacks played by enemies will only target this character";
		}

		protected override void ExtenderSpecificActivation()
		{
			CardsScreen.main.SetNewMeleeTargetMerc(appliedToCharacter);
			CardsScreen.ENewMeleeTargetMercSet += RemoveCard;
		}
		void RemoveCard()
		{
			appliedToCharacter.RemoveCharacterStipulationCard(this);
		}

		protected override void ExtenderSpecificDeactivation()
		{
			CardsScreen.main.ClearMeleeTargetMerc();
			CardsScreen.ENewMeleeTargetMercSet -= RemoveCard;
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
		damage = 40;
		staminaCost = 4;
		takeDamageCost = 20;
		targetType = TargetType.AllEnemies;
		
		name = "Suicide Charge";
		description = "Take " + takeDamageCost + " damage. Targets all enemies.";
		image = SpriteBase.mainSpriteBase.skull;
	}
}
