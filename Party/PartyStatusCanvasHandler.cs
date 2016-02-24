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
	public Button turnButton;
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
		turnButton.onClick.RemoveAllListeners();
		turnButton.onClick.AddListener(()=>
		{
			PartyManager.mainPartyManager.AdvanceMapTurn();
			this.ResetTimeTurnButton(true);
		});
		displayEnabled=true;
		ResetTimeTurnButton(true);
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
	
	public void ResetTimeTurnButton(bool enableButton)
	{
		/*
		turnButton.gameObject.SetActive(false);
		if (GameManager.main.gameStarted)
		{
			if (!EncounterCanvasHandler.main.encounterOngoing && !GameEventManager.mainEventManager.drawingEvent)
			{
				
			}
		}*/
		if (enableButton)
		{
			turnButton.gameObject.SetActive(true);
			string buttonText="";
			if (PartyManager.mainPartyManager.dayTime==12) buttonText="Daytime";
			else buttonText="Night";
			turnButton.GetComponentInChildren<Text>().text=buttonText;
		}
		else turnButton.gameObject.SetActive(false);
	}
	
	public void RefreshAssignmentButtons()
	{
		RefreshAssignmentButtons(PartyManager.mainPartyManager.selectedMembers);
	}
	
	public void RefreshAssignmentButtons(List<PartyMember> selected)
	{
		/*
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
			
			//Make sure members don't have an active task
			foreach (PartyMember member in selected)
			{
				//if (!MapManager.main.memberTokens[member].moved)
				//{
					AssignedTaskTypes emptyVar;
					if (!PartyManager.mainPartyManager.GetAssignedTask(member,out emptyVar)) membersFreeToAct.Add(member);
				//}
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
		*/
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
			if (turnButton.gameObject.activeInHierarchy)
			{
				if (GameManager.main.gameStarted)
				{
					if (EncounterCanvasHandler.main.encounterOngoing || GameEventManager.mainEventManager.drawingEvent)
					{
						ResetTimeTurnButton(false);
					}
				}
			}
			else
			{
				if (GameManager.main.gameStarted)
				{
					if (!EncounterCanvasHandler.main.encounterOngoing && !GameEventManager.mainEventManager.drawingEvent)
					{
						ResetTimeTurnButton(true);
					}
				}
			}	
			//mapXText.text="X:"+PartyManager.mainPartyManager.mapCoordX.ToString();
			//mapYText.text="Y:"+PartyManager.mainPartyManager.mapCoordY.ToString();
			timeText.text="Day:"+PartyManager.mainPartyManager.daysPassed;//PartyManager.mainPartyManager.dayTime.ToString()+":00";
			ammoText.text="Ammo:"+PartyManager.mainPartyManager.ammo.ToString();
		}
	}
}
