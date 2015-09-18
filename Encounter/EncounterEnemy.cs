using UnityEngine;
using System.Collections;

public abstract class EncounterEnemy 
{
	public abstract string name {get;}
	public abstract int health {get; set;}
	public abstract int damage {get;}
	public abstract float moveChance {get;}
	
	public virtual int TakeDamage(int dmgTaken, bool isRanged)
	{
		health-=dmgTaken;
		return dmgTaken;
	}
	
	public virtual void TurnAction()
	{
		//actionMsg=null;
		//return damage;
		EncounterManager manager=EncounterManager.mainEncounterManager;
		//int targetedPCIndex=manager.selectedMember;
		//manager.DamagePlayerCharacter(manager.selectedMember,damage);
		int realDmg=manager.selectedMember.TakeDamage(damage);
		manager.DisplayNewMessage(name+" hits "+manager.selectedMember.name+" for "+realDmg+"!");
	}
	public virtual void RoundAction() {}
	/*
	public EncounterEnemy(string enemyName, int enemyHealth, int enemyDamage, float enemyMoveChance)
	{
		name=enemyName;
		health=enemyHealth;
		damage=enemyDamage;
		moveChance=enemyMoveChance;
	}*/
}

public class FleshMass:EncounterEnemy
{
	public override string name
	{
		get {return _name;}
	}
	string _name="Flesh mass";
	public override int health
	{
		get {return _health;}
		set {_health=value;}
	}
	int _health=8;
	public override int damage
	{
		get {return _damage;}
	}
	int _damage=10;
	public override float moveChance
	{
		get {return _moveChance;}
	}
	float _moveChance=0.27f;
	
}

public class MuscleMass:EncounterEnemy
{
	public override string name
	{
		get {return _name;}
	}
	string _name="Muscle mass";
	public override int health
	{
		get {return _health;}
		set {_health=value;}
	}
	int _health=15;
	public override int damage
	{
		get {return _damage;}
	}
	int _damage=20;
	public override float moveChance
	{
		get {return _moveChance;}
	}
	float _moveChance=0.27f;
}

public class QuickMass:EncounterEnemy
{
	public override string name
	{
		get {return _name;}
	}
	string _name="Quick mass";
	public override int health
	{
		get {return _health;}
		set {_health=value;}
	}
	int _health=5;
	public override int damage
	{
		get {return _damage;}
	}
	int _damage=15;
	public override float moveChance
	{
		get {return _moveChance;}
	}
	float _moveChance=0.5f;

}

public class SlimeMass:EncounterEnemy
{
	public override string name
	{
		get {return _name;}
	}
	string _name="Slime mass";
	public override int health
	{
		get {return _health;}
		set {_health=value;}
	}
	int _health=12;
	public override int damage
	{
		get {return _damage;}
	}
	int _damage=10;
	public override float moveChance
	{
		get {return _moveChance;}
	}
	float _moveChance=0.27f;
	//int rangedResistance=3;
	float rangedDamageMultiplier=0.5f;
	
	public override int TakeDamage (int dmgTaken, bool isRanged)
	{
		if (isRanged) {dmgTaken=Mathf.FloorToInt(dmgTaken*rangedDamageMultiplier);}
		EncounterManager.mainEncounterManager.DisplayNewMessage("Shots pass clean through the slime!");
		return base.TakeDamage (dmgTaken, isRanged);
	}
}

public class Transient:EncounterEnemy
{
	public override string name
	{
		get {return _name;}
	}
	string _name="Transient";
	public override int health
	{
		get {return _health;}
		set {_health=value;}
	}
	int _health=5;
	public override int damage
	{
		get {return _damage;}
	}
	int _damage=10;
	public override float moveChance
	{
		get {return _moveChance;}
	}
	float _moveChance=0.27f;
	bool phasedIn=true;
	PhasedOut myEffect;
	
	public override int TakeDamage(int dmgTaken, bool isRanged)
	{
		int realDmg=0;
		if (phasedIn) 
		{
			realDmg=dmgTaken;
		}
		else {EncounterManager.mainEncounterManager.DisplayNewMessage("Attacks pass through air!");}
		return base.TakeDamage(realDmg, isRanged);
	}
	
	public override void TurnAction()
	{
		phasedIn=true;
		EncounterManager.mainEncounterManager.RemoveEnemyStatusEffect(myEffect);
		if (_health>0)
		{
			EncounterManager.mainEncounterManager.DisplayNewMessage(name+" phases in!");			
		}
		base.TurnAction();
	}
	
	public override void RoundAction()
	{
		if (phasedIn)
		{
			phasedIn=false;
			myEffect=new PhasedOut();
			EncounterManager.mainEncounterManager.AddEnemyStatusEffect(myEffect);
			EncounterManager.mainEncounterManager.DisplayNewMessage(name+" phases out!");
		}
	}
}

public class Gasser:EncounterEnemy
{
	public override string name
	{
		get {return _name;}
	}
	string _name="Gassy sac";
	public override int health
	{
		get {return _health;}
		set {_health=value;}
	}
	int _health=12;
	public override int damage
	{
		get {return _damage;}
	}
	int _damage=3;
	public override float moveChance
	{
		get {return _moveChance;}
	}
	float _moveChance=0.27f;
	
	int retaliationDamage=15;
	int retaliationDamageThreshold=5;
	int damageGainedThisTurn=0;
	
	public override int TakeDamage(int dmgTaken, bool isRanged)
	{
		int recievedDamage=base.TakeDamage(dmgTaken, isRanged);
		
		damageGainedThisTurn+=recievedDamage;
		if (damageGainedThisTurn>=retaliationDamageThreshold && health>0)
		{
			//EncounterManager.mainEncounterManager.DamagePlayerCharacter(EncounterManager.mainEncounterManager.selectedMember,retaliationDamage);
			PartyMember attackedMember=EncounterManager.mainEncounterManager.selectedMember;
			int realRetaliationDmg=attackedMember.TakeDamage(retaliationDamage,false);
			EncounterManager.mainEncounterManager.DisplayNewMessage(name+" releases gas!");
			EncounterManager.mainEncounterManager.DisplayNewMessage(
			 attackedMember.name+" takes"+realRetaliationDmg+"damage!");
			damageGainedThisTurn=0;
		}
		return recievedDamage;
	}
	
	public override void TurnAction ()
	{
		damageGainedThisTurn=0;
		base.TurnAction ();
	}
}

public class Spindler:EncounterEnemy
{
	public override string name
	{
		get {return _name;}
	}
	string _name="Spindler";
	public override int health
	{
		get {return _health;}
		set {_health=value;}
	}
	int _health=7;
	public override int damage
	{
		get {return _damage;}
	}
	int _damage=10;
	public override float moveChance
	{
		get {return _moveChance;}
	}
	float _moveChance=0.27f;
	
	public override void TurnAction ()
	{
		base.TurnAction ();
		EncounterManager manager=EncounterManager.mainEncounterManager;
		PartyManager.mainPartyManager.AddPartyMemberStatusEffect(manager.selectedMember,new Bleed(manager.selectedMember));
		manager.DisplayNewMessage(_name+" causes "+manager.selectedMember.name+" to bleed!");
	}
}
