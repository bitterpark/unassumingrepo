using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Marksman : EncounterEnemy
{
	protected override void CommonConstructor()
	{
		basicCombatDeck.AddCards(typeof(HighGround));

		variationCards.Add(new Finisher());
		variationCards.Add(new Softener());
	}
	
	protected override void NormalConstructor()
	{
		name = "Marksman";
		health = 60;
		armor = 70;
		stamina = 1;
		ammo = 6;
		
	}
	protected override void ToughConstructor()
	{
		name = "Tough Marksman";
		health = 60;
		armor = 120;
		stamina = 2;
		ammo = 9;

		variationCards.Add(new Sniper());
	}

	public class Finisher : EnemyVariationCard
	{
		public Finisher()
		{
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
				description = "Targets the weakest enemy";
				image = SpriteBase.mainSpriteBase.skull;

				damage = 30;
				ammoCost = 2;
				targetType = TargetType.Weakest;
			}
		}
	}

	public class Softener : EnemyVariationCard
	{
		public Softener()
		{
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

				damage = 50;
				ammoCost = 2;
				targetType = TargetType.Strongest;
			}
		}
	}

	public class Sniper : EnemyVariationCard
	{
		public Sniper()
		{
			name = "Sniper";
			image = SpriteBase.mainSpriteBase.crosshair;

			description = "Add cards to your deck";

			addedCombatCards.Add(new AimedShot());
		}

		public class AimedShot : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Aimed Shot";
				image = SpriteBase.mainSpriteBase.crosshair;
				targetType = TargetType.SelectEnemy;
				damage = 40;
				ammoCost = 3;
			}
		}
	}

	public class HighGround : EffectCard
	{
		protected override void ExtenderConstructor()
		{
			targetType = TargetType.None;
			staminaCost = 1;
			addedStipulationCard = new Killzone();

			name = "High Ground";
			description = addedStipulationCard.description;//"Play a Killzone to the character";
			image = SpriteBase.mainSpriteBase.arrow;

		}

		public class Killzone : CharacterStipulationCard
		{
			int damageForMeleeAttacks = 30;

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
				if (cardPlayer.GetType() == typeof(MercGraphic))
					cardPlayer.TakeDamage(damageForMeleeAttacks);
			}
			protected override void ExtenderSpecificDeactivation()
			{
				MeleeCard.EMeleeCardPlayed -= EffectTriggered;
			}
		}
	}
	//currently unused
	public class BoobyTrap : EffectCard
	{
		int trapDamage;

		protected override void ExtenderConstructor()
		{
			targetType = TargetType.None;
			staminaCost = 1;
			trapDamage = 20;
			addedStipulationCard = new Tripmine(trapDamage);

			name = "Booby Trap";
			description = addedStipulationCard.description;
			image = SpriteBase.mainSpriteBase.bomb;
			
		}

		public class Tripmine : CharacterStipulationCard
		{
			int damage;

			public Tripmine(int damage)
			{
				name = "Tripmine";
				image = SpriteBase.mainSpriteBase.arm;

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
//currently unused
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
	protected override void CommonConstructor()
	{
		basicCombatDeck.AddCards(typeof(Overwatch));

		variationCards.Add(new CrowdControl());
		variationCards.Add(new TargetPriority());
	}
	
	protected override void NormalConstructor()
	{
		name = "Heavy Gunner";
		health = 70;
		armor = 50;
		stamina = 3;
		ammo = 4;
	}

	protected override void ToughConstructor()
	{
		name = "Tough Gunner";
		health = 70;
		armor = 60;
		stamina = 2;
		ammo = 9;

		variationCards.Add(new SupportGunner());
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
				ammoCost = 2;
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
				ammoCost = 2;
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
				description = "Targets toughest enemy";
				image = SpriteBase.mainSpriteBase.bullets;

				damage = 50;
				ammoCost = 2;
				targetType = TargetType.Strongest;
			}
		}

		public class PinDown : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Pin Down";
				description = "Targets the toughest enemy";
				image = SpriteBase.mainSpriteBase.lightning;

				staminaDamage = 4;
				ammoCost = 2;
				targetType = TargetType.Strongest;
			}
		}
	}

	public class SupportGunner : EnemyVariationCard
	{
		public SupportGunner()
		{
			name = "Support Gunner";
			image = SpriteBase.mainSpriteBase.armor;

			description = "Add cards to your deck";

			addedCombatCards.Add(new EatLead());
			addedCombatCards.Add(new FireSupport());
		
		}

		public class EatLead : RangedCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Eat Lead";
				image = SpriteBase.mainSpriteBase.bullets;
				targetType = TargetType.SelectEnemy;
				damage = 40;
				ammoCost = 3;
			}
		}

		public class FireSupport : EffectCard
		{
			protected override void ExtenderConstructor()
			{
				targetType = TargetType.None;
				staminaCost = 2;
				addedStipulationCard = new FireSupportStipulation();

				name = "Fire Support";
				description = addedStipulationCard.description;//"Play a Killzone to the character";
				image = SpriteBase.mainSpriteBase.armor;

			}

			public class FireSupportStipulation : CharacterStipulationCard
			{
				int friendlyArmorGain = 10;

				public FireSupportStipulation()
				{
					SetLastsForRounds(3);

					name = "Killzone";
					image = SpriteBase.mainSpriteBase.armor;

					description = "For two rounds: all allies gain "+friendlyArmorGain+" armor for every ranged attack played by the character";
				}

				protected override void ExtenderSpecificActivation()
				{
					RangedCard.ERangedCardPlayed += EffectTriggered;
				}
				void EffectTriggered(CharacterGraphic cardPlayer, RangedCard playedCard)
				{
					if (cardPlayer == appliedToCharacter)
						MissionCharacterManager.main.IncrementAllCharactersResource(CharacterGraphic.Resource.Armor,friendlyArmorGain);
				}
				protected override void ExtenderSpecificDeactivation()
				{
					RangedCard.ERangedCardPlayed -= EffectTriggered;
				}
			}
		}
	}

	

	public class Overwatch : EffectCard
	{
		protected override void ExtenderConstructor()
		{
			targetType = TargetType.None;
			staminaCost = 2;
			addedStipulationCard = new OverwatchStipulation();

			name = "Overwatch";
			description = addedStipulationCard.description;//"Play a Killzone to the character";
			image = SpriteBase.mainSpriteBase.roseOfWinds;

		}

		public class OverwatchStipulation : CharacterStipulationCard
		{
			int damageForRangedAttacks = 30;

			public OverwatchStipulation()
			{
				SetLastsForRounds(2);

				name = "Killzone";
				image = SpriteBase.mainSpriteBase.roseOfWinds;

				description = "For one round: any enemy playing a ranged attack takes " + damageForRangedAttacks + " damage";
			}

			protected override void ExtenderSpecificActivation()
			{
				RangedCard.ERangedCardPlayed += EffectTriggered;
			}
			void EffectTriggered(CharacterGraphic cardPlayer, RangedCard playedCard)
			{
				if (cardPlayer.GetType() == typeof(MercGraphic))
					cardPlayer.TakeDamage(damageForRangedAttacks);
			}
			protected override void ExtenderSpecificDeactivation()
			{
				RangedCard.ERangedCardPlayed -= EffectTriggered;
			}
		}
	}
	
}

