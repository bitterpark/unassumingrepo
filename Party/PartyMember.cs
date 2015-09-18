using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartyMember 
{
	public string name;
	
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
	
	public int stamina
	{
		get {return _stamina;}
		set 
		{
			if (value>0) {_stamina=value;}
			else {_stamina=0;}
		}
	}
	public int _stamina; 
	public int maxStamina;
	
	public int hunger
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
	public int hungerIncreasePerHour;
	
	public bool hasLight=false;
	public int armorValue;
	
	public RangedWeapon equippedRangedWeapon;
	public MeleeWeapon equippedMeleeWeapon;
	public List<InventoryItem> equippedItems=new List<InventoryItem>();
	
	public List<StatusEffect> activeStatusEffects=new List<StatusEffect>();
	public List<Perk> perks=new List<Perk>();
	
	
	public PartyMember(string memberName, params Perk[] assignedPerks)
	{
		name=memberName;
		maxStamina=10;
		hunger=0;
		hungerIncreasePerHour=10;
		armorValue=0;
		if (assignedPerks!=null) perks.AddRange(assignedPerks);
		foreach (Perk myPerk in perks)
		{
			myPerk.ActivatePerk(this);
		}
		//make sure perks trigger before these to properly use modified values of maxHealth and maxStamina
		health=maxHealth;
		stamina=maxStamina;
		
		equippedMeleeWeapon=new Pipe();
		equippedRangedWeapon=null;//new NineM();//null;
		PartyManager.TimePassed+=TimePassEffect;
	}
	
	void DisposePartyMember()
	{
		PartyManager.TimePassed-=TimePassEffect;
		PartyManager.mainPartyManager.RemovePartyMember(this);
	}
	
	public void TimePassEffect(int hoursPassed)
	{
		hunger+=hungerIncreasePerHour*hoursPassed;
	}
	
	public bool Heal(int amountHealed)
	{
		bool healed=false;
		if (health<maxHealth)
		{
			health+=amountHealed;
			healed=true;
		}
		return healed;
	}
	
	public int MeleeAttack()
	{
		int damage=1;
		if (stamina>0) 
		{
			stamina-=equippedMeleeWeapon.GetStaminaUse();
			damage=equippedMeleeWeapon.GetDamage();
		}
		else
		{
			damage=1;
			EncounterManager.mainEncounterManager.DisplayNewMessage(name+"'s attack is weak!");
		}
		return damage;
	}
	
	public int RangedAttack()
	{
		int damage=equippedRangedWeapon.GetDamage();
		PartyManager.mainPartyManager.ammo-=equippedRangedWeapon.GetAmmoUsePerShot();
		return damage;
	}
	
	public bool Eat(int amountFed)
	{
		bool ate=false;
		if (hunger>0)
		{
			hunger-=amountFed;
			ate=true;
		}
		return ate;
	}
	
	public int TakeDamage(int dmgTaken)
	{
		return TakeDamage(dmgTaken,true);
	}
	public int TakeDamage(int dmgTaken, bool armorHelps)
	{
		int realDmg=dmgTaken;
		if (armorHelps) 
		{
			realDmg-=armorValue;
			if (realDmg<0) realDmg=0;
		}
		health-=realDmg;
		if (health<=0) {DisposePartyMember();}
		return realDmg;
	}
	///
	public bool EquipmentItemToggle(InventoryItem equipmentItem)
	{
		bool itemEquipped=false;
		if (!equippedItems.Contains(equipmentItem)) 
		{
			//check to see if a similar item is already equipped
			bool stackingEquipment=false;
			foreach (InventoryItem item in equippedItems)
			{
				if (item.GetType()==equipmentItem.GetType()) {stackingEquipment=true; break;}
			}
			if (!stackingEquipment)
			{
				equippedItems.Add(equipmentItem);
				PartyManager.mainPartyManager.RemoveItems(equipmentItem);
				itemEquipped=true;
			}
		}
		else
		{
			equippedItems.Remove(equipmentItem);
			PartyManager.mainPartyManager.GainItems(equipmentItem);
		}
		return itemEquipped;
	}
	
	public void UnequipItem(InventoryItem unequippedItem)
	{
		if (equippedItems.Contains(unequippedItem)) 
		{
			equippedItems.Remove(unequippedItem);
			PartyManager.mainPartyManager.GainItems(unequippedItem);
		}
	}
	
	public void EquipWeapon(Weapon newWeapon)
	{
		//check if melee
		if (newWeapon.GetType().BaseType==typeof(MeleeWeapon))
		{
			//Must be in this order for item refresh to work correctly
			if (equippedMeleeWeapon!=null) PartyManager.mainPartyManager.GainItems(equippedMeleeWeapon);
			equippedMeleeWeapon=newWeapon as MeleeWeapon;
			PartyManager.mainPartyManager.RemoveItems(newWeapon);
				
		}

		//check if ranged
		if (newWeapon.GetType().BaseType==typeof(RangedWeapon))
		{
			//Must be in this order for item refresh to work correctly
			if (equippedRangedWeapon!=null) PartyManager.mainPartyManager.GainItems(equippedRangedWeapon);
			equippedRangedWeapon=newWeapon as RangedWeapon;
			PartyManager.mainPartyManager.RemoveItems(newWeapon);
		}
	}
	
	public void UnequipWeapon(Weapon uneqippedWeapon)
	{
		if (equippedMeleeWeapon==uneqippedWeapon) 
		{
			equippedMeleeWeapon=null;
			PartyManager.mainPartyManager.GainItems(uneqippedWeapon);
		}
		
		if (equippedRangedWeapon==uneqippedWeapon) 
		{
			equippedRangedWeapon=null;
			PartyManager.mainPartyManager.GainItems(uneqippedWeapon);
		}
	}
}
