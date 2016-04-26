using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PartyMemberCanvasHandler : MonoBehaviour {

	PartyMember assignedMember;
	public Text nameText;
	public Text healthText;
	public Text armsHealthText;
	public Text legsHealthText;
	public Text staminaText;
	public Text hungerText;
	public Text moraleText;
	public Text fatigueText;
	public Button memberSelector;
	public Button assignmentButton;
	Dictionary<StatusEffect,StatusEffectImageHandler> statusEffectTokens=new Dictionary<StatusEffect, StatusEffectImageHandler>();
	public StatusEffectImageHandler tokenPrefab;
	
	public void AssignPartyMember(PartyMember assigned) {assignedMember=assigned;}
	
	public void AddStatusEffectToken(StatusEffect effect)
	{
		StatusEffectImageHandler newStatusToken=Instantiate(tokenPrefab) as StatusEffectImageHandler;
		newStatusToken.transform.SetParent(transform.FindChild("Status Tokens Group"),false);
		newStatusToken.AssignStatusEffect(effect);
		statusEffectTokens.Add(effect,newStatusToken);
	}
	public void RemoveStatusEffectToken(StatusEffect effect)
	{
		StatusEffectImageHandler removedToken=statusEffectTokens[effect];
		statusEffectTokens.Remove(effect);
		GameObject.Destroy(removedToken.gameObject);
	}
	
	public void ShowHungerTooltip()
	{
		TooltipManager.main.CreateTooltip("Starvation prevents natural healing and reduces morale",hungerText.transform.parent);
	}
	
	public void ShowFatigueTooltip()
	{
		//TooltipManager.main.CreateTooltip("Reduces max stamina",fatigueText.transform.parent);
	}
	
	public void ShowMoraleTooltip()
	{
		TooltipManager.main.CreateTooltip("Can be spent when fatigue is high",moraleText.transform.parent);
	}
	
	public void DisableTooltip()
	{
		TooltipManager.main.StopAllTooltips();
	}
	
	public void RefreshAssignmentButton()
	{
		assignmentButton.gameObject.SetActive(false);
		AssignedTaskTypes currentTaskType;
		if (PartyManager.mainPartyManager.GetAssignedTask(assignedMember, out currentTaskType))
		{
			if (currentTaskType==AssignedTaskTypes.BuildCamp)
			{
				assignmentButton.gameObject.SetActive(true);
				assignmentButton.GetComponentInChildren<Text>().text="Stop building";
				assignmentButton.onClick.RemoveAllListeners();
				assignmentButton.onClick.AddListener(
				()=>
				{
					PartyManager.mainPartyManager.RemoveMemberTask(assignedMember);//.assignedTasks.Remove(token.assignedMember);
					PartyStatusCanvasHandler.main.RefreshAssignmentButtons();
				}
				);
			}
			if (currentTaskType==AssignedTaskTypes.Rest)
			{
				assignmentButton.gameObject.SetActive(true);
				assignmentButton.GetComponentInChildren<Text>().text="Stop resting";
				assignmentButton.onClick.RemoveAllListeners();
				assignmentButton.onClick.AddListener(
				()=>
				{
					PartyManager.mainPartyManager.RemoveMemberTask(assignedMember);//.assignedTasks.Remove(token.assignedMember);
					//token.moved=false;
					PartyStatusCanvasHandler.main.RefreshAssignmentButtons();
				}
				);
			}
		}
		else
		{
			//if (!MapManager.main.memberTokens[assignedMember].moved)
			//{
				MapRegion checkedRegion=assignedMember.currentRegion;//MapManager.main.GetRegion(assignedMember.worldCoords);
				if (!checkedRegion.hasCamp)
				{
					if (assignedMember.CheckEnoughFatigue(PartyMember.campSetupFatigueCost))
					{
					assignmentButton.gameObject.SetActive(true);
					assignmentButton.GetComponentInChildren<Text>().text="Build camp("
					+PartyMember.campSetupFatigueCost+"/"+(checkedRegion.campSetupTimeRemaining*PartyMember.campSetupFatigueCost)+")";
					assignmentButton.onClick.RemoveAllListeners();
					assignmentButton.onClick.AddListener(
					()=>
					{
						int totalInvestedHours=0;
						assignedMember.ChangeFatigue(PartyMember.campSetupFatigueCost);
						checkedRegion.SetUpCamp(1);
						MapManager.main.memberTokens[assignedMember].moved=true;
						//PartyManager.mainPartyManager.PassTime(campSetupTime);
						/*
						AssignedTask campBuildingTask=new AssignedTask(assignedMember,AssignedTaskTypes.BuildCamp
						,()=>
						{
							if (!checkedRegion.hasCamp) return true;
							else return false;
						}
						,()=>{checkedRegion.SetUpCamp(1);}
						);
						PartyManager.mainPartyManager.AssignMemberNewTask(campBuildingTask);*/
						PartyStatusCanvasHandler.main.RefreshAssignmentButtons();
					}
					);
					}
				}
				//Rest in bed assigned task buttons
				//Currently deprecated
				/*
				else
				{
					if (assignedMember.CanRest())
					{
						assignmentButton.gameObject.SetActive(true);
						string buttonText="Rest";
						bool hasBed=false;
						if (checkedRegion.campInRegion.freeBeds>0) 
						{
							hasBed=true;
							buttonText="Rest(bed)";
						}
						assignmentButton.GetComponentInChildren<Text>().text=buttonText;
						assignmentButton.onClick.RemoveAllListeners();
						assignmentButton.onClick.AddListener(()=>
						{
							PartyManager.mainPartyManager.AssignMemberNewTask(assignedMember.GetRestTask(hasBed));//.Rest(member);
							PartyStatusCanvasHandler.main.RefreshAssignmentButtons();
						});
					}
				}*/
			//}
		}
	}
	
	void Start() 
	{
		memberSelector.GetComponent<Image>().color=assignedMember.color;
		memberSelector.onClick.AddListener(()=>InventoryScreenHandler.mainISHandler.AssignSelectedMember(assignedMember));
	}//PartyScreenManager.mainPSManager.MemberClicked(assignedMember));}
	
	void Update()
	{
		nameText.text=""+assignedMember.name;
		healthText.text=assignedMember.memberBodyParts.currentParts[PartyMember.BodyPartTypes.Vitals].health+"|"
		+assignedMember.vitalsMaxHealth;
		armsHealthText.text=assignedMember.memberBodyParts.currentParts[PartyMember.BodyPartTypes.Hands].health+"|"
		+assignedMember.handsMaxHealth;
		legsHealthText.text=assignedMember.memberBodyParts.currentParts[PartyMember.BodyPartTypes.Legs].health+"|"
		+assignedMember.legsMaxHealth;
		/*healthText.text="Health:"+assignedMember.memberBodyParts.currentParts[PartyMember.BodyPartTypes.Hands].health
		+"|"+assignedMember.memberBodyParts.currentParts[PartyMember.BodyPartTypes.Legs].health
		+"|"+assignedMember.memberBodyParts.currentParts[PartyMember.BodyPartTypes.Vitals].health;*///+assignedMember.health+"|"+assignedMember.maxHealth;
		//staminaText.text="Stamina:"+assignedMember.stamina+"|"+assignedMember.currentMaxStamina;
		hungerText.text="Hunger:"+assignedMember.GetHunger()+"|100";
		moraleText.text="Morale:"+assignedMember.morale+"|100";
		fatigueText.text="Fatigue:"+assignedMember.GetFatigue()+"|"+PartyMember.maxFatigue;//
	}
}