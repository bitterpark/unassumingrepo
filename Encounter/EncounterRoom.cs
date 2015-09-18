using UnityEngine;
using System.Collections;
//using System.Collections.Generic;


public class EncounterRoom
{
	public Encounter parentEncounter;
	
	public bool isExit
	{
		get {return _isExit;}
		set 
		{
			_isExit=value;
			if (_isExit) 
			{
				hasEnemies=false;
				hasLoot=false;
				description="This room leads outside";
			}
		}
	}
	
	public bool _isExit=false;
	
	public bool hasEnemies
	{
		get {return _hasEnemies;}
		set 
		{
			_hasEnemies=value;
			if (_hasEnemies) 
			{
				description="There are enemies in this room!";
				GenerateEnemy();
			}
			else 
			{
				description="This room is peaceful and has no enemies";
			}
		}
	}
	public bool _hasEnemies;
	
	//List<EncounterEnemy> enemiesInRoom
	public EncounterEnemy enemyInRoom;
	
	public bool hasLoot
	{
		get {return _hasLoot;}
		set 
		{
			_hasLoot=value;
			if (!hasEnemies) 
			{
				if (!_hasLoot) {}
			}
		}
	
	}
	public bool _hasLoot=false;
	
	public bool isVisible=false;
	
	public string description="";
	
	public int xCoord=0;
	public int yCoord=0;
	
	public EncounterRoom(Encounter parent, int x, int y)
	{
		xCoord=x;
		yCoord=y;
		parentEncounter=parent;
	}
	
	public EncounterRoom(Encounter parent) {parentEncounter=parent;}
	
	void GenerateEnemy()
	{
		switch (parentEncounter.encounterEnemyType)
		{
			case Encounter.EnemyTypes.Gasser: {enemyInRoom=new Gasser(); break;}
			case Encounter.EnemyTypes.Flesh: {enemyInRoom=new FleshMass(); break;}//EncounterEnemy("Flesh mass",5,5,0.27f); break;}
			case Encounter.EnemyTypes.Quick: {enemyInRoom=new QuickMass(); break;} //EncounterEnemy("Quick mass",5,10,0.5f); break;}
			case Encounter.EnemyTypes.Slime: {enemyInRoom=new SlimeMass(); break;} //EncounterEnemy("Slime mass",9,5,0.27f); break;}
			case Encounter.EnemyTypes.Muscle: {enemyInRoom=new MuscleMass(); break;} //EncounterEnemy("Muscle mass",7,20,0.27f); break;}
			case Encounter.EnemyTypes.Transient: {enemyInRoom=new Transient(); break;}
			case Encounter.EnemyTypes.Spindler: {enemyInRoom=new Spindler(); break;}
		}
	}
	
	public int DamageEnemy(int damage, bool isRanged)
	{
		//enemyInRoom.health-=damage;
		int actualDmgTaken=enemyInRoom.TakeDamage(damage,isRanged);
		if (enemyInRoom.health<=0) 
		{
			hasEnemies=false;
			EncounterManager.mainEncounterManager.DisplayNewMessage(enemyInRoom.name+" killed!");
		}
		return actualDmgTaken;
	}
}
