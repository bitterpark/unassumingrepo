using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BanditCards
{
	public static CombatDeck GetClassCards()
	{
		CombatDeck result = new CombatDeck();
		/*
		result.AddCards(typeof(SuckerPunch));
		result.AddCards(typeof(Roundhouse),2);
		result.AddCards(typeof(Kick));
		result.AddCards(typeof(LightsOut),2);
		result.AddCards(typeof(Throw));
		result.AddCards(typeof(Execution));
		result.AddCards(typeof(SprayNPray));
		result.AddCards(typeof(Sidearm));
		*/
		//result.AddCards(typeof(GangUp));
		//result.AddCards(typeof(NoRest));
		result.AddCards(typeof(MobMentality));
		result.AddCards(typeof(SuckerPunch));
		result.AddCards(typeof(Kick));
		result.AddCards(typeof(SprayNPray));
		result.AddCards(typeof(LightsOut));

		return result;
	}

	public static List<PrepCard> GetClassPrepCards()
	{
		List<PrepCard> result = new List<PrepCard>();
		result.Add(new Muscle());
		result.Add(new Powerhouse());
		result.Add(new Thug());
		//result.Add(new RangeBlockerPrep());


		return result;
	}

	public class Thug : PrepCard
	{
		public Thug()
		{
			placedStipulationCard = new StrengthInNumbers();
			
			name = "Thug";
			description = placedStipulationCard.description;
			image = SpriteBase.mainSpriteBase.arm;
		}	
	}

	public class StrengthInNumbers : CharacterStipulationCard
	{
		int maxStaminaCost = 1;

		public StrengthInNumbers()
		{
			name = "Strength In Numbers";
			image = SpriteBase.mainSpriteBase.arm;
			description = "Spend" + maxStaminaCost + "max stamina, enemies targeted by melee attacks gain Gang Up";
		}

		protected override void ExtenderSpecificActivation()
		{
			MeleeCard.EMeleeCardPlayed += TriggerEffect;
			appliedToCharacter.IncrementMaxStamina(-maxStaminaCost);
		}

		void TriggerEffect(CharacterGraphic rangedCardPlayer, MeleeCard playedCard)
		{
			if (rangedCardPlayer == appliedToCharacter)
				foreach (CharacterGraphic enemy in playedCard.targetChars)
					enemy.TryPlaceCharacterStipulationCard(new CrossFire(false));
		}

		protected override void ExtenderSpecificDeactivation()
		{
			MeleeCard.EMeleeCardPlayed -= TriggerEffect;
		}
	}

	public class GangUp : CharacterStipulationCard
	{
		int damagePerMeleeAttack = 10;

		public GangUp()
		{
			SetLastsForRounds(1);

			name = "Gang Up";
			image = SpriteBase.mainSpriteBase.arm;
			description = "For one round: character takes " + damagePerMeleeAttack + " extra damage from every melee attack ";
		}

		protected override void ExtenderSpecificActivation()
		{
			MeleeCard.EMeleeCardPlayed += TriggerEffect;
		}

		void TriggerEffect(CharacterGraphic meleeCardPlayer, MeleeCard playedCard)
		{
			if (playedCard.targetChars.Contains(appliedToCharacter))
				appliedToCharacter.TakeDamage(damagePerMeleeAttack);
		}

		protected override void ExtenderSpecificDeactivation()
		{
			MeleeCard.EMeleeCardPlayed -= TriggerEffect;
		}
	}
	/*
	public class GangUp : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			targetType = TargetType.SelectEnemy;
			addedStipulationCard = new TagTeam();
			//staminaCost = 4;

			name = "Gang Up";
			description = "Play to a hostile character: " + addedStipulationCard.description;
			image = SpriteBase.mainSpriteBase.cover;

		}
	}*/

	public class Muscle : PrepCard
	{
		public Muscle()
		{
			placedStipulationCard=new CloseAndPersonal();
			
			name = "Muscle";
			description = placedStipulationCard.description;
			image = SpriteBase.mainSpriteBase.arm;
		}
	}

	public class CloseAndPersonal : CharacterStipulationCard
	{
		int staminaGainPerAmmoPoint=2;

		public CloseAndPersonal()
		{
			name = "Close And Personal";
			image = SpriteBase.mainSpriteBase.bullets;
			description = "Spend all ammo, gain " + staminaGainPerAmmoPoint + " stamina per point of ammo";
		}

		protected override void ExtenderSpecificActivation()
		{
			int mercsAmmo = appliedToCharacter.GetAmmo();
			int staminaIncrement = mercsAmmo * staminaGainPerAmmoPoint;
			appliedToCharacter.IncrementStamina(staminaIncrement);
			appliedToCharacter.SetAmmo(0);
		}
	}
	/*
	public class NoRest : EffectCard
	{
		protected override bool ExtenderPrerequisitesMet(CharacterGraphic user)
		{
			if (user.GetStamina() == 0)
				return true;
			else
				return false;
		}

		int maxStaminaBonus = 2;

		protected override void ExtenderConstructor()
		{
			name = "No Rest";
			description = "If stamina is 0: gain " + maxStaminaBonus + " max stamina";
			image = SpriteBase.mainSpriteBase.restSprite;
			targetType = TargetType.None;

		}

		protected override void ApplyCardPlayEffects()
		{
			userCharGraphic.IncrementMaxStamina(maxStaminaBonus);
		}
	}*/

	public class Powerhouse : PrepCard
	{
		public Powerhouse()
		{
			placedStipulationCard = new RoidRage();
			
			name = "Powerhouse";
			image = SpriteBase.mainSpriteBase.arm;
			description = placedStipulationCard.description;
		}
	}

	public class RoidRage : CharacterStipulationCard
	{
		int tempStaminaGain = 4;
		int maxStaminaCost = 2;
		public RoidRage()
		{
			name = "Roid Rage";
			image = SpriteBase.mainSpriteBase.lightning;
			description = "Spend 2 max stamina, gain " + tempStaminaGain+" stamina";
		}

		protected override void ExtenderSpecificActivation()
		{
			appliedToCharacter.IncrementStamina(tempStaminaGain);
			appliedToCharacter.IncrementMaxStamina(-maxStaminaCost);
		}
	}

	public class RoughUp : MeleeCard
	{
		protected override bool ExtenderPrerequisitesMet(CharacterGraphic user)
		{
			return user.GetStamina() > 0;
		}

		protected override void ExtenderConstructor()
		{
			name = "Rough Up";
			description = "Spend all stamina, remove equal stamina from target";
			image = SpriteBase.mainSpriteBase.arm;

			targetType = TargetType.SelectEnemy;
			useUpAllStamina = true;
		}

		protected override void ApplyStatEffects()
		{
			staminaDamage = usedUpStaminaPoints;
			base.ApplyStatEffects();
		}
	}
}


