using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum AssignedTaskTypes {Rest,BuildCamp};
public struct AssignedTask
{
	public AssignedTaskTypes taskType;
	public System.Func<bool> preconditionCheck;
	public System.Action actionToPerform;
	public System.Action startTaskAction;
	public System.Action endTaskAction;
	public PartyMember performingMember;
	public AssignedTask(PartyMember member, AssignedTaskTypes type, System.Func<bool> check, System.Action mainAction)
	{
		taskType=type;
		preconditionCheck=check;
		actionToPerform=mainAction;
		performingMember=member;
		startTaskAction=null;
		endTaskAction=null;
	}
	public AssignedTask(PartyMember member, AssignedTaskTypes type, System.Func<bool> check
	, System.Action mainAction, System.Action startAction, System.Action endAction)
	{
		taskType=type;
		preconditionCheck=check;
		actionToPerform=mainAction;
		performingMember=member;
		if (startAction!=null) startTaskAction=startAction;
		else startTaskAction=null;
		if (endAction!=null) endTaskAction=endAction;
		else endTaskAction=null;
	}
	
	public void DoStartAction()
	{
		if (startTaskAction!=null) startTaskAction.Invoke();
	}
	public void DoEndAction()
	{
		if (endTaskAction!=null) endTaskAction.Invoke();
	}
	
	public Sprite GetTaskSprite()
	{
		Sprite result=null;
		switch (taskType)
		{
			case AssignedTaskTypes.BuildCamp: {result=SpriteBase.mainSpriteBase.buildCampSprite; break;}
			case AssignedTaskTypes.Rest: {result=SpriteBase.mainSpriteBase.restSprite; break;}
		}
		return result;
	}
}

public class PartyManager : MonoBehaviour
{
	public static PartyManager mainPartyManager;//mainPlayerState;
	
	//public int dayTime;//=0;
	//public int timePassed=0;
	//TIME
	public int daysPassed=0;
	public static int campaignDays=5;
	public int daysLeft
	{
		get {return _daysLeft;}
		set 
		{
			_daysLeft=value;
			if (_daysLeft<=0) 
			{
				//GameEventManager.mainEventManager.QueueEventToStart(new GameWinEvent(),partyMembers[0].currentRegion,partyMembers);
				//GameEventManager.mainEventManager.TryNextQueuedEvent();//GameManager.main.EndCurrentGame(true);

			}
		}
	}
	int _daysLeft=0;
	public int dayTime=12;
	
	//public int mapCoordX;//=0;
	//public int mapCoordY;//=0;
	//public Vector2 GetPartyMapCoords() {return new Vector2(mapCoordX,mapCoordY);}
	
	//public bool hasCook=false;
	
	public List<PartyMember> partyMembers;
	public List<PartyMember> selectedMembers;
	public void AddSelectedMember(PartyMember addedMember)
	{
		if (!partyMembers.Contains(addedMember)) throw new System.Exception("Attempting to select party member that doesn't exist!");
		selectedMembers.Add(addedMember);
		PartyStatusCanvasHandler.main.RefreshAssignmentButtons(selectedMembers);
	}
	public void RemoveSelectedMember(PartyMember removedMember)
	{
		if (!selectedMembers.Contains(removedMember)) throw new System.Exception("Attempting to remove non-selected member from selection!");
		selectedMembers.Remove(removedMember);
		PartyStatusCanvasHandler.main.RefreshAssignmentButtons(selectedMembers);
	}
	//public List<PartyMemberSelector> selectors;
	
	public PartyStatusCanvasHandler statusCanvas;
	public PartyMemberCanvasHandler partyMemberCanvasPrefab;
	public Dictionary<PartyMember,PartyMemberCanvasHandler> partyMemberCanvases; 
	
	//const int encounterFatigueCost=10;
	
	public int ammo
	{
		get {return _ammo;}
		set 
		{
			_ammo=value;
			if (_ammo<0) {_ammo=0;}
		}
	}
	int _ammo;
	
	public int gas;

	//public int partyVisibilityMod;
	
	//List<InventoryItem> partyInventory;
	//public List<InventoryItem> GetPartyInventory() {return partyInventory;}
	
	public delegate void InventoryChangedDeleg();
	public static event InventoryChangedDeleg InventoryChanged;
	
