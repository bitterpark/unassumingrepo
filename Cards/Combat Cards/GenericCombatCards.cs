using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenericCombatCards
{
	public static List<CombatCard> GetDefaultCommonDeckCards()
	{
		List<CombatCard> cards = new List<CombatCard>();

		cards.Add(new Charge());
		cards.Add(new Charge());
		cards.Add(new Dodge());
		cards.Add(new Dodge());
		cards.Add(new DrawABead());
		cards.Add(new DrawABead());
		cards.Add(new MakeDistance());
		cards.Add(new MakeDistance());
		cards.Add(new Defillade());
		cards.Add(new Defillade());
		cards.Add(new StayLow());
		cards.Add(new Blitz());
		cards.Add(new Blitz());
		cards.Add(new Aim());
		cards.Add(new Aim());
		cards.Add(new FullCover());
		cards.Add(new FullCover());
		cards.Add(new Flank());
		cards.Add(new Suppress());
		cards.Add(new SuicideRush());
		cards.Add(new SetUp());
		cards.Add(new SetUp());
		cards.Add(new TakeCover());
		cards.Add(new TakeCover());
		cards.Add(new ScopeIn());
		cards.Add(new ScopeIn());
		cards.Add(new RunInterference());
		cards.Add(new RunInterference());
		cards.Add(new CoverFire());
		cards.Add(new CoverFire());
		cards.Add(new Distraction());
		cards.Add(new Distraction());


		return cards;
	}
}



//Default common deck cards
public class Charge : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		userStaminaGain = 1;

		name = "Charge";
		description = "Gain "+userStaminaGain+" melee power";
		image = SpriteBase.mainSpriteBase.arrow;
	}
}

public class DrawABead : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		userAmmoGain = 1;

		name = "Draw a Bead";
		description = "Gain " + userAmmoGain + " ranged power";
		image = SpriteBase.mainSpriteBase.crosshair;
	}
}
public class Dodge : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		userArmorGain = 10;

		name = "Dodge";
		description = "Gain " + userArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.lateralArrows;
	}
}

public class MakeDistance: EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		staminaCost = 1;
		addedStipulationCard = new MeleeBlock();

		name = "Make Distance";
		description = "Gain melee block";
		image = SpriteBase.mainSpriteBase.arrow;
	}
}

public class Defillade : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		ammoCost = 1;
		addedStipulationCard = new RangeBlock();

		name = "Defillade";
		description = "Gain ranged block";
		image = SpriteBase.mainSpriteBase.rock;
	}
}

public class StayLow : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		ammoCost = 1;
		staminaCost = 1;
		addedStipulationCard = new FullBlock();

		name = "Stay Low";
		description = "Gain full block";
		image = SpriteBase.mainSpriteBase.restSprite;
	}
}

public class Blitz : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		userStaminaGain = 2;

		name = "Blitz";
		description = "Gain " + userStaminaGain + " melee power";
		image = SpriteBase.mainSpriteBase.lightning;
	}
}
public class Aim : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		userAmmoGain = 2;

		name = "Aim";
		description = "Gain " + userAmmoGain + " ranged power";
		image = SpriteBase.mainSpriteBase.crosshair;
	}
}
public class FullCover : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		userArmorGain = 20;

		name = "Full Cover";
		description = "Gain " + userArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.cover;
	}
}

public class Flank : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectEnemy;
		staminaDamage = 1;

		name = "Flank";
		description = "Target loses " + staminaDamage + " melee power";
		image = SpriteBase.mainSpriteBase.lateralArrows;
	}
}
public class Suppress : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectEnemy;
		ammoDamage = 1;

		name = "Suppress";
		description = "Target loses " + ammoDamage + " ranged power";
		image = SpriteBase.mainSpriteBase.flamingBullet;
	}
}

public class SuicideRush : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		takeDamageCost = 30;
		userStaminaGain = 1;


		name = "Suicide Rush";
		description = "Spend "+takeDamageCost+" armor, gain "+userStaminaGain+" stamina";
		image = SpriteBase.mainSpriteBase.skull;
	}
}
public class SetUp : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		staminaCost = 1;
		userAmmoGain = 1;
		
		name = "Set Up";
		description = "Spend " + staminaCost + " melee power, gain " + userAmmoGain + " ranged power";
		image = SpriteBase.mainSpriteBase.rock;
	}
}
public class TakeCover : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		userArmorGain = 40;
		staminaCost = 1;

		name = "Take Cover";
		description = "Spend " + staminaCost + " melee power, gain " + userArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.cover;
	}
}

public class ScopeIn : EffectCard
{
	float ammoPerStaminaPoint = 0.5f;

	protected override bool ExtenderPrerequisitesMet(CharacterGraphic user)
	{
		return user.GetStamina() > 1;
	}
	
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		useUpAllStamina = true;

		name = "Scope In";
		description = "Spend all melee power, gain " + 1 + " ranged power per 2 melee power";
		image = SpriteBase.mainSpriteBase.crosshair;
	}

	protected override void ApplyStatEffects()
	{
		base.ApplyStatEffects();
		int rangedPowerGain = Mathf.RoundToInt(usedUpStaminaPoints * ammoPerStaminaPoint);
		userCharGraphic.IncrementAmmo(rangedPowerGain);

	}
}

