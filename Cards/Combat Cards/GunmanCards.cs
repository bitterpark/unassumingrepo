using UnityEngine;
using System.Collections;

public class TrickMags : Effect
{
	int ammoGain = 2;
	public TrickMags()
		: base()
	{
		name = "Trick Mags";
		description = "Gain " + ammoGain + " ammo";
		image = SpriteBase.mainSpriteBase.bullets;
		targetType = TargetType.None;
		staminaCost = 4;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.IncrementAmmo(ammoGain);
	}
}

public class Tripmine : Effect
{
	public Tripmine()
		: base()
	{
		name = "Tripmine";
		description = "Places a Trip Mine in the room";
		image = SpriteBase.mainSpriteBase.bomb;
		targetType = TargetType.None;
		ammoCost = 2;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		CardsScreen.main.PlaceRoomCard(new TripmineFriendly());
	}
}

public class HollowPoint : MeleeCard
{
	int bonusDamage = 15;
	
	public HollowPoint()
		: base()
	{
		name = "Hollow Point";
		description = "If target has no armor, deal "+bonusDamage+" extra damage";
		image = SpriteBase.mainSpriteBase.crosshair;

		healthDamage = 5;
		ammoCost = 1;
		targetType = TargetType.SelectEnemy;
	}

	protected override void CardPlayEffects()
	{
		userCharGraphic.IncrementAmmo(-ammoCost);
		int totalDamage = healthDamage;
		if (targetChars[0].GetArmor() == 0)
			totalDamage += bonusDamage;

		targetChars[0].TakeDamage(totalDamage);
	}
}

public class Pistolwhip : MeleeCard
{
	public Pistolwhip()
		: base() //x2
	{
		name = "Pistol Whip";
		description = "Guns don't kill people";
		image = SpriteBase.mainSpriteBase.nineMSprite;
		targetType = TargetType.SelectEnemy;

		staminaCost = 3;
		healthDamage = 10;
	}
}

public class Doubletap : RangedCard
{
	public Doubletap()
		: base() //x2
	{
		name = "Double Tap";
		description = "Always";
		image = SpriteBase.mainSpriteBase.bullet;

		healthDamage = 10;
		ammoCost = 1;
		targetType = TargetType.SelectEnemy;
	}
}

public class ScopeIn : RangedCard
{
	public ScopeIn()
		: base()
	{
		name = "Scope In";
		description = "Whites of their eyes (ignores armor)";
		image = SpriteBase.mainSpriteBase.crosshair;

		healthDamage = 40;
		ammoCost = 3;
		targetType = TargetType.SelectEnemy;
	}

	protected override void CardPlayEffects()
	{
		targetChars[0].TakeDamage(healthDamage, true);
	}
}

public class LimbShot : RangedCard
{
	public LimbShot()
		: base()
	{
		name = "Limb Shot";
		description = "Let them suffer";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;

		healthDamage = 5;
		staminaDamage = 3;
		ammoCost = 2;
		targetType = TargetType.SelectEnemy;
	}
}

public class DetonateAmmo : RangedCard
{
	int bonusDamage= 10;

	public DetonateAmmo()
		: base()
	{
		name = "Detonate Ammo";
		description = "If target has ammo, remove ammo and deal extra "+bonusDamage+" damage";
		image = SpriteBase.mainSpriteBase.flamingBullet;

		healthDamage = 10;
		ammoCost = 2;
		targetType = TargetType.SelectEnemy;
	}

	protected override void CardPlayEffects()
	{
		int totalDamage = healthDamage;
		if (targetChars[0].GetAmmo() > 0)
			totalDamage += bonusDamage;
		targetChars[0].SetAmmo(0);
		targetChars[0].TakeDamage(totalDamage);
		base.CardPlayEffects();
	}
}
