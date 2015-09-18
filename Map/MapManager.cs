using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour 
{
	public static MapManager mainMapManager;
	
	public MapRegion mapRegionPrefab;
	public GameObject playerTokenPrefab;
	
	GameObject playerToken;
	
	public int mapHeight=0;
	public int mapWidth=0;
	
	Rect encounterStartRect=new Rect(5,210,100,25);
	
	
	
	//List<MapRegion> mapRegions=new List<MapRegion>();
	Dictionary<Vector2,MapRegion> mapRegions=new Dictionary<Vector2, MapRegion>();
	
	public void GenerateNewMap()
	{
		float xElementSize=100;
		float yElementSize=100;
		float xElementOffset=xElementSize+10;
		float yElementOffset=yElementSize+10;
		
		for (int i=0; i<mapHeight; i++)
		{
			for (int j=0; j<mapWidth; j++)
			{
				Vector2 newPos=new Vector2(xElementOffset*j,-yElementOffset*i);
				mapRegions.Add (new Vector2(j,i),CreateNewRegion(newPos,j,i));
			}
		}
		//currently token initiates at exactly the same coords as 0,0 map region anyway. Change this later so token is explicitly jumped to starting coord
		if (playerToken!=null) {GameObject.Destroy(playerToken);}
		playerToken=Instantiate(playerTokenPrefab) as GameObject;

		DiscoverRegions(0,0);
	}
	
	MapRegion CreateNewRegion(Vector2 newRegionPos, int xCoord, int yCoord)
	{
		MapRegion newRegion=Instantiate(mapRegionPrefab,newRegionPos,Quaternion.identity) as MapRegion;
		newRegion.xCoord=xCoord;
		newRegion.yCoord=yCoord;
		float rand=Random.value;
		if (rand<0.5) {newRegion.hasEncounter=true;}
		
		return newRegion;
	}
	
	public void ClearMap()
	{
		foreach(MapRegion region in mapRegions.Values) 
		{
			GameObject.Destroy(region.gameObject);
		}
		mapRegions.Clear();
		GameObject.Destroy(playerToken);
	}
	
	public MapRegion GetRegion(int coordX,int coordY) {return GetRegion(new Vector2(coordX,coordY));}
	public MapRegion GetRegion(Vector2 coords) {return mapRegions[coords];}
	
	public void RegionClicked(MapRegion clickedRegion)
	{	
		if (!EncounterManager.mainEncounterManager.encounterOngoing)
		{
			if (PartyManager.mainPartyManager.ConfirmMapMovement(clickedRegion.xCoord,clickedRegion.yCoord))//.MovePartyToMapCoords(clickedRegion.xCoord,clickedRegion.yCoord))
			{
				if (GameEventManager.mainEventManager.RollEvents())
				{
					//PartyManager.mainPartyManager.MovePartyToMapCoords(clickedRegion.xCoord,clickedRegion.yCoord);
					//DiscoverRegions(newRegion.xCoord,newRegion.yCoord);
					//MovePartyTokenToRegion(clickedRegion);
					MovePartyToRegion(clickedRegion);
				}
			}
		}	
	}
	//for special events
	public void TeleportToRegion(MapRegion teleportRegion)
	{
			//PartyManager.mainPartyManager.MovePartyToMapCoords(clickedRegion.xCoord,clickedRegion.yCoord);
			//DiscoverRegions(newRegion.xCoord,newRegion.yCoord);
			//MovePartyTokenToRegion(destinationRegion);
			MovePartyToRegion(teleportRegion);
	}
	//for regular movement
	void MovePartyToRegion(MapRegion newRegion)
	{
		PartyManager.mainPartyManager.MovePartyToMapCoords(newRegion.xCoord,newRegion.yCoord);
		DiscoverRegions(newRegion.xCoord,newRegion.yCoord);
		MovePartyTokenToRegion(newRegion);
	}
	
	public void MovePartyTokenToRegion(MapRegion newRegion)
	{
		Vector3 newTokenPos=newRegion.transform.position;
		newTokenPos.z=playerToken.transform.position.z;
		playerToken.transform.position=newTokenPos;
		
	}
	
	void DiscoverRegions(int originXCoord, int originYCoord)
	{
		int partyVisionRange=1;
		//if (PartyManager.mainPartyManager.dayTime>9 && PartyManager.mainPartyManager.dayTime<21) {partyVisionRange=2;}
		foreach (MapRegion region in mapRegions.Values)
		{
			if (Mathf.Abs(originXCoord-region.xCoord)<=partyVisionRange 
			    && Mathf.Abs(originYCoord-region.yCoord)<=partyVisionRange) {region.discovered=true;}
		}
	}
	
	void DrawRegionExploreDialog()
	{
		Rect exploreLocationAreaRect=new Rect(Screen.width*0.5f-200,Screen.height*0.1f,400,400);
		Rect locationDescriptionRect=new Rect(exploreLocationAreaRect.x+20,exploreLocationAreaRect.y+50,360,100);
		Rect locationScoutButtonRect=new Rect(exploreLocationAreaRect.x+100,exploreLocationAreaRect.y+170,60,20);
		
		MapRegion checkedRegion=mapRegions[new Vector2(PartyManager.mainPartyManager.mapCoordX,PartyManager.mainPartyManager.mapCoordY)];
		string regionEncounterDescription="";
		if (checkedRegion.scouted) 
		{regionEncounterDescription=checkedRegion.regionalEncounter.lootDescription+" infested with "+checkedRegion.regionalEncounter.enemyDescription;} 
		else {regionEncounterDescription="You have not scouted this area";}
		//GUI.Box(exploreLocationAreaRect,"");
		GUI.Box(locationDescriptionRect,regionEncounterDescription);
		if (!checkedRegion.scouted)
		{
			if (GUI.Button(locationScoutButtonRect,"Scout")) 
			{
				checkedRegion.scouted=true;
				PartyManager.mainPartyManager.PassTime(1);
			}
		}
		else
		{
			if (GUI.Button(locationScoutButtonRect,"Enter")) {EncounterManager.mainEncounterManager.StartNewEncounter(checkedRegion);}
		}
	}
	
	void Start()
	{
		mainMapManager=this;
		GameManager.GameStart+=GenerateNewMap;
		GameManager.GameOver+=ClearMap;
	}
	
	void OnGUI()
	{
		if (GameManager.mainGameManager.gameStarted && !EncounterManager.mainEncounterManager.encounterOngoing)
		{
			MapRegion checkedRegion=mapRegions[new Vector2(PartyManager.mainPartyManager.mapCoordX,PartyManager.mainPartyManager.mapCoordY)];
			if (checkedRegion.hasEncounter)
			{
				DrawRegionExploreDialog();
				//if (GUI.Button (encounterStartRect,"Explore")) 
				//{
					//if (checkedRegion.scouted) 
					//{EncounterManager.mainEncounterManager.StartNewEncounter(checkedRegion);}
					//else
					//{
						//DrawRegionExploreDialog();
					//}
				//}
			}
			//else 
			//{
				Rect restButtonRect=new Rect(110,10,80,25);
				if (GUI.Button(restButtonRect,"Wait")) {PartyManager.mainPartyManager.Rest();}
			//}
		}
	}
}
