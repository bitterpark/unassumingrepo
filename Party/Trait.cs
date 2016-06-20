using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Trait
{
	public string name;
	public virtual void ActivateEffect(PartyMember member)
	{
		if (addedCombatCards.Count > 0)
			member.GetCombatDeck().AddCards(addedCombatCards.ToArray());
	}
	//public virtual void DeactivatePerk() {}
	public abstract string GetMouseoverDescription();
	public System.Type oppositePerk=null;

	protected List<CombatCard> addedCombatCards = new List<CombatCard>();

	public List<CombatCard> GetAddedCombatCards()
	{
		return addedCombatCards;
	}

	public static List<Trait> GetTraitList()
	{
		List<Trait> allTraits=new List<Trait>();

		allTraits.Add (new Scrawny());
		allTraits.Add (new Slob());
		allTraits.Add (new LeanEater());
		allTraits.Add (new BigEater());
		allTraits.Add (new Bloodthirsty());
		allTraits.Add (new Pacifist());
		allTraits.Add (new ReassuringPresence());
		allTraits.Add (new Downer());
		allTraits.Add (new PeoplePerson());
		allTraits.Add (new Antisocial());
		allTraits.Add (new WeakArm());
		allTraits.Add (new PoorShot());
		allTraits.Add (new WeakBack());
		allTraits.Add (new Loner());
		allTraits.Add (new SocialAnimal());
		allTraits.Add (new Violent());
		allTraits.Add (new Kleptomaniac());
		allTraits.Add(new Slow());
		/*
		allTraits.Add(new Loner());
		allTraits.Add(new SocialAnimal());
		*/
		return allTraits;
	}
	//Skills
	enum Skilltree {Scout,Fighter,Carrier} //Soldier, Bandit, Gunman, Mercenary
	static List<Trait> GetSkillTree(Skilltree treeType)
	{
		List<Trait> resultList=new List<Trait>();
		switch (treeType)
		{
			case Skilltree.Fighter:
			{
				resultList.Add(new Dodge());
				resultList.Add(new Technique());
				resultList.Add(new Deadeye());
				//resultList.Add(new HitAndRun());
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
				//resultList.Add(new Quick());
				resultList.Add(new Medic());
				resultList.Add(new Quiet());
				resultList.Add(new Scout());
				//resultList.Add(new Agile());
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

	public static List<Trait> GenerateRandomSkillTree(int treeSize)
	{
		List<Trait> allSkills=new List<Trait>();
		allSkills.AddRange(GetSkillTree(Skilltree.Carrier));
		allSkills.AddRange(GetSkillTree(Skilltree.Fighter));
		allSkills.AddRange(GetSkillTree(Skilltree.Scout));

		List<Trait> randomTree=new List<Trait>();
		int skillsRequired=treeSize;
		int skillsGained=0;
		while (skillsGained<skillsRequired && allSkills.Count>0)
		{
			Trait randomlyPickedSkill=allSkills[Random.Range(0,allSkills.Count)];
			allSkills.Remove(randomlyPickedSkill);
			randomTree.Add(randomlyPickedSkill);
			skillsGained++;
		}
		return randomTree;
	}

	public static List<Trait> GenerateLevelupSkills(List<Trait>existingTraits, int requiredSkillCount)
	{
		List<Trait> allPossibleSkills=new List<Trait>();
		allPossibleSkills.AddRange(GetSkillTree(Skilltree.Carrier));
		allPossibleSkills.AddRange(GetSkillTree(Skilltree.Fighter));
		allPossibleSkills.AddRange(GetSkillTree(Skilltree.Scout));

		foreach(Trait existingTrait in existingTraits)
		{
			bool learned=true;
			if (existingTrait.GetType().BaseType==typeof(Skill))
			{
				Skill checkedSkill=existingTrait as Skill;
				if (!checkedSkill.learned) learned=false;
			}
			//Check to see if the trait from a member trait list is learned or not
			if (learned)
			{
				foreach (Trait possibleSkill in new List<Trait>(allPossibleSkills))
				{
					if (possibleSkill.GetType()==existingTrait.GetType() || possibleSkill.GetType()==existingTrait.oppositePerk) allPossibleSkills.Remove(possibleSkill);
				}
			}
		}
		List<Trait> generatedSet=new List<Trait>();
		while (generatedSet.Count<requiredSkillCount && allPossibleSkills.Count>0)
		{
			int addIndex=Random.Range(0,allPossibleSkills.Count);
			generatedSet.Add(allPossibleSkills[addIndex]);
			allPossibleSkills.RemoveAt(addIndex);
		}
		return generatedSet;
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
	float probIncreaseMult=0.5f;
	public Quiet()
	{
		name="Quiet";
	}
	public override void  ActivateEffect(PartyMember member)
	{
		base.ActivateEffect(member);
		member.movingEnemySpawnProbIncrease*=probIncreaseMult;
		//member.dodgeChance+=0.25f;
		
	}//+=healthUpgrade;}
	public override string GetMouseoverDescription (){return "Walks softly\n\nHalves movement sound";}
}
//Currently unused
public class Agile:Skill
{
	public Agile()
	{
		name="Agile";
	}
	public override void ActivateEffect(PartyMember member)
	{
		base.ActivateEffect(member);
		member.barricadeVaultCost=0;//.barricadeAvoidanceEnabled=true;
		//member.dodgeChance+=0.25f;
		
	}//+=healthUpgrade;}
	public override string GetMouseoverDescription (){return "Grace of a cat\n\nMoves freely through barricades";}
}

public class Medic:Skill
{
	public static float healMultiplier=1.5f;
	public Medic ()
	{
		name="Medic";
	}
	public override void ActivateEffect(PartyMember member) 
	{
		base.ActivateEffect(member);
		member.isMedic=true;
	}
	public override string GetMouseoverDescription () {return "Has first aid training\n\nIncreases the effectiveness of medical supplies by half";}
}
//Currently unused
public class Quick:Skill
{
	public Quick()
	{
		name="Quick";
	}
	public override void ActivateEffect(PartyMember member)
	{
		base.ActivateEffect(member);
		member.extraMoveEnabled=true;
		//member.dodgeChance+=0.25f;
		
	}//+=healthUpgrade;}
	public override string GetMouseoverDescription (){return "Good cardio\n\nExtra move per turn";}
}

public class Scout:Skill
{
	int moveCostReduction=1;
	public Scout()
	{
		name="Scout";
		oppositePerk=typeof(Slow);
	}
	public override void ActivateEffect(PartyMember member)
	{
		base.ActivateEffect(member);
		member.currentFatigueMoveModifier-=moveCostReduction;//member.=0;//.barricadeAvoidanceEnabled=true;
		//member.dodgeChance+=0.25f;
		
	}//+=healthUpgrade;}
	public override string GetMouseoverDescription (){return "Trailblazer\n\nFatigue from map movement reduced by "+moveCostReduction;}
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
	public override void ActivateEffect(PartyMember member) 
	{
		base.ActivateEffect(member);
		member.IncrementMaxDodgeChance(dodgeChanceDelta);
	}
	public override string GetMouseoverDescription () {return "Ducks and weaves\n\n+"+(dodgeChanceDelta*100)+"% to dodge chance";}//+meleeDamageChange+" to melee damage";}
}
public class Technique:Skill
{	
	//int meleeDamageChange=50;
	float hitChanceDelta=0.15f;
	public Technique()
	{
		name="Precision";
		oppositePerk=typeof(WeakArm);
	}
	public override void ActivateEffect(PartyMember member) 
	{
		base.ActivateEffect(member);
		member.meleeHitchanceMod+=hitChanceDelta;
	}
	public override string GetMouseoverDescription () {return "Fighting technique\n\n"+hitChanceDelta+" to melee hit chance";}//+meleeDamageChange+" to melee damage";}
}
//Currently unused
public class HitAndRun:Skill
{	
	//int meleeDamageChange=50;
	public HitAndRun()
	{
		name="Hit and Run";
	}
	public override void ActivateEffect(PartyMember member) {member.hitAndRunEnabled=true;}//member.meleeDamageMod+=meleeDamageChange;}
	public override string GetMouseoverDescription () {return "Fights dirty\n\nCan move after attacking";}//+meleeDamageChange+" to melee damage";}
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
	public override void ActivateEffect(PartyMember member) 
	{
		base.ActivateEffect(member);
		member.rangedHitchanceMod+=rangedHitchanceChange;
	}
	public override string GetMouseoverDescription () {return "Always hits the bullseye\n\n"+rangedHitchanceChange+" to ranged hit chance";}//+rangedDamageChange+" to ranged damage";}
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
	public override void ActivateEffect(PartyMember member) 
	{
		base.ActivateEffect(member);
		member.ChangeMaxCarryCapacity(inventorySizeChange);
	}
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
	public override void ActivateEffect(PartyMember member)
	{
		base.ActivateEffect(member);
		member.handsMaxHealth=Mathf.RoundToInt(member.handsMaxHealth*healthMod);
		member.legsMaxHealth=Mathf.RoundToInt(member.legsMaxHealth*healthMod);
		member.vitalsMaxHealth=Mathf.RoundToInt(member.vitalsMaxHealth*healthMod);
	}
	public override string GetMouseoverDescription (){return "Takes more punishment\n\nIncreased max health";}
}

public class Fit:Skill
{
	int staminaUpgrade=4;
	//string name="Enduring";
	public Fit ()
	{
		name="Fit";
		oppositePerk=typeof(Slob);
	}
	//public override string GetName (){return name;}
	public override void ActivateEffect(PartyMember member)
	{
		base.ActivateEffect(member);
		member.baseMaxStamina+=staminaUpgrade;
	}
	public override string GetMouseoverDescription (){return "Can run, jump and fight for extended periods of time\n\n+"+staminaUpgrade+" max stamina";}
}

//GENERIC SKILLS


//GENERIC TRAITS

public class Slow:Trait
{
	int moveCostIncrease=1;
	public Slow()
	{
		name="Slow";
		oppositePerk=typeof(Scout);
	}
	public override void ActivateEffect(PartyMember member)
	{
		member.currentFatigueMoveModifier+=moveCostIncrease;//member.=0;//.barricadeAvoidanceEnabled=true;
		//member.dodgeChance+=0.25f;
		
	}//+=healthUpgrade;}
	public override string GetMouseoverDescription (){return "Bad at navigating\n\nFatigue from map movement increased by "+moveCostIncrease;}
}

public class Scrawny:Trait
{
	float healthMod=0.8f;
	// name="Scrawny";
	public Scrawny ()
	{
		name="Scrawny";
		oppositePerk=typeof(Tough);
	}
	//public override string GetName (){return name;}
	public override void ActivateEffect(PartyMember member)
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
	public override void ActivateEffect(PartyMember member){member.baseMaxStamina-=staminaDowngrade;}
	public override string GetMouseoverDescription (){return "Can't keep up with the average\n\n-"+staminaDowngrade+" max stamina";}
}

public class LeanEater:Trait
{
	int hungerReduction=25;
	//string name="Lean Eater";
	public LeanEater ()
	{
		name="Lean eater";
		oppositePerk=typeof(BigEater);
	}
	//public override string GetName () {return name;}
	public override void ActivateEffect(PartyMember member) {member.hungerIncreasePerHour-=hungerReduction;}
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
	public override void ActivateEffect(PartyMember member) {member.hungerIncreasePerHour+=hungerIncrease;}
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
/*
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
}*/

public class Bloodthirsty: Trait
{
	int moraleFromKill=10;
	public Bloodthirsty ()
	{
		name="Bloodthirsty";
		oppositePerk=typeof(Pacifist);
	}
	public override void ActivateEffect(PartyMember member) {member.moraleChangeFromKills=moraleFromKill;}
	public override string GetMouseoverDescription () {return "Takes joy in violence\n\nKills increases morale";}
}

public class Pacifist: Trait
{
	int moraleFromKill=-10;
	public Pacifist ()
	{
		name="Pacifist";
		oppositePerk=typeof(Bloodthirsty);
	}
	public override void ActivateEffect(PartyMember member) {member.moraleChangeFromKills=moraleFromKill;}
	public override string GetMouseoverDescription () {return "Dislikes killing\n\nKills lowers morale";}
}

public class ReassuringPresence: Trait
{
	public static float moraleRestoreMult=2f;
	public ReassuringPresence ()
	{
		name="Reassuring presence";
		oppositePerk=typeof(Downer);
	}
	public override void ActivateEffect(PartyMember mainMember) 
	{
		//foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
		//member.moraleRestorePerHour=(int)(member.moraleRestorePerHour*moraleRestoreMult);
		mainMember.isReassuring=true;
	}
	public override string GetMouseoverDescription () {return "Takes the edge off everyone\n\nImproves party morale recovery";}
}

public class Downer: Trait
{
	public static float moraleDecayMult=2f;
	public Downer ()
	{
		name="Downer";
		oppositePerk=typeof(ReassuringPresence);
	}
	public override void ActivateEffect(PartyMember mainMember) 
	{
		//foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
		//member.moraleRestorePerHour=(int)(member.moraleDecayPerHour*moraleDecayMult);
		mainMember.isDowner=true;
	}
	public override string GetMouseoverDescription () {return "Keeps everyone down\n\nIncreases party morale decay";}
}

public class PeoplePerson: Trait
{
	float friendChanceChange=0.3f;
	public PeoplePerson ()
	{
		name="People person";
		oppositePerk=typeof(Antisocial);
	}
	public override void ActivateEffect(PartyMember member) {member.baseRelationshipChanceModifier+=friendChanceChange;}
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
	public override void ActivateEffect(PartyMember member) {member.baseRelationshipChanceModifier+=friendChanceChange;}
	public override string GetMouseoverDescription () {return "Doesn't like people\n\nMore likely to make enemies";}
}

public class SocialAnimal: Trait
{
	int soloMoraleChange=-10;
	int teamMoraleChange=10;

	public SocialAnimal ()
	{
		name="Social Animal";
		oppositePerk=typeof(Loner);
	}
	public override void ActivateEffect(PartyMember member) 
	{
		member.aloneMoraleMod=soloMoraleChange;
		member.inTeamMoraleMod=teamMoraleChange;
	}
	public override string GetMouseoverDescription () {return "Afraid of being alone\n\nLoses morale on solo missions, gains morale on team missions";}
}

public class Loner: Trait
{
	int soloMoraleChange=10;
	int teamMoraleChange=-10;

	public Loner ()
	{
		name="Loner";
		oppositePerk=typeof(SocialAnimal);
	}
	public override void ActivateEffect(PartyMember member) 
	{
		member.aloneMoraleMod=soloMoraleChange;
		member.inTeamMoraleMod=teamMoraleChange;
	}
	public override string GetMouseoverDescription () {return "Values alone time more than safety\n\nGains morale on solo missions, loses morale on team missions";}
}

public class Violent: Trait
{

	public Violent ()
	{
		name="Violent";
		//oppositePerk=typeof(Pacifist);
	}
	public override void ActivateEffect(PartyMember member) 
	{
		member.isViolent=true;
	}
	public override string GetMouseoverDescription () {return "Loses his temper\n\nSometimes lashes out at his fellows";}
}

public class Kleptomaniac: Trait
{
	public Kleptomaniac ()
	{
		name="Kleptomaniac";
	}
	public override void ActivateEffect(PartyMember member) 
	{
		member.isKleptomaniac=true;
	}
	public override string GetMouseoverDescription () {return "Does not respect ownership\n\nItems occasionally go missing";}
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
	float hitChanceDelta=-0.1f;
	public WeakArm ()
	{
		name="Weak arm";
		oppositePerk=typeof(Technique);//Powerhouse);
	}
	public override void ActivateEffect(PartyMember member) {member.meleeHitchanceMod+=hitChanceDelta;}//member.meleeDamageMod+=meleeDamageChange;}
	public override string GetMouseoverDescription () {return "Hits like a pansy\n\n"+hitChanceDelta+" to melee hit chance";}//return "Hits like a pansy\n\n"+meleeDamageChange+" to melee damage";}
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
	public override void ActivateEffect(PartyMember member) {member.rangedHitchanceMod+=rangedhitchanceChange;}
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
	public override void ActivateEffect(PartyMember member) {member.ChangeMaxCarryCapacity(inventorySizeChange);}
	public override string GetMouseoverDescription () {return "Doesn't even lift\n\n"+inventorySizeChange+" to carry capacity";}
}

public class Cook: Trait
{
	public static float hungerIncreaseMult=0.2f;
	public Cook ()
	{
		name="Cook";
	}
	public override void ActivateEffect(PartyMember member) {member.isCook=true;}
	public override string GetMouseoverDescription () {return "Does wonders with random ingredients\n\nSlows party's hunger increase";}
}

public class LockExpert: Trait
{
	public LockExpert() {name="Lock expert";}
	public override void ActivateEffect(PartyMember member) {member.isLockExpert=true;}
	public override string GetMouseoverDescription () {return "Can pick any lock\n\nOpens locks faster";}
}

