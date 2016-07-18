using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Marksman : EncounterEnemy
{
	public Marksman()
	{
		name = "Marksman";
		health = 60;
		armor = 20;
		stamina = 3;
		ammo = 3;

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

			damage = 30;
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

			damage = 20;
			ammoCost = 1;
			staminaCost = 1;
			targetType = TargetType.SelectEnemy;
		}
	}

	public class ChangePosition : EffectCard
	{
		public ChangePosition():base()
		{
			staminaCost = 2;
			userArmorGain = 30;
			targetType = TargetType.None;
			
			name = "Change Position";
			description = "Gain " + userArmorGain + " armor";
			image = SpriteBase.mainSpriteBase.lateralArrows;
		}
	}

	public class BoobyTrap : EffectCard
	{
		int trapDamage;

		public BoobyTrap()
			: base()
		{
			targetType = TargetType.None;
			ammoCost = 1;
			trapDamage = 20;
			
			name = "Booby Trap";
			description = "Places a Booby Trap in the room";
			image = SpriteBase.mainSpriteBase.bomb;
			addedStipulationCard=new TripmineEnemy(trapDamage);
		}
	}
}

public class HeavyGunner : EncounterEnemy
{
	public HeavyGunner()
	{
		name = "Heavy Gunner";
		health = 70;
		armor = 30;
		stamina = 2;
		ammo = 3;

		combatDeck.AddCards(typeof(Machinegun), 2);
		combatDeck.AddCards(typeof(Suppression));
		combatDeck.AddCards(typeof(PinDown));
		combatDeck.AddCards(typeof(CQC), 2);
	}

	public class Machinegun : RangedCard
	{
		public Machinegun()
			: base()
		{
			name = "Machinegun";
			description = "Targets all enemies";
			image = SpriteBase.mainSpriteBase.bullets;

			damage = 20;
			ammoCost = 1;
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

			staminaDamage = 1;
			staminaCost = 2;
			ammoCost = 1;
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

			damage = 5;
			staminaDamage = 3;
			staminaCost = 2;
			ammoCost = 1;
			targetType = TargetType.SelectEnemy;
		}
	}
}

public class ShockTrooper : EncounterEnemy
{
	public ShockTrooper()
	{
		name = "Shock Trooper";
		health = 60;
		armor = 20;
		stamina = 4;
		ammo = 2;

		combatDeck.AddCards(typeof(CoverToCover));
		combatDeck.AddCards(typeof(Flanking),2);
		combatDeck.AddCards(typeof(Blitz),2);
		combatDeck.AddCards(typeof(CQC), 2);
		combatDeck.AddCards(typeof(Blindfire));
	}

	public class CoverToCover : EffectCard
	{
		public CoverToCover()
			: base()
		{
			staminaCost = 2;
			userArmorGain = 40;
			targetType = TargetType.None;
			
			name = "Cover-to-Cover";
			description = "Gain " + userArmorGain + " armor";
			image = SpriteBase.mainSpriteBase.cover;
		}
	}

	public class Blindfire : RangedCard
	{
		public Blindfire()
			: base() //x2
		{
			name = "Blind Fire";
			image = SpriteBase.mainSpriteBase.bullets;

			damage = 20;
			ammoCost = 1;
			targetType = TargetType.SelectEnemy;
		}
	}

	public class Flanking : MeleeCard
	{
		public Flanking()
			: base()
		{
			staminaCost = 2;
			ammoCost = 1;
			damage = 20;
			ignoresArmor = true;
			targetType = TargetType.SelectEnemy;
			
			name = "Flanking";
			description = "Ignores armor";
			image = SpriteBase.mainSpriteBase.lateralArrows;
		}
	}

	public class Blitz : MeleeCard
	{
		public Blitz():base()
		{
			name = "Blitz";
			description = "Targets the strongest enemy";
			image = SpriteBase.mainSpriteBase.brokenArmsSprite;

			staminaCost = 3;
			damage = 50;
			targetType = TargetType.Strongest;
		}
	}

	
}

public class CQC : MeleeCard
{
	public CQC()
		: base()
	{
		name = "CQC";
		description = "Targets the strongest enemy";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;

		damage = 30;
		staminaCost = 1;
		targetType = TargetType.Strongest;
	}
}

public class FieldCommander : EncounterEnemy
{
	public FieldCommander()
	{
		name = "Field Commander";
		health = 40;
		armor = 40;
		stamina = 4;
		ammo = 2;

		combatDeck.AddCards(typeof(Rally));
		combatDeck.AddCards(typeof(Resupply));
		combatDeck.AddCards(typeof(Soften),2);
		combatDeck.AddCards(typeof(Flashbang));
		combatDeck.AddCards(typeof(Sidearm));
		combatDeck.AddCards(typeof(CQC),2);
	}

	public class Rally:EffectCard
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

	public class Resupply : EffectCard
	{
		public Resupply()
			: base()
		{
			staminaCost = 3;
			targetAmmoGain = 1;
			targetType = TargetType.AllFriendlies;
			
			name = "Resupply";
			description = "Character and all allies gain " + targetAmmoGain + " ammo";
			image = SpriteBase.mainSpriteBase.ammoBoxSprite;
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

			ammoCost = 1;
			damage = 20;
		}
	}

	public class Flashbang : RangedCard
	{
		int noCoverStaminaDamageBonus = 1;
		
		public Flashbang()
		: base()
		{
			name = "Flashbang";
			description = "Targets all enemies, characters with 0 armor lose extra " + noCoverStaminaDamageBonus+" stamina";
			image = SpriteBase.mainSpriteBase.lightning;
			targetType = TargetType.AllEnemies;

			ammoCost = 2;
			staminaCost = 1;
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