using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using Vectrosity;

public class EncounterCanvasHandler : MonoBehaviour 
{
	//int iterationCount=0;
	
	public bool encounterOngoing=false;
	//factor closer to 0 makes the zoom more abrupt, factor closer to 1 makes it more gentle
	const float zoomFactor=0.85f;
	const int minZoomPower=5;
	const int maxZoomPower=5;

	public Encounter currentEncounter=null;
	EncounterMap currentMap=null;
	
	public PartyMember selectedMember=null;
	//public EncounterEnemy selectedEnemy=null;
	public List<PartyMember> encounterMembers=new List<PartyMember>();
	public Dictionary<PartyMember,Dictionary<Vector2,int>> memberMoveMasks;
	//public EncounterRoom displayedRoom;
	public Dictionary<PartyMember,Vector2> memberCoords=new Dictionary<PartyMember, Vector2>();

	//This is for the old message system
	/*
	string lastMessage="";
	int messageCount=0;
	*/
	//Debug var
	int iterationCount=0;

	public const float lootingEnemySpawnProbIncrease=0.05f;
	public const float bashingEnemySpawnProbIncrease=0.05f;
	const float runawayEnemySpawnProbIncrease=0.15f;

    public void SetCurrentSpawnThreat(float newThreat)
    {
        currentSpawnThreat = newThreat;
        currentSpawnThreat = Mathf.Clamp(currentSpawnThreat, 0, 1);
        spawnThreatText.text = "Threat:"+(Mathf.RoundToInt(newThreat*100))+"%";
    }
	public float currentSpawnThreat;


	bool damageNumberOut=false;
	
	bool enemyTurnOngoing=false;
	//public GameObject memberToken;
	//GUI HANDLER ELEMENTS
	//PREFABS
	//public StatusEffectImageHandler enemyEffectPrefab;
	public AttackLocationSelector attackLocSelectorPrefab;
	public RoomButtonHandler roomPrefab;
	//public CombatSelectorHandler selectorPrefab;
	//public EnemySelectorHandler enemySelectorPrefab;
	public FloatingTextHandler floatingTextPrefab;	
	public EnemyTokenHandler enemyTokenPrefab;
	public MemberTokenHandler memberTokenPrefab;
	//REAL		
	public Dictionary<Vector2,RoomButtonHandler> roomButtons=new Dictionary<Vector2, RoomButtonHandler>();
	List<GameObject> wallPaddingButtons=new List<GameObject>();
	
	//public EnemyPortrait enemyPortrait;
	//public Dictionary<PartyMember,CombatSelectorHandler> selectors;
	//Dictionary <EncounterEnemy,EnemySelectorHandler> enemySelectors;
	public Dictionary<EncounterEnemy,EnemyTokenHandler> enemyTokens;
	public Dictionary<PartyMember,MemberTokenHandler> memberTokens;
	
	///GUI ELEMENTS
	
	//EXPLORE BLOCK
	/*
	public Text descriptionText;
	public Text messageText;
	public Button exitButton;
	public Button lootButton;
	public Button waitButton;
	*/
	
	//COMBAT BLOCK
	//public Canvas combatCanvas;
	/*
	public GameObject enemyInfo;
	public Text memberName;
	public Text memberHp;
	public Text memberStamina;
	public Text enemyName;
	public Text enemyHp;
	public Text enemyDamage;
	public GameObject combatHitButton;
	public GameObject combatWaitButton;
	public GameObject combatShootButton;
	*/

    public Text spawnThreatText;

	//GROUPS
	public Transform encounterMapGroup;
	public Text eventLogText;
	public Scrollbar eventLogScrollbar;
	//public Transform combatSelectorGroup;
	//public Transform enemySelectorGroup;
	//public Transform enemyEffectGroup;		
	
	//statics
	public static int encounterMaxPlayerCount=20;
	public static EncounterCanvasHandler main;
	
	//public delegate void EnemiesMoveDel(Dictionary<PartyMember, Dictionary<Vector2,int>> masks, Dictionary<PartyMember,Vector2> memberCoords);//(Dictionary<Vector2, int> costs);
	//public static event EnemiesMoveDel EMoveEnemies;
	public delegate void MemberMovedDel ();
	public event MemberMovedDel EMemberMoved;
	
	public delegate void RoundoverDel();
	public event RoundoverDel ERoundIsOver;

	public delegate void EncounterExitDel(PartyMember exitingMember);
	public event EncounterExitDel EEncounterExited;
	
	void FocusViewOnRoom(RectTransform roomTransform)
	{
		Vector3 newCenterPosition=new Vector3(-roomTransform.localPosition.x*encounterMapGroup.localScale.x
		,-roomTransform.localPosition.y*encounterMapGroup.localScale.y
		,0);
		encounterMapGroup.localPosition=newCenterPosition;
	}
	
	IEnumerator EncounterStartViewFocus(RectTransform roomTransform)
	{
		yield return new WaitForEndOfFrame();
		FocusViewOnRoom(roomTransform);
		yield break;
	}
	
	public void StartNewEncounter(Encounter newEncounter, List<PartyMember> membersOnMission, bool isAmbush)
	{
		encounterOngoing=true;
		//do party bonds
		encounterMembers.Clear();
		memberCoords.Clear();//
		
		Vector2 entranceCoords=Vector2.zero;
		foreach (EncounterRoom room in newEncounter.encounterMap.Values) 
		{
			if (room.isEntrance) entranceCoords=room.GetCoords();
		}
		
		encounterMembers.AddRange(membersOnMission.ToArray());
		foreach (PartyMember encounterMember in encounterMembers) 
		{
			encounterMember.EncounterStartTrigger(encounterMembers);
			memberCoords.Add(encounterMember,entranceCoords);
			
		}
		currentEncounter=newEncounter;
		currentMap=new EncounterMap(newEncounter.encounterMap);
		EnableRender();
        SetCurrentSpawnThreat(0);
	}
	
	void EndEncounter()
	{
		if (encounterOngoing)
		{
			//currentEncounter.RandomizeEnemyPositions();
			DisableRender();
			encounterOngoing=false;
			PartyStatusCanvasHandler.main.RefreshAssignmentButtons();
		}
	}

	public void ToggleMemberInventory(PartyMember toggledMember)
	{
		if (encounterOngoing && GetComponent<CanvasGroup>().interactable)
		{
			InventoryToggleType toggleType=GetInventoryToggleType(toggledMember);

			if (toggleType==InventoryToggleType.Open)
			{
				if (!roomButtons[memberCoords[toggledMember]].RoomHasEnemies() || memberTokens[toggledMember].CanMove())
				InventoryScreenHandler.mainISHandler.EncounterToggleNewSelectedMember(toggledMember);
			}
			if (toggleType==InventoryToggleType.Close)
			{
				if (!roomButtons[memberCoords[toggledMember]].RoomHasEnemies())
				InventoryScreenHandler.mainISHandler.EncounterToggleNewSelectedMember(toggledMember);
				else
				{
					bool doTurnOverForMember=false;
					memberTokens[toggledMember].TryMove(out doTurnOverForMember);
					InventoryScreenHandler.mainISHandler.EncounterToggleNewSelectedMember(toggledMember);
					if (doTurnOverForMember) TurnOver(toggledMember,roomButtons[memberCoords[toggledMember]].assignedRoom,false);
				}
			}
			if (toggleType==InventoryToggleType.Switch)
			{
				if (!roomButtons[memberCoords[InventoryScreenHandler.mainISHandler.selectedMember]].RoomHasEnemies()
				&& !roomButtons[memberCoords[toggledMember]].RoomHasEnemies())
				InventoryScreenHandler.mainISHandler.EncounterToggleNewSelectedMember(toggledMember);
			}
		}
	}

	enum InventoryToggleType {Close,Open,Switch};

	InventoryToggleType GetInventoryToggleType(PartyMember toggledMember)
	{
		InventoryToggleType toggleType=default(InventoryToggleType);
		if (!InventoryScreenHandler.mainISHandler.inventoryShown) toggleType=InventoryToggleType.Open;
		else
		{
			if (InventoryScreenHandler.mainISHandler.selectedMember==toggledMember) toggleType=InventoryToggleType.Close;
			else toggleType=InventoryToggleType.Switch;
		}
		return toggleType;
	}

	public void RoomClicked(RoomButtonHandler roomHandler)//EncounterRoomDrawer roomDrawer)
	{
		
		int moveDistance=0;	
		EncounterRoom startingRoom=currentEncounter.encounterMap[memberCoords[selectedMember]];
		moveDistance=Mathf.Abs((int)startingRoom.xCoord-roomHandler.roomX)+Mathf.Abs((int)startingRoom.yCoord-roomHandler.roomY);//encounterPlayerX-room.xCoord)+Mathf.Abs(encounterPlayerY-room.yCoord);
		
		if (GetComponent<CanvasGroup>().interactable
		&& moveDistance==1 
		&& !roomHandler.assignedRoom.isWall
		&& (!memberTokens[selectedMember].inFront || !roomButtons[startingRoom.GetCoords()].RoomHasEnemies())
		)
		{
			/*
			bool allowMovement=false;
			if (roomHandler.assignedRoom.barricadeInRoom==null) allowMovement=true;
			else
			{
				if (selectedMember.stamina-selectedMember.barricadeVaultCost>=0) 
				{
					allowMovement=true;
					selectedMember.stamina-=selectedMember.barricadeVaultCost;
					AddNewLogMessage(selectedMember.name+" vaults over the barricade!");
				}
			}
			*/
			StartCoroutine(MoveMemberToRoom(roomHandler));
		} 
	}
	
