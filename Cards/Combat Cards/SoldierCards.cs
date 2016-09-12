using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierCards
{
	public static CombatDeck GetClassCards()
	{
		CombatDeck result = new CombatDeck();

		result.AddCards(typeof(Grenade));
		result.AddCards(typeof(Diversion));
		result.AddCards(typeof(MarkTarget));
		result.AddCards(typeof(Sacrifice));
		result.AddCards(typeof(Defillade));
		result.AddCards(typeof(Smokescreen));
		result.AddCards(typeof(PickOff));
		result.AddCards(typeof(BurstFire));
		result.AddCards(typeof(Camaraderie));

		return result;
	}

	public static List<PrepCard> GetClassPrepCards()
	{
		List<PrepCard> result = new List<PrepCard>();
		result.Add(new Saviour());
		result.Add(new Commando());
		result.Add(new Survivor());


		return result;
	}

	public class Saviour : PrepCard
	{
		public Saviour()
		{
			name = "Saviour";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.cover;

			addedCombatCards.Add(new AllForOne());
		}

		public class AllForOne : EffectCard
		{
			protected override void ExtenderConstructor()
			{
				targetType = TargetType.SelectFriendly;
				addedStipulationCard = new CoverFire();
				staminaCost = 1;

				name = "All For One";
				description = "Play to a friendly character:"+addedStipulationCard.description;
				image = SpriteBase.mainSpriteBase.cover;

			}

			public class CoverFire : CharacterStipulationCard
			{
				int armorGainPerRangedAttack = 20;

				public CoverFire()
				{
					SetLastsForRounds(2);

					name = "Cover Fire";
					image = SpriteBase.mainSpriteBase.crosshair;
					description = "For two rounds: gain " + armorGainPerRangedAttack + " armor for every friendly ranged attack";
				}

				protected override void ExtenderSpecificActivation()
				{
					RangedCard.ERangedCardPlayed += TriggerEffect;
				}

				void TriggerEffect(CharacterGraphic rangedCardPlayer, RangedCard playedCard)
				{
					if (rangedCardPlayer.GetType() == typeof(MercGraphic))
						appliedToCharacter.IncrementArmor(armorGainPerRangedAttack);
				}

				protected override void ExtenderSpecificDeactivation()
				{
					RangedCard.ERangedCardPlayed -= TriggerEffect;
				}
			}
		}
	}
	public class Commando : PrepCard
	{
		public Commando()
		{
			name = "Commando";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.medal;

			addedCombatCards.Add(new ControlledBursts());
		}

		public class ControlledBursts : EffectCard
		{
			protected override void ExtenderConstructor()
			{
				targetType = TargetType.None;
				addedStipulationCard = new TriggerDiscipline();
				staminaCost = 1;

				name = "Controlled Bursts";
				description = addedStipulationCard.description;
				image = SpriteBase.mainSpriteBase.bullets;

			}

			public class TriggerDiscipline : CharacterStipulationCard
			{
				public TriggerDiscipline()
				{
					name = "Trigger Discipline";
					image = SpriteBase.mainSpriteBase.crosshair;
					description = "Character's next ranged attack costs stamina instead of ammo";
				}

				protected override void ExtenderSpecificActivation()
				{
					appliedToCharacter.SetRangedAttacksCostStamina(true);
					RangedCard.ERangedCardPlayed += TriggerEffect;
				}
				void TriggerEffect(CharacterGraphic cardPlayer, RangedCard playedCard)
				{
					if (cardPlayer == appliedToCharacter)
						appliedToCharacter.RemoveCharacterStipulationCard(this);
				}

				protected override void ExtenderSpecificDeactivation()
				{
					appliedToCharacter.SetRangedAttacksCostStamina(false);
					RangedCard.ERangedCardPlayed -= TriggerEffect;
				}
			}
		}
	}
	public class Survivor : PrepCard
	{
		public Survivor()
		{
			name = "Survivor";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.skull;

			addedCombatCards.Add(new Discretion());
		}

		public class Discretion : EffectCard
		{
			int armorGainPerStaminaPoint = 30;

			protected override bool ExtenderPrerequisitesMet(CharacterGraphic user)
			{
				return user.GetStamina()>0;
			}

			protected override void ExtenderConstructor()
			{
				targetType = TargetType.None;
				useUpAllStamina = true;

				name = "Discretion";
				description = "Better part of valor (spend all stamina, gain " + userArmorGain + " armor per stamina point)";
				image = SpriteBase.mainSpriteBase.cover;

			}

			protected override void ApplyEffects()
			{
				userArmorGain = usedUpStaminaPoints * armorGainPerStaminaPoint;
				base.ApplyEffects();
			}
		}
	}
}


