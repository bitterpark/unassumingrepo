using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EventCanvasHandler : MonoBehaviour 
{
	public Button decisionButtonPrefab;
	public Text descriptionText;
	
	GameEvent assignedEvent;
	MapRegion currentTeamRegion;
	List<PartyMember> teamList;
	//bool decisionMade;
	
	public void AssignEvent(GameEvent newEvent, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		GameEventManager.mainEventManager.drawingEvent=true;
		currentTeamRegion=eventRegion;
		teamList=movedMembers;
		assignedEvent=newEvent;
		descriptionText.text=newEvent.GetDescription(currentTeamRegion,teamList);
		//decisionMade=false;
	
		List<EventChoice> choices=newEvent.GetChoices(currentTeamRegion,teamList);
		
		foreach (EventChoice choice in choices)
		{
			Button newButton=Instantiate(decisionButtonPrefab);
			newButton.transform.SetParent(transform.FindChild("Event Panel").FindChild("Decision Group"),false);
			//this is required, otherwise lambda listener only captures the last choice iteration
			newButton.GetComponentInChildren<Text>().text=choice.choiceTxt;
			newButton.interactable=!choice.grayedOut;
			newButton.onClick.AddListener(()=>ResolveChoice(newButton.GetComponentInChildren<Text>().text));
		}
	}
	
	public void ResolveChoice(string choiceString)
	{
		//Find current team's region
		//MapRegion currentTeamRegion=MapManager.main.GetRegion(PartyManager.mainPartyManager.selectedMembers[0].worldCoords);
		//Resolve the decision
		descriptionText.text=assignedEvent.DoChoice(choiceString,currentTeamRegion, teamList);
		//Destroy all choice buttons
		Button[] decisionButtons=transform.FindChild("Event Panel").FindChild("Decision Group").GetComponentsInChildren<Button>();
		foreach (Button btn in decisionButtons) {GameObject.Destroy(btn.gameObject);}
		
		//Create exit button
		Button newButton=Instantiate(decisionButtonPrefab);
		newButton.transform.SetParent(transform.FindChild("Event Panel").FindChild("Decision Group"),false);
		newButton.GetComponentInChildren<Text>().text="Close";
		newButton.onClick.AddListener(CloseChoiceScreen);
	}
	
	public void CloseChoiceScreen() 
	{
		GameEventManager.mainEventManager.EndEventDraw();
		GameObject.Destroy(this.gameObject);
	}
	
	void Start()
	{
		//GetComponent<Canvas>().worldCamera=GameObject.Find("GUI Cam").GetComponent<Camera>();
	}
}
