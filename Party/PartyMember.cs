using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface Character
{
	string GetName();
	int GetHealth();
	int GetStartArmor();
	int GetMaxStamina();
	int GetStartAmmo();
	Sprite GetPortrait();
	void SetStamina(int newStamina); //Currently unused
	void SetHealth(int newHealth);
	void IncrementHealth(int delta);
	CombatDeck GetCombatDeck();
}

public interface Mercenary : Character
{
	string GetClass();
	Color GetColor();
}

public class PartyMember: Mercenary
{
	public enum BodyPartTypes {Vitals, Hands, Legs};
	public class BodyParts
	{
		PartyMember assignedMember;
		
		ProbabilityList<MemberBodyPart> partHitChances;
		public float GetPartHitChance(BodyPartTypes type)
		{
			if (partHitChances.probabilities.ContainsKey(currentParts[type])) return partHitChances.probabilities[currentParts[type]];
			else return 0;
		}
		//Dictionary<Parts,int> partsHealth=new Dictionary<Parts, int>();
		public Dictionary<BodyPartTypes,MemberBodyPart> currentParts=new Dictionary<BodyPartTypes,MemberBodyPart>();
		//float totalHitProbability;
		
		//Probability distribution within the overall hitchance range
		const float vitalsHitChance=0.2f;
		const float handsHitChance=0.4f;
		const float legsHitChance=0.4f;
		
		float GetPartTypeHitChance(BodyPartTypes type)
		{
			float hitChance=0;
			switch(type)
			{
				case BodyPartTypes.Hands: {hitChance=handsHitChance; break;}
				case BodyPartTypes.Legs: {hitChance=legsHitChance; break;}
				case BodyPartTypes.Vitals: {hitChance=vitalsHitChance; break;}
			}	
			return hitChance;
		}
		public int GetPartHealth(BodyPartTypes type)
		{
			return currentParts[type].health;
		}
		
		public BodyParts(PartyMember member, int handsMaxHealth, int legsMaxHealth, int vitalsMaxHealth)
		{
			assignedMember=member;
			
			currentParts.Add(BodyPartTypes.Hands,new MemberBodyPart(assignedMember,BodyPartTypes.Hands,handsMaxHealth));
			currentParts.Add(BodyPartTypes.Legs,new MemberBodyPart(assignedMember,BodyPartTypes.Legs,legsMaxHealth));
			currentParts.Add(BodyPartTypes.Vitals,new MemberBodyPart(assignedMember,BodyPartTypes.Vitals,vitalsMaxHealth));
			CalculateHitChances();
		}

		public void ChangePartMaxHealth(int newHealth, BodyPartTypes part)
		{
			int healthChangeDelta=newHealth-currentParts[part].maxHealth;
			currentParts[part].maxHealth=newHealth;
			if (healthChangeDelta>0) currentParts[part].Heal(healthChangeDelta);
			else currentParts[part].DamagePart(healthChangeDelta);
		}

		public ProbabilityList<MemberBodyPart> GetDodgelessHitchances()
		{
			float totalHitProbability=1;

			ProbabilityList<MemberBodyPart> dodgeLessHitChances=new ProbabilityList<MemberBodyPart>();
			
			float missingHitChanceFromBrokenParts=0;
			foreach (MemberBodyPart part in currentParts.Values)
			{
				if (part.broken) missingHitChanceFromBrokenParts+=totalHitProbability*GetPartTypeHitChance(part.partType);
			}
			System.Action<MemberBodyPart> partHitChanceCalculation=(MemberBodyPart part)=>
			{
				if (!part.broken) 
				{
					float adjustedHitChance=totalHitProbability*GetPartTypeHitChance(part.partType);
					if (missingHitChanceFromBrokenParts>0)
					{
						//Increase for body parts that get broken
						float addedHitChance=missingHitChanceFromBrokenParts*(adjustedHitChance/(totalHitProbability-missingHitChanceFromBrokenParts));
						adjustedHitChance+=addedHitChance;
						//missingHitChanceFromBrokenParts-=addedHitChance;
					}
					dodgeLessHitChances.AddProbability(currentParts[part.partType],adjustedHitChance);
				}
			};
			foreach (MemberBodyPart part in currentParts.Values)
			{
				partHitChanceCalculation.Invoke(part);
			}
			return dodgeLessHitChances;
		}

		public void CalculateHitChances()
		{
			float totalHitProbability=1-assignedMember.currentDodgeChance;
			
			partHitChances=new ProbabilityList<MemberBodyPart>();
			
			float missingHitChanceFromBrokenParts=0;
			foreach (MemberBodyPart part in currentParts.Values)
			{
				if (part.broken) missingHitChanceFromBrokenParts+=totalHitProbability*GetPartTypeHitChance(part.partType);
			}
			System.Action<MemberBodyPart> partHitChanceCalculation=(MemberBodyPart part)=>
			{
				if (!part.broken) 
				{
					float adjustedHitChance=totalHitProbability*GetPartTypeHitChance(part.partType);
					if (missingHitChanceFromBrokenParts>0)
					{
						//Increase for body parts that get broken
						float addedHitChance=missingHitChanceFromBrokenParts*(adjustedHitChance/(totalHitProbability-missingHitChanceFromBrokenParts));
						adjustedHitChance+=addedHitChance;
						//missingHitChanceFromBrokenParts-=addedHitChance;
					}
					partHitChances.AddProbability(currentParts[part.partType],adjustedHitChance);
				}
			};
			foreach (MemberBodyPart part in currentParts.Values)
			{
				partHitChanceCalculation.Invoke(part);
			}
			/*
			//For debug purposes only
			GameManager.DebugPrint("New hit probabilities:");
			foreach (MemberBodyPart part in partHitChances.probabilities.Keys)
			{
				GameManager.DebugPrint(part.partType.ToString()+":"+partHitChances.probabilities[part]);
			}*/
		}
		
		public bool TryHit(int damage, out BodyPartTypes damagedPartType)
		{
			float attackRoll=Random.value;
			MemberBodyPart damagedPart;
			damagedPartType=BodyPartTypes.Hands;
			bool success=partHitChances.RollProbability(out damagedPart);
			if (success)
			{
				damagedPartType=damagedPart.partType;
				TakeHit(damage,damagedPart);
			} 
			//else EncounterCanvasHandler.main.AddNewLogMessage("Member dodged!");
			return success;
		}

		public BodyPartTypes HitRandomPart(int damage)
		{
			float attackRoll=Random.value;
			MemberBodyPart damagedPart;
			BodyPartTypes damagedPartType=BodyPartTypes.Hands;
			if (GetDodgelessHitchances().RollProbability(out damagedPart)) 
			{	
				TakeHit(damage,damagedPart);
				damagedPartType=damagedPart.partType;
			}
			else throw new System.Exception("Could not hit member body part at hit p=1!");
			//else EncounterCanvasHandler.main.AddNewLogMessage("Member dodged!");
			return damagedPartType;
		}

