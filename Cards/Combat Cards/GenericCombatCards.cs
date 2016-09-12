﻿using UnityEngine;
using System.Collections;

public class Smash : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		name = "Smash";
		description = "When in doubt - punch things";
		image = SpriteBase.mainSpriteBase.arm;

		damage = 20;
		staminaCost = 1;
		targetType = TargetType.SelectEnemy;
	}
}

public class Jab : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		name = "Jab";
		description = "Left hand is enough";
		image = SpriteBase.mainSpriteBase.arm;

		damage = 10;
		targetType = TargetType.SelectEnemy;
	}
}



public class Hipfire : RangedCard
{
	protected override void ExtenderConstructor()
	{
		name = "Hipfire";
		description = "Aiming is for nerds";
		image = SpriteBase.mainSpriteBase.bullet;

		damage = 20;
		ammoCost = 1;
		targetType = TargetType.SelectEnemy;
	}
}

public class Throw : RangedCard
{
	protected override void ExtenderConstructor()
	{
		name = "Throw";
		description = "Everything is fair";
		image = SpriteBase.mainSpriteBase.arm;
		targetType = TargetType.SelectEnemy;

		staminaCost = 1;
		damage = 20;
	}
}

public class BurstFire : RangedCard
{
	protected override void ExtenderConstructor()
	{
		name = "Burst Fire";
		description = "Walk the shots";
		image = SpriteBase.mainSpriteBase.bullets;

		damage = 40;
		ammoCost = 2;
		targetType = TargetType.SelectEnemy;
	}
}

public class FullAuto : RangedCard
{
	int bonusDamagePerAmmoPoint = 20;

	protected override bool ExtenderPrerequisitesMet(CharacterGraphic user)
	{
		return user.GetAmmo() > 0;
	}

	protected override void ExtenderConstructor()
	{
		name = "Full Auto";
		description = "Spend all ammo, deal " + bonusDamagePerAmmoPoint + " damage per point of ammo";
		image = SpriteBase.mainSpriteBase.skull;

		damage = 20;
		staminaCost = 1;
		targetType = TargetType.SelectEnemy;
	}

	protected override void ApplyPlayCosts()
	{
		damage += userCharGraphic.GetAmmo() * bonusDamagePerAmmoPoint;
		ammoCost = userCharGraphic.GetAmmo();
		base.ApplyPlayCosts();
	}
}

public class Sidearm : RangedCard
{
	protected override void ExtenderConstructor()
	{
		name = "Sidearm";
		description = "Trick gun";
		image = SpriteBase.mainSpriteBase.pistol;
		targetType = TargetType.SelectEnemy;
		damage = 10;
	}
}

public class CrossFire : CharacterStipulationCard
{
	int damagePerRangedAttack = 20;
	System.Type appropriateAttackerType;

	public CrossFire(bool playedByEnemies)
	{
		if (playedByEnemies)
			appropriateAttackerType = typeof(EnemyGraphic);
		else
			appropriateAttackerType = typeof(MercGraphic);

		SetLastsForRounds(1);

		name = "Crossfire";
		image = SpriteBase.mainSpriteBase.arm;
		description = "For one round: character takes " + damagePerRangedAttack + " extra damage from every ranged attack ";
	}

	protected override void ExtenderSpecificActivation()
	{
		RangedCard.ERangedCardPlayed += TriggerEffect;
	}

	void TriggerEffect(CharacterGraphic rangedCardPlayer, RangedCard playedCard)
	{


		if (rangedCardPlayer.GetType() == appropriateAttackerType
			&& playedCard.targetChars[0] == appliedToCharacter)
			appliedToCharacter.TakeDamage(damagePerRangedAttack);
	}

	protected override void ExtenderSpecificDeactivation()
	{
		RangedCard.ERangedCardPlayed -= TriggerEffect;
	}
}

public class Smokescreen : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.AllFriendlies;
		ammoCost = 2;
		targetArmorGain = 30;

		name = "Smokescreen";
		description = "The user and all friendly characters gain " + targetArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.cover;

	}
}