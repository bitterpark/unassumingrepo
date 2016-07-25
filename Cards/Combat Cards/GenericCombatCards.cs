using UnityEngine;
using System.Collections;

public class Smash : MeleeCard
{
	protected override void ExtenderConstructor()
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
	protected override void ExtenderConstructor()
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
	protected override void ExtenderConstructor()
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
	protected override void ExtenderConstructor()
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
	protected override void ExtenderConstructor()
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
	protected override void ExtenderConstructor()
	{
		name = "Sidearm";
		description = "Trick gun";
		image = SpriteBase.mainSpriteBase.nineMSprite;
		targetType = TargetType.SelectEnemy;
		damage = 10;
	}
}

public class SetupDefence : EffectCard
{
	protected override void ExtenderConstructor()
	{
		name = "Setup Defence";
		description = "Adds a Melee Defence to user";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;
		targetType = TargetType.None;
		staminaCost = 1;
		addedStipulationCard = new MeleeDefence();
	}

	public class MeleeDefence : CharacterStipulationCard
	{
		int damage = 20;

		public MeleeDefence()
		{
			//SetLastsForRounds(1);
			
			name = "Melee Defence";
			image = SpriteBase.mainSpriteBase.brokenArmsSprite;

			description = "When a melee attack is played, the attacker takes " + damage + " damage";

			addedCombatCards.Add(new Jab());
			addedCombatCards.Add(new Jab());
			addedCombatCards.Add(new Jab());
		}

		protected override void ExtenderSpecificActivation()
		{
			MeleeCard.EMeleeCardPlayed += Detonate;
		}
		void Detonate(CharacterGraphic cardPlayer, MeleeCard playedCard)
		{
			if (cardPlayer.GetType() != typeof(MercGraphic))
			{
				//characterGraphic.RemoveCharacterCard(this);
				cardPlayer.TakeDamage(damage);
			}

		}
		protected override void ExtenderSpecificDeactivation()
		{
			MeleeCard.EMeleeCardPlayed -= Detonate;
		}
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
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;
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