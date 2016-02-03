using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class InventoryItem 
{
	//public static int foundAmmoCount=10;
	
	public string itemName;
	
	public abstract Sprite GetItemSprite();
	public virtual bool UseAction(PartyMember member) {return false;}
	public abstract string GetMouseoverDescription();
	
	public virtual int GetWeight() {return 1;}
	
	//Deprecated
	public enum LootItems {Medkits,Bandages,Food/*,PerishableFood*/,Ammo,Flashlight,Radio,Bed,Backpack,SettableTrap,AssaultRifle,Shotgun,NineM,Pipe,Knife,Axe,ArmorVest}
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
	
	public static string GetLootMetatypeDescription(LootMetatypes metaType)
	{
		string metatypeDesc="";
		switch(metaType)
		{
		case LootMetatypes.FoodItems:
		{
			metatypeDesc="Food";
			break;
		}
		case LootMetatypes.Medical:
		{
			metatypeDesc="Medicine";
			break;
		}
		case LootMetatypes.Melee:
		{
			metatypeDesc="Melee weapons";
			break;
		}
		case LootMetatypes.Guns:
		{
			metatypeDesc="Guns";
			break;
		}
		case LootMetatypes.Equipment:
		{
			metatypeDesc="Equipment";
			break;
		}
		case LootMetatypes.Radio:
		{
			metatypeDesc="Radio";
			break;
		}
		}
		return metatypeDesc;
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
				ProbabilityList<InventoryItem> equipmentList=new ProbabilityList<InventoryItem>();
				equipmentList.AddProbability(new SettableTrap(),0.5f);
				equipmentList.AddProbability(new ArmorVest(),0.15f);
				equipmentList.AddProbability(new Backpack(),0.15f);
				equipmentList.AddProbability(new Bed(),0.2f);
				
				InventoryItem resultingItem=new SettableTrap();
				if (equipmentList.RollProbability(out resultingItem)) setItems.Add(resultingItem);
				else throw new System.Exception("Could not roll a positive result on equipment loot table!");
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
	//System.Type myTrapType=typeof(Trap);
	int legDamage=90;
	
	public SettableTrap (){itemName="Trap";}
	
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.settableTrapSprite;}
	
	public override string GetMouseoverDescription ()
	{
		return "Settable trap\nSets a one-use trap";
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
				.SetTrap(new Trap(trappedRoomButton.assignedRoom,legDamage),true);
				trapSet=true;
			}
		}
		return trapSet;
	}
}

public class AmmoBox:InventoryItem
{
	public AmmoBox()
	{
		itemName="Ammo box";
	}
	
	int ammoAmount=12;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.ammoBoxSprite;}
	
	public override bool UseAction(PartyMember member)
	{
		PartyManager.mainPartyManager.ammo+=ammoAmount;
		return true;
	}
	public override string GetMouseoverDescription ()
	{
		return itemName+"\nAdds "+ammoAmount+" ammo";
	}
}

public class Medkit:InventoryItem
{
	public Medkit()
	{
		itemName="Medkit";
	}
	
	int healAmount=15;
	//float healPercentage=0.35f;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.medkitSprite;}
	
	public override bool UseAction(PartyMember member)
	{
		bool healingPerformed=false;
		//int healAmount=Mathf.FloorToInt(member.maxHealth*healPercentage);
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
		//int healAmount=Mathf.FloorToInt(InventoryScreenHandler.mainISHandler.selectedMember.maxHealth*healPercentage);
		return itemName+"\nHeals "+healAmount+" of hp for each body part\nCures bleed";
	}
}

public class Bandages: InventoryItem
{
	public Bandages() 
	{
		itemName="Bandages";
	}
	
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
		return itemName+"\nCure bleed";
	}
	
}

public class FoodBig:InventoryItem
{
	public FoodBig()
	{
		itemName="Canned food";
	}
	
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
		return itemName+"\nRestores "+nutritionAmount+" hunger";
	}
}

public class FoodSmall:InventoryItem
{
	public FoodSmall()
	{
		itemName="Junk food";
	}
	
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
		return itemName+"\nRestores "+nutritionAmount+" hunger";
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
	public Radio()
	{
		itemName="Radio";
	}
	
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.radioSprite;}
	
	public override bool UseAction(PartyMember member)
	{
		GameManager.main.EndCurrentGame(true);
		return false;
	}
	
	public override string GetMouseoverDescription ()
	{
		return itemName+"\nSummons a rescue";
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
	public Flashlight()
	{
		itemName="Flashlight";
	}
	
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
		return itemName+"\nCurrently has no use";
	}
}

