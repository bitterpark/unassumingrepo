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
	public static PrefabAssembler assembler;

	public List<EncounterRoom> SetupEncounterMap(Encounter mappedEncounter, Encounter.LootTypes areaType)
	{
	
		//int encounterSizeX=newEncounter.encounterSizeX;//3;
		//int encounterSizeY=newEncounter.encounterSizeY;//3;
		GameObject copyBuffer=null;
		
		/*
		if (areaType==Encounter.LootTypes.Warehouse) 
		{
			copyBuffer=Instantiate(warehousePreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
		}
		if (areaType==Encounter.LootTypes.Apartment) 
		{
			copyBuffer=Instantiate(apartmentPreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
		}
		if (areaType==Encounter.LootTypes.Store) 
		{
			copyBuffer=Instantiate(storePreset,new Vector3(0,0,-2),Quaternion.identity) as GameObject;
		}*/
		
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
		int i=0;
		int j=0;
		
		foreach (EncounterRoomDrawer child in copyBuffer.GetComponentsInChildren<EncounterRoomDrawer>())
		{
			i=child.presetY;
			j=child.presetX;
			EncounterRoom newRoom=new EncounterRoom(mappedEncounter,j,i);
			roomList.Add(newRoom);
			//Vector2 newRoomPos=encounterMapTopLeft+new Vector2((elementSizeX+elementGapX)*j,-(elementSizeY+elementGapY)*i);
			//EncounterRoomDrawer newRoomDrawer=Instantiate(encounterRoomPrefab,newRoomPos,Quaternion.identity) as EncounterRoomDrawer;
			//newRoomDrawer.SetEncounterRoomInfo(newEncounter.encounterMap[new Vector2(j,i)]);
			//encounterRoomObjects.Add (newRoomDrawer);
			//encounterMap.Add (new Vector2(j,i),newRoom);
			//newRoom.xCoord=j;
			//newRoom.yCoord=i;
			/*
				if (Random.value<0.5) 
				{
					newRoom.hasEnemies=true;
				} else {newRoom.hasEnemies=false;}*/
			if (child.isExit)//(j==encounterPlayerX && i==encounterPlayerY) 
			{
				//if (encounterPlayerToken!=null) {}
				newRoom.isExit=true;
			}
			if (child.hasLoot)
			{
				newRoom.hasLoot=true;
			}
		}
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
