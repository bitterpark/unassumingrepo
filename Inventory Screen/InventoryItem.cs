using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class InventoryItem 
{
	//public static int foundAmmoCount=10;
	
	public abstract Sprite GetItemSprite();
	public virtual bool UseAction(PartyMember member) {return false;}
	public abstract string GetMouseoverDescription();
	
	public virtual int GetWeight() {return 1;}
	
	//Deprecated
	public enum LootItems {Medkits,Bandages,Food/*,PerishableFood*/,Ammo,Flashlight,Radio,SettableTrap,AssaultRifle,Shotgun,NineM,Pipe,Knife,Axe,ArmorVest}
	public enum LootMetatypes {Medical,FoodItems,Melee,Guns,Equipment,Radio}
	
	//Deprecated, remove later!!!
	public static string GetLootingDescription(LootItems itemType)
	{
		string description=null;
		switch (itemType)
		{
			case LootItems.Ammo:{description="You find an ammo box";break;}
			//case LootItems.PerishableFood: {description="You find some perishable food"; break;}
			case LootItems.Food:{description="You find a preserved ration";break;}
			case LootItems.Bandages: {description="You find some bandages"; break;}
			case LootItems.Medkits:{description="You find a medkit";break;}
			case LootItems.Flashlight:{description="You find a flashlight";break;}
			case LootItems.ArmorVest:{description="You find an armor vest";break;}
			case LootItems.Pipe:{description="You find a pipe"; break;}
			case LootItems.Knife:{description="You find a knife";break;}
			case LootItems.Axe:{description="You find an axe";break;}
			case LootItems.NineM:{description="You find a pistol";break;}
			case LootItems.Shotgun: {description="You find a shotgun"; break;}
			case LootItems.AssaultRifle:{description="You find an assault rifle";break;}
			case LootItems.Radio:{description="You find a radio";break;}
			case LootItems.SettableTrap:{description="You find a settable trap"; break;}
		}
		return description;
	}
	
	public static List<InventoryItem> GenerateLootSet(LootMetatypes metaType)
	{
		float randomRoll=Random.value;
		List<InventoryItem> setItems=new List<InventoryItem>();
		switch(metaType)
		{
			case LootMetatypes.FoodItems:
			{
				setItems.Add (new FoodSmall());
				setItems.Add (new FoodSmall());
				if (randomRoll<0.5f)
				{
					setItems.Clear();
					setItems.Add (new FoodBig());
				}
				if (randomRoll<0.25f)
				{
					setItems.Clear();
					setItems.Add (new FoodSmall());
				}
				break;
			}
			case LootMetatypes.Medical:
			{
				//Default <1 option
				setItems.Add (new Medkit());
				//Other options
				if (randomRoll<0.4f)
				{
					setItems.Clear();
					setItems.Add (new Medkit());
					setItems.Add (new Bandages());
				}
				if (randomRoll<0.2f)
				{
					setItems.Clear();
					setItems.Add (new Bandages());
					setItems.Add (new Bandages());
				}
				if (randomRoll<0.1f)
				{
					setItems.Clear();
					setItems.Add (new Medkit());
					setItems.Add (new Medkit());
				}
				break;
			}
			case LootMetatypes.Melee:
			{
				//Default <1 option
				setItems.Add (new Knife());
				//Other options
				if (randomRoll<0.3f)
				{
					setItems.Clear();
					setItems.Add (new Axe());
				}
				break;
			}
			case LootMetatypes.Guns:
			{
				//Default <1 option
				setItems.Add (new AmmoBox());
				//Other options
				if (randomRoll<0.4f)
				{
					setItems.Clear();
					setItems.Add (new NineM());
				}
				if (randomRoll<0.2f)
				{
					setItems.Clear();
					setItems.Add (new AssaultRifle());
				}
				if (randomRoll<0.1f)
				{
					setItems.Clear();
					setItems.Add (new Shotgun());
				}
				break;
			}
			case LootMetatypes.Equipment:
			{
				//Default <1 option
				setItems.Add (new Flashlight());
				//Other options
				if (randomRoll<0.6f)
				{
					setItems.Clear();
					setItems.Add (new ArmorVest());
				}
				break;
			}
			case LootMetatypes.Radio:
			{
				setItems.Add(new Radio());
				break;
			}
		}
		return setItems;
	}
	//deprecated
	public static InventoryItem GetLootingItem(LootItems itemType)
	{
		InventoryItem lootedItem=null;
		switch (itemType)
		{
			case LootItems.Ammo:{lootedItem=new AmmoBox(); break;}
			//FOOD
			//case LootItems.Food:{lootedItem=new Food(); break;}
			//case LootItems.PerishableFood: {lootedItem=new PerishableFood(PartyManager.mainPartyManager.timePassed); break;}
			//MEDS
			case LootItems.Medkits:{lootedItem=new Medkit(); break;}
			case LootItems.Bandages:{lootedItem=new Bandages(); break;}
			//EQUIPMENT
			case LootItems.Flashlight:{lootedItem=new Flashlight(); break;}
			case LootItems.ArmorVest:{lootedItem=new ArmorVest(); break;}
			//MELEE WEAPONS
			case LootItems.Pipe:{lootedItem=new Pipe(); break;}
			case LootItems.Knife:{lootedItem=new Knife(); break;}
			case LootItems.Axe:{lootedItem=new Axe(); break;}
			//RANGED WEAPONS
			case LootItems.NineM:{lootedItem=new NineM(); break;}
			case LootItems.Shotgun: {lootedItem=new Shotgun(); break;}
			case LootItems.AssaultRifle:{lootedItem=new AssaultRifle(); break;}
			//MISC
			case LootItems.Radio:{lootedItem=new Radio(); break;}
			case LootItems.SettableTrap: {lootedItem=new SettableTrap(); break;}
		}
		return lootedItem;
	}
}

