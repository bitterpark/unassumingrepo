using UnityEngine;
using System.Collections;

public class Infestation {

}

public class Skitter : EncounterEnemy
{
	protected override void NormalConstructor()
	{
		name = "Skitter";
		health = 80;

		stamina = 6;

		basicCombatDeck.AddCards(typeof(Slash));
		basicCombatDeck.AddCards(typeof(Tear));
		variationCards.Add(new Vulture());
		variationCards.Add(new Latcher());
	}

	protected override void ToughConstructor()
	{
		name = "Tough Skitter";
		health = 80;
		armor = 60;

		stamina = 12;

		basicCombatDeck.AddCards(typeof(Slash));
		basicCombatDeck.AddCards(typeof(Tear));

		variationCards.Add(new Vulture());
		variationCards.Add(new Latcher());
		variationCards.Add(new Striker());
	}

	public class Vulture : EnemyVariationCard
	{
		public Vulture()
		{
			//SetLastsForRounds(1);

			name = "Vulture";
			image = SpriteBase.mainSpriteBase.brokenArmsSprite;

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
			image = SpriteBase.mainSpriteBase.brokenArmsSprite;

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
			image = SpriteBase.mainSpriteBase.brokenArmsSprite;

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
	protected override void NormalConstructor()
	{
		name = "Bugzilla";
		health = 140;
		stamina = 3;

		basicCombatDeck.AddCards(typeof(Throwdown));

		variationCards.Add(new Meatshield());
		variationCards.Add(new Rage());
		variationCards.Add(new Dominance());
	}

	protected override void ToughConstructor()
	{
		name = "Tough Bugzilla";
		health = 300;
		stamina = 4;

		basicCombatDeck.AddCards(typeof(Throwdown));

		variationCards.Add(new Meatshield());
		variationCards.Add(new Rage());
		variationCards.Add(new Dominance());
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
				image = SpriteBase.mainSpriteBase.brokenLegsSprite;
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
				image = SpriteBase.mainSpriteBase.brokenLegsSprite;
				damage = 50;
				staminaCost = 3;
				targetType = TargetType.Strongest;
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
	protected override void NormalConstructor()
	{
		name = "Stinger";
		health = 80;
		stamina = 4;
		ammo = 2;

		basicCombatDeck.AddCards(typeof(Lash), 2);

		variationCards.Add(new Hunter());
		variationCards.Add(new Range());
	}

	protected override void ToughConstructor()
	{
		name = "Tough Stinger";
		health = 100;
		stamina = 6;
		ammo = 6;

		basicCombatDeck.AddCards(typeof(Lash));

		variationCards.Add(new Hunter());
		variationCards.Add(new Range());
	}

	public class Range : EnemyVariationCard
	{
		public Range()
		{
			name = "Range";
			image = SpriteBase.mainSpriteBase.lateralArrows;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Sting());
			addedCombatCards.Add(new Venom());
			addedCombatCards.Add(new Regrow());
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
				staminaCost = 2;
				userAmmoGain = 2;
				targetType = TargetType.None;

				name = "Regrow";
				image = SpriteBase.mainSpriteBase.restSprite;
				description = "If character has no ammo, restore " + userAmmoGain + " ammo";
			}
		}
		public class Sting : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				damage = 20;
				ammoCost = 2;
				staminaCost = 1;
				ignoresArmor = true;
				targetType = TargetType.SelectEnemy;

				name = "Sting";
				image = SpriteBase.mainSpriteBase.lightning;
				description = "Ignores armor";
			}
		}

		public class Venom : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Venom";
				//description = "Restores " + staminaRestore + " stamina";
				image = SpriteBase.mainSpriteBase.droplet;

				staminaDamage = 3;
				staminaCost = 2;
				ammoCost = 1;
				targetType = TargetType.SelectEnemy;
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

			addedCombatCards.Add(new Lunge());
			addedCombatCards.Add(new Corner());
		}
		public class Lunge : MeleeCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Lunge";
				description = "Targets the weakest enemy";
				image = SpriteBase.mainSpriteBase.lateralArrows;
				damage = 10;
				staminaCost = 1;
				targetType = TargetType.Weakest;
			}
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

	public class Lash : RangedCard
	{
		protected override void ExtenderConstructor()
		{
			damage = 30;
			staminaCost = 2;
			targetType = TargetType.SelectEnemy;

			name = "Lash";
			image = SpriteBase.mainSpriteBase.arrow;
		}
	}
}

//Stinger cards



public class Puffer : EncounterEnemy
{
	protected override void NormalConstructor()
	{
		name = "Puffer";
		health = 120;
		stamina = 4;
		ammo = 0;

		basicCombatDeck.AddCards(typeof(Inflate), 2);

		variationCards.Add(new Spread());
		variationCards.Add(new Direct());
	}

	protected override void ToughConstructor()
	{
		name = "Tough Puffer";
		health = 240;
		stamina = 4;
		ammo = 2;

		basicCombatDeck.AddCards(typeof(Inflate));

		variationCards.Add(new Spread());
		variationCards.Add(new Direct());
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
	protected override void NormalConstructor()
	{
		name = "Hardshell";
		health = 80;
		armor = 40;
		stamina = 4;

		basicCombatDeck.AddCards(typeof(Snap));
		basicCombatDeck.AddCards(typeof(Pinch));

		variationCards.Add(new TurtleUp());
		variationCards.Add(new Warrior());
	}

	protected override void ToughConstructor()
	{
		name = "Tough Hardshell";
		health = 80;
		armor = 200;
		stamina = 4;

		basicCombatDeck.AddCards(typeof(Snap));
		basicCombatDeck.AddCards(typeof(Pinch));

		variationCards.Add(new TurtleUp());
		variationCards.Add(new Warrior());
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
					CardsScreen.ERoundIsOver += GainArmor;
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
					CardsScreen.ERoundIsOver -= GainArmor;
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
				staminaCost = 1;
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
		public Blocker()
		{
			name = "Blocker";
			image = SpriteBase.mainSpriteBase.cover;
			description = "While character has armor: all melee attacks played by enemies will only target this character";
		}

		protected override void ExtenderSpecificActivation()
		{
			CardsScreen.main.SetNewMeleeTargetEnemy(appliedToCharacter);
			CardsScreen.ENewMeleeTargetEnemySet += RemoveCard;
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
			CardsScreen.main.ClearMeleeTargetEnemy();
			CardsScreen.ENewMeleeTargetEnemySet -= RemoveCard;
			appliedToCharacter.ETookDamage -= CheckIfCharacterHasArmor;
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
	public class Snap : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Snap";
			image = SpriteBase.mainSpriteBase.skull;
			damage = 30;
			staminaCost = 2;
			targetType = TargetType.SelectEnemy;
		}
	}
}

public class Meatshield : EnemyVariationCard
{
	public Meatshield()
	{
		name = "Meatshield";
		image = SpriteBase.mainSpriteBase.cover;
		description = "All melee attacks played by enemies will only target this character";
	}

	protected override void ExtenderSpecificActivation()
	{
		CardsScreen.main.SetNewMeleeTargetEnemy(appliedToCharacter);
		CardsScreen.ENewMeleeTargetEnemySet += RemoveCard;
	}
	void RemoveCard()
	{
		appliedToCharacter.RemoveCharacterStipulationCard(this);
	}

	protected override void ExtenderSpecificDeactivation()
	{
		CardsScreen.main.ClearMeleeTargetEnemy();
		CardsScreen.ENewMeleeTargetEnemySet -= RemoveCard;
	}
}