public class Backpack: EquippableItem
{
	int carryCapIncrease=1;
	public Backpack()
	{
		itemName="Backpack";
	}
	
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.backpackSprite;}
	public override void EquipEffect (PartyMember member)
	{
		member.ChangeMaxCarryCapacity(carryCapIncrease);
	}
	public override void UnequipEffect (PartyMember member)
	{
		member.ChangeMaxCarryCapacity(-carryCapIncrease);
	}
	
	public override string GetMouseoverDescription ()
	{
		return itemName+"\nIncreases carry capacity by "+(carryCapIncrease).ToString();
	}
}

public class ArmorVest:EquippableItem
{
	public ArmorVest()
	{
		itemName="Armor vest";
	}
	
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
		return itemName+"\nReduces physical damage by "+damageReduction;
	}
}


public abstract class Weapon:InventoryItem
{	
	//damage for display on mouseover text
	//public abstract int GetMaxDamage();
	//public abstract int GetMinDamage();
	public int baseDamage;
	public float accuracyMod;
	//damage for fight calculation
	public virtual int GetDamage(float modifier) 
	{
		//int rawDamage=Mathf.RoundToInt(GaussianRandom.GetFiveStepRange(GetMinDamage(),GetMaxDamage())+modifier);//Random.Range(GetMinDamage()-0.5f,GetMaxDamage()+0.5f)+modifier);
		int actualDamage=Mathf.RoundToInt(baseDamage+modifier);//Mathf.Clamp(rawDamage,GetMinDamage(),GetMaxDamage());
		return actualDamage;
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
		return itemName+"\nDamage:"+baseDamage+"\nAmmo per shot:"+GetAmmoUsePerShot()+"\n Weight:"+GetWeight();
	}
	//public override bool UseAction (PartyMember member) {return false;}
}

public class NineM:RangedWeapon
{
	public NineM()
	{
		itemName="9mm Pistol";
		baseDamage=45;
		accuracyMod=0.2f;
	}
	
	//string name="9mm Pistol";
	//int weaponMaxDamage=100;
	//int weaponMinDamage=80;
	int ammoPerShot=1;
	
	//public override int GetMaxDamage() {return weaponMaxDamage;}
	//public override int GetMinDamage() {return weaponMinDamage;}
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
	
	public Shotgun()
	{
		itemName="Shotgun";
		oneShotDamage=45;
		accuracyMod=0;
		baseDamage=oneShotDamage*ammoPerShot;
	}
	
	//int weaponMaxDamage=200;
	//int weaponMinDamage=160;
	//int oneShotMinDamage=80;
	//int oneShotMaxDamage=100;
	int oneShotDamage;
	int ammoPerShot=2;
	//public override int GetMaxDamage() {return weaponMaxDamage;}
	//public override int GetMinDamage() {return weaponMinDamage;}
	public override int GetDamage(float modifier) 
	{
		int actualDamage=base.GetDamage(modifier);
		if (PartyManager.mainPartyManager.ammo<ammoPerShot)
		{
			actualDamage=0;
			for (int i=0; i<PartyManager.mainPartyManager.ammo; i++)
			{
				//int rawDamage=Mathf.RoundToInt(GaussianRandom.GetFiveStepRange(oneShotMinDamage,oneShotMaxDamage)+modifier);//Random.Range(oneShotMinDamage-0.5f,oneShotMaxDamage+0.5f)+modifier);
				actualDamage+=oneShotDamage;//Mathf.Clamp(rawDamage,oneShotMinDamage,oneShotMaxDamage);
			}
			actualDamage=Mathf.RoundToInt(actualDamage+modifier);
			//actualDamage=oneShotDamage*PartyManager.mainPartyManager.ammo;
		}
		return actualDamage;
	}
	public override int GetAmmoUsePerShot () {return ammoPerShot;}
	public override Sprite GetItemSprite (){return SpriteBase.mainSpriteBase.shotgunSprite;}
}

