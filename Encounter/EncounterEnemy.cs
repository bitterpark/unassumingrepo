using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBodyPart
{
	public enum PartTypes {Vitals, Hands, Legs, Other}
	public PartTypes partType;
	public string name;
	public int hp;
	public float defaultHitchanceShare;
	public float currentHitchanceShare;
	System.Action destructionEffect;
	System.Action<int> damageEffect;
	
	public EnemyBodyPart(string newName, int newHp, System.Action newDestroyEffect, System.Action<int> newDamageEffect, PartTypes type)
	{
		BasicConstructor(newName,newHp,newDestroyEffect,newDamageEffect,type,0);
	}
	public EnemyBodyPart(string newName, int newHp, System.Action newDestroyEffect, System.Action<int> newDamageEffect, PartTypes type, float hitModifier)
	{
		BasicConstructor(newName,newHp,newDestroyEffect,newDamageEffect,type,hitModifier);
	}
	
	void BasicConstructor(string newName, int newHp, System.Action newDestroyEffect, System.Action<int> newDamageEffect,PartTypes type, float hitModifier)
	{
		name=newName;
		hp=newHp;
		destructionEffect=newDestroyEffect;
		damageEffect=newDamageEffect;
		defaultHitchanceShare=hitModifier;
        //currentHitchanceShare = defaultHitchanceShare;
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

public class EnemyBody
{
	//PartyMember assignedMember;

	List<EnemyBodyPart> currentParts=new List<EnemyBodyPart>();
	public List<EnemyBodyPart> GetHealthyParts() 
	{
		List<EnemyBodyPart> healthyParts=new List<EnemyBodyPart>();
		foreach (EnemyBodyPart part in currentParts) if (part.hp>0) healthyParts.Add(part);
		return healthyParts;
	}

	public bool TryGetBodyPart(EnemyBodyPart.PartTypes type, out EnemyBodyPart part)
	{
		part=null;
		foreach (EnemyBodyPart iterPart in currentParts)
		{
			if (iterPart.partType==type && iterPart.hp>0) {part=iterPart; return true;}
		}
		return false;
	}

	public float dodgeChance;
	public void SetNewDodgeChance(float newDodgeChance)
	{
		dodgeChance=newDodgeChance;
		CalculateHitChances();
	}
		
	public EnemyBody(float totalDodgeChance,params EnemyBodyPart[] enemyParts)
	{
		foreach (EnemyBodyPart part in enemyParts) currentParts.Add(part);
		SetNewDodgeChance(totalDodgeChance);
	}
    
	public void CalculateHitChances()
	{
        float totalHitProbability = 0;//1-dodgeChance;
		
		float missingHitChanceFromBrokenParts=0;
		foreach (EnemyBodyPart part in currentParts)
		{
            totalHitProbability+=part.defaultHitchanceShare;
            if (part.hp <= 0) missingHitChanceFromBrokenParts += part.defaultHitchanceShare;//totalHitProbability*part.defaultHitchanceShare;
		}
		System.Action<EnemyBodyPart> partHitChanceCalculation=(EnemyBodyPart part)=>
		{
			if (part.hp>0) 
			{
                float adjustedHitChance = part.defaultHitchanceShare;//totalHitProbability*part.defaultHitchanceShare;
				if (missingHitChanceFromBrokenParts>0)
				{
					//Increase for body parts that get broken
					float addedHitChance=missingHitChanceFromBrokenParts*(adjustedHitChance/(totalHitProbability-missingHitChanceFromBrokenParts));
					adjustedHitChance+=addedHitChance;
				}
				//currentPartHitChances.AddProbability(part,adjustedHitChance);
				part.currentHitchanceShare=adjustedHitChance;
			}
		};
		foreach (EnemyBodyPart part in currentParts)
		{
			partHitChanceCalculation.Invoke(part);
		}
	}

	public void DamageBodyPart(EnemyBodyPart damagedPart, int damage)
	{
		if (!currentParts.Contains(damagedPart)) throw new System.Exception("Attempting to damage an enemy body part that doesn't exist in enemybody!");
		else
		{
			damagedPart.DamageBodypart(damage);
			if (damagedPart.hp<=0) EncounterCanvasHandler.main.AddNewLogMessage(damagedPart.name+" is broken!");
			CalculateHitChances();
		}
	}
}

public struct EnemyAttack
{
	public int damageDealt;
	public bool hitSuccesful;
	public bool blocked;
	public IGotHitAnimation attackedTarget;
	public EncounterEnemy attackingEnemy;
	public PartyMember.BodyPartTypes hitBodyPart;
	
	public EnemyAttack(bool attackHit,int dmg, bool blockStatus, IGotHitAnimation defender, EncounterEnemy attacker
	, PartyMember.BodyPartTypes hitPart)
	{
		damageDealt=dmg;
		blocked=blockStatus;
		attackedTarget=defender;
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

public abstract class EncounterEnemy: Character 
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
			case EnemyTypes.Spindler: {mod=1.1f; break;}
			case EnemyTypes.Flesh: {mod=1.1f; break;}
			case EnemyTypes.Muscle: {mod=0.8f; break;}
			case EnemyTypes.Slime: {mod=1.5f; break;}
			case EnemyTypes.Transient: {mod=1.5f; break;}
			case EnemyTypes.Quick: {mod=1.5f; break;}
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

	public static List<EncounterEnemy> GenerateWeightedEnemySet(int requiredTotalWeight,Vector2 spawnCoords)
	{
		Dictionary<EnemyTypes,int> weightsDict=new Dictionary<EnemyTypes, int>();
		List<EnemyTypes> allEnemyTypes=new List<EnemyTypes>();

		List<EnemyTypes> allowedTypes = new List<EnemyTypes>();
		allowedTypes.Add(EnemyTypes.Quick);

		//QUICKS ARE CURRENTLY REMOVED FROM THE LIST DUE TO THE FREEZE BUG
		foreach (EnemyTypes type in allowedTypes)//System.Enum.GetValues(typeof(EnemyTypes)))
		{
			//if (type!=EnemyTypes.Quick)
			{
				weightsDict.Add(type,GetEnemy(type,spawnCoords).weight);
				allEnemyTypes.Add(type);
			}
		}
		List<EncounterEnemy> resultList=new List<EncounterEnemy>();
		int currentTotalWeight=0;
		while (currentTotalWeight<requiredTotalWeight)
		{
			EnemyTypes randomType=allEnemyTypes[Random.Range(0,allEnemyTypes.Count)];
			if (currentTotalWeight+weightsDict[randomType]<=requiredTotalWeight)
			{	
				resultList.Add(GetEnemy(randomType,spawnCoords));
				currentTotalWeight+=weightsDict[randomType];
			}
		}
		return resultList;
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

	protected int weight=1;

	public List<EnemyBodyPart> bodyParts=new List<EnemyBodyPart>();
	public EnemyBody body;

	public int minDamage;
	public int maxDamage;

	protected float defaultDodgeChance;

	protected float bleedChance=0.5f;

	public int staminaDamage=2;
	public float damageMult=1f;
	protected int maxAttackRange=0;
	public float movesPerTurn=1;
	public float currentAccumulatedMoves=0;
	//public int encounterCount=5;
	public float moveChance=0.1f;//0.4f;
	public List<EnemyStatusEffect> activeEffects=new List<EnemyStatusEffect>();
	public int xCoord;
	public int yCoord;

	public bool inFront=true;

	protected int visionRange=0;

	public Color color;
	//protected int barricadeBashStrength=0;
	
	public delegate void StatusEffectDel();
	public event StatusEffectDel StatusEffectsChanged;
	public delegate void HealthChangeDel();
	public event HealthChangeDel HealthChanged;

	

	public string GetDamageString() {return (minDamage*damageMult)+"-"+(maxDamage*damageMult);}
	
	public Sprite GetSprite() {return SpriteBase.mainSpriteBase.genericEnemySprite;}
	
	public enum EnemyTypes {Flesh,Quick,Slime,Muscle,Transient,Gasser,Spindler};
	
	public Vector2 GetCoords() {return new Vector2(xCoord,yCoord);}
	public void SetCoords(Vector2 newCoords) {xCoord=(int)newCoords.x; yCoord=(int)newCoords.y;}
	
	protected void AddStatusEffect(EnemyStatusEffect newEffect)
	{
		activeEffects.Add(newEffect);
		if (StatusEffectsChanged!=null) StatusEffectsChanged();
	}
	protected void RemoveStatusEffect(EnemyStatusEffect removedEffect)
	{
		activeEffects.Remove(removedEffect);
		if (StatusEffectsChanged!=null) StatusEffectsChanged();
	}
	
	public virtual int TakeDamage(int dmgTaken, EnemyBodyPart damagedPart, bool isRanged)
	{
		body.DamageBodyPart(damagedPart,dmgTaken);
		return dmgTaken;
	}
	/*
	public void ToggleAttack(List<PartyMember> membersWithinReach)
	{
		if (EncounterCanvasHandler.main.encounterOngoing)
		AttackAction(membersWithinReach);
	}*/
	
	public virtual EnemyAttack AttackAction(List<IGotHitAnimation> targetsWithinReach)//Dictionary<PartyMember,Vector2> memberCoords)
	{
		IGotHitAnimation attackedTarget=null;
		EnemyAttack performedAttack=new EnemyAttack();
		if (targetsWithinReach.Count>0)
		{
			//Determine member to attack
			attackedTarget=null;//targetsWithinReach[Random.Range(0,targetsWithinReach.Count)];

			//If a barricade is in the room, pick that as target, else - pick a random available members
			if (maxAttackRange==0)
			{
				foreach (IGotHitAnimation target in targetsWithinReach)
				{
					if (target.GetType()==typeof(BarricadeToken)) {attackedTarget=target; break;}
				}
			}
			//If no barricade target is found, attack a random available member
			if (attackedTarget==null) attackedTarget=targetsWithinReach[Random.Range(0,targetsWithinReach.Count)];

			bool hitConnected=false;
			bool blocked=false;

			EncounterCanvasHandler manager=EncounterCanvasHandler.main;
			int myDamageRoll=Random.Range(minDamage,maxDamage+1);
			myDamageRoll=Mathf.RoundToInt(myDamageRoll*damageMult);
			int realDmg=0;
			PartyMember.BodyPartTypes hitMemberPart=PartyMember.BodyPartTypes.Hands;

			//See what type of target you are attacking
			if (attackedTarget.GetType()==typeof(MemberTokenHandler))
			{
				//If attacking member
				//EncounterCanvasHandler.main;
				MemberTokenHandler attackedMemberToken=attackedTarget as MemberTokenHandler;
				//See if damage gets blocked
				int realStaminaDamage=staminaDamage;
				//Have to assign this because value types can't be assigned null

				hitConnected=attackedMemberToken.TryHitAssignedMember(myDamageRoll,out realDmg, out realStaminaDamage, out hitMemberPart);
				blocked=realStaminaDamage!=0;
				//If damage is blocked, send stamina damage instead
				//if (blocked) {realDmg=realStaminaDamage;}
			}
			else
			{
				//If attacking barricade
				manager.roomButtons[GetCoords()].BashBarricade(myDamageRoll);
				realDmg=myDamageRoll;
				blocked=false;
				hitConnected=true;
			}

			performedAttack=new EnemyAttack(hitConnected,realDmg,blocked,attackedTarget,this, hitMemberPart);
			//Apply bleed
			//Make sure the bleed is being applies to a member and not a barricade
			if (performedAttack.attackedTarget.GetType() == typeof(MemberTokenHandler))
			{
				MemberTokenHandler attackedMemberToken = performedAttack.attackedTarget as MemberTokenHandler;
				PartyMember targetMember = attackedMemberToken.myMember;//memberCoords);
				//Stamina damage part makes sure it can't inflict bleed while under NoArms
				if (targetMember != null)
				{
					if (performedAttack.hitSuccesful && Random.value < bleedChance) PartyManager.mainPartyManager.AddPartyMemberStatusEffect(targetMember, new Bleed());
				}
				else { throw new System.Exception("Enemy tried to assign bleed to null PartyMember"); }
			}
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
					if (!checkedRoom.isWall) //&& /*!checkedRoom.hasEnemies &&*/ checkedRoom.barricadeInRoom==null) 
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
			//if (moveRoom.assignedRoom.barricadeInRoom==null)
			{
				//startRoom.MoveEnemyOutOfRoom(this);
				//ORDER IS IMPORTANT!
				SetCoords(moveCoords);
				moveRoom.MoveEnemyInRoom(this);
				move=moveCoords;
			}
			/*
			else
			{
			 	if (moveRoom.isVisible) EncounterCanvasHandler.main.AddNewLogMessage(name+" smashes a barricade for "+barricadeBashStrength);
				moveRoom.BashBarricade(barricadeBashStrength);
			}*/
			movePerformed=true;
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
				List<IGotHitAnimation> targetsWithinReach=new List<IGotHitAnimation>();
				//see if any of the visible members are within maximum attack range
				foreach (PartyMember member in visibleMembers) 
				{
					//Enemies can only attack members in front row, members in back row if none are in front row, or attack any row if they are ranged
					if (Mathf.Abs(memberCoords[member].x-xCoord)+Mathf.Abs(memberCoords[member].y-yCoord)<=maxAttackRange
					&& (maxAttackRange>=1 
					|| (EncounterCanvasHandler.main.memberTokens[member].inFront || EncounterCanvasHandler.main.roomButtons[GetCoords()].membersInFront.Count==0)))
					{
						targetsWithinReach.Add(EncounterCanvasHandler.main.memberTokens[member]);
					}
				}
				if (EncounterCanvasHandler.main.roomButtons[GetCoords()].assignedRoom.barricadeInRoom!=null)
				targetsWithinReach.Add(EncounterCanvasHandler.main.roomButtons[GetCoords()].currentBarricadeToken);
				//If any members are within attack range, attack, otherwise move on to the next fork
				if (targetsWithinReach.Count>0)
				{	
					performedAttack=AttackAction(targetsWithinReach);
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
						//for (totalMovesDone=0; totalMovesDone<currentAccumulatedMoves; totalMovesDone++)
						while (totalMovesDone<currentAccumulatedMoves)
						{
							Vector2 newMove=new Vector2();
							if (TryMove(visibleMembers,currentPOI,map,memberCoords, out newMove)) 
							{
								movesCoords.Add(newMove);
								totalMovesDone++;
								//If any members are within attack range, stop mid-movement
								visibleMembers=VisionCheck(map,memberCoords,newMove);
								targetsWithinReach.Clear();
								foreach (PartyMember member in visibleMembers) 
								{
									if (Mathf.Abs(memberCoords[member].x-xCoord)+Mathf.Abs(memberCoords[member].y-yCoord)<=maxAttackRange
									&& (maxAttackRange>=0 || EncounterCanvasHandler.main.memberTokens[member].inFront))
									{
										targetsWithinReach.Add(EncounterCanvasHandler.main.memberTokens[member]);
									}
								}
								if (targetsWithinReach.Count>0) break;
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
	
	public EncounterEnemy() {}

	public EncounterEnemy(Vector2 coords) 
	{
		xCoord=(int)coords.x;
		yCoord=(int)coords.y;
	}

	//NEW STUFF
	protected Deck<CombatCard> combatDeck = new Deck<CombatCard>();
	protected int stamina=0;

	public string GetName()
	{
		return name;
	}

	public int GetHealth()
	{
		return health;
	}

	public int GetStartStamina()
	{
		return stamina;
	}

	public void SetStamina(int newStamina)
	{
		stamina = newStamina;
		if (stamina < 0) stamina = 0;
	}

	public Sprite GetPortrait()
	{
		return SpriteBase.mainSpriteBase.enemyPortrait;
	}

	public Deck<CombatCard> GetCombatDeck()
	{
		return combatDeck;
	}

	public void TakeDamage(int damage) { health -= damage; }
}


public class QuickMass:EncounterEnemy
{
	public QuickMass(Vector2 coords) : base(coords)
	{
		name="Quick mass";
		health=45;
		minDamage=3;
		maxDamage=4;
		color=Color.blue;

		defaultDodgeChance=0.25f;
		body=new EnemyBody(defaultDodgeChance
		,new EnemyBodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},EnemyBodyPart.PartTypes.Vitals,0.3f)
		,new EnemyBodyPart("Arms",30,()=>{this.AddStatusEffect(new NoArms(this));},null,EnemyBodyPart.PartTypes.Hands,0.5f)
		,new EnemyBodyPart("Legs",60,()=>{this.AddStatusEffect(new NoLegs(this));},null,EnemyBodyPart.PartTypes.Legs,0.9f));
	}
}

public class Transient:EncounterEnemy
{
	public Transient(Vector2 coords) : base(coords)
	{
		name="Transient";
		health=45;
		minDamage=5;
		maxDamage=8;

		defaultDodgeChance=0.1f;
		body=new EnemyBody(defaultDodgeChance
		,new EnemyBodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},EnemyBodyPart.PartTypes.Vitals,0.2f)
		,new EnemyBodyPart("Arms",45,()=>{this.AddStatusEffect(new NoArms(this));},null,EnemyBodyPart.PartTypes.Hands,0.4f)
		,new EnemyBodyPart("Legs",90,()=>{this.AddStatusEffect(new NoLegs(this));},null,EnemyBodyPart.PartTypes.Legs,0.4f));
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
	
	public override EnemyAttack AttackAction(List<IGotHitAnimation> presentTargets)//Dictionary<PartyMember,Vector2> memberCoords)
	{
		phasedIn=true;
		//EncounterCanvasHandler.main.RemoveEnemyStatusEffect(myEffect);
		RemoveStatusEffect(myEffect);//activeEffects.Remove(myEffect);
		if (health>0)
		{
			//EncounterCanvasHandler.main.DisplayNewMessage(name+" phases in!");			
		}
		return base.AttackAction(presentTargets);
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
		health=45;
		minDamage=3;
		maxDamage=5;

		color=Color.green;

		visionRange=1;
		maxAttackRange=1;
		inFront=false;
		bleedChance = 0;
		//if (Random.value<0.5f) inFront=true;

		defaultDodgeChance=0.1f;
		body=new EnemyBody(defaultDodgeChance
		,new EnemyBodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},EnemyBodyPart.PartTypes.Vitals,0.4f)
		,new EnemyBodyPart("Legs",30,()=>{this.AddStatusEffect(new NoLegs(this));},null,EnemyBodyPart.PartTypes.Legs,0.9f));
	}
	
	//Potentially deprecate this mechanic later
	int retaliationDamage=15;
	int retaliationDamageThreshold=70;
	int damageGainedThisRound=0;
	
	public override int TakeDamage(int dmgTaken, EnemyBodyPart damagedPart, bool isRanged)
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
	
	public override EnemyAttack AttackAction (List<IGotHitAnimation> presenttargets)//Dictionary<PartyMember,Vector2> memberCoords)
	{
		damageGainedThisRound=0;
		return base.AttackAction (presenttargets);
	}
}

public class Spindler : EncounterEnemy
{
	public Spindler(Vector2 coords)
		: base(coords)
	{
		name = "Spindler";
		health = 45;
		minDamage = 5;
		maxDamage = 7;

		bleedChance = 0.8f;

		color = Color.black;

		defaultDodgeChance = 0.05f;
		body = new EnemyBody(defaultDodgeChance
		, new EnemyBodyPart("Vitals", health, null, (int newHealth) => { this.health = newHealth; }, EnemyBodyPart.PartTypes.Vitals, 0.5f)
		, new EnemyBodyPart("Arms", 30, () => { this.AddStatusEffect(new NoArms(this)); }, null, EnemyBodyPart.PartTypes.Hands, 0.8f)
		, new EnemyBodyPart("Legs", 90, () => { this.AddStatusEffect(new NoLegs(this)); }, null, EnemyBodyPart.PartTypes.Legs, 0.6f));
	}
}

public class FleshMass:EncounterEnemy
{
	public FleshMass(Vector2 coords) : base(coords)
	{
		name="Flesh mass";
		health=90;
		minDamage=6;
		maxDamage=9;

		weight = 2;

		color=Color.yellow;

		defaultDodgeChance=0.1f;
		body=new EnemyBody(defaultDodgeChance
		,new EnemyBodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},EnemyBodyPart.PartTypes.Vitals,0.4f)
		,new EnemyBodyPart("Arms",90,()=>{this.AddStatusEffect(new NoArms(this));},null,EnemyBodyPart.PartTypes.Hands,0.6f)
		,new EnemyBodyPart("Legs",180,()=>{this.AddStatusEffect(new NoLegs(this));},null,EnemyBodyPart.PartTypes.Legs,0.6f));
	}
}


public class MuscleMass:EncounterEnemy
{
	public MuscleMass(Vector2 coords) : base(coords)
	{
		name="Muscle mass";
		health=90;
		minDamage=7;
		maxDamage=11;

		color=Color.red;

		weight=2;

		defaultDodgeChance=0f;
		body=new EnemyBody(defaultDodgeChance
		,new EnemyBodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},EnemyBodyPart.PartTypes.Vitals,0.4f)
		,new EnemyBodyPart("Arms",90,()=>{this.AddStatusEffect(new NoArms(this));},null,EnemyBodyPart.PartTypes.Hands,0.7f)
		,new EnemyBodyPart("Legs",90,()=>{this.AddStatusEffect(new NoLegs(this));},null,EnemyBodyPart.PartTypes.Legs,0.8f));
	}
}