	public delegate void TimePassedDeleg();
	public static event TimePassedDeleg ETimePassed;
	public delegate void TimePassedEndDeleg();
	public static event TimePassedEndDeleg ETimePassedEnd;
	
	Dictionary<PartyMember,AssignedTask> assignedTasks;
	public bool GetAssignedTask(PartyMember member, out AssignedTaskTypes type)
	{
		type=AssignedTaskTypes.BuildCamp;
		if (!assignedTasks.ContainsKey(member)) return false;
		else
		{
			type=assignedTasks[member].taskType;
			return true;
		}
	}
	public void AssignMemberNewTask(AssignedTask newTask)
	{
		if (assignedTasks.ContainsKey(newTask.performingMember)) throw new System.Exception("Trying to assign task to member with task!");
		if (newTask.preconditionCheck.Invoke())
		{
			newTask.DoStartAction();
			assignedTasks.Add(newTask.performingMember,newTask);
			MapManager.main.memberTokens[newTask.performingMember].NewTaskSet(newTask.GetTaskSprite());
			PartyStatusCanvasHandler.main.RefreshAssignmentButtons(selectedMembers);
			//MapManager.main.memberTokens[newTask.performingMember].moved=true;
		}
	}
	
	public void TryRemoveMemberTask(PartyMember member)
	{
		if (assignedTasks.ContainsKey(member))
		{
			assignedTasks[member].DoEndAction();
			assignedTasks.Remove(member);
			MapManager.main.memberTokens[member].TaskRemoved();//.moved=false;
			PartyStatusCanvasHandler.main.RefreshAssignmentButtons(selectedMembers);
		}
	}
	public void RemoveMemberTask(PartyMember member)
	{
		if (!assignedTasks.ContainsKey(member)) throw new System.Exception("Trying to remove task from member with no task!");
		else TryRemoveMemberTask(member);
	}
	
	//Setup start of game
	public void SetDefaultState()
	{
		MapRegion startingRegion = MapManager.main.GetTown();//Random.Range(0,MapManager.main.townCenterRegions.Count)];
		//partyStatusCanvas.gameObject.SetActive(true);
		statusCanvas.EnableStatusDisplay();
		partyMemberCanvases=new Dictionary<PartyMember, PartyMemberCanvasHandler>();
		//dayTime=5;
		
		//mapCoordX=0;
		//mapCoordY=0;
		//partyVisibilityMod=0;
		//foodSupply=0;//2;
		daysLeft=campaignDays;
		daysPassed = 0;
		ammo=0;//500;//5;
		gas=0;
		
		partyMembers=new List<PartyMember>();
		selectedMembers=new List<PartyMember>();
		assignedTasks=new Dictionary<PartyMember, AssignedTask>();
		//selectors=new List<PartyMemberSelector>();
		
		//AddNewPartyMember(new PartyMember(startingRegion));
		//AddNewPartyMember(new PartyMember(startingRegion));
		
		//startingRegion.SetCar(true);

		//MapManager.main.TeleportToRegion(startingRegion);
		startingRegion.StashItem(new FoodBig());
		startingRegion.StashItem(new FoodBig());
		startingRegion.StashItem(new FoodSmall());
		startingRegion.StashItem(new FoodSmall());

		startingRegion.StashItem(new Pills());
		startingRegion.StashItem(new Pills());
		startingRegion.StashItem(new Pills());

		startingRegion.StashItem(new Scrap());
		startingRegion.StashItem(new Scrap());
		startingRegion.StashItem(new Scrap());
		startingRegion.StashItem(new ComputerParts());
		startingRegion.StashItem(new ComputerParts());
		startingRegion.StashItem(new ComputerParts());

		startingRegion.StashItem(new Pipe());

		//Do this to setup proper background color
		//PassTime(1);
		MapManager.main.FocusViewOnRegion(startingRegion.GetComponent<RectTransform>());
	}
	/*
	void PassTime(int hoursPassed)
	{
		
		dayTime=(int)Mathf.Repeat(dayTime+12,24);//hoursPassed;
		string timePassageText="";
		if (dayTime>0) timePassageText="Daytime";
		else 
		{
			timePassageText="Night";
			daysPassed+=1;
			foreach (PartyMember member in partyMembers) member.skillpoints+=1;
		}
		PartyStatusCanvasHandler.main.NewNotification(timePassageText);
		//dayTime=(int)Mathf.Repeat(dayTime+hoursPassed,24);
		//foreach (PartyMember member in partyMembers) {member.hunger+=10*hoursPassed;}
		if (ETimePassed!=null) {ETimePassed(hoursPassed);}
		
		//Adjust camera bg color
		float lightBottomThreshold=0.5f;
		float noonLightLevelBonus=0.5f;
		InventoryScreenHandler.mainISHandler.RefreshInventoryItems();
		//float newb=lightBottomThreshold+noonLightLevelBonus-(noonLightLevelBonus/12)*(Mathf.Abs(dayTime-12));//Mathf.Repeat(Camera.main.backgroundColor.b-0.083//+0.0416f*hoursPassed,1);
		//Camera.main.backgroundColor=new Color(0,0,newb);//
	}*/
	