public class ShockTrooper : EncounterEnemy
{
	protected override void CommonConstructor()
	{
		basicCombatDeck.AddCards(typeof(SmokeGrenade));
		//basicCombatDeck.AddCards(typeof(SemiAuto));

		variationCards.Add(new Flanker());
		variationCards.Add(new Blitzer());
	}
	
	protected override void NormalConstructor()
	{
		name = "Shock Trooper";
		health = 60;
		armor = 60;
		stamina = 4;
		ammo = 2;

		
	}
	protected override void ToughConstructor()
	{
		name = "Tough Trooper";
		health = 120;
		armor = 40;
		stamina = 9;
		ammo = 4;

		variationCards.Add(new Interference());
	}

	public class Flanker : EnemyVariationCard
	{
		public Flanker()
		{
			name = "Flanker";
			image = SpriteBase.mainSpriteBase.lateralArrows;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Flanking());
		}

		public class Flanking : MeleeCard
		{
			protected override void ExtenderConstructor()
			{
				staminaCost = 2;
				damage = 20;
				int penaltyDamage = 30;

				targetType = TargetType.SelectEnemy;
				addedStipulationCard = new Flanked(penaltyDamage);

				name = "Flanking";
				description = addedStipulationCard.description;
				image = SpriteBase.mainSpriteBase.lateralArrows;

				
			}

