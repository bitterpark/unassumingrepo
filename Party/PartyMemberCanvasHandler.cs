using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PartyMemberCanvasHandler : MonoBehaviour {

	PartyMember assignedMember;
	public Text nameText;
	public Text classText;

	public Text healthText;
	public Text armorText;
	public Text staminaText;
	public Text ammoText;

	public Text contractTimeText;
	public Button memberInventoryOpenButton;
	public Button assignmentButton;
	
	public void AssignPartyMember(PartyMember assigned) {assignedMember=assigned;}
	
	
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
			/*
				if (!checkedRegion.hasCamp)
				{
					if (assignedMember.CheckEnoughFatigue(PartyMember.campSetupFatigueCost))
					{
					assignmentButton.gameObject.SetActive(true);
					assignmentButton.GetComponentInChildren<Text>().text="Build camp("
					+PartyMember.campSetupFatigueCost+"/"+(checkedRegion.GetRemainingCampSetupTime()*PartyMember.campSetupFatigueCost)+")";
					assignmentButton.onClick.RemoveAllListeners();
					assignmentButton.onClick.AddListener(
					()=>
					{
						int totalInvestedHours=0;
						assignedMember.ChangeFatigue(PartyMember.campSetupFatigueCost);
						checkedRegion.SetUpCamp(1);
						MapManager.main.memberTokens[assignedMember].moved=true;
						PartyStatusCanvasHandler.main.RefreshAssignmentButtons();
					}
					);
					}
				}*/
		}
	}
	
	void Start() 
	{
		memberInventoryOpenButton.GetComponent<Image>().color=assignedMember.color;
		memberInventoryOpenButton.onClick.AddListener(()=>InventoryScreenHandler.mainISHandler.ToggleSelectedMember(assignedMember));
	}//PartyScreenManager.mainPSManager.MemberClicked(assignedMember));}
	
	void Update()
	{
		nameText.text=""+assignedMember.name;
		classText.text = assignedMember.GetClass().ToString();

		healthText.text =assignedMember.GetHealth().ToString();
		staminaText.text=assignedMember.GetMaxStamina().ToString();
		armorText.text = assignedMember.GetStartArmor().ToString();
		ammoText.text = assignedMember.GetStartAmmo().ToString();

		contractTimeText.text="Days left:"+assignedMember.hireDaysRemaining;
	}
}