	public void AdvanceMapTurn()
	{
		if (InventoryScreenHandler.mainISHandler.inventoryShown) InventoryScreenHandler.mainISHandler.CloseScreen();
		/*
		dayTime=(int)Mathf.Repeat(dayTime+12,24);//hoursPassed;
		string timePassageText="";
		if (dayTime>0) timePassageText="Daytime";
		else 
		{
			timePassageText="Night";
			daysPassed+=1;
			daysLeft-=1;
			foreach (PartyMember member in partyMembers) member.skillpoints+=1;
		}*/
		daysPassed++;
		string timePassageText="Day"+daysPassed;
		if (daysLeft>0)
		{
			PartyStatusCanvasHandler.main.NewNotification(timePassageText);
			//dayTime=(int)Mathf.Repeat(dayTime+hoursPassed,24);
			//foreach (PartyMember member in partyMembers) {member.hunger+=10*hoursPassed;}
			if (ETimePassed!=null) {ETimePassed();}
			
			//InventoryScreenHandler.mainISHandler.RefreshInventoryItems();

			//Make sure the rest tasks and time skip occur in proper order
			//List<PartyMember> membersRestingAtCamp=new List<PartyMember>();
			List<MapRegion> campingRegions=new List<MapRegion>();

			//Perform assigned tasks
			foreach (PartyMember member in new List<PartyMember>(assignedTasks.Keys)) 
			{
				if (!assignedTasks[member].preconditionCheck()) RemoveMemberTask(member);
				else
				{
					assignedTasks[member].actionToPerform.Invoke();
					/*
					if (assignedTasks[member].taskType==AssignedTaskTypes.Rest) 
					{
						membersRestingAtCamp.Add(member);
						//THIS WILL CAUSE ISSUES IF MEMBERS DO NOT ALL REST IN THE SAME REGION/TOWN
						campRegion=member.currentRegion;
					}*/
					if (!assignedTasks[member].preconditionCheck()) RemoveMemberTask(member);
				}
			}
			//See if tasks are still not finished (must be in that order)
			foreach (PartyMember member in new List<PartyMember>(assignedTasks.Keys)) 
			{
				if (!assignedTasks[member].preconditionCheck()) RemoveMemberTask(member);
			}

			//Find all current member regions
			foreach (PartyMember member in partyMembers)
			{
				if (!campingRegions.Contains(member.currentRegion)) campingRegions.Add(member.currentRegion);
			}

			//Do morale and other camp events
			foreach (MapRegion region in campingRegions)
			GameEventManager.mainEventManager.RollCampEvents(region,region.localPartyMembers);
			GameEventManager.mainEventManager.TryNextQueuedEvent();
			
			//This may cause issues on gameover
			PartyStatusCanvasHandler.main.RefreshAssignmentButtons(selectedMembers);
			//InventoryScreenHandler.mainISHandler.RefreshInventoryItems();
			if (ETimePassedEnd!=null) ETimePassedEnd();
		}
		else
		{
			GameEventManager.mainEventManager.DoEvent(new GameWinEvent(),partyMembers[0].currentRegion,partyMembers);
		}
		//PartyStatusCanvasHandler.main.StartTimeFlash();
	}
	

	
	public int PartyMemberCount() {return partyMembers.Count;}
	public PartyMember GetPartyMember(int index) {return partyMembers[index];}
	