public class SlimeMass:EncounterEnemy
{	
	public SlimeMass(Vector2 coords) : base(coords)
	{
		name="Slime mass";
		health=180;
		minDamage=4;
		maxDamage=6;

		color=Color.magenta;

		weight=2;

		defaultDodgeChance=0f;
		body=new EnemyBody(defaultDodgeChance
		,new EnemyBodyPart("Vitals",health,null,(int newHealth)=>{this.health=newHealth;},EnemyBodyPart.PartTypes.Vitals,0.5f)
		,new EnemyBodyPart("Arms",180,()=>{this.AddStatusEffect(new NoArms(this));},null,EnemyBodyPart.PartTypes.Hands,0.8f));
	}
	
	//int rangedResistance=3;
	float rangedDamageMultiplier=0.5f;
	
	public override int TakeDamage(int dmgTaken, EnemyBodyPart damagedPart, bool isRanged)
	{
		if (isRanged) 
		{
			dmgTaken=Mathf.FloorToInt(dmgTaken*rangedDamageMultiplier);
			//EncounterCanvasHandler.main.DisplayNewMessage("Shots pass clean through the slime!");
		}
		return base.TakeDamage (dmgTaken,damagedPart, isRanged);
	}
}

//New encounter enemies

public class Skitter : EncounterEnemy
{
	public Skitter(): base()
	{
		name = "Skitter";
		health = 40;

		stamina = 2;

		combatDeck.Populate(CombatCard.GetMultipleCards(typeof(Pinch),combatDeck,4));
		combatDeck.Populate(CombatCard.GetMultipleCards(typeof(Burrow), combatDeck, 2));
		combatDeck.Populate(CombatCard.GetMultipleCards(typeof(Bite), combatDeck, 2));
	}
}

