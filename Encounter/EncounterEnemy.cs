﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyPart
{
	public enum PartTypes {Vitals, Hands, Legs, Other}
	public PartTypes partType;
	public string name;
	public int hp;
	public float hitPercentageModifier;
	System.Action destructionEffect;
	System.Action<int> damageEffect;
	
	public BodyPart(string newName, int newHp, System.Action newDestroyEffect, System.Action<int> newDamageEffect, PartTypes type)
	{
		BasicConstructor(newName,newHp,newDestroyEffect,newDamageEffect,type,0);
	}
	public BodyPart(string newName, int newHp, System.Action newDestroyEffect, System.Action<int> newDamageEffect, PartTypes type, float hitModifier)
	{
		BasicConstructor(newName,newHp,newDestroyEffect,newDamageEffect,type,hitModifier);
	}
	
	void BasicConstructor(string newName, int newHp, System.Action newDestroyEffect, System.Action<int> newDamageEffect,PartTypes type, float hitModifier)
	{
		name=newName;
		hp=newHp;
		destructionEffect=newDestroyEffect;
		damageEffect=newDamageEffect;
		hitPercentageModifier=hitModifier;
		partType=type;
	}
	
	public void DamageBodypart(int damageDelta)
	{
		hp-=damageDelta;
		if (damageEffect!=null) damageEffect.Invoke(hp);
		
		//This launches destruction effect, but the limb is still removed from outside
		if (hp<=0 && destructionEffect!=null) destructionEffect.Invoke();
	}
}

public struct EnemyAttack
{
	public int damageDealt;
	public bool hitSuccesful;
	public bool blocked;
	public PartyMember attackedMember;
	public EncounterEnemy attackingEnemy;
	public PartyMember.BodyPartTypes hitBodyPart;
	
	public EnemyAttack(bool attackHit,int dmg, bool blockStatus, PartyMember defender, EncounterEnemy attacker
	, PartyMember.BodyPartTypes hitPart)
	{
		damageDealt=dmg;
		blocked=blockStatus;
		attackedMember=defender;
		attackingEnemy=attacker;
		hitSuccesful=attackHit;
		hitBodyPart=hitPart;
	}
}

public struct EnemyMove
{
	public List<Vector2> enemyMoveCoords;
	public Vector2 startCoords;
	public EnemyMove(List<Vector2> moves, Vector2 start)
	{
		startCoords=start;
		enemyMoveCoords=moves;
	}
}

public abstract class EncounterEnemy 
{	
	//STATIC
	public static string GetMapDescription(EnemyTypes enemyType)
	{
		string description=null;
		switch(enemyType)
		{
		case EnemyTypes.Gasser: {description="gas spitters"; break;}
		case EnemyTypes.Spindler: {description="spindlers"; break;}
		case EnemyTypes.Flesh: {description="flesh masses"; break;}
		case EnemyTypes.Muscle: {description="muscle masses"; break;}
		case EnemyTypes.Transient: {description="transients"; break;}
		case EnemyTypes.Quick: {description="quick masses"; break;}
		case EnemyTypes.Slime: {description="slime masses"; break;}
		}
		return description;
	}
	//Deprecated
	public static int GetEnemyCount(EnemyTypes enemyType)
	{
		int count=0;
		switch(enemyType)
		{
		case EnemyTypes.Gasser: {count=8; break;}
		case EnemyTypes.Spindler: {count=5; break;}
		case EnemyTypes.Flesh: {count=10; break;}
		case EnemyTypes.Muscle: {count=10; break;}
		case EnemyTypes.Transient: {count=10; break;}
		case EnemyTypes.Quick: {count=10; break;}
		case EnemyTypes.Slime: {count=12; break;}
		}
		return count;
	}
	//Determines how many enemies of this type should spawn compared to the baseline (half as many, twice as many and so on)
	//Gassers, Quicks and Transients - Weak tier; Flesh and Spindler - Mid Tier; Muscle and Slime - Strong tier
	//However, gassers get mid tier multiplier, because their shooting ability is too strong
	public static float GetEnemyCountModifier(EnemyTypes enemyType)
	{
		float mod=0;
		switch(enemyType)
		{
		case EnemyTypes.Gasser: {mod=1.5f; break;}
		case EnemyTypes.Spindler: {mod=1f; break;}
		case EnemyTypes.Flesh: {mod=1f; break;}
		case EnemyTypes.Muscle: {mod=0.6f; break;}
		case EnemyTypes.Slime: {mod=1.5f; break;}
		case EnemyTypes.Transient: {mod=2f; break;}
		case EnemyTypes.Quick: {mod=2f; break;}
		}
		return mod;
	}
	
