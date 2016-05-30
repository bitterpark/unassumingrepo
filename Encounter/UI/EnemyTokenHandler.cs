using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyTokenHandler : MonoBehaviour, IAttackAnimation, IGotHitAnimation
{
	//because the coroutine is called from EncounterCanvasHandler, StopAllCoroutines called from this instance doesn't work. So, this is necessary
	bool destroyed=false;
	
	float attackAnimationTime=0.6f;
	float gotHitAnimationTime=0.6f;
	float moveWaitTime=1f;
	readonly int lastseenMemberPriority=3;
	
	public Text healthText;
	public EncounterEnemy assignedEnemy;
	public Image myImage;
	
	public float accumulatedMove=0;
	
	public class PointOfInterest
	{
		public Vector2 pointCoords;
		public Dictionary<Vector2, int> pointMoveMask;
		public int POIPriority=0;
		public PointOfInterest (Vector2 coords,Dictionary<Vector2,int> mask, int priority)
		{
			pointCoords=coords;
			pointMoveMask=mask;
			POIPriority=priority;
		}
	}
	PointOfInterest currentPOI=null;
	List<PartyMember> lastSeenMembers=new List<PartyMember>();
	
	public StatusEffectImageHandler statusEffectPrefab;
	public Transform statusEffectsGroup;
	
	public void AssignEnemy(EncounterEnemy newEnemy)
	{
		myImage.sprite=newEnemy.GetSprite();
		myImage.color=newEnemy.color;
		assignedEnemy=newEnemy;
		healthText.text=assignedEnemy.health.ToString();
		newEnemy.HealthChanged+=HealthChangeHandler;
		assignedEnemy.StatusEffectsChanged+=StatusEffectChangeHandler;
		StatusEffectChangeHandler();
		EncounterCanvasHandler.main.roomButtons[assignedEnemy.GetCoords()].AttachEnemyToken(transform);
	}
	
	public void Clicked()
	{
		TooltipManager.main.StopAllTooltips();
		EncounterCanvasHandler.main.EnemyClicked(assignedEnemy);
	}
	/*
	public void AnimateAttack() 
	{
		StartCoroutine(AttackAnimation());
	}*/
	
	public IEnumerator AttackAnimation(IGotHitAnimation targetAnimation)
	{
		if (!destroyed) myImage.transform.localScale=new Vector3(1.6f,1.6f,1f);
		else yield break;
		if (targetAnimation!=null) yield return StartCoroutine(targetAnimation.GotHitAnimation());
		else yield return new WaitForSeconds(attackAnimationTime);
		if (!destroyed) myImage.transform.localScale=new Vector3(1f,1f,1f);
		yield break;
	}

	#region IGotHitAnimation implementation

	public IEnumerator GotHitAnimation ()
	{
		if (!destroyed) myImage.transform.localScale=new Vector3(0.8f,0.8f,1f);
		else yield break;
		yield return new WaitForSeconds(gotHitAnimationTime);
		//print ("yielding attack animation!");
		if (!destroyed) myImage.transform.localScale=new Vector3(1f,1f,1f);
		yield break;
	}

	#endregion
	
	public void AddNewPOI(Vector2 pointCoords, Dictionary<Vector2,int> moveMask, int priority)
	{
		//Dictionary<Vector2,int> newMask=new Dictionary<Vector2, int>();
		if (currentPOI==null) currentPOI=new PointOfInterest(pointCoords,moveMask,priority);
		else 
		{if (currentPOI.POIPriority<=priority) currentPOI=new PointOfInterest(pointCoords,moveMask,priority);}
	}
	
	public void UpdateTokenVision()
	{
		if (!destroyed && EncounterCanvasHandler.main.encounterOngoing)
		{
			List<PartyMember> currentSeenMembers
				=assignedEnemy.VisionCheck(EncounterCanvasHandler.main.currentEncounter.encounterMap,EncounterCanvasHandler.main.memberCoords);
			//If some members were seen on the previous check, but stopped being seen on this check, set the POI to the last known coordinate of the member
			//!!CAUTION!! - this actually sets the POI to the coordinates the member was at immediately after he got out of vision!
			if (currentSeenMembers.Count==0 && lastSeenMembers.Count>0) 
			{
				if (EncounterCanvasHandler.main.encounterMembers.Contains(lastSeenMembers[0]))
				{
					Vector2 newPOICoords=EncounterCanvasHandler.main.memberCoords[lastSeenMembers[0]];
					AddNewPOI(newPOICoords,null,lastseenMemberPriority);//new Dictionary<Vector2, int>(EncounterCanvasHandler.main.memberMoveMasks[lastSeenMembers[0]]));//IterativeGrassfireMapper(newPOICoords));
				}
			}
			//If some members are seen currently, forget the POI and start pursuing them instead
			if (currentSeenMembers.Count>0) {currentPOI=null;}
			lastSeenMembers=currentSeenMembers;
		}
	}
	
	public IEnumerator TokenAttackTrigger(params IGotHitAnimation[] targetsWithinReach)
	{
		if (!destroyed && EncounterCanvasHandler.main.encounterOngoing) 
		{
			EnemyAttack attackInfo=assignedEnemy.AttackAction(new List<IGotHitAnimation>(targetsWithinReach));
			EncounterCanvasHandler.main.ShowDamageToFriendlyTarget(attackInfo);
			IGotHitAnimation targetAnimation=null;
			if (attackInfo.hitSuccesful)
			{
				//if (EncounterCanvasHandler.main.memberTokens.ContainsKey(attackInfo.attackedMember)) 
				targetAnimation=attackInfo.attackedTarget;//EncounterCanvasHandler.main.memberTokens[attackInfo.attackedMember];
			}
			//(attackInfo.attackedMember,attackInfo.attackingEnemy,attackInfo.damageDealt,attackInfo.blocked);
			yield return StartCoroutine(AttackAnimation(targetAnimation));
		}
		yield break;
	}
	
	public IEnumerator TokenRoundoverTrigger(Dictionary<PartyMember, Dictionary<Vector2,int>> masks, Dictionary<PartyMember,Vector2> memberCoords)
	{
		if (!destroyed && EncounterCanvasHandler.main.encounterOngoing)
		{
			EnemyAttack attackInfo;//=new EnemyAttack();
			EnemyMove movesInfo;
			bool roundIsAttack=assignedEnemy.DoMyRound(masks,memberCoords,currentPOI,out attackInfo, out movesInfo);
			if (roundIsAttack)
			{
				EncounterCanvasHandler.main.ShowDamageToFriendlyTarget(attackInfo);
				IGotHitAnimation targetAnimation=null;
				if (attackInfo.hitSuccesful)
				{
					//if (EncounterCanvasHandler.main.memberTokens.ContainsKey(attackInfo.attackedMember)) 
					targetAnimation=attackInfo.attackedTarget;//EncounterCanvasHandler.main.memberTokens[attackInfo.attackedMember];
				}
				//(attackInfo.attackedMember,attackInfo.attackingEnemy,attackInfo.damageDealt,attackInfo.blocked);
				yield return StartCoroutine(AttackAnimation(targetAnimation));
			}
			else
			{
				int totalMovesCount=movesInfo.enemyMoveCoords.Count;
				if (totalMovesCount>0)
				{
					EncounterCanvasHandler.main.roomButtons[movesInfo.startCoords].EnemyTokenRemoved(this);
					for (int i=0; i<totalMovesCount; i++)
					{
						
						//if (EncounterCanvasHandler.main.roomButtons[movesInfo.enemyMoveCoords[i]]);
						if (i > 0) EncounterCanvasHandler.main.roomButtons[movesInfo.enemyMoveCoords[i-1]].EnemyTokenRemoved(this);
						EncounterCanvasHandler.main.roomButtons[movesInfo.enemyMoveCoords[i]].AttachEnemyToken(transform);
						UpdateTokenVision();
						bool doMoveAnimation=false;
						Vector2 animationStartRoomCoords=Vector2.zero;
						if (i==0) animationStartRoomCoords=movesInfo.startCoords;
						else animationStartRoomCoords=movesInfo.enemyMoveCoords[i-1];
						if (EncounterCanvasHandler.main.roomButtons[animationStartRoomCoords].isVisible 
						|| EncounterCanvasHandler.main.roomButtons[movesInfo.enemyMoveCoords[i]].isVisible) 
						doMoveAnimation=true;
						//totalMovesCount--;
						if (doMoveAnimation) yield return new WaitForSeconds(moveWaitTime);
					}
					/*
					while (totalMovesCount>0)
					{
						EncounterCanvasHandler.main.roomButtons[assignedEnemy.GetCoords()].AttachEnemyToken(transform);
						UpdateTokenVision();
						totalMovesCount--;
						yield return new WaitForSeconds(moveWaitTime);
					}*/
				}
			}
			//Clear POI if it has been reached
			if (currentPOI!=null) 
			{
				if (currentPOI.pointCoords==assignedEnemy.GetCoords()) currentPOI=null;
			}
			
			yield break;
			//UpdateVisionStatusDisplay();
		}
	}
	
	//Consider generalizing this to include health and status effect updates (somehow)?
	/*
	void UpdateToken()
	{
		//update color based on assignedEnemy state
		
		
		//move to new location\
		EncounterCanvasHandler.main.roomButtons[assignedEnemy.GetCoords()].AttachEnemyToken(transform);
	}*/
	
	public void MouseOverStart() 
	{
		StartCoroutine("MouseoverTextRoutine");
		/*
		string tooltipText=assignedEnemy.name;
		EncounterCanvasHandler encounterHandler=EncounterCanvasHandler.main;
		bool textForRangedAttack=false;
		bool attackPossible=encounterHandler.memberTokens[encounterHandler.selectedMember].AttackIsPossible(ref textForRangedAttack,assignedEnemy);
		if (attackPossible)//encounterHandler.memberCoords[encounterHandler.selectedMember]==assignedEnemy.GetCoords()) 
		{
			if (textForRangedAttack)//encounterHandler.memberTokens[encounterHandler.selectedMember].rangedMode) 
			{tooltipText+="\n\n"+encounterHandler.selectedMember.GetRangedAttackDescription();}
			else tooltipText+="\n\n"+encounterHandler.selectedMember.GetMeleeAttackDescription();
		}
		TooltipManager.main.CreateTooltip(tooltipText,this.transform);*/
	}
	
	IEnumerator MouseoverTextRoutine()
	{
		PartyMember referencedMember=null;//EncounterCanvasHandler.main.selectedMember;
		while (EncounterCanvasHandler.main.encounterOngoing)
		{
			if (EncounterCanvasHandler.main.selectedMember!=referencedMember)
			{
				//If selected member was switched during mouseover, update mouseover text to fit new selected member
				referencedMember=EncounterCanvasHandler.main.selectedMember;
				TooltipManager.main.StopAllTooltips();
				//NAME
				string tooltipText=assignedEnemy.name;
				//GENERAL INFO
                tooltipText += "\nDamage: " + assignedEnemy.GetDamageString();
                tooltipText += "\nDodge: " + (Mathf.RoundToInt(assignedEnemy.body.dodgeChance*100)) + "%";

                EncounterCanvasHandler encounterHandler=EncounterCanvasHandler.main;
				tooltipText+="\n";
                //LIMB INFO
				//tooltipText+="\nB";
				foreach (EnemyBodyPart part in assignedEnemy.body.GetHealthyParts())
				{
					float adjustedHitChance=Mathf.Clamp
					(referencedMember.GetCurrentAttackHitChance(EncounterCanvasHandler.main.memberTokens[referencedMember].rangedMode)
					+part.currentHitchanceShare-assignedEnemy.body.dodgeChance,0,1f);
					tooltipText+="\n"+part.name+":"+part.hp+" ("+Mathf.RoundToInt((adjustedHitChance*100f)).ToString()+"%)";
				}
				//Info on attacking this enemy
				bool textForRangedAttack=false;
				bool attackPossible=encounterHandler.memberTokens[encounterHandler.selectedMember].AttackIsPossible(ref textForRangedAttack,assignedEnemy);
				if (attackPossible)//encounterHandler.memberCoords[encounterHandler.selectedMember]==assignedEnemy.GetCoords()) 
				{
					if (textForRangedAttack)//encounterHandler.memberTokens[encounterHandler.selectedMember].rangedMode) 
					{tooltipText+="\n\n"+encounterHandler.selectedMember.GetRangedAttackDescription();}
					else tooltipText+="\n\n"+encounterHandler.selectedMember.GetMeleeAttackDescription();
				}
				TooltipManager.main.CreateTooltip(tooltipText,this.transform);
			}
			yield return new WaitForFixedUpdate();
		}
	}
	
	
	public void MouseOverEnd() 
	{
		StopCoroutine ("MouseoverTextRoutine");
		TooltipManager.main.StopAllTooltips();
	}
	
	void HealthChangeHandler()
	{
		healthText.text=assignedEnemy.health.ToString();
	}
	
	void StatusEffectChangeHandler()
	{
		Dictionary<EnemyStatusEffect,StatusEffectImageHandler> effectHandlers=new Dictionary<EnemyStatusEffect, StatusEffectImageHandler>();
		//StatusEffectImageHandler[] currentEffects=GetComponentsInChildren<StatusEffectImageHandler>();
		foreach (StatusEffectImageHandler handler in GetComponentsInChildren<StatusEffectImageHandler>())
		{
			effectHandlers.Add(handler.assignedEffect as EnemyStatusEffect,handler);
		}
		
		//remove outdated
		foreach (StatusEffectImageHandler handler in effectHandlers.Values)
		{
			if (!assignedEnemy.activeEffects.Contains(handler.assignedEffect as EnemyStatusEffect)) 
			{GameObject.Destroy(handler.gameObject);}
		}
		foreach (EnemyStatusEffect effect in assignedEnemy.activeEffects)
		{
			if (!effectHandlers.ContainsKey(effect)) 
			{
				StatusEffectImageHandler newHandler=Instantiate(statusEffectPrefab);
				newHandler.AssignStatusEffect(effect);
				newHandler.transform.SetParent(statusEffectsGroup,false);
				//This should be done by the enemy's basic tooltip
				//newHandler.GetComponent<Image>().raycastTarget=false;
			}
		}
	}
	/*
	void Update()
	{
		accumulatedMove=assignedEnemy.currentAccumulatedMoves;
	}*/
	
	void OnDestroy()
	{
		assignedEnemy.HealthChanged-=HealthChangeHandler;
		assignedEnemy.StatusEffectsChanged-=StatusEffectChangeHandler;
		destroyed=true;
	}
}
