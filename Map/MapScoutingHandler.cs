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
	public Text ambushThreatText;

	public GameObject scoutMoreButton;
	
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
			ambushThreatText.text="";	
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
			ambushThreatText.text="Ambush threat:"+assignedRegion.CalculateThreatLevel(0);
		}
		if (assignedRegion.ambientThreatNumber<=0) scoutMoreButton.SetActive(false);
		else scoutMoreButton.SetActive(true);
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
		//&& selectedForMission.Count<assignedRegion.regionalEncounter.maxAllowedMembers
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
		ambushThreatText.text="Ambush threat:"+assignedRegion.CalculateThreatLevel(selectedForMission.Count);
	}
	//Enter button goes here (and scout button)
	public void ConfirmPressed()
	{
		if (!assignedRegion.scouted) 
		{
			assignedRegion.scouted=true;
			EndDialog();
			GameEventManager.mainEventManager.RollScavengeEvents(assignedRegion,assignedRegion.localPartyMembers);
			//UpdateAfterScouting();
		}
		else 
		{
			/*
			if (selectedForMission.Count>=assignedRegion.regionalEncounter.minRequiredMembers 
			&& selectedForMission.Count<=assignedRegion.regionalEncounter.maxAllowedMembers)//>0)*/

				float targetValue=0;
				switch (assignedRegion.CalculateThreatLevel(selectedForMission))
				{
					case MapRegion.ThreatLevels.None: {targetValue=0; break;}
					case MapRegion.ThreatLevels.Low: {targetValue=0.3f; break;}
					case MapRegion.ThreatLevels.Medium: {targetValue=0.6f; break;}
					case MapRegion.ThreatLevels.High: {targetValue=0.9f; break;}
				}

				if (Random.value<targetValue) 
				{
					StartCoroutine(WaitForAmbushEvent(selectedForMission));
					//GameEventManager.mainEventManager.DoEvent(new AmbushEvent(),assignedRegion,selectedForMission);
				}
				else StartEncounter(selectedForMission);
				//EncounterCanvasHandler.mainEncounterCanvasHandler.StartNewEncounter(assignedRegion,selectedForMission);
				//EndDialog();
				//PartyManager.mainPartyManager.EnterPartyIntoEncounter(selectedForMission);
				//Use this to indicate encounter having been visited

			
		}
	}

	public void ScoutMorePressed()
	{
		StartCoroutine(ScoutMoreRoutine());
	}
	IEnumerator ScoutMoreRoutine()
	{
		GameEventManager.mainEventManager.DoEvent(new CleanupEvent(),assignedRegion,selectedForMission);
		while (GameEventManager.mainEventManager.drawingEvent) yield return new WaitForFixedUpdate();
		if (GameManager.main.gameStarted)
		{
			if (assignedRegion.ambientThreatNumber<=0) scoutMoreButton.SetActive(false);
		}
		yield break;
	}

	IEnumerator WaitForAmbushEvent(List<PartyMember> eventMembers)
	{
		yield return StartCoroutine(GameEventManager.mainEventManager.WaitForEventEnd(new AmbushEvent(),assignedRegion,eventMembers));
		List<PartyMember> survivingMembers=new List<PartyMember>();
		foreach (PartyMember member in eventMembers)
		{
			if (PartyManager.mainPartyManager.partyMembers.Contains(member)) survivingMembers.Add(member);
		}
		if (GameManager.main.gameStarted && survivingMembers.Count>0) StartEncounter(survivingMembers);
		yield break;
	}

	void StartEncounter(List<PartyMember> encounterMembers)
	{
		//Highlights the region in white, marking the encounter as visited
		assignedRegion.visible=true;
		MapManager.main.EnterEncounter(assignedRegion.regionalEncounter,encounterMembers,false);
	}

	public void CancelPressed()
	{
		EndDialog();
	}
}
