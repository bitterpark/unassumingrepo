using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapManager : MonoBehaviour 
{
	public static MapManager main;
	
	public Transform regionsGridGroup;
	
	public MapRegion mapRegionPrefab;
	public GameObject playerTokenPrefab;
	public MemberMapToken memberTokenPrefab;
	
	GameObject playerToken;
	
	public int mapHeight=0;
	public int mapWidth=0;
	
	public Dictionary<PartyMember,MemberMapToken> memberTokens;
	public List<Horde> hordes;
	public Dictionary<Horde,HordeTokenDrawer> hordeTokens;
	public HordeTokenDrawer hordeTokenPrefab;
	
	public MapScoutingHandler scoutingHandler;
	public bool hordeLocked=false;
	//List<MapRegion> mapRegions=new List<MapRegion>();
	Dictionary<Vector2,MapRegion> mapRegions=new Dictionary<Vector2, MapRegion>();
	
	public void GenerateNewMap()
	{
		regionsGridGroup.GetComponent<GridLayoutGroup>().constraintCount=mapWidth;
		
		float xElementSize=80;
		float yElementSize=80;
		float xElementOffset=xElementSize+10;
		float yElementOffset=yElementSize+10;
		float baseOffsetX=0;//360;
		float baseOffsetY=GetComponent<RectTransform>().rect.height*0.5f;//380;
		
		for (int i=0; i<mapHeight; i++)
		{
			for (int j=0; j<mapWidth; j++)
			{
				Vector2 newPos=new Vector2(xElementOffset*j-baseOffsetX,-yElementOffset*i+baseOffsetY);
				mapRegions.Add (new Vector2(j,i),CreateNewRegion(newPos,j,i));
			}
		}
		//selectedMembers=new List<PartyMember>();
		memberTokens=new Dictionary<PartyMember,MemberMapToken>();
		//deprecate this later
		hordes=new List<Horde>();
		hordeTokens=new Dictionary<Horde, HordeTokenDrawer>();
		//PartyManager.TimePassed+=MoveAllHordes;
		//city generation step
		//Initial mask boundaries
		List<Vector2> doubleTownSpots=new List<Vector2>();
		List<Vector2> tripleTownSpots=new List<Vector2>();
		
		for (int i=0; i<mapHeight; i++)
		{
			for (int j=0; j<mapWidth; j++)
			{
				if (i<mapHeight-1 && j<mapWidth-1) {doubleTownSpots.Add (new Vector2(i,j));}
				if (i<mapHeight-2 && j<mapWidth-2) {tripleTownSpots.Add (new Vector2(i,j));}
			}
		}
		//Adding in towns
		int tripleTownDesiredCount=3;
		int tripleTownCount=0;
		//Add triples
		while (tripleTownCount!=tripleTownDesiredCount && tripleTownSpots.Count>0)
		{
			Vector2 newTownTopLeft=tripleTownSpots[Random.Range(0,tripleTownSpots.Count)];
			
			for (int i=0; i<3; i++)
			{
				for (int j=0; j<3; j++)
				{
					Vector2 addressVector=newTownTopLeft+new Vector2(i,j);
					mapRegions[addressVector].GenerateEncounter(false);
					tripleTownSpots.Remove(addressVector);
					doubleTownSpots.Remove(addressVector);
				}
			}
			tripleTownCount++;
			//update masks
			for (int i=-3; i<4; i++)
			{
				for (int j=-3; j<4; j++)
				{
					Vector2 addressVector=newTownTopLeft+new Vector2(i,j);
					tripleTownSpots.Remove(addressVector);
					if (i>-3 && j>-3) {doubleTownSpots.Remove(addressVector);}
				}
			}
		}
		int doubleTownDesiredCount=3;
		int doubleTownCount=0;
		//Add doubles
		while (doubleTownCount!=doubleTownDesiredCount && doubleTownSpots.Count>0)
		{
			Vector2 newTownTopLeft=doubleTownSpots[Random.Range(0,doubleTownSpots.Count)];
			
			for (int i=0; i<2; i++)
			{
				for (int j=0; j<2; j++)
				{
					Vector2 addressVector=newTownTopLeft+new Vector2(i,j);
					mapRegions[addressVector].GenerateEncounter(false);//hasEncounter=true;
					tripleTownSpots.Remove(addressVector);
					doubleTownSpots.Remove(addressVector);
				}
			}
			doubleTownCount++;
			//update doubletown mask
			for (int i=-2; i<3; i++)
			{
				for (int j=-2; j<3; j++)
				{
					Vector2 addressVector=newTownTopLeft+new Vector2(i,j);
					doubleTownSpots.Remove(addressVector);
				}
			}
		}
		//Add endgame encounter
		Vector2 radioStationAddress=new Vector2(Random.Range(0,mapHeight),Random.Range(0,mapWidth));
		mapRegions[radioStationAddress].GenerateEncounter(true);
		//mapRegions[radioStationAddress].regionalEncounter=new Encounter(true);
		
		//final step - party placement
		if (playerToken!=null) {GameObject.Destroy(playerToken);}
		playerToken=Instantiate(playerTokenPrefab) as GameObject;
		Canvas.ForceUpdateCanvases();
		GetComponentInChildren<ScrollRect>().normalizedPosition=new Vector2(0,1);
		//DiscoverRegions(0,0);
		//TeleportToRegion(mapRegions[new Vector2(0,0)]);
	}
	
	MapRegion CreateNewRegion(Vector2 newRegionPos, int xCoord, int yCoord)
	{
		MapRegion newRegion=Instantiate(mapRegionPrefab,newRegionPos,Quaternion.identity) as MapRegion;
		newRegion.xCoord=xCoord;
		newRegion.yCoord=yCoord;
		newRegion.transform.SetParent(regionsGridGroup);
		/*
		float rand=Random.value;
		if (rand<0.5) {newRegion.hasEncounter=true;}
		*/
		return newRegion;
	}
	
	public void ClearMap()
	{
		foreach(MapRegion region in mapRegions.Values) 
		{
			GameObject.Destroy(region.gameObject);
		}
		mapRegions.Clear();
		hordes.Clear();
		foreach (HordeTokenDrawer token in hordeTokens.Values) {GameObject.Destroy(token.gameObject);}
		hordeTokens.Clear();
		GameObject.Destroy(playerToken);
	}
	
	public void AddMemberToken(PartyMember member)
	{
		MemberMapToken newToken=Instantiate(memberTokenPrefab);
		//MapRegion memberRegion=GetRegion(member.worldCoords);
		//newToken.transform.SetParent(memberRegion.transform,false);
		//newToken.transform.position=memberRegion.transform.position;
		newToken.AssignPartyMember(member);
		memberTokens.Add(member,newToken);
	}
	
	public void RemoveMemberToken(PartyMember member)
	{
		if (memberTokens.ContainsKey(member))
		{
			GameObject.Destroy(memberTokens[member].gameObject);
			memberTokens.Remove(member);
		}
		else throw new System.Exception("Attempting to delete nonexistent map member token!");
	}
	
	
	public MapRegion GetRegion(int coordX,int coordY) {return GetRegion(new Vector2(coordX,coordY));}
	public MapRegion GetRegion(Vector2 coords) {return mapRegions[coords];}
	
	public void RegionClicked(MapRegion clickedRegion)
	{	
		if (PartyManager.mainPartyManager.selectedMembers.Count>0)
		{
			if (!EncounterCanvasHandler.main.encounterOngoing 
			    && mapRegions[PartyManager.mainPartyManager.selectedMembers[0].worldCoords].hordeEncounter==null)
			{
				//See which of the selected party members are capable of moving
				/*
				List<PartyMember> movedMembers=new List<PartyMember>();
				foreach (PartyMember member in PartyManager.mainPartyManager.selectedMembers)
				{
					if (!memberTokens[member].moved) {movedMembers.Add(member); print ("Member added to movedMembers");}
				}
				//If none of the selected members are capable of moving, quit
				if (movedMembers.Count==0) return;*/
				List<PartyMember> movedMembers;
				if (PartyManager.mainPartyManager.ConfirmMapMovement(clickedRegion.xCoord,clickedRegion.yCoord, out movedMembers))//.MovePartyToMapCoords(clickedRegion.xCoord,clickedRegion.yCoord))
				{
					bool noEventBasedMove=true;
					bool eventHappened=GameEventManager.mainEventManager.RollEvents(ref noEventBasedMove,clickedRegion,movedMembers);
					if (noEventBasedMove)
					{
						//MovePartyToRegion(clickedRegion);
						//foreach (PartyMember member in movedMembers)
						//{
							MoveMembersToRegion(clickedRegion,movedMembers.ToArray());
						//}
						DiscoverRegions(clickedRegion.GetCoords());
						//Threat level roll
						
						//Searches if a scout is included in the party, if so - no ambushes trigger
						bool membersCanBeAmbushed=true;
						foreach (PartyMember member in movedMembers)
						{
							if (member.isScout) {membersCanBeAmbushed=false; break;}
						}
						
						if (clickedRegion.hasEncounter && !eventHappened && membersCanBeAmbushed)
						{
							float randomAttackChance=0;
							switch(clickedRegion.threatLevel)
							{
							case MapRegion.ThreatLevels.Low: {randomAttackChance=0.1f; break;}
							case MapRegion.ThreatLevels.Medium: {randomAttackChance=0.25f; break;}
							case MapRegion.ThreatLevels.High: {randomAttackChance=0.4f; break;}
							}
							if (Random.value<randomAttackChance) 
							GameEventManager.mainEventManager.DoEvent(new AmbushEvent(),clickedRegion,movedMembers);
							//EnterEncounter(new RandomAttack(clickedRegion.regionalEncounter.encounterEnemyType),movedMembers,true);
						}//*/
					}
				}
			}
		}	
	}
	/*
	//for special events
	public void TeleportToRegion(MapRegion teleportRegion)
	{
		MovePartyToRegion(teleportRegion);
	}
	//for regular movement
	void MovePartyToRegion(MapRegion newRegion)
	{
		PartyManager.mainPartyManager.MovePartyToMapCoords(newRegion.xCoord,newRegion.yCoord);
		DiscoverRegions(newRegion.xCoord,newRegion.yCoord);
		UpdatePartyToken(newRegion);
		scoutingHandler.EndDialog();
		//CheckPartyRegionForHordes(newRegion);
	}
	
	void UpdatePartyToken(MapRegion newRegion)
	{
		Vector3 newTokenPos=newRegion.transform.position;
		newTokenPos.z=playerToken.transform.position.z;
		playerToken.transform.position=newTokenPos;
		playerToken.transform.SetParent(newRegion.transform);
	}*/
	
	public void MoveMembersToRegion(Vector2 newRegionCoords, params PartyMember[] movedMembers)
	{
		MoveMembersToRegion(GetRegion(newRegionCoords),movedMembers);
	}
	
	public void MoveMembersToRegion(MapRegion newRegion, params PartyMember[] movedMembers)
	{
		foreach (PartyMember movedMember in movedMembers)
		{
			mapRegions[movedMember.worldCoords].localPartyMembers.Remove(movedMember);
			movedMember.worldCoords=newRegion.GetCoords();
			newRegion.localPartyMembers.Add(movedMember);
			memberTokens[movedMember].MoveToken(newRegion.transform);
		}
		DiscoverRegions(newRegion.GetCoords());
		PartyStatusCanvasHandler.main.RefreshAssignmentButtons(PartyManager.mainPartyManager.selectedMembers);
		
	}	
	
	void DiscoverRegions(Vector2 coords) {DiscoverRegions((int)coords.x,(int)coords.y);}
	
	void DiscoverRegions(int originXCoord, int originYCoord)
	{
		int partyVisionRange=2;
		//if (PartyManager.mainPartyManager.dayTime>9 && PartyManager.mainPartyManager.dayTime<21) {partyVisionRange=2;}
		foreach (MapRegion region in mapRegions.Values)
		{
			if (Mathf.Abs(originXCoord-region.xCoord)<=partyVisionRange 
			    && Mathf.Abs(originYCoord-region.yCoord)<=partyVisionRange) 
			{
			  	region.discovered=true;
			   	region.visible=true;
			}
			else {region.visible=false;}
		}
	}
	/*
	void CheckPartyRegionForHordes(MapRegion region)
	{
		if (region.hordeEncounter!=null && !EncounterCanvasHandler.main.encounterOngoing) 
		{
			//EnterEncounter(region.hordeEncounter,PartyManager.mainPartyManager.partyMembers);
			hordeLocked=true;
		}
	}*/

	public void EnterEncounter(Encounter newEncounter, List<PartyMember> team, bool isAmbush)
	{
		EncounterCanvasHandler.main.StartNewEncounter(newEncounter,team,isAmbush);
		scoutingHandler.EndDialog();
	}
	/*
	public void AddHorde(Vector2 hiveCoords,EncounterEnemy.EnemyTypes hordeType)
	{
		Vector2 hordeCoords=hiveCoords;
		//determine if horde will shift by 1 x or 1 y from hive
		if (mapRegions[hordeCoords].hordeEncounter==null)
		{
			HordeTokenDrawer newToken=Instantiate(hordeTokenPrefab);
			newToken.transform.position=mapRegions[hordeCoords].transform.position;
			Horde newHorde=new Horde(hordeType,(int)hiveCoords.x,(int)hiveCoords.y);
			hordes.Add(newHorde);
			hordeTokens.Add (newHorde,newToken);
			mapRegions[hordeCoords].hordeEncounter=newHorde;
			MoveHorde(newHorde);
		}
	}
	*/
	public void RemoveHorde(Horde removedHorde)
	{
		/*
		Vector2 removedHordeCoords=new Vector2(removedHorde.mapX,removedHorde.mapY);
		mapRegions[removedHordeCoords].hordeEncounter=null;
		hordes.Remove(removedHorde);
		GameObject.Destroy(hordeTokens[removedHorde].gameObject);
		hordeTokens.Remove(removedHorde);*/
	}
	/*
	public void MoveAllHordes(int timeVar)
	{
		//!!IMPORTANT!! - Dispose of expired hordes first!
		List<Horde> bufferList=new List<Horde>();
		bufferList.AddRange(hordes.ToArray());
		foreach (Horde horde in bufferList) {horde.ReduceLifespan();}
		foreach (Horde horde in hordes)
		{
			MoveHorde(horde);
		}
		//CheckPartyRegionForHordes(mapRegions[new Vector2(PartyManager.mainPartyManager.mapCoordX,PartyManager.mainPartyManager.mapCoordY)]);
	}
	
	void MoveHorde(Horde horde)
	{
		Vector2 oldHordeCoords=new Vector2(horde.mapX,horde.mapY);
		
		//prevent party and horde phasing through eachother
		if (oldHordeCoords.x!=PartyManager.mainPartyManager.mapCoordX | oldHordeCoords.y!=PartyManager.mainPartyManager.mapCoordY)
		{
			Vector2 newHordeCoords=new Vector2();
			if (FindHordeMoveLoc(horde,out newHordeCoords))
			{
				horde.mapX=(int)newHordeCoords.x;
				horde.mapY=(int)newHordeCoords.y;
				hordeTokens[horde].MoveToDestination(mapRegions[oldHordeCoords],mapRegions[newHordeCoords]);
				mapRegions[oldHordeCoords].hordeEncounter=null;
				mapRegions[newHordeCoords].hordeEncounter=horde;
			}
		}
	}
	//Only used in MoveHorde
	bool FindHordeMoveLoc(Horde horde, out Vector2 moveLoc)
	{
		Vector2 oldHordeCoords=new Vector2(horde.mapX,horde.mapY);
		List<Vector2> moveLocs=new List<Vector2>();
		for (int i=-1; i<=1; i++)
		{
			for (int j=-1; j<=1; j++)
			{
				Vector2 locCoords=oldHordeCoords+new Vector2(j,i);
				if (mapRegions.ContainsKey(locCoords))
				{
					if (locCoords!=oldHordeCoords & mapRegions[locCoords].hordeEncounter==null)
					{
						moveLocs.Add(locCoords);
					}
				}
			}
		}
		moveLoc=Vector2.zero;
		bool movePossible=false;
		if (moveLocs.Count>0)
		{
			movePossible=true;
			moveLoc=moveLocs[Random.Range(0,moveLocs.Count)];
		}
		return movePossible;
	}*/
	
	void Start()
	{
		main=this;
		//GameManager.MapManagerStartDelegate+=GenerateNewMap;
		//GameManager.GameStart+=GenerateNewMap;
		GameManager.GameOver+=ClearMap;
	}
	/*
	void OnGUI()
	{
		if (GameManager.main.gameStarted && !EncounterCanvasHandler.main.encounterOngoing && PartyManager.mainPartyManager.selectedMembers.Count>0)
		{
			if (!InventoryScreenHandler.mainISHandler.inventoryShown && !GameEventManager.mainEventManager.drawingEvent)
			{
				MapRegion checkedRegion=mapRegions[PartyManager.mainPartyManager.selectedMembers[0].worldCoords];
				Rect encounterStartRect=new Rect(110,10,80,25);
				
				List<PartyMember> membersFreeToAct=new List<PartyMember>();
				
				foreach (PartyMember member in PartyManager.mainPartyManager.selectedMembers)
				{
					if (!memberTokens[member].moved)
					{
						membersFreeToAct.Add(member);
					}
				}
				//{
				Rect restButtonRect=new Rect(110,35,140,25);
				if (membersFreeToAct.Count>0)
				{
					if (checkedRegion.hasEncounter)
					{
						if (GUI.Button (encounterStartRect,"Explore")) 
						{
							if (scoutingHandler.GetComponent<Canvas>().enabled) {scoutingHandler.EndDialog();}
							else
							{	
								scoutingHandler.StartDialog(checkedRegion);
							}
						}
					}
					
					
					if (checkedRegion.hasCamp)
					{
						if (GUI.Button(restButtonRect,"Rest")) 
						{
							foreach (PartyMember member in membersFreeToAct)
							{
								PartyManager.mainPartyManager.Rest(member);
							}
						
						}
					}
					else
					{
						if (GUI.Button(restButtonRect,"Setup camp("+checkedRegion.campSetupTimeRemaining+" hours)")) 
						{
							int totalInvestedHours=0;
							//PartyManager.mainPartyManager.PassTime(campSetupTime);
							foreach (PartyMember member in membersFreeToAct)
							{
								//token.moved=true;
								//totalInvestedHours+=1;
								//if (totalInvestedHours==checkedRegion.campSetupTimeRemaining) break;
								AssignedTask campBuildingTask=new AssignedTask(member,AssignedTaskTypes.BuildCamp
								,()=>
								{
									if (!checkedRegion.hasCamp) return true;
									else return false;
								}
								,()=>
								{
									checkedRegion.SetUpCamp(1);
								}
								);
								PartyManager.mainPartyManager.AssignMemberNewTask(campBuildingTask);
							}
							//checkedRegion.SetUpCamp(totalInvestedHours);
						}
					}
				}
				else
				{
					if (checkedRegion.hasCamp)
					{
						List<PartyMember> buildingMembers=new List<PartyMember>();
						foreach (PartyMember member in PartyManager.mainPartyManager.selectedMembers)
						{
							AssignedTaskTypes memberTaskType;
							if (PartyManager.mainPartyManager.GetAssignedTask(member,out memberTaskType))
							{
								if (memberTaskType==AssignedTaskTypes.Rest) 
									buildingMembers.Add(member);
							}
						}
						if (buildingMembers.Count>0)
						{
							if (GUI.Button(restButtonRect,"Stop resting")) 
							{
								foreach (PartyMember member in buildingMembers)
								{
									PartyManager.mainPartyManager.RemoveMemberTask(member);//.assignedTasks.Remove(token.assignedMember);
									//token.moved=false;
								}
								
							}
						}
					}
					else
					{
						List<PartyMember> buildingMembers=new List<PartyMember>();
						foreach (PartyMember member in PartyManager.mainPartyManager.selectedMembers)
						{
							AssignedTaskTypes memberTaskType;
							if (PartyManager.mainPartyManager.GetAssignedTask(member,out memberTaskType))
							{
								if (memberTaskType==AssignedTaskTypes.BuildCamp) 
								buildingMembers.Add(member);
							}
						}
						if (buildingMembers.Count>0)
						{
							if (GUI.Button(restButtonRect,"Stop building camp")) 
							{
								foreach (PartyMember member in buildingMembers)
								{
									PartyManager.mainPartyManager.RemoveMemberTask(member);//.assignedTasks.Remove(token.assignedMember);
									//token.moved=false;
								}
								
							}
						}
					}
				}
			}
		}
	}*/
}
