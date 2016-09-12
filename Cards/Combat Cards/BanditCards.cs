using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BanditCards
{
	public static CombatDeck GetClassCards()
	{
		CombatDeck result = new CombatDeck();

		result.AddCards(typeof(SuckerPunch));
		result.AddCards(typeof(Roundhouse));
		result.AddCards(typeof(Kick));
		result.AddCards(typeof(LightsOut));
		result.AddCards(typeof(Throw));
		result.AddCards(typeof(Execution));
		result.AddCards(typeof(SprayNPray));

		return result;
	}

	public static List<PrepCard> GetClassPrepCards()
	{
		List<PrepCard> result = new List<PrepCard>();
		result.Add(new Enforcer());
		result.Add(new Thug());
		result.Add(new Muscle());


		return result;
	}

	public class Thug : PrepCard
	{
		public Thug()
		{
			name = "Thug";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.arm;

			addedCombatCards.Add(new GangUp());
		}

		public class GangUp : MeleeCard
		{
			protected override void ExtenderConstructor()
			{
				targetType = TargetType.SelectEnemy;
				addedStipulationCard = new TagTeam();
				//staminaCost = 4;

				name = "Gang Up";
				description = "Play to a hostile character: "+addedStipulationCard.description;
				image = SpriteBase.mainSpriteBase.cover;

			}

			public class TagTeam : CharacterStipulationCard
			{
				int damagePerMeleeAttack = 10;

				public TagTeam()
				{
					SetLastsForRounds(1);

					name = "Tag Team";
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
		}
	}

	public class Muscle : PrepCard
	{
		public Muscle()
		{
			name = "Muscle";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.arm;

			addedCombatCards.Add(new NoRest());
		}

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
				description = "If stamina is 0: gain "+maxStaminaBonus+" max stamina";
				image = SpriteBase.mainSpriteBase.restSprite;
				targetType = TargetType.None;

			}

			protected override void CardPlayEffects()
			{
				userCharGraphic.IncrementMaxStamina(maxStaminaBonus);
			}
			/*
			public class SecondWind : CharacterStipulationCard
			{
				public SecondWind()
				{
					name = "Second Wind";
					image = SpriteBase.mainSpriteBase.lightning;
					description = "The character's next melee attack is free";
				}

				protected override void ExtenderSpecificActivation()
				{
					appliedToCharacter.SetMeleeAttacksFree(true);
					MeleeCard.EMeleeCardPlayed += UseUpCard;
				}

				void UseUpCard(CharacterGraphic meleeCardPlayer, MeleeCard playedCard)
				{
					if (meleeCardPlayer == appliedToCharacter)
					{
						appliedToCharacter.RemoveCharacterStipulationCard(this);
					}
				}

				protected override void ExtenderSpecificDeactivation()
				{
					MeleeCard.EMeleeCardPlayed -= UseUpCard;
					appliedToCharacter.SetMeleeAttacksFree(false);
				}
			}*/
		}
	}

	public class Enforcer : PrepCard
	{
		public Enforcer()
		{
			name = "Enforcer";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.crosshair;

			addedCombatCards.Add(new RoughUp());
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

			protected override void ApplyEffects()
			{
				staminaDamage = usedUpStaminaPoints;
				base.ApplyEffects();
			}
		}
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
		name = "Lights Out";
		description = "Remove "+staminaDamage+" stamina from target";
		image = SpriteBase.mainSpriteBase.leg;
		targetType = TargetType.SelectEnemy;

		staminaCost = 4;
		staminaDamage = 4;
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
		description = "Guard break (ignores armor)";
		image = SpriteBase.mainSpriteBase.leg;
		targetType = TargetType.SelectEnemy;
		ignoresArmor = true;

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
