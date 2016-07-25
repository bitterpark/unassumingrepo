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
	List<PrepCard> GetWeaponPrepCards();
	List<PrepCard> GetClassPrepCards();
}

public class PartyMember: Mercenary
{
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
		
		return namesList[Random.Range(0,namesList.Count)];
	}
	
	static List<Color> GetColors() 
	{
		List<Color> possibleColors=new List<Color>(SpriteBase.mainSpriteBase.possibleMemberColors);
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

	int startArmor = 0;

	public int hireDaysRemaining = 0;
	
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
	
	public RangedWeapon equippedRangedWeapon;
	public MeleeWeapon equippedMeleeWeapon;
	public List<InventoryItem> equippedItems=new List<InventoryItem>();
	public List<InventoryItem> carriedItems=new List<InventoryItem>();
	public void RemoveCarriedItem(InventoryItem item)
	{
		if (!carriedItems.Contains(item)) throw new System.Exception("No carried item!");
		else carriedItems.Remove(item);
	}
	
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
	
	//NEW STUFF!!!
	public enum MercClass {Soldier,Bandit,Gunman,Mercenary};
	public MercClass myClass;
	CombatDeck combatDeck = new CombatDeck();
	List<PrepCard> weaponPrepCards = new List<PrepCard>();
	List<PrepCard> classPrepCards = new List<PrepCard>();

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
			startArmor = 60;
			stamina = 6;
			ammo = 1;
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

	static CombatDeck GenerateClassCombatDeck(MercClass mercClass)
	{
		CombatDeck result = new CombatDeck();

		if (mercClass == MercClass.Soldier)
			result = SoldierCards.GetClassCards();
		if (mercClass == MercClass.Bandit)
			result = BanditCards.GetClassCards();
		if (mercClass == MercClass.Gunman)
			result = GunmanCards.GetClassCards();
		if (mercClass==MercClass.Mercenary)
			result = MercenaryCards.GetClassCards();
		//result = new CombatDeck();
		//result.AddCards(typeof(Headshot),2);
		//result.AddCards(typeof(Hipfire), 2);

		return result;
	}

	static List<PrepCard> GenerateClassPrepCards(MercClass mercClass)
	{
		List<PrepCard> result = new List<PrepCard>();

		if (mercClass == MercClass.Soldier)
			result = SoldierCards.GetClassPrepCards();
		if (mercClass == MercClass.Bandit)
			result = BanditCards.GetClassPrepCards();
		if (mercClass == MercClass.Gunman)
			result = GunmanCards.GetClassPrepCards();
		if (mercClass == MercClass.Mercenary)
			result = MercenaryCards.GetClassPrepCards();

		return result;
	}

	static List<PrepCard> GenerateDefaultWeaponPrepCards()
	{
		List<PrepCard> defaultPrepCardList = new List<PrepCard>();
		defaultPrepCardList.Add(new DefaultMeleePrepCard());
		defaultPrepCardList.Add(new DefaultRangedPrepCard());
		return defaultPrepCardList;
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
		combatDeck = GenerateClassCombatDeck(myClass);
		classPrepCards = GenerateClassPrepCards(myClass);
		weaponPrepCards = GenerateDefaultWeaponPrepCards();
		
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

		name=memberName;

		baseMaxStamina=10;
		armorValue=0;
		maxCarryCapacity=4;//

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
		UpdateCurrentCarryCapacity();
		equippedRangedWeapon=null;
	}
	
	void DisposePartyMember()
	{
		occupiedNames.Remove(name);
		//PartyManager.ETimePassed-=TimePassEffect;
		//PartyManager.ETimePassedEnd-=LateTimePassEffect;
		PartyManager.mainPartyManager.RemovePartyMember(this,true);
		PartyStatusCanvasHandler.main.NewNotification(name+" has died!");
	}

	void DropAllEquippedItems()
	{
		//Drop carried items
		//Pool together all items
		List<InventoryItem> allItems = new List<InventoryItem>();
		foreach (InventoryItem item in carriedItems) 
			allItems.Add(item);
		foreach (InventoryItem item in equippedItems)
			allItems.Add(item);
		//May want to clear the lists after this
		if (equippedRangedWeapon != null)
			allItems.Add(equippedRangedWeapon);
		if (equippedMeleeWeapon != null)
			allItems.Add(equippedMeleeWeapon);

		foreach (InventoryItem item in allItems) 
			currentRegion.StashItem(item);
	}
	
	public void TimePassEffect()
	{	
		//REGEN/LOSE HEALTH
		IncrementHealth(TownManager.main.GetDailyMercHealRate());

		hireDaysRemaining--;
		if (hireDaysRemaining < 1)
			FinishContract();	
		//DO RELATIONSHIPS
		//RollRelationships();
	}
    //Currently unused, might want to use later
	public void LateTimePassEffect()
	{
		
	}

	void FinishContract()
	{
		DropAllEquippedItems();
		PartyManager.mainPartyManager.RemovePartyMember(this, false);
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
			currentRegion.StashItem(item);
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

		PrepCard cardAddedByWeapon;
		if (newWeapon.TryGetAddedPrepCard(out cardAddedByWeapon))
			weaponPrepCards.Add(cardAddedByWeapon);
		
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

		PrepCard cardAddedByWeapon;
		if (uneqippedWeapon.TryGetAddedPrepCard(out cardAddedByWeapon))
			weaponPrepCards.Remove(cardAddedByWeapon);

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

	public List<PrepCard> GetWeaponPrepCards(){
		return weaponPrepCards;
	}

	public List<PrepCard> GetClassPrepCards()
	{
		return classPrepCards;
	}

	public CombatDeck GetCombatDeck() { 
		return combatDeck; 
	}
	public List<CombatCard> GetAllUsedCards() { 
		return combatDeck.GetDeckCards();
	}

	public int GetStartAmmo() 
	{ 
		return ammo; 
	}

	//OLD CRAP
}