public class SettableTrap: InventoryItem
{
	System.Type myTrapType=typeof(Trap);
	
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.settableTrapSprite;}
	
	public override string GetMouseoverDescription ()
	{
		return "Settable trap\nAdds tarp";
	}
	
	public override bool UseAction(PartyMember member)
	{
		bool trapSet=false;
		EncounterCanvasHandler encounterHandler=EncounterCanvasHandler.main;
		if (encounterHandler.encounterOngoing)
		{
			RoomButtonHandler trappedRoomButton=encounterHandler.roomButtons[encounterHandler.memberCoords[member]];
			if (trappedRoomButton.assignedRoom.trapInRoom==null)
			{
				encounterHandler.roomButtons[encounterHandler.memberCoords[member]]
				.SetTrap(new Trap(trappedRoomButton.assignedRoom),true);
				trapSet=true;
			}
		}
		return trapSet;
	}
}

public class AmmoBox:InventoryItem
{
	int ammoAmount=12;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.ammoBoxSprite;}
	
	public override bool UseAction(PartyMember member)
	{
		PartyManager.mainPartyManager.ammo+=ammoAmount;
		return true;
	}
	public override string GetMouseoverDescription ()
	{
		return "Ammo box\nAdds "+ammoAmount+" ammo";
	}
}

public class Medkit:InventoryItem
{
	//int healAmount=20;
	float healPercentage=0.35f;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.medkitSprite;}
	
	public override bool UseAction(PartyMember member)
	{
		bool healingPerformed=false;
		int healAmount=Mathf.FloorToInt(member.maxHealth*healPercentage);
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
		//if (healingPerformed) PartyManager.mainPartyManager.RemoveItems(this);
		return healingPerformed;
	}
	//Currently only works if tooltip is viewed from inventory screen
	public override string GetMouseoverDescription ()
	{
		int healAmount=Mathf.FloorToInt(InventoryScreenHandler.mainISHandler.selectedMember.maxHealth*healPercentage);
		return "Medkit\nHeals "+healAmount+"% of hp\nCures bleed";
	}
}

public class Bandages: InventoryItem
{
	int healAmount=20;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.bandageSprite;}
	
	public override bool UseAction(PartyMember member)
	{
		bool healingPerformed=false;
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
		//if (healingPerformed) PartyManager.mainPartyManager.RemoveItems(this);
		return healingPerformed;
	}
	public override string GetMouseoverDescription ()
	{
		return "Bandages\nCure bleed";
	}
	
}