	public static EncounterEnemy GetEnemy(EnemyTypes enemyType, Vector2 coords)
	{
		EncounterEnemy enemy=null;
		switch(enemyType)
		{
		case EnemyTypes.Gasser: {enemy=new Gasser(coords); break;}
		case EnemyTypes.Spindler: {enemy=new Spindler(coords); break;}
		case EnemyTypes.Flesh: {enemy=new FleshMass(coords); break;}
		case EnemyTypes.Muscle: {enemy=new MuscleMass(coords); break;}
		case EnemyTypes.Transient: {enemy=new Transient(coords); break;}
		case EnemyTypes.Quick: {enemy=new QuickMass(coords); break;}
		case EnemyTypes.Slime: {enemy=new SlimeMass(coords); break;}
		}
		return enemy;
	}
	
	//NON-STATIC
	public string name;
	public int health
	{
		get {return _health;}
		set 
		{
			_health=value;
			if (HealthChanged!=null) HealthChanged();
		}
	}
	int _health;
	public List<BodyPart> bodyParts=new List<BodyPart>();
	public bool TryGetBodyPart(BodyPart.PartTypes type, out BodyPart part)
	{
		part=null;
		foreach (BodyPart iterPart in bodyParts)
		{
			if (iterPart.partType==type) {part=iterPart; return true;}
		}
		return false;
	}
	public int minDamage;
	public int maxDamage;
	public int staminaDamage=2;
	public float damageMod=1f;
	protected int maxAttackRange=0;
	public float movesPerTurn=1;
	protected float currentAccumulatedMoves=0;
	//public int encounterCount=5;
	public float moveChance=0.1f;//0.4f;
	public List<StatusEffect> activeEffects=new List<StatusEffect>();
	int xCoord;
	int yCoord;
	protected int visionRange=0;
	protected int barricadeBashStrength=0;
	
	public delegate void StatusEffectDel();
	public event StatusEffectDel StatusEffectsChanged;
	public delegate void HealthChangeDel();
	public event HealthChangeDel HealthChanged;
	
	public string GetDamageString() {return minDamage+"-"+maxDamage;}
	
	public Sprite GetSprite() {return SpriteBase.mainSpriteBase.genericEnemySprite;}
	
	public enum EnemyTypes {Flesh,Quick,Slime,Muscle,Transient,Gasser,Spindler};
	
	public Vector2 GetCoords() {return new Vector2(xCoord,yCoord);}
	public void SetCoords(Vector2 newCoords) {xCoord=(int)newCoords.x; yCoord=(int)newCoords.y;}
	
	protected void AddStatusEffect(StatusEffect newEffect)
	{
		activeEffects.Add(newEffect);
		if (StatusEffectsChanged!=null) StatusEffectsChanged();
	}
	protected void RemoveStatusEffect(StatusEffect removedEffect)
	{
		activeEffects.Remove(removedEffect);
		if (StatusEffectsChanged!=null) StatusEffectsChanged();
	}
	
	public virtual int TakeDamage(int dmgTaken, BodyPart damagedPart, bool isRanged)
	{
		//if (bodyParts.Count>0)
		{
			//BodyPart damagedPart=attackedPart;//bodyParts[0];
			damagedPart.DamageBodypart(dmgTaken);//.hp-=dmgTaken;
			//Make this work within the damage routine in encountercanvashandler later
			EncounterCanvasHandler.main.AddNewLogMessage(name+"'s "+damagedPart.name+" is damaged for "+dmgTaken);
			if (damagedPart.hp<=0) 
			{
				//if (damagedPart.destructionEffect!=null) damagedPart.destructionEffect.Invoke();
				EncounterCanvasHandler.main.AddNewLogMessage(name+"'s "+damagedPart.name+" is broken!");
				bodyParts.Remove(damagedPart);//.Remove(damagedPart);
			}
		}
		/*
		else
		{
			health-=dmgTaken;
			if (HealthChanged!=null) {HealthChanged();}
		}*/
		return dmgTaken;
	}
	/*
	public void ToggleAttack(List<PartyMember> membersWithinReach)
	{
		if (EncounterCanvasHandler.main.encounterOngoing)
		AttackAction(membersWithinReach);
	}*/
	
