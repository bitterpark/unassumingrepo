#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]  
public class EncounterMapRootEditor : MonoBehaviour {
	
	public GameObject rootPrefab;
	public EncounterRoomDrawer roomPrefab;
	public Dictionary<Vector2,EncounterRoomDrawer> encounterRooms;
	
	//public Transform currentMapRoot;
	
	public enum AppendRoomDirection {Left,Right,Up,Down};
	
	public const int roomSize=20;
	
	public void AddRoomNextToExisting(EncounterRoomDrawer existingRoom,AppendRoomDirection dir)
	{	
		switch (dir)
		{
			case AppendRoomDirection.Right:
			{
				CreateRoom(new Vector2(existingRoom.presetX+1,existingRoom.presetY));
				break;
			}
			case AppendRoomDirection.Left:
			{
				CreateRoom(new Vector2(existingRoom.presetX-1,existingRoom.presetY));
				break;
			}
			case AppendRoomDirection.Up:
			{
				CreateRoom(new Vector2(existingRoom.presetX,existingRoom.presetY-1));
				break;
			}
			case AppendRoomDirection.Down:
			{
				CreateRoom(new Vector2(existingRoom.presetX,existingRoom.presetY+1));
				break;
			}
		}
	}
	
	public void AppendNewRoom(Vector2 newRoomCoords)
	{	
		CreateRoom(newRoomCoords);
	}
	
	void CreateRoom(Vector2 roomCoords)
	{
		if (!encounterRooms.ContainsKey(roomCoords))
		{
			EncounterRoomDrawer newRoom=Instantiate(roomPrefab);
			newRoom.transform.SetParent(transform.GetChild(0),false);
			newRoom.transform.position=transform.GetChild(0).position+new Vector3(roomCoords.x*(roomSize+5),-(roomCoords.y*(roomSize+5)),0);
			newRoom.presetX=(int)roomCoords.x;
			newRoom.presetY=(int)roomCoords.y;
			encounterRooms.Add(roomCoords,newRoom);
		}
		else {throw new System.Exception("Creating a room on an already existing room!");}
	}
	
	void AddExistingRoom(EncounterRoomDrawer addedRoom)
	{
		encounterRooms.Add(new Vector2(addedRoom.presetX,addedRoom.presetY),addedRoom);
	}
	
	//public void AddRoom()
	public void DeleteRoom(EncounterRoomDrawer deletedRoom) 
	{
		encounterRooms.Remove(new Vector2(deletedRoom.presetX,deletedRoom.presetY));
		GameObject.DestroyImmediate(deletedRoom.gameObject);
	}
	/*
	void Start() 
	{
		
		if (encounterRooms==null)
		{
			//encounterRooms=new Dictionary<Vector2, EncounterRoomDrawer>();
			//CreateRoom(Vector2.zero);
		}
	}*/
	
	public void StartNewTemplate()
	{
		ClearCurrentTemplate();
		encounterRooms=new Dictionary<Vector2, EncounterRoomDrawer>();
		GameObject newRoot=Instantiate(rootPrefab,transform.position,Quaternion.identity) as GameObject;
		//currentMapRoot=newRoot.transform;
		newRoot.transform.SetParent(this.transform,false);
		newRoot.transform.position=this.transform.position;
		CreateRoom(Vector2.zero);
	}
	
	public void ClearCurrentTemplate()
	{
		if (transform.childCount>0)
		{
			GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
			encounterRooms=null;
		}
	}
	
	void Update()
	{
		//This is used only to load in existing templates
		if (transform.childCount>0 && encounterRooms==null) 
		{
			transform.GetChild(0).position=transform.position;
			encounterRooms=new Dictionary<Vector2, EncounterRoomDrawer>();
			foreach (EncounterRoomDrawer child in transform.GetChild(0).GetComponentsInChildren<EncounterRoomDrawer>())
			{
				AddExistingRoom(child);
			}
		}
		//This is used for manually deleting building templates
		if (transform.childCount==0 && encounterRooms!=null)
		{
			encounterRooms=null;
		}
	}
}

[CustomEditor(typeof(EncounterMapRootEditor))]
class EncounterMapRootEditorInspector : Editor 
{
	
	public override void OnInspectorGUI () 
	{
		DrawDefaultInspector();
		EncounterMapRootEditor myTarget=(EncounterMapRootEditor)target;
		//myTarget.lookAtPoint = EditorGUILayout.Vector3Field ("Look At Point", myTarget.lookAtPoint);
		if (myTarget.encounterRooms==null)
		{
			if (GUILayout.Button("Create new template")) {myTarget.StartNewTemplate();}
		}
		else 
		{
			if (GUILayout.Button("Clear current template")) 
			{myTarget.ClearCurrentTemplate();}
		}
		
		if (GUI.changed) EditorUtility.SetDirty (target);
	}
	
	
	void OnSceneGUI () 
	{
		
		EncounterMapRootEditor myTarget=(EncounterMapRootEditor)target;
		
		if (myTarget.encounterRooms!=null)
		{
			Dictionary<Vector2,EncounterRoomDrawer> roomsBuffer=new Dictionary<Vector2, EncounterRoomDrawer>(myTarget.encounterRooms);
			foreach (Vector2 roomCoords in roomsBuffer.Keys)
			{
				EncounterRoomDrawer room=roomsBuffer[roomCoords];
				//Create to the right
				//Vector2 newRoomCoords=new Vector2(room.presetX+1,room.presetY);
				
				System.Action<Vector2> createRoomButton=(Vector2 newRoomCoords)=>
				{
					if (!myTarget.encounterRooms.ContainsKey(newRoomCoords))
					{
						Vector3 buttonPos=myTarget.transform.GetChild(0).position;//currentMapRoot.position;
						buttonPos+=new Vector3(newRoomCoords.x*(EncounterMapRootEditor.roomSize+5),-newRoomCoords.y*(EncounterMapRootEditor.roomSize+5),0);
						if (Handles.Button(buttonPos,Quaternion.identity,5,4,Handles.CircleCap))
						{
							//myTarget.AddRoomNextToExisting(room,EncounterMapRootEditor.AppendRoomDirection.Right);
							myTarget.AppendNewRoom(newRoomCoords);
						}
					}
				};
				
				//RIGHT
				createRoomButton.Invoke(new Vector2(room.presetX+1,room.presetY));
				//LEFT
				createRoomButton.Invoke(new Vector2(room.presetX-1,room.presetY));
				//UP
				createRoomButton.Invoke(new Vector2(room.presetX,room.presetY-1));
				//DOWN
				createRoomButton.Invoke(new Vector2(room.presetX,room.presetY+1));
				//Delete button (MUST BE LAST!!!)
				if (Handles.Button(room.transform.position+new Vector3(8,-8,0),Quaternion.identity,3,2,Handles.DotCap)) {myTarget.DeleteRoom(room);}
				
				
			}
			
			
			/*
		//myTarget.lookAtPoint = Handles.PositionHandle (myTarget.lookAtPoint, Quaternion.identity);
		if (Handles.Button(myTarget.lookAtPoint,Quaternion.identity,50,40,Handles.DotCap))
		{
			myTarget.lookAtPoint=new Vector3(myTarget.lookAtPoint.y,myTarget.lookAtPoint.x,myTarget.lookAtPoint.z);
			EditorUtility.SetDirty (myTarget);
		}
		//=Handles.bu
		if (GUI.changed)
			EditorUtility.SetDirty (myTarget);*/
		}
	}
	
}
#endif