public class FoodBig:InventoryItem
{
	int nutritionAmount=50;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.foodSpriteBig;}
	
	public override bool UseAction(PartyMember member)
	{
		bool use=false;
		if (member.Eat(nutritionAmount))//PartyManager.mainPartyManager.FeedPartyMember(member,100))
		{
			//PartyManager.mainPartyManager.partyInventory.Remove(this);
			//PartyManager.mainPartyManager.RemoveItems(this);
			use=true;
		}
		return use;
	}
	
	public override string GetMouseoverDescription ()
	{
		return "Canned food\nRestores "+nutritionAmount+" hunger";
	}
}

public class FoodSmall:InventoryItem
{
	int nutritionAmount=20;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.foodSpriteSmall;}
	
	public override bool UseAction(PartyMember member)
	{
		bool use=false;
		if (member.Eat(nutritionAmount))//PartyManager.mainPartyManager.FeedPartyMember(member,100))
		{
			//PartyManager.mainPartyManager.partyInventory.Remove(this);
			//PartyManager.mainPartyManager.RemoveItems(this);
			use=true;
		}
		return use;
	}
	
	public override string GetMouseoverDescription ()
	{
		return "Junk food\nRestores "+nutritionAmount+" hunger";
	}
}
/*
public class PerishableFood:InventoryItem
{
	int nutritionAmount=100;
	int expireTime=5;
	int healthPentalty=10;
	int pickupHour;
	
	public PerishableFood()
	{
		pickupHour=PartyManager.mainPartyManager.timePassed;
	}
	
	public PerishableFood (int pickupTime)
	{
		pickupHour=pickupTime;
	}
	
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.perishableFoodSprite;}
	
	public override bool UseAction(PartyMember member)
	{
		bool use=false;
		if (member.Eat(nutritionAmount))//PartyManager.mainPartyManager.FeedPartyMember(member,100))
		{
			//IF more time has passed since pickup than expiry date allows - apply penalty
			if (PartyManager.mainPartyManager.timePassed-pickupHour>=expireTime) {member.health-=healthPentalty;}
			use=true;
		}
		return use;
	}
	
	public override string GetMouseoverDescription ()
	{
		string desc="Perishable food\nRemoves all hunger";
		int timeToExpiry=expireTime-(PartyManager.mainPartyManager.timePassed-pickupHour);
		if (timeToExpiry>0) {desc+="\nExpires in:"+timeToExpiry+"hours";}
		else {desc+="\nExpired";}
		
		return desc;//"Perishable food\nRemoves all hunger";
	}

}
*/
public class Radio:InventoryItem
{
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.radioSprite;}
	
	public override bool UseAction(PartyMember member)
	{
		GameManager.main.EndCurrentGame(true);
		return false;
	}
	
	public override string GetMouseoverDescription ()
	{
		return "Radio\nSummons a rescue";
	}
}

public abstract class EquippableItem:InventoryItem
{
	public abstract void EquipEffect(PartyMember member);
	public abstract void UnequipEffect(PartyMember member);
	//public override bool UseAction(PartyMember member) {return false;}
}

public class Flashlight: EquippableItem
{
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.flashlightSprite;}
	public override void EquipEffect (PartyMember member)
	{
		member.hasLight=true;
	}
	public override void UnequipEffect (PartyMember member)
	{
		member.hasLight=false;
	}
	
	public override string GetMouseoverDescription ()
	{
		return "Flashlight\nImproves visibility at night";
	}
}

public class ArmorVest:EquippableItem
{
	int damageReduction=3;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.armorvestSprite;}
	public override void EquipEffect (PartyMember member)
	{
		member.armorValue+=damageReduction;
	}
	public override void UnequipEffect (PartyMember member)
	{
		member.armorValue-=damageReduction;
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
	public abstract int GetMinDamage();
	//damage for fight calculation
	public virtual int GetDamage(float modifier) 
	{
		int rawDamage=Mathf.RoundToInt(GaussianRandom.GetFiveStepRange(GetMinDamage(),GetMaxDamage())+modifier);//Random.Range(GetMinDamage()-0.5f,GetMaxDamage()+0.5f)+modifier);
		int actualDamage=Mathf.Clamp(rawDamage,GetMinDamage(),GetMaxDamage());
		return actualDamage;
	}
	public abstract string GetName();//virtual string GetName() {return "";}
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
		return GetName()+"\nDamage:"+GetMinDamage()+"-"+GetMaxDamage()+"\nAmmo per shot:"+GetAmmoUsePerShot()+"\n Weight:"+GetWeight();
	}
	//public override bool UseAction (PartyMember member) {return false;}
}