	public virtual EnemyAttack AttackAction(List<PartyMember> membersWithinReach)//Dictionary<PartyMember,Vector2> memberCoords)
	{
		/*List<PartyMember> membersInRoom=new List<PartyMember>();
		foreach (PartyMember key in memberCoords.Keys) 
		{
			if (memberCoords[key]==new Vector2(xCoord,yCoord)) membersInRoom.Add(key);
		}*/
		PartyMember attackedMember=null;
		EnemyAttack performedAttack=new EnemyAttack();
		if (membersWithinReach.Count>0)
		{
			//Determine member to attack
			attackedMember=membersWithinReach[Random.Range(0,membersWithinReach.Count)];
			//actionMsg=null;
			//return damage;
			EncounterCanvasHandler manager=EncounterCanvasHandler.main;//EncounterCanvasHandler.main;
			//See if a member without defence mode on can be found within reach, if so pick the target out of those members
			List<PartyMember> membersWithoutDefence=new List<PartyMember>();
			foreach (PartyMember member in membersWithinReach)
			{
				if (member.stamina<staminaDamage)//!manager.memberTokens[member].defenceMode) 
				{
					membersWithoutDefence.Add(member);
				}
			}
			if (membersWithoutDefence.Count>0) attackedMember=membersWithoutDefence[Random.Range(0,membersWithoutDefence.Count)];
			
			//PartyMember attackedMember=manager.selectedMember;
			//int targetedPCIndex=manager.selectedMember;
			//manager.DamagePlayerCharacter(manager.selectedMember,damage);
			//See if damage gets blocked
			int myDamageRoll=Random.Range(minDamage,maxDamage+1);
			myDamageRoll=Mathf.RoundToInt(myDamageRoll*damageMod);
			int realStaminaDamage=staminaDamage;
			int realDmg=0;//manager.memberTokens[attackedMember].DamageAssignedMember(myDamageRoll,ref realStaminaDamage);//attackedMember.TakeDamage(myDamageRoll);
			//Have to assign this because value types can't be assigned null
			PartyMember.BodyPartTypes hitMemberPart=PartyMember.BodyPartTypes.Hands;
			bool hitConnected=manager.memberTokens[attackedMember].TryHitAssignedMember(myDamageRoll,out realDmg, out realStaminaDamage, out hitMemberPart);
			bool blocked=realStaminaDamage!=0;
			
			//If damage is blocked, send stamina damage instead
			//if (blocked) {realDmg=realStaminaDamage;}
			//manager.VisualizeDamageToMember(realDmg,blocked,attackedMember,this);
			performedAttack=new EnemyAttack(hitConnected,realDmg,blocked,attackedMember,this, hitMemberPart);
			int attackNoiseIntensity=1;
			manager.MakeNoise(GetCoords(),attackNoiseIntensity);
			//StartCoroutine(manager.VisualizeAttack(realDmg,manager.enemyTokens[this],manager.memberTokens[attackedMember]));
		} 
		else {throw new System.Exception("EncounterEnemy attack called while no members are within reach!");}
		return performedAttack;//attackedMember;
	}
	public virtual void TurnAction() {}
	
	//Incapsulates the actual moving of the token and enemy, does not determine whether or not to move this turn
	/*
	protected void MoveAction(Vector2 moveFromCoords, Vector2 moveToCoords)
	{
		RoomButtonHandler moveRoom=EncounterCanvasHandler.main.roomButtons[moveToCoords];
		RoomButtonHandler startRoom=EncounterCanvasHandler.main.roomButtons[moveFromCoords];
		if (moveRoom.assignedRoom.barricadeInRoom==null)
		{
			startRoom.MoveEnemyOutOfRoom(this);
			//ORDER IS IMPORTANT!
			SetCoords(moveToCoords);
			moveRoom.MoveEnemyInRoom(this);
		}
		else
		{
			moveRoom.BashBarricade(barricadeBashStrength);
		}
	}*/
	