//Skitter cards
public class Pinch : CombatCard
{
	public Pinch(Deck<CombatCard> originDeck)
		: base(originDeck)
	{
		name = "Pinch";
		description = "";
		image=SpriteBase.mainSpriteBase.bite;
		healthDamage = 5;
		staminaCost = 2;
		targetType = TargetType.Character;
	}

	public override void PlayCard()
	{
		userCharGraphic.IncrementStamina(-staminaCost);
		targetCharGraphic.TakeHealthDamage(healthDamage);
	}
}

public class Burrow : CombatCard
{
	int staminaRestore = 4;
	public Burrow(Deck<CombatCard> originDeck)
		: base(originDeck)
	{
		name = "Burrow";
		description = "Restores "+staminaRestore+" stamina";
		image = SpriteBase.mainSpriteBase.restSprite;
		//healthDamage = 5;
		//staminaCost = 1;
		targetType = TargetType.None;
	}

	public override void PlayCard()
	{
		userCharGraphic.IncrementStamina(staminaRestore);
	}
}

public class Bite : CombatCard
{
	public Bite(Deck<CombatCard> originDeck)
		: base(originDeck)
	{
		name = "Bite";
		//description = "Restores " + staminaRestore + " stamina";
		image = SpriteBase.mainSpriteBase.bite;
		healthDamage = 10;
		staminaCost = 4;
		targetType = TargetType.Character;
	}