		public void TakeHit(int damage, BodyPartTypes damagedPartType)
		{
			TakeHit(damage,currentParts[damagedPartType]);
		}
		
		public void TakeHit(int damage, MemberBodyPart damagedPart)
		{
			damagedPart.DamagePart(-damage);
			//EncounterCanvasHandler.main.AddNewLogMessage(damagedPart.partType.ToString()+" damaged!");
			if (damagedPart.broken)
			{ 
				if (EncounterCanvasHandler.main.encounterOngoing)
				EncounterCanvasHandler.main.AddNewLogMessage(damagedPart.partType.ToString()+" broken!");
				CalculateHitChances();
			}
		}
		
		public bool HealPart(BodyPartTypes healedPart, int healAmount)
		{
			bool healed=currentParts[healedPart].Heal(healAmount);
			if (healed) CalculateHitChances();
			return healed;
		}
		
		//Internal MemberBodyParts class
		public class MemberBodyPart
		{
			public BodyPartTypes partType;
			public int maxHealth;
			public int health;
			public bool broken=false;
			PartyMember member;
			MemberStatusEffect ongoingEffect;
			//public readonly string partName;
			
			public MemberBodyPart(PartyMember partOwner,BodyPartTypes type, int hp)
			{
				partType=type;
				maxHealth=hp;
				health=maxHealth;
				member=partOwner;
			}
			
			void PartDamagedEffect()
			{
				broken=true;
				switch(partType)
				{
				case BodyPartTypes.Hands:
				{
					ongoingEffect=new BrokenArmsMember();
					PartyManager.mainPartyManager.AddPartyMemberStatusEffect(member,ongoingEffect);
					break;
				}
				case BodyPartTypes.Legs:
				{
					ongoingEffect=new BrokenLegsMember();
					PartyManager.mainPartyManager.AddPartyMemberStatusEffect(member,ongoingEffect);
					break;
				}
				case BodyPartTypes.Vitals:
				{
					member.DisposePartyMember();
					break;
				}
				}
			}
			void PartRestoredEffect()
			{
				broken=false;
				switch(partType)
				{
				case BodyPartTypes.Hands:
				{
					BrokenArmsMember effect=ongoingEffect as BrokenArmsMember;
					effect.CureArms();
					ongoingEffect=null;
					break;
				}
				case BodyPartTypes.Legs:
				{
					//PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(member,ongoingEffect);
					BrokenLegsMember effect=ongoingEffect as BrokenLegsMember;
					effect.CureLegs();
					ongoingEffect=null;
					break;
				}
				case BodyPartTypes.Vitals:
				{
					
					break;
				}
				}
			}
			
			public void DamagePart(int healthDelta)
			{
				health+=healthDelta;
				if (health<=0)
				{
					health=0;
					if (!broken) PartDamagedEffect();
				}
			}
			
			public bool Heal(int healAmount)
			{
				bool healed=(health<maxHealth);
				if (healed) 
				{	
					health+=healAmount;
					if (health>=maxHealth)
					{
						health=maxHealth;
						if (broken) PartRestoredEffect();
					}
				}
				return healed;
			}
		} 		
	}
	
	public string name;
	public Color color;
	
	public MapRegion currentRegion;
	
	static List<string> occupiedNames=new List<string>();
	static string GenerateName()
	{
		
		List<string> namesList=new List<string>();
		namesList.Add("Ivan");
		namesList.Add("Misha");
		namesList.Add("Sasha");
		namesList.Add("Vadim");
		namesList.Add("Pyotr");
		namesList.Add("Timur");
		namesList.Add ("Kolya");
		namesList.Add ("Max");
		/*
		foreach (string name in occupiedNames) {namesList.Remove(name);}
		if (namesList.Count==0) {throw new System.Exception("All names are occupied!");}
		
		string finalName=namesList[Random.Range(0,namesList.Count)];
		occupiedNames.Add(finalName);*/
		
		return namesList[Random.Range(0,namesList.Count)];
	}
	
	static List<Color> GetColors() 
	{
		List<Color> possibleColors=new List<Color>(SpriteBase.mainSpriteBase.possibleMemberColors);
		//possibleColors.Add(Color.gray);
		//Blue and gray are reserved, black is too dark
		/*
		possibleColors.Add(Color.red);
		possibleColors.Add(Color.cyan);
		possibleColors.Add(Color.green);
		possibleColors.Add(Color.yellow);
		possibleColors.Add(Color.magenta);*/
		return possibleColors;
	}
	static List<Color> availableColors=null;
	static int lastUsedColorIndex = 0;
	static Color GetPortraitColor()
	{
		if (availableColors == null) availableColors = GetColors();
		int newColorIndex=lastUsedColorIndex+1;
		if (newColorIndex > availableColors.Count - 1) newColorIndex = 0; 
		lastUsedColorIndex=newColorIndex;
		return availableColors[lastUsedColorIndex];
	}

	
		/*
	{
		get {return memberBodyParts.currentParts[BodyPartTypes.Vitals].health;}
	}*/
	//public int _health;
	//public int maxHealth;
	//float healthRegenPercentage=0.2f;
	//Body part health
	//Starting presets
	const int defaultHandsHealth=25;
	const int defaultLegsHealth=25;
	const int defaultVitalsHealth=50;
	//Current values
	public int handsMaxHealth
	{
		get {return memberBodyParts.currentParts[BodyPartTypes.Hands].maxHealth;}//_handsMaxHealth;}
		set 
		{
			//int changeDelta=_handsMaxHealth-value;
			memberBodyParts.ChangePartMaxHealth(value,BodyPartTypes.Hands);
			//_handsMaxHealth=value;
		}
	}
	//int _handsMaxHealth=25;
	public int legsMaxHealth
	{
		get {return memberBodyParts.currentParts[BodyPartTypes.Legs].maxHealth;}
		set 
		{
			//int changeDelta=_handsMaxHealth-value;
			memberBodyParts.ChangePartMaxHealth(value,BodyPartTypes.Legs);
			//_legsMaxHealth=value;
		}
	}
	//int _legsMaxHealth=25;
	public int vitalsMaxHealth
	{
		get {return memberBodyParts.currentParts[BodyPartTypes.Vitals].maxHealth;}
		set 
		{
			//int changeDelta=_handsMaxHealth-value;
			memberBodyParts.ChangePartMaxHealth(value,BodyPartTypes.Vitals);
			//_vitalsMaxHealth=value;
		}
	}
	//int _vitalsMaxHealth=25;
	
	//ATTACK HIT CHANCE
	public float baseAttackHitChance=0f;
	public float meleeHitchanceMod=0;
	public float rangedHitchanceMod=0;
	float hitChanceReductionPerStaminaPoint=0f;//0.025f;//0.05f;
	public float GetCurrentAttackHitChance(bool rangedMode)
	{
		float currentAttackHitChance=baseAttackHitChance;
		if (rangedMode) currentAttackHitChance+=rangedHitchanceMod;
		else currentAttackHitChance+=meleeHitchanceMod;
		int missingStamina=currentMaxStamina-stamina;
		currentAttackHitChance=currentAttackHitChance-missingStamina*hitChanceReductionPerStaminaPoint;
		return currentAttackHitChance;
	}
	
