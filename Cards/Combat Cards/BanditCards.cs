using UnityEngine;
using System.Collections;

public class SuckerPunch : MeleeCard
{
	public SuckerPunch()
	{
		name = "Sucker Punch";
		description = "Won't know what hit them";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;
		targetType = TargetType.SelectEnemy;

		staminaCost = 2;
		healthDamage = 10;
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
		targetType = TargetType.SelectEnemy;

		staminaCost = 4;
		staminaDamage = 4;
		//healthDamage = 5;
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
		targetType = TargetType.SelectEnemy;

		staminaCost = 6;
		healthDamage = 60;
	}
}

public class Execution : MeleeCard
{
	public Execution()
		: base()
	{
		name = "Execution";
		description = "Targets the weakest enemy";
		image = SpriteBase.mainSpriteBase.skull;
		targetType = TargetType.Weakest;

		staminaCost = 4;
		healthDamage = 40;
	}
}

public class SprayNPray : RangedCard
{
	public SprayNPray()
	{
		name = "Spray And Pray";
		description = "Quantity over quality (Targets random enemy)";
		image = SpriteBase.mainSpriteBase.bullets;
		targetType = TargetType.Random;

		ammoCost = 2;
		healthDamage = 30;
	}
}

public class Kick : MeleeCard
{
	public Kick()
		: base()
	{
		name = "Kick";
		description = "Guard break (ignores armor)";
		image = SpriteBase.mainSpriteBase.brokenLegsSprite;
		targetType = TargetType.SelectEnemy;

		staminaCost = 5;
		healthDamage = 25;
	}

	protected override void CardPlayEffects()
	{
		userCharGraphic.IncrementStamina(-staminaCost);
		targetChars[0].TakeDamage(healthDamage, false);
	}
}