	protected bool TryMove(List<PartyMember> visibleMembers, EnemyTokenHandler.PointOfInterest currentPOI
	, Dictionary<Vector2,EncounterRoom> map, Dictionary<PartyMember,Vector2> memberCoords, out Vector2 move)
	{
		bool movePerformed=false;
		Vector2 moveCoords=new Vector2(xCoord,yCoord);
		Vector2 startCoords=new Vector2(xCoord,yCoord);
		move=startCoords;
		//MOVE IS DONE HERE
		//If some members are visible, or a Point of Interest exists
		if (visibleMembers.Count>0 | currentPOI!=null) 
		{
			//Dictionary<Vector2,int> targetMemberMask=null;
			Vector2 targetCoord=Vector2.zero;
			//If the enemy is seeing members right now
			if (visibleMembers.Count>0)
			{
				/*
				//see if any of the visible members are "free" and not currently fighting
				List<PartyMember> freeVisibleMembers=new List<PartyMember>();
				
				foreach (PartyMember member in visibleMembers) 
				{
					if (!map[memberCoords[member]].hasEnemies) 
					{
						freeVisibleMembers.Add(member);
					}
				}
				
				//IF FREE VISIBLE MEMBERS EXIST, CHOOSE TARGET FROM THE SET OF FREE MEMBERS, ELSE CHOOSE FROM THE NON-FREE SET
				if (freeVisibleMembers.Count>0) {visibleMembers=freeVisibleMembers;}*/
				
				//Determine which member to target (in this case based on nearest distance)
				//targetMemberMask=masks[visibleMembers[0]];//Vector2 closestMemberCoords=memberCoords[visibleMembers[0]];
				int minRange=int.MaxValue;
				
				foreach (PartyMember visibleMember in visibleMembers)
				{
					//Vector2 =EncounterCanvasHandler.main.memberCoords[visibleMember];
					if (Mathf.Abs(memberCoords[visibleMember].x-xCoord)+Mathf.Abs(memberCoords[visibleMember].y-yCoord)<minRange)
					{
						targetCoord=memberCoords[visibleMember];
						minRange=(int)(Mathf.Abs(memberCoords[visibleMember].x-xCoord)+Mathf.Abs(memberCoords[visibleMember].y-yCoord));
					}
					/*
						//If my position in cycled member's mask is closer than in the previous one, make cycled member closest
						if (masks[visibleMember][new Vector2(xCoord,yCoord)]<targetMemberMask[new Vector2(xCoord,yCoord)]) 
						targetMemberMask=masks[visibleMember];*/
				}
				
			}
			else
			{
				//if no members are visible right now, but a POI to move towards exists
				//targetMemberMask=currentPOI.pointMoveMask;
				targetCoord=currentPOI.pointCoords;
				
			}
			//This should ensure that the actor will pick next waypoint for a path longer than 1 node, and pick current node otherwise
			var path=EncounterCanvasHandler.main.GetPathInCurrentEncounter(GetCoords(),targetCoord);
			moveCoords=path[Mathf.Clamp(path.Count-1,0,1)];
			//!!! CHANGE THIS TO COLLECT A LIST OF POSSIBLE MOVES AND PICK ONE OUT OF IT !!!
			//check nearby cells to find shortest route to player
			/*
					Vector2 minCoords=startCoords;
					float minValue=Mathf.Infinity;
					
					//Incapsulated check for position applicability
					System.Action<Vector2> pursuitCheck=(Vector2 cursorPos)=>
					{
						if (targetMemberMask.ContainsKey(cursorPos)) 
						{
							if (minValue>targetMemberMask[cursorPos] && !map[cursorPos].hasEnemies) 
							{
								minCoords=cursorPos;
								minValue=targetMemberMask[cursorPos];
							}
						}
					
					};
					
					//Up
					pursuitCheck.Invoke(startCoords+new Vector2(0,-1));
					//Down
					pursuitCheck.Invoke(startCoords+new Vector2(0,1));
					//Left
					pursuitCheck.Invoke(startCoords+new Vector2(-1,0));
					//Right
					pursuitCheck.Invoke(startCoords+new Vector2(1,0));
					moveCoords=minCoords;*/
			
		}
		else
		{
			//a) determine rooms available for move
			//Incapsulated check for position applicability and adding to available pool
			List<Vector2> availableCoords=new List<Vector2>();
			System.Action<Vector2> roamCheck=(Vector2 cursorPos)=>
			{
				if (map.ContainsKey(cursorPos)) 
				{
					EncounterRoom checkedRoom=map[cursorPos];
					if (!checkedRoom.isWall && /*!checkedRoom.hasEnemies &&*/ checkedRoom.barricadeInRoom==null) 
					{availableCoords.Add(cursorPos);}
				}
				
			};
			
			//List<EncounterRoom> availableRooms=new List<EncounterRoom>();
			//UP
			roamCheck.Invoke(new Vector2(xCoord,yCoord-1));
			//DOWN
			roamCheck.Invoke(new Vector2(xCoord,yCoord+1));
			//LEFT
			roamCheck.Invoke(new Vector2(xCoord-1,yCoord));
			//RIGHT
			roamCheck.Invoke(new Vector2(xCoord+1,yCoord));
			//b) randomly pick one to move to
			if (availableCoords.Count>0 && Random.value<moveChance) 
			{
				moveCoords=availableCoords[Random.Range(0,availableCoords.Count)];
			}
		}
		if (moveCoords!=startCoords)
		{
			RoomButtonHandler moveRoom=EncounterCanvasHandler.main.roomButtons[moveCoords];
			RoomButtonHandler startRoom=EncounterCanvasHandler.main.roomButtons[startCoords];
			if (moveRoom.assignedRoom.barricadeInRoom==null)
			{
				movePerformed=true;
				startRoom.MoveEnemyOutOfRoom(this);
				//ORDER IS IMPORTANT!
				SetCoords(moveCoords);
				moveRoom.MoveEnemyInRoom(this);
				move=moveCoords;
			}
			else
			{
				EncounterCanvasHandler.main.AddNewLogMessage(name+" smashes a barricade for "+barricadeBashStrength);
				moveRoom.BashBarricade(barricadeBashStrength);
			}
		}
		return movePerformed;
	}
	
