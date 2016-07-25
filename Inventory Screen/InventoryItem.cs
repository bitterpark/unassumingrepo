using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class InventoryItem 
{
	//public static int foundAmmoCount=10;
	
	public string itemName;
	public int itemCost=100;
	
	public abstract Sprite GetItemSprite();
	public virtual bool UseAction() {return false;}
	public abstract string GetMouseoverDescription();
	
	public virtual int GetWeight() {return 1;}
	

	public enum LootMetatypes {Medical,FoodItems,Melee,Guns,Equipment,Radio,Salvage,ApartmentSalvage,WarehouseSalvage,Gear}
	
	//Deprecated, remove later!!!
	/*
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
	}*/
	
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
			case LootMetatypes.ApartmentSalvage:
			{
				metatypeDesc="Firewood";
				break;
			}
			case LootMetatypes.WarehouseSalvage:
			{
				metatypeDesc="Scrap";
				break;
			}
			case LootMetatypes.Gear:
			{
				metatypeDesc="Gear";
				break;
			}
		}
		return metatypeDesc;
	}


	//Deprecated (un-deprecated as of right now) (used for crafting)
	public enum LootItems 
	{Medkits,Bandages,Pills
	,Gas
	,Food,Junkfood,Cookedfood/*,PerishableFood*/
	,Ammopack,Ammo,Firecracker
	,Firewood,CampBarricade
	,Flashlight,Radio,Bed,Backpack
	,Scrap,Gunpowder, ComputerParts, FusionModule
	,SettableTrap
	,AssaultRifle,Shotgun,NineM,Pipegun
	,Pipe,Knife,Axe
	,ArmorVest}
	public static InventoryItem GetLootingItem(LootItems itemType)
	{
		InventoryItem lootedItem=null;
		switch (itemType)
		{
			//MELEE WEAPONS
			case LootItems.Pipe:{lootedItem=new Pipe(); break;}
			case LootItems.Knife:{lootedItem=new Knife(); break;}
			case LootItems.Axe:{lootedItem=new Axe(); break;}
			//RANGED WEAPONS
			case LootItems.NineM:{lootedItem=new NineM(); break;}
			case LootItems.Shotgun: {lootedItem=new Shotgun(); break;}
			case LootItems.AssaultRifle:{lootedItem=new AssaultRifle(); break;}
			case LootItems.Pipegun:{lootedItem=new Pipegun(); break;}
			//MISC
			//INGREDIENTS
			case LootItems.Scrap:{lootedItem=new Scrap(); break;}
			case LootItems.Gunpowder:{lootedItem=new Gunpowder(); break;}
			case LootItems.ComputerParts: { lootedItem = new ComputerParts(); break;}
			case LootItems.FusionModule: { lootedItem = new FusionCore(); break; }
		}
		return lootedItem;
	}
}


public abstract class EquippableItem:InventoryItem
{
	public List<CombatCard> addedCombatCards = new List<CombatCard>();
	
	public abstract void EquipEffect(PartyMember member);
	public abstract void UnequipEffect(PartyMember member);

	public override string GetMouseoverDescription()
	{
		string desc="";
		if (addedCombatCards.Count > 0)
		{
			desc += "\nAdds cards:";
			foreach (CombatCard card in addedCombatCards)
			{
				desc += "\n-" + card.name;
			}
		}
		return desc;
	}
	//public override bool UseAction(PartyMember member) {return false;}
}

public abstract class Weapon:InventoryItem
{	
	public int baseDamage;
	public int weight=1;
	public float accuracyMod;

	public int ammoBonus = 0;
	public int staminaBonus = 0;

	protected List<CombatCard> addedCombatCards = new List<CombatCard>();
	PrepCard addedPrepCard = null;

	public Weapon()
	{
		ExtenderConstructor();
		TryGeneratePrepCard();
	}

	protected abstract void ExtenderConstructor();

	public override int GetWeight()
	{
		return weight;
	}

	//damage for fight calculation
	public virtual int GetDamage(float modifier) 
	{
		//int rawDamage=Mathf.RoundToInt(GaussianRandom.GetFiveStepRange(GetMinDamage(),GetMaxDamage())+modifier);//Random.Range(GetMinDamage()-0.5f,GetMaxDamage()+0.5f)+modifier);
		int actualDamage=Mathf.RoundToInt(baseDamage+modifier);//Mathf.Clamp(rawDamage,GetMinDamage(),GetMaxDamage());
		return actualDamage;
	}

	
	void TryGeneratePrepCard()
	{
		if (addedCombatCards.Count > 0)
			addedPrepCard = new CustomPrepCard(itemName, "Add cards to your deck", GetItemSprite(), addedCombatCards);
		else
			addedPrepCard = null;
	}

	public bool TryGetAddedPrepCard(out PrepCard card)
	{
		card = addedPrepCard;
		if (card == null)
			return false;
		else
			return true;
	}

	public virtual void Unequip(PartyMember member)//virtual void Unequip (PartyMember member) {}
	{
		member.UnequipWeapon(this);
	}

