using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Perk
{
	public string name;
	public abstract void ActivatePerk(PartyMember member);
	//public virtual void DeactivatePerk() {}
	public abstract string GetMouseoverDescription();
	public System.Type oppositePerk=null;
	
	public static List<Perk> GetPerkList()
	{
		List<Perk> allPerks=new List<Perk>();
		allPerks.Add(new Tough());
		allPerks.Add (new Scrawny());
		allPerks.Add(new Fit());
		allPerks.Add (new Slob());
		allPerks.Add (new LeanEater());
		allPerks.Add (new BigEater());
		//allPerks.Add (new Sneaky());
		//allPerks.Add (new Loud());
		allPerks.Add (new ColdBlooded());
		allPerks.Add (new Moody());
		allPerks.Add (new Bloodthirsty());
		allPerks.Add (new Pacifist());
		allPerks.Add (new ReassuringPresence());
		allPerks.Add (new Downer());
		allPerks.Add (new PeoplePerson());
		allPerks.Add (new Antisocial());
		allPerks.Add (new Powerhouse());
		allPerks.Add (new WeakArm());
		allPerks.Add (new Deadeye());
		allPerks.Add (new PoorShot());
		allPerks.Add (new Medic());
		allPerks.Add (new Cook());
		allPerks.Add (new StrongBack());
		allPerks.Add (new WeakBack());
		
		return allPerks;
	}
}

public class Tough:Perk
{
	int healthUpgrade=25;
	//string name="Tough";
	
	//public override string GetName (){return name;}
	public Tough ()
	{
		name="Tough";
		oppositePerk=typeof(Scrawny);
	}
	public override void ActivatePerk(PartyMember member){member.maxHealth+=healthUpgrade;}
	public override string GetMouseoverDescription (){return "Takes more punishment\n\n+"+healthUpgrade+" max health";}
}

public class Scrawny:Perk
{
	int healthDowngrade=30;
	// name="Scrawny";
	public Scrawny ()
	{
		name="Scrawny";
		oppositePerk=typeof(Tough);
	}
	//public override string GetName (){return name;}
	public override void ActivatePerk(PartyMember member){member.maxHealth-=healthDowngrade;}
	public override string GetMouseoverDescription (){return "Easy to kill\n\n-"+healthDowngrade+" max health";}
}

public class Fit:Perk
{
	int staminaUpgrade=5;
	//string name="Enduring";
	public Fit ()
	{
		name="Fit";
		oppositePerk=typeof(Slob);
	}
	//public override string GetName (){return name;}
	public override void ActivatePerk(PartyMember member){member.baseMaxStamina+=staminaUpgrade;}
	public override string GetMouseoverDescription (){return "Can run, jump and fight for extended periods of time\n\n+"+staminaUpgrade+" max stamina";}
}

public class Slob:Perk
{
	int staminaDowngrade=5;
	//string name="Weak";
	public Slob ()
	{
		name="Slob";
		oppositePerk=typeof(Fit);
	}
	//public override string GetName (){return name;}
	public override void ActivatePerk(PartyMember member){member.baseMaxStamina-=staminaDowngrade;}
	public override string GetMouseoverDescription (){return "Can't keep up with the average\n\n-"+staminaDowngrade+" max stamina";}
}

public class LeanEater:Perk
{
	int hungerReduction=2;
	//string name="Lean Eater";
	public LeanEater ()
	{
		name="Lean eater";
		oppositePerk=typeof(BigEater);
	}
	//public override string GetName () {return name;}
	public override void ActivatePerk(PartyMember member) {member.hungerIncreasePerHour-=hungerReduction;}
	public override string GetMouseoverDescription () {return "Used to going without food\n\n+"+hungerReduction+" hunger reduction";}
}

