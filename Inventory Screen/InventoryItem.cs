using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class InventoryItem 
{
	//public static int foundAmmoCount=10;

	public string itemName;
	public int itemCost=100;
	protected Sprite itemSprite;

	public Sprite GetItemSprite()
	{
		return itemSprite;
	}
	public virtual bool UseAction() {return false;}
	public abstract string GetMouseoverDescription();
	
	public virtual int GetWeight() {return 1;}
}



public interface IEquipmentItem
{
	bool TryGetAddedPrepCard(out PrepCard card);
	void Unequip(PartyMember member);
	CombatCard[] GetAddedCombatCards();
	int GetStaminaModifier();
	int GetAmmoModifier();
	int GetArmorModifier();

}

public abstract class EquippableItem : InventoryItem, IEquipmentItem
{
	
	protected int ammoModifier = 0;
	protected int staminaModifier = 0;
	protected int armorModifier = 0;
	protected List<CombatCard> addedCombatCards = new List<CombatCard>();
	PrepCard addedPrepCard = null;

	public EquippableItem()
	{
		ExtenderConstructor();
		TryGeneratePrepCard();
	}

	protected abstract void ExtenderConstructor();

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
		member.UnequipItem(this);
	}

	public override string GetMouseoverDescription()
	{
		string desc = itemName;
		if (staminaModifier > 0)
			desc += "\nStamina bonus:" + staminaModifier;
		if (ammoModifier > 0)
			desc += "\nAmmo bonus:" + ammoModifier;
		if (armorModifier > 0)
			desc += "\nArmor bonus:" + armorModifier;

		if (addedCombatCards.Count > 0)
			desc += "\nAdds cards:";

		return desc;
	}
	//Only for tooltips
	public CombatCard[] GetAddedCombatCards()
	{
		return addedCombatCards.ToArray();
	}

	public int GetStaminaModifier()
	{
		return staminaModifier;
	}
	public int GetAmmoModifier()
	{
		return ammoModifier;
	}
	public int GetArmorModifier()
	{
		return armorModifier;
	}
}

public abstract class Weapon:InventoryItem, IEquipmentItem
{	
	protected int ammoModifier = 0;
	protected int staminaModifier = 0;
	protected int armorModifier = 0;

	protected List<CombatCard> addedCombatCards = new List<CombatCard>();
	PrepCard addedPrepCard = null;

	public Weapon()
	{
		ExtenderConstructor();
		TryGeneratePrepCard();
	}

	protected abstract void ExtenderConstructor();

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
		if (staminaModifier>0)
			desc += "\nStamina bonus:" + staminaModifier;
		if (ammoModifier>0)
			desc += "\nAmmo bonus:" + ammoModifier;

		if (addedCombatCards.Count>0)
			desc+="\nAdds cards:";

		return desc;
	}
	//Only for tooltips
	public CombatCard[] GetAddedCombatCards()
	{
		return addedCombatCards.ToArray();
	}

	public int GetStaminaModifier()
	{
		return staminaModifier;
	}
	public int GetAmmoModifier()
	{
		return ammoModifier;
	}
	public int GetArmorModifier()
	{
		return armorModifier;
	}
}

public abstract class RangedWeapon:Weapon
{

}

public class NineM : Weapon
{
	protected override void ExtenderConstructor()
	{
		itemName="9mm Pistol";
		itemSprite = SpriteBase.mainSpriteBase.pistol;

		itemCost = 250;

		addedCombatCards = new List<CombatCard>();
		addedCombatCards.Add(new FullMetalJacket());
		addedCombatCards.Add(new Hipfire());
		addedCombatCards.Add(new Hipfire());
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

}

public class Shotgun : Weapon
{