public class NineM:RangedWeapon
{
	string name="9mm Pistol";
	int weaponMaxDamage=6;
	int weaponMinDamage=5;
	int ammoPerShot=1;
	
	public override int GetMaxDamage() {return weaponMaxDamage;}
	public override int GetMinDamage() {return weaponMinDamage;}
	public override string GetName() {return name;}
	public override int GetAmmoUsePerShot (){return ammoPerShot;}
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.nineMSprite;
	}
	/*
	public override string GetMouseoverDescription ()
	{
		return name+"\nDamage:"+weaponMaxDamage+"\nAmmo per shot:"+ammoPerShot;
	}*/
}

public class Shotgun:RangedWeapon
{
	string name="Shotgun";
	int weaponMaxDamage=10;
	int weaponMinDamage=8;
	int oneShotMinDamage=4;
	int oneShotMaxDamage=5;
	int ammoPerShot=2;
	public override int GetMaxDamage() {return weaponMaxDamage;}
	public override int GetMinDamage() {return weaponMinDamage;}
	public override int GetDamage(float modifier) 
	{
		int actualDamage=base.GetDamage(modifier);
		if (PartyManager.mainPartyManager.ammo<ammoPerShot)
		{
			actualDamage=0;
			for (int i=0; i<PartyManager.mainPartyManager.ammo; i++)
			{
				int rawDamage=Mathf.RoundToInt(GaussianRandom.GetFiveStepRange(oneShotMinDamage,oneShotMaxDamage)+modifier);//Random.Range(oneShotMinDamage-0.5f,oneShotMaxDamage+0.5f)+modifier);
				actualDamage+=Mathf.Clamp(rawDamage,oneShotMinDamage,oneShotMaxDamage);
			}
			//actualDamage=oneShotDamage*PartyManager.mainPartyManager.ammo;
		}
		return actualDamage;
	}
	public override string GetName() {return name;}
	public override int GetAmmoUsePerShot () {return ammoPerShot;}
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.shotgunSprite;
	}
	
}

public class AssaultRifle:RangedWeapon
{
	string name="Assault Rifle";
	int weaponMaxDamage=15;
	int weaponMinDamage=12;
	int oneShotMinDamage=4;
	int oneShotMaxDamage=5;
	int ammoPerShot=3;
	public override int GetMaxDamage() {return weaponMaxDamage;}
	public override int GetMinDamage() {return weaponMinDamage;}
	public override int GetDamage(float modifier) 
	{
		int actualDamage=base.GetDamage(modifier);
		if (PartyManager.mainPartyManager.ammo<ammoPerShot)
		{
			actualDamage=0;
			for (int i=0; i<PartyManager.mainPartyManager.ammo; i++)
			{
				int rawDamage=Mathf.RoundToInt(GaussianRandom.GetFiveStepRange(oneShotMinDamage,oneShotMaxDamage)+modifier);//Random.Range(oneShotMinDamage-0.5f,oneShotMaxDamage+0.5f)+modifier);
				actualDamage+=Mathf.Clamp(rawDamage,oneShotMinDamage,oneShotMaxDamage);
			}
			//actualDamage=oneShotDamage*PartyManager.mainPartyManager.ammo;
		}
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
		return name+"\nDamage:"+weaponMaxDamage+"\nAmmo per shot:"+ammoPerShot;
	}*/
}

