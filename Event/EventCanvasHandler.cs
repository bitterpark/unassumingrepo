using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EventCanvasHandler : MonoBehaviour 
{
	public Button decisionButtonPrefab;
	public Text descriptionText;
	
	GameEvent assignedEvent;
	//bool decisionMade;
	
	public void AssignEvent(GameEvent newEvent)
	{
		assignedEvent=newEvent;
		descriptionText.text=newEvent.GetDescription();
		//decisionMade=false;
		List<string> choices=newEvent.GetChoices();
		
		foreach (string choice in choices)
		{
			Button newButton=Instantiate(decisionButtonPrefab);
			newButton.transform.SetParent(transform.FindChild("Event Panel").FindChild("Decision Group"),false);
			//this is required, otherwise lambda listener only captures the last choice iteration
			newButton.GetComponentInChildren<Text>().text=choice;
			newButton.onClick.AddListener(()=>ResolveChoice(newButton.GetComponentInChildren<Text>().text));
		}
	}
	
	public void ResolveChoice(string choiceString)
	{
		//Resolve the decision
		descriptionText.text=assignedEvent.DoChoice(choiceString);
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
		GameObject.Destroy(this.gameObject);
	}
}
