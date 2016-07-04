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

		damage = 30;
		staminaCost = 2;
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

		damage = 20;
		staminaCost = 1;
		targetType = TargetType.SelectEnemy;
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

		damage = 30;
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

		staminaCost = 1;
		damage = 20;
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

		damage = 50;
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
		damage = 10;
	}
}