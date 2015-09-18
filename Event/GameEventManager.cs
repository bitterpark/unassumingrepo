using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameEventManager : MonoBehaviour 
{

	//Dictionary<float,GameEvent> eventPossibilities=new Dictionary<float, GameEvent>();
	List<EventChance> possibleEvents=new List<EventChance>();
	public static GameEventManager mainEventManager;
	
	GameEvent drawnEvent=null;
	string currentDescription=null;
	bool drawingEvent=false;
	bool choiceMade=false;
	
	public EventCanvasHandler eventScreenPrefab;
	/*
	void FormatEventDecription(string rawDesc)
	{
		string workString=rawDesc;
		currentDescription="";
		//cut desc up into lines
		int lineCharLimit=10;
		int spaceIndex=0;
		string newLine="";
		while (true)
		{
			
			if ((workString.Contains("\n") && workString.IndexOf("\n")<=lineCharLimit) | workString.Length<=lineCharLimit) 
			{
				currentDescription+=newLine;
				currentDescription+=workString;
				break;
			}
			else
			{
				if (newLine.Length+workString.IndexOf(" ")>lineCharLimit) 
				{
					currentDescription+=newLine+"\n";
					newLine="";
				}
				newLine+=workString.Substring(0,workString.IndexOf(" "));
				workString.Remove(0,workString.IndexOf(" ")+1);
			}
		}
	}*/
	struct EventChance
	{
		public float chance;
		public GameEvent myEvent;
		public EventChance(float newChance,GameEvent newEvent)
		{
			chance=newChance;
			myEvent=newEvent;
		}
	}
	
	public bool RollEvents()
	{
		/*
		Dictionary<float,GameEvent> eligibleEvents=new Dictionary<float, GameEvent>();
		foreach (float key in eventPossibilities.Keys) 
		{
			if (eventPossibilities[key].PreconditionsMet()) {eligibleEvents.Add(key,eventPossibilities[key]);}
		
		}*/
		//find events with met preconditions
		List<EventChance> eligibleEvents=new List<EventChance>();
		foreach(EventChance chance in possibleEvents) 
		{
			if (chance.myEvent.PreconditionsMet()) {eligibleEvents.Add (chance);}
		}
		
		//form a dictionary based on probability intervals
		float eventPositiveProbabilitySpace=0;
		foreach (EventChance chance in eligibleEvents) {eventPositiveProbabilitySpace+=chance.chance;}
		
		Dictionary<float,GameEvent> intervalDict=new Dictionary<float, GameEvent>();
		foreach (EventChance chance in eligibleEvents)
		{
			intervalDict.Add (eventPositiveProbabilitySpace,chance.myEvent);
			eventPositiveProbabilitySpace-=chance.chance;
		}
		
		//roll on resulting intervals	
		float roll=Random.value;
		GameEvent resEvent=null;
		foreach (float chance in intervalDict.Keys)
		{
			if (roll<chance) {resEvent=intervalDict[chance];}
		}
		if (resEvent!=null) 
		{
			StartEventDraw(resEvent);
			return resEvent.AllowMapMove();
		}
		return true;
	}
	
	void StartEventDraw(GameEvent newDrawnEvent)
	{
		/*
		drawingEvent=true;
		drawnEvent=newDrawnEvent;
		choiceMade=false;
		//FormatEventDecription(newDrawnEvent.GetDescription());
		currentDescription=newDrawnEvent.GetDescription();*/
		//currentDescription=
		EventCanvasHandler newEventScreen=Instantiate(eventScreenPrefab);
		newEventScreen.AssignEvent(newDrawnEvent);
	}
	
	void DrawEvent()
	{
		//main bg
		
		float eventScreenWidth=800;
		float eventScreenHeight=350;
		Rect eventScreenRect=new 
		 Rect(Screen.width*0.6f-eventScreenWidth*0.5f,Screen.height*0.5f-eventScreenHeight*0.5f,eventScreenWidth,eventScreenHeight);
		GUI.Box(eventScreenRect,"");
		
		//description
		Rect eventDescRect=new Rect(eventScreenRect);
		eventDescRect.x+=20;
		eventDescRect.y+=25;
		eventDescRect.width*=0.8f;
		eventDescRect.height*=0.6f;
		GUIStyle style = new GUIStyle("box");	
		style.wordWrap = true;
		GUI.Box (eventDescRect,currentDescription,style);
		
		float choiceWidth=100f;
		float choiceHeight=30f;
		float choiceVPad=5f;
		Rect choicesRect=new Rect(eventScreenRect);
		choicesRect.x+=30;
		choicesRect.y+=eventScreenRect.height-(choiceHeight+choiceVPad);
		choicesRect.width=choiceWidth;
		choicesRect.height=choiceHeight;
		
		if (!choiceMade)
		{
			//int i=0;
			List<string> choices=drawnEvent.GetChoices();
			choicesRect.y-=(choiceHeight+choiceVPad)*choices.Count-1;
			foreach (string choice in choices)
			{
				if (GUI.Button(choicesRect,choice)) {currentDescription=drawnEvent.DoChoice(choice);/*FormatEventDecription(drawnEvent.DoChoice(choice));*/ choiceMade=true;}
				choicesRect.y+=choiceHeight+choiceVPad;
				//i++;
			}
		}
		else
		{
			//Rect closeButtonRect=new Rect();
			if (GUI.Button(choicesRect,"Close")) {EndEventDraw();}
		}
	}
	
	void EndEventDraw()
	{
		drawnEvent=null;
		drawingEvent=false;
		choiceMade=false;
	}
	
	void Start()
	{
		mainEventManager=this;
		possibleEvents.Add (new EventChance(0.04f,new FoodSpoilage()));
		possibleEvents.Add (new EventChance(0.04f,new MonsterAttack()));
		possibleEvents.Add (new EventChance(0.04f,new CacheInAnomaly()));
		possibleEvents.Add (new EventChance(0.04f,new LostInAnomaly()));
		/*
		//Add highest first for proper rolling
		eventPossibilities.Add (0.16f,new LostInAnomaly());
		eventPossibilities.Add (0.12f,new CacheInAnomaly());
		eventPossibilities.Add (0.08f,new MonsterAttack());
		eventPossibilities.Add (0.04f,new FoodSpoilage());//0.04f, new FoodSpoilage());
		*/
		//eventPossibilities.Add (0.05f,new NewSurvivor());
		
	}	
	
	void OnGUI()
	{
		if (drawingEvent) {DrawEvent();}
	}
}