	public override string GetMouseoverDescription()
	{
 		string desc = itemName;
		if (staminaBonus>0)
			desc += "\nStamina bonus:" + staminaBonus;
		if (ammoBonus>0)
			desc += "\nAmmo bonus:" + ammoBonus;

		if (addedCombatCards.Count>0)
			desc+="\nAdds cards:";

		return desc;
	}
	//Only for tooltips
	public CombatCard[] GetAddedCombatCards()
	{
		return addedCombatCards.ToArray();
	}
}

public abstract class RangedWeapon:Weapon
{
	public abstract int GetAmmoUsePerShot();
	//public override bool UseAction (PartyMember member) {return false;}
}

public class NineM:RangedWeapon
{
	protected override void ExtenderConstructor()
	{
		itemName="9mm Pistol";
		baseDamage=45;
		accuracyMod=0.1f;

		itemCost = 250;

		ammoBonus = 2;
		addedCombatCards = new List<CombatCard>();
		addedCombatCards.Add(new Sidearm());
		addedCombatCards.Add(new Sidearm());
		addedCombatCards.Add(new Hipfire());
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

	protected override void ExtenderConstructor()
	{
		itemName="Shotgun";
		oneShotDamage=45;
		accuracyMod=0;
		baseDamage=oneShotDamage*ammoPerShot;

		itemCost = 250;

		ammoBonus = 2;
		addedCombatCards = new List<CombatCard>();
		addedCombatCards.Add(new Buckshot());
		addedCombatCards.Add(new Spread());
		addedCombatCards.Add(new PointBlank());
	}

	public class Spread : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Spread";
			description = "Targets all enemies";
			image = SpriteBase.mainSpriteBase.buckshot;

			damage = 40;
			ammoCost = 3;
			targetType = TargetType.AllEnemies;
		}
	}

	public class PointBlank : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Point Blank";
			description = "";
			image = SpriteBase.mainSpriteBase.buckshot;

			damage = 30;
			ammoCost = 1;
			targetType = TargetType.SelectEnemy;
		}
	}

	public class Buckshot : RangedCard
	{
		protected override void ExtenderConstructor()
		{
			damage = 20;
			unarmoredBonusDamage = 40;
			ammoCost = 2;
			targetType = TargetType.SelectEnemy;
			
			name = "Buckshot";
			description = "If target has no armor, deal " + unarmoredBonusDamage + " extra damage";
			image = SpriteBase.mainSpriteBase.buckshot;
	
		}
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
	protected override void ExtenderConstructor()
	{
		itemName="Assault Rifle";
		oneShotDamage=60;
		accuracyMod=-0.1f;
		baseDamage=oneShotDamage*ammoPerShot;

		itemCost = 250;

		ammoBonus = 2;
		addedCombatCards = new List<CombatCard>();
		addedCombatCards.Add(new FullAuto());
		addedCombatCards.Add(new FullAuto());
		addedCombatCards.Add(new FullMetalJacket());
	}

	public class FullMetalJacket : RangedCard
	{
		protected override void ExtenderConstructor()
		{
			name = "FMJ";
			description = "Ignores armor";
			image = SpriteBase.mainSpriteBase.bullet;

			damage = 50;
			ammoCost = 2;
			staminaCost = 1;
			ignoresArmor = true;
			targetType = TargetType.SelectEnemy;
		}
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

public class Pipegun:RangedWeapon
{
	protected override void ExtenderConstructor()
	{
		itemName="Pipegun";
		baseDamage=30;
		accuracyMod=0f;
		weight=0;
	}
	

	int ammoPerShot=1;
	public override int GetAmmoUsePerShot (){return ammoPerShot;}
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.pipegunSprite;
	}
	/*
	public override string GetMouseoverDescription ()
	{
		return name+"\nDamage:"+weaponMaxDamage+"\nAmmo per shot:"+ammoPerShot;
	}*/
}

//MELEE
public abstract class MeleeWeapon:Weapon
{
	public abstract int GetStaminaUse();
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
	protected override void ExtenderConstructor()
	{
		itemName="Pipe";
		baseDamage=90;
		accuracyMod=0;

		itemCost = 250;

		staminaBonus = 3;
		addedCombatCards = new List<CombatCard>();
		addedCombatCards.Add(new Crush());
		addedCombatCards.Add(new Bump());
		addedCombatCards.Add(new Bump());
	}

	public class Crush : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			damage = 20;
			unarmoredBonusDamage = 20;
			staminaCost = 2;
			targetType = TargetType.SelectEnemy;
			
			name = "Crush";
			description = "If target has no armor, deal " + unarmoredBonusDamage + " extra damage";
			image = SpriteBase.mainSpriteBase.pipe;
		}
	}

	public class Bump : MeleeCard
	{

		protected override void ExtenderConstructor()
		{
			name = "Bump";
			image = SpriteBase.mainSpriteBase.pipe;

			damage = 10;
			targetType = TargetType.SelectEnemy;
		}
	}

	//public int weaponMaxDamage=110;
	//public int weaponMinDamage=70;
	int staminaUse=2;
	
	// override int GetMaxDamage() {return weaponMaxDamage;}
	//public override int GetMinDamage() {return weaponMinDamage;}
	public override int GetStaminaUse() {return staminaUse;}
	
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.pipe;
	}
	/*
	public override string GetMouseoverDescription ()
	{
		return name+"\nDamage:"+weaponMaxDamage;
	}*/
}

