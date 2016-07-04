using UnityEngine;
using System.Collections;

public class BanditCards
{
	public static CombatDeck GetClassCards()
	{
		CombatDeck result = new CombatDeck();

		result.AddCards(typeof(SuckerPunch),2);
		result.AddCards(typeof(LightsOut));
		result.AddCards(typeof(Jab));
		result.AddCards(typeof(Roundhouse));
		result.AddCards(typeof(Kick), 2);
		result.AddCards(typeof(Execution), 2);
		result.AddCards(typeof(Sidearm));
		result.AddCards(typeof(Throw),2);

		return result;
	}

}

public class SuckerPunch : MeleeCard
{
	public SuckerPunch()
	{
		name = "Sucker Punch";
		description = "Won't know what him 'em";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;
		targetType = TargetType.SelectEnemy;

		staminaCost = 3;
		damage = 40;
	}
}

public class LightsOut : MeleeCard
{
	public LightsOut()
		: base()
	{
		name = "Lights Out";
		description = "Like a truck";
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

		staminaCost = 5;
		damage = 60;
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

		staminaCost = 2;
		damage = 40;
	}
}
//UNused
public class SprayNPray : RangedCard
{
	public SprayNPray()
	{
		name = "Spray And Pray";
		description = "Quantity over quality (Targets random enemy)";
		image = SpriteBase.mainSpriteBase.bullets;
		targetType = TargetType.Random;

		ammoCost = 2;
		damage = 30;
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
		ignoresArmor = true;

		staminaCost = 4;
		damage = 30;
	}
}