	//Updates token visual if any members are seen
	/*
	public void VisionUpdate()
	{
		Dictionary<Vector2,EncounterRoom> encounterMap=EncounterCanvasHandler.main.currentEncounter.encounterMap;
		Dictionary<PartyMember,Vector2> memberCoords=EncounterCanvasHandler.main.memberCoords;
		if (VisionCheck(encounterMap,memberCoords).Count>0) {seesMember=true;}
		else {seesMember=false;}
	}*/
	//Returns a list of currently visible members
	public List<PartyMember> VisionCheck(Dictionary<Vector2,EncounterRoom> encounterMap, Dictionary<PartyMember,Vector2> memberCoords)
	{
		return VisionCheck(encounterMap,memberCoords,new Vector2((int)xCoord,(int)yCoord));
	}
	
	public List<PartyMember> VisionCheck(Dictionary<Vector2,EncounterRoom> encounterMap, Dictionary<PartyMember,Vector2> memberCoords, Vector2 pointOfView)
	{
		//First - find members within vision range
		List<PartyMember> visibleMembers=new List<PartyMember>();
		foreach (PartyMember key in memberCoords.Keys)
		{
			//See if member passes vision range check
			//if (Mathf.Abs(memberCoords[key].x-xCoord)+Mathf.Abs(memberCoords[key].y-yCoord)
			// <=Mathf.Max(visionRange+key.visibilityMod,1))
			if (Mathf.Abs(memberCoords[key].x-(int)pointOfView.x)<=visionRange && Mathf.Abs(memberCoords[key].y-(int)pointOfView.y)<=visionRange) 
			{
				bool memberVisible=true;
				//Second - see if any walls are blocking member
				BresenhamLines.Line(xCoord,yCoord,(int)memberCoords[key].x,(int)memberCoords[key].y
				 ,(int x, int y)=>
				 {
					 //bool visionClear=true;
					 if (encounterMap[new Vector2(x,y)].isWall) {memberVisible=false;}
					 return memberVisible;
				 });
				 if (memberVisible) {visibleMembers.Add (key);}
			}
		}
		return visibleMembers;
	}
	
