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
		result.AddCards(typeof(Throw));
		result.AddCards(typeof(Sacrifice));
		result.AddCards(typeof(Defillade));
		result.AddCards(typeof(Smokescreen));
		result.AddCards(typeof(PickOff));
		result.AddCards(typeof(Camaraderie));

		return result;
	}

	public static List<PrepCard> GetClassPrepCards()
	{
		List<PrepCard> result = new List<PrepCard>();
		result.Add(new Saviour());
		result.Add(new Commander());
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

			//addedCombatCards.Add(new Sacrifice());
			//addedCombatCards.Add(new Smokescreen());
			//addedCombatCards.Add(new AllForOne());
			addedCombatCards.Add(new AllForOne());
		}
	}
	public class Commander : PrepCard
	{
		public Commander()
		{
			name = "Commander";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.medal;

			//addedCombatCards.Add(new MarkTarget());
			addedCombatCards.Add(new MarkTarget());
			//addedCombatCards.Add(new PickOff());
			//addedCombatCards.Add(new Camaraderie());
		}
	}
	public class Survivor : PrepCard
	{
		public Survivor()
		{
			name = "Survivor";
			description = "Adds cards to your deck";
			image = SpriteBase.mainSpriteBase.skull;

			//addedCombatCards.Add(new Defillade());
			//addedCombatCards.Add(new Defillade());
			//addedCombatCards.Add(new Smokescreen());
			addedCombatCards.Add(new Discretion());
		}

	}
}

public class Discretion : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		staminaCost = 4;
		userArmorGain = 80;

		name = "Discretion";
		description = "Better part of valor (gain " + userArmorGain + " armor)";
		image = SpriteBase.mainSpriteBase.cover;

	}
}

public class Defillade : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.None;
		staminaCost = 1;
		userArmorGain = 30;
		
		name = "Defillade";
		description = "Gain " + userArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.rock;
	}
}

public class AllForOne : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectFriendly;
		addedStipulationCard = new CoverFire();
		staminaCost = 1;

		name = "All For One";
		description = "Play a Covering Fire card to a friendly character";
		image = SpriteBase.mainSpriteBase.cover;

	}
	
	public class CoverFire : CharacterStipulationCard
	{
		int armorGainPerRangedAttack = 30;
		
		public CoverFire()
		{
			SetLastsForRounds(1);

			name = "Cover Fire";
			image = SpriteBase.mainSpriteBase.crosshair;
			description = "For one round: gain "+armorGainPerRangedAttack+" armor for every friendly ranged attack";
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
				CardsScreen.main.RemoveRoomStipulationCard(this);
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
}

public class Camaraderie : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectFriendlyOther;
		staminaCost = 3;
		targetStaminaGain = 3;
		
		name = "Camaraderie";
		description = "A selected friendly character gains " + targetStaminaGain + " stamina";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;
	}
}

public class Sacrifice : EffectCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.SelectFriendlyOther;
		staminaCost = 1;
		takeDamageCost = 10;
		targetArmorGain = 30;
		
		name = "Sacrifice";
		description = "Take " + takeDamageCost + " damage, a selected friendly character gains " + targetArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;
		
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
		damage = 20;
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
		damage = 40;
	}
}

public class MarkTarget : RangedCard
{
	protected override void ExtenderConstructor()
	{
		targetType = TargetType.Strongest;
		ammoCost = 1;
		damage = 20;
		addedStipulationCard = new CrossFire(false);
		
		name = "Mark Target";
		description = "Play Crossfire to the strongest enemy";
		image = SpriteBase.mainSpriteBase.crosshair;	
	}
}

public class Diversion : MeleeCard
{
	int damagePenalty = 10;

	protected override void ExtenderConstructor()
	{
		name = "Diversion";
		description = "Targets all enemies, take "+damagePenalty+" damage";
		image = SpriteBase.mainSpriteBase.brokenLegsSprite;
		targetType = TargetType.AllEnemies;

		staminaCost = 4;
		staminaDamage= 3;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.TakeDamage(damagePenalty);
	}
}