	IEnumerator MoveMemberToRoom(RoomButtonHandler roomHandler)
	{
		AttackLocationSelector.DestroySelectors();
		EncounterRoom startingRoom=currentEncounter.encounterMap[memberCoords[selectedMember]];
		//If the room has no other party members move there regularly, else - do the swap
		//currently switched off
		//See if member has enough stamina to move (if it does, the token itself will deduct stamina)
		bool doTurnover;
		if (memberTokens[selectedMember].TryMove(out doTurnover))//!memberCoords.ContainsValue(roomHandler.GetRoomCoords()))
		{
			//1 to account for the last members (the selectedMember, currently being moved to another room)
			if (startingRoom.lootIsLocked && roomButtons[startingRoom.GetCoords()].GetTotalMembersInRoom()==1) 
			{
				startingRoom.ResetLockStrength();
			}
			//Enemies get a free swipe if you are the last to move out of their spot
			//(moving member's coords don't get updated until after the check, so this is necessary)
			/*
				List<Vector2> nonMovingMemberCoords=new List<Vector2>(memberCoords.Values);
				nonMovingMemberCoords.Remove(memberCoords[selectedMember]);
				if (!nonMovingMemberCoords.Contains(startingRoom.GetCoords()))
				*/
			if (roomButtons[startingRoom.GetCoords()].RoomHasEnemies())// && !selectedMember.isScout)
			{
				int enemiesBlockedByMembers=roomButtons[startingRoom.GetCoords()].membersInFront.Count;
                if (startingRoom.barricadeInRoom != null) enemiesBlockedByMembers += 1;
				GetComponent<CanvasGroup>().interactable=false;
				List<PartyMember> argumentListOfOne=new List<PartyMember>();
				argumentListOfOne.Add(selectedMember);
				foreach (EnemyTokenHandler enemyToken in roomButtons[startingRoom.GetCoords()].GetAllEnemiesInRoom()) 
				{
					if (enemiesBlockedByMembers<=0) yield return StartCoroutine(enemyTokens[enemyToken.assignedEnemy].TokenAttackTrigger(memberTokens[selectedMember]));
					else enemiesBlockedByMembers--;
				}
				GetComponent<CanvasGroup>().interactable=true;
			}
			//if last party member didn't die moving away
			if (encounterOngoing) 
			{
				//MakeNoise(startingRoom.GetCoords(),1);
				//if (!selectedMember.isQuiet) MakeNoise(roomHandler.GetRoomCoords(),1);
				MakeNoise(roomHandler.GetRoomCoords(),selectedMember.movingEnemySpawnProbIncrease);
				TriggerEnemySpawnProbability(roomHandler);
				roomButtons[startingRoom.GetCoords()].MemberGone(memberTokens[selectedMember]);
				MovePartyMemberToRoom(selectedMember,roomHandler,false);
				DeadMemberCleanupCheck();
				//roomButtons[startingRoom.GetCoords()].UpdateMemberGroup();
				
				if (encounterOngoing)
				{
					if (roomButtons[startingRoom.GetCoords()].RoomHasEnemies())
					{
						//Despawn enemies from rooms when all members ran away from them
						if (roomButtons[startingRoom.GetCoords()].GetTotalMembersInRoom()==0)
						{ 
							foreach (EnemyTokenHandler enemyToken in new List<EnemyTokenHandler>(roomButtons[startingRoom.GetCoords()].GetAllEnemiesInRoom())) 
							{DespawnEnemy(enemyToken.assignedEnemy);}
							//Increase room enemy spawn probability if all members ran away
                            MakeNoise(startingRoom.GetCoords(), runawayEnemySpawnProbIncrease);
						}
					}
					//If moved member died during move, try to switch to next available member
					if (!encounterMembers.Contains(selectedMember))
					{
						bool unactedMemberFound=false;
						foreach (MemberTokenHandler handler in memberTokens.Values)
						{
							if (!handler.turnTaken)
							{
								unactedMemberFound=true;
								SelectMember(handler);
								break;
							}
						}
						if (!unactedMemberFound) ToggleRoundSwitch();
					}
					else
					{
						//If moved member survived during move
						if (doTurnover) TurnOver(selectedMember,roomButtons[memberCoords[selectedMember]].assignedRoom,false);
					}
				}
			}
		}
		yield break;
	}

	public void MoveMemberWithinRoomClicked(RoomButtonHandler roomHandler, bool toBack)
	{
		int roomDistance=0;	
		EncounterRoom startingRoom=roomHandler.assignedRoom;
		roomDistance=Mathf.Abs((int)startingRoom.xCoord-roomHandler.roomX)+Mathf.Abs((int)startingRoom.yCoord-roomHandler.roomY);//encounterPlayerX-room.xCoord)+Mathf.Abs(encounterPlayerY-room.yCoord);
		
		if (GetComponent<CanvasGroup>().interactable && roomDistance==0)
		{
			StartCoroutine(MoveMemberWithinRoom(roomHandler,toBack));
		} 
	}

	IEnumerator MoveMemberWithinRoom(RoomButtonHandler roomHandler, bool toBack)
	{
		AttackLocationSelector.DestroySelectors();
		//See if member has enough stamina to move (if it does, the token itself will deduct stamina)
		bool doTurnover;
		if (memberTokens[selectedMember].TryMove(out doTurnover))//!memberCoords.ContainsValue(roomHandler.GetRoomCoords()))
		{
		//Find members still left in row (-1 for member currently leaving row)

			if (roomHandler.RoomHasEnemies() && toBack)// && !selectedMember.isScout)
			{
				//-1 to account for the member that's moving away (as the away move doesn't get registered until later)
				int enemiesBlockedByMembers=roomHandler.membersInFront.Count-1;
                if (roomHandler.assignedRoom.barricadeInRoom != null) enemiesBlockedByMembers += 1;
				GetComponent<CanvasGroup>().interactable=false;
				List<PartyMember> argumentListOfOne=new List<PartyMember>();
				argumentListOfOne.Add(selectedMember);
				foreach (EnemyTokenHandler enemyToken in roomHandler.GetAllEnemiesInRoom()) 
				{
					if (enemiesBlockedByMembers<=0) yield return StartCoroutine(enemyToken.TokenAttackTrigger(memberTokens[selectedMember]));
					else enemiesBlockedByMembers--;
				}
				GetComponent<CanvasGroup>().interactable=true;
			}
			//if last party member didn't die moving away
			if (encounterOngoing) 
			{
				if (toBack) roomHandler.MoveMemberToBack(memberTokens[selectedMember]);
				else roomHandler.MoveMemberToFront(memberTokens[selectedMember]);
				DeadMemberCleanupCheck();
				
				
				if (encounterOngoing)
				{
                    roomButtons[roomHandler.GetRoomCoords()].UpdateMemberGroup();
                    //If moved member died during move, try to switch to next available member
					if (!encounterMembers.Contains(selectedMember))
					{
						bool unactedMemberFound=false;
						foreach (MemberTokenHandler handler in memberTokens.Values)
						{
							if (!handler.turnTaken)
							{
								unactedMemberFound=true;
								SelectMember(handler);
								break;
							}
						}
						if (!unactedMemberFound) ToggleRoundSwitch();
					}
					else
					{
						//If moved member survived during move
						//if (selectedMember.legsBroken) doTurnover=true;
						if (doTurnover) TurnOver(selectedMember,roomButtons[memberCoords[selectedMember]].assignedRoom,false);
					}
				}
			}
		}
		yield break;
	}

	//Consider deprecating this later
	void SwapMemberPlaces(PartyMember member1, PartyMember member2)
	{
		//if (member1==member2) throw new System.Exception("swapping member with itself!");
		Vector2 cachedMember1Coords=memberCoords[member1];
		MovePartyMemberToRoom(member1,roomButtons[memberCoords[member2]], true);
		MovePartyMemberToRoom(member2,roomButtons[cachedMember1Coords], true);
	}
	
	//Finds the member a place to stand alone at encounter start
	void PlacePartyMemberAtEntrance(PartyMember member, EncounterRoom entranceRoom, List<Vector2> roomsSortedByDistance)
	{
		//first - pick out an unoccupied room
		
		RoomButtonHandler cursorRoom=roomButtons[new Vector2(entranceRoom.xCoord,entranceRoom.yCoord)];
		/*
		while (cursorRoom.GetComponentInChildren<MemberTokenHandler>()!=null)
		{
			roomsSortedByDistance.Remove(new Vector2(cursorRoom.assignedRoom.xCoord,cursorRoom.assignedRoom.yCoord));
			cursorRoom=roomButtons[roomsSortedByDistance[0]];
			//check all rooms adjecent to exit
		}*/
		MovePartyMemberToRoom(member,cursorRoom,false);
		/*
		memberCoords[member]=new Vector2(cursorRoom.assignedRoom.xCoord,cursorRoom.assignedRoom.yCoord);
		//update member token pos
		memberTokens[member].UpdateTokenPos(roomButtons[memberCoords[member]]);
		//turnover check (enemies move equivalent)? -- no need, not until roundover
		//reveal party vision
		UpdateMemberMoveMask(member);
		RevealPartyVision();
		if (EMemberMoved!=null) EMemberMoved();*/
	}
	
	void MovePartyMemberToRoom(PartyMember member, RoomButtonHandler roomButton, bool doTurnover)
	{	
		memberCoords[member]=roomButton.GetRoomCoords();
		//update member token pos
		memberTokens[member].UpdateTokenPos(roomButton);
		//reveal party vision
		//UpdateMemberMoveMask(member);
		RevealPartySenses();
        roomButton.SetVisited();
		if (EMemberMoved!=null) EMemberMoved();
		if (doTurnover) TurnOver(member,roomButton.assignedRoom,false);
		//RefreshRoom(room);
		//CheckRoomForEnemies(room);
		//check for combat start
	}
	//called after member move or initial member placement - !!! CONSIDER MOVING THIS TO MOvePartyMemberToRoom !!!
	//!!!!DEPRECATED!!!! remove this later
	void UpdateMemberMoveMask(PartyMember member)
	{
		//memberMoveMasks[member].Clear();
		//Dictionary<Vector2,int> moveMask=memberMoveMasks[member];
		//memberMoveMasks[member]=IterativeGrassfireMapper(memberCoords[member]);
		Dictionary<Vector2,int> memberMoveMask=memberMoveMasks[member];
		IterativeGrassfireMapper(memberCoords[member],ref memberMoveMask);
	}
	