public class BigEater:Perk
{
	int hungerIncrease=2;
	//string name="Big Eater";
	public BigEater ()
	{
		name="Big eater";
		oppositePerk=typeof(LeanEater);
	}
	//public override string GetName () {return name;}
	public override void ActivatePerk(PartyMember member) {member.hungerIncreasePerHour+=hungerIncrease;}
	public override string GetMouseoverDescription () {return "Used to large, filling meals\n\n+"+hungerIncrease+" hunger increase";}
}

public class Sneaky: Perk
{
	int enemyVisionMod=-1;
	public Sneaky ()
	{
		name="Sneaky";
		oppositePerk=typeof(Loud);
	}
	
	//public override string GetName () {return name;}
	public override void ActivatePerk(PartyMember member) {member.visibilityMod+=enemyVisionMod;}
	public override string GetMouseoverDescription () {return "Moves quietly\n\nHarder to spot when alone";}
}

public class Loud: Perk
{
	int enemyVisionMod=1;
	public Loud ()
	{
		name="Loud";
		oppositePerk=typeof(Sneaky);
	}
	//public override string GetName () {return name;}
	public override void ActivatePerk(PartyMember member) {member.visibilityMod+=enemyVisionMod;}
	public override string GetMouseoverDescription () {return "Like a bull in a china shop\n\nMakes the party easier to spot";}
}

public class ColdBlooded: Perk
{
	float moraleModifierMult=2f;
	public ColdBlooded ()
	{
		name="Cold blooded";
		oppositePerk=typeof(Moody);
	}
	public override void ActivatePerk(PartyMember member) {member.moraleDamageMod*=moraleModifierMult;}
	public override string GetMouseoverDescription () {return "Always keeps a cool head\n\nMorale has much less influence on damage";}
}

public class Moody: Perk
{
	float moraleModifierMult=0.5f;
	public Moody ()
	{
		name="Moody";
		oppositePerk=typeof(ColdBlooded);
	}
	public override void ActivatePerk(PartyMember member) {member.moraleDamageMod*=moraleModifierMult;}
	public override string GetMouseoverDescription () {return "Led by emotions\n\nMorale has much more influence on damage";}
}

public class Bloodthirsty: Perk
{
	int moraleFromKill=3;
	public Bloodthirsty ()
	{
		name="Bloodthirsty";
		oppositePerk=typeof(Pacifist);
	}
	public override void ActivatePerk(PartyMember member) {member.moraleChangeFromKills=moraleFromKill;}
	public override string GetMouseoverDescription () {return "Takes joy in violence\n\nKills increases morale";}
}

public class Pacifist: Perk
{
	int moraleFromKill=-5;
	public Pacifist ()
	{
		name="Pacifist";
		oppositePerk=typeof(Bloodthirsty);
	}
	public override void ActivatePerk(PartyMember member) {member.moraleChangeFromKills=moraleFromKill;}
	public override string GetMouseoverDescription () {return "Dislikes killing\n\nKills lowers morale";}
}

public class ReassuringPresence: Perk
{
	float moraleRestoreMult=2f;
	public ReassuringPresence ()
	{
		name="Reassuring presence";
		oppositePerk=typeof(Downer);
	}
	public override void ActivatePerk(PartyMember member) {member.moraleRestorePerHour=(int)(member.moraleRestorePerHour*moraleRestoreMult);}
	public override string GetMouseoverDescription () {return "Takes the edge off everyone\n\nImproves morale recovery";}
}

public class Downer: Perk
{
	float moraleDecayMult=2f;
	public Downer ()
	{
		name="Downer";
		oppositePerk=typeof(ReassuringPresence);
	}
	public override void ActivatePerk(PartyMember member) {member.moraleDecayPerHour=(int)(member.moraleDecayPerHour*moraleDecayMult);}
	public override string GetMouseoverDescription () {return "Keeps everyone down\n\nIncreases morale decay";}
}