			public class Flanked : CharacterStipulationCard
			{
				int damage;

				public Flanked(int damage)
				{
					SetLastsForRounds(2);
					this.damage = damage;
					
					name = "Flanked";
					image = SpriteBase.mainSpriteBase.arm;

					description = "For one round: character takes "+damage+" damage for playing ranged cards";
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

	public class Blitzer : EnemyVariationCard
	{
		public Blitzer()
		{
			name = "Blitzer";
			image = SpriteBase.mainSpriteBase.lightning;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Blitz());
		}

		public class Blitz : MeleeCard
		{
			protected override void ExtenderConstructor()
			{
				name = "Blitz";
				description = "";
				image = SpriteBase.mainSpriteBase.lightning;

				useUpAllStamina = true;
				damage = 10;
				damagePerStaminaPoint = 10;
				targetType = TargetType.SelectEnemy;
			}
		}
	}

	public class Interference : EnemyVariationCard
	{
		public Interference()
		{
			name = "Interference";
			image = SpriteBase.mainSpriteBase.lightning;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Shock());
		}

		public class Shock : EffectCard
		{
			protected override void ExtenderConstructor()
			{
				staminaCost = 3;
				targetType = TargetType.None;
				addedStipulationCard = new ShockStipulation();
				
				name = "Shock";
				description = addedStipulationCard.description;
				image = SpriteBase.mainSpriteBase.lightning;

				
			}

			public class ShockStipulation : CharacterStipulationCard
			{
				int meleeAttackPenaltyDamage = 30;

				public ShockStipulation()
				{
					SetLastsForRounds(2);
					
					name = "Shock";
					image = SpriteBase.mainSpriteBase.lightning;
					description = "For one round: when an enemy plays a melee attack against any ally, deal " + meleeAttackPenaltyDamage+" damage";
				}

				protected override void ExtenderSpecificActivation()
				{
					MeleeCard.EMeleeCardPlayed += TriggerEffect;
				}
				void TriggerEffect( CharacterGraphic cardPlayer, MeleeCard card)
				{
					if (!card.targetChars.Contains(appliedToCharacter))
						cardPlayer.TakeDamage(meleeAttackPenaltyDamage);
				}

				protected override void ExtenderSpecificDeactivation()
				{
					MeleeCard.EMeleeCardPlayed -= TriggerEffect;
				}
			}
		}
	}

	public class SmokeGrenade : EffectCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Smoke Grenade";
			description = "Gain "+userArmorGain+" armor";
			image = SpriteBase.mainSpriteBase.lightning;

			userArmorGain = 20;
			ammoCost = 1;
			targetType = TargetType.None;
		}
	}
}

public class CQC : MeleeCard
{
	protected override void ExtenderConstructor()
	{
		name = "CQC";
		description = "Targets the toughest enemy";
		image = SpriteBase.mainSpriteBase.arm;

		damage = 40;
		staminaCost = 2;
		targetType = TargetType.Strongest;
	}
}