	public void AddNewPartyMember(PartyMember newMember)
	{
		partyMembers.Add (newMember);
		PartyManager.ETimePassed += newMember.TimePassEffect;
		PartyManager.ETimePassedEnd += newMember.LateTimePassEffect;

		PartyMemberCanvasHandler newPartyMemberCanvas=Instantiate(partyMemberCanvasPrefab);
		newPartyMemberCanvas.AssignPartyMember(newMember);
		newPartyMemberCanvas.transform.SetParent(statusCanvas.memberCanvasGroup,false);
		partyMemberCanvases.Add(newMember,newPartyMemberCanvas);
		
		//MapManager.main.AddMemberToken(newMember);
		//MapManager.main.MoveMembersToRegion(newMember.currentRegion,newMember);
		
		statusCanvas.NewNotification(newMember.name+" has joined the party");
	}
	
	public void RemovePartyMember(PartyMember removedMember) {RemovePartyMember(removedMember,true);}

	public void RemovePartyMember(PartyMember removedMember, bool dead)
	{
		RemoveMemberFromParty(removedMember);
		if (!dead) TownManager.main.MercenaryContractFinished(removedMember);
	}

	void RemoveMemberFromParty(PartyMember removedMember)
	{
		removedMember.currentRegion.localPartyMembers.Remove(removedMember);
		if (assignedTasks.ContainsKey(removedMember)) RemoveMemberTask(removedMember);
		if (selectedMembers.Contains(removedMember)) selectedMembers.Remove(removedMember);
		PartyMemberCanvasHandler deletedHandler = partyMemberCanvases[removedMember];
		partyMemberCanvases.Remove(removedMember);
		GameObject.Destroy(deletedHandler.gameObject);
		partyMembers.Remove(removedMember);

		//this is required for the vertical layout formation to update on time (damn bugs)
		foreach (PartyMemberCanvasHandler memberCanvas in partyMemberCanvases.Values)
		{
			memberCanvas.GetComponent<Canvas>().enabled = false;
			memberCanvas.GetComponent<Canvas>().enabled = true;
		}
		ETimePassed -= removedMember.TimePassEffect;
		ETimePassedEnd -= removedMember.LateTimePassEffect;
	}

	public void MapMemberClicked(PartyMember selectedMember)
	{
		if (selectedMembers.Contains(selectedMember))
		{
			//selectedMembers.Remove(selectedMember);
			MapManager.main.memberTokens[selectedMember].Deselect();
		}
		else 
		{
			if (selectedMembers.Count>0)
			{
				List<PartyMember> membersInWrongLocation=new List<PartyMember>();
				foreach (PartyMember member in selectedMembers)
				{
					if (member.currentRegion!=selectedMember.currentRegion) membersInWrongLocation.Add(member);
				}
				foreach(PartyMember member in membersInWrongLocation) 
				{
					MapManager.main.memberTokens[member].Deselect();
					//selectedMembers.Remove(member);
				}
			} 
			//selectedMembers.Add(selectedMember);
			MapManager.main.memberTokens[selectedMember].Select();
		}
	}
	//Makes sure members can't move too far away, and takes care of move penalties
	public bool ConfirmMapMovement(MapRegion moveRegion, out List<PartyMember> movedList)
	{
		bool moveSuccesful=false;
		//cancel if not enough stamina to move
		/*
		foreach (PartyMember member in partyMembers) 
		{
			if (member.stamina<1) {return moveSuccesful;}
		}*/
		movedList=new List<PartyMember>();
		//float moveLength=1f;//(new Vector2(x,y)-selectedMembers[0].worldCoords).magnitude;//Mathf.Abs(x-mapCoordX)+Mathf.Abs(y-mapCoordY);//Mathf.Max (Mathf.Abs(x-mapCoordX),Mathf.Abs(y-mapCoordY));
		if (selectedMembers[0].currentRegion!=moveRegion 
		&& selectedMembers[0].currentRegion.connections.ContainsKey(moveRegion))
		{
			moveSuccesful=true;
			//If moving between towns, confirm to prime for the event, else - reduce fatigue
			if (selectedMembers[0].currentRegion.connections[moveRegion].isIntercity)
			{
				{
					moveSuccesful=true;
					//PartyManager.mainPartyManager.gas-=selectedMembers[0].currentRegion.connections[moveRegion].moveCost;
					foreach (PartyMember member in selectedMembers)
					{
						movedList.Add(member);
					}
				}
			}
			else
			{
				//IF MOVING BETWEEN TOWN NODES
				foreach (PartyMember member in selectedMembers)
				{
					if	(member.CheckEnoughFatigue(member.currentRegion.connections[moveRegion].moveCost)) movedList.Add(member);//!MapManager.main.memberTokens[member].moved) movedList.Add(member);
				}
				
				if (movedList.Count>0)
				{
					moveSuccesful=true;
					//Deselect all non-moved members
					List<PartyMember> cachedSelectedMembers=new List<PartyMember>(selectedMembers);
					foreach (PartyMember member in cachedSelectedMembers)
					{
						if (!movedList.Contains(member)) MapManager.main.memberTokens[member].Deselect();
					}
					//Apply fatigue to all moved members
					foreach (PartyMember member in movedList) 
					{
						member.ChangeFatigue(member.currentRegion.connections[moveRegion].moveCost+member.currentFatigueMoveModifier);
						MapManager.main.memberTokens[member].moved=true;
					}
				} 
				else moveSuccesful=false;
			}
		}
		return moveSuccesful;
	}
	