	//STAMINA
	public int stamina;

	public int _stamina; 
	public int baseMaxStamina;
	public int currentMaxStamina
	{
		get {return _currentMaxStamina;}
		set 
		{
			if (value>=4) _currentMaxStamina=value;
			else _currentMaxStamina=4;
		}
	}
	int _currentMaxStamina;
	public void RefreshMaxStamina()
	{
		currentMaxStamina=baseMaxStamina;
		//Currently cancelled
		/*
		currentMaxStamina
		=baseMaxStamina-Mathf.RoundToInt(maxStaminaReductionFromHunger*hunger*0.01f)-Mathf.RoundToInt(maxStaminaReductionFromFatigue*fatigue*0.01f);
		*/
	}
	
	public int staminaRegen=3;
	public int staminaInventoryCost = 2;
	public int staminaMoveCost=2;
	public int staminaBashLootCost = 2;
	public int staminaLootCost = 2;
	public int staminaBarricadeBuildCost = 2;
	public int staminaBarricadeBashCost = 2;
	public int barricadeVaultCost=2;

	int startArmor = 0;

	//HUNGER
	int hunger
	{
		get {return _hunger;}
		set 
		{
			_hunger=value;
			if (_hunger<0) {_hunger=0;}
			//if (_hunger>100) {_hunger=100;}
		}
	}
	int _hunger;
	public int GetHunger() {return hunger;}
	public void SetHunger(int newHunger) 
	{
		hunger=newHunger;
		//RefreshMaxStamina();		
	}
	public void ChangeHunger(int hungerDelta)
	{
		SetHunger(hunger+hungerDelta);
	}
	public int hungerIncreasePerHour;
	//Deprecate this later
	public int maxStaminaReductionFromHunger=0;
	public int maxFatigueRestoreReductionFromHunger=40;
	public float maxHealthRegenReductionFromHunger=0.2f;
	
	//FATIGUE
	int fatigue
	{
		get {return _fatigue;}
		set 
		{
			_fatigue=value;
			if (_fatigue<0) {_fatigue=0;}
			if (_fatigue>maxFatigue) {_fatigue=maxFatigue;}
		}
	}
	int _fatigue;
	public int GetFatigue() {return fatigue;}
	public void SetFatigue(int newFatigue)
	{
		if (newFatigue>maxFatigue)
		{
			int moraleExchangedForFatigue=(newFatigue-maxFatigue)*10;
			newFatigue=maxFatigue;
			morale-=moraleExchangedForFatigue;
		}
		fatigue=newFatigue;
		RefreshMaxStamina();
	}
	public void ChangeFatigue(int fatigueDelta)
	{
		SetFatigue(fatigue+fatigueDelta);
		//else ChangeHunger(fatigueDelta);
	}
	public bool CheckEnoughFatigue(int taskRequirement)
	{
		if (fatigue+taskRequirement>maxFatigue)
		{
			int fatigueOverflow=fatigue+taskRequirement-maxFatigue;
			if (morale-fatigueOverflow*10>=moraleFatigueMinThreshold) return true;
			else return false;
		}
		else return true;
	}
	public int fatigueRestoreWait=6;
	//These are applied on top of fatigueRestoreWait (both are actually applied by two different methods)
	public int fatigueRestoreSleep=10;
	//public int fatigueRestoreSleepInBed=60;
	
	//public int fatigueIncreasePerAction;
	public int maxStaminaReductionFromFatigue=6;
	public const int fatigueIncreasePerEncounter=2;
	//public const int mapMoveFatigueCost=25;
	public const int campSetupFatigueCost=1;
	public const int defaultFatigueMoveCost=2;
	public int currentFatigueMoveModifier=0;
	public int currentFatigueCraftPenalty=0;
	public const int maxFatigue=10;

	
	//MORALE
	public int morale
	{
		get {return _morale;}
		set 
		{
			_morale=value;
			if (_morale>100) {_morale=100;}
			if (_morale<0) {_morale=0;}
		}
	}
	int _morale;
	int baseMorale;
	public int moraleRestorePerHour=10;
	public int moraleDecayPerHour=10;
	public int moraleFatigueMinThreshold=0;
	//per point above/below 50
	public float moraleDamageMod;
	public int moraleChangeFromKills;

	public int aloneMoraleMod=0;
	public int inTeamMoraleMod=0;

	public int hireDaysRemaining = 0;

	public float relationshipChancePerMoralePoint=0.035f;
	public float baseRelationshipChanceModifier=0;
	public float GetCurrentRelationshipMod()
	{
		float currentMod=relationshipChancePerMoralePoint*(morale-50)+baseRelationshipChanceModifier;
		return currentMod;
	}
	//DODGE
	public float currentDodgeChance
	{
		get {return _currentDodgeChance;}
	}
    void RecalculateCurrentDodgeChance()
    {
        _currentDodgeChance = maxDodgeChance * dodgeMultiplier;
        if (memberBodyParts != null) memberBodyParts.CalculateHitChances();
    }

	float _currentDodgeChance;

    float dodgeMultiplier=1;
    public void SetDodgeMultiplier(float newDodgeMultiplier)
    {
        dodgeMultiplier = newDodgeMultiplier;
        RecalculateCurrentDodgeChance();
    }

    public void IncrementMaxDodgeChance(float maxDodgeIncrement) 
    { 
        maxDodgeChance += maxDodgeIncrement;
        RecalculateCurrentDodgeChance();
    }
	float maxDodgeChance=0.5f;
	public BodyParts memberBodyParts;
	
	//public float friendshipChance;
	
	public int skillpoints
	{
		get {return _skillpoints;}
		set 
		{
			if ((_skillpoints==0 && value!=0) || (value<_skillpoints && value>0)) traits.AddRange(Trait.GenerateLevelupSkills(traits,3));
			_skillpoints=value;
		}
	}
	int _skillpoints;

	public float movingEnemySpawnProbIncrease=0.05f;

	public bool hasLight=false;
	public bool hasBedroll=false;
	public bool isCook=false;
	public bool isDowner=false;
	public bool isReassuring=false;
	public bool isLockExpert=false;
	public bool isMedic=false;
	public bool isViolent=false;
	public bool isKleptomaniac=false;
	
	//Scout
	public bool isScout=false;
	public bool extraMoveEnabled=false; 
	//public bool barricadeAvoidanceEnabled=false;
	//Fighter
	public bool hitAndRunEnabled=false;
	
