using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapScoutingHandler : MonoBehaviour {

	public static bool scoutingDialogOngoing=false;

	public static MapScoutingHandler main;

	public Text descriptionText;
	public Text selectCountText;
	public Button confirmButton;
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
			confirmButton.gameObject.SetActive(true);
			confirmButton.GetComponentInChildren<Text>().text="Scout";
			selectCountText.text="";
			ambushThreatText.text="";
			selectionText.text="";	
		}
		else 
		{
			UpdateAfterScouting();
		}
		TryRefreshEncounterDialog();
	}

	void UpdateAfterScouting()
	{
		if (assignedRegion.regionalEncounter!=null)
		{
			descriptionText.text=assignedRegion.regionalEncounter.lootDescription+" infested with "+assignedRegion.regionalEncounter.enemyDescription;
			confirmButton.gameObject.SetActive(true);
			confirmButton.GetComponentInChildren<Text>().text="Enter ("+PartyMember.fatigueIncreasePerEncounter+" fatigue)";
			foreach (PartyMember member in assignedRegion.localPartyMembers)//PartyManager.mainPartyManager.selectedMembers) 
			{
				MissionSelectorHandler newSelector=Instantiate (memberSelectorPrefab) as MissionSelectorHandler;
				newSelector.AssignMember(member);
				newSelector.GetComponent<Button>().onClick.AddListener(()=>ToggleMissionMember(newSelector));
				newSelector.transform.SetParent(memberSelectorGroup,false);
				selectCountText.text=selectedForMission.Count+"/"+assignedRegion.regionalEncounter.maxRequiredMembers;//+EncounterCanvasHandler.encounterMaxPlayerCount;
				selectionText.text="";
			}
		}
		else
		{
			//If a region has no encounter, assume that it has an event
			descriptionText.text=assignedRegion.regionalEvent.GetScoutingDescription();
			if (!assignedRegion.regionalEvent.eventCompleted)
			{
				confirmButton.GetComponentInChildren<Text>().text="Investigate";
				//confirmButtonText.GetComponentInParent<Button>().interactable=true;
			}
			//else confirmButtonText.GetComponentInParent<Button>().gameObject.SetActive(false);
		}
	}

	public void TryRefreshEncounterDialog()
	{
		if (scoutingDialogOngoing)
		{
			if (assignedRegion.scouted)
			{
				if (assignedRegion.regionalEncounter!=null)
				{
					ambushThreatText.text="Ambush threat:"+assignedRegion.CalculateThreatLevel(selectedForMission.Count);
					if (assignedRegion.ambientThreatNumber<=0) scoutMoreButton.SetActive(false);
					else 
					{
						scoutMoreButton.SetActive(true);
						//if (selectedForMission.Count>0)
						//scoutMoreButton.GetComponent<Button>().interactable=true;
						//else scoutMoreButton.GetComponent<Button>().interactable=false;
					}
					//Prevent from entering encounter when no party members are selected
					if (selectedForMission.Count>0) confirmButton.interactable=true;
					else confirmButton.interactable=false;
				}
				else
				{
					//If a region has no encounter, assume that it has an event
					selectCountText.text="";
					ambushThreatText.text="";
					selectionText.text="";	
					if (!assignedRegion.regionalEvent.eventCompleted)
					{
						confirmButton.gameObject.SetActive(true);
						confirmButton.interactable=true;
					}
					else confirmButton.gameObject.SetActive(false);
					scoutMoreButton.SetActive(false);
				}
			}
			else 
			{
				scoutMoreButton.SetActive(false);
				confirmButton.interactable=true;
			}

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
		//&& selectedForMission.Count<assignedRegion.regionalEncounter.maxAllowedMembers
		&& handler.assignedMember.CheckEnoughFatigue(PartyMember.fatigueIncreasePerEncounter)
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
		selectCountText.text=selectedForMission.Count+"/"+assignedRegion.regionalEncounter.maxRequiredMembers;
		if (selectedForMission.Count==0) {selectionText.text="";}
		else
		{
			selectionText.text="";
			foreach (PartyMember selectedMember in selectedForMission) {selectionText.text+=selectedMember.name+",";}
			selectionText.text=selectionText.text.Remove(selectionText.text.LastIndexOf(","));
		}
		TryRefreshEncounterDialog();
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
			if (assignedRegion.regionalEncounter!=null)
			{
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
			else
			{
				//If a region has no encounter, assume that it has an event
				EndDialog();
				GameEventManager.mainEventManager.DoEvent(assignedRegion.regionalEvent,assignedRegion,assignedRegion.localPartyMembers);
			}
			
		}
	}

	public void ScoutMorePressed()
	{
		StartCoroutine(ScoutMoreRoutine());
	}
	IEnumerator ScoutMoreRoutine()
	{
		GameEventManager.mainEventManager.DoEvent(new CleanupEvent(),assignedRegion,assignedRegion.localPartyMembers);
		while (GameEventManager.mainEventManager.drawingEvent) yield return new WaitForFixedUpdate();
		if (GameManager.main.gameStarted)
		{
			TryRefreshEncounterDialog();
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

	void Start()
	{
		main=this;
	}
}
