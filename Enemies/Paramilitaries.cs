using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Marksman : EncounterEnemy
{
	protected override void NormalConstructor()
	{
		name = "Marksman";
		health = 60;
		armor = 20;
		stamina = 2;
		ammo = 4;

		basicCombatDeck.AddCards(typeof(Reload));
		basicCombatDeck.AddCards(typeof(BoobyTrap));

		variationCards.Add(new Finisher());
		variationCards.Add(new Softener());
	}
	protected override void ToughConstructor()
	{
		name = "Tough Marksman";
		health = 60;
		armor = 80;
		stamina = 2;
		ammo = 8;

		basicCombatDeck.AddCards(typeof(Reload));
		basicCombatDeck.AddCards(typeof(BoobyTrap));

		variationCards.Add(new Finisher());
		variationCards.Add(new Softener());
		variationCards.Add(new Sniper());
	}

	public class Finisher : EnemyVariationCard
	{
		public Finisher()
		{
			//SetLastsForRounds(1);

			name = "Finisher";
			image = SpriteBase.mainSpriteBase.skull;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Eliminate());
		}

		public class Eliminate : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Eliminate";
				description = "Targets weakest enemy";
				image = SpriteBase.mainSpriteBase.skull;

				damage = 20;
				ammoCost = 2;
				targetType = TargetType.Weakest;
			}
		}
	}

	public class Softener : EnemyVariationCard
	{
		int damage = 20;

		public Softener()
		{
			//SetLastsForRounds(1);

			name = "Softener";
			image = SpriteBase.mainSpriteBase.crosshair;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Soften());
		}

		public class Soften : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Soften";
				description = "Targets the toughest enemy";
				image = SpriteBase.mainSpriteBase.bullet;

				damage = 40;
				ammoCost = 2;
				targetType = TargetType.Strongest;
			}
		}
	}

	public class Sniper : EnemyVariationCard
	{
		int damage = 20;

		public Sniper()
		{
			name = "Sniper";
			image = SpriteBase.mainSpriteBase.crosshair;

			description = "Add cards to your deck";

			addedCombatCards.Add(new HighGround());
			addedCombatCards.Add(new SemiAuto());
		}

		public class HighGround : EffectCard
		{
			protected override void ExtenderConstructor()
			{
				targetType = TargetType.None;
				ammoCost = 4;

				name = "High Ground";
				description = "Play a Killzone to the character";
				image = SpriteBase.mainSpriteBase.arrow;
				addedStipulationCard = new Killzone();
			}
		}

		public class Killzone : CharacterStipulationCard
		{
			int damageForMeleeAttacks=60;

			public Killzone()
			{
				SetLastsForRounds(2);
				
				name = "Killzone";
				image = SpriteBase.mainSpriteBase.roseOfWinds;

				description = "For one round: any enemy playing a melee attack takes " + damageForMeleeAttacks + " damage";
			}

			protected override void ExtenderSpecificActivation()
			{
				MeleeCard.EMeleeCardPlayed += EffectTriggered;
			}
			void EffectTriggered(CharacterGraphic cardPlayer, MeleeCard playedCard)
			{
				if (cardPlayer.GetType()==typeof(MercGraphic))
					cardPlayer.TakeDamage(damageForMeleeAttacks);
			}
			protected override void ExtenderSpecificDeactivation()
			{
				MeleeCard.EMeleeCardPlayed -= EffectTriggered;
			}
		}
	}

	public class BoobyTrap : EffectCard
	{
		int trapDamage;

		protected override void ExtenderConstructor()
		{
			targetType = TargetType.None;
			ammoCost = 1;
			trapDamage = 30;

			name = "Booby Trap";
			description = "Play a Tripmine to the character";
			image = SpriteBase.mainSpriteBase.bomb;
			addedStipulationCard = new Tripmine(trapDamage);
		}

		public class Tripmine : CharacterStipulationCard
		{
			int damage;

			public Tripmine(int damage)
			{
				name = "Tripmine";
				image = SpriteBase.mainSpriteBase.brokenArmsSprite;

				this.damage = damage;
				description = "Any enemy playing a melee attack against this character takes "+damage+" damage, and removes the card";
			}

			protected override void ExtenderSpecificActivation()
			{
				MeleeCard.EMeleeCardPlayed += EffectTriggered;
			}
			void EffectTriggered(CharacterGraphic cardPlayer, MeleeCard playedCard)
			{
				if (playedCard.targetChars[0] == appliedToCharacter)
				{
					cardPlayer.TakeDamage(damage);
					appliedToCharacter.RemoveCharacterStipulationCard(this);
				}
			}
			protected override void ExtenderSpecificDeactivation()
			{
				MeleeCard.EMeleeCardPlayed -= EffectTriggered;
			}
		}
	}
}