	//reveals stuff around current party room
	void RevealPartySenses()
	{
		//First - list all rooms as non-visible
		List<RoomButtonHandler> unseenRooms=new List<RoomButtonHandler>(roomButtons.Values);
		List<RoomButtonHandler> inaudibleRooms=new List<RoomButtonHandler>(roomButtons.Values);
		/*
		foreach (RoomButtonHandler cycledRoom in roomButtons.Values) 
		{
			cycledRoom.SetVisibility(false);
			cycledRoom.SetHearing(false);
		}*/
		
		//Iterate through every present player
		foreach (PartyMember member in encounterMembers)
		{
			//Default daytime range
			int visionRange=1;
			//int verticalVisionRange=3;
			//int horizontalVisionRange=4;
			/*
			if (PartyManager.mainPartyManager.dayTime<6 | PartyManager.mainPartyManager.dayTime>18) 
			{
				Nighttime range
				visionRange=1;
				Nighttime range with flashlight
				if (member.hasLight) {visionRange+=2;}
			}*/
			//////
			//Make sure this is >= to max of the visionrange values
			int hearingRange=2;
			int maxRange=Mathf.Max(visionRange,hearingRange);
			
			for (int i=-maxRange; i<=maxRange; i++)//Mathf.Max(hearingRange,Mathf.Max(horizontalVisionRange,verticalVisionRange)); i++)
			{	
				for (int j=-maxRange; j<=maxRange; j++)//Mathf.Max(hearingRange,Mathf.Max(horizontalVisionRange,verticalVisionRange)); j++)
				{
					//int rangeToRoom=Mathf.Abs(i)+Mathf.Abs(j);
					//Set up hearing reveal
					//Currently set to reveal hearing in a square
					//if (rangeToRoom<=hearingRange)
					if (Mathf.Abs(i)<=hearingRange && Mathf.Abs(j)<=hearingRange)
					{
						Vector2 roomCoords=new Vector2(memberCoords[member].x+j,memberCoords[member].y+i);
						if (roomButtons.ContainsKey(roomCoords)) inaudibleRooms.Remove(roomButtons[roomCoords]);
					}
					//Do los-based reveal
					int rangeToRoom=Mathf.Abs(i)+Mathf.Abs(j);
					//Set up vision reveal
					if (rangeToRoom==visionRange)
					{
						//Bresenham vision line
						List<Vector2> lineCoords=new List<Vector2>();
						BresenhamLines.Line((int)memberCoords[member].x,(int)memberCoords[member].y
						                    ,(int)memberCoords[member].x+j,(int)memberCoords[member].y+i,(int x, int y)=>//encounterPlayerX,encounterPlayerY,encounterPlayerX+j,encounterPlayerY+i,(int x, int y)=>
						                    {
							int storedX=x;
							int storedY=y;
							bool noBlock=true;
							if (!currentEncounter.encounterMap.ContainsKey(new Vector2(x,y))) {noBlock=false;}	
							else 
							{
								//disable the two lines below to remove wall-based los
								if (currentEncounter.encounterMap[new Vector2(x,y)].isWall) {noBlock=false;}
								else {noBlock=true;}
								lineCoords.Add(new Vector2(storedX,storedY));
							}
							return noBlock;
						});
						foreach (Vector2 roomCoords in lineCoords) 
						{
							unseenRooms.Remove(roomButtons[roomCoords]);
						}
					}
				}
				
			}

			//Update all rooms according to lists
			foreach(RoomButtonHandler roomButton in roomButtons.Values)
			{
				if (unseenRooms.Contains(roomButton)) roomButton.SetVisibility(false);
				else roomButton.SetVisibility(true);
				if (inaudibleRooms.Contains(roomButton)) roomButton.SetHearing(false);
				else roomButton.SetHearing(true);
			}

			//!! CONSIDER MOVING THIS TO HEARING RANGE CHECK
			
			/*
			for (int i=-visionRange;i<=visionRange;i++)//verticalVisionRange; i<=verticalVisionRange; i++)
			{	
				for (int j=-visionRange;j<=visionRange;j++)//horizontalVisionRange; j<=horizontalVisionRange; j++)
				{
					int rangeToRoom=Mathf.Abs(i)+Mathf.Abs(j);
					//Set up vision reveal
					if (rangeToRoom==visionRange)
					{
						//Bresenham vision line
						List<Vector2> lineCoords=new List<Vector2>();
						BresenhamLines.Line((int)memberCoords[member].x,(int)memberCoords[member].y
						,(int)memberCoords[member].x+j,(int)memberCoords[member].y+i,(int x, int y)=>//encounterPlayerX,encounterPlayerY,encounterPlayerX+j,encounterPlayerY+i,(int x, int y)=>
						{
							int storedX=x;
							int storedY=y;
							bool noBlock=true;
							if (!currentEncounter.encounterMap.ContainsKey(new Vector2(x,y))) {noBlock=false;}	
							else 
							{
								//disable the two lines below to remove wall-based los
								if (currentEncounter.encounterMap[new Vector2(x,y)].isWall) {noBlock=false;}
								else {noBlock=true;}
								lineCoords.Add(new Vector2(storedX,storedY));
							}
							return noBlock;
						});
						foreach (Vector2 roomCoords in lineCoords) {roomButtons[roomCoords].SetVisibility(true);}//currentEncounter.encounterMap[roomCoords].isVisible=true;}
					}
				}
			}*/
		}
	}
	
	void TurnOver(PartyMember finishedMember,EncounterRoom room, bool turnSkipped)
	{
		//if member hasn't left the encounter, update the selector
		if (encounterMembers.Contains(finishedMember)) {memberTokens[finishedMember].FinishTurn(turnSkipped);}//selectors[finishedMember].actionTaken=true;}
		//EncounterRoom currentRoom=currentEncounter.encounterMap[memberCoords[encounterMembers[0]]];//new Vector2(encounterPlayerX,encounterPlayerY)];
		if (roomButtons[room.GetCoords()].RoomHasEnemies())
		{
			foreach (EnemyTokenHandler enemyToken in roomButtons[room.GetCoords()].GetAllEnemiesInRoom()) {enemyToken.assignedEnemy.TurnAction();}
			DeadMemberCleanupCheck();
		}
		
		//select next member that hasn't acted yet, if one exists, else do turnover
		bool allActionsTaken=true;
		foreach (MemberTokenHandler token in memberTokens.Values)//CombatSelectorHandler selector in selectors.Values) 
		{
			if (!token.turnTaken) 
			{
				allActionsTaken=false; 
				SelectMember(token);
				//print ("not all actions done, selecting next unacted member");
				break;
			}
		}
		//If currently selected member left the encounter - select another member
		if (!encounterMembers.Contains(selectedMember)) SelectMember(memberTokens.Values.First<MemberTokenHandler>());
		//if all members acted (none available to move current selection to)
		if (allActionsTaken) 
		{
			//Do roundover
			ToggleRoundSwitch();
			/*
			RoundOver();
			DeadMemberCleanupCheck();
			NextRound();*/
			/*
			if (encounterOngoing)
			{
				//Update tokens on round end
				foreach (MemberTokenHandler token in memberTokens.Values)//CombatSelectorHandler selector in selectors.Values) 
				{
					token.NextTurn();
				}
				//Make sure this doesn't update after the encounter is finished (from a TPK)
				SelectMember(memberTokens[encounterMembers[0]]);//selectors[encounterMembers[0]]);
			}*/
		}
	}
	
	void ToggleRoundSwitch()
	{
		//StartCoroutine(TurnoverSignRoutine());
		StartCoroutine(RoundSwitchRoutine());
		
	}
	
	IEnumerator RoundSwitchRoutine()
	{
		float noteTime=0.5f;
		PartyStatusCanvasHandler.main.NewNotification("Enemy turn",noteTime);
		//!!IMPORTANT!! THIS IS CALLED ON TOP OF =true AND =false IN ATTACK VISUALIZATION ROUTINE. It is here in case no attack animations occur
		//during the enemy turn, otherwise the =true after an attack animation will cancel this!!!
		enemyTurnOngoing=true;
		GetComponent<CanvasGroup>().interactable=false;
		yield return new WaitForSeconds(noteTime);
		if (ERoundIsOver!=null) ERoundIsOver(); //necessary for hearing token placement
		//EnemiesMove();
		
		//Wait for all the enemies to take their turns and all the animations to play
		foreach (EnemyTokenHandler enemyHandler in enemyTokens.Values)
		{
			yield return StartCoroutine(enemyHandler.TokenRoundoverTrigger(memberMoveMasks,memberCoords));
		}
		DeadMemberCleanupCheck();
		//SEE ABOVE
		GetComponent<CanvasGroup>().interactable=true;
		if (encounterOngoing) 
		{
			PartyStatusCanvasHandler.main.NewNotification("Survivors' turn",noteTime);
			NextRound();
		}
		yield break;
	}
	/*
	IEnumerator TurnoverSignRoutine()
	{
		PartyStatusCanvasHandler.main.NewNotification("Enemy turn");
		yield return new WaitForSeconds(NotePanelHandler.lifeTime);
		
	}*/
	
	void NextRound()
	{
		//if (encounterOngoing)
		{
			//Update tokens on round end
			foreach (MemberTokenHandler token in memberTokens.Values)//CombatSelectorHandler selector in selectors.Values) 
			{
				token.NextTurn();
			}
			//Make sure this doesn't update after the encounter is finished (from a TPK)
			SelectMember(memberTokens[encounterMembers[0]]);//selectors[encounterMembers[0]]);
		}
	}
	
	void DeadMemberCleanupCheck()
	{
		//Remove members that have been killed after turnover/roundover actions
		//Make sure this doesn't update after the manager has stopped render
		
		if (encounterOngoing)
		{
			List<PartyMember> memberBuffer=new List<PartyMember>();
			memberBuffer.AddRange(encounterMembers.ToArray());
			
			foreach (PartyMember member in memberBuffer)
			{
				if (!PartyManager.mainPartyManager.partyMembers.Contains(member)) 
				{
					RemoveEncounterMember(member);
				}
			}
			//Stop encounter if all encounter members died, else try to select an unacted member, or end round if all remaining members acted
			if (encounterMembers.Count==0) {EndEncounter();}
			/*
			else
			{
				if (!encounterMembers.Contains(selectedMember))
				{
					bool unactedMemberFound=false;
					foreach (MemberTokenHandler handler in memberTokens.Values)
					{
						if (!handler.turnTaken)
						{
							unactedMemberFound=true;
							SelectMember(handler);
							break;
						}
					}
					if (!unactedMemberFound) RoundOver();
				}
			}*/
		}
	}
	//Consider moving this to the only place where it is used
	List<Vector2> GetFreeRoomsNearEntrance(Vector2 entranceCoords)
	{
		//Dictionary<Vector2, int> rawCostsDictionary=new Dictionary<Vector2, int>();
		//RecursiveGrassfireMapper(entranceCoords,ref rawCostsDictionary,0);
		Dictionary<Vector2, int> rawCostsDictionary=IterativeGrassfireMapper(entranceCoords);
		//This sorts the raw dictionary using a LINQ query!
		var items = from pair in rawCostsDictionary
			orderby pair.Value ascending
				select pair.Key;
		
		//List<Vector2> fucknuts=new List<Vector2>();
		//fucknuts.AddRange(items);
		return new List<Vector2>(items);	
	}
	
