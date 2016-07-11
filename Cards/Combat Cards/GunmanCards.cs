using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunmanCards
{
	public static CombatDeck GetClassCards()
	{
		CombatDeck result = new CombatDeck();

		result.AddCards(typeof(Pistolwhip));
		result.AddCards(typeof(Jab));
		result.AddCards(typeof(Sidearm), 2);
		result.AddCards(typeof(BurstFire));
		result.AddCards(typeof(FullAuto));
		result.AddCards(typeof(LimbShot), 2);
		result.AddCards(typeof(ScopeIn));
		result.AddCards(typeof(DetonateAmmo));
		result.AddCards(typeof(Tripmine));
		result.AddCards(typeof(HollowPoint));

		return result;
	}

}

public class FullAuto : RangedCard
{
	public FullAuto()
		: base()
	{
		name = "Full Auto";
		description = "Say hello";
		image = SpriteBase.mainSpriteBase.skull;

		damage = 80;
		ammoCost = 3;
		staminaCost = 1;
		targetType = TargetType.SelectEnemy;
	}
}

public class TrickMags : EffectCard
{
	public TrickMags()
		: base()
	{
		targetType = TargetType.None;
		staminaCost = 4;
		userAmmoGain = 2;
		
		name = "Trick Mags";
		description = "Gain " + userAmmoGain + " ammo";
		image = SpriteBase.mainSpriteBase.bullets;
		
	}
}

public class Tripmine : EffectCard
{
	int mineDamage;
	public Tripmine()
		: base()
	{
		name = "Tripmine";
		description = "Places a Trip Mine in the room";
		image = SpriteBase.mainSpriteBase.bomb;
		targetType = TargetType.None;
		ammoCost = 1;
		mineDamage = 40;
		addedStipulationCard = new TripmineFriendly(mineDamage);
	}
}

public class HollowPoint : RangedCard
{	
	public HollowPoint()
		: base()
	{
		damage = 10;
		unarmoredBonusDamage = 30;
		ammoCost = 1;
		targetType = TargetType.SelectEnemy;
		
		name = "Hollow Point";
		description = "If target has no armor, deal " + unarmoredBonusDamage + " extra damage";
		image = SpriteBase.mainSpriteBase.crosshair;
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

		staminaCost = 2;
		damage = 20;
	}
}
//Unused
public class Doubletap : RangedCard
{
	public Doubletap()
		: base() //x2
	{
		name = "Double Tap";
		description = "Always";
		image = SpriteBase.mainSpriteBase.bullet;

		damage = 30;
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

		damage = 30;
		ammoCost = 1;
		staminaCost = 1;
		ignoresArmor = true;
		targetType = TargetType.SelectEnemy;
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

		staminaDamage = 2;
		staminaCost = 2;
		targetType = TargetType.SelectEnemy;
	}
}

public class DetonateAmmo : RangedCard
{
	int bonusDamage= 20;

	public DetonateAmmo()
		: base()
	{
		name = "Detonate Ammo";
		description = "If target has ammo, remove ammo and deal extra "+bonusDamage+" damage";
		image = SpriteBase.mainSpriteBase.flamingBullet;

		damage = 10;
		ammoCost = 2;
		targetType = TargetType.SelectEnemy;
	}

	protected override void DamageTargets()
	{
		int totalDamage = damage;
		if (targetChars[0].GetAmmo() > 0)
			totalDamage += bonusDamage;
		targetChars[0].SetAmmo(0);
		targetChars[0].TakeDamage(totalDamage);
	}
}