	public void AddPartyMemberStatusEffect(PartyMember member, MemberStatusEffect effect)//(int memberIndex, StatusEffect effect)
	{
		if (partyMembers.Contains(member))
		{
			//member.activeStatusEffects.Add(effect);
			if (member.TryAddOrStackStatusEffect(effect)) 
			{
				partyMemberCanvases[member].AddStatusEffectToken(effect);
				//If an encounter is happening, add effect tokens to encounter member tokens too
				if (EncounterCanvasHandler.main.encounterOngoing)
				{
					MemberTokenHandler memberToken;
					if (EncounterCanvasHandler.main.memberTokens.TryGetValue(member,out memberToken)) memberToken.AddNewStatusEffectToken(effect);
				}
			}
		}
	}
	
	public void RemovePartyMemberStatusEffect(PartyMember member, MemberStatusEffect effect)//(int memberIndex, StatusEffect effect)
	{
		if (partyMembers.Contains(member))
		{
			member.activeStatusEffects.Remove(effect);
			partyMemberCanvases[member].RemoveStatusEffectToken(effect);
			//If an encounter is happening, remove effect tokens from encounter member tokens too
			if (EncounterCanvasHandler.main.encounterOngoing)
			{
				MemberTokenHandler memberToken;
				if (EncounterCanvasHandler.main.memberTokens.TryGetValue(member,out memberToken)) memberToken.RemoveOldstatusEffectToken(effect);
			}
		}
		effect.CleanupEffect();
	}
	
	public void GameOverCleanup()
	{
		ETimePassed=null;
		ETimePassedEnd=null;
		List<PartyMember> buffer=new List<PartyMember>();
		buffer.AddRange(partyMembers.ToArray());
		foreach (PartyMember member in buffer) {RemovePartyMember(member);}
		statusCanvas.DisableStatusDisplay();
	}
	
	void Start()
	{
		mainPartyManager=this;//mainPlayerState=this;
		//GameManager.main.PartyManagerStartDelegate+=SetDefaultState;
		//GameManager.GameStart+=SetDefaultState;
		GameManager.GameOver+=GameOverCleanup;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.K)) 
		{
			partyMembers[0].TakeDamage(25,false,PartyMember.BodyPartTypes.Hands);
			partyMembers[0].TakeDamage(25, false, PartyMember.BodyPartTypes.Legs);
			partyMembers[0].TakeDamage(49, false, PartyMember.BodyPartTypes.Vitals);
			partyMembers[1].TakeDamage(25, false, PartyMember.BodyPartTypes.Hands);
			partyMembers[1].TakeDamage(25, false, PartyMember.BodyPartTypes.Legs);
			partyMembers[1].TakeDamage(49, false, PartyMember.BodyPartTypes.Vitals);
		}
		if (Input.GetKeyDown(KeyCode.Space)) AdvanceMapTurn();
	}
}