	public Dictionary<Vector2, int> IterativeGrassfireMapper(Vector2 goal)
	{
		Dictionary<Vector2,int> costs=new Dictionary<Vector2, int>();
		IterativeGrassfireMapper(goal,ref costs);
		return costs;
	}
	
	public List<Vector2> GetPathInCurrentEncounter(Vector2 startCoords, Vector2 endCoords)
	{
		//List<Vector2> path=
		return new AStarSearch(currentMap,new Location((int)startCoords.x,(int)startCoords.y)
		,new Location((int)endCoords.x,(int)endCoords.y)).path;
	}
	
	//For EnemiesMove (currently updates on member move) and for enemyToken POIs, based on djikstra maps (roguebasin)
	public void IterativeGrassfireMapper(Vector2 goal, ref Dictionary<Vector2,int> rewriteDictionary)
	{
		Dictionary<Vector2, EncounterRoom> map=currentEncounter.encounterMap;
		if (!map.ContainsKey(goal)) {throw new System.Exception("Goal set for grassfire mapper doesn't exist in the map!");}
		Dictionary<Vector2,int> costs=rewriteDictionary;//new Dictionary<Vector2, int>(rewriteDictionary);
		if (costs.Count==0) {costs.Add(goal,0);} //print ("generating raw map");}
		else 
		{
			costs[goal]=0; //print ("rewriting existing map");
			//if (costs.ContainsKey(Vector2.zero)) print ("Old (0,0) cost:"+costs[Vector2.zero]);
			//else print ("No (0,0) key found!");
		}
		iterationCount=0;
		//costs.Add(goal,0);
		bool changesMade=false;
		//Vector2 currentRoomCoords=Vector2.zero;
		
		System.Action<Vector2> neighbourCheck=(Vector2 currentRoomCoords)=>
		{
			iterationCount++;
			int minNeighbourValue=999;
			System.Action<Vector2>cursorCheck =(Vector2 neighbourCoords)=>
			{
				//check
				if (map.ContainsKey(neighbourCoords)) 
				{
					if (costs.ContainsKey(neighbourCoords) && !map[neighbourCoords].isWall) 
					{
						if (costs[neighbourCoords]<minNeighbourValue) {minNeighbourValue=costs[neighbourCoords];}
					}
				}
			};
			//Up
			cursorCheck.Invoke(currentRoomCoords+Vector2.down);
			//Down
			cursorCheck.Invoke(currentRoomCoords+Vector2.up);
			//Left
			cursorCheck.Invoke(currentRoomCoords+Vector2.left);
			//Right
			cursorCheck.Invoke(currentRoomCoords+Vector2.right);
			
			if (!costs.ContainsKey(currentRoomCoords)) 
			{
				costs.Add(currentRoomCoords,minNeighbourValue+1);
				changesMade=true;
			}
			else 
			{
				if (costs[currentRoomCoords]!=minNeighbourValue+1 && currentRoomCoords!=goal)//>minNeighbourValue+1 | (costs[currentRoomCoords]==0 && currentRoomCoords!=goal)) 
				{
					costs[currentRoomCoords]=minNeighbourValue+1;
					changesMade=true;
				}
			}
			
		};
		do
		{	
			changesMade=false;
			foreach (Vector2 roomCoords in map.Keys)
			{
				if (!map[roomCoords].isWall) neighbourCheck.Invoke(roomCoords);
			}
		} while (changesMade);
		//return costs;
		rewriteDictionary=costs;//
	}
	
	//CONSIDER MAKING THIS STATIC - currently unused, iterative mapper is more efficent
	void RecursiveGrassfireMapper(Vector2 coords, ref Dictionary<Vector2, int> costs, int currentCost)
	{
		if (currentEncounter.encounterMap.ContainsKey(coords)) 
		{
			if (!currentEncounter.encounterMap[coords].isWall)
			{
				if(!costs.ContainsKey(coords)) {costs.Add(coords,currentCost);}
				//currentEncounter.encounterMap[coords].currentCost=currentCost;
				currentCost+=1;
				//iterationCount++;
				//ASSIGN AROUND
				//up
				Vector2 newCoords=coords+new Vector2(0,-1);
				if (currentEncounter.encounterMap.ContainsKey(newCoords)) 
				{
					if (!currentEncounter.encounterMap[newCoords].isWall)
					{
						if (!costs.ContainsKey(newCoords))
						{costs.Add(newCoords,currentCost); }
						else
						{
							if (costs[newCoords]>currentCost) 
							{costs[newCoords]=currentCost; }
						}
					}
				}
				//down
				newCoords=coords+new Vector2(0,1);
				if (currentEncounter.encounterMap.ContainsKey(newCoords)) 
				{
					if (!currentEncounter.encounterMap[newCoords].isWall)
					{
						if (!costs.ContainsKey(newCoords))
						{costs.Add(newCoords,currentCost); }
						else
						{
							if (costs[newCoords]>currentCost) 
							{costs[newCoords]=currentCost; }
						}
					}
				}
				//left
				newCoords=coords+new Vector2(-1,0);
				if (currentEncounter.encounterMap.ContainsKey(newCoords)) 
				{
					if (!currentEncounter.encounterMap[newCoords].isWall)
					{
						if (!costs.ContainsKey(newCoords))
						{costs.Add(newCoords,currentCost); }
						else
						{
							if (costs[newCoords]>currentCost) 
							{costs[newCoords]=currentCost; }
						}
					}
				}
				//right
				newCoords=coords+new Vector2(1,0);
				if (currentEncounter.encounterMap.ContainsKey(newCoords)) 
				{
					if (!currentEncounter.encounterMap[newCoords].isWall)
					{
						if (!costs.ContainsKey(newCoords))
						{costs.Add(newCoords,currentCost); }
						else
						{
							if (costs[newCoords]>currentCost) 
							{costs[newCoords]=currentCost; }
						}
					}
				}
				//MOVE CURSOR
				//up
				newCoords=coords+new Vector2(0,-1);
				if (currentEncounter.encounterMap.ContainsKey(newCoords)) 
				{	
					bool moveCursor=false;
					if (!costs.ContainsKey(newCoords)) {moveCursor=true;}
					else
					{
						if (costs[newCoords]==currentCost) moveCursor=true;
					}
					if (moveCursor) RecursiveGrassfireMapper(newCoords,ref costs,currentCost);
				}
				//down
				newCoords=coords+new Vector2(0,1);
				if (currentEncounter.encounterMap.ContainsKey(newCoords)) 
				{	
					bool moveCursor=false;
					if (!costs.ContainsKey(newCoords)) {moveCursor=true;}
					else
					{
						if (costs[newCoords]==currentCost) moveCursor=true;
					}
					if (moveCursor) RecursiveGrassfireMapper(newCoords,ref costs,currentCost);
				}
				//left
				newCoords=coords+new Vector2(-1,0);
				if (currentEncounter.encounterMap.ContainsKey(newCoords)) 
				{	
					bool moveCursor=false;
					if (!costs.ContainsKey(newCoords)) {moveCursor=true;}
					else
					{
						if (costs[newCoords]==currentCost) moveCursor=true;
					}
					if (moveCursor) RecursiveGrassfireMapper(newCoords,ref costs,currentCost);
				}
				//right
				newCoords=coords+new Vector2(1,0);
				if (currentEncounter.encounterMap.ContainsKey(newCoords)) 
				{	
					bool moveCursor=false;
					if (!costs.ContainsKey(newCoords)) {moveCursor=true;}
					else
					{
						if (costs[newCoords]==currentCost) moveCursor=true;
					}
					if (moveCursor) RecursiveGrassfireMapper(newCoords,ref costs,currentCost);
				}
			}
		}
	}
	
	//used for roundover
	void EnemiesMove()
	{		
		//if (EMoveEnemies!=null) EMoveEnemies(memberMoveMasks,memberCoords);
	}
	
	/*
	public void DisplayNewMessage(string message)
	{
		StopCoroutine("AddTimedMessage");
		StartCoroutine("AddTimedMessage",message);
	}
	
	IEnumerator AddTimedMessage(string message)
	{
		lastMessage+=message+"\n";

		while (lastMessage.Contains("\n"))
		{
			messageText.text=lastMessage;
			yield return new WaitForSeconds(1f);
			lastMessage=lastMessage.Remove(0,lastMessage.IndexOf("\n")+1);
		}
		lastMessage="";
		messageText.text=lastMessage;
		//messageCount=0;
		yield break;	
	}*/

	void TriggerEnemySpawn(EncounterRoom spawnRoom)
	{
		RoomButtonHandler spawnRoomButton=roomButtons[spawnRoom.GetCoords()];
		foreach (EnemyTokenHandler newEnemyToken in spawnRoomButton.SpawnEnemiesWeightedOnSpawnChance(currentSpawnThreat))
		{
			enemyTokens.Add (newEnemyToken.assignedEnemy,newEnemyToken);
			EMemberMoved+=newEnemyToken.UpdateTokenVision;
			if (spawnRoom.trapInRoom!=null) spawnRoom.trapInRoom.SetOff(newEnemyToken.assignedEnemy);
		}
	}