	public int armorValue;
	public int maxCarryCapacity;
	public void ChangeMaxCarryCapacity(int changeDelta)
	{
		maxCarryCapacity+=changeDelta;
		UpdateCurrentCarryCapacity();
	}
	public int currentCarryCapacity;
	public void UpdateCurrentCarryCapacity()
	{
		currentCarryCapacity=maxCarryCapacity;
		if (equippedMeleeWeapon!=null) currentCarryCapacity-=equippedMeleeWeapon.GetWeight();
		if (equippedRangedWeapon!=null) currentCarryCapacity-=equippedRangedWeapon.GetWeight();
		while (carriedItems.Count>Mathf.Max(currentCarryCapacity,0))
		{
			DropItem(carriedItems[0]);
		}
	}
	public int visibilityMod;
	
	public RangedWeapon equippedRangedWeapon;
	public int rangedDamageMod;
	public MeleeWeapon equippedMeleeWeapon;
	//int minUnarmedDamage=0;
	//int maxUnarmedDamage=4;
	int unarmedDamage=30;
	int unarmedAttackStaminaUse=1;
	public int meleeDamageMod;
	public List<InventoryItem> equippedItems=new List<InventoryItem>();
	public List<InventoryItem> carriedItems=new List<InventoryItem>();
	public void RemoveCarriedItem(InventoryItem item)
	{
		if (!carriedItems.Contains(item)) throw new System.Exception("No carried item!");
		else carriedItems.Remove(item);
	}
	
	public List<MemberStatusEffect> activeStatusEffects=new List<MemberStatusEffect>();
	public List<Trait> traits=new List<Trait>();
	public void ActivateSkill(Skill activatedSkill)
	{
		if (!traits.Contains(activatedSkill)) throw new System.Exception("Learned trait not found in member!");
		else
		{
			activatedSkill.learned=true;
			activatedSkill.ActivateEffect(this);
			foreach (Trait memberTrait in new List<Trait>(traits))
			{
				if (memberTrait.GetType().BaseType==typeof(Skill))
				{
					Skill memberSkill=memberTrait as Skill;
					if (!memberSkill.learned) traits.Remove(memberTrait);
				}
			} 
		}
	}

	public Dictionary<PartyMember,Relationship> relationships=new Dictionary<PartyMember, Relationship>();
	
	//NEW STUFF!!!
	public enum MercClass {Soldier,Bandit,Gunman,Mercenary};
	public MercClass myClass;
	CombatDeck combatDeck = new CombatDeck();

	int ammo = 10;
	int healthMax = 100;
	int health;

	void SetClassStats(MercClass mercClass)
	{
		if (mercClass == MercClass.Soldier)
		{
			healthMax = 60;
			startArmor = 40;
			stamina = 5;
			ammo = 2;
		}
		if (mercClass == MercClass.Bandit)
		{
			healthMax = 60;
			startArmor = 40;
			stamina = 8;
			ammo = 0;
		}
		if (mercClass == MercClass.Gunman)
		{
			healthMax = 60;
			startArmor = 40;
			stamina = 2;
			ammo = 4;
		}
		if (mercClass == MercClass.Mercenary)
		{
			healthMax = 60;
			startArmor = 70;
			stamina = 4;
			ammo = 2;
		}
	}

	static CombatDeck GetClassDeck(MercClass mercClass)
	{
		CombatDeck result = new CombatDeck();

		if (mercClass == MercClass.Soldier)
		{
			result = SoldierCards.GetClassCards();
		}
		if (mercClass == MercClass.Bandit)
		{
			result = BanditCards.GetClassCards();
		}
		if (mercClass == MercClass.Gunman)
		{
			result = GunmanCards.GetClassCards();
		}
		if (mercClass==MercClass.Mercenary)
		{
			result = MercenaryCards.GetClassCards();
		}
		//result = new CombatDeck();
		//result.AddCards(typeof(SemperFi),4);

		return result;
	}

	public PartyMember ()//Vector2 startingWorldCoords)
	{
		GeneratePartyMember(GenerateName());
	}
	
	public PartyMember(string memberName)//, params Perk[] assignedPerks)
	{
		GeneratePartyMember(memberName);
	}

	void GeneratePartyMember(string memberName)
	{
		MapRegion startingRegion = MapManager.main.GetTown();
		
		var classtypes=System.Enum.GetValues(typeof(MercClass));
		myClass = (MercClass)classtypes.GetValue(Random.Range(0,classtypes.Length));
		combatDeck = GetClassDeck(myClass);
		SetClassStats(myClass);

		//Pick color out of ones left
		//worldCoords=startingCoords;
		currentRegion=startingRegion;
		startingRegion.localPartyMembers.Add(this);

		color = GetPortraitColor();
		
		
		//Randomly pick out a specialty
		List<Trait> possibleSpecialtyPerks=Trait.GenerateRandomSkillTree(5);

		//Pick out a starting specialty perk
		Skill startingLearnedSkill=possibleSpecialtyPerks[Random.Range(0,possibleSpecialtyPerks.Count)] as Skill;
		startingLearnedSkill.learned=true;
		traits.Add(startingLearnedSkill);
		
		//Fill out trait list
		List<Trait> possibleGenericPerks=Trait.GetTraitList();
		//Deactivate the opposite traits of the starting perk
		foreach(Trait genericPerk in possibleGenericPerks)
		{
			if (genericPerk.GetType()==startingLearnedSkill.oppositePerk) 
			{
				if (possibleGenericPerks.Contains(genericPerk)) possibleGenericPerks.Remove(genericPerk); 
				break;
			}
		}
		
		/*
		int requiredSpecialtyPerks=1;
		while (requiredSpecialtyPerks>0)
		{
			requiredSpecialtyPerks--;
			Perk newPerk=possibleSpecialtyPerks[Random.Range(0,possibleSpecialtyPerks.Count)];
			perks.Add(newPerk);
			possibleSpecialtyPerks.Remove(newPerk);
			if (newPerk.oppositePerk!=null) 
			{
				foreach (Perk possiblePerk in possibleSpecialtyPerks) 
				{
					if (possiblePerk.GetType()==newPerk.oppositePerk) {possibleSpecialtyPerks.Remove(possiblePerk); break;}
				}
				foreach (Perk possiblePerk in possibleGenericPerks) 
				{
					if (possiblePerk.GetType()==newPerk.oppositePerk) {possibleGenericPerks.Remove(possiblePerk); break;}
				}
			}
		}*/
		
		//Randomly pick out generic perks
		int necessaryPerkCount=2;
		int addedPerksCount=0;
		while (addedPerksCount<necessaryPerkCount && possibleGenericPerks.Count>0)
		{
			Trait newPerk=possibleGenericPerks[Random.Range(0,possibleGenericPerks.Count)];
			traits.Add(newPerk);
			addedPerksCount++;
			possibleGenericPerks.Remove(newPerk);
			if (newPerk.oppositePerk!=null) 
			{
				foreach (Trait possibleGenericPerk in possibleGenericPerks) 
				{
					if (possibleGenericPerk.GetType()==newPerk.oppositePerk) 
					{
						if (possibleGenericPerks.Contains(possibleGenericPerk)) possibleGenericPerks.Remove(possibleGenericPerk); 
						break;
					}
				}
				foreach (Trait possibleSpecialtyPerk in possibleSpecialtyPerks) 
				{
					if (possibleSpecialtyPerk.GetType()==newPerk.oppositePerk) 
					{
						if (possibleSpecialtyPerks.Contains(possibleSpecialtyPerk)) possibleSpecialtyPerks.Remove(possibleSpecialtyPerk); 
						break;
					}
				}
			}
		}
		//Add the rest of the skills that can be learned
		//traits.AddRange(possibleSpecialtyPerks);
		//debug
		//if (memberName=="Jimbo") {perks.Clear(); perks.Add(new WeakArm());}
		
		///////
		name=memberName;
		_currentDodgeChance=maxDodgeChance;
		memberBodyParts=new BodyParts(this, defaultHandsHealth,defaultLegsHealth,defaultVitalsHealth);

		baseMaxStamina=10;
		baseMorale=50;
		//moraleChangePerHour=10;
		
		hunger=0;
		hungerIncreasePerHour=50;
		
		fatigue=0;
		//fatigueIncreasePerAction=10;



		armorValue=0;
		maxCarryCapacity=4;//
		visibilityMod=0;
		moraleDamageMod=0;//0.02f;
		moraleChangeFromKills=0;
		//friendshipChance=0.6f;
		rangedDamageMod=0;
		meleeDamageMod=0;
		hasLight=false;
		isCook=false;
		isMedic=false;
		isLockExpert=false;
		isScout=false;
		foreach (Trait myPerk in traits)
		{
			if (myPerk.GetType().BaseType==typeof(Trait)) myPerk.ActivateEffect(this);
			else
			{
				Skill mySkill=myPerk as Skill;
				if (mySkill.learned) mySkill.ActivateEffect(this);
			}
		}
		//make sure perks trigger before these to properly use modified values of maxHealth and maxStamina
		health = healthMax;
		currentMaxStamina=baseMaxStamina;
		//stamina=currentMaxStamina;
		morale=baseMorale;
		UpdateCurrentCarryCapacity();
		//maxHealth=vitalsMaxHealth;

		//currentCarryCapacity=maxCarryCapacity;
		
		//EquipWeapon(new Pipe());
		//equippedMeleeWeapon=new Pipe();
		equippedRangedWeapon=null;//new AssaultRifle();//null;
		
	}
	
