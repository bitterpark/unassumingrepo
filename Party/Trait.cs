﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Trait
{
	public string name;
	public abstract void ActivatePerk(PartyMember member);
	//public virtual void DeactivatePerk() {}
	public abstract string GetMouseoverDescription();
	public System.Type oppositePerk=null;
	
	public static List<Trait> GetTraitList()
	{
		List<Trait> allTraits=new List<Trait>();
		//allPerks.Add(new Tough());
		allTraits.Add (new Scrawny());
		//allPerks.Add(new Fit());
		allTraits.Add (new Slob());
		allTraits.Add (new LeanEater());
		allTraits.Add (new BigEater());
		//allPerks.Add (new Sneaky());
		//allPerks.Add (new Loud());
		allTraits.Add (new ColdBlooded());
		allTraits.Add (new Moody());
		allTraits.Add (new Bloodthirsty());
		allTraits.Add (new Pacifist());
		allTraits.Add (new ReassuringPresence());
		allTraits.Add (new Downer());
		allTraits.Add (new PeoplePerson());
		allTraits.Add (new Antisocial());
		//allPerks.Add (new Powerhouse());
		allTraits.Add (new WeakArm());
		//allPerks.Add (new Deadeye());
		allTraits.Add (new PoorShot());
		allTraits.Add (new Medic());
		allTraits.Add (new Cook());
		//allPerks.Add (new StrongBack());
		allTraits.Add (new WeakBack());
		
		return allTraits;
	}
	//Skills
	enum Skilltree {Scout,Fighter,Carrier}
	static List<Trait> GetSkillTree(Skilltree treeType)
	{
		List<Trait> resultList=new List<Trait>();
		switch (treeType)
		{
			case Skilltree.Fighter:
			{
				resultList.Add(new Dodge());
				resultList.Add(new Technique());
				resultList.Add(new HitAndRun());
				break;
			}
			case Skilltree.Carrier:
			{
				resultList.Add(new Carrier());
				resultList.Add(new Fit());
				resultList.Add(new Tough());
				break;
			}
			case Skilltree.Scout:
			{
				resultList.Add(new Quick());
				resultList.Add(new Quiet());
				resultList.Add(new Agile());
				break;
			}
		}
		return resultList;
	}
	public static List<Trait> GetRandomSkillTree()
	{
		List<Skilltree> possibleSkillTrees=new List<Skilltree>();
		possibleSkillTrees.Add(Skilltree.Fighter);
		possibleSkillTrees.Add(Skilltree.Carrier);
		possibleSkillTrees.Add(Skilltree.Scout);
		return GetSkillTree(possibleSkillTrees[Random.Range(0,possibleSkillTrees.Count)]);
	}
	///
	//Deprecate this
	/*
	public static List<Trait> GetSpecialtyPerkList()
	{
		List<Trait> specialtyPerks=new List<Trait>();
		specialtyPerks.Add(new Scout());
		specialtyPerks.Add(new Fighter());
		specialtyPerks.Add(new Carrier());
		return specialtyPerks;
	}*/
}

public abstract class Skill:Trait
{
	public bool learned=false;
}

//SPECIALTY SKILLS
//SCOUT SKILLS
public class Quiet:Skill
{	
	public Quiet()
	{
		name="Quiet";
	}
	public override void ActivatePerk(PartyMember member)
	{
		member.isQuiet=true;
		//member.dodgeChance+=0.25f;
		
	}//+=healthUpgrade;}
	public override string GetMouseoverDescription (){return "Walks softly\n\nNo walking sound";}
}

