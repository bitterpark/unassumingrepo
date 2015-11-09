using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EncounterEnemy 
{
	public string name;
	public int health;
	public int minDamage;
	public int maxDamage;
	public float moveChance=0.27f;
	public List<StatusEffect> activeEffects=new List<StatusEffect>();
	public bool seesMember=false;
	int xCoord;
	int yCoord;
	int visionRange=2;
	
	public delegate void StatusEffectDel();
	public event StatusEffectDel StatusEffectsChanged;
	public delegate void HealthChangeDel();
	public event HealthChangeDel HealthChanged;
	
	public string GetDamageString() {return minDamage+"-"+maxDamage;}
	
	public Sprite GetSprite() {return SpriteBase.mainSpriteBase.genericEnemySprite;}
	
	public enum EnemyTypes {Flesh,Quick,Slime,Muscle,Transient,Gasser,Spindler};
	
	public Vector2 GetCoords() {return new Vector2(xCoord,yCoord);}
	
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
	
	public virtual int TakeDamage(int dmgTaken, bool isRanged)
	{
		health-=dmgTaken;
		if (HealthChanged!=null) {HealthChanged();}
		return dmgTaken;
	}
	
	public virtual PartyMember RoundAction(List<PartyMember> membersInRoom)//Dictionary<PartyMember,Vector2> memberCoords)
	{
		/*List<PartyMember> membersInRoom=new List<PartyMember>();
		foreach (PartyMember key in memberCoords.Keys) 
		{
			if (memberCoords[key]==new Vector2(xCoord,yCoord)) membersInRoom.Add(key);
		}*/
		PartyMember attackedMember=null;
		if (membersInRoom.Count>0)
		{
			
			//actionMsg=null;
			//return damage;
			EncounterCanvasHandler manager=EncounterCanvasHandler.main;//EncounterCanvasHandler.main;
			attackedMember=membersInRoom[Random.Range(0,membersInRoom.Count)];
			//PartyMember attackedMember=manager.selectedMember;
			//int targetedPCIndex=manager.selectedMember;
			//manager.DamagePlayerCharacter(manager.selectedMember,damage);
			int myDamageRoll=Random.Range(minDamage,maxDamage+1);
			int realDmg=attackedMember.TakeDamage(myDamageRoll);
			
			//manager.DisplayNewMessage(name+" hits "+attackedMember.name+" for "+realDmg+"!");
			manager.StartDamageNumber(realDmg,attackedMember,this);
		} 
		else {throw new System.Exception("EncounterEnemy attack called while in an empty room!");}
		return attackedMember;
	}
	public virtual void TurnAction() {}
	
	public static string GetMapDescription(EnemyTypes enemyType)
	{
		string description=null;
		switch(enemyType)
		{
			case EnemyTypes.Gasser: {description="gas sacs"; break;}
			case EnemyTypes.Spindler: {description="spindlers"; break;}
			case EnemyTypes.Flesh: {description="flesh masses"; break;}
			case EnemyTypes.Muscle: {description="muscle masses"; break;}
			case EnemyTypes.Transient: {description="transients"; break;}
			case EnemyTypes.Quick: {description="quick masses"; break;}
			case EnemyTypes.Slime: {description="slime masses"; break;}
		}
		return description;
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
	
	//Updates token visual if any members are seen
	public void VisionUpdate()
	{
		Dictionary<Vector2,EncounterRoom> encounterMap=EncounterCanvasHandler.main.currentEncounter.encounterMap;
		Dictionary<PartyMember,Vector2> memberCoords=EncounterCanvasHandler.main.memberCoords;
		if (VisionCheck(encounterMap,memberCoords).Count>0) {seesMember=true;}
		else {seesMember=false;}
	}
	//Returns a list of currently visible members
	List<PartyMember> VisionCheck(Dictionary<Vector2,EncounterRoom> encounterMap, Dictionary<PartyMember,Vector2> memberCoords)
	{
		//First - find members within vision range
		List<PartyMember> visibleMembers=new List<PartyMember>();
		foreach (PartyMember key in memberCoords.Keys)
		{
			//See if member passes vision range check
			if (Mathf.Abs(memberCoords[key].x-xCoord)+Mathf.Abs(memberCoords[key].y-yCoord)
			 <=Mathf.Max(visionRange+key.visibilityMod,1)) 
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
	public void Move(Dictionary<PartyMember, Dictionary<Vector2,int>> masks, Dictionary<PartyMember,Vector2> memberCoords)
	{
		//Ensure enemies don't move after EncounterCanvasHandler shut down encounter render
		if (EncounterCanvasHandler.main.encounterOngoing)
		{
			//make sure the party didn't move into your coord
			Dictionary<Vector2,EncounterRoom> map=EncounterCanvasHandler.main.currentEncounter.encounterMap;
			//Make sure no party members are in the room with you
			List<PartyMember> presentMembers=new List<PartyMember>();
			//bool membersPresent=false;
			foreach (PartyMember key in memberCoords.Keys)
			{
				if (memberCoords[key].x==xCoord && memberCoords[key].y==yCoord) {presentMembers.Add (key);}//{membersPresent=true; break;}
			}
			//If no member present in my room do the move, otherwise do RoundAction
			if (presentMembers.Count==0)//!membersPresent)
			{	
				Vector2 moveCoords=new Vector2(xCoord,yCoord);
				Vector2 startCoords=new Vector2(xCoord,yCoord);
				
				//Find if any members are visible
				List<PartyMember> visibleMembers=VisionCheck(map,memberCoords);
				
				//If some members are visible
				if (visibleMembers.Count>0) 
				{
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
					if (freeVisibleMembers.Count>0) {visibleMembers=freeVisibleMembers;}
					
					//Determine which member to target (in this case based on shortest route)
					Dictionary<Vector2,int> targetMemberMask=masks[visibleMembers[0]];//Vector2 closestMemberCoords=memberCoords[visibleMembers[0]];
					foreach (PartyMember visibleMember in visibleMembers)
					{
						//If my position in cycled member's mask is closer than in the previous one, make cycled member closest
						if (masks[visibleMember][new Vector2(xCoord,yCoord)]<targetMemberMask[new Vector2(xCoord,yCoord)]) 
							targetMemberMask=masks[visibleMember];
					}
					//!!! CHANGE THIS TO COLLECT A LIST OF POSSIBLE MOVES AND PICK ONE OUT OF IT !!!
					//check nearby cells to find shortest route to player
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
					moveCoords=minCoords;
				}
				else
				{
					//a) determine rooms available for move
					
					//Incapsulated check for position applicability and add to available pool
					List<Vector2> availableCoords=new List<Vector2>();
					System.Action<Vector2> roamCheck=(Vector2 cursorPos)=>
					{
						if (map.ContainsKey(cursorPos)) 
						{
							EncounterRoom upRoom=map[cursorPos];
							if (!upRoom.isWall && !upRoom.hasEnemies) {availableCoords.Add(cursorPos);}
						}
						
					};
					
					//List<EncounterRoom> availableRooms=new List<EncounterRoom>();
					//UP
					roamCheck.Invoke(new Vector2(xCoord,yCoord-1));
					/*
					Vector2 upCoords=new Vector2(xCoord,yCoord-1);
					if (map.ContainsKey(upCoords)) 
					{
						EncounterRoom upRoom=map[upCoords];
						if (!upRoom.isWall) {availableCoords.Add(upCoords);}
					}*/
					//DOWN
					roamCheck.Invoke(new Vector2(xCoord,yCoord+1));
					/*
					Vector2 downCoords=new Vector2(xCoord,yCoord+1);
					if (map.ContainsKey(downCoords)) 
					{
						EncounterRoom downRoom=map[downCoords];
						if (!downRoom.isWall) {availableCoords.Add(downCoords);}
					}*/
					//LEFT
					roamCheck.Invoke(new Vector2(xCoord-1,yCoord));
					/*
					Vector2 leftCoords=new Vector2(xCoord-1,yCoord);
					if (map.ContainsKey(leftCoords)) 
					{
						EncounterRoom leftRoom=map[leftCoords];
						if (!leftRoom.isWall) {availableCoords.Add(leftCoords);}
					}*/
					//RIGHT
					roamCheck.Invoke(new Vector2(xCoord+1,yCoord));
					/*
					Vector2 rightCoords=new Vector2(xCoord+1,yCoord);
					if (map.ContainsKey(rightCoords)) 
					{
						EncounterRoom rightRoom=map[rightCoords];
						if (!rightRoom.isWall) {availableCoords.Add(rightCoords);}
					}*/
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
					startRoom.MoveEnemyOutOfRoom(this);
					moveRoom.MoveEnemyInRoom(this);
					xCoord=(int)moveCoords.x;
					yCoord=(int)moveCoords.y;
				}
				//update post-move vision
				VisionUpdate();
			}
			else
			{
				seesMember=true;
				RoundAction(presentMembers);
			}
		} //else {GameManager.DebugPrint("skipping turn!");}
	}
	
	public EncounterEnemy(Vector2 coords) 
	{
		xCoord=(int)coords.x;
		yCoord=(int)coords.y;
	}
	
}

public class FleshMass:EncounterEnemy
{
	public FleshMass(Vector2 coords) : base(coords)
	{
		name="Flesh mass";
		health=10;
		minDamage=5;
		maxDamage=10;
	}
}

public class MuscleMass:EncounterEnemy
{
	public MuscleMass(Vector2 coords) : base(coords)
	{
		name="Muscle mass";
		health=15;
		minDamage=15;
		maxDamage=20;
	}
}

public class QuickMass:EncounterEnemy
{
	public QuickMass(Vector2 coords) : base(coords)
	{
		name="Quick mass";
		health=8;
		minDamage=10;
		maxDamage=15;
		moveChance=0.5f;
	}
}

public class SlimeMass:EncounterEnemy
{	
	public SlimeMass(Vector2 coords) : base(coords)
	{
		name="Slime mass";
		health=15;
		minDamage=5;
		maxDamage=10;
	}
	
	//int rangedResistance=3;
	float rangedDamageMultiplier=0.5f;
	
	public override int TakeDamage (int dmgTaken, bool isRanged)
	{
		if (isRanged) 
		{
			dmgTaken=Mathf.FloorToInt(dmgTaken*rangedDamageMultiplier);
			//EncounterCanvasHandler.main.DisplayNewMessage("Shots pass clean through the slime!");
		}
		return base.TakeDamage (dmgTaken, isRanged);
	}
}

public class Transient:EncounterEnemy
{
	public Transient(Vector2 coords) : base(coords)
	{
		name="Transient";
		health=5;
		minDamage=5;
		maxDamage=10;
	}
	
	bool phasedIn=true;
	PhasedOut myEffect;
	
	public override int TakeDamage(int dmgTaken, bool isRanged)
	{
		int realDmg=0;
		if (phasedIn) 
		{
			phasedIn=false;
			realDmg=dmgTaken;
			myEffect=new PhasedOut();
			AddStatusEffect(myEffect);//activeEffects.Add(myEffect);
			//EncounterCanvasHandler.main.DisplayNewMessage(name+" phases out!");
		}
		else 
		{
			//EncounterCanvasHandler.main.DisplayNewMessage("Attack passes through air!");
		}
		return base.TakeDamage(realDmg, isRanged);
	}
	
	public override PartyMember RoundAction(List<PartyMember> presentMembers)//Dictionary<PartyMember,Vector2> memberCoords)
	{
		phasedIn=true;
		//EncounterCanvasHandler.main.RemoveEnemyStatusEffect(myEffect);
		RemoveStatusEffect(myEffect);//activeEffects.Remove(myEffect);
		if (health>0)
		{
			//EncounterCanvasHandler.main.DisplayNewMessage(name+" phases in!");			
		}
		return base.RoundAction(presentMembers);
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
		name="Gas sac";
		health=12;
		minDamage=2;
		maxDamage=3;
	}
	
	int retaliationDamage=15;
	int retaliationDamageThreshold=7;
	int damageGainedThisRound=0;
	
	public override int TakeDamage(int dmgTaken, bool isRanged)
	{
		int recievedDamage=base.TakeDamage(dmgTaken, isRanged);
		
		damageGainedThisRound+=recievedDamage;
		if (damageGainedThisRound>=retaliationDamageThreshold && health>0)
		{
			PartyMember attackedMember=EncounterCanvasHandler.main.selectedMember;
			int realRetaliationDmg=attackedMember.TakeDamage(retaliationDamage,false);
			/*
			EncounterCanvasHandler.main.DisplayNewMessage(name+" releases gas!");
			EncounterCanvasHandler.main.DisplayNewMessage(
			 attackedMember.name+" takes"+realRetaliationDmg+"damage!");*/
			EncounterCanvasHandler.main.StartDamageNumber(realRetaliationDmg,attackedMember,this);
			damageGainedThisRound=0;
		}
		return recievedDamage;
	}
	
	public override PartyMember RoundAction (List<PartyMember> presentMembers)//Dictionary<PartyMember,Vector2> memberCoords)
	{
		damageGainedThisRound=0;
		return base.RoundAction (presentMembers);
	}
}

public class Spindler:EncounterEnemy
{
	public Spindler(Vector2 coords) : base(coords)
	{
		name="Spindler";
		health=7;
		minDamage=5;
		maxDamage=10;
	}
	
	public override PartyMember RoundAction (List<PartyMember> presentMembers)//Dictionary<PartyMember,Vector2> memberCoords)
	{
		PartyMember targetMember=base.RoundAction (presentMembers);//memberCoords);
		if (targetMember!=null)
		{
			//EncounterCanvasHandler manager=EncounterCanvasHandler.main;
			PartyManager.mainPartyManager.AddPartyMemberStatusEffect(targetMember,new Bleed(targetMember));
			//EncounterCanvasHandler.main.DisplayNewMessage(name+" causes "+targetMember.name+" to bleed!");
		} 
		else {throw new System.Exception("Spindler tried to assign bleed to null PartyMember");}
		return targetMember;
	}
}