	public bool TryAddOrStackStatusEffect(MemberStatusEffect newEffect)
	{
		//bool newEffectAdded=true;
		foreach (MemberStatusEffect activeEffect in activeStatusEffects)
		{
			if(activeEffect.GetType()==newEffect.GetType())
			{
				//newEffectAdded=false;
				if (activeEffect.canStack)
				{	
					activeEffect.StackEffect();
					newEffect.CleanupEffect();
				}
				return false;
			}
		}
		newEffect.ActivateEffect(this);
		activeStatusEffects.Add(newEffect);
		return true;
	}
	
	void DisposePartyMember()
	{
		//GameManager.DebugPrint("member disposed!");
		//Drop carried items
		//Pool together all items
		List<InventoryItem> allItems=new List<InventoryItem>();
		foreach (InventoryItem item in carriedItems) {allItems.Add(item);}
		foreach (InventoryItem item in equippedItems) {allItems.Add(item);}
		//May want to clear the lists after this
		if (equippedRangedWeapon!=null) 
			allItems.Add(equippedRangedWeapon);
		if (equippedMeleeWeapon!=null) 
			allItems.Add(equippedMeleeWeapon); 
		//Drop in encounter
		if (EncounterCanvasHandler.main.encounterOngoing)
		{
			if (EncounterCanvasHandler.main.encounterMembers.Contains(this))
			{
				RoomButtonHandler currentRoom=EncounterCanvasHandler.main.roomButtons[EncounterCanvasHandler.main.memberCoords[this]];
				foreach (InventoryItem item in allItems) {currentRoom.DropItemOnFloor(item);}
			}
			else foreach (InventoryItem item in allItems) {currentRegion.StashItem(item);}
		}
		else
		{
			//Drop in map
			foreach (InventoryItem item in allItems) {currentRegion.StashItem(item);}
		}
		
		occupiedNames.Remove(name);
		//PartyManager.ETimePassed-=TimePassEffect;
		//PartyManager.ETimePassedEnd-=LateTimePassEffect;
		PartyManager.mainPartyManager.RemovePartyMember(this,true);
		foreach (MemberStatusEffect effect in activeStatusEffects) {effect.CleanupEffect();}
		activeStatusEffects=null;
		PartyStatusCanvasHandler.main.NewNotification(name+" has died!");
	}
	//Currently deprecated
	/*
	void RollRelationships()
	{

		if (Random.value<0f)
		{
			PartyMember newRelationGuy=PartyManager.mainPartyManager.partyMembers[Random.Range(0,PartyManager.mainPartyManager.partyMembers.Count)];
			if (newRelationGuy!=this & !relationships.ContainsKey(newRelationGuy)) 
			{
				if (Random.value<friendshipChance) 
				{
					SetRelationship(newRelationGuy,Relationship.RelationTypes.Friend);
					//newRelationGuy.SetRelationship(this,Relationship.RelationTypes.Friend);
				}
				else 
				{
					SetRelationship(newRelationGuy,Relationship.RelationTypes.Enemy);
					//newRelationGuy.SetRelationship(this,Relationship.RelationTypes.Enemy);
				}
			}
		}

	}*/

	public void SetRelationship(PartyMember member, Relationship.RelationTypes type)
	{
		relationships.Add(member,new Relationship(member,type));
		if (member.relationships.ContainsKey(this)) member.relationships.Remove(this);
		member.relationships.Add(this,new Relationship(this,type));
		/*
		string typeDesc="";
		if (type==Relationship.RelationTypes.Friend){ typeDesc="friends";}
		else {typeDesc="enemies";}
		PartyStatusCanvasHandler.main.NewNotification(name+" became "+typeDesc+" with "+member.name);
		*/
	}
	public void RemoveRelatonship(PartyMember removedMember) {relationships.Remove(removedMember);}
	