public class PeoplePerson: Perk
{
	float friendChanceChange=0.2f;
	public PeoplePerson ()
	{
		name="People person";
		oppositePerk=typeof(Antisocial);
	}
	public override void ActivatePerk(PartyMember member) {member.friendshipChance+=friendChanceChange;}
	public override string GetMouseoverDescription () {return "Easily makes friends\n\nMore likely to make friends";}
}
public class Antisocial: Perk
{
	float friendChanceChange=-0.2f;
	public Antisocial ()
	{
		name="Antisocial";
		oppositePerk=typeof(PeoplePerson);
	}
	public override void ActivatePerk(PartyMember member) {member.friendshipChance+=friendChanceChange;}
	public override string GetMouseoverDescription () {return "Doesn't like people\n\nMore likely to make enemies";}
}

public class Powerhouse: Perk
{
	int meleeDamageChange=1;
	public Powerhouse ()
	{
		name="Powerhouse";
		oppositePerk=typeof(WeakArm);
	}
	public override void ActivatePerk(PartyMember member) {member.meleeDamageMod+=meleeDamageChange;}
	public override string GetMouseoverDescription () {return "Hits like a truck\n\n"+meleeDamageChange+" to melee damage";}
}
public class WeakArm: Perk
{
	int meleeDamageChange=-1;
	public WeakArm ()
	{
		name="Weak arm";
		oppositePerk=typeof(Powerhouse);
	}
	public override void ActivatePerk(PartyMember member) {member.meleeDamageMod+=meleeDamageChange;}
	public override string GetMouseoverDescription () {return "Hits like a pansy\n\n"+meleeDamageChange+" to melee damage";}
}

public class Deadeye: Perk
{
	int rangedDamageChange=2;
	public Deadeye ()
	{
		name="Deadeye";
		oppositePerk=typeof(PoorShot);
	}
	public override void ActivatePerk(PartyMember member) {member.rangedDamageMod+=rangedDamageChange;}
	public override string GetMouseoverDescription () {return "Always hits the bullseye\n\n"+rangedDamageChange+" to ranged damage";}
}
public class PoorShot: Perk
{
	int rangedDamageChange=-2;
	public PoorShot ()
	{
		name="Poor shot";
		oppositePerk=typeof(Deadeye);
	}
	public override void ActivatePerk(PartyMember member) {member.rangedDamageMod+=rangedDamageChange;}
	public override string GetMouseoverDescription () {return "Can't hit the broad side of a barn\n\n"+rangedDamageChange+" to ranged damage";}
}
public class StrongBack: Perk
{
	int inventorySizeChange=1;
	public StrongBack ()
	{
		name="Strong back";
		oppositePerk=typeof(WeakBack);
	}
	public override void ActivatePerk(PartyMember member) {member.maxCarryCapacity+=inventorySizeChange;}
	public override string GetMouseoverDescription () {return "A real packmule\n\n"+inventorySizeChange+" to carry capacity";}
}
public class WeakBack: Perk
{
	int inventorySizeChange=-1;
	public WeakBack ()
	{
		name="Weak back";
		oppositePerk=typeof(StrongBack);
	}
	public override void ActivatePerk(PartyMember member) {member.maxCarryCapacity+=inventorySizeChange;}
	public override string GetMouseoverDescription () {return "Doesn't even lift\n\n"+inventorySizeChange+" to carry capacity";}
}

public class Medic: Perk
{
	public static int healBonus=10;
	public Medic ()
	{
		name="Medic";
	}
	public override void ActivatePerk(PartyMember member) {member.isMedic=true;}
	public override string GetMouseoverDescription () {return "Has first aid training\n\nIncreases healing from medkits";}
}
public class Cook: Perk
{
	public static float hungerIncreaseMult=0.5f;
	public Cook ()
	{
		name="Cook";
	}
	public override void ActivatePerk(PartyMember member) {member.isCook=true;}
	public override string GetMouseoverDescription () {return "Does wonders with random ingredients\n\nSlows party's hunger increase";}
}

public class LockExpert: Perk
{
	public LockExpert() {name="Lock expert";}
	public override void ActivatePerk(PartyMember member) {member.isLockExpert=true;}
	public override string GetMouseoverDescription () {return "Can pick any lock\n\nOpens locks faster";}
}

