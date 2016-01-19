using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class EncounterCanvasHandler : MonoBehaviour 
{
	//int iterationCount=0;
	
	public bool encounterOngoing=false;

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
	delegate void MemberMovedDel ();
	event MemberMovedDel EMemberMoved;
	
	public delegate void RoundoverDel();
	public event RoundoverDel ERoundIsOver;
	
	void FocusViewOnRoom(RectTransform roomTransform)
	{
		Vector2 newPosition=roomTransform.localPosition;
		newPosition.x/=encounterMapGroup.GetComponent<RectTransform>().rect.width;
		newPosition.y/=encounterMapGroup.GetComponent<RectTransform>().rect.height;//
		encounterMapGroup.parent.GetComponent<ScrollRect>().normalizedPosition=newPosition;
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
		memberCoords.Clear();
		
		encounterMembers.AddRange(membersOnMission.ToArray());
		foreach (PartyMember encounterMember in encounterMembers) 
		{
			encounterMember.EncounterStartTrigger(encounterMembers);
			memberCoords.Add(encounterMember,Vector2.zero);
			
		}
		currentEncounter=newEncounter;
		currentMap=new EncounterMap(newEncounter.encounterMap);
		EnableRender();
		//If members ambushed, alert all enemies to their presence
		if (isAmbush) MakeNoise(memberCoords[encounterMembers[0]],100);
	}
	
	void EndEncounter()
	{
		if (encounterOngoing)
		{
			currentEncounter.RandomizeEnemyPositions();
			DisableRender();
			encounterOngoing=false;
		}
	}
	
	public void RoomClicked(RoomButtonHandler roomHandler)//EncounterRoomDrawer roomDrawer)
	{
		
		int moveDistance=0;	
		EncounterRoom startingRoom=currentEncounter.encounterMap[memberCoords[selectedMember]];
		moveDistance=Mathf.Abs((int)startingRoom.xCoord-roomHandler.roomX)+Mathf.Abs((int)startingRoom.yCoord-roomHandler.roomY);//encounterPlayerX-room.xCoord)+Mathf.Abs(encounterPlayerY-room.yCoord);
		
		if (GetComponent<CanvasGroup>().interactable
		&& moveDistance==1 
		&& !roomHandler.assignedRoom.isWall 
		&& (roomHandler.assignedRoom.barricadeInRoom==null || selectedMember.isScout)) //&& !currentEncounter.encounterMap[memberCoords[selectedMember]].hasEnemies)//new Vector2(encounterPlayerX,encounterPlayerY)].hasEnemies)
		{
			StartCoroutine(MoveMemberToRoom(roomHandler));
		}//
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
			//Enemies get a free swipe if you are the last to move out of their spot
			//(moving member's coords don't get updated until after the check, so this is necessary)
			/*
				List<Vector2> nonMovingMemberCoords=new List<Vector2>(memberCoords.Values);
				nonMovingMemberCoords.Remove(memberCoords[selectedMember]);
				if (!nonMovingMemberCoords.Contains(startingRoom.GetCoords()))
				*/
			if (startingRoom.hasEnemies)// && !selectedMember.isScout)
			{
				GetComponent<CanvasGroup>().interactable=false;
				List<PartyMember> argumentListOfOne=new List<PartyMember>();
				argumentListOfOne.Add(selectedMember);
				foreach (EncounterEnemy enemy in startingRoom.enemiesInRoom) 
				{
					yield return StartCoroutine(enemyTokens[enemy].TokenAttackTrigger(selectedMember));
				}
				GetComponent<CanvasGroup>().interactable=true;
			}
			//if last party member didn't die moving away
			if (encounterOngoing) 
			{
				MovePartyMemberToRoom(selectedMember,roomHandler,false);
				MakeNoise(roomHandler.GetRoomCoords(),2);
				DeadMemberCleanupCheck();
				//If moved member died during move, try to switch to next available member
				if (encounterOngoing)
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
						if (!unactedMemberFound) ToggleRoundSwitch();
					}
					else
					{
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
		//First - unreveal everything
		foreach (RoomButtonHandler cycledRoom in roomButtons.Values) 
		{
			cycledRoom.SetVisibility(false);
			cycledRoom.SetHearing(false);
		}
		
		//Iterate through every present player
		foreach (PartyMember member in encounterMembers)
		{
			//Default daytime range
			int visionRange=1;
			//int verticalVisionRange=3;
			//int horizontalVisionRange=4;
			if (PartyManager.mainPartyManager.dayTime<6 | PartyManager.mainPartyManager.dayTime>18) 
			{
				//Nighttime range
				//visionRange=1;
				//Nighttime range with flashlight
				//if (member.hasLight) {visionRange+=2;}
			}
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
						if (roomButtons.ContainsKey(roomCoords)) roomButtons[roomCoords].SetHearing(true);
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
						foreach (Vector2 roomCoords in lineCoords) {roomButtons[roomCoords].SetVisibility(true);}//currentEncounter.encounterMap[roomCoords].isVisible=true;}
					}
				}
				
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
		if (room.hasEnemies)
		{
			foreach (EncounterEnemy enemy in room.enemiesInRoom) {enemy.TurnAction();}
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
		if (encounterOngoing) NextRound();
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
		//print ("entrance mask created. Iteration count:"+iterationCount);
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
		enemyTokens=new Dictionary<EncounterEnemy, EnemyTokenHandler>();
		memberTokens=new Dictionary<PartyMember, MemberTokenHandler>();
		
		//combatWaitButton.SetActive(true);
		memberMoveMasks=new Dictionary<PartyMember, Dictionary<Vector2, int>>();
		//Encounter currentEncounter=EncounterManager.mainEncounterManager.currentEncounter;
		encounterMapGroup.GetComponent<GridLayoutGroup>().constraintCount=(currentEncounter.maxX+1)-currentEncounter.minX;
		
		EncounterRoom entranceRoom=null;
		for (int i=currentEncounter.minY; i<=currentEncounter.maxY;i++)
		{
			for (int j=currentEncounter.minX; j<=currentEncounter.maxX;j++)
			{
				EncounterRoom room=currentEncounter.encounterMap[new Vector2(j,i)];
				RoomButtonHandler newRoomButton=Instantiate(roomPrefab);
				roomButtons.Add(new Vector2(j,i),newRoomButton);
				newRoomButton.AssignRoom(room);
				newRoomButton.transform.SetParent(encounterMapGroup,false);
				
				//Add enemy token if room has enemy
				if (room.hasEnemies) 
				{
					foreach (EncounterEnemy enemy in room.enemiesInRoom)
					{
						EnemyTokenHandler newEnemyToken=Instantiate(enemyTokenPrefab);
						newEnemyToken.AssignEnemy(enemy);
						//EncounterEnemy catchEnemy=enemy;
						enemyTokens.Add (enemy,newEnemyToken);
						//EMoveEnemies+=newEnemyToken.TokenRoundoverTrigger;//catchEnemy.Move;
						EMemberMoved+=newEnemyToken.UpdateTokenVision;//catchEnemy.VisionUpdate;
					}
				}
				
				//EnemyTokenHandler newHandler=
				if (room.isEntrance) 
				{
					entranceRoom=room;
				}
			}
		}
		
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
		
		//MovePartyToRoom(entranceRoom);
	}
	
	void DisableRender() 
	{
		eventLogText.text="";
		GetComponent<Canvas>().enabled=false;
		//eventLogMask.enabled=false;
		currentMap=null;
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
		foreach (RoomButtonHandler handler in roomButtons.Values) 
		{
			/*
			if (handler.assignedRoom.hasEnemies)
			{
				foreach (EncounterEnemy enemy in handler.assignedRoom.enemiesInRoom) 
				{
					EMoveEnemies-=enemyTokens[enemy].DoTokenMove;//enemy.Move;
					EMemberMoved-=enemyTokens[enemy].UpdateTokenVision;//enemy.VisionUpdate;
				}
			}*/
			GameObject.Destroy(handler.gameObject);
		}
		roomButtons.Clear();
		
		
	}
	
	public void RemoveEncounterMember(PartyMember member)
	{
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
		if (!roomHandler.assignedRoom.hasEnemies && memberCoords[selectedMember]==new Vector2(roomHandler.assignedRoom.xCoord,roomHandler.assignedRoom.yCoord))
		{
			AddNewLogMessage(selectedMember.name+" escapes!");
			EncounterRoom exitRoom=roomHandler.assignedRoom;//currentEncounter.encounterMap[memberCoords[selectedMember]];
			RemoveEncounterMember(selectedMember);
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
				PartyManager.mainPartyManager.PassTime(1);
				EndEncounter();
				MapManager.main.hordeLocked=false;
			}
		}
	}
	
	public void LootClicked(RoomButtonHandler roomHandler)
	{
		if (!roomHandler.assignedRoom.hasEnemies 
		&& memberCoords[selectedMember]==new Vector2(roomHandler.assignedRoom.xCoord,roomHandler.assignedRoom.yCoord)
		)//&& !memberTokens[selectedMember].moveTaken)
		{
			AddNewLogMessage(selectedMember.name+" loots the stash");
			roomHandler.LootRoom();
			TurnOver(selectedMember,roomHandler.assignedRoom,false);
		}
	}
	
	 
	
	public void BashClicked(RoomButtonHandler roomHandler)
	{
		if (!roomHandler.assignedRoom.hasEnemies 
		&& memberCoords[selectedMember]==new Vector2(roomHandler.assignedRoom.xCoord,roomHandler.assignedRoom.yCoord)
		)//&& !memberTokens[selectedMember].moveTaken)
		{
			int bashStrength=1;
			int bashNoiseDistance=5;
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
			MakeNoise(roomHandler.GetRoomCoords(),bashNoiseDistance);
			TurnOver(selectedMember,roomHandler.assignedRoom,false);
		}
	}
	
	public void BarricadeBuildClicked(RoomButtonHandler roomHandler)
	{
		if (!roomHandler.assignedRoom.hasEnemies 
		    && memberCoords[selectedMember]==new Vector2(roomHandler.assignedRoom.xCoord,roomHandler.assignedRoom.yCoord)
		    )//&& !memberTokens[selectedMember].moveTaken)
		{
			AddNewLogMessage(selectedMember.name+" throws together a barricade");
			roomHandler.BarricadeRoom();
			TurnOver(selectedMember,roomHandler.assignedRoom,false);
		}
	}
	
	public void BarricadeBashClicked(RoomButtonHandler roomHandler)
	{
		if (!roomHandler.assignedRoom.hasEnemies 
		    && Mathf.Abs(memberCoords[selectedMember].x-roomHandler.assignedRoom.xCoord)
		    +Mathf.Abs(memberCoords[selectedMember].y-roomHandler.assignedRoom.yCoord)<=1
		    )//&& !memberTokens[selectedMember].moveTaken)
		{
			AddNewLogMessage(selectedMember.name+" smashes the barricade");
			roomHandler.BashBarricade(1);
			if (roomHandler.assignedRoom.barricadeInRoom==null) {AddNewLogMessage("The barricade is cleared!");}
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
	
	public void MakeNoise(Vector2 originCoords, int carryDistance)
	{
		//Create text
		if (encounterOngoing)
		{
			SendFloatingMessage("Noise",roomButtons[originCoords].transform);
			//Do effect
			//Dictionary<Vector2,int> moveMaskToSource=IterativeGrassfireMapper(originCoords);
			for (int i=-carryDistance; i<=carryDistance; i++)
			{
				for (int j=-carryDistance; j<=carryDistance; j++)
				{
					Vector2 cursorCoords=originCoords+new Vector2(j,i);
					if (roomButtons.ContainsKey(cursorCoords))
					{
						if (roomButtons[cursorCoords].assignedRoom.hasEnemies)
						{
							foreach (EncounterEnemy enemy in roomButtons[cursorCoords].assignedRoom.enemiesInRoom)
							{
								enemyTokens[enemy].AddNewPOI(originCoords,null);//moveMaskToSource);
							}		
						}
					}
				}
			}
		}
	}	
	
	void SelectMember(MemberTokenHandler selectedHandler) 
	{
		selectedHandler.Select();
		foreach (MemberTokenHandler handler in memberTokens.Values)//CombatSelectorHandler handler in selectors.Values)
		{
			if (handler!=selectedHandler) {handler.Deselect();}
		}
		selectedMember=selectedHandler.myMember;
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
	
	public void ShowDamageToPartyMember(PartyMember damagedMember, EncounterEnemy attackingEnemy, int damage, bool blocked)
	{
		if (memberTokens.ContainsKey(damagedMember))
		{
			string attackMessage="";
			if (blocked) attackMessage=damagedMember.name+" blocks "+attackingEnemy.name+", losing "+damage+" stamina";
			else attackMessage=attackingEnemy.name+" attacks "+damagedMember.name+" for "+damage+" damage";
			AddNewLogMessage(attackMessage);
			
			Color textColor=Color.red;
			if (blocked) textColor=Color.green;
			SendFloatingMessage(damage.ToString(),memberTokens[damagedMember].transform,textColor);
		}
	}
	
	IEnumerator VisualizeAttack(int dmg, IAttackAnimation attacker, MonoBehaviour defender)
	{
		return VisualizeAttack(dmg,false,attacker,defender);
	}
	
	IEnumerator VisualizeAttack(int dmg,bool blocked, IAttackAnimation attacker, MonoBehaviour defender)
	{
		FloatingTextHandler newHandler=Instantiate(floatingTextPrefab);
		newHandler.AssignNumber(dmg);
		//If attacking member
		if (defender.GetType()==typeof(MemberTokenHandler))
		{
			MemberTokenHandler defenderToken=defender as MemberTokenHandler;
			if (memberTokens.ContainsValue(defenderToken)) //selectors.ContainsKey(info.damagedMember)) 
			{
				if (blocked) newHandler.GetComponent<Text>().color=Color.green;
				else  newHandler.GetComponent<Text>().color=Color.red;
				newHandler.transform.SetParent(defenderToken.transform,false);
				newHandler.transform.position=defenderToken.transform.position;
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
			}
		}
		GetComponent<CanvasGroup>().interactable=false;
		yield return StartCoroutine(attacker.AttackAnimation());
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
	
	
	//Used by callbacks from member tokens and also by set off traps
	public void RegisterDamage(int damage, BodyPart attackedPart, bool isRanged,EncounterEnemy attackedEnemy, IAttackAnimation attackingEntity)//Object attackingEntity)//PartyMember attackingMember)
	{
		//EncounterRoom currentRoom=currentEncounter.encounterMap[memberCoords[selectedMember]];
		//int actualDmg=currentRoom.DamageEnemy(damage,attackedEnemy,isRanged);
		int actualDmg=roomButtons[attackedEnemy.GetCoords()].AttackEnemyInRoom(damage,attackedPart,attackedEnemy,isRanged);
		
		//bool blockInteraction=attackingEntity.GetType()==typeof(MemberTokenHandler);
		StartCoroutine(VisualizeAttack(actualDmg,attackingEntity,enemyTokens[attackedEnemy]));//VisualizeAttackOnEnemy(actualDmg,attackedEnemy, attackingEntity));
		bool attackerIsMember=false;
		MemberTokenHandler attackingMemberToken=null;
		if (attackingEntity.GetType()==typeof(MemberTokenHandler)) attackingMemberToken=attackingEntity as MemberTokenHandler;
		int attackSoundIntensity=2;
		if (attackingMemberToken!=null) 
		{
			MakeNoise(memberCoords[attackingMemberToken.myMember],attackSoundIntensity);
			string hitMessage=" hits ";
			if (isRanged) hitMessage=" shoots ";
			AddNewLogMessage(attackingMemberToken.myMember.name+hitMessage+attackedEnemy.name+" for "+damage+" damage");
			AddNewLogMessage(attackingMemberToken.myMember.name+" makes some noise!");
		}
		else AddNewLogMessage("a trap does "+damage+" damage to "+attackedEnemy.name);
		if (attackedEnemy.health<=0) 
		{
			AddNewLogMessage(attackedEnemy.name+" is killed!");
			if (attackingMemberToken!=null) 
			{
				attackingMemberToken.myMember.ReactToKill();
			}
			//EMoveEnemies-=attackedEnemy.Move;
			//EMemberMoved-=attackedEnemy.VisionUpdate;
			//EMoveEnemies-=enemyTokens[attackedEnemy].TokenRoundoverTrigger;//enemy.Move;
			EMemberMoved-=enemyTokens[attackedEnemy].UpdateTokenVision;//enemy.VisionUpdate;
			GameObject.Destroy(enemyTokens[attackedEnemy].gameObject);
			enemyTokens.Remove(attackedEnemy);
		}
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
	
	public void AttackOnEnemy(EncounterEnemy enemy, bool ranged, BodyPart attackedPart)
	{
		int actualDmg=0;
		if (ranged) actualDmg=selectedMember.RangedAttack();
		else actualDmg=selectedMember.MeleeAttack();
		RegisterDamage(actualDmg,attackedPart,ranged,enemy,memberTokens[selectedMember]);
		TurnOver(selectedMember,roomButtons[memberCoords[selectedMember]].assignedRoom,false);
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
			
			if (Input.GetKeyDown(KeyCode.Q)) 
			{
				List<PartyMember> cyclableMembers=new List<PartyMember>(encounterMembers);
				foreach (MemberTokenHandler token in memberTokens.Values)
				{
					if (token.turnTaken) {cyclableMembers.Remove(token.myMember);}
				}
				
				SelectMember(memberTokens[cyclableMembers[(int)Mathf.Repeat(cyclableMembers.IndexOf(selectedMember)+1,cyclableMembers.Count)]]);
				
				/*
				bool allActionsTaken=true;
				foreach (MemberTokenHandler token in memberTokens.Values)//CombatSelectorHandler selector in selectors.Values) 
				{
					if (!token.actionTaken) 
					{
						allActionsTaken=false; 
						if (token.myMember!=selectedMember) {SelectMember(token); break;}
					}
				}
				if (allActionsTaken) {throw new System.Exception("Toggling next member when all members have acted!");}*/
			}
			if (Input.GetKeyDown(KeyCode.E)) 
			{
				MakeNoise(memberCoords[selectedMember],2);
				AddNewLogMessage(selectedMember.name+" makes some noise");
			}
			
		}
	}
}
