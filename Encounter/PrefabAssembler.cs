using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabAssembler : MonoBehaviour 
{

	public GameObject apartmentPreset;
	public GameObject warehousePreset;
	public GameObject storePreset;
	public GameObject policeStationPreset;
	public GameObject hospitalPreset;
	public GameObject radioPreset;
	public GameObject hordePreset;
	public static PrefabAssembler assembler;

	public List<EncounterRoom> SetupEncounterMap(Encounter mappedEncounter, Encounter.LootTypes areaType)
	{
	
		//int encounterSizeX=newEncounter.encounterSizeX;//3;
		//int encounterSizeY=newEncounter.encounterSizeY;//3;
		GameObject copyBuffer=null;
		
		switch (areaType)
		{
			case Encounter.LootTypes.Warehouse: 
			{
				copyBuffer=Instantiate(warehousePreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject; 
				break;
			}
			case Encounter.LootTypes.Apartment: 
			{
				copyBuffer=Instantiate(apartmentPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				break;
			}
			case Encounter.LootTypes.Store: 
			{
				copyBuffer=Instantiate(storePreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				break;
			}
			case Encounter.LootTypes.Police:
			{
				copyBuffer=Instantiate(policeStationPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				break;
			}
			case Encounter.LootTypes.Hospital:
			{
				copyBuffer=Instantiate(hospitalPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				break;
			}
			case Encounter.LootTypes.Endgame:
			{
				copyBuffer=Instantiate(radioPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				break;
			}
			case Encounter.LootTypes.Horde:
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
		//GameManager.DebugPrint("New horde added, maxX:"+xMax);
		//}
		//}
		GameObject.Destroy(copyBuffer);
		return roomList;
	}
	
	
	
	void Start()
	{
		assembler=this;
	}
}
