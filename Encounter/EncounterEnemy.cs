using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		return enemy;
	}

	public static List<EncounterEnemy> GenerateWeightedEnemySet(int requiredTotalWeight,Vector2 spawnCoords)
	{
		List<EncounterEnemy> resultList=new List<EncounterEnemy>();
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
	protected CombatDeck combatDeck = new CombatDeck();
	protected int stamina=0;
	protected int ammo = 0;
	protected int armor = 0;
	protected List<CharacterStipulationCard> variationCards = new List<CharacterStipulationCard>();

	public List<CharacterStipulationCard> GetRandomVariationCards(int countRequired)
	{
		List<CharacterStipulationCard> resultList = new List<CharacterStipulationCard>(variationCards);
		if (variationCards.Count > 0)
		{
			if (countRequired > variationCards.Count)
				countRequired = variationCards.Count;
			while (resultList.Count > countRequired)
				resultList.RemoveAt(Random.Range(0, resultList.Count));
		}
		return resultList;
	}

	public string GetName()
	{
		return name;
	}

	public int GetHealth()
	{
		return health;
	}

	public void IncrementHealth(int delta)
	{
		SetHealth(health + delta);
	}

	public void SetHealth(int newValue)
	{
		health = newValue;
	}

	public int GetStartArmor()
	{
		return armor;
	}

	public int GetMaxStamina()
	{
		return stamina;
	}

	public void SetStamina(int newStamina)
	{
		stamina = newStamina;
		if (stamina < 0) stamina = 0;
	}

	public int GetStartAmmo() { return ammo; }

	public Sprite GetPortrait()
	{
		return SpriteBase.mainSpriteBase.enemyPortrait;
	}

	public CombatDeck GetCombatDeck()
	{
		return combatDeck;
	}

	public void TakeDamage(int damage) { health -= damage; }
}

//New encounter enemies




//

public class EnemyBodyPart
{
	public enum PartTypes { Vitals, Hands, Legs, Other }
	public PartTypes partType;
	public string name;
	public int hp;
	public float defaultHitchanceShare;
	public float currentHitchanceShare;
	System.Action destructionEffect;
	System.Action<int> damageEffect;

	public EnemyBodyPart(string newName, int newHp, System.Action newDestroyEffect, System.Action<int> newDamageEffect, PartTypes type)
	{
		BasicConstructor(newName, newHp, newDestroyEffect, newDamageEffect, type, 0);
	}
	public EnemyBodyPart(string newName, int newHp, System.Action newDestroyEffect, System.Action<int> newDamageEffect, PartTypes type, float hitModifier)
	{
		BasicConstructor(newName, newHp, newDestroyEffect, newDamageEffect, type, hitModifier);
	}

	void BasicConstructor(string newName, int newHp, System.Action newDestroyEffect, System.Action<int> newDamageEffect, PartTypes type, float hitModifier)
	{
		name = newName;
		hp = newHp;
		destructionEffect = newDestroyEffect;
		damageEffect = newDamageEffect;
		defaultHitchanceShare = hitModifier;
		//currentHitchanceShare = defaultHitchanceShare;
		partType = type;
	}

	public void DamageBodypart(int damageDelta)
	{
		hp -= damageDelta;
		if (damageEffect != null) damageEffect.Invoke(hp);

		//This launches destruction effect, but the limb is still removed from outside
		if (hp <= 0 && destructionEffect != null) destructionEffect.Invoke();
	}
}

public class EnemyBody
{
	//PartyMember assignedMember;

	List<EnemyBodyPart> currentParts = new List<EnemyBodyPart>();
	public List<EnemyBodyPart> GetHealthyParts()
	{
		List<EnemyBodyPart> healthyParts = new List<EnemyBodyPart>();
		foreach (EnemyBodyPart part in currentParts) if (part.hp > 0) healthyParts.Add(part);
		return healthyParts;
	}

	public bool TryGetBodyPart(EnemyBodyPart.PartTypes type, out EnemyBodyPart part)
	{
		part = null;
		foreach (EnemyBodyPart iterPart in currentParts)
		{
			if (iterPart.partType == type && iterPart.hp > 0) { part = iterPart; return true; }
		}
		return false;
	}

	public float dodgeChance;
	public void SetNewDodgeChance(float newDodgeChance)
	{
		dodgeChance = newDodgeChance;
		CalculateHitChances();
	}

	public EnemyBody(float totalDodgeChance, params EnemyBodyPart[] enemyParts)
	{
		foreach (EnemyBodyPart part in enemyParts) currentParts.Add(part);
		SetNewDodgeChance(totalDodgeChance);
	}

	public void CalculateHitChances()
	{
		float totalHitProbability = 0;//1-dodgeChance;

		float missingHitChanceFromBrokenParts = 0;
		foreach (EnemyBodyPart part in currentParts)
		{
			totalHitProbability += part.defaultHitchanceShare;
			if (part.hp <= 0) missingHitChanceFromBrokenParts += part.defaultHitchanceShare;//totalHitProbability*part.defaultHitchanceShare;
		}
		System.Action<EnemyBodyPart> partHitChanceCalculation = (EnemyBodyPart part) =>
		{
			if (part.hp > 0)
			{
				float adjustedHitChance = part.defaultHitchanceShare;//totalHitProbability*part.defaultHitchanceShare;
				if (missingHitChanceFromBrokenParts > 0)
				{
					//Increase for body parts that get broken
					float addedHitChance = missingHitChanceFromBrokenParts * (adjustedHitChance / (totalHitProbability - missingHitChanceFromBrokenParts));
					adjustedHitChance += addedHitChance;
				}
				//currentPartHitChances.AddProbability(part,adjustedHitChance);
				part.currentHitchanceShare = adjustedHitChance;
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
			if (damagedPart.hp <= 0) EncounterCanvasHandler.main.AddNewLogMessage(damagedPart.name + " is broken!");
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

	public EnemyAttack(bool attackHit, int dmg, bool blockStatus, IGotHitAnimation defender, EncounterEnemy attacker
	, PartyMember.BodyPartTypes hitPart)
	{
		damageDealt = dmg;
		blocked = blockStatus;
		attackedTarget = defender;
		attackingEnemy = attacker;
		hitSuccesful = attackHit;
		hitBodyPart = hitPart;
	}
}

public struct EnemyMove
{
	public List<Vector2> enemyMoveCoords;
	public Vector2 startCoords;
	public EnemyMove(List<Vector2> moves, Vector2 start)
	{
		startCoords = start;
		enemyMoveCoords = moves;
	}
}