public class MobMentality : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectFriendlyOther;
		useUpAllStamina = true;
		addedStipulationCard = new MeleeBlock();

		name = "Mob Mentality";
		description = "Remove all stamina from self and friendly target, both gain melee block";
		image = SpriteBase.mainSpriteBase.cover;

	}

	protected override void ApplyCardPlayEffects()
	{
		base.ApplyCardPlayEffects();
		userCharGraphic.TryPlaceCharacterStipulationCard(new MeleeBlock());
		targetChars[0].RemoveAllStamina();
	}
}

public class SuckerPunch : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		name = "Sucker Punch";
		description = "Won't know what him 'em";
		image = SpriteBase.mainSpriteBase.arm;
		targetType = TargetType.SelectEnemy;

		staminaCost = 2;
		damage = 30;
	}

}

public class LightsOut : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectEnemy;

		staminaCost = 4;
		staminaDamage = 4;

		name = "Lights Out";
		description = "Remove " + staminaDamage + " stamina from target";
		image = SpriteBase.mainSpriteBase.leg;
	}
}

public class Roundhouse : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		name = "Roundhouse kick";
		description = "Badass";
		image = SpriteBase.mainSpriteBase.leg;
		targetType = TargetType.SelectEnemy;

		damagePerStaminaPoint = 10;
		useUpAllStamina = true;
		damage = 10;
	}
}

public class Execution : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		name = "Execution";
		description = "Targets the weakest enemy";
		image = SpriteBase.mainSpriteBase.skull;
		targetType = TargetType.Weakest;

		staminaCost = 2;
		damage = 40;
	}
}


public class Kick : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		name = "Kick";
		description = "Guard break (ignores blocks)";
		image = SpriteBase.mainSpriteBase.leg;
		targetType = TargetType.SelectEnemy;
		ignoresBlocks = true;

		staminaCost = 4;
		damage = 30;
	}
}



public class SprayNPray : RangedCard
{
	protected override void ExtenderConstructor()
	{
		name = "Spray And Pray";
		description = "Quantity over quality (Targets random enemy)";
		image = SpriteBase.mainSpriteBase.bullets;
		targetType = TargetType.Random;

		ammoCost = 1;
		damage = 30;
	}
}