	public void TimePassEffect()
	{
		/*
		float maxStamReductionPerHungerPoint=0.1f;
		int hungerIncreasePerStamPoint=10;
		
		int newStaminaValue=maxStamina;//-(int)(hunger*maxStamReductionPerHungerPoint);
		//Restore stamina according to current hunger
		stamina=newStaminaValue;*/
		//Increase hunger for restoring stamina
		/*
		if (newStaminaValue-stamina>0) 
		{hunger+=((newStaminaValue-stamina)*hungerIncreasePerStamPoint)*hoursPassed;}*/
		
		//REGEN/LOSE HEALTH
		IncrementHealth(TownManager.main.GetDailyMercHealRate());
		
		//DO HUNGER INCREASE
		/*
		float cookMult=1;
		foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
		{
			if (member.isCook) {cookMult=Cook.hungerIncreaseMult; break;}
		}	
		ChangeHunger((int)(hungerIncreasePerHour*cookMult));
		//Calculate OverHunger
		int hungerOverload=Mathf.FloorToInt((hunger-100)*0.1f);
		//Reset hunger from over 100
		if (hunger>100) SetHunger(100);

		//Calculate morale decay and restore values
		int adjustedDecayValue=moraleDecayPerHour;
		int adjustedRestoreValue=moraleRestorePerHour;
		bool reassuringMemberFound=false;
		bool downerMemberFound=false;
		//Adjust values based on whether or not a Downer and a Reassuring Presence members are found
		foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
		{
			if (!reassuringMemberFound) 
			{
				if (member.isReassuring)
				{
					reassuringMemberFound=true;
					adjustedRestoreValue=Mathf.RoundToInt(adjustedRestoreValue*ReassuringPresence.moraleRestoreMult);
				}
			}
			if (!downerMemberFound) 
			{
				if (member.isDowner)
				{
					downerMemberFound=true;
					adjustedDecayValue=Mathf.RoundToInt(adjustedDecayValue*Downer.moraleDecayMult);
				}
			} 
		}

		//If hunger reached >100
		if (hungerOverload>0)
		{
			TakeDamage(5,false,BodyPartTypes.Hands);
			TakeDamage(5,false,BodyPartTypes.Legs);
			TakeDamage(5,false,BodyPartTypes.Vitals);	

			//DO MORALE
			//if party is starving, morale drops
			morale-=adjustedDecayValue;
		}
		else
		{
			if (morale!=baseMorale)
			{
				//find morale decay direction
				int decaySign=(int)Mathf.Sign(baseMorale-morale);
				int moraleChange;//=moraleChangePerHour*decaySign*hoursPassed;
				//if morale is increasing (from <base)
				if (decaySign>0)
				{
					moraleChange=adjustedRestoreValue*decaySign;
					if (morale+moraleChange>baseMorale) {morale=baseMorale;}
					else {morale+=moraleChange;}
				}
				//if morale is lowering (from >base)
				if (decaySign<0)
				{
					moraleChange=adjustedDecayValue*decaySign;
					if (morale+moraleChange<baseMorale) {morale=baseMorale;}
					else {morale+=moraleChange;}
				}
			}
			//hunger+=(int)(hungerIncreasePerHour*cookMult)*hoursPassed;
		}*/
		//Resting fatigue restore
		int fatigueRestoreAmount = fatigueRestoreWait;
		if (currentRegion.hasCamp || hasBedroll) 
			fatigueRestoreAmount = fatigueRestoreSleep;
		ChangeFatigue(-fatigueRestoreAmount);

		hireDaysRemaining--;
		if (hireDaysRemaining < 1) 
			PartyManager.mainPartyManager.RemovePartyMember(this,false);
		//DO RELATIONSHIPS
		//RollRelationships();
	}
    //Currently unused, might want to use later
	public void LateTimePassEffect()
	{
		
	}
	/*
	public AssignedTask GetRestTask(bool restInBed)
	{
		AssignedTaskTypes taskType=AssignedTaskTypes.Rest;
		if (restInBed) 
		{
			//taskType=AssignedTaskTypes.RestInBed;
			return new AssignedTask(this,taskType
			,()=>
			{
				if (this.fatigue>0 || this.health<this.maxHealth
				|| this.memberBodyParts.GetPartHealth(BodyPartTypes.Hands)<handsMaxHealth
				|| this.memberBodyParts.GetPartHealth(BodyPartTypes.Legs)<legsMaxHealth) return true;
				else return false;
				
			}
			,()=>{RestEffect(restInBed);}
			,()=>{currentRegion.campInRegion.freeBeds-=1;}
			,()=>{currentRegion.campInRegion.freeBeds+=1;}
			);
		}
		else
		{
			return new AssignedTask(this,taskType
			,()=>
			{
				if (this.fatigue>0 || this.health<this.maxHealth
				|| this.memberBodyParts.GetPartHealth(BodyPartTypes.Hands)<handsMaxHealth
				|| this.memberBodyParts.GetPartHealth(BodyPartTypes.Legs)<legsMaxHealth) return true;
				else return false;
			}
			,()=>{RestEffect(restInBed);}
			);
		}
	}
	
	public bool CanRest()
	{
		if (//!MapManager.main.memberTokens[this].moved &&
		(this.fatigue>0 || this.health<this.maxHealth
		|| this.memberBodyParts.GetPartHealth(BodyPartTypes.Hands)<handsMaxHealth
		|| this.memberBodyParts.GetPartHealth(BodyPartTypes.Legs)<legsMaxHealth)) return true;
		else return false;
	}
	
	void RestEffect(bool hasBed)
	{	
		//int bedModifier=20;
		int totalRest=fatigueRestoreSleep;//80-fatigueRestoreWait;
		if (hasBed) totalRest=fatigueRestoreSleepInBed;//+=bedModifier;
		int newFatigue=_fatigue-totalRest;//Mathf.RoundToInt(maxFatigueRestoreReductionFromHunger*hunger*0.01f);
		SetFatigue(newFatigue);
		//Regen is currently disabled
		
		if (hunger<100)
		{
			float healthRegen=5f;//healthRegenPercentage;//-maxHealthRegenReductionFromHunger*hunger*0.01f;
			memberBodyParts.currentParts[BodyPartTypes.Vitals].Heal(Mathf.RoundToInt(healthRegen));//memberBodyParts.currentParts[BodyPartTypes.Vitals].health*healthRegen));
			memberBodyParts.currentParts[BodyPartTypes.Hands].Heal(Mathf.RoundToInt(healthRegen));//memberBodyParts.currentParts[BodyPartTypes.Hands].health*healthRegen));
			memberBodyParts.currentParts[BodyPartTypes.Legs].Heal(Mathf.RoundToInt(healthRegen));//memberBodyParts.currentParts[BodyPartTypes.Legs].health*healthRegen));
			//health+=Mathf.RoundToInt(health*healthRegen);
		}
	}*/
	
	public void EncounterStartTrigger(List<PartyMember> team)
	{
		if (team.Count==1) {morale+=aloneMoraleMod;}
		else 
		{
			morale+=inTeamMoraleMod;
			foreach (PartyMember member in team) 
			{
				if (relationships.ContainsKey(member)) morale+=relationships[member].OnMissionTogether();
			}
		}
		ChangeFatigue(fatigueIncreasePerEncounter);
	}
	
	public void EncounterEndTrigger()
	{
		//ChangeFatigue(fatigueIncreasePerEncounter);
	}
	
