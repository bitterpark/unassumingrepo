using UnityEngine;
using System.Collections;

public class Infestation {

}

public class Skitter : EncounterEnemy
{
	protected override void CommonConstructor()
	{
		basicCombatDeck.AddCards(typeof(Slash));
		basicCombatDeck.AddCards(typeof(Tear));

		variationCards.Add(new Vulture());
		variationCards.Add(new Latcher());
	}
	
	protected override void NormalConstructor()
	{
		name = "Skitter";
		health = 140;
		stamina = 6;
	}

	protected override void ToughConstructor()
	{
		name = "Tough Skitter";
		health = 140;
		armor = 60;
		stamina = 12;

		variationCards.Add(new Striker());
	}

	public class Vulture : EnemyVariationCard
	{
		public Vulture()
		{
			//SetLastsForRounds(1);

			name = "Vulture";
			image = SpriteBase.mainSpriteBase.arm;

			description = "Add 2 Bite cards to your deck";

			addedCombatCards.Add(new Bite());
			addedCombatCards.Add(new Bite());
		}

		public class Bite : MeleeCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Bite";
				description = "Targets the weakest enemy";
				image = SpriteBase.mainSpriteBase.skull;
				damage = 20;
				staminaCost = 2;
				targetType = TargetType.Weakest;
			}
		}
	}

	public class Latcher : EnemyVariationCard
	{
		public Latcher()
		{
			//SetLastsForRounds(1);

			name = "Striker";
			image = SpriteBase.mainSpriteBase.arm;

			description = "Add 2 Latch cards to your deck";

			addedCombatCards.Add(new Latch());
			addedCombatCards.Add(new Latch());
		}

		public class Latch : MeleeCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Latch";
				image = SpriteBase.mainSpriteBase.skull;
				damage = 40;
				staminaCost = 3;
				targetType = TargetType.SelectEnemy;
			}
		}
	}

	public class Striker : EnemyVariationCard
	{
		public Striker()
		{
			//SetLastsForRounds(1);

			name = "Striker";
			image = SpriteBase.mainSpriteBase.arm;

			description = "Add Strike to your deck";

			addedCombatCards.Add(new Strike());
		}

		public class Strike : MeleeCard
		{
			int damagePerStaminaPoint;
			int totalDamage;
			
			protected override void ExtenderConstructor()
			{
				staminaCost = 6;
				targetType = TargetType.SelectEnemy;
				damagePerStaminaPoint = 5;
				
				name = "Strike";
				description = "Remove all stamina, deal " + damagePerStaminaPoint + " damage per point of stamina";
				image = SpriteBase.mainSpriteBase.skull;
				
			}

			protected override void ApplyPlayCosts()
			{
				damage = userCharGraphic.GetStamina() * damagePerStaminaPoint;
				base.ApplyPlayCosts();
			}
		}
	}

	public class Slash : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Slash";
			description = "Targets the strongest enemy";
			image = SpriteBase.mainSpriteBase.skull;
			damage = 30;
			staminaCost = 1;
			targetType = TargetType.Strongest;
		}
	}

	public class Tear : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Tear";
			description = "Targets the strongest enemy";
			image = SpriteBase.mainSpriteBase.skull;
			damage = 50;
			staminaCost = 3;
			targetType = TargetType.Strongest;
		}
	}
}

//Skitter cards


public class Bugzilla : EncounterEnemy
{
	protected override void CommonConstructor()
	{
		basicCombatDeck.AddCards(typeof(Throwdown));

		variationCards.Add(new Meatshield());
		variationCards.Add(new Rage());
		variationCards.Add(new Dominance());
	}
	
	protected override void NormalConstructor()
	{
		name = "Bugzilla";
		health = 200;
		stamina = 3;
	}

	protected override void ToughConstructor()
	{
		name = "Tough Bugzilla";
		health = 360;
		stamina = 4;
	}

	public class Rage : EnemyVariationCard
	{
		public Rage()
		{
			//SetLastsForRounds(1);

			name = "Rage";
			image = SpriteBase.mainSpriteBase.lightning;

			description = "Set stamina to 0, add cards to your deck";

			addedCombatCards.Add(new Enrage());
			addedCombatCards.Add(new Gore());
			addedCombatCards.Add(new Charge());
		}

		protected override void ExtenderSpecificActivation()
		{
			appliedToCharacter.SetStamina(0);
		}