	//Used for both move and attack actions "acting, the enemy uses its "move" for the turn
	public bool DoMyRound(Dictionary<PartyMember, Dictionary<Vector2,int>> masks, Dictionary<PartyMember,Vector2> memberCoords
	, EnemyTokenHandler.PointOfInterest currentPOI, out EnemyAttack performedAttack, out EnemyMove move)
	{
		//This lets the controlling enemy token know an attack needs to be animated
		bool roundIsAttack=false;
		//This lets the token know how many move stops it must do
		//Save start coords to update the move info after they change due to moving
		Vector2 startingCoords=GetCoords();
		move=new EnemyMove(new List<Vector2>(),GetCoords());
		performedAttack=new EnemyAttack();
		//Ensure enemies don't move after EncounterCanvasHandler shut down encounter render
		if (EncounterCanvasHandler.main.encounterOngoing)
		{
			//make sure the party didn't move into your coord
			Dictionary<Vector2,EncounterRoom> map=EncounterCanvasHandler.main.currentEncounter.encounterMap;
			/*
			//Make sure no party members are in the room with you
			List<PartyMember> presentMembers=new List<PartyMember>();
			//bool membersPresent=false;
			foreach (PartyMember key in memberCoords.Keys)
			{
				if (memberCoords[key].x==xCoord && memberCoords[key].y==yCoord) {presentMembers.Add (key);}//{membersPresent=true; break;}
			}
			//If no member present in my room do the move, otherwise do RoundAction
			if (presentMembers.Count==0)//!membersPresent)
			{*/	
				
				
				//Find if any members are visible !!!CONSIDER CHANGING List<PartyMember> TO List<Vector2>!!!!!
				List<PartyMember> visibleMembers=VisionCheck(map,memberCoords);
				List<PartyMember> membersWithinAttackRange=new List<PartyMember>();
				//see if any of the visible members are within maximum attack range
				foreach (PartyMember member in visibleMembers) 
				{
					if (Mathf.Abs(memberCoords[member].x-xCoord)+Mathf.Abs(memberCoords[member].y-yCoord)<=maxAttackRange)
					{
						membersWithinAttackRange.Add(member);
					}
				}
				//If any members are within attack range, attack, otherwise move on to the next fork
				if (membersWithinAttackRange.Count>0)
				{	
					performedAttack=AttackAction(membersWithinAttackRange);
					roundIsAttack=true;		
				}
				else
				{
					//If moves per turn are reduced below 1, enemy will move every other turn
					//TryMove(startCoords,moveCoords,visibleMembers,currentPOI,map,memberCoords);
					
					currentAccumulatedMoves+=Mathf.Max(0.5f,movesPerTurn);
					if (currentAccumulatedMoves>=1)
					{
						
						int totalMovesDone=0;
						List<Vector2> movesCoords=new List<Vector2>();
						for (totalMovesDone=0; totalMovesDone<currentAccumulatedMoves; totalMovesDone++)
						{
							Vector2 newMove=new Vector2();
							if (TryMove(visibleMembers,currentPOI,map,memberCoords, out newMove)) 
							{
								movesCoords.Add(newMove);
								//If any members are within attack range, stop mid-movement
								visibleMembers=VisionCheck(map,memberCoords,newMove);
								membersWithinAttackRange=new List<PartyMember>();
								foreach (PartyMember member in visibleMembers) 
								{
									if (Mathf.Abs(memberCoords[member].x-xCoord)+Mathf.Abs(memberCoords[member].y-yCoord)<=maxAttackRange)
									{
										membersWithinAttackRange.Add(member);
									}
								}
								if (membersWithinAttackRange.Count>0) break;
							}
						}
						move=new EnemyMove(movesCoords,startingCoords);
						currentAccumulatedMoves-=totalMovesDone;
						//movesCount=totalMovesDone;
					}
					
				}
		}
		return roundIsAttack;
	}
	
	public EncounterEnemy(Vector2 coords) 
	{
		xCoord=(int)coords.x;
		yCoord=(int)coords.y;
	}
	
}

//LIGHT TIER
public class QuickMass:EncounterEnemy
{
	public QuickMass(Vector2 coords) : base(coords)
	{
		name="Quick mass";
		health=180;
		minDamage=5;
		maxDamage=8;
		movesPerTurn=2;
		moveChance=0.3f;
		barricadeBashStrength=Mathf.RoundToInt(minDamage*0.5f);
		bodyParts.Add(new BodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},BodyPart.PartTypes.Vitals));
		bodyParts.Add(new BodyPart("Arms",90,()=>{this.AddStatusEffect(new NoArms(this));},null,BodyPart.PartTypes.Hands,0.1f));
		bodyParts.Add(new BodyPart("Legs",90,()=>{this.AddStatusEffect(new NoLegs(this));},null,BodyPart.PartTypes.Legs,-0.1f));
	}
}