public class Agile:Skill
{
	public Agile()
	{
		name="Agile";
	}
	public override void ActivatePerk(PartyMember member)
	{
		member.barricadeAvoidanceEnabled=true;
		//member.dodgeChance+=0.25f;
		
	}//+=healthUpgrade;}
	public override string GetMouseoverDescription (){return "Grace of a cat\n\nMoves freely through barricades";}
}
public class Quick:Skill
{
	public Quick()
	{
		name="Quick";
	}
	public override void ActivatePerk(PartyMember member)
	{
		member.extraMoveEnabled=true;
		//member.dodgeChance+=0.25f;
		
	}//+=healthUpgrade;}
	public override string GetMouseoverDescription (){return "Good cardio\n\nExtra move per turn";}
}
//FIGHTER SKILLS
/*
public class Fighter:Skill
{	
	//int meleeDamageChange=50;
	float hitChanceDelta=0.2f;
	public Fighter()
	{
		name="Powerhouse";
		oppositePerk=typeof(WeakArm);
	}
	public override void ActivatePerk(PartyMember member) {member.meleeHitchanceMod+=hitChanceDelta;}//member.meleeDamageMod+=meleeDamageChange;}
	public override string GetMouseoverDescription () {return "Hits like a truck\n\n"+hitChanceDelta+" to melee hit chance";}//+meleeDamageChange+" to melee damage";}
}*/
public class Dodge:Skill
{	
	//int meleeDamageChange=50;
	float dodgeChanceDelta=0.1f;
	public Dodge()
	{
		name="Avoidance";
	}
	public override void ActivatePerk(PartyMember member) {member.dodgeChance+=dodgeChanceDelta;}//member.meleeDamageMod+=meleeDamageChange;}
	public override string GetMouseoverDescription () {return "Ducks and weaves\n\n+"+dodgeChanceDelta+" to dodge chance";}//+meleeDamageChange+" to melee damage";}
}
public class Technique:Skill
{	
	//int meleeDamageChange=50;
	float hitChanceDelta=0.25f;
	public Technique()
	{
		name="Precision";
		oppositePerk=typeof(WeakArm);
	}
	public override void ActivatePerk(PartyMember member) {member.meleeHitchanceMod+=hitChanceDelta;}//member.meleeDamageMod+=meleeDamageChange;}
	public override string GetMouseoverDescription () {return "Fighting technique\n\n"+hitChanceDelta+" to melee hit chance";}//+meleeDamageChange+" to melee damage";}
}
public class HitAndRun:Skill
{	
	//int meleeDamageChange=50;
	public HitAndRun()
	{
		name="Hit and Run";
	}
	public override void ActivatePerk(PartyMember member) {member.hitAndRunEnabled=true;}//member.meleeDamageMod+=meleeDamageChange;}
	public override string GetMouseoverDescription () {return "Fights dirty\n\nCan move after attacking";}//+meleeDamageChange+" to melee damage";}
}

//CARRIER SKILLS
public class Carrier:Skill
{
	int inventorySizeChange=1;
	public Carrier ()
	{
		name="Strong back";
		oppositePerk=typeof(WeakBack);
	}
	public override void ActivatePerk(PartyMember member) {member.ChangeMaxCarryCapacity(inventorySizeChange);}
	public override string GetMouseoverDescription () {return "A real packmule\n\n"+inventorySizeChange+" to carry capacity";}
}

public class Tough:Skill
{
	float healthMod=1.3f;
	//string name="Tough";
	
	//public override string GetName (){return name;}
	public Tough ()
	{
		learned=false;
		name="Tough";
		oppositePerk=typeof(Scrawny);
	}
	public override void ActivatePerk(PartyMember member)
	{
		//member.maxHealth=Mathf.Max(1,Mathf.RoundToInt(member.maxHealth*(1+healthMod)));
		member.handsMaxHealth=Mathf.RoundToInt(member.handsMaxHealth*healthMod);
		member.legsMaxHealth=Mathf.RoundToInt(member.legsMaxHealth*healthMod);
		member.vitalsMaxHealth=Mathf.RoundToInt(member.vitalsMaxHealth*healthMod);
	}
	public override string GetMouseoverDescription (){return "Takes more punishment\n\nIncreased max health";}
}

