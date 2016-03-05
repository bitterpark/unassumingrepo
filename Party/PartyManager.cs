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
	public int daysPassed=0;
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
	public StatusEffectDrawer effectTokenPrefab;
	Dictionary<int,List<StatusEffectDrawer>> statusEffectTokens;
	
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
	
	public delegate void TimePassedDeleg(int hours);
	public static event TimePassedDeleg TimePassed;
	
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
	public void RemoveMemberTask(PartyMember member)
	{
		if (!assignedTasks.ContainsKey(member)) throw new System.Exception("Trying to remove task from member with no task!");
		assignedTasks[member].DoEndAction();
		assignedTasks.Remove(member);
		MapManager.main.memberTokens[member].TaskRemoved();//.moved=false;
		PartyStatusCanvasHandler.main.RefreshAssignmentButtons(selectedMembers);
	}
	//public PartyMemberSelector selectorPrefab;
	
	//public void SetDefaultState() {}
	
	//Setup start of game
	public void SetDefaultState()
	{
		Vector2 startingPartyWorldCoords=new Vector2(0,0);
		MapRegion startingRegion=MapManager.main.mapRegions[0];
		//partyStatusCanvas.gameObject.SetActive(true);
		statusCanvas.EnableStatusDisplay();
		partyMemberCanvases=new Dictionary<PartyMember, PartyMemberCanvasHandler>();
		//dayTime=5;
		
		//mapCoordX=0;
		//mapCoordY=0;
		//partyVisibilityMod=0;
		//foodSupply=0;//2;
		ammo=500;//500;//5;
		gas=1;
		
		partyMembers=new List<PartyMember>();
		selectedMembers=new List<PartyMember>();
		assignedTasks=new Dictionary<PartyMember, AssignedTask>();
		//selectors=new List<PartyMemberSelector>();
		//statusEffectTokens=new Dictionary<int, List<StatusEffectDrawer>>();
		
		AddNewPartyMember(new PartyMember(startingRegion));
		AddNewPartyMember(new PartyMember(startingRegion));
		
		//AddPartyMemberStatusEffect(partyMembers[0],new Bleed(partyMembers[0]));
		//AddPartyMemberStatusEffect(partyMembers[0],new Bleed(partyMembers[0]));
		//AddNewPartyMember(new PartyMember(startingPartyWorldCoords));
		
		
		//MapManager.main.TeleportToRegion(startingRegion);
		startingRegion.StashItem(new FoodBig());
		startingRegion.StashItem(new FoodBig());
		startingRegion.StashItem(new FoodSmall());
		startingRegion.StashItem(new FoodSmall());
		
		MapManager.main.FocusViewOnRegion(startingRegion.GetComponent<RectTransform>());
		
		//startingRegion.StashItem(new SettableTrap());
		//startingRegion.StashItem(new SettableTrap());
		//startingRegion.StashItem(new Bandages());
		//startingRegion.StashItem(new AssaultRifle());
		//startingRegion.StashItem(new AssaultRifle());
		//startingRegion.StashItem(new Medkit());
		//startingRegion.StashItem(new Medkit());
		//startingRegion.StashItem(new Backpack());
		//startingRegion.StashItem(new Backpack());
		//startingRegion.StashItem(new ArmorVest());
		//startingRegion.StashItem(new ArmorVest());
		//startingRegion.StashItem(new Bed());
		//startingRegion.StashItem(new Bed());
		//startingRegion.StashItem(new Pot());//
		//Do this to setup proper background color
		//PassTime(1);
	}
	
	void PassTime(int hoursPassed)
	{
		
		dayTime=(int)Mathf.Repeat(dayTime+12,24);//hoursPassed;
		string timePassageText="";
		if (dayTime>0) timePassageText="Daytime";
		else 
		{
			timePassageText="Night";
			daysPassed+=1;
		}
		PartyStatusCanvasHandler.main.NewNotification(timePassageText);
		//dayTime=(int)Mathf.Repeat(dayTime+hoursPassed,24);
		//foreach (PartyMember member in partyMembers) {member.hunger+=10*hoursPassed;}
		if (TimePassed!=null) {TimePassed(hoursPassed);}
		
		//Adjust camera bg color
		float lightBottomThreshold=0.5f;
		float noonLightLevelBonus=0.5f;
		//float newb=lightBottomThreshold+noonLightLevelBonus-(noonLightLevelBonus/12)*(Mathf.Abs(dayTime-12));//Mathf.Repeat(Camera.main.backgroundColor.b-0.083//+0.0416f*hoursPassed,1);
		//Camera.main.backgroundColor=new Color(0,0,newb);//
	}
	
	public void AdvanceMapTurn()
	{
		PassTime(1);
		//Make sure the rest tasks and time skip occur in proper order
		foreach (MemberMapToken token in MapManager.main.memberTokens.Values) 
		if (!assignedTasks.ContainsKey(token.assignedMember)) token.moved=false;
		//Perform assigned tasks
		foreach (PartyMember member in new List<PartyMember>(assignedTasks.Keys)) 
		{
			if (!assignedTasks[member].preconditionCheck()) RemoveMemberTask(member);
			else
			{
				assignedTasks[member].actionToPerform.Invoke();
				if (!assignedTasks[member].preconditionCheck()) RemoveMemberTask(member);
			}
		}
		//See if tasks are still not finished (must be in that order)
		foreach (PartyMember member in new List<PartyMember>(assignedTasks.Keys)) 
		{
			if (!assignedTasks[member].preconditionCheck()) RemoveMemberTask(member);
		}
		//This may cause issues on gameover
		PartyStatusCanvasHandler.main.RefreshAssignmentButtons(selectedMembers);
		//PartyStatusCanvasHandler.main.StartTimeFlash();
	}
	
	//Debug
	void Update() 
	{
		if (Input.GetKeyDown(KeyCode.M)) 
		{
			//partyMembers[0].morale=0;//
			//partyMembers[0].TakeDamage(70,false,PartyMember.BodyPartTypes.Vitals);
			//partyMembers[0].TakeDamage(15,false,PartyMember.BodyPartTypes.Legs);
			//AddPartyMemberStatusEffect(partyMembers[0],new Bleed(partyMembers[0]));
		}
		/*
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (GameManager.main.gameStarted && !EncounterCanvasHandler.main.encounterOngoing && !GameEventManager.mainEventManager.drawingEvent)
			{
				AdvanceMapTurn();
				//foreach (MemberMapToken token in MapManager.main.memberTokens) token.moved=false;
			}
		}*/
	}
	
	public int PartyMemberCount() {return partyMembers.Count;}
	public PartyMember GetPartyMember(int index) {return partyMembers[index];}
	
	public void AddNewPartyMember(PartyMember newMember)
	{
		partyMembers.Add (newMember);
		PartyMemberCanvasHandler newPartyMemberCanvas=Instantiate(partyMemberCanvasPrefab);
		newPartyMemberCanvas.AssignPartyMember(newMember);
		newPartyMemberCanvas.transform.SetParent(statusCanvas.memberCanvasGroup,false);
		partyMemberCanvases.Add(newMember,newPartyMemberCanvas);
		
		MapManager.main.AddMemberToken(newMember);
		MapManager.main.MoveMembersToRegion(newMember.currentRegion,newMember);
		
		statusCanvas.NewNotification(newMember.name+" has joined the party");
	}
	
	public void RemovePartyMember(PartyMember removedMember)
	{
		if (assignedTasks.ContainsKey(removedMember)) RemoveMemberTask(removedMember);
		if (selectedMembers.Contains(removedMember)) selectedMembers.Remove(removedMember);
		MapManager.main.RemoveMemberToken(removedMember);
		PartyMemberCanvasHandler deletedHandler=partyMemberCanvases[removedMember];
		partyMemberCanvases.Remove(removedMember);
		GameObject.Destroy(deletedHandler.gameObject);
		partyMembers.Remove(removedMember);
		if (partyMembers.Count==0) {GameManager.main.EndCurrentGame(false);}//StartCoroutine(DoGameOver(false));}
		else
		{
			//this is required for the vertical layout formation to update on time (damn bugs)
			foreach (PartyMemberCanvasHandler memberCanvas in partyMemberCanvases.Values) 
			{
				memberCanvas.GetComponent<Canvas>().enabled=false;
				memberCanvas.GetComponent<Canvas>().enabled=true;
			}
			
			foreach (PartyMember member in partyMembers) 
			{
				if (member.relationships.ContainsKey(removedMember))
				{
					member.morale-=40;
					member.RemoveRelatonship(removedMember);
				}
				else member.morale-=25;
			}
		}
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
			//If moving between towns, remove gas cost, else - reduce fatigue
			if (selectedMembers[0].currentRegion.connections[moveRegion].isIntercity)
			{
				if (PartyManager.mainPartyManager.gas-selectedMembers[0].currentRegion.connections[moveRegion].moveCost>-1)
				{
					moveSuccesful=true;
					PartyManager.mainPartyManager.gas-=selectedMembers[0].currentRegion.connections[moveRegion].moveCost;
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
					if	(member.GetFatigue()+member.currentRegion.connections[moveRegion].moveCost<=100) movedList.Add(member);//!MapManager.main.memberTokens[member].moved) movedList.Add(member);
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
						member.ChangeFatigue(member.currentRegion.connections[moveRegion].moveCost);
						MapManager.main.memberTokens[member].moved=true;
					}
				} 
				else moveSuccesful=false;
			}
		}
		return moveSuccesful;
	}
	
	public void AddPartyMemberStatusEffect(PartyMember member, StatusEffect effect)//(int memberIndex, StatusEffect effect)
	{
		if (partyMembers.Contains(member))
		{
			//member.activeStatusEffects.Add(effect);
			if (member.AddStatusEffect(effect)) partyMemberCanvases[member].AddStatusEffectToken(effect);
		}
	}
	
	public void RemovePartyMemberStatusEffect(PartyMember member, StatusEffect effect)//(int memberIndex, StatusEffect effect)
	{
		if (partyMembers.Contains(member))
		{
			member.activeStatusEffects.Remove(effect);
			partyMemberCanvases[member].RemoveStatusEffectToken(effect);
		}
	}
	/*
	public void DamagePartyMember(PartyMember member, int dmg)
	{
		//PartyMember affected=partyMembers[memberIndex];
		member.health-=dmg;
		if (member.health==0) 
		{
			RemovePartyMember(member);
		}
	}*/
	/*
	public bool HealPartyMember(int memberIndex, int amountHealed)
	{
		return HealPartyMember(partyMembers[memberIndex],amountHealed);
	}*/
	/*
	public bool HealPartyMember (PartyMember member, int amountHealed)
	{
		bool healingSuccesful=false;
		{
			if (member.health<member.healthMax)
			{
				member.health+=amountHealed;
				healingSuccesful=true;
			}
		}
		return healingSuccesful;
	}*/
	/*
	public bool FeedPartyMember(PartyMember member, int amountFed)
	{
		bool feedSuccesful=false;
		if (member.hunger>0)
		{
			//member.health+=amountHealed;
			member.hunger-=amountFed;
			feedSuccesful=true;
		}
		return feedSuccesful;
	}*/
	/*
	public bool partyMemberDoPunch(PartyMember member)
	{
		bool strongPunch=false;
		if (member.stamina>0) 
		{
			member.stamina-=1;
			strongPunch=true;
		}
		return strongPunch;
	}*/
	
	/*
	void DrawPartyMemberStats()
	{
		float startX=partyStatsStartX;
		float startY=partyStatsStartY;
		//float offsetY=0;
		float sizeX=100;
		float sizeY=25;
		
		Rect itemRect=new Rect(startX,startY,sizeX,sizeY);
		int i=0;
		foreach(PartyMember member in partyMembers)
		{
			GUI.Box(itemRect,member.name);
			itemRect.y+=sizeY+17;
			Vector3 newSelectorPos=Camera.main.ScreenToWorldPoint(new Vector2(itemRect.x+sizeX*0.5f,Screen.height-itemRect.y));
			newSelectorPos.z=-2;
			//adjust selectors
			selectors[i].transform.position=newSelectorPos;//Camera.main.ScreenToWorldPoint(new Vector2(500,Screen.height-500));//itemRect.x,Screen.height-itemRect.y));
			//adjust status effect tokens
			if (statusEffectTokens[i].Count>0)
			{
				Vector3 newTokenPos=newSelectorPos+new Vector3(25,0,0);
				foreach(StatusEffectDrawer drawer in statusEffectTokens[i]) 
				{
					drawer.transform.position=newTokenPos;
					newTokenPos+=new Vector3(25,0,0);
				}
			}
			itemRect.y+=17;
			GUI.Box(itemRect,"Health:"+member.health);
			itemRect.y+=sizeY+3;
			GUI.Box(itemRect,"Stamina:"+member.stamina);
			itemRect.y+=sizeY+3;
			GUI.Box(itemRect,"Hunger:"+member.hunger);
			itemRect.y+=sizeY+3;
			i++;
		}
		//Camera.main.ScreenToWorldPoint(new Vector2(partyMemberStartX+partyMemberXGap*i,Screen.height-(partyMemberStartY)))
	}*/
	
	public void GameOverCleanup()
	{
		TimePassed=null;
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
	
	void OnGUI()
	{
		if (GameManager.main.gameStarted)
		{
			//GUI.Box(timeOfDayRect,dayTime.ToString()+":00");
			//GUI.Box(playerXCoordRect,"Map x:"+mapCoordX.ToString());
			//GUI.Box(playerYCoordRect,"Map y:"+mapCoordY.ToString());
			//GUI.Box (healthRect,"Health:"+health.ToString());
			//GUI.Box (foodSupplyRect,"Food:"+foodSupply.ToString());
			//GUI.Box (ammoSupplyRect,"Ammo:"+ammo);
			//GUI.Box (staminaRect,"Stamina:"+stamina);
			//DrawPartyMemberStats();
			
			Rect partyScreenButtonRect=new Rect(5,300,50,20);
			/*
			if (GUI.Button(partyScreenButtonRect,"Party")) {drawPartyScreen=!drawPartyScreen;}
			if (drawPartyScreen)
			{
				PartyScreenManager.mainPSManager.DrawPartyScreen();	
			}*/
		}
		/*
		if (gameOver)
		{
			string endMessage="Your party died";
			if (gameWin) {endMessage="You were rescued!";}
			GUI.Box(new Rect(Screen.width*0.5f-25f,Screen.height*0.5f-50f,100,50),endMessage);
		}*/
	}
}