public class Knife:MeleeWeapon
{
	protected override void ExtenderConstructor()
	{
		itemName="Knife";
		baseDamage=45;
		accuracyMod=0.2f;
		weight=0;

		itemCost = 250;

		staminaBonus = 3;
		addedCombatCards = new List<CombatCard>();
		addedCombatCards.Add(new Stab());
		addedCombatCards.Add(new Stab());
		addedCombatCards.Add(new Throw());
	}

	public class Stab : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			damage = 20;
			staminaCost = 3;
			ignoresArmor = true;
			targetType = TargetType.SelectEnemy;
			
			name = "Stab";
			description = "Ignores armor";
			image = SpriteBase.mainSpriteBase.knife;
		}
	}

	//public int weaponMaxDamage=50;
	//public int weaponMinDamage=10;
	
	int staminaUse=1;
	
	//public override int GetMaxDamage() {return weaponMaxDamage;}
	//public override int GetMinDamage() {return weaponMinDamage;}
	public override int GetStaminaUse() {return staminaUse;}
	
	public override Sprite GetItemSprite ()
	{
		return SpriteBase.mainSpriteBase.knife;
	}
	/*
	public override string GetMouseoverDescription ()
	{
		return name+"\nDamage:"+weaponMaxDamage;
	}*/
}

public class Axe:MeleeWeapon
{
	protected override void ExtenderConstructor()
	{
		itemName="Axe";
		baseDamage=180;
		accuracyMod=-0.15f;

		itemCost = 250;

		staminaBonus = 3;
		addedCombatCards = new List<CombatCard>();
		addedCombatCards.Add(new Cleave());
		addedCombatCards.Add(new Chop());
		addedCombatCards.Add(new Chop());
	}

	public class Cleave : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Cleave";
			description = "Targets all enemies";
			image = SpriteBase.mainSpriteBase.axe;

			damage = 40;
			staminaCost = 6;
			targetType = TargetType.AllEnemies;
		}
	}

	public class Chop : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Chop";
			description = "";
			image = SpriteBase.mainSpriteBase.axe;

			damage = 60;
			staminaCost = 5;
			targetType = TargetType.SelectEnemy;
		}
	}

	//public int weaponMaxDamage=200;
	//public int weaponMinDamage=160;
	//int weakDamage=3;
	int staminaUse=4;
	
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
		return SpriteBase.mainSpriteBase.axe;
	}
}


public class Scrap:InventoryItem
{
	public Scrap()
	{
		itemName="Scrap";
		itemCost = 400;
	}
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.scrapSprite;}
	
	public override bool UseAction()
	{
		return false;
	}
	
	public override string GetMouseoverDescription ()
	{
		return itemName+"\nFrom battlecruiser armor plates to rusty old nails, everything finds a use";
	}
}

public class Gunpowder:InventoryItem
{
	public Gunpowder()
	{
		itemName="Gunpowder";
	}
	public override Sprite GetItemSprite() {return SpriteBase.mainSpriteBase.gunpowderSprite;}
	
	public override bool UseAction()
	{
		return false;
	}
	
	public override string GetMouseoverDescription ()
	{
		return itemName+"\nUsed for crafting bullets";
	}
}

public class ComputerParts : InventoryItem
{
	public ComputerParts()
	{
		itemName = "Computer Parts";
		itemCost = 500;
	}
	public override Sprite GetItemSprite() { return SpriteBase.mainSpriteBase.computerParts; }

	public override bool UseAction()
	{
		return false;
	}

	public override string GetMouseoverDescription()
	{
		return itemName + "\nComputing circuits for space travel and related tasks";
	}
}

public class FusionCore : InventoryItem
{
	public FusionCore()
	{
		itemName = "Fusion Core";
	}
	public override Sprite GetItemSprite() { return SpriteBase.mainSpriteBase.lightning; }

	public override bool UseAction()
	{
		return false;
	}

	public override string GetMouseoverDescription()
	{
		return itemName + "\nThe central component of every Drive, engineered from alien technology that is still not fully understood.";
	}
}

public class FakeMoney : InventoryItem
{
	int incomeValue = 1300;
	public FakeMoney()
	{
		itemName = "Fake Money";
		itemCost = 500;
	}
	public override Sprite GetItemSprite() { return SpriteBase.mainSpriteBase.money; }

	public override bool UseAction()
	{
		TownManager.main.AddFutureIncome(incomeValue,"Fake money");
		return true;
	}

	public override string GetMouseoverDescription()
	{
		string desc = itemName + "\nStacks of counterfeit banknotes and chits. It will take some time to find enough suckers to offload all this.";
		desc += "\nUse: Gain " + incomeValue+" income";
		return desc;
	}
}