public class AssaultRifle:RangedWeapon
{
	public AssaultRifle()
	{
		itemName="Assault Rifle";
		oneShotDamage=90;
		accuracyMod=-0.15f;
		baseDamage=oneShotDamage*ammoPerShot;
	}
	int oneShotDamage;
	//int weaponMaxDamage=360;
	//int weaponMinDamage=330;
	//int oneShotMinDamage=110;
	//int oneShotMaxDamage=120;
	int ammoPerShot=3;
	//public override int GetMaxDamage() {return weaponMaxDamage;}
	//public override int GetMinDamage() {return weaponMinDamage;}
	public override int GetDamage(float modifier) 
	{
		int actualDamage=base.GetDamage(modifier);
		if (PartyManager.mainPartyManager.ammo<ammoPerShot)
		{
			actualDamage=0;
			for (int i=0; i<PartyManager.mainPartyManager.ammo; i++)
			{
				//int rawDamage=Mathf.RoundToInt(GaussianRandom.GetFiveStepRange(oneShotMinDamage,oneShotMaxDamage)+modifier);//Random.Range(oneShotMinDamage-0.5f,oneShotMaxDamage+0.5f)+modifier);
				actualDamage+=oneShotDamage;//Mathf.Clamp(rawDamage,oneShotMinDamage,oneShotMaxDamage);
			}
			actualDamage=Mathf.RoundToInt(actualDamage+modifier);
			//actualDamage=oneShotDamage*PartyManager.mainPartyManager.ammo;
		}
		return actualDamage;
	}
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
		return itemName+"\nDamage:"+baseDamage+"\nAccuracy modifier:"+accuracyMod+"\nStamina per hit:"+GetStaminaUse()+"\n Weight:"+GetWeight();
	}
	public int GetDamage (float moraleModifier, int additionModifier)
	{
		//float missingStaminaMod=Mathf.Min(1f,Mathf.Max((float)EncounterCanvasHandler.main.selectedMember.stamina,0.5f)/(float)GetStaminaUse());
		//float adjustedMinDamage=(GetMinDamage()+additionModifier)*missingStaminaMod;
		//float adjustedMaxDamage=(GetMaxDamage()+additionModifier)*missingStaminaMod;
		//float rawDamage=GaussianRandom.GetFiveStepRange(adjustedMinDamage,adjustedMaxDamage)+moraleModifier;

		int actualDamage=baseDamage+additionModifier;//Mathf.RoundToInt(Mathf.Clamp(rawDamage,adjustedMinDamage,adjustedMaxDamage));
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
	public Pipe()
	{
		itemName="Pipe";
		baseDamage=90;
		accuracyMod=0;
	}
	
	//public int weaponMaxDamage=110;
	//public int weaponMinDamage=70;
	int staminaUse=2;
	
	// override int GetMaxDamage() {return weaponMaxDamage;}
	//public override int GetMinDamage() {return weaponMinDamage;}
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
	public Knife()
	{
		itemName="Knife";
		baseDamage=45;
		accuracyMod=0.2f;
	}
	
	//public int weaponMaxDamage=50;
	//public int weaponMinDamage=10;
	
	int staminaUse=1;
	
	//public override int GetMaxDamage() {return weaponMaxDamage;}
	//public override int GetMinDamage() {return weaponMinDamage;}
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
	public Axe()
	{
		itemName="Axe";
		baseDamage=180;
		accuracyMod=-0.1f;
	}
	
	//public int weaponMaxDamage=200;
	//public int weaponMinDamage=160;
	//int weakDamage=3;
	int staminaUse=3;
	
	//public override int GetMaxDamage() {return weaponMaxDamage;}
	//public override int GetMinDamage() {return weaponMinDamage;}
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
	public override int GetStaminaUse() {return staminaUse;}
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.axeSprite;
	}
}

public class Bed:InventoryItem
{
	public Bed()
	{
		itemName="Bedroll";
	}
	
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.bedSprite;}
	
	public override bool UseAction(PartyMember member)
	{
		return false;
	}
	
	public override string GetMouseoverDescription ()
	{
		return itemName+"\nProvides one sleeping space";
	}
}

public class Pot:InventoryItem
{
	public Pot()
	{
		itemName="Cooking pot";
	}
	
	int nutritionAmount=50;
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.perishableFoodSprite;}
	
	public override bool UseAction(PartyMember member)
	{
		return false;
	}
	
	public override string GetMouseoverDescription ()
	{
		return itemName+"\nRestores "+nutritionAmount+" hunger";
	}
}
