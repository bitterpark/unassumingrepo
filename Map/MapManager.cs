using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapManager : MonoBehaviour 
{
	public static MapManager main;
	
	public Transform regionsGridGroup;
	
	public MapRegion mapRegionPrefab;
	//public GameObject playerTokenPrefab;
	public MemberMapToken memberTokenPrefab;
	
	//GameObject playerToken;
	
	public int mapHeight=0;
	public int mapWidth=0;
	
	public Dictionary<PartyMember,MemberMapToken> memberTokens;
	public HordeTokenDrawer hordeTokenPrefab;
	
	public MapScoutingHandler scoutingHandler;
	public bool hordeLocked=false;
	//List<MapRegion> mapRegions=new List<MapRegion>();
	//Dictionary<Vector2,MapRegion> mapRegions=new Dictionary<Vector2, MapRegion>();
	public List<MapRegion> mapRegions=new List<MapRegion>();
	
	public const int townNodeFatigueCost=25;
	public const int townToTownGasCost=1;
	
	//THIS METHOD CAUSES THE DISPLACEMENT BUG
	public void FocusViewOnRegion(RectTransform regionTransform)
	{
		Vector2 newPosition=regionTransform.rect.center+(Vector2)regionTransform.localPosition;
		//print ("Moving to local position:"+regionTransform.localPosition);
		Vector2 scrollrectFocus=Vector2.zero;
		scrollrectFocus.x=(newPosition.x/regionsGridGroup.GetComponent<RectTransform>().rect.width);
		//!!!Y NEEDS TO BE CORRECTLY INVERTED!!! - in scrollrects, y=0 represents bottom, which is inverse to the way the rest of this setup works
		scrollrectFocus.y=(regionsGridGroup.GetComponent<RectTransform>().rect.height-Mathf.Abs(newPosition.y))
		/regionsGridGroup.GetComponent<RectTransform>().rect.height;
		//scrollrectFocus=new Vector2(-5.0f,-5.0f);
		//newPosition.x/=regionsGridGroup.GetComponent<RectTransform>().rect.width;
		//newPosition.y/=regionsGridGroup.GetComponent<RectTransform>().rect.height;//
		//print ("Scrollrect focus x is:"+scrollrectFocus.x+ToString());
		/*
		float testMod=0.5f;
		print ("Zooming to x:"+(testMod*regionsGridGroup.GetComponent<RectTransform>().rect.width)
		+", y:"+(testMod*regionsGridGroup.GetComponent<RectTransform>().rect.height));//*/
		regionsGridGroup.parent.GetComponent<ScrollRect>().normalizedPosition=scrollrectFocus;
		/*
		CreateRegion(new Vector2
		(testMod*regionsGridGroup.GetComponent<RectTransform>().rect.width
		,-(regionsGridGroup.GetComponent<RectTransform>().rect.height-(testMod*regionsGridGroup.GetComponent<RectTransform>().rect.height))));
		Canvas.ForceUpdateCanvases();//*/
	}
	
	IEnumerator MapGenViewFocus(RectTransform regionTransform)
	{
		yield return new WaitForEndOfFrame();
		FocusViewOnRegion(regionTransform);
		yield break;
	}
	
	public void GenerateNewMap()
	{
		memberTokens=new Dictionary<PartyMember,MemberMapToken>();
		
		//Town-related boundaries
		float townAreaSpriteSize=75f;
		float townNodeGap=5f;
		float townNodeWiggleRoom=100f;
		float townNodeSideSize=townAreaSpriteSize+townNodeGap+townNodeWiggleRoom;
		
		int townNodeSlots=3;
		
		float largeTownSideSize=(townNodeSideSize+townNodeGap)*townNodeSlots;//375f;
		//Vector2 smallTownSize=new Vector2(smallTownRadius,smallTownRadius);
		Vector2 largeTownSize=new Vector2(largeTownSideSize,largeTownSideSize);
		
		//Map border boundaries
		int horTownSlots=5;
		int vertTownSlots=3;
		
		float borderOffset=20f;
		
		//float townMinX=40f;
		//float townMinY=40f;
		
		
		float townSlotWiggleRoom=200f;
		float townSlotSideSize=largeTownSideSize+townSlotWiggleRoom;
		
		float gapBetweenTownSlots=10f;
		float mapWidth=borderOffset*2+(townSlotSideSize+gapBetweenTownSlots)*horTownSlots;//2000f;
		float mapHeight=borderOffset*2+(townSlotSideSize+gapBetweenTownSlots)*vertTownSlots;//1200f;
		//regionsGridGroup.GetComponent<RectTransform>().sizeDelta=new Vector2(mapWidth,mapHeight);
		
		
		
		//int horTownSlots=Mathf.FloorToInt(mapWidth/(townSlotSideSize+gapBetweenTownSlots));
		//int vertTownSlots=Mathf.FloorToInt(mapHeight/(townSlotSideSize+gapBetweenTownSlots));
		//print ("Horslots:"+horTownSlots+" Vertslots:"+vertTownSlots);
		
		
		List<Vector2> townSlotCenters=new List<Vector2>();
		for (int i=0; i<vertTownSlots; i++)
		{
			for (int j=0; j<horTownSlots; j++)
			{
				//Y coord must be inverted because in UI elements higher Y goes up and lower Y goes down
				Vector2 newTownUpperleftPos
				=new Vector2(borderOffset+(townSlotSideSize+gapBetweenTownSlots)*j,-(borderOffset+(townSlotSideSize+gapBetweenTownSlots)*i));
				townSlotCenters.Add(newTownUpperleftPos+new Vector2(townSlotSideSize*0.5f,-townSlotSideSize*0.5f));//new Vector2(townSlotSideSize*j,townSlotSideSize*i));
				//print ("Slot:"+j+"|"+i+":"+newTownUpperleftPos);
			}
		}
		
		
		int largeTownCount=3;
		
		System.Func<float,Vector2,Vector2> slotOffsetter=(float sidePaddingMaxOffset,Vector2 slotCoords)=>
		{
			Vector2 randomSlotOffset=Random.insideUnitCircle*(Random.Range(1f,sidePaddingMaxOffset));
			Vector2 newTownCenterCoords=slotCoords+randomSlotOffset;
			return newTownCenterCoords;
		};
		
		//Randomly select town slots to use, and offset them slightly off center, also
		//Find slot coord offset, to remove empty space from scrollrect, while keeping all used slots in the same places relative to eachother
		float mapMinX=Mathf.Infinity;
		float mapMinY=mapMinX;
		float mapMaxX=Mathf.NegativeInfinity;
		float mapMaxY=mapMaxX;
		
		List<Vector2> townCenters=new List<Vector2>();
		Vector2 endgameCenter=Vector2.zero;
		for (int i=0; i<=largeTownCount; i++)
		{
			//Vector2 randomPointInSlot=Random.insideUnitCircle*(Random.Range(1f,townSlotSideSize*0.5f-largeTownSideSize*0.5f));
			//int randomSlotIndex=Random.Range(0,townSlotCenters.Count);
			//Vector2 newTownCenterCoords=townSlotCenters[randomSlotIndex]+randomPointInSlot;
			//townSlotCenters.RemoveAt(randomSlotIndex);
			Vector2 randomSlot=townSlotCenters[Random.Range(0,townSlotCenters.Count)];
			Vector2 newTownCenterCoords=slotOffsetter.Invoke(Random.Range(1f,townSlotSideSize*0.5f-largeTownSideSize*0.5f),randomSlot);
			townSlotCenters.Remove(randomSlot);
			
			//Find min and max slot edge coords (Mind the rule that bigger Y is higher and lesser Y is lower)
			Vector2 adjustedSlotMinEdgeCoords=newTownCenterCoords-new Vector2(townSlotSideSize*0.5f,townSlotSideSize*0.5f);
			Vector2 adjustedSlotMaxEdgeCoords=newTownCenterCoords+new Vector2(townSlotSideSize*0.5f,townSlotSideSize*0.5f);
			mapMinX=Mathf.Min(adjustedSlotMinEdgeCoords.x,mapMinX);
			mapMinY=Mathf.Min(adjustedSlotMinEdgeCoords.y,mapMinY);
			mapMaxX=Mathf.Max(adjustedSlotMaxEdgeCoords.x,mapMaxX);
			mapMaxY=Mathf.Max(adjustedSlotMaxEdgeCoords.y,mapMaxY);
			//Add final slot to townCenters to be used for town placement
			if (i<largeTownCount) townCenters.Add(newTownCenterCoords);
			else endgameCenter=newTownCenterCoords; // - prepares endgame coordinates
			//print ("new town center coords:"+newTownRect.center);
		}
		
		//Offset all town slot coords to fit top used slot to scrollrect top, and leftmost used slot to scrollrect left, also
		//Find final map dimensions
		
		float townslotsXOffset=-(mapMinX-borderOffset);
		float townslotsYOffset=-borderOffset-mapMaxY;
		//Nullify min and max values again
		mapMinX=Mathf.Infinity;
		mapMinY=mapMinX;
		mapMaxX=Mathf.NegativeInfinity;
		mapMaxY=mapMaxX;
		
		for (int i=0; i<townCenters.Count; i++)//each (Vector2 townCenter in townCenters)
		{
			Vector2 adjustedTownCenter=new Vector2(townCenters[i].x+townslotsXOffset,townCenters[i].y+townslotsYOffset);
			townCenters[i]=adjustedTownCenter;
			Vector2 adjustedSlotMinEdgeCoords=adjustedTownCenter-new Vector2(townSlotSideSize*0.5f,townSlotSideSize*0.5f);
			Vector2 adjustedSlotMaxEdgeCoords=adjustedTownCenter+new Vector2(townSlotSideSize*0.5f,townSlotSideSize*0.5f);
			mapMinX=Mathf.Min(adjustedSlotMinEdgeCoords.x,mapMinX);
			mapMinY=Mathf.Min(adjustedSlotMinEdgeCoords.y,mapMinY);
			mapMaxX=Mathf.Max(adjustedSlotMaxEdgeCoords.x,mapMaxX);
			mapMaxY=Mathf.Max(adjustedSlotMaxEdgeCoords.y,mapMaxY);
		}
		endgameCenter+=new Vector2(townslotsXOffset,townslotsYOffset);
		//Include endgame region into estimation
		Vector2 adjustedEndgameSlotMinEdgeCoords=endgameCenter-new Vector2(townSlotSideSize*0.5f,townSlotSideSize*0.5f);
		Vector2 adjustedEndgameSlotMaxEdgeCoords=endgameCenter+new Vector2(townSlotSideSize*0.5f,townSlotSideSize*0.5f);
		mapMinX=Mathf.Min(adjustedEndgameSlotMinEdgeCoords.x,mapMinX);
		mapMinY=Mathf.Min(adjustedEndgameSlotMinEdgeCoords.y,mapMinY);
		mapMaxX=Mathf.Max(adjustedEndgameSlotMaxEdgeCoords.x,mapMaxX);
		mapMaxY=Mathf.Max(adjustedEndgameSlotMaxEdgeCoords.y,mapMaxY);
		
		//Determine final size of scrollgroup
		float mapFinalWidth=borderOffset*2+Mathf.Abs(mapMaxX-mapMinX);
		float mapFinalHeight=borderOffset*2+Mathf.Abs(mapMaxY-mapMinY);
		regionsGridGroup.GetComponent<RectTransform>().sizeDelta=new Vector2(mapFinalWidth,mapFinalHeight);
		
		//Create towns from rects
		
		List<MapRegion> newTowns=new List<MapRegion>();
		foreach (Vector2 townCenter in townCenters)
		{
			//Create new town
			MapRegion newTown=CreateRegion(townCenter);
			foreach(MapRegion region in newTowns) {newTown.AddConnectedRegion(region,true,townToTownGasCost);}
			//newRegion.SetConnectedRegions(new List<MapRegion>(mapRegions));
			newTowns.Add(newTown);	
		}
		
		//POPULATE TOWNS
		//Create node slot coords		
		List<Vector2> populatingOffsets=new List<Vector2>();
		float startOffset=-townNodeSideSize;
		for (int i=0; i<townNodeSlots; i++)
		{
			for (int j=0; j<townNodeSlots; j++)
			{
				if (!(j==1 && i==1)) populatingOffsets.Add(new Vector2(startOffset+townNodeSideSize*j,startOffset+townNodeSideSize*i));
			}
		}
		//Fill in nodes
		
		
		foreach(MapRegion town in newTowns)
		{
			int nodeCount=Random.Range(6,9);
			int loopCount=0;
			int gauntletCount=0;
			
			float randomRoll=Random.value;
			//See if town will have any special nodes at all
			if (randomRoll<=0.8f)
			{
				//See if it will have only loops, gauntlets, or both
				if (Random.value>0.66f) {loopCount=1; gauntletCount=1;}
				if (Random.value<=0.66f) {loopCount=2; gauntletCount=0;}
				if (Random.value<=0.33f) {loopCount=0; gauntletCount=2;}
			}
			
			List<Vector2> unusedOffsets=new List<Vector2>(populatingOffsets);
			//Create gauntlets and loops
			int specialNodesPerLoop=Random.Range(2,4);
			int specialNodesPerGauntlet=2;
			int nodesRemaining=nodeCount;
			
			//print ("Town gauntlets:"+gauntletCount+", town loops:"+loopCount);
			//print ("Nodes per:"+specialNodesPer);
			//print ("Total slots:"+unusedOffsets.Count);
			//Do gauntlets
			for (int i=0; i<gauntletCount;i++)
			{
				int startingSlotIndex=Random.Range(0,unusedOffsets.Count-specialNodesPerGauntlet);
				
				List<MapRegion> previousNodes=new List<MapRegion>();
				for (int j=0; j<specialNodesPerGauntlet; j++)
				{
					Vector2 usedSlot=unusedOffsets[startingSlotIndex+j];
					
					Vector2 newOffset=slotOffsetter.Invoke(Random.Range(1f,townNodeSideSize*0.5f-townAreaSpriteSize*0.5f),usedSlot);
					MapRegion newNode=CreateRegion((Vector2)town.transform.localPosition+newOffset);
					if (j>0) newNode.AddConnectedRegion(previousNodes[previousNodes.Count-1],false,townNodeFatigueCost);
					else newNode.AddConnectedRegion(town,false,townNodeFatigueCost);
					previousNodes.Add(newNode);
					nodesRemaining--;
				}
				//This is necesary to properly iterate and dispose indices
				unusedOffsets.RemoveRange(startingSlotIndex,specialNodesPerGauntlet);
			}
			//Do loops
			for (int i=0; i<loopCount;i++)
			{
				if (specialNodesPerLoop==3 && specialNodesPerLoop>=nodesRemaining) specialNodesPerLoop--;
				int startingSlotIndex=Random.Range(0,unusedOffsets.Count-specialNodesPerLoop);
				
				List<MapRegion> previousNodes=new List<MapRegion>();
				for (int j=0; j<specialNodesPerLoop; j++)
				{
					Vector2 usedSlot=unusedOffsets[startingSlotIndex+j];
					
					Vector2 newOffset=slotOffsetter.Invoke(Random.Range(1f,townNodeSideSize*0.5f-townAreaSpriteSize*0.5f),usedSlot);
					MapRegion newNode=CreateRegion((Vector2)town.transform.localPosition+newOffset);
					if (j>0) newNode.AddConnectedRegion(previousNodes[previousNodes.Count-1],false,townNodeFatigueCost);
					if (j==0 || j==specialNodesPerLoop-1) newNode.AddConnectedRegion(town,false,townNodeFatigueCost);
					previousNodes.Add(newNode);
					nodesRemaining--;//
				}
				//This is necessary to properly track indices and dispose
				unusedOffsets.RemoveRange(startingSlotIndex,specialNodesPerLoop);
			}
			
			for (int i=0; i<nodesRemaining; i++)
			{
				//Vector2 randomPointInNode=Random.insideUnitCircle*(Random.Range(1f,townNodeSideSize*0.5f-townAreaSpriteSize*0.5f));
				Vector2 usedSlot=unusedOffsets[Random.Range(0,unusedOffsets.Count)];
				unusedOffsets.Remove(usedSlot);
				Vector2 newOffset=slotOffsetter.Invoke(Random.Range(1f,townNodeSideSize*0.5f-townAreaSpriteSize*0.5f),usedSlot);
				
				MapRegion newNode=CreateRegion((Vector2)town.transform.localPosition+newOffset);
				newNode.AddConnectedRegion(town,false,townNodeFatigueCost);
			}
		}
		
		
		//Add endgame encounter
		//randomPointInSlot=Random.insideUnitCircle*(Random.Range(1f,townSlotSideSize*0.5f-largeTownSideSize*0.5f));
		//randomSlotIndex=Random.Range(0,townSlotCenters.Count);
		//Vector2 endgameCenterCoords=townSlotCenters[randomSlotIndex]+randomPointInSlot;
		//townSlotCenters.RemoveAt(randomSlotIndex);
		MapRegion endgameEncounter=CreateRegion(endgameCenter);
		foreach(MapRegion region in newTowns) {endgameEncounter.AddConnectedRegion(region,true,townToTownGasCost);}
		
		//Final step - party placement
		//StartCoroutine(MapGenViewFocus(mapRegions[0].GetComponent<RectTransform>()));
		//regionsGridGroup.parent.GetComponent<ScrollRect>().normalizedPosition=Vector2.zero;
		//regionsGridGroup.GetComponent<RectTransform>().localPosition=Vector2.zero;
		//Canvas.ForceUpdateCanvases();//
		
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.H)) {FocusViewOnRegion(mapRegions[0].GetComponent<RectTransform>());}
	}
	
	/*
	MapRegion CreateNewRegion(Vector2 newRegionPos, int xCoord, int yCoord)
	{
		MapRegion newRegion=Instantiate(mapRegionPrefab,newRegionPos,Quaternion.identity) as MapRegion;
		newRegion.xCoord=xCoord;
		newRegion.yCoord=yCoord;
		newRegion.transform.SetParent(regionsGridGroup);
		return newRegion;
	}*/
	/*
	MapRegion CreateNewTownRegion(Vector2 newRegionPos)
	{
		MapRegion newRegion=Instantiate(mapRegionPrefab,newRegionPos,Quaternion.identity) as MapRegion;
		newRegion.transform.SetParent(regionsGridGroup);
		newRegion.GenerateEncounter(false);
		return newRegion;
	}*/
	
	MapRegion CreateRegion(Vector2 newRegionPos)
	{
		MapRegion newRegion=Instantiate(mapRegionPrefab,newRegionPos,Quaternion.identity) as MapRegion;
		newRegion.transform.SetParent(regionsGridGroup);
		newRegion.transform.localPosition=newRegionPos;
		newRegion.GenerateEncounter(false);
		mapRegions.Add(newRegion);
		return newRegion;
	}
	
	public void ClearMap()
	{
		foreach(MapRegion region in mapRegions)//.Values) 
		{
			GameObject.Destroy(region.gameObject);
		}
		mapRegions.Clear();
		//GameObject.Destroy(playerToken);
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
	
	//deprecated
	//public MapRegion GetRegion(int coordX,int coordY) {return GetRegion(new Vector2(coordX,coordY));}
	//public MapRegion GetRegion(Vector2 coords) {return mapRegions[coords];}
	
	public void RegionClicked(MapRegion clickedRegion)
	{	
		bool tryingPartyMove=false;
		if (PartyManager.mainPartyManager.selectedMembers.Count>0)
		{
			//IF MOVING PARTY
			if (PartyManager.mainPartyManager.selectedMembers[0].currentRegion!=clickedRegion)
			{
				if (!EncounterCanvasHandler.main.encounterOngoing)
				{
					tryingPartyMove=true;
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
					if (PartyManager.mainPartyManager.ConfirmMapMovement(clickedRegion, out movedMembers))//.MovePartyToMapCoords(clickedRegion.xCoord,clickedRegion.yCoord))
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
							//DiscoverRegions(clickedRegion.GetCoords());
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
		//IF ENTERING ENCOUNTER INSTEAD
		if (!tryingPartyMove)
		{
			if (clickedRegion.hasEncounter && clickedRegion.localPartyMembers.Count>0) 
			MapManager.main.scoutingHandler.StartDialog(clickedRegion);
		}
		
	}
	/*
	public void MoveMembersToRegion(Vector2 newRegionCoords, params PartyMember[] movedMembers)
	{
		MoveMembersToRegion(GetRegion(newRegionCoords),movedMembers);
	}*/
	
	public void MoveMembersToRegion(MapRegion newRegion, params PartyMember[] movedMembers)
	{
		foreach (PartyMember movedMember in movedMembers)
		{
			movedMember.currentRegion.localPartyMembers.Remove(movedMember);
			movedMember.currentRegion=newRegion;//.worldCoords=newRegion.GetCoords();
			newRegion.localPartyMembers.Add(movedMember);
			memberTokens[movedMember].MoveToken(newRegion.memberTokenGroup);
		}
		//DiscoverRegions(newRegion.GetCoords());
		PartyStatusCanvasHandler.main.RefreshAssignmentButtons(PartyManager.mainPartyManager.selectedMembers);
		
	}	
	//deprecated
	/*
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
	}*/

	public void EnterEncounter(Encounter newEncounter, List<PartyMember> team, bool isAmbush)
	{
		EncounterCanvasHandler.main.StartNewEncounter(newEncounter,team,isAmbush);
		scoutingHandler.EndDialog();
		PartyStatusCanvasHandler.main.RefreshAssignmentButtons();
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
	/*
	public void RemoveHorde(Horde removedHorde)
	{
		
		Vector2 removedHordeCoords=new Vector2(removedHorde.mapX,removedHorde.mapY);
		mapRegions[removedHordeCoords].hordeEncounter=null;
		hordes.Remove(removedHorde);
		GameObject.Destroy(hordeTokens[removedHorde].gameObject);
		hordeTokens.Remove(removedHorde);
	}*/
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
