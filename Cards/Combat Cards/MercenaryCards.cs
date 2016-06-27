using UnityEngine;
using System.Collections;

public class Gambit : Effect
{
	int armorGain = 25;
	int healthLoss = 10;
	public Gambit()
		: base()
	{
		name = "Gambit";
		description = "High stakes (gain " + armorGain + " armor, lose " + healthLoss + " health)";
		image = SpriteBase.mainSpriteBase.lateralArrows;
		targetType = TargetType.None;
		staminaCost = 1;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.IncrementArmor(armorGain);
		userCharGraphic.TakeDamage(healthLoss, true);
	}
}

public class TakeCover : Effect
{
	int armorGain = 30;
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

public class Discretion : Effect
{
	int armorGain = 60;
	public Discretion()
		: base()
	{
		name = "";
		description = "Better part of valor (gain " + armorGain + " armor)";
		image = SpriteBase.mainSpriteBase.cover;
		targetType = TargetType.None;
		staminaCost = 6;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.IncrementArmor(armorGain);
	}
}

public class LastStand : Effect
{
	int staminaRestore = 4;
	int damagePenalty = 20;
	public LastStand()
		: base()
	{
		name = "Last Stand";
		description = "Restore " + staminaRestore + " stamina, take " + damagePenalty + " damage";
		image = SpriteBase.mainSpriteBase.restSprite;
		targetType = TargetType.None;
	}

	protected override void CardPlayEffects()
	{
		userCharGraphic.IncrementStamina(staminaRestore);
		userCharGraphic.TakeDamage(damagePenalty);
	}
}

public class RunAndGun : MeleeCard
{
	int takeDamageCost = 10;
	public RunAndGun()
		: base()
	{
		name = "Run And Gun";
		description = "Bread and butter (Take " + takeDamageCost + " damage)";
		image = SpriteBase.mainSpriteBase.lateralArrows;

		healthDamage = 40;
		ammoCost = 1;
		staminaCost = 3;
		targetType = TargetType.SelectEnemy;
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

public class Overconfidence : MeleeCard
{
	int takeDamageCost = 10;
	
	public Overconfidence()
		: base()
	{
		name = "Overconfidence";
		description = "You can't die twice (targets strongest enemy, mercenary takes "+takeDamageCost+" damage)";
		image = SpriteBase.mainSpriteBase.skull;
		targetType = TargetType.Strongest;

		staminaCost = 3;
		healthDamage = 40;
	}

	protected override void CardPlayEffects()
	{
		userCharGraphic.TakeDamage(takeDamageCost);
		base.CardPlayEffects();
	}
}
public class SuicideCharge : MeleeCard
{
	int damageTakenCost = 10;
	public SuicideCharge()
		: base()
	{
		name = "Suicide Charge";
		description = "Take " + damageTakenCost + " damage. Targets all enemies.";
		image = SpriteBase.mainSpriteBase.skull;

		healthDamage = 25;
		staminaCost = 5;
		targetType = TargetType.AllEnemies;
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