public class Fit:Skill
{
	int staminaUpgrade=2;
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

//GENERIC TRAITS

public class Scrawny:Trait
{
	float healthMod=0.7f;
	// name="Scrawny";
	public Scrawny ()
	{
		name="Scrawny";
		oppositePerk=typeof(Tough);
	}
	//public override string GetName (){return name;}
	public override void ActivatePerk(PartyMember member)
	{
		member.handsMaxHealth=Mathf.RoundToInt(member.handsMaxHealth*healthMod);
		member.legsMaxHealth=Mathf.RoundToInt(member.legsMaxHealth*healthMod);
		member.vitalsMaxHealth=Mathf.RoundToInt(member.vitalsMaxHealth*healthMod);
		//member.maxHealth=Mathf.Max(1,Mathf.RoundToInt(member.maxHealth*(1+healthMod)));
	}
	public override string GetMouseoverDescription (){return "Easy to kill\n\nReduced max health";}
}

public class Slob:Trait
{
	int staminaDowngrade=2;
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

public class LeanEater:Trait
{
	int hungerReduction=20;
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

public class BigEater:Trait
{
	int hungerIncrease=25;
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
/*
public class Sneaky: Trait
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

public class Loud: Trait
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
*/
public class ColdBlooded: Trait
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

public class Moody: Trait
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

public class Bloodthirsty: Trait
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

public class Pacifist: Trait
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

public class ReassuringPresence: Trait
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

public class Downer: Trait
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

public class PeoplePerson: Trait
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
public class Antisocial: Trait
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
/*
public class Powerhouse: Perk
{
	int meleeDamageChange=10;
	public Powerhouse ()
	{
		name="Powerhouse";
		oppositePerk=typeof(WeakArm);
	}
	public override void ActivatePerk(PartyMember member) {member.meleeDamageMod+=meleeDamageChange;}
	public override string GetMouseoverDescription () {return "Hits like a truck\n\n"+meleeDamageChange+" to melee damage";}
}*/
public class WeakArm: Trait
{
	//int meleeDamageChange=-10;
	float hitChanceDelta=-0.15f;
	public WeakArm ()
	{
		name="Weak arm";
		oppositePerk=typeof(Technique);//Powerhouse);
	}
	public override void ActivatePerk(PartyMember member) {member.meleeHitchanceMod+=hitChanceDelta;}//member.meleeDamageMod+=meleeDamageChange;}
	public override string GetMouseoverDescription () {return "Hits like a pansy\n\n"+hitChanceDelta+" to melee hit chance";}//return "Hits like a pansy\n\n"+meleeDamageChange+" to melee damage";}
}

public class Deadeye: Skill
{
	//int rangedDamageChange=20;
	float rangedHitchanceChange=0.3f;
	public Deadeye ()
	{
		name="Deadeye";
		oppositePerk=typeof(PoorShot);
	}
	public override void ActivatePerk(PartyMember member) {member.rangedHitchanceMod+=rangedHitchanceChange;}
	public override string GetMouseoverDescription () {return "Always hits the bullseye\n\n"+rangedHitchanceChange+" to ranged hit chance";}//+rangedDamageChange+" to ranged damage";}
}
public class PoorShot: Trait
{
	//int rangedDamageChange=-20;
	float rangedhitchanceChange=-0.2f;
	public PoorShot ()
	{
		name="Poor shot";
		oppositePerk=typeof(Deadeye);
	}
	public override void ActivatePerk(PartyMember member) {member.rangedHitchanceMod+=rangedhitchanceChange;}
	public override string GetMouseoverDescription () {return "Can't hit the broad side of a barn\n\n"+rangedhitchanceChange+" to ranged hit chance";}//+rangedDamageChange+" to ranged damage";}
}
/*
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
}*/
public class WeakBack: Trait
{
	int inventorySizeChange=-1;
	public WeakBack ()
	{
		name="Weak back";
		oppositePerk=typeof(Carrier);//StrongBack);
	}
	public override void ActivatePerk(PartyMember member) {member.ChangeMaxCarryCapacity(inventorySizeChange);}
	public override string GetMouseoverDescription () {return "Doesn't even lift\n\n"+inventorySizeChange+" to carry capacity";}
}

public class Medic: Trait
{
	public static int healBonus=15;
	public Medic ()
	{
		name="Medic";
	}
	public override void ActivatePerk(PartyMember member) {member.isMedic=true;}
	public override string GetMouseoverDescription () {return "Has first aid training\n\nIncreases healing from medkits";}
}
public class Cook: Trait
{
	public static float hungerIncreaseMult=0.2f;
	public Cook ()
	{
		name="Cook";
	}
	public override void ActivatePerk(PartyMember member) {member.isCook=true;}
	public override string GetMouseoverDescription () {return "Does wonders with random ingredients\n\nSlows party's hunger increase";}
}

public class LockExpert: Trait
{
	public LockExpert() {name="Lock expert";}
	public override void ActivatePerk(PartyMember member) {member.isLockExpert=true;}
	public override string GetMouseoverDescription () {return "Can pick any lock\n\nOpens locks faster";}
}