	//turn encounter render on and off (may consider moving this to StartEncounter, since it's only used in that one method
	//RENAME TO "Encounter Start"???
	void EnableRender() 
	{
		GetComponent<Canvas>().enabled=true;
		//eventLogMask.enabled=true;
		//eventLogMask.GetComponent<Text>()
		//selectors=new Dictionary<PartyMember, CombatSelectorHandler>();
		//enemySelectors=new Dictionary<EncounterEnemy, EnemySelectorHandler>();
		roomButtons=new Dictionary<Vector2, RoomButtonHandler>();
		wallPaddingButtons=new List<GameObject>();
		
		enemyTokens=new Dictionary<EncounterEnemy, EnemyTokenHandler>();
		memberTokens=new Dictionary<PartyMember, MemberTokenHandler>();
		
		//combatWaitButton.SetActive(true);
		memberMoveMasks=new Dictionary<PartyMember, Dictionary<Vector2, int>>();
		//Encounter currentEncounter=EncounterManager.mainEncounterManager.currentEncounter;
		encounterMapGroup.GetComponent<GridLayoutGroup>().constraintCount=(currentEncounter.maxX+1)-currentEncounter.minX;
		
		EncounterRoom entranceRoom=null;
		float eventLogTopOffset=eventLogText.transform.parent.parent.GetComponent<RectTransform>().rect.height;//220;
		float leftBorderOffset=200f;
		
		float elementGap=0;
		float elementSize=roomPrefab.GetComponent<RectTransform>().rect.width;
		float elementSkipLength=elementGap+elementSize;
		
		//print ("setting up encounter with minx:"+currentEncounter.minX+", miny:"+currentEncounter.miny);
		//print ("adjusted minx:"+currentEncounter.minX-xCoordAdjustment+", miny:"+currentEncounter.miny-yCoordAdjustment);
		float gridStartX=leftBorderOffset+elementSize*0.5f;//currentEncounter.minX*elementSize;
		float gridStartY=-elementSize*0.5f-eventLogTopOffset;//currentEncounter.minY*elementSize;
		
		//float minX=Mathf.Infinity;
		float minY=Mathf.Infinity;
		float maxX=Mathf.NegativeInfinity;
		//float maxY=maxX;
		for (int i=currentEncounter.minY; i<=currentEncounter.maxY;i++)
		{
			for (int j=currentEncounter.minX; j<=currentEncounter.maxX;j++)
			{
				EncounterRoom room=currentEncounter.encounterMap[new Vector2(j,i)];
				RoomButtonHandler newRoomButton=Instantiate(roomPrefab);
				//Keep "invisible" wall objects in a separate collection, so they don't needlessly pad the proper room operations
				if (!room.hideImage) roomButtons.Add(new Vector2(j,i),newRoomButton);
				else wallPaddingButtons.Add(newRoomButton.gameObject);
				Vector2 newRoomLocalCoords
				=new Vector2(gridStartX+elementSkipLength*(j-currentEncounter.minX),gridStartY+elementSkipLength*-(i-currentEncounter.minY));
				//minX=Mathf.Min(minX,newRoomLocalCoords.x);
				minY=Mathf.Min(minY,newRoomLocalCoords.y);
				maxX=Mathf.Max(maxX,newRoomLocalCoords.x);
				//maxY=Mathf.Max(maxY,newRoomLocalCoords.y);
				newRoomButton.transform.SetParent(encounterMapGroup,false);
				newRoomButton.transform.localPosition=newRoomLocalCoords;
				newRoomButton.AssignRoom(room);
				
				//Add enemy token if room has enemy
				//Disabled under new enemies system
				/*
				if (roomButtons[room.GetCoords()].RoomHasEnemies()) 
				{
					foreach (EnemyTokenHandler enemyToken in roomButtons[room.GetCoords()].GetAllEnemiesInRoom())
					{
						EnemyTokenHandler newEnemyToken=Instantiate(enemyTokenPrefab);
						newEnemyToken.AssignEnemy(enemy);
						//EncounterEnemy catchEnemy=enemy;
						enemyTokens.Add (enemy,newEnemyToken);
						//EMoveEnemies+=newEnemyToken.TokenRoundoverTrigger;//catchEnemy.Move;
						EMemberMoved+=newEnemyToken.UpdateTokenVision;//catchEnemy.VisionUpdate;
					}
				}*/
				
				//EnemyTokenHandler newHandler=
				if (room.isEntrance) 
				{
					entranceRoom=room;
				}
			}
		}
		
		/*
		Vector2 newSizeDelta=new Vector2(encounterMapGroup.transform.parent.GetComponent<RectTransform>().rect.width
		,encounterMapGroup.transform.parent.GetComponent<RectTransform>().rect.height);
		newSizeDelta=new Vector2(Mathf.Max(maxX+elementSize,newSizeDelta.x),Mathf.Max(Mathf.Abs(minY)+elementSize,newSizeDelta.y));*/
		encounterMapGroup.GetComponent<RectTransform>().sizeDelta=/*newSizeDelta;*/new Vector2(maxX+elementSize,Mathf.Abs(minY)+elementSize);
		//
		
		//Create party tokens
		foreach (PartyMember member in encounterMembers)
		{
			
			//Add member tokens
			MemberTokenHandler newMemberToken=Instantiate(memberTokenPrefab);
			newMemberToken.AssignMember(member);
			//newMemberToken.GetComponent<Image>().color=member.color;
			memberTokens.Add(member,newMemberToken);
			//TESTING
			//print ("generating raw member movemask!");
			iterationCount=0;
			memberMoveMasks.Add(member,IterativeGrassfireMapper(entranceRoom.GetCoords()));//new Dictionary<Vector2, int>());
			//print ("raw movemask generated! Iteration count:"+iterationCount);
			//Add member selectors
			/*
			CombatSelectorHandler newSelector=Instantiate(selectorPrefab);
			newSelector.AssignMember(member,newMemberToken.GetComponent<MemberTokenHandler>());
			newSelector.transform.SetParent(combatSelectorGroup,false);
			selectors.Add(member,newSelector);*/
			
			//position party at entrance
			//
		}
		
		//Place all members at entrance
		List<Vector2> roomsSortedByDistanceFromEntrance=GetFreeRoomsNearEntrance(new Vector2(entranceRoom.xCoord,entranceRoom.yCoord));
		foreach (PartyMember member in encounterMembers)
		{
			//MovePartyMemberToRoom(member,entranceRoom);
			PlacePartyMemberAtEntrance(member,entranceRoom,roomsSortedByDistanceFromEntrance);
		}
		SelectMember(memberTokens[encounterMembers[0]]);
		//Necessary for correct spawn sequencing
		StartCoroutine(EncounterStartViewFocus(roomButtons[memberCoords[encounterMembers[0]]].GetComponent<RectTransform>()));
		//FocusViewOnRoom(roomButtons[memberCoords[encounterMembers[0]]].GetComponent<RectTransform>());
		//MovePartyToRoom(entranceRoom);
	}
	
	void DisableRender() 
	{
		eventLogText.text="";
		GetComponent<Canvas>().enabled=false;
		//eventLogMask.enabled=false;
		currentMap=null;
		EMemberMoved=null;
		selectedMember=null;
		//memberToken.transform.SetParent(this.transform,false);
		//memberToken.SetActive(false);
		
		//clear member selectors
		//foreach (CombatSelectorHandler memberSelector in selectors.Values) {GameObject.Destroy(memberSelector.gameObject);}
		//selectors.Clear();
		
		//clear member tokens
		foreach (MemberTokenHandler memberToken in memberTokens.Values) 
		{
			GameObject.Destroy(memberToken.gameObject);
		}
		memberTokens.Clear();
		memberMoveMasks.Clear();
		
		//clean enemy tokens from rooms
		foreach (EnemyTokenHandler tokenHandler in enemyTokens.Values) 
		{
			//EMoveEnemies-=tokenHandler.TokenRoundoverTrigger;
			EMemberMoved-=tokenHandler.UpdateTokenVision;
			GameObject.Destroy(tokenHandler.gameObject);
		}
		enemyTokens.Clear();
		
		//clean map rooms
		foreach (RoomButtonHandler handler in roomButtons.Values) {GameObject.Destroy(handler.gameObject);}
		roomButtons.Clear();
		foreach (GameObject invisibleWall in wallPaddingButtons) {GameObject.Destroy(invisibleWall);}
		
		
	}
	
	public void RemoveEncounterMember(PartyMember member)
	{
		if (encounterOngoing) roomButtons[memberCoords[member]].MemberGone(memberTokens[member]);
		encounterMembers.Remove(member);
		memberCoords.Remove(member);
		//GameObject.Destroy(selectors[member].gameObject);
		//selectors.Remove(member);
		GameObject.Destroy(memberTokens[member].gameObject);
		memberTokens.Remove(member);
		/*
		//If this was the last member, end encounter. Else - switch to next unacted member, or end round if all remaining members acted.
		if (encounterMembers.Count==0) {EndEncounter();}
		else
		{
			bool unactedMemberFound=false;
			foreach (MemberTokenHandler handler in memberTokens.Values)
			{
				if (!handler.turnTaken) 
				{
					SelectMember(handler);
					unactedMemberFound=true;
					break;
				}
			}
			if (!unactedMemberFound) RoundOver();
		}*/	
	}

	public void ExitClicked(RoomButtonHandler roomHandler)
	{
		if (memberCoords[selectedMember]==new Vector2(roomHandler.assignedRoom.xCoord,roomHandler.assignedRoom.yCoord))
		{
			AddNewLogMessage(selectedMember.name+" escapes!");
			EncounterRoom exitRoom=roomHandler.assignedRoom;//currentEncounter.encounterMap[memberCoords[selectedMember]];
			RemoveEncounterMember(selectedMember);
			if (EEncounterExited!=null) EEncounterExited(selectedMember);
			//selectedMember.EncounterEndTrigger();
			//PartyManager.mainPartyManager.MemberLeavesEncounter();
			//Dumps member's inventory to common inventory
			/*
			List<InventoryItem> membersItems=new List<InventoryItem>();
			membersItems.AddRange(selectedMember.carriedItems.ToArray());
			foreach (InventoryItem carriedItem in membersItems) 
			{
				selectedMember.carriedItems.Remove(carriedItem);
				PartyManager.mainPartyManager.GainItems(carriedItem);
			}*/
			
			if (encounterMembers.Count>0) 
			{
				
				RevealPartySenses();
				TurnOver(selectedMember,exitRoom,false);
			}
			else
			{
				EndEncounter();
				MapManager.main.hordeLocked=false;
			}
		}
	}
	
	public void LootClicked(RoomButtonHandler roomHandler)
	{
		if (memberCoords[selectedMember]==new Vector2(roomHandler.assignedRoom.xCoord,roomHandler.assignedRoom.yCoord))//&& !memberTokens[selectedMember].moveTaken)
		{
			AddNewLogMessage(selectedMember.name+" loots the room");
			roomHandler.LootRoom();
			MakeNoise(roomHandler.GetRoomCoords(),lootingEnemySpawnProbIncrease);
			TriggerEnemySpawnProbability(roomHandler);
			TurnOver(selectedMember,roomHandler.assignedRoom,false);
		}
	}
	
	 
	