public class Transient:EncounterEnemy
{
	public Transient(Vector2 coords) : base(coords)
	{
		name="Transient";
		health=180;
		minDamage=5;
		maxDamage=8;
		barricadeBashStrength=minDamage;
		int armsHealth=90;
		int legsHealth=90;
		/*
		if (Random.value<0.5f)
		{
			armsHealth=150;
			legsHealth=90;
		}
		else 
		{
			armsHealth=90;
			legsHealth=150;
		}*/
		float armsHitChanceDelta=0;
		float legsHitChanceDelta=0;
		
		if (Random.value<0.5f)
		{
			armsHitChanceDelta=0.2f;
			legsHitChanceDelta=-0.2f;
		}
		else 
		{
			armsHitChanceDelta=-0.2f;
			legsHitChanceDelta=0.2f;
		}
		
		bodyParts.Add(new BodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},BodyPart.PartTypes.Vitals));
		bodyParts.Add(new BodyPart("Arms",armsHealth,()=>{this.AddStatusEffect(new NoArms(this));},null,BodyPart.PartTypes.Hands,armsHitChanceDelta));
		bodyParts.Add(new BodyPart("Legs",legsHealth,()=>{this.AddStatusEffect(new NoLegs(this));},null,BodyPart.PartTypes.Legs,legsHitChanceDelta));
	}
	
	bool phasedIn=true;
	PhasedOut myEffect;
	/*
	public override int TakeDamage(int dmgTaken, BodyPart damagedPart, bool isRanged)
	{
		int realDmg=0;
		if (phasedIn) 
		{
			phasedIn=false;
			realDmg=dmgTaken;
			//myEffect=new PhasedOut();
			//AddStatusEffect(myEffect);//activeEffects.Add(myEffect);
			//EncounterCanvasHandler.main.DisplayNewMessage(name+" phases out!");
		}
		else 
		{
			//EncounterCanvasHandler.main.DisplayNewMessage("Attack passes through air!");
		}
		return base.TakeDamage(realDmg, isRanged);
	}*/
	
	public override EnemyAttack AttackAction(List<PartyMember> presentMembers)//Dictionary<PartyMember,Vector2> memberCoords)
	{
		phasedIn=true;
		//EncounterCanvasHandler.main.RemoveEnemyStatusEffect(myEffect);
		RemoveStatusEffect(myEffect);//activeEffects.Remove(myEffect);
		if (health>0)
		{
			//EncounterCanvasHandler.main.DisplayNewMessage(name+" phases in!");			
		}
		return base.AttackAction(presentMembers);
	}
	/*
	public override void TurnAction()
	{
		
		if (phasedIn)
		{
			phasedIn=false;
			myEffect=new PhasedOut();
			activeEffects.Add(myEffect);
			//EncounterCanvasHandler.main.AddEnemyStatusEffect(myEffect);
			EncounterCanvasHandler.main.DisplayNewMessage(name+" phases out!");
		}
	}*/
}

public class Gasser:EncounterEnemy
{
	public Gasser(Vector2 coords) : base(coords)
	{
		name="Gas spitter";
		health=135;
		minDamage=5;
		maxDamage=8;
		barricadeBashStrength=minDamage;
		visionRange=1;
		maxAttackRange=1;
		bodyParts.Add(new BodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},BodyPart.PartTypes.Vitals));
		bodyParts.Add(new BodyPart("Legs",90,()=>{this.AddStatusEffect(new NoLegs(this));},null,BodyPart.PartTypes.Legs,0.2f));
	}
	
	//Potentially deprecate this mechanic later
	int retaliationDamage=15;
	int retaliationDamageThreshold=70;
	int damageGainedThisRound=0;
	
	public override int TakeDamage(int dmgTaken, BodyPart damagedPart, bool isRanged)
	{
		int recievedDamage=base.TakeDamage(dmgTaken,damagedPart, isRanged);
		/*
		damageGainedThisRound+=recievedDamage;
		if (damageGainedThisRound>=retaliationDamageThreshold && health>0)
		{
			PartyMember attackedMember=EncounterCanvasHandler.main.selectedMember;
			int realRetaliationDmg=attackedMember.TakeDamage(retaliationDamage,false);
			EncounterCanvasHandler.main.StartDamageNumber(realRetaliationDmg,attackedMember,this);
			damageGainedThisRound=0;
		}*/
		return recievedDamage;
	}
	
	public override EnemyAttack AttackAction (List<PartyMember> presentMembers)//Dictionary<PartyMember,Vector2> memberCoords)
	{
		damageGainedThisRound=0;
		return base.AttackAction (presentMembers);
	}
}

