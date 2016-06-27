using UnityEngine;
using System.Collections;

public class Smokescreen : Effect
{
	int armorBonus = 30;
	public Smokescreen()
		: base()
	{
		name = "Smokescreen";
		description = "The user and all friendly characters gain "+armorBonus+" armor";
		image = SpriteBase.mainSpriteBase.cover;
		targetType = TargetType.AllFriendlies;
		ammoCost = 3;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		foreach (CharacterGraphic friendly in targetChars)
			friendly.IncrementArmor(armorBonus);
	}
}

public class BoundingOverwatch : Effect
{
	public class Overwatch : RoomStipulationCard
	{
		int damage = 30;

		public Overwatch()
		{
			name = "Overwatch";
			image = SpriteBase.mainSpriteBase.bomb;
			description = "When any enemy character plays a ranged attack, they lose " + damage + " health and this card is removed";
		}

		public override void ActivateCard()
		{
			MeleeCard.EMeleeCardPlayed += Trigger;
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
			MeleeCard.EMeleeCardPlayed -= Trigger;
		}
	}
	
	public BoundingOverwatch()
		: base()
	{
		name = "Bounding Overwatch";
		description = "Places an Overwatch in the room";
		image = SpriteBase.mainSpriteBase.crosshair;
		targetType = TargetType.None;

		ammoCost = 1;
		staminaCost = 2;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		CardsScreen.main.PlaceRoomCard(new Overwatch());
	}
}

public class Camaraderie : Effect
{
	int friendlyCharStaminaGain = 4;

	public Camaraderie()
		: base()
	{
		name = "Camaraderie";
		description = "A selected friendly character gains " + friendlyCharStaminaGain + " stamina";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;
		targetType = TargetType.AllEnemies;

		staminaCost = 4;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		targetChars[0].IncrementStamina(friendlyCharStaminaGain);
	}
}

public class Sacrifice : Effect
{
	int friendlyCharArmorGain = 30;
	int selfDamage = 10;

	public Sacrifice()
		: base()
	{
		name = "Sacrifice";
		description = "Take " + selfDamage +" damage, a selected friendly character gains " + friendlyCharArmorGain + " armor";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;
		targetType = TargetType.SelectFriendly;

		staminaCost = 1;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.TakeDamage(selfDamage);
		targetChars[0].IncrementArmor(friendlyCharArmorGain);
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

		ammoCost = 3;
		healthDamage = 30;
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
		healthDamage = 20;
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
		healthDamage = 20;
	}
}

public class Diversion : MeleeCard
{
	int damagePenalty = 20;
	
	public Diversion()
		: base()
	{
		name = "Diversion";
		description = "Targets all enemies, take "+damagePenalty+" damage";
		image = SpriteBase.mainSpriteBase.brokenLegsSprite;
		targetType = TargetType.AllEnemies;

		staminaCost = 5;
		staminaDamage= 3;
	}

	protected override void CardPlayEffects()
	{
		base.CardPlayEffects();
		userCharGraphic.TakeDamage(damagePenalty);
	}
}