	public bool Heal(int amountHealed)
	{
		//GameManager.DebugPrint("performing heal");
		bool healed=false;
		//if (health<maxHealth)
		{
			//if member is in encounter
			if (EncounterCanvasHandler.main.encounterOngoing)
			{
				foreach (PartyMember member in EncounterCanvasHandler.main.encounterMembers)
				{
					if (member.isMedic) {amountHealed=Mathf.RoundToInt(amountHealed*Medic.healMultiplier); break;}
				}	
			}
			else
			{
				//if healing is done outside of encounter
				foreach (PartyMember member in InventoryScreenHandler.mainISHandler.selectedMember.currentRegion.localPartyMembers)
				{
					if (member.isMedic) { amountHealed = Mathf.RoundToInt(amountHealed*Medic.healMultiplier); break; }
				}	
			}
			//health+=amountHealed;
			bool handsHealed=memberBodyParts.HealPart(BodyPartTypes.Hands,amountHealed);
			bool legsHealed=memberBodyParts.HealPart(BodyPartTypes.Legs,amountHealed);
			bool vitalsHealed=memberBodyParts.HealPart(BodyPartTypes.Vitals,amountHealed);
			if (handsHealed || legsHealed || vitalsHealed) healed=true;
		}
		return healed;
	}
	//for debug purposes
	/*
	public void GetMeleeAttackDamage()
	{
		int damage=0;
		if (equippedMeleeWeapon!=null)
		{
			//if (stamina<equippedMeleeWeapon.GetStaminaUse()) {EncounterCanvasHandler.main.DisplayNewMessage(name+"'s attack is weak!");}
			//if (stamina>0) 
			//{
			//stamina-=equippedMeleeWeapon.GetStaminaUse();
			damage=equippedMeleeWeapon.GetDamageRoll((morale-baseMorale)*moraleDamageMod);
			//GameManager.DebugPrint("Modifier is:(morale "+morale+"- baseMorale "+baseMorale+")*moraleDamageMod "+moraleDamageMod);
			//GameManager.DebugPrint("Modifier="+(morale-baseMorale)*moraleDamageMod);
			GameManager.DebugPrint("Final damage="+damage);
		}
		else
		{
			//int minUnarmedDamage=0;
			//int maxUnarmedDamage=4;
			int unarmedAttackStaminaUse=1;
			
			if (stamina<unarmedAttackStaminaUse) 
			{
				//EncounterCanvasHandler.main.DisplayNewMessage(name+"'s attack is weak!");
				stamina-=unarmedAttackStaminaUse;
				damage=minUnarmedDamage;
			}
			else {damage=Random.Range(minUnarmedDamage,maxUnarmedDamage+1);}
		}
	}*/
	
	public string GetMeleeDamageString()
	{
		string res="";
		if (equippedMeleeWeapon!=null) 
		{res=(equippedMeleeWeapon.baseDamage+meleeDamageMod).ToString();}//(equippedMeleeWeapon.GetMinDamage()+meleeDamageMod)+"-"+(equippedMeleeWeapon.GetMaxDamage()+meleeDamageMod);}
		else {res=(unarmedDamage+meleeDamageMod).ToString();}//(Mathf.Max(minUnarmedDamage+meleeDamageMod,0))+"-"+(maxUnarmedDamage+meleeDamageMod);}
		return res;
	}
	
	public string GetMeleeAttackDescription()
	{
		string res="Hit for ";
		int requiredStamina=unarmedAttackStaminaUse;
		int baseDamage=unarmedDamage;
		if (equippedMeleeWeapon!=null) 
		{
			requiredStamina=equippedMeleeWeapon.GetStaminaUse();
			//float staminaMod=Mathf.Min(1f,(float)stamina/(float)requiredStamina);
			baseDamage=equippedMeleeWeapon.baseDamage;
			/*(Mathf.RoundToInt((equippedMeleeWeapon.GetMinDamage()+meleeDamageMod)*staminaMod))
			+"-"+(Mathf.RoundToInt((equippedMeleeWeapon.GetMaxDamage()+meleeDamageMod)*staminaMod));*/
		}
		if (stamina<requiredStamina) baseDamage=Mathf.CeilToInt(baseDamage*0.5f);
		res+=(baseDamage+meleeDamageMod).ToString();
		res+=" damage\n("+requiredStamina+" stamina)";
		return res;
	}
	
	public int MeleeAttack()
	{
		int damage=unarmedDamage;
		int requiredStamina=unarmedAttackStaminaUse;
		if (equippedMeleeWeapon!=null)
		{
		//if (stamina<equippedMeleeWeapon.GetStaminaUse()) {EncounterCanvasHandler.main.DisplayNewMessage(name+"'s attack is weak!");}
		//if (stamina>0) 
		//{
			damage=equippedMeleeWeapon.GetDamage((morale-baseMorale)*moraleDamageMod,0);
			requiredStamina=equippedMeleeWeapon.GetStaminaUse();
		}
		if (stamina<requiredStamina) damage=Mathf.CeilToInt(damage*0.5f);
		stamina-=requiredStamina;
		damage+=meleeDamageMod;
		return damage;
	}
	
	public int RangedAttack()
	{
		int damage=equippedRangedWeapon.GetDamage((morale-baseMorale)*moraleDamageMod);
		PartyManager.mainPartyManager.ammo-=equippedRangedWeapon.GetAmmoUsePerShot();
		return damage+rangedDamageMod;
	}
	
	public string GetRangedAttackDescription()
	{
		string res="Shoot for";
		if (equippedRangedWeapon!=null) 
		{
			//int roundedMod=Mathf.FloorToInt((morale-baseMorale)*moraleDamageMod)+rangedDamageMod;
			res+=(equippedRangedWeapon.baseDamage+rangedDamageMod).ToString();//(equippedRangedWeapon.GetMinDamage()+rangedDamageMod)+"-"+(equippedRangedWeapon.GetMaxDamage()+rangedDamageMod);
		}
		else {res+="no ranged weapon!";}
		return res;
	}
	
	public string GetRangedDamage()
	{
		string res="";
		if (equippedRangedWeapon!=null) 
		{
			//int roundedMod=Mathf.FloorToInt((morale-baseMorale)*moraleDamageMod)+rangedDamageMod;
			res=(equippedRangedWeapon.baseDamage+rangedDamageMod).ToString();//(equippedRangedWeapon.GetMinDamage()+rangedDamageMod)+"-"+(equippedRangedWeapon.GetMaxDamage()+rangedDamageMod);
		}
		else {res="0";}
		return res;
	}
	
	public void ReactToKill() {morale+=moraleChangeFromKills;}
	