public abstract class MeleeWeapon:Weapon
{
	public abstract int GetStaminaUse();
	public override string GetMouseoverDescription ()
	{
		return GetName()+"\nDamage:"+GetMinDamage()+"-"+GetMaxDamage()+"\nStamina per hit:"+GetStaminaUse()+"\n Weight:"+GetWeight();
	}
	public int GetDamage (float moraleModifier, float additionModifier)
	{
		//int actualDamage=base.GetDamage(modifier);
		/*
		int rawDamage=Mathf.RoundToInt(GaussianRandom.GetFiveStepRange(GetMinDamage(),GetMaxDamage())+modifier);//Random.Range(GetMinDamage()-0.5f,GetMaxDamage()+0.5f)+modifier);
		int actualDamage=Mathf.Clamp(rawDamage,GetMinDamage(),GetMaxDamage());
		return actualDamage;
		*/
		float missingStaminaMod=Mathf.Min(1f,(float)EncounterCanvasHandler.main.selectedMember.stamina/(float)GetStaminaUse());
		//GameManager.DebugPrint("Dividing stamina mod result:"+(float)EncounterCanvasHandler.main.selectedMember.stamina/(float)GetStaminaUse());
		//GameManager.DebugPrint("Missing stamina mod is:"+missingStaminaMod);
		float adjustedMinDamage=(GetMinDamage()+additionModifier)*missingStaminaMod;
		float adjustedMaxDamage=(GetMaxDamage()+additionModifier)*missingStaminaMod;
		float rawDamage=GaussianRandom.GetFiveStepRange(adjustedMinDamage,adjustedMaxDamage)+moraleModifier;
		//GameManager.DebugPrint("Actual min:"+adjustedMinDamage);
		//GameManager.DebugPrint("Actual max:"+adjustedMaxDamage);
		//GameManager.DebugPrint("Raw damage value"+rawDamage);
		//GameManager.DebugPrint("Clamp Result:"+Mathf.Clamp(rawDamage,adjustedMinDamage,adjustedMaxDamage));
		int actualDamage=Mathf.RoundToInt(Mathf.Clamp(rawDamage,adjustedMinDamage,adjustedMaxDamage));
		/*
		if (EncounterCanvasHandler.main.selectedMember.stamina<GetStaminaUse()) 
		{
			actualDamage=GetMinDamage();	
		}*/
		return actualDamage;
	}
	//public int GetDamageRoll(float modifier)
	//{
		//return base.GetDamage(modifier);
	//}
}

public class Pipe:MeleeWeapon
{
	public int weaponMaxDamage=11;
	public int weaponMinDamage=7;
	int staminaUse=3;
	public string name="Pipe";
	
	public override int GetMaxDamage() {return weaponMaxDamage;}
	public override int GetMinDamage() {return weaponMinDamage;}
	public override string GetName() {return name;}
	public override int GetStaminaUse() {return staminaUse;}
	
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.pipeSprite;
	}
	/*
	public override string GetMouseoverDescription ()
	{
		return name+"\nDamage:"+weaponMaxDamage;
	}*/
}

public class Knife:MeleeWeapon
{
	public int weaponMaxDamage=5;
	public int weaponMinDamage=1;
	
	public string name="Knife";
	int staminaUse=2;
	
	public override int GetMaxDamage() {return weaponMaxDamage;}
	public override int GetMinDamage() {return weaponMinDamage;}
	public override string GetName() {return name;}
	public override int GetStaminaUse() {return staminaUse;}
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.knifeSprite;
	}
	/*
	public override string GetMouseoverDescription ()
	{
		return name+"\nDamage:"+weaponMaxDamage;
	}*/
}

public class Axe:MeleeWeapon
{
	public int weaponMaxDamage=20;
	public int weaponMinDamage=16;
	//int weakDamage=3;
	public string name="Axe";
	int staminaUse=4;
	
	public override int GetMaxDamage() {return weaponMaxDamage;}
	public override int GetMinDamage() {return weaponMinDamage;}
	/*
	public override int GetDamage() 
	{
		int actualDamage=base.GetDamage();
		if (EncounterManager.mainEncounterManager.selectedMember.stamina<staminaUse) 
		{
			actualDamage=weaponMinDamage;	
		}
		return actualDamage;
	}*/
	public override string GetName() {return name;}
	public override int GetStaminaUse() {return staminaUse;}
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.axeSprite;
	}
}
