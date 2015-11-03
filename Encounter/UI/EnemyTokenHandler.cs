using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyTokenHandler : MonoBehaviour 
{
	public Text healthText;
	EncounterEnemy assignedEnemy;
	public Image myImage;
	
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
	
	public void AnimateAttack() 
	{
		StartCoroutine(AttackAnimation());
	}
	
	IEnumerator AttackAnimation()
	{
		myImage.transform.localScale=new Vector3(1.5f,1.5f,1f);
		yield return new WaitForSeconds(0.5f);
		myImage.transform.localScale=new Vector3(1f,1f,1f);
		yield break;
	}
	
	public void UpdateTokenVision()
	{
		assignedEnemy.VisionUpdate();
		UpdateVisionStatusDisplay();
	}
	
	public void DoTokenMove(Dictionary<PartyMember, Dictionary<Vector2,int>> masks, Dictionary<PartyMember,Vector2> memberCoords)
	{
		assignedEnemy.Move(masks,memberCoords);
		EncounterCanvasHandler.main.roomButtons[assignedEnemy.GetCoords()].AttachEnemyToken(transform);
		UpdateVisionStatusDisplay();
	}
	
	void UpdateVisionStatusDisplay()
	{
		if (assignedEnemy.seesMember) {myImage.color=Color.red;}
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
		string tooltipText=assignedEnemy.name;
		EncounterCanvasHandler encounterHandler=EncounterCanvasHandler.main;
		if (encounterHandler.memberCoords[encounterHandler.selectedMember]==assignedEnemy.GetCoords()) 
		{
			if (encounterHandler.memberTokens[encounterHandler.selectedMember].rangedMode) 
			{tooltipText+="\n\n"+encounterHandler.selectedMember.GetRangedAttackDescription();}
			else tooltipText+="\n\n"+encounterHandler.selectedMember.GetMeleeAttackDescription();
		}
		TooltipManager.main.CreateTooltip(tooltipText,this.transform);
	}
	public void MouseOverEnd() {TooltipManager.main.StopAllTooltips();}
	
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
	}
}