	public bool Eat(int amountFed)
	{
		bool ate=false;
		if (hunger>0)
		{
			ChangeHunger(-amountFed);
			ate=true;
		}
		return ate;
	}
	public bool TryTakeDamage(int dmgDealt, ref int actualDmg, out BodyPartTypes damagedPart)
	{
		return TryTakeDamage(dmgDealt,true,ref actualDmg, out damagedPart);
	}
	public bool TryTakeDamage(int dmgDealt, bool armorHelps, ref int actualDmg, out BodyPartTypes damagedPart)
	{
		actualDmg=0;
		damagedPart=BodyPartTypes.Hands;
		bool tookDamage=false;
		if (armorHelps) dmgDealt=Mathf.Max(0,dmgDealt-armorValue);
		if (dmgDealt>0)
		{
			tookDamage=memberBodyParts.TryHit(dmgDealt, out damagedPart);
			if (tookDamage) actualDmg=dmgDealt;//TakeDamage(dmgDealt,true);
		}
		return tookDamage;
	}
	//For unavoidable damage, such as hunger damage or special events
	public BodyPartTypes TakeRandomPartDamage(int dmgTaken, bool armorHelps)
	{
		if (armorHelps) dmgTaken-=armorValue;
		if (dmgTaken<0) dmgTaken=0; 
		return memberBodyParts.HitRandomPart(dmgTaken);
	}
	

	public void TakeDamage(int dmgTaken, bool armorHelps)
	{
		TakeDamage(dmgTaken,armorHelps,BodyPartTypes.Vitals);
	}
	public void TakeDamage(int dmgTaken, bool armorHelps, BodyPartTypes bodyPart)
	{
		if (armorHelps) dmgTaken-=armorValue;
		if (dmgTaken>0) memberBodyParts.TakeHit(dmgTaken,bodyPart);
	}
	
	
	/*
	public int TakeDamage(int dmgTaken)
	{
		return TakeDamage(dmgTaken,0,true);
	}
	public int TakeDamage(int dmgTaken, bool armorHelps) {return TakeDamage(dmgTaken,0,armorHelps);}
	public int TakeDamage(int dmgTaken, int defenseMod, bool armorHelps)
	{
		//check for bleed chaseups and other post member delete nonsense
		int realDmg=dmgTaken-defenseMod;
		if (armorHelps) 
		{
			realDmg-=armorValue;
			if (realDmg<0) realDmg=0;
		}
		if (health>0)
		{
			health-=realDmg;
			if (health<=0) {DisposePartyMember();}
		}
		return realDmg;
	}*/
	
	public bool CanPickUpItem()//InventoryItem pickedUpItem)
	{
		bool pickedUp=false;
		if (carriedItems.Count<currentCarryCapacity)
		{
			//carriedItems.Add(pickedUpItem);
			pickedUp=true;
		}
		return pickedUp;
	}
	
	public void DropItem(InventoryItem item)
	{
		if (carriedItems.Contains(item))
		{
			carriedItems.Remove(item);
			
			EncounterCanvasHandler encounterHdlr=EncounterCanvasHandler.main;
			if (encounterHdlr.encounterOngoing)
			{
				if (encounterHdlr.encounterMembers.Contains(this)) 
				{
					encounterHdlr.roomButtons[encounterHdlr.memberCoords[this]].DropItemOnFloor(item);	
				} else {throw new System.Exception("Dropping items as a non-member of encounter!");}
			}
			else currentRegion.StashItem(item);
			//{PartyManager.mainPartyManager.GainItems(item);}
			
		} else {throw new System.Exception("Dropped item does not exist in member's carriedItems!");}
	}
	
	public bool CanEquipItem(EquippableItem equippedItem)
	{
		bool allowEquip=true;
		foreach (EquippableItem item in equippedItems)
		{
			if (item.GetType()==equippedItem.GetType()) {allowEquip=false; break;}
		}
		return allowEquip;
	}
	
	public void EquipItem(EquippableItem equippedItem)
	{
		if (CanEquipItem(equippedItem))
		{
			equippedItem.EquipEffect(this);
			equippedItems.Add(equippedItem);
			combatDeck.AddCards(equippedItem.addedCombatCards.ToArray());
		}
		else {throw new System.Exception("Trying to equip an item that's already equipped twice");}
	}
	
	public void UnequipItem(EquippableItem unequippedItem)
	{
		if (equippedItems.Contains(unequippedItem)) 
		{
			unequippedItem.UnequipEffect(this);
			equippedItems.Remove(unequippedItem);
			combatDeck.RemoveCards(unequippedItem.addedCombatCards.ToArray());
		}
	}
	
	public void EquipWeapon(Weapon newWeapon)
	{
		if (newWeapon.GetType().BaseType == typeof(MeleeWeapon))
			equippedMeleeWeapon = newWeapon as MeleeWeapon;
		if (newWeapon.GetType().BaseType == typeof(RangedWeapon))
			equippedRangedWeapon = newWeapon as RangedWeapon;
		
		combatDeck.AddCards(newWeapon.addedCombatCards.ToArray());
		
		stamina += newWeapon.staminaBonus;
		ammo += newWeapon.ammoBonus;

		UpdateCurrentCarryCapacity();
		//currentCarryCapacity=currentCarryCapacity-newWeapon.GetWeight();//Mathf.Max(0,currentCarryCapacity-newWeapon.GetWeight());
		
	}
	
	public void UnequipWeapon(Weapon uneqippedWeapon)
	{
		if (equippedMeleeWeapon == uneqippedWeapon)
			equippedMeleeWeapon = null;
		if (equippedRangedWeapon == uneqippedWeapon)
			equippedRangedWeapon = null;
		
		combatDeck.RemoveCards(uneqippedWeapon.addedCombatCards.ToArray());

		stamina -= uneqippedWeapon.staminaBonus;
		ammo -= uneqippedWeapon.ammoBonus;

		UpdateCurrentCarryCapacity();
		//currentCarryCapacity=Mathf.Min(maxCarryCapacity,currentCarryCapacity+uneqippedWeapon.GetWeight());
	}

	public string GetName()
	{
		return name;
	}

	public int GetHealth()
	{
		return health;
	}

	//Currently used one
	/*
	public void TakeDamage(int dmgTaken) 
	{
		IncrementHealth(-dmgTaken); 
	}*/

	public void IncrementHealth(int delta)
	{
		SetHealth(health + delta);
	}

	public void SetHealth(int newHealth)
	{
		health = newHealth;
		if (health > healthMax) 
			health = healthMax;
		if (health <= 0) 
			DisposePartyMember();
	}

	public int GetStartArmor()
	{
		return startArmor;
	}

	public int GetMaxStamina()
	{
		return stamina;
	}

	public void SetStamina(int newStamina)
	{
		stamina = newStamina;
	}

	public Sprite GetPortrait()
	{
		return SpriteBase.mainSpriteBase.mercPortrait;
	}

	public string GetClass()
	{
		return myClass.ToString();
	}

	public Color GetColor()
	{
		return color;
	}

	public CombatDeck GetCombatDeck() { return combatDeck; }
	public List<CombatCard> GetAllUsedCards() { return combatDeck.GetDeckCards();}

	public int GetStartAmmo() 
	{ 
		return ammo; 
	}
}