	public void BashClicked(RoomButtonHandler roomHandler)
	{
		if (memberCoords[selectedMember]==new Vector2(roomHandler.assignedRoom.xCoord,roomHandler.assignedRoom.yCoord)
		)//&& !memberTokens[selectedMember].moveTaken)
		{
			int bashStrength=selectedMember.MeleeAttack();
			/*
			bool lockExpertInTeam=false;
			foreach (PartyMember member in encounterMembers)
			{
				if (member.isLockExpert) {lockExpertInTeam=true; break;}
			}*/
			AddNewLogMessage(selectedMember.name+" bashes the lock!");
			AddNewLogMessage(selectedMember.name+" makes a lot of noise!");
			roomHandler.BashLock(bashStrength);
			if (!roomHandler.assignedRoom.lootIsLocked) {AddNewLogMessage("The lock breaks!");}
			//Do noise
			MakeNoise(roomHandler.GetRoomCoords(),bashingEnemySpawnProbIncrease);
			TriggerEnemySpawnProbability(roomHandler);
			TurnOver(selectedMember,roomHandler.assignedRoom,false);
		}
	}
	
	public void BarricadeBuildClicked(RoomButtonHandler roomHandler)
	{
		if (memberCoords[selectedMember]==new Vector2(roomHandler.assignedRoom.xCoord,roomHandler.assignedRoom.yCoord)
		    )//&& !memberTokens[selectedMember].moveTaken)
		{
			AddNewLogMessage(selectedMember.name+" throws together a barricade");
			roomHandler.BarricadeRoom();
			TurnOver(selectedMember,roomHandler.assignedRoom,false);
		}
	}
	
	public void BarricadeBashClicked(RoomButtonHandler roomHandler)
	{
		if (!roomHandler.RoomHasEnemies() 
		    && Mathf.Abs(memberCoords[selectedMember].x-roomHandler.assignedRoom.xCoord)
		    +Mathf.Abs(memberCoords[selectedMember].y-roomHandler.assignedRoom.yCoord)<=1
		    )//&& !memberTokens[selectedMember].moveTaken)
		{
			int damageToBarricade=selectedMember.MeleeAttack();
			AddNewLogMessage(selectedMember.name+" smashes the barricade for "+damageToBarricade);
			roomHandler.BashBarricade(damageToBarricade);
			//SendFloatingMessage(damageToBarricade.ToString(),roomHandler.transform,Color.black);
			//if (roomHandler.assignedRoom.barricadeInRoom==null) {AddNewLogMessage("The barricade is cleared!");}
			TurnOver(selectedMember,roomHandler.assignedRoom,false);
		}
	}
	
	public void TurnoverSelectedMember()
	{
		TurnOver(selectedMember,roomButtons[memberCoords[selectedMember]].assignedRoom,false);
	}
	
	
	//select active member for combat (for button press and also private turnover reset)
	
	public void ClickMember(MemberTokenHandler selectedHandler)
	{
		if (selectedMember!=selectedHandler.myMember)
		{
			SelectMember(selectedHandler);
		} 
		else TurnOver(selectedMember,roomButtons[memberCoords[selectedMember]].assignedRoom,true);
	}
	
	public void MakeNoise(Vector2 originCoords, float probabilityIncrease)
	{
		//Create text
		//Noise distance mechanic currently disabled
		int carryDistance=0;
		if (encounterOngoing)
		{
			string noiseText="Noise ("+probabilityIncrease+")";
			//for (int i=0; i<carryDistance; i++) {noiseText+="+";}
			SendFloatingMessage(noiseText,roomButtons[originCoords].transform);
			//Do effect
			//Dictionary<Vector2,int> moveMaskToSource=IterativeGrassfireMapper(originCoords);
			for (int i=-carryDistance; i<=carryDistance; i++)
			{
				for (int j=-carryDistance; j<=carryDistance; j++)
				{
					Vector2 cursorCoords=originCoords+new Vector2(j,i);
					if (roomButtons.ContainsKey(cursorCoords))
					{
						if (!roomButtons[cursorCoords].assignedRoom.isWall) 
                        SetCurrentSpawnThreat(currentSpawnThreat+probabilityIncrease);
						//roomButtons[cursorCoords].enemySpawnProbability+=probabilityIncrease;//SpawnEnemy(roomButtons[cursorCoords].assignedRoom);
						/*
						if (roomButtons[cursorCoords].assignedRoom.hasEnemies)
						{
							foreach (EncounterEnemy enemy in roomButtons[cursorCoords].assignedRoom.enemiesInRoom)
							{
								enemyTokens[enemy].AddNewPOI(originCoords,null,carryDistance);//moveMaskToSource);
							}		
						}*/
						//SpawnEnemy(,);

					}
				}
			}
		}
	}	

	void TriggerEnemySpawnProbability(RoomButtonHandler spawnRoomHandler)
	{
		//Roll to spawn enemies on moving into room
        float actualSpawnThreat = currentSpawnThreat;
        if (spawnRoomHandler.assignedRoom.isVisited) actualSpawnThreat *= 0.5f;
        if (Random.value < actualSpawnThreat) TriggerEnemySpawn(spawnRoomHandler.assignedRoom);
	}

	void SelectMember(MemberTokenHandler selectedHandler) 
	{
		PartyMember previousSelectedMember=null;
		if (selectedMember!=null) previousSelectedMember=selectedMember;
		selectedHandler.Select();
		foreach (MemberTokenHandler handler in memberTokens.Values)//CombatSelectorHandler handler in selectors.Values)
		{
			if (handler!=selectedHandler) {handler.Deselect();}
		}
		selectedMember=selectedHandler.myMember;
		roomButtons[memberCoords[selectedHandler.myMember]].SelectedMemberMovesUpdate(selectedHandler);

		if (previousSelectedMember!=null)
		{
			//Remove old move to front/back buttons on member select change (if selecting a member in a different room)
			if (memberCoords.ContainsKey(previousSelectedMember)) roomButtons[memberCoords[previousSelectedMember]].UpdateMemberGroup();
		}

		//FocusViewOnRoom(roomButtons[memberCoords[selectedMember]].GetComponent<RectTransform>());
	}
	/*
	//against enemies
	IEnumerator VisualizeAttackOnEnemy(int dmg, EncounterEnemy attackedEnemy, IAttackAnimation attackingEntity)//, bool blockInteraction)//PartyMember attackingMember)
	{
		DamageNumberHandler newHandler=Instantiate(damageNumberPrefab);
		newHandler.AssignNumber(dmg);
		
		if (enemyTokens.ContainsKey(attackedEnemy))
		{
			newHandler.GetComponent<Text>().color=Color.magenta;
			newHandler.transform.SetParent(enemyTokens[attackedEnemy].transform,false);
			
			//if (blockInteraction) 
			//{
				GetComponent<CanvasGroup>().interactable=false;
				//print ("Encounter manager beginning wait on attack animation routine");
				yield return StartCoroutine(attackingEntity.AttackAnimation());
				//print ("Encounter manager resuming from attack animation routine");
				GetComponent<CanvasGroup>().interactable=true;
			//}
			//else {StartCoroutine(attackingEntity.AttackAnimation());}
			
			//memberTokens[attackingMember].AnimateAttack();
		}
	}*/
	
	public void SendFloatingMessage(string text, Transform startTransform) {SendFloatingMessage(text, startTransform, Color.red);}
	
	public void SendFloatingMessage(string text, Transform startTransform, Color textColor)
	{
		FloatingTextHandler newHandler=Instantiate(floatingTextPrefab);
		newHandler.AssignText(text);
		newHandler.transform.SetParent(startTransform,false);
		newHandler.transform.position=startTransform.position;
		newHandler.GetComponent<Text>().color=textColor;
		
		//newHandler.AssignNumber(dmg);
	}
	
	public void ShowDamageToFriendlyTarget(EnemyAttack attack)//PartyMember damagedMember, EncounterEnemy attackingEnemy, int damage, bool blocked)
	{
		
		//PartyMember damagedMember=attack.attackedMember;
		IGotHitAnimation damagedTarget=attack.attackedTarget;
		EncounterEnemy attackingEnemy=attack.attackingEnemy;
		int damage=attack.damageDealt;
		bool blocked=attack.blocked;// -currently removed
		bool attackHit=attack.hitSuccesful;
		//If the damaged target is a member - do member popups
		string attackMessage="";
		Color textColor=Color.red;
		if (damagedTarget.GetType()==typeof(MemberTokenHandler))
		{
			MemberTokenHandler damagedMemberToken=damagedTarget as MemberTokenHandler;
			if (memberTokens.ContainsValue(damagedMemberToken))
			{
				
				//if (blocked) attackMessage=damagedMember.name+" blocks "+attackingEnemy.name+", losing "+damage+" stamina";
				//else attackMessage=attackingEnemy.name+" attacks "+damagedMember.name+" for "+damage+" damage";

				if (attackHit)
				{
					attackMessage=attackingEnemy.name+" attacks "+damagedMemberToken.myMember.name+"'s "+attack.hitBodyPart.ToString()+" for "+damage+" damage";
					textColor=Color.red;
					SendFloatingMessage(attack.hitBodyPart.ToString()+" -"+damage.ToString(),damagedMemberToken.transform,textColor);
				}
				else
				{
					attackMessage=attackingEnemy.name+" misses "+damagedMemberToken.myMember.name;
					textColor=Color.green;
					SendFloatingMessage("Miss",damagedMemberToken.transform,textColor);
				}
				
				AddNewLogMessage(attackMessage);
				
				
				//if (blocked) textColor=Color.green;
			}
		}
		else
		{
			BarricadeToken damagedBarricadeToken=damagedTarget as BarricadeToken;
			attackMessage=attackingEnemy.name+" attacks a barricade for "+damage+" damage";
			textColor=Color.black;
			SendFloatingMessage("-"+damage.ToString(),damagedBarricadeToken.transform,textColor);
		}
	}

