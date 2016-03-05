using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabAssembler : MonoBehaviour 
{
	/*
	public GameObject apartmentPreset;
	public GameObject warehousePreset;
	public GameObject storePreset;
	public GameObject policeStationPreset;
	public GameObject hospitalPreset;
	public GameObject radioPreset;
	public GameObject hordePreset;
	*/
	public List<GameObject> warehousePresets=new List<GameObject>();
	public List<GameObject> policeStationPresets=new List<GameObject>();
	public List<GameObject> apartmentPresets=new List<GameObject>();
	public List<GameObject> hospitalPresets=new List<GameObject>();
	public List<GameObject> storePresets=new List<GameObject>();
	public List<GameObject> radioPresets=new List<GameObject>();
	
	List<GameObject> GetRandomPresetsList(Encounter.AreaTypes areaType, int segmentCount)
	{
		if (segmentCount<=0) throw new System.Exception("Trying to gain list of encounter segments <=0!");
		List<GameObject> resultList=new List<GameObject>();
		switch (areaType)
		{
			case Encounter.AreaTypes.Warehouse:
			{
				for (int i=0; i<segmentCount; i++)
				{
					resultList.Add(warehousePresets[Random.Range(0,warehousePresets.Count)]);
				}
				break;
			}
			case Encounter.AreaTypes.Police:
			{
				for (int i=0; i<segmentCount; i++)
				{
					resultList.Add(policeStationPresets[Random.Range(0,policeStationPresets.Count)]);
				}
				break;
			}
			case Encounter.AreaTypes.Apartment:
			{
				for (int i=0; i<segmentCount; i++)
				{
					resultList.Add(apartmentPresets[Random.Range(0,apartmentPresets.Count)]);
				}
				break;
			}
			case Encounter.AreaTypes.Hospital:
			{
				for (int i=0; i<segmentCount; i++)
				{
					resultList.Add(hospitalPresets[Random.Range(0,hospitalPresets.Count)]);
				}
				break;
			}
			case Encounter.AreaTypes.Store:
			{
				for (int i=0; i<segmentCount; i++)
				{
					resultList.Add(storePresets[Random.Range(0,storePresets.Count)]);
				}
				break;
			}
			case Encounter.AreaTypes.Endgame:
			{
				for (int i=0; i<segmentCount; i++)
				{
					resultList.Add(radioPresets[Random.Range(0,radioPresets.Count)]);
				}
				break;
			}
		}
		return resultList;
	}
	List<GameObject> GetAllPresetsList(Encounter.AreaTypes areaType)
	{
		List<GameObject> resultList=new List<GameObject>();
		switch (areaType)
		{
			case Encounter.AreaTypes.Warehouse:
			{
				resultList=warehousePresets;
				break;
			}
			case Encounter.AreaTypes.Store:
			{
				resultList=storePresets;
				break;
			}
			case Encounter.AreaTypes.Police:
			{
				resultList=policeStationPresets;
				break;
			}
			case Encounter.AreaTypes.Apartment:
			{
				resultList=apartmentPresets;
				break;
			}
			case Encounter.AreaTypes.Hospital:
			{
				resultList=hospitalPresets;
				break;
			}
			case Encounter.AreaTypes.Endgame:
			{
				resultList=radioPresets;
				break;
			}
		}
		return resultList;
	}
	
	public static PrefabAssembler assembler;
	
	public class DungeonSlot
	{
		public int minX;
		public int maxX;
		public int minY;
		public int maxY;
		public bool occupied;
		public DungeonSlot (int minx, int maxx, int miny, int maxy)
		{
			minX=minx;
			maxX=maxx;
			minY=miny;
			maxY=maxy;
			occupied=false;
		}
		public int GetWidth() {return maxX-minX+1;}
		public int GetHeight() {return maxY-minY+1;}
		public void SetOccupied() {occupied=true;}
	}
	/*
	public struct EncounterSegmentPrefab
	{
		public int width;
		public int height;
		public List<EncounterRoom> segmentRooms;
		public EncounterSegmentPrefab (List<EncounterRoom> rooms, int w, int h)
		{
			segmentRooms=rooms;
			width=w;
			height=h;
		}
	}*/
	
	public Dictionary<Vector2,Dictionary<Vector2,EncounterRoom>> GenerateEncounterMap(Encounter mappedEncounter, Encounter.AreaTypes generatedAreaType
	, int maxTeamSize, out List<EncounterRoom> nonSegmentRooms)
	{
		int slotsPerSide=3;
		//Determine width and height of segments used in setup, then make width=max possible width and height=max possible height
		//List<EncounterSegmentPrefab> prefabSegments=new List<EncounterSegmentPrefab>();
		Dictionary<GameObject,Vector2> prefabTypeDimensions=new Dictionary<GameObject, Vector2>();
		/*
		List<Encounter.AreaTypes> prefabTypes=new List<Encounter.AreaTypes>();
		prefabTypes.Add(Encounter.AreaTypes.Police);
		prefabTypes.Add(Encounter.AreaTypes.Warehouse);*/

		//Encounter.AreaTypes generatedAreaType=Encounter.AreaTypes.Warehouse;
		int roomsPerVertSide=0;
		int roomsPerHorSide=0;
		foreach(GameObject segmentPrefab in GetAllPresetsList(generatedAreaType))//Encounter.AreaTypes areaType in prefabTypes)
		{
			//EncounterSegmentPrefab segment=SetupPrefabMap(mappedEncounter,areaType,false);
			Vector2 prefabMapDimensions=GetSegmentDimensions(segmentPrefab);
			prefabTypeDimensions.Add(segmentPrefab,prefabMapDimensions);
			roomsPerHorSide=Mathf.Max(roomsPerHorSide,(int)prefabMapDimensions.x);
			roomsPerVertSide=Mathf.Max(roomsPerVertSide,(int)prefabMapDimensions.y);
		}
		//print ("Slot side dimensions: "+roomsPerHorSide+";"+roomsPerVertSide);
		
		int gapBetweenSlots=1;
		//Create slots for encounter sections
		Dictionary<Vector2,DungeonSlot> encounterSlots=new Dictionary<Vector2, DungeonSlot>();
		for (int i=0; i<slotsPerSide; i++)
		{
			for (int j=0; j<slotsPerSide; j++)
			{
				int slotMinX=0+(roomsPerHorSide+gapBetweenSlots)*j;
				int slotMaxX=slotMinX+roomsPerHorSide-1;//roomsPerSlotSide-1+(roomsPerSlotSide)*j;
				int slotMinY=0+(roomsPerVertSide+gapBetweenSlots)*i;//0+(roomsPerSlotSide+gapBetweenSlots)*i;
				int slotMaxY=slotMinY+roomsPerVertSide-1;//roomsPerSlotSide-1+(roomsPerSlotSide)*i;
				//print ("Creating slot:"+new Vector2(j,i)+" Minx:"+slotMinX+" Miny:"+slotMinY);
				encounterSlots.Add(new Vector2(j,i),new DungeonSlot(slotMinX,slotMaxX,slotMinY,slotMaxY));
			}
		}
		
		//Checks neighbours of supplied room coord
		System.Func<Vector2,Dictionary<Vector2,DungeonSlot>,List<Vector2>> TryExpand=(Vector2 cursorPos,Dictionary<Vector2,DungeonSlot> availableSlots)=>
		{
			List<Vector2> possibleExpandSlots=new List<Vector2>();
			//NEGATIVE Y GOES UP, POSITIVE Y GOES DOWN
			
			//Check down
			if (availableSlots.ContainsKey(cursorPos+Vector2.up)) 
			{
				if (!availableSlots[cursorPos+Vector2.up].occupied)
				possibleExpandSlots.Add(cursorPos+Vector2.up);
			}
			
			//Check up
			if (availableSlots.ContainsKey(cursorPos+Vector2.down)) 
			{
				if (!availableSlots[cursorPos+Vector2.down].occupied)
				possibleExpandSlots.Add(cursorPos+Vector2.down);
			}
			
			//Check left
			if (availableSlots.ContainsKey(cursorPos+Vector2.left)) 
			{
				if (!availableSlots[cursorPos+Vector2.left].occupied)
				possibleExpandSlots.Add(cursorPos+Vector2.left);
			}
			//Check right
			if (availableSlots.ContainsKey(cursorPos+Vector2.right)) 
			{
				if (!availableSlots[cursorPos+Vector2.right].occupied)
				possibleExpandSlots.Add(cursorPos+Vector2.right);
			}
			return possibleExpandSlots;
		};
		
		
		//Create segments
		List<Vector2> roomsToMoveFrom=new List<Vector2>();
		int entranceSlotX=1;
		int entranceSlotY=1;
		Dictionary<Vector2,Dictionary<Vector2,EncounterRoom>> prefabsBySlot=new Dictionary<Vector2, Dictionary<Vector2, EncounterRoom>>();
		Dictionary<Vector2,EncounterRoom> connectorRooms=new Dictionary<Vector2, EncounterRoom>();
		
		//print ("Creating dungeon");
		int roomsCount=maxTeamSize;
		List<GameObject> usedPrefabs=GetRandomPresetsList(generatedAreaType,roomsCount);
		foreach (GameObject prefabSegment in usedPrefabs)
		{
			//print ("Segment used:"+prefabSegment.name);
		}
		
		for (int i=0; i<roomsCount; i++)
		{
			DungeonSlot usedSlot;
			//Variable assigned later in a roundabout way (compiler doesn't understand it will always be assigned in proper course)
			Vector2 usedSlotCoords=Vector2.zero;
			//Variable not used during entrance creation
			Vector2 roomToExpandFrom=Vector2.zero;
			//if creating first room
			if (i==0) usedSlotCoords=new Vector2(entranceSlotX,entranceSlotY);
			else 
			{
				//Try and find expand slots, going back to previously created rooms if necessary
				//Dictionary<Vector2,Vector2>=new List<Vector2>();
				List<Vector2> slotKeySelection=new List<Vector2>();
				while (roomsToMoveFrom.Count>0 && slotKeySelection.Count==0)
				{
					//Randomly pick a previous room to try and expand from
					roomToExpandFrom=roomsToMoveFrom[Random.Range(0,roomsToMoveFrom.Count-1)];
					//Try to get potential expand directions
					slotKeySelection.AddRange(TryExpand.Invoke(roomToExpandFrom,encounterSlots));
					//If no directions are available, remove room from expandable rooms list and try again
					if (slotKeySelection.Count==0) roomsToMoveFrom.Remove(roomToExpandFrom);
				}
				/*
				int expandingFromIndex=roomsToMoveFrom.Count-1;
				while (expandingFromIndex>=0)
				{
					previousRoomSlot=roomsToMoveFrom[expandingFromIndex];
					slotKeySelection.AddRange(TryExpand.Invoke(roomsToMoveFrom[expandingFromIndex],encounterSlots));
					expandingFromIndex--;
				}*/
				//Randomly select a slot out of the available
				if (slotKeySelection.Count==0) throw new System.Exception("Unable to find expand slot for dungeon gen!");
				else usedSlotCoords=slotKeySelection[Random.Range(0,slotKeySelection.Count)];
			}
		
			
			
				//Fit start coords to slot
				usedSlot=encounterSlots[usedSlotCoords];
				GameObject selectedSegmentPrefab=usedPrefabs[i];//Random.Range(0,usedPrefabs.Count)];
				int freeHorSpace=(int)(usedSlot.GetWidth()-prefabTypeDimensions[selectedSegmentPrefab].x);
				int freeVertSpace=(int)(usedSlot.GetHeight()-prefabTypeDimensions[selectedSegmentPrefab].y);
				
				if (freeHorSpace>0)
				{
					if (freeHorSpace%2>0)
					{
						freeHorSpace-=1;
						if (Random.value<0.5f) usedSlot.minX+=1;
					}
					usedSlot.minX+=freeHorSpace/2;
					//print ("New minx is:"+usedSlot.minX);
				}
				if (freeVertSpace>0)
				{
					if (freeVertSpace%2>0)
					{
						freeVertSpace-=1;
						if (Random.value<0.5f) usedSlot.minY+=1;
					}
					usedSlot.minY+=freeVertSpace/2;
				}
				//Create rooms
				bool entrance=i==0;
				//print ("Segment used:"+selectedSegmentPrefab.name);
				//print ("Segment location:"+new Vector2(usedSlot.minX,usedSlot.minY));
				prefabsBySlot.Add(usedSlotCoords,SetupSegmentRooms(mappedEncounter,selectedSegmentPrefab,entrance,usedSlot));
				//Mark used slot as occupied (marked as negative?)
				if (!entrance) ConnectRoom(usedSlotCoords,roomToExpandFrom,mappedEncounter,prefabsBySlot,ref connectorRooms);
				usedSlot.SetOccupied();
				roomsToMoveFrom.Add(usedSlotCoords);
				//.Remove(expandSlotKey);
		}
		
		//Find total map bounds and fill in encounterRooms dict
		Dictionary<Vector2,EncounterRoom> encounterRooms=new Dictionary<Vector2,EncounterRoom>();
		int minX=int.MaxValue;
		int maxY=int.MinValue;
		int maxX=int.MinValue;
		int minY=int.MaxValue;
		//Add prefab template rooms into the mix
		foreach (Vector2 slotCoord in prefabsBySlot.Keys)//EncounterRoom room in encounterRooms.Values)
		{
			foreach (Vector2 roomCoord in prefabsBySlot[slotCoord].Keys)
			{
				EncounterRoom room=prefabsBySlot[slotCoord][roomCoord];
				maxX=Mathf.Max(maxX,room.xCoord);
				maxY=Mathf.Max(maxY,room.yCoord);
				minX=Mathf.Min (minX,room.xCoord);
				minY=Mathf.Min (minY,room.yCoord);
				if (encounterRooms.ContainsKey(roomCoord)) throw new System.Exception("Coords repeat at:"+roomCoord);
				encounterRooms.Add(roomCoord,room);
			}
			//Vector2 existingCoords=new Vector2(room.xCoord,room.yCoord);
			//if (encounterRooms.ContainsKey(existingCoords)) print("Coords repeat at:"+existingCoords);
		}
		
		nonSegmentRooms=new List<EncounterRoom>();
		//Add connection rooms
		foreach (Vector2 roomCoord in connectorRooms.Keys)
		{
			EncounterRoom room=connectorRooms[roomCoord];
			maxX=Mathf.Max(maxX,room.xCoord);
			maxY=Mathf.Max(maxY,room.yCoord);
			minX=Mathf.Min (minX,room.xCoord);
			minY=Mathf.Min (minY,room.yCoord);
			if (encounterRooms.ContainsKey(roomCoord)) throw new System.Exception("Coords repeat at:"+roomCoord);
			encounterRooms.Add(roomCoord,room);
			nonSegmentRooms.Add(room);
		}
		
		System.Func <Dictionary<Vector2,EncounterRoom>,Vector2, bool> HideImageCheck=(Dictionary<Vector2,EncounterRoom> roomsDict,Vector2 cursor)=>
		{
			bool hideImage=true;
			if (roomsDict.ContainsKey(cursor)) 
			{
				if (!roomsDict[cursor].isWall) hideImage=false; 
			}
			return hideImage;
		};
		
		//Fill in walls
		for (int i=minY-1; i<=maxY+1; i++)
		{
			for (int j=minX-1; j<=maxX+1; j++)
			{
				Vector2 coords=new Vector2(j,i);
				if (!encounterRooms.ContainsKey(coords))
				{
					EncounterRoom newRoom=new EncounterRoom(mappedEncounter,j,i);
					encounterRooms.Add(new Vector2(j,i),newRoom);
					nonSegmentRooms.Add(newRoom);
					newRoom.isWall=true;
					//Hide the image if no non-wall rooms are adjacent
					newRoom.hideImage=true;
					//Goes clockwise
					//Check up
					if (!HideImageCheck.Invoke(encounterRooms,coords+Vector2.down)) newRoom.hideImage=false;
					//Top right
					if (!HideImageCheck.Invoke(encounterRooms,coords+Vector2.down+Vector2.right)) newRoom.hideImage=false;
					//Right etc
					if (!HideImageCheck.Invoke(encounterRooms,coords+Vector2.right)) newRoom.hideImage=false;
					if (!HideImageCheck.Invoke(encounterRooms,coords+Vector2.right+Vector2.up)) newRoom.hideImage=false;
					if (!HideImageCheck.Invoke(encounterRooms,coords+Vector2.up)) newRoom.hideImage=false;
					if (!HideImageCheck.Invoke(encounterRooms,coords+Vector2.up+Vector2.left)) newRoom.hideImage=false;
					if (!HideImageCheck.Invoke(encounterRooms,coords+Vector2.left)) newRoom.hideImage=false;
					if (!HideImageCheck.Invoke(encounterRooms,coords+Vector2.left+Vector2.down)) newRoom.hideImage=false;
				}
			}
		}
		
		return prefabsBySlot;//new List<EncounterRoom>(encounterRooms.Values);
		
	}
	//prefabsBySlot segmentsDict
	void ConnectRoom(Vector2 fromSlot, Vector2 toSlot,Encounter mappedEncounter
	,Dictionary<Vector2,Dictionary<Vector2,EncounterRoom>> regionsBySlot, ref Dictionary<Vector2,EncounterRoom> connectorRoomDict)//,int minXCurrent, int maxXCurrent, int minYCurrent, int maxYCurrent, int minXPrevious, int maxXPrevious, int minYPrevious, int maxYPrevious)
	{
		Dictionary<Vector2, EncounterRoom> toRoomDict=regionsBySlot[toSlot];
		Dictionary<Vector2, EncounterRoom> fromRoomDict=regionsBySlot[fromSlot];
		//Find limits of prefab we are connecting to
		int minXPrevious=int.MaxValue;
		int maxXPrevious=int.MinValue;
		int minYPrevious=int.MaxValue;
		int maxYPrevious=int.MinValue;
		foreach(Vector2 roomKey in toRoomDict.Keys)
		{
			minXPrevious=Mathf.Min(minXPrevious,(int)roomKey.x);
			minYPrevious=Mathf.Min(minYPrevious,(int)roomKey.y);
			maxXPrevious=Mathf.Max(maxXPrevious,(int)roomKey.x);
			maxYPrevious=Mathf.Max(maxYPrevious,(int)roomKey.y);
		}
		
		//Find the limits of new prefab we are trying to connect
		int minXCurrent=int.MaxValue;
		int maxXCurrent=int.MinValue;
		int minYCurrent=int.MaxValue;
		int maxYCurrent=int.MinValue;
		
		foreach (Vector2 roomKey in fromRoomDict.Keys)
		{
			minXCurrent=Mathf.Min(minXCurrent,(int)roomKey.x);
			minYCurrent=Mathf.Min(minYCurrent,(int)roomKey.y);
			maxXCurrent=Mathf.Max(maxXCurrent,(int)roomKey.x);
			maxYCurrent=Mathf.Max(maxYCurrent,(int)roomKey.y);
		}
		//Execute connection in appropriate direction
		//Find start room
		List<EncounterRoom> potentialStartRooms=new List<EncounterRoom>();
		Vector2 connectDirection=toSlot-fromSlot;
		//EncounterRoom connectionStartRoom=null;
		//Down
		if (connectDirection==Vector2.up)
		{
			int minXSearchLimit=Mathf.Max(minXCurrent,minXPrevious);
			int maxXSearchLimit=Mathf.Min(maxXCurrent,maxXPrevious);
			int totalSearchWidth=maxXSearchLimit-minXSearchLimit+1;
			int yCursor=maxYCurrent;
			
			while (potentialStartRooms.Count==0 && yCursor>=minYCurrent)
			{
				for (int j=0; j<totalSearchWidth; j++)
				{
					Vector2 tryKey=new Vector2(minXSearchLimit+j,yCursor);
					if (regionsBySlot[fromSlot].ContainsKey(tryKey)) 
					{
						potentialStartRooms.Add(regionsBySlot[fromSlot][tryKey]);
					}
				}
				if (potentialStartRooms.Count>0) break;
				else yCursor--;
			}
		}
		//Up
		if (connectDirection==Vector2.down)
		{
			int minXSearchLimit=Mathf.Max(minXCurrent,minXPrevious);
			int maxXSearchLimit=Mathf.Min(maxXCurrent,maxXPrevious);
			int totalSearchWidth=maxXSearchLimit-minXSearchLimit+1;
			int yCursor=minYCurrent;
			
			while (potentialStartRooms.Count==0 && yCursor<=maxYCurrent)
			{
				for (int j=0; j<totalSearchWidth; j++)
				{
					Vector2 tryKey=new Vector2(minXSearchLimit+j,yCursor);
					if (regionsBySlot[fromSlot].ContainsKey(tryKey)) 
					{
						potentialStartRooms.Add(regionsBySlot[fromSlot][tryKey]);
					}
				}
				if (potentialStartRooms.Count>0) break;
				else yCursor++;
			}
		}
		//Right
		if (connectDirection==Vector2.right)
		{
			int minYSearchLimit=Mathf.Max(minYCurrent,minYPrevious);
			int maxYSearchLimit=Mathf.Min(maxYCurrent,maxYPrevious);
			int totalSearchWidth=maxYSearchLimit-minYSearchLimit+1;
			int xCursor=maxXCurrent;
			
			while (potentialStartRooms.Count==0 && xCursor>=minXCurrent)
			{
				for (int j=0; j<totalSearchWidth; j++)
				{
					Vector2 tryKey=new Vector2(xCursor,minYSearchLimit+j);
					if (regionsBySlot[fromSlot].ContainsKey(tryKey)) 
					{
						potentialStartRooms.Add(regionsBySlot[fromSlot][tryKey]);
					}
				}
				if (potentialStartRooms.Count>0) break;
				else xCursor--;
			}
		}
		//Left
		if (connectDirection==Vector2.left)
		{
			int minYSearchLimit=Mathf.Max(minYCurrent,minYPrevious);
			int maxYSearchLimit=Mathf.Min(maxYCurrent,maxYPrevious);
			int totalSearchWidth=maxYSearchLimit-minYSearchLimit+1;
			int xCursor=minXCurrent;
			
			while (potentialStartRooms.Count==0 && xCursor<=maxXCurrent)
			{
				for (int j=0; j<totalSearchWidth; j++)
				{
					Vector2 tryKey=new Vector2(xCursor,minYSearchLimit+j);
					if (regionsBySlot[fromSlot].ContainsKey(tryKey)) 
					{
						potentialStartRooms.Add(regionsBySlot[fromSlot][tryKey]);
					}
				}
				if (potentialStartRooms.Count>0) break;
				else xCursor++;
			}
		}
		
		//Draw connection from start room, until it hits and existing room
		if (potentialStartRooms.Count==0) throw new System.Exception("Could not find area connection start room");
		else
		{
			Vector2 cursor=potentialStartRooms[Random.Range(0,potentialStartRooms.Count-1)].GetCoords();
			cursor+=connectDirection;
			while (!regionsBySlot[toSlot].ContainsKey(cursor) && !connectorRoomDict.ContainsKey(cursor))
			{
				EncounterRoom connectionRoom=new EncounterRoom(mappedEncounter,cursor);
				connectorRoomDict.Add(cursor,connectionRoom);
				cursor+=connectDirection;
			}	
		}
	}
	
	Vector2 GetSegmentDimensions(GameObject segmentPrefab)//Encounter.AreaTypes areaType)
	{
		Vector2 res;
		Dictionary<Vector2,EncounterRoom> rooms=SetupSegmentRooms(null,segmentPrefab,false,new DungeonSlot(0,0,100,100), out res);

		return res;
	}
	
	Dictionary<Vector2,EncounterRoom> SetupSegmentRooms(Encounter mappedEncounter, GameObject segmentPrefab/*Encounter.AreaTypes areaType*/, bool makeEntrance, DungeonSlot slotInfo)
	{
		Vector2 emptyOut;
		return SetupSegmentRooms(mappedEncounter,segmentPrefab,makeEntrance,slotInfo,out emptyOut);
	}
	
	Dictionary<Vector2,EncounterRoom> SetupSegmentRooms(Encounter mappedEncounter, GameObject segmentPrefab/*Encounter.AreaTypes areaType*/, bool makeEntrance, DungeonSlot slotInfo
	, out Vector2 dimensions)
	{
		//return SetupEncounterMap(mappedEncounter,areaType,true);
		GameObject copyBuffer=Instantiate(segmentPrefab,new Vector3(0,0,-2),Quaternion.identity) as GameObject; //null;
		/*
		switch (areaType)
		{
		case Encounter.AreaTypes.Warehouse: 
		{
			copyBuffer=Instantiate(warehousePreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject; 
			break;
		}
		case Encounter.AreaTypes.Apartment: 
		{
			copyBuffer=Instantiate(apartmentPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
			break;
		}
		case Encounter.AreaTypes.Store: 
		{
			copyBuffer=Instantiate(storePreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
			break;
		}
		case Encounter.AreaTypes.Police:
		{
			copyBuffer=Instantiate(policeStationPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
			break;
		}
		case Encounter.AreaTypes.Hospital:
		{
			copyBuffer=Instantiate(hospitalPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
			break;
		}
		case Encounter.AreaTypes.Endgame:
		{
			copyBuffer=Instantiate(radioPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
			break;
		}
		case Encounter.AreaTypes.Horde:
		{
			copyBuffer=Instantiate(hordePreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
			break;
		}
		}*/
		
		//List<Vector2> prefabAreas=new List<Vector2>();
		//foreach (EncounterRoomDrawer child in apartmentPreset.GetComponentsInChildren<EncounterRoomDrawer>())
		//{
		//prefabAreas.Add ();
		//}
		//for (int i=0; i<encounterSizeY; i++)
		//{
		//for (int j=0; j<encounterSizeX;j++)
		//{
		List<EncounterRoom> roomList=new List<EncounterRoom>();
		//List<Vector2> coordsList=new List<Vector2>();
		int xMax=int.MinValue;
		int yMax=int.MinValue;
		int xMin=int.MaxValue;
		int yMin=int.MaxValue;
		
		foreach (EncounterRoomDrawer child in copyBuffer.GetComponentsInChildren<EncounterRoomDrawer>())
		{
			xMax=Mathf.Max(xMax,child.presetX);
			yMax=Mathf.Max(yMax,child.presetY);
			xMin=Mathf.Min (xMin,child.presetX);
			yMin=Mathf.Min (yMin,child.presetY);
			EncounterRoom newRoom=new EncounterRoom(mappedEncounter,child.presetX,child.presetY);
			roomList.Add(newRoom);
			//coordsList.Add(new Vector2(child.presetX,child.presetY));
			//if (child.isWall) {newRoom.isWall=true;}
			if (child.isExit && makeEntrance){newRoom.isExit=true;}
			if (child.isEntrance && makeEntrance) {newRoom.isEntrance=true;}
			if (child.hasLoot){newRoom.hasLoot=true;}
		}
		//Find height and width
		dimensions=new Vector2(xMax-xMin+1,yMax-yMin+1);
		
		//Find new coord system offset
		Vector2 newCoordsSystemOffset=(new Vector2(xMin,yMin)*-1)+new Vector2(slotInfo.minX,slotInfo.minY);//Vector2.zero;
		
		Dictionary<Vector2,EncounterRoom> roomDict=new Dictionary<Vector2, EncounterRoom>();
		//Adjust all room coords to slot
		foreach (EncounterRoom room in roomList) 
		{
			room.SetCoords(room.GetCoords()+newCoordsSystemOffset);
			roomDict.Add(room.GetCoords(),room);
		}
		
		
		GameObject.Destroy(copyBuffer);
		return roomDict;//new EncounterSegmentPrefab(roomList,xMax-xMin+1,yMax-yMin+1);//roomList;
	}
	//Deprecate this later
	/*
	public List<EncounterRoom> SetupEncounterMap(Encounter mappedEncounter, Encounter.AreaTypes areaType)
	{
	
		//int encounterSizeX=newEncounter.encounterSizeX;//3;
		//int encounterSizeY=newEncounter.encounterSizeY;//3;
		GameObject copyBuffer=null;
		
		switch (areaType)
		{
			case Encounter.AreaTypes.Warehouse: 
			{
				copyBuffer=Instantiate(warehousePreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject; 
				break;
			}
			case Encounter.AreaTypes.Apartment: 
			{
				copyBuffer=Instantiate(apartmentPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				break;
			}
			case Encounter.AreaTypes.Store: 
			{
				copyBuffer=Instantiate(storePreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				break;
			}
			case Encounter.AreaTypes.Police:
			{
				copyBuffer=Instantiate(policeStationPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				break;
			}
			case Encounter.AreaTypes.Hospital:
			{
				copyBuffer=Instantiate(hospitalPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				break;
			}
			case Encounter.AreaTypes.Endgame:
			{
				copyBuffer=Instantiate(radioPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				break;
			}
			case Encounter.AreaTypes.Horde:
			{
				copyBuffer=Instantiate(hordePreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				break;
			}
		}
		
		//List<Vector2> prefabAreas=new List<Vector2>();
		//foreach (EncounterRoomDrawer child in apartmentPreset.GetComponentsInChildren<EncounterRoomDrawer>())
		//{
		//prefabAreas.Add ();
		//}
		//for (int i=0; i<encounterSizeY; i++)
		//{
		//for (int j=0; j<encounterSizeX;j++)
		//{
		List<EncounterRoom> roomList=new List<EncounterRoom>();
		List<Vector2> coordsList=new List<Vector2>();
		int xMax=0;
		int yMax=0;
		int xMin=0;
		int yMin=0;
		
		foreach (EncounterRoomDrawer child in copyBuffer.GetComponentsInChildren<EncounterRoomDrawer>())
		{
			xMax=Mathf.Max(xMax,child.presetX);
			yMax=Mathf.Max(yMax,child.presetY);
			xMin=Mathf.Min (xMin,child.presetX);
			yMin=Mathf.Min (yMin,child.presetY);
			EncounterRoom newRoom=new EncounterRoom(mappedEncounter,child.presetX,child.presetY);
			roomList.Add(newRoom);
			coordsList.Add(new Vector2(child.presetX,child.presetY));
			//if (child.isWall) {newRoom.isWall=true;}
			if (child.isExit){newRoom.isExit=true;}
			if (child.isEntrance) {newRoom.isEntrance=true;}
			if (child.hasLoot){newRoom.hasLoot=true;}
		}
		
		//fill in empty spaces
		for (int i=yMin-1; i<=yMax+1; i++)
		{
			for (int j=xMin-1; j<=xMax+1; j++)
			{
				Vector2 coords=new Vector2(j,i);
				if (!coordsList.Contains(coords))
				{
					EncounterRoom newRoom=new EncounterRoom(mappedEncounter,j,i);
					roomList.Add(newRoom);
					newRoom.isWall=true;
				}
			}
		}
		GameObject.Destroy(copyBuffer);
		return roomList;
	}*/
	
	
	
	
	void Start()
	{
		assembler=this;
	}
}
