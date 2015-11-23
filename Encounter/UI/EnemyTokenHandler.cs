﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyTokenHandler : MonoBehaviour, IAttackAnimation
{
	//because the coroutine is called from EncounterCanvasHandler, StopAllCoroutines called from this instance doesn't work. So, this is necessary
	bool destroyed=false;
	
	public Text healthText;
	EncounterEnemy assignedEnemy;
	public Image myImage;
	
	public class PointOfInterest
	{
		public Vector2 pointCoords;
		public Dictionary<Vector2, int> pointMoveMask;
		public PointOfInterest (Vector2 coords,Dictionary<Vector2,int> mask)
		{
			pointCoords=coords;
			pointMoveMask=mask;
		}
	}
	PointOfInterest currentPOI=null;
	List<PartyMember> lastSeenMembers=new List<PartyMember>();
	
	public StatusEffectImageHandler statusEffectPrefab;
	public Transform statusEffectsGroup;
	
	public void AssignEnemy(EncounterEnemy newEnemy)
	{
		myImage.sprite=newEnemy.GetSprite();
		assignedEnemy=newEnemy;
		healthText.text=assignedEnemy.health.ToString();
		newEnemy.HealthChanged+=HealthChangeHandler;
		assignedEnemy.StatusEffectsChanged+=StatusEffectChangeHandler;
		StatusEffectChangeHandler();
		EncounterCanvasHandler.main.roomButtons[assignedEnemy.GetCoords()].AttachEnemyToken(transform);
		UpdateVisionStatusDisplay();
	}
	
	public void Clicked()
	{
		TooltipManager.main.StopAllTooltips();
		EncounterCanvasHandler.main.EnemyPressed(assignedEnemy);
	}
	/*
	public void AnimateAttack() 
	{
		StartCoroutine(AttackAnimation());
	}*/
	
	public IEnumerator AttackAnimation()
	{
		if (!destroyed) myImage.transform.localScale=new Vector3(1.5f,1.5f,1f);
		else yield break;
		yield return new WaitForSeconds(0.5f);
		if (!destroyed) myImage.transform.localScale=new Vector3(1f,1f,1f);
		yield break;
	}
	
	public void AddNewPOI(Vector2 pointCoords, Dictionary<Vector2,int> moveMask)
	{
		//Dictionary<Vector2,int> newMask=new Dictionary<Vector2, int>();
		currentPOI=new PointOfInterest(pointCoords,moveMask);//EncounterCanvasHandler.main.IterativeGrassfireMapper(pointCoords));
	}
	
	public void UpdateTokenVision()
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
				AddNewPOI(newPOICoords,null);//new Dictionary<Vector2, int>(EncounterCanvasHandler.main.memberMoveMasks[lastSeenMembers[0]]));//IterativeGrassfireMapper(newPOICoords));
			}
		}
		//If some members are seen currently, forget the POI and start pursuing them instead
		if (currentSeenMembers.Count>0) {currentPOI=null;}
		lastSeenMembers=currentSeenMembers;
		//assignedEnemy.VisionUpdate();
		UpdateVisionStatusDisplay();
	}
	
	public void DoTokenMove(Dictionary<PartyMember, Dictionary<Vector2,int>> masks, Dictionary<PartyMember,Vector2> memberCoords)
	{
		if (currentPOI!=null) 
		{
			if (currentPOI.pointCoords==assignedEnemy.GetCoords()) currentPOI=null;
		}
		assignedEnemy.Move(masks,memberCoords,currentPOI);
		EncounterCanvasHandler.main.roomButtons[assignedEnemy.GetCoords()].AttachEnemyToken(transform);
		UpdateTokenVision();
		//UpdateVisionStatusDisplay();
	}
	
	void UpdateVisionStatusDisplay()
	{
		if (lastSeenMembers.Count>0) {myImage.color=Color.red;}
		else {myImage.color=Color.gray;}
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
				//
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
		Dictionary<StatusEffect,StatusEffectImageHandler> effectHandlers=new Dictionary<StatusEffect, StatusEffectImageHandler>();
		//StatusEffectImageHandler[] currentEffects=GetComponentsInChildren<StatusEffectImageHandler>();
		foreach (StatusEffectImageHandler handler in GetComponentsInChildren<StatusEffectImageHandler>())
		{
			effectHandlers.Add(handler.assignedEffect,handler);
		}
		
		//remove outdated
		foreach (StatusEffectImageHandler handler in effectHandlers.Values)
		{
			if (!assignedEnemy.activeEffects.Contains(handler.assignedEffect)) 
			{GameObject.Destroy(handler.gameObject);}
		}
		foreach (StatusEffect effect in assignedEnemy.activeEffects)
		{
			if (!effectHandlers.ContainsKey(effect)) 
			{
				StatusEffectImageHandler newHandler=Instantiate(statusEffectPrefab);
				newHandler.AssignStatusEffect(effect);
				newHandler.transform.SetParent(statusEffectsGroup,false);
				//This should be done by the enemy's basic tooltip
				newHandler.GetComponent<Image>().raycastTarget=false;
			}
		}
	}
	
	void OnDestroy()
	{
		assignedEnemy.HealthChanged-=HealthChangeHandler;
		assignedEnemy.StatusEffectsChanged-=StatusEffectChangeHandler;
		destroyed=true;
	}
}