		public class Enrage : EffectCard
		{
			protected override void ExtenderConstructor()
			{
				targetType = TargetType.None;
				userStaminaGain = 2;
				
				name = "Enrage";
				description = "Gain "+userStaminaGain+" stamina";
				image = SpriteBase.mainSpriteBase.skull;
			}
		}
		public class Charge : MeleeCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Charge";
				description = "Targets all enemies";
				image = SpriteBase.mainSpriteBase.lateralArrows;
				damage = 10;
				staminaCost = 3;
				targetType = TargetType.AllEnemies;
			}
		}
		public class Gore : MeleeCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Gore";
				image = SpriteBase.mainSpriteBase.leg;
				damage = 40;
				staminaCost = 3;
				targetType = TargetType.SelectEnemy;
			}
		}
	}

	public class Dominance : EnemyVariationCard
	{
		public Dominance()
		{
			name = "Dominance";
			image = SpriteBase.mainSpriteBase.skull;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Pummel());
			addedCombatCards.Add(new Stomp());
		}
		public class Pummel : MeleeCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Pummel";
				description = "Targets the strongest enemy";
				image = SpriteBase.mainSpriteBase.skull;
				damage = 40;
				staminaCost = 2;
				targetType = TargetType.Strongest;
			}
		}
		public class Stomp : MeleeCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Stomp";
				description = "Targets the strongest enemy";
				image = SpriteBase.mainSpriteBase.leg;
				damage = 50;
				staminaCost = 3;
				targetType = TargetType.Strongest;
			}
		}
	}

	public class Meatshield : EnemyVariationCard
	{
		CharacterStipulationCard placedStipulationCard;

		public Meatshield()
		{
			addedCombatCards.Add(new Shield());
			placedStipulationCard = new Shielder();
			
			name = "Meatshield";
			image = SpriteBase.mainSpriteBase.armor;

			description = "Add cards to your deck, play "+placedStipulationCard.name;

			
		}

		protected override void ExtenderSpecificActivation()
		{
			appliedToCharacter.TryPlaceCharacterStipulationCard(placedStipulationCard);
		}

		public class Shield : EffectCard
		{
			CharacterStipulationCard placedStipulationCard;

			protected override bool ExtenderPrerequisitesMet(CharacterGraphic user)
			{
				if (!CombatCardTargeter.main.HasMeleeTargetEnemy())
					return true;
				else
					return false;
			}

			protected override void ExtenderConstructor()
			{
				staminaCost = 1;
				targetType = TargetType.SelectEnemy;
				damage = 10;

				placedStipulationCard = new Shielder();

				name = "Shield";
				description = "If no block is present:" + placedStipulationCard.description;
				image = SpriteBase.mainSpriteBase.armor;
			}

			protected override void ApplyEffects()
			{
				userCharGraphic.TryPlaceCharacterStipulationCard(placedStipulationCard);
			}
		}

		public class Shielder : CharacterStipulationCard
		{
			public Shielder()
			{
				name = "Shielder";
				image = SpriteBase.mainSpriteBase.cover;
				description = "All melee attacks played by enemies will only target this character";
			}

			protected override void ExtenderSpecificActivation()
			{
				CombatCardTargeter.main.SetNewMeleeTargetEnemy(appliedToCharacter);
				CombatCardTargeter.ENewMeleeTargetEnemySet += RemoveCard;
			}
			void RemoveCard()
			{
				appliedToCharacter.RemoveCharacterStipulationCard(this);
			}

			protected override void ExtenderSpecificDeactivation()
			{
				CombatCardTargeter.main.ClearMeleeTargetEnemy();
				CombatCardTargeter.ENewMeleeTargetEnemySet -= RemoveCard;
			}
		}
	}

	public class Throwdown : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Throwdown";
			description = "Targets the strongest enemy";
			image = SpriteBase.mainSpriteBase.skull;
			damage = 30;
			staminaCost = 1;
			targetType = TargetType.Strongest;
		}
	}
}

public class Stinger : EncounterEnemy
{
	protected override void CommonConstructor()
	{
		basicCombatDeck.AddCards(typeof(Lash));
		basicCombatDeck.AddCards(typeof(Regrow));

		variationCards.Add(new Hunter());
		variationCards.Add(new Poison());
	}
	
	protected override void NormalConstructor()
	{
		name = "Stinger";
		health = 100;
		stamina = 2;
		ammo = 6;	
	}

	protected override void ToughConstructor()
	{
		name = "Tough Stinger";
		health = 160;
		stamina = 4;
		ammo = 8;
	}

	public class Poison : EnemyVariationCard
	{
		public Poison()
		{
			name = "Poison";
			image = SpriteBase.mainSpriteBase.lateralArrows;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Venom());
		}

		public class Venom : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				damage = 20;
				ammoCost = 3;
				ignoresArmor = true;
				targetType = TargetType.SelectEnemy;