	IEnumerator VisualizeAttack(int dmg, IAttackAnimation attacker, MonoBehaviour defender)
	{
		return VisualizeAttack(true,dmg,false,attacker,defender);
	}
	IEnumerator VisualizeAttack(bool hitSuccessful, int dmg, IAttackAnimation attacker, MonoBehaviour defender)
	{
		return VisualizeAttack(hitSuccessful,dmg,false,attacker,defender);
	}
	IEnumerator VisualizeAttack(bool hitSuccessful, int dmg,bool blocked, IAttackAnimation attacker, MonoBehaviour defender)
	{
		FloatingTextHandler newHandler=Instantiate(floatingTextPrefab);
		if (hitSuccessful) newHandler.AssignNumber(dmg);
		else newHandler.AssignText("Miss");
		//If attacking member
		IGotHitAnimation targetAnimation=null;
		if (defender.GetType()==typeof(MemberTokenHandler))
		{
			MemberTokenHandler defenderToken=defender as MemberTokenHandler;
			if (memberTokens.ContainsValue(defenderToken)) //selectors.ContainsKey(info.damagedMember)) 
			{
				if (blocked) newHandler.GetComponent<Text>().color=Color.green;
				else  newHandler.GetComponent<Text>().color=Color.red;
				newHandler.transform.SetParent(defenderToken.transform,false);
				newHandler.transform.position=defenderToken.transform.position;
				if (hitSuccessful) targetAnimation=defenderToken;
			}
		}
		if (defender.GetType()==typeof(EnemyTokenHandler))
		{
			EnemyTokenHandler defenderToken=defender as EnemyTokenHandler;
			if (enemyTokens.ContainsValue(defenderToken))
			{
				newHandler.GetComponent<Text>().color=Color.magenta;
				newHandler.transform.SetParent(defenderToken.transform,false);
				newHandler.transform.position=defenderToken.transform.position;
				if (hitSuccessful) targetAnimation=defenderToken;
			}
		}
		GetComponent<CanvasGroup>().interactable=false;
		yield return StartCoroutine(attacker.AttackAnimation(targetAnimation));
		GetComponent<CanvasGroup>().interactable=true;
		//If attacking enemy
	}
	/*
	IEnumerator LockButtonsUntilAnimationFinishes(IEnumerator animationRoutine)
	{
		GetComponent<CanvasGroup>().interactable=false;
		yield return StartCoroutine(animationRoutine());
		GetComponent<CanvasGroup>().interactable=true;
		yield break;
	}*/
	
	//against party members
	struct DamageToParty
	{
		public PartyMember damagedMember;
		public int damage;
		public EncounterEnemy attackingEnemy;
		public DamageToParty (PartyMember member, int dmg, EncounterEnemy attacker)
		{
			damagedMember=member;
			damage=dmg;
			attackingEnemy=attacker;
		} 
	}
	/*
	public void StartDamageNumber(int dmg, PartyMember damagedMember, EncounterEnemy attackingEnemy)
	{
		StartCoroutine("StaggerOutDamageNumbers",new DamageToParty(damagedMember,dmg, attackingEnemy));
	}
	
	IEnumerator StaggerOutDamageNumbers(DamageToParty info)
	{
		while (damageNumberOut)
		{
			yield return new WaitForFixedUpdate();
		}
		if (memberTokens.ContainsKey(info.damagedMember)) //selectors.ContainsKey(info.damagedMember)) 
		{
			DamageNumberHandler newHandler=Instantiate(damageNumberPrefab);
			newHandler.GetComponent<Text>().color=Color.red;
			newHandler.AssignNumber(info.damage);
			newHandler.transform.SetParent(memberTokens[info.damagedMember].transform,false);//selectors[info.damagedMember].transform,false);
			//enemyTokens[info.attackingEnemy].AnimateAttack();
			damageNumberOut=true;
			yield return new WaitForSeconds(0.5f);
			damageNumberOut=false;
		}
		yield break;
	}*/
	/*
	public void RegisterTrapDamage(int damage, EncounterEnemy attackedEnemy)
	{
		int actualDmg=roomButtons[attackedEnemy.GetCoords()].AttackEnemyInRoom(damage,attackedEnemy,isRanged);
		
		StartCoroutine(VisualizeMemberAttack(actualDmg,attackedEnemy, attackingMember));
		if (attackedEnemy.health<=0) 
		{
			selectedMember.ReactToKill();
			//EMoveEnemies-=attackedEnemy.Move;
			//EMemberMoved-=attackedEnemy.VisionUpdate;
			EMoveEnemies-=enemyTokens[attackedEnemy].DoTokenMove;//enemy.Move;
			EMemberMoved-=enemyTokens[attackedEnemy].UpdateTokenVision;//enemy.VisionUpdate;
			GameObject.Destroy(enemyTokens[attackedEnemy].gameObject);
			enemyTokens.Remove(attackedEnemy);
		}
	}*/
	/*
		//Order of PartyMember and EncounterEnemy parameters is important!!!
	public void StartDamageNumber(int dmg, PartyMember damagedMember, EncounterEnemy attackingEnemy)
	{
		StartCoroutine("StaggerOutDamageNumbers",new DamageToParty(damagedMember,dmg, attackingEnemy));
	}
	*/
	
	//Damage dealt to enemies
	//Used by callbacks from set off traps
	public void RegisterDamageToEnemy(int damage, EnemyBodyPart attackedPart, bool isRanged,EncounterEnemy attackedEnemy, IAttackAnimation attackingEntity, bool hideResult)
	{
		StartCoroutine(RegisterDamageToEnemy(true,damage,attackedPart,isRanged,attackedEnemy,attackingEntity,hideResult));
	}
	public IEnumerator RegisterDamageToEnemy(bool hitSuccessful, int damage
	, EnemyBodyPart attackedPart, bool isRanged,EncounterEnemy attackedEnemy, IAttackAnimation attackingEntity)
	{
		return RegisterDamageToEnemy(hitSuccessful,damage,attackedPart,isRanged,attackedEnemy,attackingEntity,false);
	}
	//Used by callbacks from member tokens (and also daisy-chained by the overload above)
	public IEnumerator RegisterDamageToEnemy(bool hitSuccessful, int damage
	, EnemyBodyPart attackedPart, bool isRanged,EncounterEnemy attackedEnemy, IAttackAnimation attackingEntity, bool hideResult)//Object attackingEntity)//PartyMember attackingMember)
	{
		//EncounterRoom currentRoom=currentEncounter.encounterMap[memberCoords[selectedMember]];
		//int actualDmg=currentRoom.DamageEnemy(damage,attackedEnemy,isRanged);
		int actualDmg=0;
		if (hitSuccessful) actualDmg=attackedEnemy.TakeDamage(damage,attackedPart,isRanged);//roomButtons[attackedEnemy.GetCoords()].AttackEnemyInRoom(damage,attackedPart,attackedEnemy,isRanged);


		//bool blockInteraction=attackingEntity.GetType()==typeof(MemberTokenHandler);
		//If hide is not set, do proper attack/defend animation sequencing
		if (!hideResult) yield return StartCoroutine(VisualizeAttack(hitSuccessful,actualDmg,attackingEntity,enemyTokens[attackedEnemy]));//VisualizeAttackOnEnemy(actualDmg,attackedEnemy, attackingEntity));
		MemberTokenHandler attackingMemberToken=null;
		//Find if attacker is member or trap
		if (attackingEntity.GetType()==typeof(MemberTokenHandler)) attackingMemberToken=attackingEntity as MemberTokenHandler;
		//If attacker is member
		if (attackingMemberToken!=null) 
		{
			if (!hideResult)
			{
				if (hitSuccessful)
				{
					//MakeNoise(memberCoords[attackingMemberToken.myMember],attackSoundIntensity);

					string hitMessage=" hits ";
					if (isRanged) hitMessage=" shoots ";
					AddNewLogMessage(attackingMemberToken.myMember.name+" makes some noise!");
					AddNewLogMessage(attackingMemberToken.myMember.name+hitMessage+attackedEnemy.name+" for "+damage+" damage");
				}
				else
				{
					string hitMessage=" misses ";
					AddNewLogMessage(attackingMemberToken.myMember.name+hitMessage+attackedEnemy.name+"!");
				}
			}
		}//if attacker is trap !!! RESULTS WILL ONLY BE HIDDEN FOR TRAP ATTACKS
		else 
		{
			if (!hideResult) AddNewLogMessage("a trap does "+damage+" damage to "+attackedEnemy.name+"'s "+attackedPart.name);
		}
		//CAUTION!-the enemy's token should only be destroyed here, to make sure it doesn't get destroyed before the hit animation proprely plays
		if (attackedEnemy.health<=0) 
		{
			if (!hideResult) AddNewLogMessage(attackedEnemy.name+" is killed!");
			if (attackingMemberToken!=null) 
			{
				attackingMemberToken.myMember.ReactToKill();
			}

			RoomButtonHandler enemyRoom=roomButtons[attackedEnemy.GetCoords()];
			DespawnEnemy(attackedEnemy);
			//Reduce enemy spawn probability down to 0 if all enemies have been killed
            if (!enemyRoom.RoomHasEnemies()) SetCurrentSpawnThreat(0);
		}
		yield break;
	}

	public void DespawnEnemy(EncounterEnemy enemy)
	{
		if (enemyTokens.ContainsKey(enemy))
		{
			roomButtons[enemy.GetCoords()].EnemyTokenDestroyed(enemyTokens[enemy]);//.assignedRoom.enemiesInRoom.Remove(enemy);
			EMemberMoved-=enemyTokens[enemy].UpdateTokenVision;//enemy.VisionUpdate;
			GameObject.Destroy(enemyTokens[enemy].gameObject);
			enemyTokens.Remove(enemy);
		}
		else throw new System.Exception("Attempting to despawn enemy without a token!");
	}
	/*
	//If the attack is blocked, damage is stamina damage, else damage is health damage
	public void VisualizeDamageToMember(int damage,bool blocked,PartyMember attackedMember, EncounterEnemy attackingEnemy)
	{
		string attackMessage="";
		if (blocked) attackMessage=attackedMember.name+" blocks "+attackingEnemy.name+", losing "+damage+" stamina";
		else attackMessage=attackingEnemy.name+" attacks "+attackedMember.name+" for "+damage+" damage";
		AddNewLogMessage(attackMessage);
		StartCoroutine(AddEnemyAttackAnimationToQueue(damage, blocked, attackedMember,attackingEnemy));
	}
	//If the attack is blocked, damage is stamina damage, else damage is health damage
	IEnumerator AddEnemyAttackAnimationToQueue(int damage, bool blocked, PartyMember attackedMember, EncounterEnemy attackingEnemy)
	{
		while (damageNumberOut)
		{
			yield return new WaitForFixedUpdate();
		}
		if (memberTokens.ContainsKey(attackedMember)) //selectors.ContainsKey(info.damagedMember)) 
		{
			damageNumberOut=true;
			yield return StartCoroutine(VisualizeAttack(damage,blocked,enemyTokens[attackingEnemy],memberTokens[attackedMember]));
			damageNumberOut=false;
		}
		yield break;//
	}*/
	
