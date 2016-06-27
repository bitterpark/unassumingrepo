using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Marksman : EncounterEnemy
{
	public Marksman()
	{
		name = "Marksman";
		health = 40;
		armor = 30;
		stamina = 3;
		ammo = 9;

		combatDeck.AddCards(typeof(Eliminate),2);
		combatDeck.AddCards(typeof(WeakspotShot));
		combatDeck.AddCards(typeof(ChangePosition));
		combatDeck.AddCards(typeof(Sidearm));
		combatDeck.AddCards(typeof(BoobyTrap));
		combatDeck.AddCards(typeof(Pistolwhip));
	}

	public class Eliminate : RangedCard
	{
		public Eliminate():base()
		{
			name = "Eliminate";
			description = "Targets weakest enemy";
			image = SpriteBase.mainSpriteBase.skull;

			healthDamage = 20;
			ammoCost = 2;
			targetType = TargetType.Weakest;
		}
	}

	public class WeakspotShot : RangedCard
	{
		public WeakspotShot()
			: base()
		{
			name = "Weakspot Shot";
			description = "Ignores armor";
			image = SpriteBase.mainSpriteBase.cover;

			healthDamage = 45;
			ammoCost = 5;
			staminaCost = 2;
			targetType = TargetType.SelectEnemy;
		}
	}

	public class ChangePosition : Effect
	{
		int armorGain = 20;
		
		public ChangePosition():base()
		{
			name = "Change Position";
			description = "Gain "+armorGain+" armor";
			image = SpriteBase.mainSpriteBase.lateralArrows;

			staminaCost = 2;
			targetType = TargetType.None;
		}
	}

	public class BoobyTrap : Effect
	{
		public BoobyTrap()
			: base()
		{
			name = "Booby Trap";
			description = "Places a Booby Trap in the room";
			image = SpriteBase.mainSpriteBase.bomb;
			targetType = TargetType.None;
			ammoCost = 2;
		}

		protected override void CardPlayEffects()
		{
			base.CardPlayEffects();
			CardsScreen.main.PlaceRoomCard(new TripmineEnemy());
		}
	}
}

public class HeavyGunner : EncounterEnemy
{
	public HeavyGunner()
	{
		name = "Heavy Gunner";
		health = 60;
		armor = 30;
		stamina = 4;
		ammo = 6;

		combatDeck.AddCards(typeof(FullAuto),2);
		combatDeck.AddCards(typeof(Suppression));
		combatDeck.AddCards(typeof(PinDown));
		combatDeck.AddCards(typeof(CQC), 2);
	}

	public class FullAuto : RangedCard
	{
		public FullAuto()
			: base()
		{
			name = "Full Auto";
			description = "Targets all enemies";
			image = SpriteBase.mainSpriteBase.bullets;

			healthDamage = 15;
			ammoCost = 2;
			staminaCost = 2;
			targetType = TargetType.AllEnemies;
		}
	}

	public class Suppression : RangedCard
	{
		public Suppression()
			: base()
		{
			name = "Suppressive Fire";
			description = "Targets all enemies";
			image = SpriteBase.mainSpriteBase.bullets;

			healthDamage = 5;
			staminaDamage = 2;
			staminaCost = 2;
			ammoCost = 3;
			targetType = TargetType.AllEnemies;
		}
	}

	public class PinDown : RangedCard
	{
		public PinDown()
			: base()
		{
			name = "Pin Down";
			//description = "Targets all enemies";
			image = SpriteBase.mainSpriteBase.bullets;

			healthDamage = 5;
			staminaDamage = 4;
			staminaCost = 2;
			ammoCost = 2;
			targetType = TargetType.SelectEnemy;
		}
	}
}

public class ShockTrooper : EncounterEnemy
{
	public ShockTrooper()
	{
		name = "Shock Trooper";
		health = 70;
		armor = 30;
		stamina = 9;
		ammo = 3;

		combatDeck.AddCards(typeof(CoverToCover));
		combatDeck.AddCards(typeof(Flanking),2);
		combatDeck.AddCards(typeof(Blitz),2);
		combatDeck.AddCards(typeof(CQC), 2);
		combatDeck.AddCards(typeof(Blindfire));
	}

	public class CoverToCover : Effect
	{
		int armorGain = 20;

		public CoverToCover()
			: base()
		{
			name = "Cover-to-Cover";
			description = "Gain "+armorGain+" armor";
			image = SpriteBase.mainSpriteBase.cover;

			staminaCost = 2;
			targetType = TargetType.None;
		}