				name = "Venom";
				image = SpriteBase.mainSpriteBase.droplet;
				description = "Ignores armor";
			}
		}
	}

	public class Hunter : EnemyVariationCard
	{
		public Hunter()
		{
			name = "Hunter";
			image = SpriteBase.mainSpriteBase.lateralArrows;

			description = "Add cards to your deck";
			addedCombatCards.Add(new Corner());
		}
		

		public class Corner : MeleeCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Corner";
				description = "Targets the weakest enemy";
				image = SpriteBase.mainSpriteBase.skull;
				damage = 20;
				staminaCost = 2;
				targetType = TargetType.Weakest;
			}
		}
	}

	public class Regrow : EffectCard
	{
		protected override bool ExtenderPrerequisitesMet(CharacterGraphic user)
		{
			if (user.GetAmmo() == 0)
				return true;
			else
				return false;
		}

		protected override void ExtenderConstructor()
		{
			staminaCost = 1;
			targetType = TargetType.None;

			name = "Regrow";
			image = SpriteBase.mainSpriteBase.restSprite;
			description = "If character has no ammo, restore ammo";
		}

		protected override void ApplyEffects()
		{
			userCharGraphic.ResetCharacterResource(CharacterGraphic.Resource.Ammo);
		}
	}

	public class Lash : RangedCard
	{
		protected override void ExtenderConstructor()
		{
			damage = 30;
			ammoCost = 2;
			targetType = TargetType.SelectEnemy;

			name = "Lash";
			image = SpriteBase.mainSpriteBase.arrow;
		}
	}

	public class Sting : RangedCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Sting";
			//description = "Restores " + staminaRestore + " stamina";
			image = SpriteBase.mainSpriteBase.droplet;

			staminaDamage = 2;
			ammoCost = 2;
			targetType = TargetType.SelectEnemy;
		}
	}
}

//Stinger cards



public class Puffer : EncounterEnemy
{
	protected override void CommonConstructor()
	{
		basicCombatDeck.AddCards(typeof(Inflate));

		variationCards.Add(new Spread());
		variationCards.Add(new Direct());
	}
	
	protected override void NormalConstructor()
	{
		name = "Puffer";
		health = 180;
		stamina = 4;
		ammo = 0;
	}

	protected override void ToughConstructor()
	{
		name = "Tough Puffer";
		health = 280;
		stamina = 5;
		ammo = 2;
	}

	public class Spread : EnemyVariationCard
	{
		public Spread()
		{
			name = "Spread";
			image = SpriteBase.mainSpriteBase.lateralArrows;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Pop());
		}
		public class Pop : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				ammoCost = 2;
				removeHealthCost = 20;
				damage = 20;
				targetType = TargetType.AllEnemies;

				name = "Pop";
				description = "Targets all enemies, puffer loses " + removeHealthCost + " health";
				image = SpriteBase.mainSpriteBase.cloud;
			}
		}
	}
	public class Direct : EnemyVariationCard
	{
		public Direct()
		{
			name = "Direct";
			image = SpriteBase.mainSpriteBase.arrow;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Vomit());
		}
		public class Vomit : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				ammoCost = 1;
				removeHealthCost = 10;
				damage = 30;
				targetType = TargetType.SelectEnemy;

				name = "Vomit";
				description = "Lose " + removeHealthCost + " health";
				image = SpriteBase.mainSpriteBase.droplet;
			}
		}
	}

	public class Inflate : EffectCard
	{
		protected override bool ExtenderPrerequisitesMet(CharacterGraphic user)
		{
			if (user.GetAmmo() == 0)
				return true;
			else
				return false;
		}
		
		protected override void ExtenderConstructor()
		{
			staminaCost = 3;
			userAmmoGain = 2;
			targetType = TargetType.None;

			name = "Inflate";
			description = "If character has no ammo, gain " + userAmmoGain + " ammo";
			image = SpriteBase.mainSpriteBase.restSprite;
		}
	}
}
public class Hardshell : EncounterEnemy
{
	protected override void CommonConstructor()
	{
		basicCombatDeck.AddCards(typeof(Pinch));

		variationCards.Add(new TurtleUp());
		variationCards.Add(new Warrior());
	}
	
	protected override void NormalConstructor()
	{
		name = "Hardshell";
		health = 80;
		armor = 100;
		stamina = 4;

		
	}

	protected override void ToughConstructor()
	{
		name = "Tough Hardshell";
		health = 80;
		armor = 200;
		stamina = 4;

		variationCards.Add(new Blocker());
	}

	public class TurtleUp : EnemyVariationCard
	{
		public TurtleUp()
		{
			name = "Turtle Up";
			image = SpriteBase.mainSpriteBase.cover;

			description = "Add cards to your deck";

			addedCombatCards.Add(new CurlUp());
		}

