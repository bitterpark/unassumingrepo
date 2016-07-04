using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapManager : MonoBehaviour 
{
	public static MapManager main;
	
	public Transform regionsGridGroup;
	
	public MapRegion mapRegionPrefab;
	//public GameObject playerTokenPrefab;
	public MemberMapToken memberTokenPrefab;
	
	//GameObject playerToken;

	//Deprecate this
	public int mapHeight=0;
	public int mapWidth=0;

	const float zoomFactor=0.9f;

	public Dictionary<PartyMember,MemberMapToken> memberTokens;
	public HordeTokenDrawer hordeTokenPrefab;
	
	public MapScoutingHandler scoutingHandler;
	public bool hordeLocked=false;
	//List<MapRegion> mapRegions=new List<MapRegion>();
	//Dictionary<Vector2,MapRegion> mapRegions=new Dictionary<Vector2, MapRegion>();
	public List<MapRegion> mapRegions=new List<MapRegion>();
	public List<MapRegion> townCenterRegions=new List<MapRegion>();
	
	//public const int townNodeFatigueCost=25;
	public const int townToTownGasCost=1;

	public static MapRegion.TemperatureRating mapTemperatureRating;
	/*
	public static string GetMapTemperatureDescription()
	{
		string description="";
		switch(mapTemperatureNumber)
		{
			case 0:{description="Freezing"; break;}
			case 1:{description="Cold"; break;}
			case 2:{description="Okay"; break;}
		}
		return description;
	}*/

	void SetDailyTemperatureRating()
	{
		float rand=Random.value;
		if (rand<=1f) mapTemperatureRating=MapRegion.TemperatureRating.Freezing;
		if (rand<=0.66f) mapTemperatureRating=MapRegion.TemperatureRating.Cold;
		if (rand<=0.33f) mapTemperatureRating=MapRegion.TemperatureRating.Okay; 
	}

	public void FocusViewOnRegion(RectTransform regionTransform)
	{
		FocusViewOnLocalPoint(regionTransform.localPosition);
	}
	void FocusViewOnScreenPoint(Vector2 screenPointCoords)
	{
		Vector2 localPoint;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(regionsGridGroup.GetComponent<RectTransform>(),screenPointCoords,null
		,out localPoint)) FocusViewOnLocalPoint(localPoint);
	}
	void FocusViewOnLocalPoint(Vector2 localPoint)
	{
		Vector3 result=new Vector3(-localPoint.x*regionsGridGroup.transform.localScale.x
		,-localPoint.y*regionsGridGroup.transform.localScale.y,0);
		regionsGridGroup.localPosition=result;
	}

	//NEW STUFF

	Dictionary<Vector2, MapSlot> mapSlots;
	float townSlotSideSize;
	float townAreaSpriteSize;

	MapRegion town;

	List<MapNode> currentEncounterNodes = new List<MapNode>();

	struct MapSlot
	{
		public MapSlot(Vector2 index, Vector2 coords)
		{
			slotIndex = index;
			slotCoords = coords;
			occupied = false;
		}		
		public MapSlot(Vector2 index,Vector2 coords, bool isOccupied)
		{
			slotIndex = index;
			slotCoords = coords;
			occupied = isOccupied;
		}

		public Vector2 slotIndex;
		public Vector2 slotCoords;
		public bool occupied;
		public MapSlot SetOccupied(bool newValue)
		{
			MapSlot slot = new MapSlot(slotIndex,slotCoords,newValue);
			return slot;
		}

	}

	struct MapNode
	{
		public MapNode(MapSlot newSlot, MapRegion newRegion)
		{
			occupiedSlot = newSlot;
			region = newRegion;
		}
		
		public MapSlot occupiedSlot;
		public MapRegion region;
	}

	public MapRegion GetTown() { return town;}

	public void GamestartMapSetup()
	{
		//memberTokens = new Dictionary<PartyMember, MemberMapToken>();
		mapSlots = GenerateMapSlots();

		//Determine final size of scrollgroup
		float mapFinalWidth = Screen.width;//borderOffset * 2 + Mathf.Abs(mapMaxX - mapMinX);
		float mapFinalHeight = Screen.height; //borderOffset * 2 + Mathf.Abs(mapMaxY - mapMinY);
		regionsGridGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(mapFinalWidth, mapFinalHeight);

		Vector2 townSlotIndex = new Vector2(1, 1);
		CreateTown(townSlotIndex);

		GenerateEncounters(3);		
	}

	Dictionary<Vector2, MapSlot> GenerateMapSlots()
	{
		int horTownSlots = 3;
		int vertTownSlots = 3;

		townSlotSideSize=Screen.height/vertTownSlots;

		float borderOffset = 20f;

		townAreaSpriteSize = 75f;

		float gapBetweenTownSlots = 0f;

		Dictionary<Vector2, MapSlot> townSlotCenters = new Dictionary<Vector2, MapSlot>();
		for (int i = 0; i < vertTownSlots; i++)
		{
			for (int j = 0; j < horTownSlots; j++)
			{
				//Y coord must be inverted because in UI elements higher Y goes up and lower Y goes down
				
				Vector2 newTownUpperleftPos
				= new Vector2(borderOffset + (townSlotSideSize + gapBetweenTownSlots) * j, -(borderOffset + (townSlotSideSize + gapBetweenTownSlots) * i));
				MapSlot newSlot = new MapSlot(new Vector2(j, i), newTownUpperleftPos + new Vector2(townSlotSideSize * 0.5f, -townSlotSideSize * 0.5f));
				townSlotCenters.Add(new Vector2(j, i), newSlot);//new Vector2(townSlotSideSize*j,townSlotSideSize*i));
				//print ("Slot:"+j+"|"+i+":"+newTownUpperleftPos);
			}
		}
		return townSlotCenters;
	}

	void DailyEncounterRefresh()
	{
		ClearEncounters();
		GenerateEncounters(3);
	}

	void GenerateEncounters(int count)
	{
		List<Vector2> unoccupiedSlotIndeces = new List<Vector2>();
		int createdNodesCount=0;
		do
		{
			unoccupiedSlotIndeces.Clear();
			foreach (Vector2 index in mapSlots.Keys)
			{
				if (!mapSlots[index].occupied)
				{
					unoccupiedSlotIndeces.Add(index);
				}
			}
			if (unoccupiedSlotIndeces.Count>0)
			{
				Vector2 randomSlotIndex = unoccupiedSlotIndeces[Random.Range(0, unoccupiedSlotIndeces.Count)];
				GenerateRandomEncounterNode(randomSlotIndex);
				createdNodesCount++;
			}
		} while (unoccupiedSlotIndeces.Count > 0 && createdNodesCount < count);
	}

	void ClearEncounters()
	{
		ClearEncounters(false);
	}
	void ClearEncounters(bool clearStoryNodes)
	{
		foreach (MapNode node in new List<MapNode>(currentEncounterNodes))
		{
			if (clearStoryNodes
				|| node.region.encounterInRegion.IsFinished()
				|| node.region.encounterInRegion.GetType() != typeof(StoryMissionOne))
			{
				DestroyMapNode(node);
			}
		}
	}

	void GenerateRandomEncounterNode(Vector2 slotIndex)
	{
		MapRegion newEncounterNode=CreateEncounterNode(slotIndex);
		newEncounterNode.AssignEncounter(new Encounter(true));
	}

	public void UnlockStoryEncounterOne()
	{
		foreach (Vector2 index in mapSlots.Keys)
		{
			if (!mapSlots[index].occupied)
			{
				MapRegion storyNode=CreateEncounterNode(index);
				storyNode.AssignEncounter(new StoryMissionOne());
				break;
			}
		}
	}

	MapRegion CreateEncounterNode(Vector2 coordsIndex)
	{
		if (!mapSlots.ContainsKey(coordsIndex)) throw new System.Exception("Encounter node slot not found!");
		MapSlot usedSlot=mapSlots[coordsIndex];
		mapSlots[coordsIndex] = mapSlots[coordsIndex].SetOccupied(true);
		Vector2 slotCenterStartPoint = usedSlot.slotCoords;
		Vector2 randomSlotOffset=Vector2.zero;

		float offsetFromCenterMaxRadius = townSlotSideSize * 0.5f - townAreaSpriteSize;
		List<float> offsetRadiusSteps = new List<float>();
		offsetRadiusSteps.Add(offsetFromCenterMaxRadius);
		offsetRadiusSteps.Add(offsetFromCenterMaxRadius * 0.75f);
		offsetRadiusSteps.Add(offsetFromCenterMaxRadius * 0.5f);
		offsetRadiusSteps.Add(offsetFromCenterMaxRadius * 0.3f);
		offsetRadiusSteps.Add(0);

		randomSlotOffset = Random.insideUnitCircle * (offsetRadiusSteps[Random.Range(0, offsetRadiusSteps.Count)]);
		Vector2 finalCoords = slotCenterStartPoint + randomSlotOffset;

		MapRegion newNodeObj = InstantiateMapNode(finalCoords);
		//newNodeObj.GenerateEncounter();
		currentEncounterNodes.Add(new MapNode(usedSlot, newNodeObj));
		return newNodeObj;
	}

	void EndOfGameClearAll()
	{
		ClearEncounters(true);
		ClearTown();
	}

	void CreateTown(Vector2 coordsIndex)
	{
		if (!mapSlots.ContainsKey(coordsIndex)) throw new System.Exception("Town slot not found!");
		Vector2 coords = mapSlots[coordsIndex].slotCoords;
		mapSlots[coordsIndex] = mapSlots[coordsIndex].SetOccupied(true);

		town = InstantiateMapNode(coords);

	}

	void ClearTown()
	{
		mapSlots[new Vector2(1, 1)] = mapSlots[new Vector2(1, 1)].SetOccupied(false);
		GameObject.Destroy(town.gameObject);
		town = null;
	}


	MapRegion InstantiateMapNode(Vector2 coords)
	{
		MapRegion newNode = Instantiate(mapRegionPrefab, coords, Quaternion.identity) as MapRegion;
		newNode.transform.SetParent(regionsGridGroup);
		newNode.transform.localPosition = coords;
		return newNode;
	}

	void DestroyMapNode(MapNode destroyedNode)
	{
		mapSlots[destroyedNode.occupiedSlot.slotIndex] = mapSlots[destroyedNode.occupiedSlot.slotIndex].SetOccupied(false);
		currentEncounterNodes.Remove(destroyedNode);
		GameObject.Destroy(destroyedNode.region.gameObject);
	}

	//NEW STUFF END
	public void GenerateNewMap()
	{
		memberTokens=new Dictionary<PartyMember,MemberMapToken>();
		
		//Town-related boundaries
		float townAreaSpriteSize=75f;
		float townNodeGap=5f;
		float townNodeWiggleRoom=100f;
		float townNodeSideSize=townAreaSpriteSize+townNodeGap+townNodeWiggleRoom;
		
		int townNodeSlots=1;
		
		float largeTownSideSize=(townNodeSideSize+townNodeGap)*townNodeSlots;//375f;
		//Vector2 smallTownSize=new Vector2(smallTownRadius,smallTownRadius);
		Vector2 largeTownSize=new Vector2(largeTownSideSize,largeTownSideSize);
		
		//Map border boundaries
		int horTownSlots=3;
		int vertTownSlots=3;

		float borderOffset = 20f;
		
		//float townMinX=40f;
		//float townMinY=40f;
		
		
		float townSlotWiggleRoom=200f;
		float townSlotSideSize=largeTownSideSize+townSlotWiggleRoom;
		
		float gapBetweenTownSlots=10f;
		float mapWidth=borderOffset*2+(townSlotSideSize+gapBetweenTownSlots)*horTownSlots;//2000f;
		float mapHeight=borderOffset*2+(townSlotSideSize+gapBetweenTownSlots)*vertTownSlots;//1200f;
		//regionsGridGroup.GetComponent<RectTransform>().sizeDelta=new Vector2(mapWidth,mapHeight);
		
		
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
		
		
		int largeTownCount=4;
		
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
		for (int i=0; i<largeTownCount; i++)
		{
			//Vector2 randomPointInSlot=Random.insideUnitCircle*(Random.Range(1f,townSlotSideSize*0.5f-largeTownSideSize*0.5f));
			//int randomSlotIndex=Random.Range(0,townSlotCenters.Count);
			//Vector2 newTownCenterCoords=townSlotCenters[randomSlotIndex]+randomPointInSlot;
			//townSlotCenters.RemoveAt(randomSlotIndex);
			Vector2 randomSlot=townSlotCenters[Random.Range(0,townSlotCenters.Count)];
			if (i == 0)
			{
				//Set the settlement in the middle
				randomSlot = townSlotCenters[4];
			}
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
			//else endgameCenter=newTownCenterCoords; // - prepares endgame coordinates
			//print ("new town center coords:"+newTownRect.center);
		}
		
		//Offset all town slot coords to fit top used slot to scrollrect top, and leftmost used slot to scrollrect left, also
		//Find final map dimensions
		
		float townslotsXOffset=-(mapMinX-borderOffset);
		float townslotsYOffset=-borderOffset-mapMaxY;
		//Reset min and max values again
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
		//Currently unused
		endgameCenter+=new Vector2(townslotsXOffset,townslotsYOffset);
		//Include endgame region into estimation
		/*
		Vector2 adjustedEndgameSlotMinEdgeCoords=endgameCenter-new Vector2(townSlotSideSize*0.5f,townSlotSideSize*0.5f);
		Vector2 adjustedEndgameSlotMaxEdgeCoords=endgameCenter+new Vector2(townSlotSideSize*0.5f,townSlotSideSize*0.5f);
		mapMinX=Mathf.Min(adjustedEndgameSlotMinEdgeCoords.x,mapMinX);
		mapMinY=Mathf.Min(adjustedEndgameSlotMinEdgeCoords.y,mapMinY);
		mapMaxX=Mathf.Max(adjustedEndgameSlotMaxEdgeCoords.x,mapMaxX);
		mapMaxY=Mathf.Max(adjustedEndgameSlotMaxEdgeCoords.y,mapMaxY);
		*/

		//Determine final size of scrollgroup
		float mapFinalWidth=borderOffset*2+Mathf.Abs(mapMaxX-mapMinX);
		float mapFinalHeight=borderOffset*2+Mathf.Abs(mapMaxY-mapMinY);
		regionsGridGroup.GetComponent<RectTransform>().sizeDelta=new Vector2(mapFinalWidth,mapFinalHeight);
		
		//Create towns from rects	
		//List<MapRegion> newTowns=new List<MapRegion>();
		foreach (Vector2 townCenter in townCenters)
		{
			//Create new town
			MapRegion newTown=CreateRegion(townCenter);
			//foreach(MapRegion region in townCenterRegions) {newTown.AddConnectedRegion(region,true,townToTownGasCost);}
			townCenterRegions.Add(newTown);
			if (townCenterRegions.Count > 1) newTown.AddConnectedRegion(townCenterRegions[0],true,townToTownGasCost);
		}
		
		//POPULATE TOWNS
		//PopulateTowns(townNodeSideSize,townAreaSpriteSize,townNodeSlots,slotOffsetter);
		PartyManager.ETimePassed+=SetDailyTemperatureRating;
	}

	void PopulateTowns(float townNodeSideSize, float townAreaSpriteSize, int townNodeSlotCount, System.Func<float, Vector2, Vector2> slotOffsetter)
	{
		List<Vector2> populatingOffsets = CreateTownNodeSlotCoords(townNodeSideSize, townNodeSlotCount);
		Dictionary<MapRegion, List<MapRegion>> regionsByTown = PopulateInnerTownNodes(townNodeSideSize, townAreaSpriteSize, populatingOffsets, slotOffsetter);
		PersistentEventsAdd(regionsByTown);
	}

	Dictionary<MapRegion, List<MapRegion>> PopulateInnerTownNodes(float townNodeSideSize, float townAreaSpriteSize, List<Vector2> populatingOffsets, System.Func<float, Vector2, Vector2> slotOffsetter)
	{
		//Prepare node lists by town
		Dictionary<MapRegion, List<MapRegion>> regionsByTown = new Dictionary<MapRegion, List<MapRegion>>();
		
		//Fill in nodes
		foreach (MapRegion town in townCenterRegions)
		{
			int nodeCount = Random.Range(6, 9);
			int loopCount = 0;
			int gauntletCount = 0;

			float randomRoll = Random.value;
			//See if town will have any special nodes at all
			if (randomRoll <= 0.8f)
			{
				//See if it will have only loops, gauntlets, or both
				if (Random.value > 0.66f) { loopCount = 1; gauntletCount = 1; }
				if (Random.value <= 0.66f) { loopCount = 2; gauntletCount = 0; }
				if (Random.value <= 0.33f) { loopCount = 0; gauntletCount = 2; }
			}

			List<Vector2> unusedOffsets = new List<Vector2>(populatingOffsets);
			//Create gauntlets and loops
			int specialNodesPerLoop = Random.Range(2, 4);
			int specialNodesPerGauntlet = Random.Range(2, 4);
			int nodesRemaining = nodeCount;

			//print ("Town gauntlets:"+gauntletCount+", town loops:"+loopCount);
			//print ("Nodes per:"+specialNodesPer);
			//print ("Total slots:"+unusedOffsets.Count);
			List<MapRegion> createdTownRegions = new List<MapRegion>();
			regionsByTown.Add(town, createdTownRegions);
			//Do gauntlets
			for (int i = 0; i < gauntletCount; i++)
			{
				int startingSlotIndex = Random.Range(0, unusedOffsets.Count - specialNodesPerGauntlet);

				List<MapRegion> previousNodes = new List<MapRegion>();
				for (int j = 0; j < specialNodesPerGauntlet; j++)
				{
					Vector2 usedSlot = unusedOffsets[startingSlotIndex + j];

					Vector2 newOffset = slotOffsetter.Invoke(Random.Range(1f, townNodeSideSize * 0.5f - townAreaSpriteSize * 0.5f), usedSlot);
					MapRegion newNode = CreateRegion((Vector2)town.transform.localPosition + newOffset);
					createdTownRegions.Add(newNode);
					newNode.townCenter = town;
					if (j > 0) newNode.AddConnectedRegion(previousNodes[previousNodes.Count - 1], false, PartyMember.defaultFatigueMoveCost);
					else newNode.AddConnectedRegion(town, false, PartyMember.defaultFatigueMoveCost);
					previousNodes.Add(newNode);
					nodesRemaining--;
				}
				//This is necesary to properly iterate and dispose indices
				unusedOffsets.RemoveRange(startingSlotIndex, specialNodesPerGauntlet);
			}
			//Do loops
			for (int i = 0; i < loopCount; i++)
			{
				if (specialNodesPerLoop == 3 && specialNodesPerLoop >= nodesRemaining) specialNodesPerLoop--;
				int startingSlotIndex = Random.Range(0, unusedOffsets.Count - specialNodesPerLoop);

				List<MapRegion> previousNodes = new List<MapRegion>();
				for (int j = 0; j < specialNodesPerLoop; j++)
				{
					Vector2 usedSlot = unusedOffsets[startingSlotIndex + j];

					Vector2 newOffset = slotOffsetter.Invoke(Random.Range(1f, townNodeSideSize * 0.5f - townAreaSpriteSize * 0.5f), usedSlot);
					MapRegion newNode = CreateRegion((Vector2)town.transform.localPosition + newOffset);
					createdTownRegions.Add(newNode);
					newNode.townCenter = town;
					if (j > 0) newNode.AddConnectedRegion(previousNodes[previousNodes.Count - 1], false, PartyMember.defaultFatigueMoveCost);
					if (j == 0 || j == specialNodesPerLoop - 1) newNode.AddConnectedRegion(town, false, PartyMember.defaultFatigueMoveCost);
					previousNodes.Add(newNode);
					nodesRemaining--;//
				}
				//This is necessary to properly track indices and dispose
				unusedOffsets.RemoveRange(startingSlotIndex, specialNodesPerLoop);
			}
			//Do regular nodes
			for (int i = 0; i < nodesRemaining; i++)
			{
				//Vector2 randomPointInNode=Random.insideUnitCircle*(Random.Range(1f,townNodeSideSize*0.5f-townAreaSpriteSize*0.5f));
				Vector2 usedSlot = unusedOffsets[Random.Range(0, unusedOffsets.Count)];
				unusedOffsets.Remove(usedSlot);
				Vector2 newOffset = slotOffsetter.Invoke(Random.Range(1f, townNodeSideSize * 0.5f - townAreaSpriteSize * 0.5f), usedSlot);

				MapRegion newNode = CreateRegion((Vector2)town.transform.localPosition + newOffset);
				createdTownRegions.Add(newNode);
				newNode.townCenter = town;
				newNode.AddConnectedRegion(town, false, PartyMember.defaultFatigueMoveCost);
			}
		}
		return regionsByTown;
	}

	List<Vector2> CreateTownNodeSlotCoords(float townNodeSideSize, int townNodeSlots)
	{
		List<Vector2> populatingOffsets = new List<Vector2>();
		float startOffset = -townNodeSideSize;
		for (int i = 0; i < townNodeSlots; i++)
		{
			for (int j = 0; j < townNodeSlots; j++)
			{
				if (!(j == 1 && i == 1)) populatingOffsets.Add(new Vector2(startOffset + townNodeSideSize * j, startOffset + townNodeSideSize * i));
			}
		}
		return populatingOffsets;
	}



	void PersistentEventsAdd(Dictionary<MapRegion,List<MapRegion>> regionsByTown)
	{
		//Add persistent events and gas event to each town
		foreach (MapRegion town in regionsByTown.Keys)
		{
			int requiredPersistentEventNodes = 1;
			List<MapRegion> encounterRegionsRemaining = new List<MapRegion>(regionsByTown[town]);
			for (int i = 0; i <= requiredPersistentEventNodes; i++)
			{
				//If there aren't enough non-persistent nodes left, terminate
				if (encounterRegionsRemaining.Count == 0) break;
				else
				{
					//Randomly pick a region index out of the remaining regions
					int randomIndex = Random.Range(0, encounterRegionsRemaining.Count);
					//Do gas event first
					if (i == 0) encounterRegionsRemaining[randomIndex].SetRegionalEvent(new GasolineEvent());
					else
					{
						//Try to add a random persistent event.
						PersistentEvent assignedEvent = GameEventManager.mainEventManager.GetPersistentEvent();
						if (assignedEvent != null) encounterRegionsRemaining[randomIndex].SetRegionalEvent(assignedEvent);
					}
					//WARNING - even if no random persistent event could be found above (because they'd all been used up), this will still
					//remove regions from available pool
					encounterRegionsRemaining.RemoveAt(randomIndex);
				}
			}
		}
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
		newRegion.GenerateEncounter();
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
		townCenterRegions.Clear();
		PartyManager.ETimePassed-=SetDailyTemperatureRating;
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
		/*
		if (PartyManager.mainPartyManager.selectedMembers.Count>0)
		{
			//IF MOVING PARTY
			if (PartyManager.mainPartyManager.selectedMembers[0].currentRegion!=clickedRegion)
			{
				if (!EncounterCanvasHandler.main.encounterOngoing)
				{
					tryingPartyMove=true;
					
					List<PartyMember> movedMembers;
					if (PartyManager.mainPartyManager.ConfirmMapMovement(clickedRegion, out movedMembers))//.MovePartyToMapCoords(clickedRegion.xCoord,clickedRegion.yCoord))
					{
						bool intercityMove=clickedRegion.connections[PartyManager.mainPartyManager.selectedMembers[0].currentRegion].isIntercity;
						//This bool is really awkward because of a bunch of unconnected changes I made over time
						bool noEventBasedMove=true;
						if (intercityMove) 
						{
							movedMembers=movedMembers[0].currentRegion.localPartyMembers;
							noEventBasedMove=false;
							GameEventManager.mainEventManager.DoEvent(new TownMove(),clickedRegion,movedMembers);
						}
						
						//bool eventHappened=false;
						
						if (noEventBasedMove)
						{
							//MovePartyToRegion(clickedRegion);
							//foreach (PartyMember member in movedMembers)
							//{
							MoveMembersToRegion(clickedRegion,movedMembers.ToArray());
							//}
							//DiscoverRegions(clickedRegion.GetCoords());
							//Threat level roll
							
						}
						//Should deprecate this later
						GameEventManager.mainEventManager.TryNextQueuedEvent();
					}
				}
			}
		}
		*/
		//IF ENTERING ENCOUNTER INSTEAD
		if (!tryingPartyMove)
		{
			//if (clickedRegion.localPartyMembers.Count>0) 
			if (clickedRegion == town) TownScreen.main.OpenTownScreen();
			else MapManager.main.scoutingHandler.StartDialog(clickedRegion);
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
			PartyManager.mainPartyManager.TryRemoveMemberTask(movedMember);
			movedMember.currentRegion.localPartyMembers.Remove(movedMember);
			movedMember.currentRegion=newRegion;//.worldCoords=newRegion.GetCoords();
			newRegion.localPartyMembers.Add(movedMember);
			memberTokens[movedMember].MoveToken(newRegion.memberTokenGroup);
			//newRegion.scouted=true;
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
		GameManager.GameOver += EndOfGameClearAll;
		PartyManager.ETimePassed += DailyEncounterRefresh;
	}

	void Update()
	{
		/*
		if (!EncounterCanvasHandler.main.encounterOngoing)
		{
			//ZOOM OUT
			if (Input.GetAxis("Mouse ScrollWheel")<0)
			{
				//Do zoom
				float scaleFactor=Mathf.Max(regionsGridGroup.transform.localScale.x*zoomFactor,Mathf.Pow(zoomFactor,3));
				regionsGridGroup.transform.localScale=new Vector3(scaleFactor,scaleFactor,1);
			}
			//ZOOM IN
			if (Input.GetAxis("Mouse ScrollWheel")>0)
			{
				//Do zoom
				float scaleFactor=Mathf.Min(regionsGridGroup.transform.localScale.x/zoomFactor,1/Mathf.Pow(zoomFactor,3));
				regionsGridGroup.transform.localScale=new Vector3(scaleFactor,scaleFactor,1);
				//Refocus camera
				FocusViewOnScreenPoint(Input.mousePosition);
			}
		}*/
	}
}