	public override void PlayCard()
	{
		userCharGraphic.IncrementStamina(-staminaCost);
		targetCharGraphic.TakeHealthDamage(healthDamage);
	}
}

public class Bugzilla : EncounterEnemy
{
	public Bugzilla()
		: base()
	{
		name = "Bugzilla";
		health = 80;
		stamina = 3;

		combatDeck.Populate(CombatCard.GetMultipleCards(typeof(Swing), combatDeck, 2));
		combatDeck.Populate(CombatCard.GetMultipleCards(typeof(Posture), combatDeck, 4));
		combatDeck.Populate(CombatCard.GetMultipleCards(typeof(Charge), combatDeck, 2));
	}
}

//Bugzilla cards
public class Swing : CombatCard
{
	public Swing(Deck<CombatCard> originDeck)
		: base(originDeck)
	{
		name = "Swing";
		description = "";
		image = SpriteBase.mainSpriteBase.bite;
		healthDamage = 5;
		staminaCost = 0;
		targetType = TargetType.Character;
	}

	public override void PlayCard()
	{
		userCharGraphic.IncrementStamina(-staminaCost);
		targetCharGraphic.TakeHealthDamage(healthDamage);
	}
}

public class  Posture: CombatCard
{
	int staminaRestore = 3;
	public Posture(Deck<CombatCard> originDeck)
		: base(originDeck)
	{
		name = "Posture";
		description = "Restores " + staminaRestore + " stamina";
		image = SpriteBase.mainSpriteBase.restSprite;
		//healthDamage = 5;
		//staminaCost = 1;
		targetType = TargetType.None;
	}

