using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PartyStatusCanvasHandler : MonoBehaviour {

	bool displayEnabled=false;
	
	public Text mapXText;
	public Text mapYText;
	public Text timeText;
	public Text ammoText;
	public Button assignmentButton;
	public Button exploreButton;
	public Transform memberCanvasGroup;
	
	public static PartyStatusCanvasHandler main;
	
	public NotePanelHandler notePanelPrefab;
	
	public void NewNotification(string notificationText) {NewNotification(notificationText,2f);}
	public void NewNotification(string notificationText, float noteLifetime)
	{
		//if (displayEnabled)
		{
			NotePanelHandler newPanel=Instantiate(notePanelPrefab);
			newPanel.AssignNote(notificationText,noteLifetime);
			//newPanel.transform.SetParent(transform,false);
		}
	}
	
	public void EnableStatusDisplay()
	{
		GetComponent<Canvas>().enabled=true;
		displayEnabled=true;
	}
	public void DisableStatusDisplay()
	{
		GetComponent<Canvas>().enabled=false;
		displayEnabled=false;
	}
	/*
	IEnumerator TimeFlashSequence()
	{
		float flashingTime=3f;
		while (flashingTime>0)
		{
			timeText.GetComponentInParent<Image>().CrossFadeAlpha(0.5f,flashingTime,false);
		}
	}*/
	/*
	public void StartTimeFlash()
	{
		float flashingTime=3f;
		timeText.GetComponentInParent<Image>().CrossFadeAlpha(0.5f,flashingTime,false);
	}*/
	
	public void RefreshAssignmentButtons()
	{
		RefreshAssignmentButtons(PartyManager.mainPartyManager.selectedMembers);
	}
	
	public void RefreshAssignmentButtons(List<PartyMember> selected)
	{
		if (selected.Count==0 || EncounterCanvasHandler.main.encounterOngoing)
		{
			assignmentButton.gameObject.SetActive(false);
			exploreButton.gameObject.SetActive(false);
		}
		else
		{
			assignmentButton.gameObject.SetActive(false);
			exploreButton.gameObject.SetActive(false);
			MapRegion checkedRegion=selected[0].currentRegion;
			List<PartyMember> membersFreeToAct=new List<PartyMember>();
			
			foreach (PartyMember member in selected)
			{
				if (!MapManager.main.memberTokens[member].moved)
				{
					membersFreeToAct.Add(member);
				}
			}
			//First-see if you can add non-acting members to acting members
			if (membersFreeToAct.Count>0)
			{
				if (checkedRegion.hasEncounter)
				{
					exploreButton.gameObject.SetActive(true);
					//exploreButton.GetComponentInChildren<Text>().text="Explore";
					exploreButton.onClick.RemoveAllListeners();
					exploreButton.onClick.AddListener(()=>
					{
						if (MapManager.main.scoutingHandler.GetComponent<Canvas>().enabled) MapManager.main.scoutingHandler.EndDialog();
						else MapManager.main.scoutingHandler.StartDialog(checkedRegion);
						//MapManager.main.scoutingHandler.StartDialog(checkedRegion);
						RefreshAssignmentButtons(PartyManager.mainPartyManager.selectedMembers);
					});	
				}
			}
		}
		
		foreach (PartyMemberCanvasHandler memberCanvas in PartyManager.mainPartyManager.partyMemberCanvases.Values)
		{
			memberCanvas.RefreshAssignmentButton();
		}
	}
	
	void Start() {main=this;}
	
	// Update is called once per frame
	void Update () 
	{
		if (displayEnabled)
		{
			//mapXText.text="X:"+PartyManager.mainPartyManager.mapCoordX.ToString();
			//mapYText.text="Y:"+PartyManager.mainPartyManager.mapCoordY.ToString();
			timeText.text="Time:"+PartyManager.mainPartyManager.dayTime.ToString()+":00";
			ammoText.text="Ammo:"+PartyManager.mainPartyManager.ammo.ToString();
		}
	}
}
