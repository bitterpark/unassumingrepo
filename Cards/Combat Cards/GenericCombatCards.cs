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