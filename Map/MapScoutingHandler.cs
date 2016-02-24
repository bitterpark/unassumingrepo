using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapScoutingHandler : MonoBehaviour {

	public static bool scoutingDialogOngoing=false;
	public Text descriptionText;
	public Text selectCountText;
	public Text confirmButtonText;
	public Text selectionText;
	
	public Transform memberSelectorGroup;
	public MissionSelectorHandler memberSelectorPrefab;
	List<PartyMember> selectedForMission=new List<PartyMember>();
	
	MapRegion assignedRegion;
	
	void AssignRegion(MapRegion region)
	{
		assignedRegion=region;
		
		if (!assignedRegion.scouted) 
		{
			descriptionText.text="You have not scouted this area";
			confirmButtonText.text="Scout";
			selectCountText.text="";		
		}
		else 
		{
			UpdateAfterScouting();
		}
	}
	
	
	
	void UpdateAfterScouting()
	{
		descriptionText.text=assignedRegion.regionalEncounter.lootDescription+" infested with "+assignedRegion.regionalEncounter.enemyDescription;
		confirmButtonText.text="Enter ("+PartyMember.fatigueIncreasePerEncounter+" fatigue)";
		foreach (PartyMember member in assignedRegion.localPartyMembers)//PartyManager.mainPartyManager.selectedMembers) 
		{
			MissionSelectorHandler newSelector=Instantiate (memberSelectorPrefab) as MissionSelectorHandler;
			newSelector.AssignMember(member);
			newSelector.GetComponent<Button>().onClick.AddListener(()=>ToggleMissionMember(newSelector));
			newSelector.transform.SetParent(memberSelectorGroup,false);
			selectCountText.text=selectedForMission.Count+"/"+assignedRegion.regionalEncounter.maxAllowedMembers;//+EncounterCanvasHandler.encounterMaxPlayerCount;
			selectionText.text="";
		}
	}
	
	public void StartDialog(MapRegion dialogRegion) 
	{
		GetComponent<Canvas>().enabled=true;
		scoutingDialogOngoing=true;
		AssignRegion(dialogRegion);
	}
	public void EndDialog()
	{
		scoutingDialogOngoing=false;
		foreach (MissionSelectorHandler child in memberSelectorGroup.GetComponentsInChildren<MissionSelectorHandler>()) {GameObject.Destroy(child.gameObject);}
		selectedForMission.Clear();
		GetComponent<Canvas>().enabled=false;
	}
	
	public void ToggleMissionMember(MissionSelectorHandler handler)
	{
		PartyMember member=handler.assignedMember;
		AssignedTaskTypes emptyOutVar;
		if (!selectedForMission.Contains(member)
		&& selectedForMission.Count<assignedRegion.regionalEncounter.maxAllowedMembers
		&& handler.assignedMember.GetFatigue()+PartyMember.fatigueIncreasePerEncounter<=100
		&& !PartyManager.mainPartyManager.GetAssignedTask(member, out emptyOutVar))//EncounterCanvasHandler.encounterMaxPlayerCount) 
		{
			selectedForMission.Add(member);
			handler.selected=true;
		}
		else
		{
			selectedForMission.Remove(member);	
			handler.selected=false;
		}
		selectCountText.text=selectedForMission.Count+"/"+assignedRegion.regionalEncounter.maxAllowedMembers;
		if (selectedForMission.Count==0) {selectionText.text="";}
		else
		{
			selectionText.text="";
			foreach (PartyMember selectedMember in selectedForMission) {selectionText.text+=selectedMember.name+",";}
			selectionText.text=selectionText.text.Remove(selectionText.text.LastIndexOf(","));
		}
	}
	//Enter button goes here (and scout button)
	public void ConfirmPressed()
	{
		if (!assignedRegion.scouted) 
		{
			assignedRegion.scouted=true;
			UpdateAfterScouting();
		}
		else 
		{
			if (selectedForMission.Count>=assignedRegion.regionalEncounter.minRequiredMembers 
			&& selectedForMission.Count<=assignedRegion.regionalEncounter.maxAllowedMembers)//>0)
			{
				//EncounterCanvasHandler.mainEncounterCanvasHandler.StartNewEncounter(assignedRegion,selectedForMission);
				//EndDialog();
				//PartyManager.mainPartyManager.EnterPartyIntoEncounter(selectedForMission);
				//Use this to indicate encounter having been visited
				assignedRegion.visible=true;
				MapManager.main.EnterEncounter(assignedRegion.regionalEncounter,selectedForMission,false);
			}
		}
	}
	
	public void CancelPressed()
	{
		EndDialog();
	}
}
