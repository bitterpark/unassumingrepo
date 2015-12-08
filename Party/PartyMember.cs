using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartyMember 
{
	public string name;
	public Color color;
	
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
		foreach (string name in occupiedNames) {namesList.Remove(name);}
		if (namesList.Count==0) {throw new System.Exception("All names are occupied!");}
		
		string finalName=namesList[Random.Range(0,namesList.Count)];
		occupiedNames.Add(finalName);
		
		return finalName;
	}
	
	static List<Color> GetColors() 
	{
		List<Color> possibleColors=new List<Color>();
		//possibleColors.Add(Color.gray);
		//Blue and gray are reserved, black is too dark
		possibleColors.Add(Color.red);
		possibleColors.Add(Color.cyan);
		possibleColors.Add(Color.green);
		possibleColors.Add(Color.yellow);
		possibleColors.Add(Color.magenta);
		return possibleColors;
	}
	
	public int health
	{
		get {return _health;}
		set 
		{
			if (value<=0) {_health=0;}//StartCoroutine(DoGameOver());}
			else 
			{
				_health=value;
				if (_health>maxHealth) {_health=maxHealth;}
			}
			//if (_health<=0) {GameManager.}
		}
	}
	public int _health;
	public int maxHealth=100;
	
	//STAMINA
	public int stamina
	{
		get {return _stamina;}
		set 
		{
			_stamina=value;
			if (_stamina<0) {_stamina=0;}
			if (_stamina>currentMaxStamina) {_stamina=currentMaxStamina;}
			
		}
	}
	public int _stamina; 
	public int baseMaxStamina;
	int currentMaxStamina
	{
		get {return _currentMaxStamina;}
		set 
		{
			if (value>=4) _currentMaxStamina=value;
			else _currentMaxStamina=4;
		}
	}
	int _currentMaxStamina;
	void RefreshMaxStamina()
	{
		currentMaxStamina
		=baseMaxStamina-Mathf.RoundToInt(maxStaminaReductionFromHunger*hunger*0.01f)-Mathf.RoundToInt(maxStaminaReductionFromFatigue*fatigue*0.01f);
		stamina=currentMaxStamina;
	}
	
	public int staminaRegen=2;
	public int staminaMoveCost=2;
	
	//HUNGER
	int hunger
	{
		get {return _hunger;}
		set 
		{
			_hunger=value;
			if (_hunger<0) {_hunger=0;}
			if (_hunger>100) {_hunger=100;}
		}
	}
	int _hunger;
	public int GetHunger() {return hunger;}
	public void SetHunger(int newHunger) 
	{
		hunger=newHunger;
		RefreshMaxStamina();		
	}
	public void ChangeHunger(int hungerDelta)
	{
		SetHunger(hunger+hungerDelta);
	}
	public int hungerIncreasePerHour;
	//Deprecate this later
	public int maxStaminaReductionFromHunger=0;
	public int maxFatigueRestoreReductionFromHunger=40;
	public float maxHealthRegenReductionFromHunger=0.1f;
	
	//FATIGUE
	int fatigue
	{
		get {return _fatigue;}
		set 
		{
			_fatigue=value;
			if (_fatigue<0) {_fatigue=0;}
			if (_fatigue>100) {_fatigue=100;}
		}
	}
	int _fatigue;
	public int GetFatigue() {return fatigue;}
	public void SetFatigue(int newFatigue)
	{
		fatigue=newFatigue;
		RefreshMaxStamina();
	}
	public void ChangeFatigue(int fatigueDelta)
	{
		SetFatigue(fatigue+fatigueDelta);
	}
	//public int fatigueIncreasePerAction;
	public int maxStaminaReductionFromFatigue=6;
	const int fatigueIncreasePerEncounter=20;
	
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
	public int moraleRestorePerHour;
	public int moraleDecayPerHour;
	//per point above/below 50
	public float moraleDamageMod;
	public int moraleChangeFromKills;
	
	public float friendshipChance;
	
	public bool hasLight;
	public bool isCook;
	public bool isLockExpert;
	public bool isMedic;
	public int armorValue;
	public int maxCarryCapacity;
	public int currentCarryCapacity;
	public int visibilityMod;
	
	public RangedWeapon equippedRangedWeapon;
	public int rangedDamageMod;
	public MeleeWeapon equippedMeleeWeapon;
	int minUnarmedDamage=0;
	int maxUnarmedDamage=4;
	public int meleeDamageMod;
	public List<InventoryItem> equippedItems=new List<InventoryItem>();
	public List<InventoryItem> carriedItems=new List<InventoryItem>();
	
	public List<StatusEffect> activeStatusEffects=new List<StatusEffect>();
	public List<Perk> perks=new List<Perk>();
	public Dictionary<PartyMember,Relationship> relationships=new Dictionary<PartyMember, Relationship>();
	
	public PartyMember ()
	{
		GeneratePartyMember(GenerateName());
	}
	
	public PartyMember(string memberName)//, params Perk[] assignedPerks)
	{
		GeneratePartyMember(memberName);
	}
	
	void GeneratePartyMember(string memberName)
	{
		//Pick color out of ones left
		color=Color.white;
		foreach (Color c in GetColors())
		{
			bool colorFound=false;
			foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
			{
				if (member.color==c) {colorFound=true; break;}	
			}
			if (!colorFound) {color=c; break;}
		}
		
		//Randomly pick out perks
		int necessaryPerkCount=3;
		List<Perk> possiblePerks=Perk.GetPerkList();
		while (perks.Count<necessaryPerkCount)
		{
			Perk newPerk=possiblePerks[Random.Range(0,possiblePerks.Count)];
			perks.Add(newPerk);
			possiblePerks.Remove(newPerk);
			if (newPerk.oppositePerk!=null) 
			{
				foreach (Perk possiblePerk in possiblePerks) 
				{
					if (possiblePerk.GetType()==newPerk.oppositePerk) {possiblePerks.Remove(possiblePerk); break;}
				}
			}
		}
		
		//debug
		//if (memberName=="Jimbo") {perks.Clear(); perks.Add(new WeakArm());}
		
		///////
		name=memberName;
		baseMaxStamina=10;
		baseMorale=50;
		//moraleChangePerHour=10;
		moraleRestorePerHour=1;
		moraleDecayPerHour=1;
		
		hunger=0;
		hungerIncreasePerHour=5;
		
		fatigue=0;
		//fatigueIncreasePerAction=10;
		
		armorValue=0;
		maxCarryCapacity=4;//
		visibilityMod=0;
		moraleDamageMod=0.02f;
		moraleChangeFromKills=0;
		friendshipChance=0.6f;
		rangedDamageMod=0;
		meleeDamageMod=0;
		hasLight=false;
		isCook=false;
		isMedic=false;
		isLockExpert=false;
		foreach (Perk myPerk in perks)
		{
			myPerk.ActivatePerk(this);
		}
		//make sure perks trigger before these to properly use modified values of maxHealth and maxStamina
		health=maxHealth;
		currentMaxStamina=baseMaxStamina;
		stamina=currentMaxStamina;
		morale=baseMorale;
		currentCarryCapacity=maxCarryCapacity;
		
		EquipWeapon(new Pipe());
		//equippedMeleeWeapon=new Pipe();
		equippedRangedWeapon=null;//new AssaultRifle();//null;
		PartyManager.TimePassed+=TimePassEffect;
	}
	
	public bool AddStatusEffect(StatusEffect newEffect)
	{
		//bool newEffectAdded=true;
		foreach (StatusEffect activeEffect in activeStatusEffects)
		{
			if(activeEffect.GetType()==newEffect.GetType())
			{
				//newEffectAdded=false;
				if (activeEffect.canStack)activeEffect.StackEffect();
				return false;
			}
		}
		activeStatusEffects.Add(newEffect);
		return true;
	}
	
	void DisposePartyMember()
	{
		//GameManager.DebugPrint("member disposed!");
		occupiedNames.Remove(name);
		PartyManager.TimePassed-=TimePassEffect;
		PartyManager.mainPartyManager.RemovePartyMember(this);
		foreach (StatusEffect effect in activeStatusEffects) {effect.CleanupEffect();}
		activeStatusEffects=null;
		PartyStatusCanvasHandler.main.NewNotification(name+" has died!");
	}
	
	void RollRelationships()
	{
		if (Random.value<0.1f)
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
	}
	
	public void SetRelationship(PartyMember member, Relationship.RelationTypes type)
	{
		relationships.Add(member,new Relationship(member,type));
		string typeDesc="";
		if (type==Relationship.RelationTypes.Friend){ typeDesc="friends";}
		else {typeDesc="enemies";}
		PartyStatusCanvasHandler.main.NewNotification(name+" became "+typeDesc+" with "+member.name);
	}
	public void RemoveRelatonship(PartyMember removedMember) {relationships.Remove(removedMember);}
	
	public void TimePassEffect(int hoursPassed)
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
		//if (hunger<100){health+=2*hoursPassed;}
		if (hunger==100){health-=2*hoursPassed;}
		
		//DO MAX STAMINA
		/*
		currentMaxStamina
		=baseMaxStamina-Mathf.RoundToInt(maxStaminaReductionFromHunger*hunger*0.01f)-Mathf.RoundToInt(maxStaminaReductionFromFatigue*fatigue*0.01f);
		stamina=currentMaxStamina;*/
		//DO MORALE
		//if party is starving, morale drops
		if (hunger>=100) {morale-=moraleDecayPerHour*hoursPassed;}
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
					moraleChange=moraleRestorePerHour*hoursPassed*decaySign;
					if (morale+moraleChange>baseMorale) {morale=baseMorale;}
					else {morale+=moraleChange;}
				}
				//if morale is lowering (from >base)
				if (decaySign<0)
				{
					moraleChange=moraleDecayPerHour*hoursPassed*decaySign;
					if (morale+moraleChange<baseMorale) {morale=baseMorale;}
					else {morale+=moraleChange;}
				}
			}
		}
		
		//DO HUNGER INCREASE
		float cookMult=1;
		foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
		{
			if (member.isCook) {cookMult=Cook.hungerIncreaseMult; break;}
		}	
		//hunger+=(int)(hungerIncreasePerHour*cookMult)*hoursPassed;
		ChangeHunger((int)(hungerIncreasePerHour*cookMult)*hoursPassed);
		
		//DO RELATIONSHIPS
		RollRelationships();
	}
	
	public void RestEffect()
	{
		int newFatigue=Mathf.RoundToInt(maxFatigueRestoreReductionFromHunger*hunger*0.01f);
		SetFatigue(newFatigue);
		if (hunger<100)
		{
			float healthRegen=0.1f-maxHealthRegenReductionFromHunger*hunger*0.01f;
			health+=Mathf.RoundToInt(health*healthRegen);
		}
	}
	
	public void EncounterStartTrigger(List<PartyMember> team)
	{
		foreach (PartyMember member in team) 
		{
			if (relationships.ContainsKey(member)) morale+=relationships[member].OnMissionTogether();
		}
	}
	
	public void EncounterEndTrigger()
	{
		ChangeFatigue(fatigueIncreasePerEncounter);
	}
	
	public bool Heal(int amountHealed)
	{
		bool healed=false;
		if (health<maxHealth)
		{
			//if member is in encounter
			if (EncounterCanvasHandler.main.encounterOngoing)
			{
				foreach (PartyMember member in EncounterCanvasHandler.main.encounterMembers)
				{
					if (member.isMedic) {amountHealed+=Medic.healBonus; break;}
				}	
			}
			else
			{
				//if healing is done outside of encounter
				foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
				{
					if (member.isMedic) {amountHealed+=Medic.healBonus; break;}
				}	
			}
			health+=amountHealed;
			healed=true;
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
		{res=(equippedMeleeWeapon.GetMinDamage()+meleeDamageMod)+"-"+(equippedMeleeWeapon.GetMaxDamage()+meleeDamageMod);}
		else {res=(Mathf.Max(minUnarmedDamage+meleeDamageMod,0))+"-"+(maxUnarmedDamage+meleeDamageMod);}
		return res;
	}
	
	public string GetMeleeAttackDescription()
	{
		string res="Hit for ";
		int requiredStamina=1;
		if (equippedMeleeWeapon!=null) 
		{
			requiredStamina=equippedMeleeWeapon.GetStaminaUse();
			float staminaMod=Mathf.Min(1f,(float)stamina/(float)requiredStamina);
			res+=(Mathf.RoundToInt((equippedMeleeWeapon.GetMinDamage()+meleeDamageMod)*staminaMod))
			+"-"+(Mathf.RoundToInt((equippedMeleeWeapon.GetMaxDamage()+meleeDamageMod)*staminaMod));
		}
		else {res+=(Mathf.Max(minUnarmedDamage+meleeDamageMod,0))+"-"+(maxUnarmedDamage+meleeDamageMod);}
		res+=" damage\n("+requiredStamina+" stamina)";
		return res;
	}
	
	public int MeleeAttack()
	{
		int damage=0;
		if (equippedMeleeWeapon!=null)
		{
		//if (stamina<equippedMeleeWeapon.GetStaminaUse()) {EncounterCanvasHandler.main.DisplayNewMessage(name+"'s attack is weak!");}
		//if (stamina>0) 
		//{
			damage=equippedMeleeWeapon.GetDamage((morale-baseMorale)*moraleDamageMod,meleeDamageMod);
			stamina-=equippedMeleeWeapon.GetStaminaUse();
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
		//}
		//else
		//{
			//damage=1;
		
		//}
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
			res+=(equippedRangedWeapon.GetMinDamage()+rangedDamageMod)+"-"+(equippedRangedWeapon.GetMaxDamage()+rangedDamageMod);
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
			res=(equippedRangedWeapon.GetMinDamage()+rangedDamageMod)+"-"+(equippedRangedWeapon.GetMaxDamage()+rangedDamageMod);
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
	}
	
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
			else {PartyManager.mainPartyManager.GainItems(item);}
			
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
		}
		else {throw new System.Exception("Trying t equip an item that's already equipped twice");}
	}
	
	public void UnequipItem(EquippableItem unequippedItem)
	{
		if (equippedItems.Contains(unequippedItem)) 
		{
			unequippedItem.UnequipEffect(this);
			equippedItems.Remove(unequippedItem);
			//PartyManager.mainPartyManager.GainItems(unequippedItem);
		}
	}
	
	public void EquipWeapon(Weapon newWeapon)
	{
		//check if melee
		if (newWeapon.GetType().BaseType==typeof(MeleeWeapon)){equippedMeleeWeapon=newWeapon as MeleeWeapon;}
		//check if ranged
		if (newWeapon.GetType().BaseType==typeof(RangedWeapon)){equippedRangedWeapon=newWeapon as RangedWeapon;}
		currentCarryCapacity=currentCarryCapacity-newWeapon.GetWeight();//Mathf.Max(0,currentCarryCapacity-newWeapon.GetWeight());
		while (carriedItems.Count>Mathf.Max(currentCarryCapacity,0))
		{
			DropItem(carriedItems[0]);
		}
	}
	
	public void UnequipWeapon(Weapon uneqippedWeapon)
	{
		if (equippedMeleeWeapon==uneqippedWeapon) {equippedMeleeWeapon=null;}
		if (equippedRangedWeapon==uneqippedWeapon) {equippedRangedWeapon=null;}
		currentCarryCapacity=Mathf.Min(maxCarryCapacity,currentCarryCapacity+uneqippedWeapon.GetWeight());
	}
}
