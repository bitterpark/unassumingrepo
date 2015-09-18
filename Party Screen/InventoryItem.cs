using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class InventoryItem 
{
	public abstract Sprite GetItemSprite();
	public abstract void UseAction(PartyMember member);
	public abstract string GetMouseoverDescription();
}

public class Medkit:InventoryItem
{
	int healAmount=20;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.medkitSprite;}
	
	public override void UseAction(PartyMember member)
	{
		bool healingPerformed=false;
		//do health
		if (member.Heal(healAmount)){healingPerformed=true;}
		
		//do bleeding
		//find all bleeding lacerations
		List<Bleed> lacerations=new List<Bleed>();
		foreach (StatusEffect effect in member.activeStatusEffects)
		{
			if (effect.GetType()==typeof(Bleed)) {lacerations.Add(effect as Bleed);}
		}
		if (lacerations.Count>0) {healingPerformed=true;}
		//treat all found lacerations
		foreach (Bleed laceration in lacerations) {laceration.CureBleed();}//PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(member,laceration);}
		//Spend medkit if it did anything
		if (healingPerformed) PartyManager.mainPartyManager.RemoveItems(this);
	}
	public override string GetMouseoverDescription ()
	{
		return "Medkit\nHeals "+healAmount+" hp\nCures bleed";
	}
}

public class Food:InventoryItem
{
	int nutritionAmount=100;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.foodSprite;}
	
	public override void UseAction(PartyMember member)
	{
		if (member.Eat(nutritionAmount))//PartyManager.mainPartyManager.FeedPartyMember(member,100))
		{
			//PartyManager.mainPartyManager.partyInventory.Remove(this);
			PartyManager.mainPartyManager.RemoveItems(this);
		}
	}
	
	public override string GetMouseoverDescription ()
	{
		return "Ration\nRemoves all hunger";
	}
}


public class Flashlight:InventoryItem
{
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.flashlightSprite;}
	
	public override void UseAction(PartyMember member)
	{
		if (member.EquipmentItemToggle(this)) {member.hasLight=true;} 
		else {member.hasLight=false;}
	}
	
	public override string GetMouseoverDescription ()
	{
		return "Flashlight\nImproves visibility at night";
	}
}

public class ArmorVest:InventoryItem
{
	int damageReduction=3;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.armorvestSprite;}
	
	public override void UseAction(PartyMember member)
	{
		if (member.EquipmentItemToggle(this)) {member.armorValue+=damageReduction;} 
		else {member.armorValue-=damageReduction;}
	}
	
	public override string GetMouseoverDescription ()
	{
		return "Armor vest\nReduces physical damage by "+damageReduction;
	}
}


public abstract class Weapon:InventoryItem
{
	//damage for display on mouseover text
	public abstract int GetMaxDamage();
	//damage for fight calculation
	public virtual int GetDamage() {return GetMaxDamage();}//virtual int GetDamage() {return 0;}
	public abstract string GetName();//virtual string GetName() {return "";}
	
	public override void UseAction (PartyMember member)
	{
		member.EquipWeapon(this);
	}
	public virtual void Unequip(PartyMember member)//virtual void Unequip (PartyMember member) {}
	{
		member.UnequipWeapon(this);
	}
}

public abstract class RangedWeapon:Weapon
{
	public abstract int GetAmmoUsePerShot();
	public override string GetMouseoverDescription ()
	{
		return GetName()+"\nDamage:"+GetMaxDamage()+"\nAmmo per shot:"+GetAmmoUsePerShot();
	}
}

public class NineM:RangedWeapon
{
	string name="9mm Pistol";
	int weaponDamage=5;
	int ammoPerShot=1;
	
	public override int GetMaxDamage() {return weaponDamage;}
	public override string GetName() {return name;}
	public override int GetAmmoUsePerShot (){return ammoPerShot;}
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.nineMSprite;
	}
	/*
	public override string GetMouseoverDescription ()
	{
		return name+"\nDamage:"+weaponDamage+"\nAmmo per shot:"+ammoPerShot;
	}*/
}

public class AssaultRifle:RangedWeapon
{
	string name="Assault Rifle";
	int weaponDamage=15;
	int oneShotDamage=5;
	int ammoPerShot=3;
	public override int GetMaxDamage() {return weaponDamage;}
	public override int GetDamage() 
	{
		int actualDamage=weaponDamage;
		if (PartyManager.mainPartyManager.ammo<ammoPerShot){actualDamage=oneShotDamage*PartyManager.mainPartyManager.ammo;}
		return actualDamage;
	}
	public override string GetName() {return name;}
	public override int GetAmmoUsePerShot () {return ammoPerShot;}
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.assaultRifleSprite;
	}
	/*
	public override string GetMouseoverDescription ()
	{
		return name+"\nDamage:"+weaponDamage+"\nAmmo per shot:"+ammoPerShot;
	}*/
}

public abstract class MeleeWeapon:Weapon
{
	public abstract int GetStaminaUse();
	public override string GetMouseoverDescription ()
	{
		return GetName()+"\nDamage:"+GetMaxDamage()+"\nStamina per hit:"+GetStaminaUse();
	}
	//public virtual int GetDamage() {return 0;}
	//public virtual string GetName() {return "";} 
	/*
	public override void UseAction(PartyMember member)
	{
		//member.SwapMeleeWeapon(this);
		PartyManager.mainPartyManager.RemoveItems(this);
	} 
	
	public override void Unequip(PartyMember member) {member.UnequipMeleeWeapon();}*/
}

public class Pipe:MeleeWeapon
{
	public int weaponDamage=2;
	int staminaUse=1;
	public string name="Pipe";
	
	public override int GetMaxDamage() {return weaponDamage;}
	public override string GetName() {return name;}
	public override int GetStaminaUse() {return staminaUse;}
	
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.pipeSprite;
	}
	/*
	public override string GetMouseoverDescription ()
	{
		return name+"\nDamage:"+weaponDamage;
	}*/
}

public class Knife:MeleeWeapon
{
	public int weaponDamage=3;
	public string name="Knife";
	int staminaUse=1;
	
	public override int GetMaxDamage() {return weaponDamage;}
	public override string GetName() {return name;}
	public override int GetStaminaUse() {return staminaUse;}
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.knifeSprite;
	}
	/*
	public override string GetMouseoverDescription ()
	{
		return name+"\nDamage:"+weaponDamage;
	}*/
}

public class Axe:MeleeWeapon
{
	public int weaponDamage=7;
	int weakDamage=3;
	public string name="Axe";
	int staminaUse=2;
	public override int GetMaxDamage() {return weaponDamage;}
	public override int GetDamage() 
	{
		int actualDamage=weaponDamage;
		if (EncounterManager.mainEncounterManager.selectedMember.stamina<staminaUse) 
		{
			actualDamage=weakDamage;	
		}
		return actualDamage;
	}
	public override string GetName() {return name;}
	public override int GetStaminaUse() {return staminaUse;}
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.axeSprite;
	}
}
