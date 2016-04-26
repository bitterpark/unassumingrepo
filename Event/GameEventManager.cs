using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameEventManager : MonoBehaviour 
{

	//Dictionary<float,GameEvent> eventPossibilities=new Dictionary<float, GameEvent>();
	List<EventChance> possibleEvents=new List<EventChance>();
	List<EventChance> moraleEvents=new List<EventChance>();
	List<EventChance> friendshipEvents=new List<EventChance>();
	List<EventChance> grudgeEvents=new List<EventChance>();
	List<EventChance> coldEvents=new List<EventChance>();
	List<EventChance> campSafetyEvents=new List<EventChance>();
	List<EventChance> scavengingEvents=new List<EventChance>();
	PriorityList<PersistentEvent> possiblePersistentEvents=new PriorityList<PersistentEvent>();
	public static GameEventManager mainEventManager;
	
	GameEvent drawnEvent=null;
	string currentDescription=null;
	public bool drawingEvent=false;
	bool choiceMade=false;
	List<NewEventInfo> queuedEvents=new List<NewEventInfo>();
	
	public struct NewEventInfo
	{
		public GameEvent newEvent;
		public MapRegion eventRegion;
		public List<PartyMember> eventMembers;
		public NewEventInfo(GameEvent addedEvent, MapRegion region, List<PartyMember> members)
		{
			newEvent=addedEvent;
			eventRegion=region;
			eventMembers=members;
		}
	
	}
	
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
		public EventChance(GameEvent newEvent,float newChance)
		{
			chance=newChance;
			myEvent=newEvent;
		}
	}

	public PersistentEvent GetPersistentEvent()
	{
		PersistentEvent result=null;
		if (possiblePersistentEvents.GetCount()>0)
		{
			result=possiblePersistentEvents.Get();
			if (!result.repeatable) possiblePersistentEvents.TryRemove(result);
		}
		return result;
	}

	public void RollCampEvents(MapRegion eventRegion,List<PartyMember> presentMembers)
	{
		float eventsRoll=Random.value;
		EventChance trackedEventRecord=new EventChance();
		//Morale events
		GameEvent resEvent=PickRandomEvent(moraleEvents, ref trackedEventRecord,eventsRoll, eventRegion, presentMembers);
		if (resEvent!=null) QueueEvent(resEvent,eventRegion,presentMembers);//StartEventDraw(resEvent, eventRegion, presentMembers);
		//Cold events
		resEvent=PickRandomEvent(coldEvents, ref trackedEventRecord,eventsRoll, eventRegion, presentMembers);
		if (resEvent!=null) QueueEvent(resEvent,eventRegion,presentMembers);//StartEventDraw(resEvent, eventRegion, presentMembers);
		//Attack events
		resEvent=PickRandomEvent(campSafetyEvents, ref trackedEventRecord,eventsRoll, eventRegion, presentMembers);
		if (resEvent!=null) QueueEvent(resEvent,eventRegion,presentMembers);
		//Relationship events
		resEvent=PickRandomEvent(grudgeEvents, ref trackedEventRecord,eventsRoll, eventRegion, presentMembers);
		if (resEvent!=null) QueueEvent(resEvent,eventRegion,presentMembers);
		resEvent=PickRandomEvent(friendshipEvents, ref trackedEventRecord,eventsRoll, eventRegion, presentMembers);
		if (resEvent!=null) QueueEvent(resEvent,eventRegion,presentMembers);
	}
	public void RollScavengeEvents(MapRegion eventRegion,List<PartyMember> presentMembers)
	{
		GameEvent resEvent=null;
		//if (eventRegion.hasGasoline) resEvent=new GasolineEvent();
		//else 
		{
			float eventsRoll=Random.value;
			EventChance trackedEventRecord=new EventChance();
			resEvent=PickRandomEvent(scavengingEvents, ref trackedEventRecord,eventsRoll, eventRegion, presentMembers);
		}
		if (resEvent!=null) StartEventDraw(resEvent, eventRegion, presentMembers);
	}
	//Currently unused
	public void RollEvents(ref bool moveAllowed, MapRegion eventRegion, List<PartyMember> movedMembers, bool queueEvent)
	{
		//bool eventFired=false;
		GameEvent resEvent=null;
		//if (eventRegion.hasGasoline)resEvent=new GasolineEvent();
		//else
		{
			float eventsRoll=Random.value;
			EventChance trackedEventRecord=new EventChance();
			resEvent=PickRandomEvent(possibleEvents, ref trackedEventRecord,eventsRoll,eventRegion, movedMembers);
			if (!resEvent.repeatable) possibleEvents.Remove(trackedEventRecord);
		}
		//If a regular event fired, remove it from events list
		if (resEvent!=null) 
		{
			//eventFired=true;
			if (queueEvent) {QueueEvent(resEvent,eventRegion,movedMembers);}
			else StartEventDraw(resEvent, eventRegion, movedMembers);
			moveAllowed=resEvent.AllowMapMove();
		}
		else moveAllowed=true;
		
		//return eventFired;
	}
	
	public void DoEvent(GameEvent newEvent, MapRegion eventRegion, List<PartyMember> eventMembers)
	{
		StartEventDraw(newEvent, eventRegion, eventMembers);
	}
	// Used by ambush events, to make sure encounter enter is triggered after the event is resolved (if any members survive it)
	public IEnumerator WaitForEventEnd(GameEvent newEvent, MapRegion eventRegion, List<PartyMember> eventMembers)
	{
		StartEventDraw(newEvent,eventRegion,eventMembers);
		while (drawingEvent && GameManager.main.gameStarted) yield return new WaitForFixedUpdate();
		yield break;
	}
	
	public void QueueEvent(GameEvent newEvent, MapRegion eventRegion, List<PartyMember> eventMembers)
	{
		//StartEventDraw(newEvent, eventRegion, eventMembers);
		queuedEvents.Add(new NewEventInfo(newEvent,eventRegion,eventMembers));
	}
	public void QueueEventToStart(GameEvent newEvent, MapRegion eventRegion, List<PartyMember> eventMembers)
	{
		//StartEventDraw(newEvent, eventRegion, eventMembers);
		queuedEvents.Insert(0,new NewEventInfo(newEvent,eventRegion,eventMembers));
	}
	public void TryNextQueuedEvent()
	{
		if (queuedEvents.Count>0)
		{
			//FIFO
			NewEventInfo firingEvent=queuedEvents[0];
			queuedEvents.Remove(firingEvent);
			//Update members list to remove ones who died
			List<PartyMember> survivingMovedMembers=new List<PartyMember>();
			foreach (PartyMember member in firingEvent.eventMembers)
			{
				if (PartyManager.mainPartyManager.partyMembers.Contains(member)) survivingMovedMembers.Add(member);
			}
			if (survivingMovedMembers.Count>0) DoEvent(firingEvent.newEvent,firingEvent.eventRegion,survivingMovedMembers);
			else return;
		} 
	}
	
	
	GameEvent PickRandomEvent(List<EventChance> eventsList, ref EventChance eventRecord, float roll
	, MapRegion eventRegion,List<PartyMember> movedMembers)
	{
		//find events with met preconditions
		//MapRegion currentTeamRegion=MapManager.main.GetRegion(PartyManager.mainPartyManager.selectedMembers.);
		List<EventChance> eligibleEvents=new List<EventChance>();
		foreach(EventChance chance in eventsList) 
		{
			if (chance.myEvent.PreconditionsMet(eventRegion,movedMembers)) {eligibleEvents.Add (chance);}
		}
		
		//form a dictionary based on probability intervals
		float eventPositiveProbabilitySpace=0;
		foreach (EventChance chance in eligibleEvents) {eventPositiveProbabilitySpace+=chance.chance;}
		
		Dictionary<float,EventChance> intervalDict=new Dictionary<float, EventChance>();
		foreach (EventChance chance in eligibleEvents)
		{
			intervalDict.Add (eventPositiveProbabilitySpace,chance);
			eventPositiveProbabilitySpace-=chance.chance;
		}
		
		//roll on resulting intervals	
		GameEvent resEvent=null;
		//Compiler made me assign this, should be null
		//EventChance eventRecord=new EventChance();
		foreach (float chance in intervalDict.Keys)
		{
			if (roll<chance) 
			{
				resEvent=intervalDict[chance].myEvent;
				eventRecord=intervalDict[chance];
			}
		}
		return resEvent;
	}
	
	void StartEventDraw(GameEvent newDrawnEvent,MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		/*
		drawingEvent=true;
		drawnEvent=newDrawnEvent;
		choiceMade=false;
		//FormatEventDecription(newDrawnEvent.GetDescription());
		currentDescription=newDrawnEvent.GetDescription();*/
		//currentDescription=
		drawingEvent=true;
		EventCanvasHandler newEventScreen=Instantiate(eventScreenPrefab);
		newEventScreen.AssignEvent(newDrawnEvent, eventRegion, movedMembers);
	}
	/*
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
				if (GUI.Button(choicesRect,choice)) {currentDescription=drawnEvent.DoChoice(choice); choiceMade=true;}
				choicesRect.y+=choiceHeight+choiceVPad;
				//i++;
			}
		}
		else
		{
			//Rect closeButtonRect=new Rect();
			if (GUI.Button(choicesRect,"Close")) {EndEventDraw();}
		}
	}*/
	
	public void EndEventDraw()
	{
		/*
		drawnEvent=null;
		drawingEvent=false;
		choiceMade=false;*/
		drawingEvent=false;
		TryNextQueuedEvent();
	}
	
	public void ClearEventQueue() {queuedEvents.Clear();}

	public void GamestartGenerateEventLists()
	{
		scavengingEvents.Clear();
		scavengingEvents.Add(new EventChance(new ScavengeEventOne(),1f));
		scavengingEvents.Add(new EventChance(new CarFindEvent(),0.1f));
		scavengingEvents.Add (new EventChance(new CacheInAnomaly(),0.04f));
		//scavengingEvents.Add( new EventChance(new NewSurvivor(),0.04f));
		scavengingEvents.Add( new EventChance(new MedicalCache(),0.04f));
		//scavengingEvents.Add (new EventChance(new SurvivorRescue(),0.04f));
		//scavengingEvents.Add(new EventChance(new SearchForSurvivor(),0.04f));

		moraleEvents.Clear();
		//moraleEvents.Add (new EventChance(new LowMoraleSpiral(),0.2f));
		moraleEvents.Add(new EventChance(new LowMoraleSteal(),0.4f));
		moraleEvents.Add (new EventChance(new LowMoraleFight(),0.25f));
		//moraleEvents.Add (new EventChance(new LowMoraleEnmity(),0.10f));
		moraleEvents.Add (new EventChance(new LowMoraleQuit(),0.15f));

		coldEvents.Clear();
		coldEvents.Add(new EventChance(new MemberIsCold(),1f));
		coldEvents.Add(new EventChance(new MembersAreFreezing(),1f));

		campSafetyEvents.Clear();
		campSafetyEvents.Add(new EventChance(new AttackOnCamp(),1f));

		friendshipEvents.Clear();
		friendshipEvents.Add(new EventChance(new HighMoraleFriendship(),1f));
		grudgeEvents.Clear();
		grudgeEvents.Add(new EventChance(new LowMoraleEnmity(),1f));

		possiblePersistentEvents.ClearList();
		possiblePersistentEvents.Add(new WoundedSurvivor(),2);
		possiblePersistentEvents.Add(new ScrapTrade(),1);
	}

	void Start()
	{
		mainEventManager=this;
		//possibleEvents.Add (new EventChance(new FoodSpoilage(),0.04f));
		//possibleEvents.Add (new EventChance(new MonsterAttack(),0.04f));
		//Putting total probability space to >1 should work properly, as long as it is done as the first probability
		
		//possibleEvents.Add (new EventChance(new LostInAnomaly(),0.04f));
		/*
		possibleEvents.Add( new EventChance(new NewSurvivor(),0.04f));
		possibleEvents.Add( new EventChance(new MedicalCache(),0.04f));
		possibleEvents.Add (new EventChance(new SurvivorRescue(),0.04f));
		possibleEvents.Add(new EventChance(new SearchForSurvivor(),0.04f));
		*/


		GameManager.GameOver+=ClearEventQueue;
		/*
		//Add highest first for proper rolling
		eventPossibilities.Add (0.16f,new LostInAnomaly());
		eventPossibilities.Add (0.12f,new CacheInAnomaly());
		eventPossibilities.Add (0.08f,new MonsterAttack());
		eventPossibilities.Add (0.04f,new FoodSpoilage());//0.04f, new FoodSpoilage());
		*/
		//eventPossibilities.Add (0.05f,new NewSurvivor());
		
	}	
	/*
	void OnGUI()
	{
		if (drawingEvent) {DrawEvent();}
	}*/
}
