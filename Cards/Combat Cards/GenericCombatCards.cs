using UnityEngine;
using System.Collections;

public class Smash : MeleeCard
{
	public Smash()
		: base()
	{
		name = "Smash";
		description = "When in doubt - punch things";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;

		healthDamage = 30;
		staminaCost = 4;
		targetType = TargetType.SelectEnemy;
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
		staminaCost = 1;
		targetType = TargetType.SelectEnemy;
	}
}
//Unused
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


public class Hipfire : RangedCard
{
	public Hipfire()
		: base()
	{
		name = "Hipfire";
		description = "Aiming is for nerds";
		image = SpriteBase.mainSpriteBase.bullet;

		healthDamage = 10;
		ammoCost = 1;
		targetType = TargetType.SelectEnemy;
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
		targetType = TargetType.SelectEnemy;

		staminaCost = 2;
		healthDamage = 10;
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
		ammoCost = 2;
		targetType = TargetType.SelectEnemy;
	}
}

public class Sidearm : RangedCard
{
	public Sidearm()
		: base() //x2
	{
		name = "Sidearm";
		description = "Trick gun";
		image = SpriteBase.mainSpriteBase.nineMSprite;
		targetType = TargetType.SelectEnemy;
		healthDamage = 5;
	}
}