		protected override void CardPlayEffects()
		{
			base.CardPlayEffects();
			userCharGraphic.IncrementArmor(armorGain);
		}
	}

	public class Blindfire : RangedCard
	{
		public Blindfire()
			: base() //x2
		{
			name = "Blindfire";
			image = SpriteBase.mainSpriteBase.bullets;

			healthDamage = 10;
			ammoCost = 2;
			targetType = TargetType.SelectEnemy;
		}
	}

	public class Flanking : MeleeCard
	{
		public Flanking()
			: base()
		{
			name = "Flanking";
			description = "Ignores armor";
			image = SpriteBase.mainSpriteBase.lateralArrows;

			staminaCost = 4;
			ammoCost = 1;
			healthDamage = 25;
			targetType = TargetType.SelectEnemy;
		}

		protected override void CardPlayEffects()
		{
			userCharGraphic.IncrementStamina(-staminaCost);
			userCharGraphic.IncrementAmmo(-ammoCost);
			targetChars[0].TakeDamage(healthDamage,true);
		}
	}

	public class Blitz : MeleeCard
	{
		public Blitz():base()
		{
			name = "Blitz";
			image = SpriteBase.mainSpriteBase.brokenArmsSprite;

			staminaCost = 5;
			healthDamage = 45;
			targetType = TargetType.SelectEnemy;
		}
	}

	
}

public class CQC : MeleeCard
{
	public CQC()
		: base()
	{
		name = "CQC";
		//description = "Targets all enemies";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;

		healthDamage = 20;
		staminaCost = 3;
		targetType = TargetType.SelectEnemy;
	}
}

public class FieldCommander : EncounterEnemy
{
	public FieldCommander()
	{
		name = "Field Commander";
		health = 40;
		armor = 40;
		stamina = 6;
		ammo = 6;

		combatDeck.AddCards(typeof(Rally));
		combatDeck.AddCards(typeof(Resupply));
		combatDeck.AddCards(typeof(Soften),2);
		combatDeck.AddCards(typeof(Flashbang));
		combatDeck.AddCards(typeof(Sidearm));
		combatDeck.AddCards(typeof(CQC),2);
	}

	public class Rally:Effect
	{
		int friendliesStaminaGain = 2;

		public Rally()
			: base()
		{
			name = "Rally";
			description = "Allies gain " + friendliesStaminaGain + " stamina";
			image = SpriteBase.mainSpriteBase.cover;

			staminaCost = 4;
			targetType = TargetType.AllFriendlies;
		}

		protected override void CardPlayEffects()
		{
			base.CardPlayEffects();
			foreach (CharacterGraphic graphic in targetChars)
			{
				if (graphic != userCharGraphic)
					graphic.IncrementStamina(friendliesStaminaGain);
			}
		}
	}

	public class Resupply : Effect
	{
		int friendliesAmmoGain = 2;

		public Resupply()
			: base()
		{
			name = "Resupply";
			description = "Character and all allies gain " + friendliesAmmoGain + " ammo";
			image = SpriteBase.mainSpriteBase.cover;

			staminaCost = 4;
			targetType = TargetType.AllFriendlies;
		}

		protected override void CardPlayEffects()
		{
			base.CardPlayEffects();
			foreach (CharacterGraphic graphic in targetChars)
			{
				graphic.IncrementAmmo(friendliesAmmoGain);
			}
		}
	}

	public class Soften : RangedCard
	{
		public Soften()
		: base()
		{
			name = "Soften";
			description = "Targets the strongest enemy";
			image = SpriteBase.mainSpriteBase.crosshair;
			targetType = TargetType.Strongest;

			ammoCost = 2;
			healthDamage = 20;
		}
	}

	public class Flashbang : RangedCard
	{
		int noCoverStaminaDamageBonus = 2;
		
		public Flashbang()
		: base()
		{
			name = "Flashbang";
			description = "Targets all enemies, characters with 0 armor lose extra " + noCoverStaminaDamageBonus+" stamina";
			image = SpriteBase.mainSpriteBase.lightning;
			targetType = TargetType.AllEnemies;

			ammoCost = 4;
			staminaDamage=1;
		}

		protected override void CardPlayEffects()
		{
			base.CardPlayEffects();
			foreach (CharacterGraphic graphic in targetChars)
			{
				if (graphic.GetArmor() == 0)
					graphic.IncrementStamina(-noCoverStaminaDamageBonus);
			}
		}

	}

}