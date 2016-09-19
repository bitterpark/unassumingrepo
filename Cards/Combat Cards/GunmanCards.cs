using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunmanCards
{
	public static CombatDeck GetClassCards()
	{
		CombatDeck result = new CombatDeck();

		/*
		result.AddCards(typeof(Pistolwhip));
		result.AddCards(typeof(Hipfire),2);
		result.AddCards(typeof(BurstFire));
		result.AddCards(typeof(FullAuto));
		result.AddCards(typeof(Camo));
		result.AddCards(typeof(Tripmine));
		result.AddCards(typeof(LimbShot));
		result.AddCards(typeof(HollowPoint));
		result.AddCards(typeof(PersonalSpace));
		*/
		result.AddCards(typeof(DetonateAmmo));
		result.AddCards(typeof(Hipfire));
		result.AddCards(typeof(BurstFire));
		result.AddCards(typeof(Tripmine));
		result.AddCards(typeof(FullAuto));

		return result;
	}

	public static List<PrepCard> GetClassPrepCards()
	{
		List<PrepCard> result = new List<PrepCard>();
		//result.Add(new RangeBlockerPrep());
		//result.Add(new Sniper());
		//result.Add(new Sprayer());
		//result.Add(new Prepared());


		return result;
	}

	

	public class Prepared : PrepCard
	{
		public Prepared()
		{
			name = "Prepared";
			description = placedStipulationCard.description;
			image = SpriteBase.mainSpriteBase.flamingBullet;
		}
	}

	public class Sniper : PrepCard
	{
		public Sniper()
		{
			placedStipulationCard = new ScopeIn();
			
			name = "Sniper";
			description = placedStipulationCard.description;
			image = SpriteBase.mainSpriteBase.crosshair;
		}
	}

	public class ScopeIn : CharacterStipulationCard
	{
		RangedCard affectedCard = null;

		int maxStaminaCost = 1;

		public ScopeIn()
		{
			name = "Scope In";
			image = SpriteBase.mainSpriteBase.crosshair;
			description = "Spend "+maxStaminaCost+" max stamina, character's ranged attacks ignore blocks";
		}

		protected override void ExtenderSpecificActivation()
		{
			appliedToCharacter.IncrementMaxStamina(-maxStaminaCost);
			RangedCard.ERangedCardPlayed += TriggerCard;
		}
		void TriggerCard(CharacterGraphic cardPlayer, RangedCard playedCard)
		{
			if (cardPlayer == appliedToCharacter)
			{
				affectedCard = playedCard;
				affectedCard.SetIgnoresBlocks(true);
				//appliedToCharacter.RemoveCharacterStipulationCard(this);
			}
		}

		protected override void ExtenderSpecificDeactivation()
		{
			//affectedCard.SetDefaultState();
			RangedCard.ERangedCardPlayed -= TriggerCard;
		}
	}

	public class Sprayer : PrepCard
	{
		int ammoPerPointOfMaxStamina = 1;
		
		public Sprayer()
		{
			placedStipulationCard = new TrickMags();

			
			name = "Sprayer";
			description = placedStipulationCard.description;//"Reduce max stamina to 0, gain "+ammoPerPointOfMaxStamina+" ammo per point of max stamina";
			image = SpriteBase.mainSpriteBase.bullets;

			//addedCombatCards.Add(new TrickMags());
		}
	}

	public class TrickMags : CharacterStipulationCard
	{
		int ammoPerPointOfMaxStamina = 1;
		public TrickMags()
		{
			name = "Trick Mags";
			image = SpriteBase.mainSpriteBase.bullets;
			description = "Set max stamina to 0, gain "+ammoPerPointOfMaxStamina+" ammo per point of max stamina";
		}

		protected override void ExtenderSpecificActivation()
		{
			int mercsMaxStamina = appliedToCharacter.GetMaxStamina();
			int ammoIncrement = mercsMaxStamina * ammoPerPointOfMaxStamina;
			appliedToCharacter.IncrementMaxStamina(-mercsMaxStamina);
			appliedToCharacter.IncrementAmmo(ammoIncrement);
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
		description = addedStipulationCard.description;
		image = SpriteBase.mainSpriteBase.ammoBox;

	}

	public class ExtendedMag : CharacterStipulationCard
	{
		int ammoCostReduction = 1;

		public ExtendedMag()
		{
			name = "Extended Mag";
			image = SpriteBase.mainSpriteBase.bullets;
			description = "Character's next ranged attack costs "+ammoCostReduction+" less ammo";
		}

		protected override void ExtenderSpecificActivation()
		{
			appliedToCharacter.ChangeRangedAttacksAmmoCostReduction(ammoCostReduction);
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
			appliedToCharacter.ChangeRangedAttacksAmmoCostReduction(-ammoCostReduction);
		}
	}
}
//deprecated
public class Headshot : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		//addedStipulationCard = new ScopeIn();
		staminaCost = 1;

		name = "Headshot";
		description = addedStipulationCard.description;
		image = SpriteBase.mainSpriteBase.skull;

	}
}

public class Tripmine : EffectCard
{
	int mineDamage;
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		ammoCost = 1;
		mineDamage = 10;
		addedStipulationCard = new TripmineFriendly(mineDamage);
		
		name = "Tripmine";
		description = addedStipulationCard.description;
		image = SpriteBase.mainSpriteBase.bomb;
	}
}

public class Camo : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		staminaCost = 1;
		userArmorGain = 20;

		name = "Camo";
		description = "Gain " + userArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.arrow;
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

		damage = 20;
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
		image = SpriteBase.mainSpriteBase.arm;

		staminaDamage = 2;
		ammoCost = 1;
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

		damage = 20;
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
		image = SpriteBase.mainSpriteBase.pistol;
		targetType = TargetType.SelectEnemy;

		damage = 10;
	}
}

public class PersonalSpace : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		takeDamageCost = 10;
		ammoCost = 1;
		damage = 30;
		
		name = "Personal Space";
		description = "Too close for comfort (take "+takeDamageCost+" damage)";
		image = SpriteBase.mainSpriteBase.bullet;
		targetType = TargetType.SelectEnemy;
	}
}
