using UnityEngine;
using System.Collections;

public class MercenaryCards
{
	public static CombatDeck GetClassCards()
	{
		CombatDeck result = new CombatDeck();

		result.AddCards(typeof(TakeCover), 2);
		result.AddCards(typeof(Discretion));
		result.AddCards(typeof(Gambit));
		result.AddCards(typeof(RunAndGun));
		result.AddCards(typeof(Hipfire));
		result.AddCards(typeof(Overconfidence));
		result.AddCards(typeof(SuicideCharge));
		result.AddCards(typeof(Jab));
		result.AddCards(typeof(Smash));
		result.AddCards(typeof(LastStand),2);

		return result;
	}
}

public class Gambit : EffectCard
{
	public Gambit()
		: base()
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
	public TakeCover()
		: base()
	{
		targetType = TargetType.None;
		staminaCost = 2;
		userArmorGain = 45;
		
		name = "Take Cover";
		description = "Hit the deck (gain " + userArmorGain + " armor)";
		image = SpriteBase.mainSpriteBase.cover;
	}
}

public class Discretion : EffectCard
{
	public Discretion()
		: base()
	{
		targetType = TargetType.None;
		staminaCost = 4;
		userArmorGain = 75;
		
		name = "Discretion";
		description = "Better part of valor (gain " + userArmorGain + " armor)";
		image = SpriteBase.mainSpriteBase.cover;
		
	}
}

public class LastStand : EffectCard
{
	public LastStand()
		: base()
	{
		targetType = TargetType.None;
		takeDamageCost = 40;
		userStaminaGain = 3;
		
		name = "Last Stand";
		description = "Restore " + userStaminaGain + " stamina, take " + takeDamageCost + " damage";
		image = SpriteBase.mainSpriteBase.restSprite;
	}
}

public class RunAndGun : MeleeCard
{
	public RunAndGun()
		: base()
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
	public Overconfidence()
		: base()
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
	public SuicideCharge()
		: base()
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