public class Reload : EffectCard
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
		name = "Reload";
		description = "If ammo is 0, restore ammo to full";
		image = SpriteBase.mainSpriteBase.bullet;

		staminaCost = 1;
		targetType = TargetType.None;
	}

	protected override void ApplyEffects()
	{
		userCharGraphic.SetStartAmmo();
	}
}

public class HeavyGunner : EncounterEnemy
{
	protected override void NormalConstructor()
	{
		name = "Heavy Gunner";
		health = 70;
		armor = 30;
		stamina = 2;
		ammo = 3;

		basicCombatDeck.AddCards(typeof(Reload));
		basicCombatDeck.AddCards(typeof(CQC));

		variationCards.Add(new CrowdControl());
		variationCards.Add(new TargetPriority());
	}

	protected override void ToughConstructor()
	{
		name = "Tough Gunner";
		health = 70;
		armor = 60;
		stamina = 2;
		ammo = 6;

		basicCombatDeck.AddCards(typeof(Reload));
		basicCombatDeck.AddCards(typeof(CQC));

		variationCards.Add(new CrowdControl());
		variationCards.Add(new TargetPriority());
	}

	public class CrowdControl : EnemyVariationCard
	{
		public CrowdControl()
		{
			name = "Crowd Control";
			image = SpriteBase.mainSpriteBase.bullets;

			description = "Add cards to your deck";

			addedCombatCards.Add(new WideAngle());
			addedCombatCards.Add(new Suppression());
		}

		public class WideAngle : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Wide Angle";
				description = "Targets all enemies";
				image = SpriteBase.mainSpriteBase.bullets;

				damage = 10;
				ammoCost = 3;
				targetType = TargetType.AllEnemies;
			}
		}

		public class Suppression : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Suppressive Fire";
				description = "Targets all enemies";
				image = SpriteBase.mainSpriteBase.bullets;

				staminaDamage = 1;
				ammoCost = 3;
				targetType = TargetType.AllEnemies;
			}
		}
	}

	public class TargetPriority : EnemyVariationCard
	{
		public TargetPriority()
		{
			name = "Target Priority";
			image = SpriteBase.mainSpriteBase.bullets;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Machinegun());
			addedCombatCards.Add(new PinDown());
		}

		public class Machinegun : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Machinegun";
				description = "Targets strongest enemy";
				image = SpriteBase.mainSpriteBase.bullets;

				damage = 50;
				ammoCost = 3;
				targetType = TargetType.Strongest;
			}
		}

		public class PinDown : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Pin Down";
				image = SpriteBase.mainSpriteBase.lightning;

				staminaDamage = 3;
				ammoCost = 3;
				targetType = TargetType.SelectEnemy;
			}
		}
	}

	
}

public class ShockTrooper : EncounterEnemy
{
	protected override void NormalConstructor()
	{
		name = "Shock Trooper";
		health = 60;
		armor = 20;
		stamina = 4;
		ammo = 2;

		basicCombatDeck.AddCards(typeof(Reload));
		basicCombatDeck.AddCards(typeof(Blindfire));
		basicCombatDeck.AddCards(typeof(CQC));

		variationCards.Add(new Flanker());
		variationCards.Add(new Shock());
	}
	protected override void ToughConstructor()
	{
		name = "Tough Trooper";
		health = 120;
		armor = 40;
		stamina = 6;
		ammo = 4;

		basicCombatDeck.AddCards(typeof(Reload));
		basicCombatDeck.AddCards(typeof(Blindfire));
		basicCombatDeck.AddCards(typeof(CQC));

		variationCards.Add(new Flanker());
		variationCards.Add(new Shock());
	}

	public class Flanker : EnemyVariationCard
	{
		int damage = 20;

		public Flanker()
		{
			name = "Flanker";
			image = SpriteBase.mainSpriteBase.lateralArrows;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Flanking());
		}

		public class Flanking : MeleeCard
		{
			int penaltyDamage;

			protected override void ExtenderConstructor()
			{
				staminaCost = 1;
				ammoCost = 1;
				damage = 10;
				penaltyDamage = 20;
				targetType = TargetType.SelectEnemy;

				name = "Flanking";
				description = "Play a Flanked card to the target";
				image = SpriteBase.mainSpriteBase.lateralArrows;

				addedStipulationCard = new Flanked(penaltyDamage);
			}

			public class Flanked : CharacterStipulationCard
			{
				int damage;

				public Flanked(int damage)
				{
					SetLastsForRounds(2);
					this.damage = damage;
					
					name = "Flanked";
					image = SpriteBase.mainSpriteBase.brokenArmsSprite;

					description = "For one round: character takes "+damage+" damage for every ranged card played";
				}