public class RunInterference : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectFriendlyOther;
		staminaCost = 2;
		targetAmmoGain = 1;


		name = "Run Interference";
		description = "Spend " + staminaCost + " melee power, target gains " + targetAmmoGain + " ranged power";
		image = SpriteBase.mainSpriteBase.lightning;
	}
}
public class CoverFire : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectFriendlyOther;
		ammoCost = 1;
		targetStaminaGain = 2;

		name = "Cover Fire";
		description = "Spend " + staminaCost + " ranged power, target gains " + userAmmoGain + " melee power";
		image = SpriteBase.mainSpriteBase.bullets;
	}
}
public class Distraction : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectFriendlyOther;
		takeDamageCost = 30;
		targetArmorGain = 30;

		name = "Distraction";
		description = "Take " + takeDamageCost + " damage, target gains " + targetArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.fire;
	}
}
//

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

public class DoubleTap : RangedCard
{
	protected override void ExtenderConstructor()
	{
		name = "Double Tap";
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
		image = SpriteBase.mainSpriteBase.crosshair;
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
		addedStipulationCard = new RangeBlock();

		name = "Smokescreen";
		description = "The user and all friendly characters gain a ranged block";
		image = SpriteBase.mainSpriteBase.cloud;

	}
}


public class FullBlock : CharacterStipulationCard
{
	public FullBlock()
	{
		name = "Full Block";
		image = SpriteBase.mainSpriteBase.cover;
		description = "Blocks one melee or ranged attack";
	}

	protected override void ExtenderSpecificActivation()
	{
		appliedToCharacter.SetRangedBlock(true);
		appliedToCharacter.SetMeleeBlock(true);
		appliedToCharacter.ERangedBlockAssigned += RemoveStipulationCard;
		appliedToCharacter.EMeleeBlockAssigned += RemoveStipulationCard;
		RangedCard.ERangedCardPlayed += TryTriggerBlock;
		MeleeCard.EMeleeCardPlayed += TryTriggerBlock;
	}

	protected override void ExtenderSpecificDeactivation()
	{
		appliedToCharacter.SetRangedBlock(false);
		appliedToCharacter.SetMeleeBlock(false);
		appliedToCharacter.ERangedBlockAssigned -= RemoveStipulationCard;
		appliedToCharacter.EMeleeBlockAssigned -= RemoveStipulationCard;
		RangedCard.ERangedCardPlayed -= TryTriggerBlock;
		MeleeCard.EMeleeCardPlayed -= TryTriggerBlock;
	}

	void TryTriggerBlock(CharacterGraphic cardPlayer, CombatCard playedCard)
	{
		if (playedCard.targetChars.Contains(appliedToCharacter) && !playedCard.ignoresBlocks)
		{
			playedCard.targetChars.Remove(appliedToCharacter);
			RemoveStipulationCard();
		}
	}
	void RemoveStipulationCard()
	{
		appliedToCharacter.RemoveCharacterStipulationCard(this);
	}
}

public class MeleeBlock : CharacterStipulationCard
{
	public MeleeBlock()
	{
		name = "Melee Block";
		image = SpriteBase.mainSpriteBase.arm;
		description = "Blocks one melee attack";
	}

	protected override void ExtenderSpecificActivation()
	{
		appliedToCharacter.SetMeleeBlock(true);
		appliedToCharacter.EMeleeBlockAssigned += RemoveStipulationCard;
		MeleeCard.EMeleeCardPlayed += TryTriggerBlock;
	}

	protected override void ExtenderSpecificDeactivation()
	{
		appliedToCharacter.SetMeleeBlock(false);
		appliedToCharacter.EMeleeBlockAssigned -= RemoveStipulationCard;
		MeleeCard.EMeleeCardPlayed -= TryTriggerBlock;
	}

	void TryTriggerBlock(CharacterGraphic cardPlayer, MeleeCard playedCard)
	{
		if (playedCard.targetChars.Contains(appliedToCharacter) && !playedCard.ignoresBlocks)
		{
			playedCard.targetChars.Remove(appliedToCharacter);
			RemoveStipulationCard();
		}
	}
	void RemoveStipulationCard()
	{
		appliedToCharacter.RemoveCharacterStipulationCard(this);
	}
}

public class RangeBlock : CharacterStipulationCard
{

	public RangeBlock()
	{
		name = "Range Block";
		image = SpriteBase.mainSpriteBase.crosshair;
		description = "Blocks one ranged attack";
	}

	protected override void ExtenderSpecificActivation()
	{
		appliedToCharacter.SetRangedBlock(true);
		appliedToCharacter.ERangedBlockAssigned += RemoveStipulationCard;
		RangedCard.ERangedCardPlayed += TryTriggerBlock;
	}

	protected override void ExtenderSpecificDeactivation()
	{
		appliedToCharacter.SetRangedBlock(false);
		appliedToCharacter.ERangedBlockAssigned -= RemoveStipulationCard;
		RangedCard.ERangedCardPlayed -= TryTriggerBlock;
	}

	void TryTriggerBlock(CharacterGraphic cardPlayer, RangedCard playedCard)
	{
		if (playedCard.targetChars.Contains(appliedToCharacter) && !playedCard.ignoresBlocks)
		{
			playedCard.targetChars.Remove(appliedToCharacter);
			RemoveStipulationCard();
		}
	}
	void RemoveStipulationCard()
	{
		appliedToCharacter.RemoveCharacterStipulationCard(this);
	}
}

public class RangeBlockerPrep : PrepCard
{
	public RangeBlockerPrep()
	{
		placedStipulationCard = new RangeBlock();

		name = "Range blocker";
		description = "Play " + placedStipulationCard.name;
		image = SpriteBase.mainSpriteBase.crosshair;
	}

}