public class Commander : EncounterEnemy
{
	protected override void CommonConstructor()
	{
		//basicCombatDeck.AddCards(typeof(Reload));
		basicCombatDeck.AddCards(typeof(SemiAuto));
		//basicCombatDeck.AddCards(typeof(Flashbang));

		variationCards.Add(new Maneuvers());
		variationCards.Add(new Leadership());
	}
	
	protected override void NormalConstructor()
	{
		name = "Commander";
		health = 60;
		armor = 40;
		stamina = 4;
		ammo = 3;

		
	}
	protected override void ToughConstructor()
	{
		name = "Tough Commander";
		health = 60;
		armor = 120;
		stamina = 6;
		ammo = 4;

		variationCards.Add(new Tactics());
	}

	public class Maneuvers : EnemyVariationCard
	{
		public Maneuvers()
		{
			name = "Maneuvers";
			image = SpriteBase.mainSpriteBase.armor;

			description = "Add cards to your deck";
			addedCombatCards.Add(new Smokescreen());
		}

		public class Regroup : EffectCard
		{
			protected override void ExtenderConstructor()
			{
				targetType = TargetType.AllFriendlies;
				staminaCost = 4;
				targetArmorGain = 20;

				name = "Regroup";
				description = "The user and all friendly characters gain " + targetArmorGain + " armor";
				image = SpriteBase.mainSpriteBase.cover;

			}
		}
	}

	public class Leadership : EnemyVariationCard
	{
		public Leadership()
		{
			name = "Leadership";
			image = SpriteBase.mainSpriteBase.medal;

			description = "Add cards to your deck";

			addedCombatCards.Add(new Prioritize());
		}

		public class Prioritize : RangedCard
		{
			protected override void ExtenderConstructor()
			{	
				targetType = TargetType.SelectEnemy;
				staminaCost = 4;
				damage = 20;
				addedStipulationCard = new FocusFire();

				name = "Prioritize";
				description = addedStipulationCard.description;
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
					CombatCardTargeter.main.SetNewRangedTargetMerc(appliedToCharacter);
					CombatCardTargeter.ENewRangedTargetMercSet += RemoveCard;
				}
				void RemoveCard()
				{
					appliedToCharacter.RemoveCharacterStipulationCard(this);
				}

				protected override void ExtenderSpecificDeactivation()
				{
					CombatCardTargeter.main.ClearRangedTargetMerc();
					CombatCardTargeter.ENewRangedTargetMercSet -= RemoveCard;
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

			addedCombatCards.Add(new Maneuvers());
		}

		public class Maneuvers : EffectCard
		{
			int damageToArmor = 20;
			
			protected override void ExtenderConstructor()
			{
				targetType = TargetType.AllEnemies;
				staminaCost = 3;

				name = "Maneuvers";
				description = "Remove " + damageToArmor + " armor from all enemies";
				image = SpriteBase.mainSpriteBase.lateralArrows;

			}

			protected override void ApplyEffects()
			{
				foreach (CharacterGraphic graphic in targetChars)
					graphic.IncrementArmor(-damageToArmor);
			}
		}

	}
	//currently unused
	public class Flashbang : RangedCard
	{
		int noCoverStaminaDamageBonus = 1;

		protected override void ExtenderConstructor()
		{
			name = "Flashbang";
			description = "Targets all enemies, characters with 0 armor lose extra " + noCoverStaminaDamageBonus + " stamina";
			image = SpriteBase.mainSpriteBase.lightning;
			targetType = TargetType.AllEnemies;

			staminaCost = 4;
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

public class SemiAuto : RangedCard
{
	protected override void ExtenderConstructor()
	{
		name = "Semi Auto";
		image = SpriteBase.mainSpriteBase.pistol;
		targetType = TargetType.SelectEnemy;
		damage = 20;
		ammoCost = 1;
	}
}