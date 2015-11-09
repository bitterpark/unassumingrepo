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
	
	public PartyMember selectedMember=null;
	//public EncounterEnemy selectedEnemy=null;
	public List<PartyMember> encounterMembers=new List<PartyMember>();
	Dictionary<PartyMember,Dictionary<Vector2,int>> memberMoveMasks;
	//public EncounterRoom displayedRoom;
	public Dictionary<PartyMember,Vector2> memberCoords=new Dictionary<PartyMember, Vector2>();
	
	string lastMessage="";
	int messageCount=0;
	bool damageNumberOut=false;
	
	//public GameObject memberToken;
	//GUI HANDLER ELEMENTS
	//PREFABS
	//public StatusEffectImageHandler enemyEffectPrefab;
	public RoomButtonHandler roomPrefab;
	//public CombatSelectorHandler selectorPrefab;
	//public EnemySelectorHandler enemySelectorPrefab;
	public DamageNumberHandler damageNumberPrefab;	
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
	//public Transform combatSelectorGroup;
	//public Transform enemySelectorGroup;
	//public Transform enemyEffectGroup;		
	
	//statics
	public static int encounterMaxPlayerCount=20;
	public static EncounterCanvasHandler main;
	
	public delegate void EnemiesMoveDel(Dictionary<PartyMember, Dictionary<Vector2,int>> masks, Dictionary<PartyMember,Vector2> memberCoords);//(Dictionary<Vector2, int> costs);
	public static event EnemiesMoveDel EMoveEnemies;
	delegate void MemberMovedDel ();
	event MemberMovedDel EMemberMoved;
	
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
	
	public void StartNewEncounter(Encounter newEncounter, List<PartyMember> membersOnMission)
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
		EnableRender();
	}
	
	void EndEncounter()
	{
		DisableRender();
		encounterOngoing=false;
	}
	
	public void RoomClicked(RoomButtonHandler roomHandler)//EncounterRoomDrawer roomDrawer)
	{
		//roomButtons[new Vector2(room.xCoord,room.yCoord)].GetMyLocalPos();
		int moveDistance=0;	
		EncounterRoom startingRoom=currentEncounter.encounterMap[memberCoords[selectedMember]];
		moveDistance=Mathf.Abs((int)startingRoom.xCoord-roomHandler.roomX)+Mathf.Abs((int)startingRoom.yCoord-roomHandler.roomY);//encounterPlayerX-room.xCoord)+Mathf.Abs(encounterPlayerY-room.yCoord);
		
		if (moveDistance==1 && !roomHandler.assignedRoom.isWall) //&& !currentEncounter.encounterMap[memberCoords[selectedMember]].hasEnemies)//new Vector2(encounterPlayerX,encounterPlayerY)].hasEnemies)
		{
			//If the room has no other party members move there regularly, else - do the swap
			if (!memberCoords.ContainsValue(roomHandler.GetRoomCoords()))
			{
				//Enemies get a free swipe if you move out of their spot
				List<PartyMember> argumentListOfOne=new List<PartyMember>();
				argumentListOfOne.Add(selectedMember);
				foreach (EncounterEnemy enemy in startingRoom.enemiesInRoom) {enemy.RoundAction(argumentListOfOne);}//attack equivalentenemy.Move();}
				MovePartyMemberToRoom(selectedMember,roomHandler,true);
			}
			else
			{
				PartyMember memberAtDestination=null;
				foreach (PartyMember key in memberCoords.Keys)
				{
					if (memberCoords[key]==roomHandler.GetRoomCoords()) 
					{
						memberAtDestination=key;
						break;
					}
				}
				if (memberAtDestination==null) {throw new System.Exception("Could not find member at destination room!");}
				if (!memberTokens[memberAtDestination].actionTaken) {SwapMemberPlaces(selectedMember,memberAtDestination);}
			}
		}//
	}
	
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
		while (cursorRoom.GetComponentInChildren<MemberTokenHandler>()!=null)
		{
			roomsSortedByDistance.Remove(new Vector2(cursorRoom.assignedRoom.xCoord,cursorRoom.assignedRoom.yCoord));
			cursorRoom=roomButtons[roomsSortedByDistance[0]];
			//check all rooms adjecent to exit
		}
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
		UpdateMemberMoveMask(member);
		RevealPartyVision();
		if (EMemberMoved!=null) EMemberMoved();
		if (doTurnover) TurnOver(member,roomButton.assignedRoom);
		//RefreshRoom(room);
		//CheckRoomForEnemies(room);
		//check for combat start
	}
	//called after member move or initial member placement - !!! CONSIDER MOVING THIS TO MOvePartyMemberToRoom !!!
	void UpdateMemberMoveMask(PartyMember member)
	{
		memberMoveMasks[member].Clear();
		Dictionary<Vector2,int> moveMask=memberMoveMasks[member];
		memberMoveMasks[member]=IterativeGrassfireMapper(memberCoords[member]);
	}
	
	//reveals stuff around current party room
	void RevealPartyVision()
	{
		//First - unreveal everything
		foreach (RoomButtonHandler cycledRoom in roomButtons.Values) {cycledRoom.SetVisibility(false);}//EncounterRoom cycledRoom in currentEncounter.encounterMap.Values) {cycledRoom.isVisible=false;}
		
		//Iterate through every present player
		foreach (PartyMember member in encounterMembers)
		{
			//Default daytime range
			int visionRange=4;
			if (PartyManager.mainPartyManager.dayTime<6 | PartyManager.mainPartyManager.dayTime>18) 
			{
				//Nighttime range
				visionRange=1;
				//Nighttime range with flashlight
				if (member.hasLight) {visionRange+=2;}
			}
			//////
			
		
			//Do los-based reveal
			for (int i=-visionRange; i<=visionRange; i++)
			{
				for (int j=-visionRange; j<=visionRange; j++)
				{
					if (Mathf.Abs(i)+Mathf.Abs(j)==visionRange)
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
		}
	}
	
	void TurnOver(PartyMember finishedMember,EncounterRoom room)
	{
		//if member hasn't left the encounter, update the selector
		if (encounterMembers.Contains(finishedMember)) {memberTokens[finishedMember].actionTaken=true;}//selectors[finishedMember].actionTaken=true;}
		//EncounterRoom currentRoom=currentEncounter.encounterMap[memberCoords[encounterMembers[0]]];//new Vector2(encounterPlayerX,encounterPlayerY)];
		if (room.hasEnemies)
		{
			foreach (EncounterEnemy enemy in room.enemiesInRoom) {enemy.TurnAction();}
			DeadMemberCleanupCheck();
		}
		AdvanceToNextMember();
	}
	
	void RoundOver()
	{
		EnemiesMove();
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
			//Stop encounter if all encounter members died
			if (encounterMembers.Count==0) {EndEncounter();}
		}
	}
	
	List<Vector2> CreateGrassfireMaskForEntrance(Vector2 entranceCoords)
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
	
	//For EnemiesMove only (currently updates on member move), based on djikstra maps (roguebasin)
	
	Dictionary<Vector2, int> IterativeGrassfireMapper(Vector2 goal)
	{
		Dictionary<Vector2, EncounterRoom> map=currentEncounter.encounterMap;
		if (!map.ContainsKey(goal)) {throw new System.Exception("Goal set for grassfire mapper doesn't exist in the map!");}
		Dictionary<Vector2,int> costs=new Dictionary<Vector2, int>();
		//iterationCount=0;
		costs.Add(goal,0);
		bool changesMade=false;
		//Vector2 currentRoomCoords=Vector2.zero;
		
		System.Action<Vector2> neighbourCheck=(Vector2 currentRoomCoords)=>
		{
			//iterationCount++;
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
				if (costs[currentRoomCoords]>minNeighbourValue+1) 
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
		return costs;
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
	
	//used for Wait button callback, loot button, player move and other things
	public void EnemiesMove()
	{		
		// enemy move block	
		//////SETUP GRASSFIRE MOVE MASKS
		/*
		Dictionary<PartyMember,Dictionary<Vector2,int>> memberMoveMasks=new Dictionary<PartyMember, Dictionary<Vector2, int>>();
		
		foreach (PartyMember member in encounterMembers)
		{
			Dictionary<Vector2,int> newMemberMask=new Dictionary<Vector2, int>();
			RecursiveGrassfireMapper(memberCoords[member],ref newMemberMask,0);
			memberMoveMasks.Add(member,newMemberMask);
		}*/
		
		///////END OF SETUP
		if (EMoveEnemies!=null) EMoveEnemies(memberMoveMasks,memberCoords);
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
						EMoveEnemies+=newEnemyToken.DoTokenMove;//catchEnemy.Move;
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
			
			memberMoveMasks.Add(member,new Dictionary<Vector2, int>());
			
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
		List<Vector2> roomsSortedByDistanceFromEntrance=CreateGrassfireMaskForEntrance(new Vector2(entranceRoom.xCoord,entranceRoom.yCoord));
		foreach (PartyMember member in encounterMembers)
		{
			//MovePartyMemberToRoom(member,entranceRoom);
			PlacePartyMemberAtEntrance(member,entranceRoom,roomsSortedByDistanceFromEntrance);
		}
		SelectMember(memberTokens[encounterMembers[0]]);
		StartCoroutine(EncounterStartViewFocus(roomButtons[memberCoords[encounterMembers[0]]].GetComponent<RectTransform>()));
		
		//MovePartyToRoom(entranceRoom);
	}
	
	void DisableRender() 
	{
		GetComponent<Canvas>().enabled=false;
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
			EMoveEnemies-=tokenHandler.DoTokenMove;
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
	
	void RemoveEncounterMember(PartyMember member)
	{
		encounterMembers.Remove(member);
		memberCoords.Remove(member);
		//GameObject.Destroy(selectors[member].gameObject);
		//selectors.Remove(member);
		GameObject.Destroy(memberTokens[member].gameObject);
		memberTokens.Remove(member);
	}

	public void ExitClicked(RoomButtonHandler roomHandler)
	{
		if (!roomHandler.assignedRoom.hasEnemies && memberCoords[selectedMember]==new Vector2(roomHandler.assignedRoom.xCoord,roomHandler.assignedRoom.yCoord))
		{
			EncounterRoom exitRoom=roomHandler.assignedRoom;//currentEncounter.encounterMap[memberCoords[selectedMember]];
			RemoveEncounterMember(selectedMember);
			//Dumps member's inventory to common inventory
			List<InventoryItem> membersItems=new List<InventoryItem>();
			membersItems.AddRange(selectedMember.carriedItems.ToArray());
			foreach (InventoryItem carriedItem in membersItems) 
			{
				selectedMember.carriedItems.Remove(carriedItem);
				PartyManager.mainPartyManager.GainItems(carriedItem);
			}
			
			if (encounterMembers.Count>0) 
			{
				TurnOver(selectedMember,exitRoom);
			}
			else
			{
				PartyManager.mainPartyManager.PassTime(1);
				EndEncounter();
				MapManager.mainMapManager.hordeLocked=false;
			}
		}
	}
	
	public void LootClicked(RoomButtonHandler roomHandler)
	{
		if (!roomHandler.assignedRoom.hasEnemies && memberCoords[selectedMember]==new Vector2(roomHandler.assignedRoom.xCoord,roomHandler.assignedRoom.yCoord))
		{
			roomHandler.LootRoom();
			TurnOver(selectedMember,roomHandler.assignedRoom);
		}
	}
	
	public void BashClicked(RoomButtonHandler roomHandler)
	{
		if (!roomHandler.assignedRoom.hasEnemies && memberCoords[selectedMember]==new Vector2(roomHandler.assignedRoom.xCoord,roomHandler.assignedRoom.yCoord))
		{
			int bashStrength=1;
			foreach (PartyMember member in encounterMembers)
			{
				if (member.isLockExpert) {bashStrength=3; break;}
			}
			roomHandler.BashLock(bashStrength);
			TurnOver(selectedMember,roomHandler.assignedRoom);
		}
	}
	
	
	//select active member for combat (for button press and also private turnover reset)
	
	public void ClickMember(MemberTokenHandler selectedHandler)
	{
		if (selectedMember!=selectedHandler.myMember)
		{
			SelectMember(selectedHandler);
		} 
		else TurnOver(selectedMember,roomButtons[memberCoords[selectedMember]].assignedRoom);
	}
	
	void SelectMember(MemberTokenHandler selectedHandler) 
	{
		selectedHandler.Select();
		foreach (MemberTokenHandler handler in memberTokens.Values)//CombatSelectorHandler handler in selectors.Values)
		{
			if (handler!=selectedHandler) {handler.Deselect();}
		}
		selectedMember=selectedHandler.myMember;
	}
	
	//against enemies
	void StartDamageNumber(int dmg, EncounterEnemy attackedEnemy, PartyMember attackingMember)
	{
		DamageNumberHandler newHandler=Instantiate(damageNumberPrefab);
		newHandler.AssignNumber(dmg);
		
		if (enemyTokens.ContainsKey(attackedEnemy))
		{
			newHandler.GetComponent<Text>().color=Color.magenta;
			newHandler.transform.SetParent(enemyTokens[attackedEnemy].transform,false);
			memberTokens[attackingMember].AnimateAttack();
		}
	}
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
	//Order of PartyMember and EncounterEnemy parameters is important!!!
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
			enemyTokens[info.attackingEnemy].AnimateAttack();
			damageNumberOut=true;
			yield return new WaitForSeconds(0.5f);
			damageNumberOut=false;
		}
		yield break;
	}
	
	void RegisterDamage(int damage, string message, bool isRanged,EncounterEnemy attackedEnemy, PartyMember attackingMember)
	{
		//EncounterRoom currentRoom=currentEncounter.encounterMap[memberCoords[selectedMember]];
		//int actualDmg=currentRoom.DamageEnemy(damage,attackedEnemy,isRanged);
		int actualDmg=roomButtons[memberCoords[selectedMember]].AttackEnemyInRoom(damage,attackedEnemy,isRanged);
		
		StartDamageNumber(actualDmg,attackedEnemy, attackingMember);
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
		TurnOver(selectedMember,currentEncounter.encounterMap[memberCoords[selectedMember]]);
	}
	
	//For now only works for melee
	public void EnemyPressed(EncounterEnemy enemy)
	{
		//int distance=Mathf.Max(Mathf.Abs(memberCoords[selectedMember].x-enemy.GetCoords().x)
		//,Mathf.Abs(memberCoords[selectedMember].y-enemy.GetCoords().y));
		if (memberCoords[selectedMember]==enemy.GetCoords())//distance==0)
		{
			int actualDmg=0;
			//Member token does the check to see if ranged attack is possible
			bool ranged=memberTokens[selectedMember].rangedMode;
			if (ranged) {actualDmg=selectedMember.RangedAttack();}
			else actualDmg=selectedMember.MeleeAttack();
			string msg="";//selectedMember.name+" hits the "+enemy.name+" for "+actualDmg+"!";
			RegisterDamage(actualDmg,msg,ranged,enemy,selectedMember);
		}
	}

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
	}
	
	void Start() 
	{
		main=this;
		GameManager.GameOver+=EndEncounter;
	}
}