//MEDIUM TIER
public class FleshMass:EncounterEnemy
{
	public FleshMass(Vector2 coords) : base(coords)
	{
		name="Flesh mass";
		health=270;
		minDamage=7;
		maxDamage=10;
		barricadeBashStrength=minDamage;
		bodyParts.Add(new BodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},BodyPart.PartTypes.Vitals));
		bodyParts.Add(new BodyPart("Arms",135,()=>{this.AddStatusEffect(new NoArms(this));},null,BodyPart.PartTypes.Hands,0.1f));
		bodyParts.Add(new BodyPart("Legs",90,()=>{this.AddStatusEffect(new NoLegs(this));},null,BodyPart.PartTypes.Legs,0.1f));
	}
}

public class Spindler:EncounterEnemy
{
	public Spindler(Vector2 coords) : base(coords)
	{
		name="Spindler";
		health=270;
		minDamage=6;
		maxDamage=9;
		barricadeBashStrength=minDamage;
		bodyParts.Add(new BodyPart("Systems",health,null,(int newHealth)=>{this.health=newHealth;},BodyPart.PartTypes.Vitals));
		bodyParts.Add(new BodyPart("Blades",90,()=>{this.AddStatusEffect(new NoArms(this));},null,BodyPart.PartTypes.Hands,0.15f));
		bodyParts.Add(new BodyPart("Legs",90,()=>{this.AddStatusEffect(new NoLegs(this));},null,BodyPart.PartTypes.Legs));
	}
	
	public override EnemyAttack AttackAction (List<PartyMember> presentMembers)//Dictionary<PartyMember,Vector2> memberCoords)
	{
		EnemyAttack performedAttack=base.AttackAction(presentMembers);
		PartyMember targetMember=performedAttack.attackedMember;//memberCoords);
		//Stamina damage part makes sure it can't inflict bleed while under NoArms
		if (targetMember!=null)
		{
			if (staminaDamage>=2 && performedAttack.hitSuccesful) PartyManager.mainPartyManager.AddPartyMemberStatusEffect(targetMember,new Bleed(targetMember));
		} 
		else {throw new System.Exception("Spindler tried to assign bleed to null PartyMember");}
		return performedAttack;
	}
}

//HEAVY TIER
public class MuscleMass:EncounterEnemy
{
	public MuscleMass(Vector2 coords) : base(coords)
	{
		name="Muscle mass";
		health=360;
		minDamage=10;
		maxDamage=12;
		barricadeBashStrength=minDamage;
		staminaDamage=3;
		bodyParts.Add(new BodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},BodyPart.PartTypes.Vitals));
		bodyParts.Add(new BodyPart("Arms",135,()=>{this.AddStatusEffect(new NoArms(this));},null,BodyPart.PartTypes.Hands,0.1f));
		bodyParts.Add(new BodyPart("Legs",135,()=>{this.AddStatusEffect(new NoLegs(this));},null,BodyPart.PartTypes.Legs,0.2f));
	}
}



public class SlimeMass:EncounterEnemy
{	
	public SlimeMass(Vector2 coords) : base(coords)
	{
		name="Slime mass";
		health=400;
		minDamage=11;
		maxDamage=15;
		barricadeBashStrength=minDamage;
		moveChance=0.1f;
		movesPerTurn=0;
		staminaDamage=3;
		bodyParts.Add(new BodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},BodyPart.PartTypes.Vitals));
		bodyParts.Add(new BodyPart("Arms",135,()=>{this.AddStatusEffect(new NoArms(this));},null,BodyPart.PartTypes.Hands,0.1f));
	}
	
	//int rangedResistance=3;
	float rangedDamageMultiplier=0.5f;
	
	public override int TakeDamage(int dmgTaken, BodyPart damagedPart, bool isRanged)
	{
		if (isRanged) 
		{
			dmgTaken=Mathf.FloorToInt(dmgTaken*rangedDamageMultiplier);
			//EncounterCanvasHandler.main.DisplayNewMessage("Shots pass clean through the slime!");
		}
		return base.TakeDamage (dmgTaken,damagedPart, isRanged);
	}
}


