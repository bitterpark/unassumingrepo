using UnityEngine;
using System.Collections;

public class SoldierCards
{
	public static CombatDeck GetClassCards()
	{
		CombatDeck result = new CombatDeck();

		result.AddCards(typeof(Smokescreen));
		result.AddCards(typeof(BoundingOverwatch));
		result.AddCards(typeof(Grenade));
		result.AddCards(typeof(PickOff));
		result.AddCards(typeof(MarkTarget));
		result.AddCards(typeof(Defillade),2);
		result.AddCards(typeof(Diversion));
		result.AddCards(typeof(Smash));
		result.AddCards(typeof(Jab));
		result.AddCards(typeof(Camaraderie));
		result.AddCards(typeof(Sacrifice));

		return result;
	}
}

public class Smokescreen : EffectCard
{
	public Smokescreen()
		: base()
	{
		targetType = TargetType.AllFriendlies;
		ammoCost = 2;
		targetArmorGain = 30;
		
		name = "Smokescreen";
		description = "The user and all friendly characters gain " + targetArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.cover;
		
	}
}

public class Defillade : EffectCard
{
	public Defillade()
		: base()
	{
		targetType = TargetType.None;
		staminaCost = 1;
		userArmorGain = 30;
		
		name = "Defillade";
		description = "Gain " + userArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.rock;
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
		void Trigger(CharacterGraphic cardPlayer)
		{
			if (cardPlayer.GetType() != typeof(MercGraphic))
			{
				CardsScreen.main.RemoveRoomCard(this);
				cardPlayer.TakeDamage(damage,true);
			}

		}
		public override void DeactivateCard()
		{
			RangedCard.ERangedCardPlayed -= Trigger;
		}
	}
	
	public BoundingOverwatch()
		: base()
	{
		name = "Bounding Overwatch";
		description = "Places an Overwatch in the room";
		image = SpriteBase.mainSpriteBase.crosshair;
		targetType = TargetType.None;

		staminaCost = 2;
	}

	protected override void ApplyEffects()
	{
		CardsScreen.main.PlaceRoomCard(new Overwatch());
	}
}

public class Camaraderie : EffectCard
{
	public Camaraderie()
		: base()
	{
		targetType = TargetType.SelectFriendly;
		staminaCost = 3;
		targetStaminaGain = 3;
		
		name = "Camaraderie";
		description = "A selected friendly character gains " + targetStaminaGain + " stamina";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;
	}
}

public class Sacrifice : EffectCard
{
	public Sacrifice()
		: base()
	{
		targetType = TargetType.SelectFriendly;
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
	public Grenade()
		: base()
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
	public PickOff()
		: base()
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
	public MarkTarget()
		: base()
	{
		name = "Mark Target";
		description = "Targets the strongest enemy";
		image = SpriteBase.mainSpriteBase.crosshair;
		targetType = TargetType.Strongest;

		ammoCost = 1;
		damage = 40;
	}
}

public class Diversion : MeleeCard
{
	int damagePenalty = 10;
	
	public Diversion()
		: base()
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