	public void EnemyClicked(EncounterEnemy enemy)
	{
		/*
		//int distance=Mathf.Max(Mathf.Abs(memberCoords[selectedMember].x-enemy.GetCoords().x)
		//,Mathf.Abs(memberCoords[selectedMember].y-enemy.GetCoords().y));
		bool ranged=memberTokens[selectedMember].rangedMode;
		int attackRange;
		if (ranged) {attackRange=100;}
		else {attackRange=0;}

		//See if clicked enemy passes attack range check
		if (Mathf.Abs(memberCoords[selectedMember].x-enemy.GetCoords().x)+Mathf.Abs(memberCoords[selectedMember].y-enemy.GetCoords().y)
		<=attackRange) 
		{
			bool memberReachable=true;
			//Second - see if any walls are blocking member (for ranged attacks)
			if (attackRange>0)
			{
				BresenhamLines.Line((int)memberCoords[selectedMember].x,(int)memberCoords[selectedMember].y
				,(int)enemy.GetCoords().x,(int)enemy.GetCoords().y,(int x, int y)=>
				{
					//bool visionClear=true;
					if (currentEncounter.encounterMap[new Vector2(x,y)].isWall) {memberReachable=false;}
					return memberReachable;
				});
			}
			
			if (memberReachable) 
			{
				int actualDmg=0;
				if (ranged) {actualDmg=selectedMember.RangedAttack();}
				else {actualDmg=selectedMember.MeleeAttack();}
				RegisterDamage(actualDmg,ranged,enemy,selectedMember);
			}
		}*/
		bool ranged=false;
		bool enemyReachable=memberTokens[selectedMember].AttackIsPossible(ref ranged,enemy);
		
		if (enemyReachable)
		{
			//AttackOnEnemy(enemy, ranged);
			AttackLocationSelector newAtkSelector=Instantiate(attackLocSelectorPrefab);
			newAtkSelector.EnableNewAttackSelector(enemyTokens[enemy],ranged);
		}
		
		/*
		if (memberCoords[selectedMember]==enemy.GetCoords())//distance==0)
		{
			int actualDmg=0;
			//Member token does the check to see if ranged attack is possible
			
			if (ranged) {actualDmg=selectedMember.RangedAttack();}
			else actualDmg=selectedMember.MeleeAttack();
			string msg="";//selectedMember.name+" hits the "+enemy.name+" for "+actualDmg+"!";
			RegisterDamage(actualDmg,msg,ranged,enemy,selectedMember);
		}*/
	}
	
	public void AttackOnEnemy(EncounterEnemy enemy, bool ranged, EnemyBodyPart attackedPart, float hitChance)
	{
		StartCoroutine(AttackAnimationTurnoverSequence(enemy,ranged,attackedPart,hitChance));
	}
	IEnumerator AttackAnimationTurnoverSequence(EncounterEnemy enemy, bool ranged, EnemyBodyPart attackedPart, float hitChance)
	{
		int actualDmg=0;
		bool hitSuccessful=false;
		if (Random.value<=hitChance) hitSuccessful=true;
		{
			if (ranged) actualDmg=selectedMember.RangedAttack();
			else actualDmg=selectedMember.MeleeAttack();
		}
		yield return StartCoroutine(RegisterDamageToEnemy(hitSuccessful,actualDmg,attackedPart,ranged,enemy,memberTokens[selectedMember]));
		bool doTurnover=true;
		if (selectedMember.hitAndRunEnabled) 
		{
			memberTokens[selectedMember].TryMove(out doTurnover);
			memberTokens[selectedMember].attackDone=true;
		}
		if (doTurnover) TurnOver(selectedMember,roomButtons[memberCoords[selectedMember]].assignedRoom,false);
	}
	
	public void AddNewLogMessage(string newMessage)
	{
		eventLogText.text+="\n-"+newMessage;
		Canvas.ForceUpdateCanvases();
		//scrollRect.verticalScrollbar.value=0f;
		//eventLogScrollbar.value=0;
		eventLogText.transform.GetComponentInParent<ScrollRect>().verticalNormalizedPosition=0;
		Canvas.ForceUpdateCanvases();
	}
	/*
	//MOVE THIS TO THE ONLY PLACE IT IS USED IN LATER
	void AdvanceToNextMember()
	{
		//select next member that hasn't acted yet, if one exists, else do turnover
		bool allActionsTaken=true;
		foreach (MemberTokenHandler token in memberTokens.Values)//CombatSelectorHandler selector in selectors.Values) 
		{
			if (!token.actionTaken) 
			{
				allActionsTaken=false; 
				SelectMember(token);
				break;
			}
		}
		//if all members acted (none available to move current selection to)
		if (allActionsTaken) 
		{
			foreach (MemberTokenHandler token in memberTokens.Values)//CombatSelectorHandler selector in selectors.Values) 
			{
				token.actionTaken=false;
			}
			//Do roundover
			RoundOver();
			DeadMemberCleanupCheck();
			//Make sure this doesn't update after the encounter is finished (from a TPK)
			if (encounterOngoing)
			{
				SelectMember(memberTokens[encounterMembers[0]]);//selectors[encounterMembers[0]]);
			}
		}
	}*/
	
	void Start() 
	{
		main=this;
		GameManager.GameOver+=EndEncounter;
	}
	
	void Update()
	{
		if (encounterOngoing && GetComponent<CanvasGroup>().interactable)
		{
			Vector2 cursor=memberCoords[selectedMember];
			if (Input.GetKeyDown(KeyCode.W)) 
			{
				cursor=memberCoords[selectedMember]+Vector2.down;
				if (roomButtons.ContainsKey(cursor))
				{
					if (!roomButtons[cursor].assignedRoom.isWall)
					RoomClicked(roomButtons[cursor]);
				}
			}
			if (Input.GetKeyDown(KeyCode.A)) 
			{
				cursor=memberCoords[selectedMember]+Vector2.left;
				if (roomButtons.ContainsKey(cursor))
				{
					if (!roomButtons[cursor].assignedRoom.isWall)
						RoomClicked(roomButtons[cursor]);
				}
			}
			if (Input.GetKeyDown(KeyCode.S)) 
			{
				cursor=memberCoords[selectedMember]+Vector2.up;
				if (roomButtons.ContainsKey(cursor))
				{
					if (!roomButtons[cursor].assignedRoom.isWall)
						RoomClicked(roomButtons[cursor]);
				}
			}
			if (Input.GetKeyDown(KeyCode.D)) 
			{
				cursor=memberCoords[selectedMember]+Vector2.right;
				if (roomButtons.ContainsKey(cursor))
				{
					if (!roomButtons[cursor].assignedRoom.isWall)
						RoomClicked(roomButtons[cursor]);
				}
			}
			if (Input.GetKeyDown(KeyCode.Space)) 
			{
				List<PartyMember> unactedMembers=new List<PartyMember>();
				foreach (PartyMember member in encounterMembers)
				{
					if (!memberTokens[member].turnTaken) unactedMembers.Add(member);//TurnOver(member,roomButtons[memberCoords[selectedMember]].assignedRoom,true);
				}
				foreach (PartyMember unactedMember in unactedMembers) 
				{
					TurnOver(unactedMember,roomButtons[memberCoords[unactedMember]].assignedRoom,true);
				}
			}

			if (Input.GetKeyDown(KeyCode.I))
			{
				ToggleMemberInventory(selectedMember);
			}

			if (Input.GetKeyDown(KeyCode.Q)) 
			{
				if (!InventoryScreenHandler.mainISHandler.inventoryShown)
				{
					List<PartyMember> cyclableMembers=new List<PartyMember>(encounterMembers);
					foreach (MemberTokenHandler token in memberTokens.Values)
					{
						if (token.turnTaken) {cyclableMembers.Remove(token.myMember);}
					}
					SelectMember(memberTokens[cyclableMembers[(int)Mathf.Repeat(cyclableMembers.IndexOf(selectedMember)+1,cyclableMembers.Count)]]);
				}
			}
			/*
			if (Input.GetKeyDown(KeyCode.P))
			{
				TriggerEnemySpawn(roomButtons[memberCoords[selectedMember]].assignedRoom);
			}*/

			//ZOOM OUT
			if (Input.GetAxis("Mouse ScrollWheel")<0)
			{
				//Do zoom
				float scaleFactor=Mathf.Max(encounterMapGroup.transform.localScale.x*zoomFactor,Mathf.Pow(zoomFactor,minZoomPower));
				encounterMapGroup.transform.localScale=new Vector3(scaleFactor,scaleFactor,1);
			}
			//ZOOM IN
			if (Input.GetAxis("Mouse ScrollWheel")>0)
			{
				//Find room under cursor before zoom occurs
				PointerEventData pointerData=new PointerEventData(EventSystem.current);
				pointerData.position=Input.mousePosition;
				List<RaycastResult> raycastResults=new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerData,raycastResults);

				RectTransform zoomRoomTransform=null;
				//print("Raycast done, parsing raycast result");
				foreach (RaycastResult result in raycastResults)
				{
					//print("Checking hit object type:"+result.gameObject.GetType());
					if (result.gameObject.GetComponent<RoomButtonHandler>()!=null)
					{
						zoomRoomTransform=result.gameObject.GetComponent<RectTransform>();
						//print("View focused on cursor room!");
						break;
					}
				}

				//Do zoom
				float scaleFactor=Mathf.Min(encounterMapGroup.transform.localScale.x/zoomFactor,1/Mathf.Pow(zoomFactor,maxZoomPower));
				encounterMapGroup.transform.localScale=new Vector3(scaleFactor,scaleFactor,1);
				//Focus view on cursor room after zoom
				if (zoomRoomTransform!=null) FocusViewOnRoom(zoomRoomTransform);
			}
		}
	}
}