	public override void PlayCard()
	{
		userCharGraphic.IncrementStamina(staminaRestore);
	}
}

public class Charge : CombatCard
{
	int staminaRestore = 4;
	public Charge(Deck<CombatCard> originDeck)
		: base(originDeck)
	{
		name = "Charge";
		//description = "Restores " + staminaRestore + " stamina";
		image = SpriteBase.mainSpriteBase.bite;
		healthDamage = 20;
		staminaCost = 6;
		targetType = TargetType.Character;
	}

	public override void PlayCard()
	{
		userCharGraphic.IncrementStamina(-staminaCost);
		targetCharGraphic.TakeHealthDamage(healthDamage);
	}
}

public class Stinger : EncounterEnemy
{
	public Stinger()
		: base()
	{
		name = "Stinger";
		health = 60;
		stamina = 0;

		combatDeck.Populate(CombatCard.GetMultipleCards(typeof(Swing), combatDeck, 2));
		combatDeck.Populate(CombatCard.GetMultipleCards(typeof(Posture), combatDeck, 3));
		combatDeck.Populate(CombatCard.GetMultipleCards(typeof(Venom), combatDeck, 3));
	}
}

//Stringer cards

public class Venom : CombatCard
{
	int staminaRestore = 4;
	public Venom(Deck<CombatCard> originDeck)
		: base(originDeck)
	{
		name = "Venom";
		//description = "Restores " + staminaRestore + " stamina";
		image = SpriteBase.mainSpriteBase.venom;
		healthDamage = 5;
		staminaCost = 3;
		staminaDamage = 1;
		targetType = TargetType.Character;
	}

	public override void PlayCard()
	{
		userCharGraphic.IncrementStamina(-staminaCost);
		targetCharGraphic.TakeHealthDamage(healthDamage);
		targetCharGraphic.IncrementStamina(-staminaDamage);
	}
}


