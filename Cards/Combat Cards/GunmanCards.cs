﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunmanCards
{
	public static CombatDeck GetClassCards()
	{
		CombatDeck result = new CombatDeck();

		result.AddCards(typeof(Pistolwhip));
		result.AddCards(typeof(Hipfire));
		result.AddCards(typeof(BurstFire));
		result.AddCards(typeof(FullAuto));
		result.AddCards(typeof(Camo));
		result.AddCards(typeof(Tripmine));
		result.AddCards(typeof(LimbShot));
		result.AddCards(typeof(DetonateAmmo));
		result.AddCards(typeof(HollowPoint));

		return result;
	}

	public static List<PrepCard> GetClassPrepCards()
	{
		List<PrepCard> result = new List<PrepCard>();
		result.Add(new Sniper());
		result.Add(new Sprayer());
		result.Add(new Prepared());


		return result;
	}

	public class Prepared : PrepCard
	{
		public Prepared()
		{
			name = "Prepared";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.medal;

			//addedCombatCards.Add(new Camo());
			addedCombatCards.Add(new PersonalSpace());
			//addedCombatCards.Add(new Pistolwhip());
			//addedCombatCards.Add(new Tripmine());
		}
	}

	public class Sniper : PrepCard
	{
		public Sniper()
		{
			name = "Sniper";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.crosshair;

			//addedCombatCards.Add(new LimbShot());
			//addedCombatCards.Add(new LimbShot());
			addedCombatCards.Add(new Headshot());
			//addedCombatCards.Add(new DetonateAmmo());
		}
	}
	public class Sprayer : PrepCard
	{
		public Sprayer()
		{
			name = "Sprayer";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.bullets;

			//addedCombatCards.Add(new FullAuto());
			//addedCombatCards.Add(new HollowPoint());
			//addedCombatCards.Add(new TrickMags());
			addedCombatCards.Add(new TrickMags());
		}
	}

}

public class TrickMags : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		addedStipulationCard = new ExtendedMag();
		staminaCost = 1;

		name = "Trick Mags";
		description = "Character's next ranged attack costs no ammo";
		image = SpriteBase.mainSpriteBase.ammoBoxSprite;

	}

	public class ExtendedMag : CharacterStipulationCard
	{
		public ExtendedMag()
		{
			name = "Extended Mag";
			image = SpriteBase.mainSpriteBase.bullets;
			description = "Character's next ranged attack costs no ammo";
		}

		protected override void ExtenderSpecificActivation()
		{
			appliedToCharacter.SetRangedAttacksFree(true);
			RangedCard.ERangedCardPlayed += UseUpCard;
		}
		void UseUpCard(CharacterGraphic cardPlayer,RangedCard playedCard)
		{
			if (cardPlayer==appliedToCharacter)
				appliedToCharacter.RemoveCharacterStipulationCard(this);
		}

		protected override void ExtenderSpecificDeactivation()
		{
			RangedCard.ERangedCardPlayed -= UseUpCard;
			appliedToCharacter.SetRangedAttacksFree(false);
		}
	}
}

public class Headshot : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		addedStipulationCard = new ScopeIn();
		staminaCost = 1;

		name = "Headshot";
		description = "Play a Scope In card to the character";
		image = SpriteBase.mainSpriteBase.skull;

	}

	public class ScopeIn : CharacterStipulationCard
	{
		RangedCard affectedCard = null;
		
		public ScopeIn()
		{
			name = "Scope In";
			image = SpriteBase.mainSpriteBase.crosshair;
			description = "Character's next ranged attack ignores armor";
		}

		protected override void ExtenderSpecificActivation()
		{
			RangedCard.ERangedCardPlayed += UseUpCard;
		}
		void UseUpCard(CharacterGraphic cardPlayer,RangedCard playedCard)
		{
			if (cardPlayer == appliedToCharacter)
			{
				affectedCard = playedCard;
				affectedCard.SetIgnoresArmor(true);
				appliedToCharacter.RemoveCharacterStipulationCard(this);
			}
		}

		protected override void ExtenderSpecificDeactivation()
		{
			//affectedCard.SetDefaultState();
			RangedCard.ERangedCardPlayed -= UseUpCard;
		}
	}
}

public class Tripmine : EffectCard
{
	int mineDamage;
	protected override void ExtenderConstructor()
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

public class Camo : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		staminaCost = 1;
		userArmorGain = 30;

		name = "Camo";
		description = "Gain " + userArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.arrow;
	}
}

public class FullAuto : RangedCard
{
	protected override void ExtenderConstructor()
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

public class HollowPoint : RangedCard
{
	protected override void ExtenderConstructor()
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

//Unused
public class Doubletap : RangedCard
{
	protected override void ExtenderConstructor()
	{
		name = "Double Tap";
		description = "Always";
		image = SpriteBase.mainSpriteBase.bullet;

		damage = 30;
		ammoCost = 1;
		targetType = TargetType.SelectEnemy;
	}
}

public class LimbShot : RangedCard
{
	protected override void ExtenderConstructor()
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

	protected override void ExtenderConstructor()
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

public class Pistolwhip : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		name = "Pistol Whip";
		description = "Guns don't kill people";
		image = SpriteBase.mainSpriteBase.nineMSprite;
		targetType = TargetType.SelectEnemy;

		staminaCost = 2;
		damage = 20;
	}
}

public class PersonalSpace : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		takeDamageCost = 10;
		ammoCost = 1;
		damage = 40;
		
		name = "Personal Space";
		description = "Too close for comfort (take "+takeDamageCost+" damage)";
		image = SpriteBase.mainSpriteBase.bullet;
		targetType = TargetType.SelectEnemy;
	}
}