	protected override void ExtenderConstructor()
	{
		itemName="Shotgun";
		itemSprite = SpriteBase.mainSpriteBase.shotgun;

		itemCost = 250;



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

			damage = 50;
			ammoCost = 2;
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
}

public class AssaultRifle : Weapon
{
	protected override void ExtenderConstructor()
	{
		itemName="Assault Rifle";
		itemSprite = SpriteBase.mainSpriteBase.assaultRifle;

		itemCost = 250;

		addedCombatCards = new List<CombatCard>();
		addedCombatCards.Add(new DoubleTap());
		addedCombatCards.Add(new DoubleTap());
		addedCombatCards.Add(new FullAuto());
	}
}

//MELEE
public abstract class MeleeWeapon:Weapon
{

}

public class Pipe : Weapon
{
	protected override void ExtenderConstructor()
	{
		itemName="Pipe";
		itemSprite = SpriteBase.mainSpriteBase.pipe;

		itemCost = 250;

		addedCombatCards = new List<CombatCard>();
		addedCombatCards.Add(new Crush());
		addedCombatCards.Add(new Thwack());
		addedCombatCards.Add(new Bruise());
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

	public class Thwack : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Thwack";
			image = SpriteBase.mainSpriteBase.pipe;

			staminaCost = 3;

			damage = 40;
			targetType = TargetType.SelectEnemy;
		}
	}
	public class Bruise : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			name = "Bruise";
			image = SpriteBase.mainSpriteBase.pipe;

			staminaCost = 2;
			staminaDamage = 2;

			targetType = TargetType.SelectEnemy;
		}
	}
}

public class Knife : Weapon
{
	protected override void ExtenderConstructor()
	{
		itemName="Knife";
		itemSprite = SpriteBase.mainSpriteBase.knife;

		itemCost = 250;

		addedCombatCards = new List<CombatCard>();
		addedCombatCards.Add(new Stab());
		addedCombatCards.Add(new Slash());
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
	public class Slash : MeleeCard
	{
		protected override void ExtenderConstructor()
		{
			damage = 10;
			targetType = TargetType.SelectEnemy;

			name = "Slash";
			image = SpriteBase.mainSpriteBase.knife;
		}
	}
}

public class Axe : Weapon
{
	protected override void ExtenderConstructor()
	{
		itemName="Axe";
		itemSprite = SpriteBase.mainSpriteBase.axe;

		itemCost = 250;

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

			damage = 30;
			staminaCost = 5;
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

			damage = 50;
			staminaCost = 4;
			targetType = TargetType.SelectEnemy;
		}
	}
}


public class Stim : EquippableItem
{
	protected override void ExtenderConstructor()
	{
		itemName = "Stim";
		itemSprite = SpriteBase.mainSpriteBase.lightning;

		itemCost = 250;

		staminaModifier = 3;
	}
}

public class AmmoPouch : EquippableItem
{
	protected override void ExtenderConstructor()
	{
		itemName = "Ammo Pouch";
		itemSprite = SpriteBase.mainSpriteBase.backpack;

		itemCost = 250;

		ammoModifier = 2;
	}
}

public class ArmorVest : EquippableItem
{
	protected override void ExtenderConstructor()
	{
		itemName = "Armor Vest";
		itemSprite = SpriteBase.mainSpriteBase.armor;

		itemCost = 250;

		armorModifier = 60;
	}
}


public class Scrap:InventoryItem
{
	public Scrap()
	{
		itemName="Scrap";
		itemSprite = SpriteBase.mainSpriteBase.wrench;
		itemCost = 400;
	}
	
	public override bool UseAction()
	{
		return false;
	}
	
	public override string GetMouseoverDescription ()
	{
		return itemName+"\nFrom battlecruiser armor plates to rusty old nails, everything finds a use";
	}
}

public class ComputerParts : InventoryItem
{
	public ComputerParts()
	{
		itemName = "Computer Parts";
		itemSprite = SpriteBase.mainSpriteBase.screwdriver;
		itemCost = 500;
	}

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
		itemSprite = SpriteBase.mainSpriteBase.lightning;
	}

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
		itemSprite = SpriteBase.mainSpriteBase.money;
	}

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