		public class CurlUp : EffectCard
		{
			protected override void ExtenderConstructor()
			{
				staminaCost = 2;
				targetType = TargetType.None;

				name = "Curl Up";
				description = "Play Harden to the character";
				image = SpriteBase.mainSpriteBase.restSprite;

				addedStipulationCard = new Harden();
			}

			public class Harden : CharacterStipulationCard
			{
				int armorGain = 30;

				public Harden()
				{
					name = "Harden";
					image = SpriteBase.mainSpriteBase.cover;
					description = "Until damaged: gain " + armorGain + " armor per round";
				}

				protected override void ExtenderSpecificActivation()
				{
					appliedToCharacter.ETookDamage += RemoveCard;
					CombatManager.ERoundIsOver += GainArmor;
				}
				void GainArmor()
				{
					appliedToCharacter.IncrementArmor(armorGain);
				}
				void RemoveCard()
				{
					appliedToCharacter.RemoveCharacterStipulationCard(this);
				}

				protected override void ExtenderSpecificDeactivation()
				{
					appliedToCharacter.ETookDamage -= RemoveCard;
					CombatManager.ERoundIsOver -= GainArmor;
				}
			}
		}
	}

	public class Warrior : EnemyVariationCard
	{
		public Warrior()
		{
			name = "Warrior";
			image = SpriteBase.mainSpriteBase.skull;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Ram());
		}

		public class Ram : MeleeCard
		{
			int bonusDamageForHavingArmor = 30;
			
			protected override void ExtenderConstructor()
			{
				staminaCost = 2;
				targetType = TargetType.SelectEnemy;
				damage = 10;

				name = "Ram";
				description = "If hardshell has armor, deal extra "+bonusDamageForHavingArmor+" damage";
				image = SpriteBase.mainSpriteBase.lateralArrows;
			}

			protected override void DamageTargets()
			{
				if (userCharGraphic.GetArmor() > 0)
					damage += bonusDamageForHavingArmor;
				base.DamageTargets();
			}
		}
	}

	public class Blocker : EnemyVariationCard
	{
		CharacterStipulationCard placedStipulationCard;
			
		public Blocker()
		{
			addedCombatCards.Add(new Block());
			placedStipulationCard = new BlockStipulationCard();
			
			name = "Blocker";
			image = SpriteBase.mainSpriteBase.armor;

			description = "Add cards to your deck, play " + placedStipulationCard.name;
		}

		protected override void ExtenderSpecificActivation()
		{
			appliedToCharacter.TryPlaceCharacterStipulationCard(placedStipulationCard);
		}

		public class Block : EffectCard
		{
			CharacterStipulationCard placedStipulationCard;

			protected override bool ExtenderPrerequisitesMet(CharacterGraphic user)
			{
				if (user.GetArmor() > 0 && !CombatCardTargeter.main.HasMeleeTargetEnemy())
					return true;
				else
					return false;
			}

			protected override void ExtenderConstructor()
			{
				staminaCost = 1;
				targetType = TargetType.None;

				placedStipulationCard = new BlockStipulationCard();

				name = "Block";
				description = "If no block is present:" + placedStipulationCard.description;
				image = SpriteBase.mainSpriteBase.armor;
			}

			protected override void ApplyEffects()
			{
				userCharGraphic.TryPlaceCharacterStipulationCard(placedStipulationCard);
			}
		}

		public class BlockStipulationCard : CharacterStipulationCard
		{
			public BlockStipulationCard()
			{
				name = "Block";
				image = SpriteBase.mainSpriteBase.cover;
				description = "While character has armor: all melee attacks played by enemies will only target this character";
			}

			protected override void ExtenderSpecificActivation()
			{
				CombatCardTargeter.main.SetNewMeleeTargetEnemy(appliedToCharacter);
				CombatCardTargeter.ENewMeleeTargetEnemySet += RemoveCard;
				appliedToCharacter.ETookDamage += CheckIfCharacterHasArmor;
			}

			void CheckIfCharacterHasArmor()
			{
				if (appliedToCharacter.GetArmor() == 0)
					RemoveCard();
			}
			void RemoveCard()
			{
				appliedToCharacter.RemoveCharacterStipulationCard(this);
			}

			protected override void ExtenderSpecificDeactivation()
			{
				CombatCardTargeter.main.ClearMeleeTargetEnemy();
				CombatCardTargeter.ENewMeleeTargetEnemySet -= RemoveCard;
				appliedToCharacter.ETookDamage -= CheckIfCharacterHasArmor;
			}
		}
	}

	public class Pinch : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Pinch";
			image = SpriteBase.mainSpriteBase.skull;
			damage = 20;
			staminaCost = 1;
			targetType = TargetType.SelectEnemy;
		}
	}
}