				protected override void ExtenderSpecificActivation()
				{
					MeleeCard.EMeleeCardPlayed += EffectTriggered;
				}
				void EffectTriggered(CharacterGraphic cardPlayer, MeleeCard playedCard)
				{
					if (playedCard.targetChars[0] == appliedToCharacter)
					{
						cardPlayer.TakeDamage(damage);
						appliedToCharacter.RemoveCharacterStipulationCard(this);
					}

				}
				protected override void ExtenderSpecificDeactivation()
				{
					MeleeCard.EMeleeCardPlayed -= EffectTriggered;
				}
			}
		}
	}

	public class Shock : EnemyVariationCard
	{
		int damage = 20;

		public Shock()
		{
			name = "Shock";
			image = SpriteBase.mainSpriteBase.lightning;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Blitz());
		}
	}

	public class Blitz : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Blitz";
			description = "";
			image = SpriteBase.mainSpriteBase.lightning;

			staminaCost = 2;
			ammoCost = 1;
			damage = 40;
			targetType = TargetType.SelectEnemy;
		}
	}

	public class Blindfire : RangedCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Blind Fire";
			image = SpriteBase.mainSpriteBase.bullets;

			damage = 20;
			ammoCost = 1;
			targetType = TargetType.SelectEnemy;
		}
	}
}

public class CQC : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		name = "CQC";
		description = "Targets the strongest enemy";
		image = SpriteBase.mainSpriteBase.brokenArmsSprite;

		damage = 40;
		staminaCost = 2;
		targetType = TargetType.Strongest;
	}
}

public class Commander : EncounterEnemy
{
	protected override void NormalConstructor()
	{
		name = "Commander";
		health = 40;
		armor = 40;
		stamina = 4;
		ammo = 2;

		basicCombatDeck.AddCards(typeof(Reload));
		basicCombatDeck.AddCards(typeof(SemiAuto));

		variationCards.Add(new Support());
		variationCards.Add(new Tactics());
	}
	protected override void ToughConstructor()
	{
		name = "Tough Commander";
		health = 40;
		armor = 120;
		stamina = 6;
		ammo = 4;

		basicCombatDeck.AddCards(typeof(Reload));
		basicCombatDeck.AddCards(typeof(SemiAuto));
		basicCombatDeck.AddCards(typeof(CQC));

		variationCards.Add(new Support());
		variationCards.Add(new Tactics());
	}

	public class Support : EnemyVariationCard
	{
		public Support()
		{
			name = "Support";
			image = SpriteBase.mainSpriteBase.ammoBoxSprite;

			description = "Add cards to your deck";

			//addedCombatCards.Add(new Rally());
			addedCombatCards.Add(new Smokescreen());
		}

		public class Rally : EffectCard
		{
			int friendliesStaminaGain = 2;

			protected override void ExtenderConstructor()
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
	}

	public class Tactics : EnemyVariationCard
	{
		public Tactics()
		{
			name = "Tactics";
			image = SpriteBase.mainSpriteBase.arrow;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Prioritize());
			addedCombatCards.Add(new Flashbang());
		}

		public class Prioritize : RangedCard
		{
			protected override void ExtenderConstructor()
			{	
				targetType = TargetType.SelectEnemy;
				ammoCost = 1;
				damage = 20;
				addedStipulationCard = new FocusFire();

				name = "Prioritize";
				description = "Play Focus Fire to the target";
				image = SpriteBase.mainSpriteBase.crosshair;
			}

			public class FocusFire : CharacterStipulationCard
			{
				public FocusFire()
				{
					SetLastsForRounds(3);
					
					name = "Focus Fire";
					image = SpriteBase.mainSpriteBase.crosshair;
					description = "For two rounds: all ranged attacks played by enemies will only target this character";
				}

				protected override void ExtenderSpecificActivation()
				{
					CardsScreen.main.SetNewRangedTargetMerc(appliedToCharacter);
					CardsScreen.ENewRangedTargetMercSet += RemoveCard;
				}
				void RemoveCard()
				{
					appliedToCharacter.RemoveCharacterStipulationCard(this);
				}

				protected override void ExtenderSpecificDeactivation()
				{
					CardsScreen.main.ClearRangedTargetMerc();
					CardsScreen.ENewRangedTargetMercSet -= RemoveCard;
				}
			}
		}

		public class Flashbang : RangedCard
		{
			int noCoverStaminaDamageBonus = 1;

			protected override void ExtenderConstructor()
			{
				name = "Flashbang";
				description = "Targets all enemies, characters with 0 armor lose extra " + noCoverStaminaDamageBonus + " stamina";
				image = SpriteBase.mainSpriteBase.lightning;
				targetType = TargetType.AllEnemies;

				ammoCost = 2;
				staminaCost = 1;
				staminaDamage = 1;
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
}

public class SemiAuto : RangedCard
{
	protected override void ExtenderConstructor()
	{
		name = "Semi Auto";
		image = SpriteBase.mainSpriteBase.nineMSprite;
		targetType = TargetType.SelectEnemy;
		damage = 30;
		ammoCost = 2;
	}
}