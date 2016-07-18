using UnityEngine;
using System.Collections;

public class Infestation {

}

public class Skitter : EncounterEnemy
{
	public Skitter()
		: base()
	{
		name = "Skitter";
		health = 80;

		stamina = 6;

		combatDeck.AddCards(typeof(Slash), 2);
		combatDeck.AddCards(typeof(Tear), 2);
		variationCards.Add(new Vulture());
		variationCards.Add(new Striker());
	}

	public class Vulture : CharacterStipulationCard
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

	public class Striker : CharacterStipulationCard
	{
		int damage = 20;

		public Striker()
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
	public Bugzilla()
		: base()
	{
		name = "Bugzilla";
		health = 140;
		stamina = 3;

		combatDeck.AddCards(typeof(Throwdown), 2);

		variationCards.Add(new Meatshield());
		variationCards.Add(new Rage());
		variationCards.Add(new Dominance());
	}

	public class Meatshield : CharacterStipulationCard
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

	public class Rage : CharacterStipulationCard
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

	public class Dominance : CharacterStipulationCard
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
	public Stinger()
		: base()
	{
		name = "Stinger";
		health = 80;
		stamina = 4;
		ammo = 2;

		combatDeck.AddCards(typeof(Lash), 2);

		variationCards.Add(new Hunter());
		variationCards.Add(new Range());
	}

	public class Range : CharacterStipulationCard
	{
		public Range()
		{
			name = "Range";
			image = SpriteBase.mainSpriteBase.lateralArrows;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Sting());
			addedCombatCards.Add(new Venom());
			addedCombatCards.Add(new Venom());
			addedCombatCards.Add(new Regrow());
		}
		public class Regrow : EffectCard
		{
			protected override void ExtenderConstructor()
			{
				staminaCost = 2;
				userAmmoGain = 2;
				targetType = TargetType.None;

				name = "Regrow";
				image = SpriteBase.mainSpriteBase.restSprite;
				description = "Restores " + userAmmoGain + " ammo";
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

	public class Hunter : CharacterStipulationCard
	{
		public Hunter()
		{
			name = "Hunter";
			image = SpriteBase.mainSpriteBase.lateralArrows;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Lunge());
			addedCombatCards.Add(new Lunge());
			addedCombatCards.Add(new Corner());
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
			image = SpriteBase.mainSpriteBase.skull;
		}
	}
}

//Stinger cards



public class Puffer : EncounterEnemy
{
	public Puffer()
		: base()
	{
		name = "Puffer";
		health = 120;
		stamina = 4;
		ammo = 0;

		combatDeck.AddCards(typeof(Inflate), 2);

		variationCards.Add(new Spread());
		variationCards.Add(new Direct());
	}

	public class Spread : CharacterStipulationCard
	{
		public Spread()
		{
			name = "Spread";
			image = SpriteBase.mainSpriteBase.lateralArrows;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Pop());
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
	public class Direct : CharacterStipulationCard
	{
		public Direct()
		{
			name = "Direct";
			image = SpriteBase.mainSpriteBase.arrow;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Vomit());
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

				name = "Pop";
				description = "Lose " + removeHealthCost + " health";
				image = SpriteBase.mainSpriteBase.droplet;
			}
		}
	}

	public class Inflate : EffectCard
	{
		protected override void ExtenderConstructor()
		{
			staminaCost = 3;
			userAmmoGain = 2;
			targetType = TargetType.None;

			name = "Inflate";
			description = "Gains " + userAmmoGain + " ammo";
			image = SpriteBase.mainSpriteBase.restSprite;
			//healthDamage = 5;

		}
	}
}
public class Hardshell : EncounterEnemy
{
	public Hardshell()
		: base()
	{
		name = "Hardshell";
		health = 80;
		armor = 40;
		stamina = 4;

		combatDeck.AddCards(typeof(Snap));
		combatDeck.AddCards(typeof(Pinch));

		variationCards.Add(new TurtleUp());
		variationCards.Add(new Warrior());
	}

	public class TurtleUp : CharacterStipulationCard
	{
		public TurtleUp()
		{
			name = "Turtle Up";
			image = SpriteBase.mainSpriteBase.cover;

			description = "Add cards to your deck";

			addedCombatCards.Add(new CurlUp());
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

	public class Warrior : CharacterStipulationCard
	{
		public Warrior()
		{
			name = "Warrior";
			image = SpriteBase.mainSpriteBase.skull;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Ram());
			addedCombatCards.Add(new Ram());
		}

		public class Ram : MeleeCard
		{
			int bonusDamageForHavingArmor = 30;
			
			protected override void ExtenderConstructor()
			{
				staminaCost = 1;
				targetType = TargetType.None;
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