public class Defillade : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		staminaCost = 2;
		userArmorGain = 40;
		
		name = "Defillade";
		description = "Gain " + userArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.rock;
	}
}


/*
public class BoundingOverwatch : EffectCard
{
	public class Overwatch : RoomStipulationCard
	{
		int damage = 30;

		public Overwatch()
		{
			name = "Overwatch";
			image = SpriteBase.mainSpriteBase.crosshair;
			description = "When any enemy character plays a ranged attack, they lose " + damage + " health and this card is removed";
		}

		public override void ActivateCard()
		{
			RangedCard.ERangedCardPlayed += Trigger;
		}
		void Trigger(CharacterGraphic cardPlayer, RangedCard playedCard)
		{
			if (cardPlayer.GetType() != typeof(MercGraphic))
			{
				CombatManager.main.RemoveRoomStipulationCard(this);
				cardPlayer.TakeDamage(damage,true);
			}

		}
		public override void DeactivateCard()
		{
			RangedCard.ERangedCardPlayed -= Trigger;
		}
	}

	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		addedStipulationCard=new Overwatch();
		staminaCost = 2;
		
		name = "Bounding Overwatch";
		description = "Places an Overwatch in the room";
		image = SpriteBase.mainSpriteBase.crosshair;
		
	}
}*/

public class Camaraderie : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectFriendlyOther;
		staminaCost = 2;
		targetStaminaGain = 2;
		
		name = "Camaraderie";
		description = "A selected friendly character gains " + targetStaminaGain + " stamina";
		image = SpriteBase.mainSpriteBase.arm;
	}
}

public class Sacrifice : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectFriendlyOther;
		staminaCost = 1;
		takeDamageCost = 20;
		targetArmorGain = 40;
		
		name = "Sacrifice";
		description = "Take " + takeDamageCost + " damage, a selected friendly character gains " + targetArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.arm;
		
	}
}

public class Grenade : RangedCard
{
	protected override void ExtenderConstructor()
	{
		name = "Grenade";
		description = "Fire in the hole (damages all enemies)";
		image = SpriteBase.mainSpriteBase.fire;
		targetType = TargetType.AllEnemies;

		ammoCost = 2;
		damage = 10;
	}
}

public class PickOff : RangedCard
{
	protected override void ExtenderConstructor()
	{
		name = "Pick Off";
		description = "Targets the weakest enemy";
		image = SpriteBase.mainSpriteBase.skull;
		targetType = TargetType.Weakest;

		ammoCost = 1;
		damage = 30;
	}
}

public class MarkTarget : RangedCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.Strongest;
		ammoCost = 1;
		damage = 10;
		addedStipulationCard = new CrossFire(false);
		
		name = "Mark Target";
		description = "Play to the toughest enemy: "+addedStipulationCard.description;
		image = SpriteBase.mainSpriteBase.crosshair;	
	}
}

public class Diversion : MeleeCard
{
	int damagePenalty = 20;

	protected override void ExtenderConstructor()
	{
		name = "Diversion";
		description = "Targets all enemies, take "+damagePenalty+" damage";
		image = SpriteBase.mainSpriteBase.leg;
		targetType = TargetType.AllEnemies;

		staminaCost = 2;
		staminaDamage = 1;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.TakeDamage(damagePenalty);
	}
}