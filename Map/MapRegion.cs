using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapRegion : MonoBehaviour
{
	public class RegionConnection
	{
		public VectorUI roadLine;
		public List<MapRegion> connectsRegions;
		public int moveCost;
		public bool isIntercity;
		public RegionConnection (VectorUI road,int cost,bool intercity, params MapRegion[] connectingRegions)
		{
			roadLine=road;
			isIntercity=intercity;
			//This can be fatigue cost or gas cost, depending on whether or not it's intercity
			moveCost=cost;
			connectsRegions=new List<MapRegion>(connectingRegions);
			
		}
	}
	
	//Actual town centers have this set to null, nodes radiating from them have it set to their center
	public MapRegion townCenter=null;
	//public Vector2 GetCoords() {return new Vector2(xCoord,yCoord);}
	public Dictionary<MapRegion,RegionConnection> connections=new Dictionary<MapRegion,RegionConnection>();
	
	public VectorUI roadLinePrefab;
	
	//For creating connections in MapManager mapgen
	public void AddConnectedRegion(MapRegion region, bool intercity, int moveCost)
	{
		//Create graphic
		VectorUI newVectorUI=Instantiate(roadLinePrefab);
		List<Vector2> roadPoints=new List<Vector2>();
		roadPoints.Add(transform.position);
		roadPoints.Add(region.transform.position);
		newVectorUI.AssignVectorLine("Road Line",transform.parent,false,roadPoints,8f,Color.black);
		//Create connection
		RegionConnection newConnection=new RegionConnection(newVectorUI,moveCost,intercity,this,region);
		region.AddConnectedRegion(this,newConnection);
		connections.Add(region,newConnection);
	}
	//For adding created connections to point B of the connection
	public void AddConnectedRegion(MapRegion region, RegionConnection connection)
	{	
		if (!connections.ContainsKey(region))
		{
			if (!region.connections.ContainsKey(this))
			{
				connections.Add(region,connection);
			}
		}
	}
	
	public Sprite undiscoveredLocSprite;
	public Sprite emptyLocSprite;
	public Sprite encounterSprite;
	public Sprite policeStationSprite;
	public Sprite storeSprite;
	public Sprite warehouseSprite;
	public Sprite apartmentSprite;
	public Sprite hospitalSprite;
	public Sprite radioStationSprite;
	
	public Sprite hordeSprite;
	
	//public GameObject campTokenPrefab;
	public Image regionGraphic;
	public Image stashToken;
	public Image campToken;
	public Image carToken;
	
	public Transform memberTokenGroup;
	
	public bool discovered
	{
		get {return _discovered;}
		set
		{
			_discovered=value;
			SetSprite();
		}
	}
	public bool _discovered=false;
	
	public bool scouted
	{
		get {return _scouted;}
		set 
		{
			_scouted=value;
			SetSprite();
		}
	}
	
	public bool _scouted=false;
	
	public bool visible
	{
		get {return _visible;}
		set 
		{
			_visible=value;
			SetSprite();
		}
	}
	public bool _visible=false;
	
	public bool hasGasoline=false;
	public bool hasCar
	{
		get {return _hasCar;}
	}
	bool _hasCar=false;

	public Camp.TemperatureRating GetTemperature()
	{
		if (hasCamp) return campInRegion.GetTemperature();
		else return Camp.TemperatureRating.Very_Cold;
	}

	public ThreatLevels GetCampingThreat()
	{
		if (hasCamp) return campInRegion.GetThreatLevel();
		else return ThreatLevels.High; 
	}

	public void SetCar(bool carIsInregion)
	{
		_hasCar=carIsInregion;
		if (_hasCar) carToken.enabled=true;
		else carToken.enabled=false;
	}
	
	public enum ThreatLevels {None,Low,Medium,High};
	//public ThreatLevels threatLevel=ThreatLevels.Low;
	public ThreatLevels CalculateThreatLevel(List<PartyMember> movingMembers)
	{
		return CalculateThreatLevel(movingMembers.Count);
	}
	public ThreatLevels CalculateThreatLevel(int memberCount)
	{
		ThreatLevels estimatedThreat=ThreatLevels.None;
		if (hasEncounter)
		{
				//int requiredMemberDifference=Mathf.Abs(regionalEncounter.maxAllowedMembers-movingMembers.Count);
			int memberCountThreat=Mathf.Abs(regionalEncounter.maxAllowedMembers-memberCount);

			if (memberCountThreat+ambientThreatNumber>0) estimatedThreat=ThreatLevels.Low;
			if (memberCountThreat+ambientThreatNumber>1) estimatedThreat=ThreatLevels.Medium;
			if (memberCountThreat+ambientThreatNumber>2) estimatedThreat=ThreatLevels.High;
		}
		return estimatedThreat;
	}
	public int ambientThreatNumber=0;
	
	public bool hasEncounter=false;
	
	public bool hasCamp=false;
	public int campSetupTimeRemaining=5;
	public Camp campInRegion;
	
	public List<PartyMember> localPartyMembers=new List<PartyMember>();
	
	List<InventoryItem> stashedItems=new List<InventoryItem>();
	public List<InventoryItem> GetStashedItems() {return stashedItems;}
	public void StashItem(InventoryItem stashedItem)
	{
		stashedItems.Add(stashedItem);
		stashToken.enabled=true;
	}
	public void TakeStashItem(InventoryItem takenItem)
	{
		if (!stashedItems.Contains(takenItem)) throw new System.Exception("Trying to take item that does not exist in region");
		else
		{
			stashedItems.Remove(takenItem);
			if (stashedItems.Count==0) stashToken.enabled=false;
		}
	}
	/*
	{
		get {return _hasEncounter;}
		set 
		{
			_hasEncounter=value;
			if (_hasEncounter) 
			{
				//GetComponent<SpriteRenderer>().sprite=encounterSprite;//.material.color=Color.blue;
				
				regionalEncounter=new Encounter();
				if (regionalEncounter.encounterLootType!=Encounter.LootTypes.Endgame)
				{
					if (Random.value<0f) {isHive=true;}
				}
			}
			//else {GetComponent<SpriteRenderer>().sprite=emptyLocSprite;}//.material.color=Color.gray;}
			SetSprite();
		}
	}
	public bool _hasEncounter=false;*/
	public Encounter regionalEncounter;
	
	/*
	bool hasHorde=false;
	public Horde hordeEncounter
	{
		get {return _hordeEncounter;}
		set
		{
			_hordeEncounter=value;
			if (_hordeEncounter!=null)
			{
				hasHorde=true;
			}
			else {hasHorde=false;}
			SetSprite();
		}
	}
	Horde _hordeEncounter;*/
	/*
	//hive is rolled on hasEncounter=true
	public bool isHive
	{
		get {return _isHive;}
		set 
		{
			_isHive=value;
			if (_isHive) 
			{
				//transform current encounter into a hive
				regionalEncounter=new Hive(this,regionalEncounter.encounterAreaType,regionalEncounter.encounterEnemyType);
				PartyManager.TimePassed+=GenerateHorde;
			}
			else {PartyManager.TimePassed-=GenerateHorde;}
		}
	}
	bool _isHive=false;
	*/
	bool drawMouseoverText=false;
	
	void SetSprite()
	{
		if (!_discovered)
		{
			regionGraphic.sprite=undiscoveredLocSprite;
		}
		else
		{
			/*
			if (hasHorde && visible)
			{
				regionGraphic.sprite=hordeSprite;
			}*/
			//else
			{
				if (hasEncounter)
				{
					if (scouted)
					{
						switch (regionalEncounter.encounterAreaType)
						{
							case Encounter.AreaTypes.Hospital: {regionGraphic.sprite=hospitalSprite; break;}
							case Encounter.AreaTypes.Apartment: {regionGraphic.sprite=apartmentSprite; break;}
							case Encounter.AreaTypes.Store: {regionGraphic.sprite=storeSprite; break;}
							case Encounter.AreaTypes.Warehouse:{regionGraphic.sprite=warehouseSprite; break;}
							case Encounter.AreaTypes.Police: {regionGraphic.sprite=policeStationSprite; break;}
							case Encounter.AreaTypes.Endgame: {regionGraphic.sprite=radioStationSprite; break;}
						}
					}
					else
					{
						regionGraphic.sprite=encounterSprite;
					}
				}
				else
				{
					regionGraphic.sprite=emptyLocSprite;
				}
			}
			
			//if (isHive) regionGraphic.color=Color.red;
			if (visible)
			{
				//if (hasHorde) {regionGraphic.color=Color.red;} 
				//else {regionGraphic.color=Color.white;}
				regionGraphic.color=Color.white;
			}
			else
			{
				regionGraphic.color=Color.gray;
			}
		}
	}
	
	public void GenerateEncounter(bool isEndgame)
	{
		hasEncounter=true;
		if (isEndgame) 
		{
			regionalEncounter=new Encounter(true);
			//threatLevel=ThreatLevels.High;
		}
		else 
		{
			regionalEncounter=new Encounter();
			
			float threatRoll=Random.value;
			if (threatRoll<0.75f) ambientThreatNumber=1;
			if (threatRoll<0.5f) ambientThreatNumber=2;
			if (threatRoll<0.25f) ambientThreatNumber=3;
		}
		//if (threatLevel==ThreatLevels.Low) campSetupTimeRemaining+=2;
		//if (threatLevel==ThreatLevels.Medium) campSetupTimeRemaining+=4;
		//if (threatLevel==ThreatLevels.High) campSetupTimeRemaining+=6;
	}
	
	public void GenerateHorde(int emptySigVar)
	{
		/*
		if (Random.value<0.1f) 
		{
			MapManager.main.AddHorde(new Vector2(xCoord,yCoord),regionalEncounter.encounterEnemyType);
			//GameManager.DebugPrint("New horde added, maxX:");
		}*/
	}
	
	public void SetUpCamp(int manhoursInvested)
	{
		campSetupTimeRemaining-=manhoursInvested;
		campToken.gameObject.SetActive(true);
		if (campSetupTimeRemaining<=0)
		{
			hasCamp=true;
			//print ("camp setup");
			campInRegion=new Camp();
			campToken.GetComponentInChildren<Text>().enabled=false;
			InventoryScreenHandler.mainISHandler.RefreshInventoryItems();
			//GameObject campToken=Instantiate(campTokenPrefab);
			//campToken.transform.SetParent(this.transform);//,true);
			//campToken.transform.position=this.transform.position;
		}
		else campToken.GetComponentInChildren<Text>().text=campSetupTimeRemaining.ToString();
	}
	
	//void Start() {transform.Rotate(new Vector3(-90,0,0));}
	void Start() {SetSprite();}

	void OnDestroy()
	{
		foreach (RegionConnection connection in connections.Values){GameObject.Destroy(connection.roadLine.gameObject);}
	}

	public void GraphicClicked()
	{
		MapManager.main.RegionClicked(this);
	}
	/*
	#region IPointerDownHandler implementation

	public void OnPointerDown (PointerEventData eventData)
	{
		MapManager.main.RegionClicked(this);
	}

	#endregion
	*/
	/*
	void OnMouseDown()
	{
		if (!EventSystem.current.IsPointerOverGameObject()) MapManager.mainMapManager.RegionClicked(this);
	}*/
	/*
	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		bool textExists=true;
		string areaDescription="";
		if (!discovered) {areaDescription="Undiscovered";}
		else 
		{
			{
				if (hasEncounter)
				{
					if (!scouted) {areaDescription="Not scouted";}
					else
					{
						areaDescription+=regionalEncounter.lootDescription+"\n\n";
						//Describe all potential loot
						areaDescription+="May contain:\n";
						foreach (InventoryItem.LootMetatypes metatype in regionalEncounter.possibleLootTypes.Values)
						{
							areaDescription+="-"+InventoryItem.GetLootMetatypeDescription(metatype)+"\n";
						}
						areaDescription+="Enemies: "+regionalEncounter.enemyDescription+"\n";
						
						//if (isHive) {areaDescription+="\nHive";}
						areaDescription+="\nExploration threat: "+CalculateThreatLevel(0);
						areaDescription+="\nRest threat:"+GetCampingThreat();
					}
					areaDescription+="\nTemperature: "+GetTemperature();
					areaDescription+="\nRequired team size: "+regionalEncounter.minRequiredMembers+"-"+regionalEncounter.maxAllowedMembers;
					//if (PartyManager.mainPartyManager.selectedMembers.Count>0)

				}
				else {textExists=false;}
			}
		}
		if (PartyManager.mainPartyManager.selectedMembers.Count>0)
		{
			MapRegion cursorRegion=PartyManager.mainPartyManager.selectedMembers[0].currentRegion;
			if (cursorRegion.connections.ContainsKey(this))
			{
				if (textExists) areaDescription+="\n";
				textExists=true;
				areaDescription+="Move cost:"+cursorRegion.connections[this].moveCost;
				if (cursorRegion.connections[this].isIntercity) areaDescription+=" gas or 100 fatigue";
				else areaDescription+=" fatigue";
				
			}
		}
		
		if (textExists)	TooltipManager.main.CreateTooltip(areaDescription,this.transform);
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		TooltipManager.main.StopAllTooltips();
	}

	#endregion*/

	public void ShowRegionTooltip()
	{
		bool textExists=true;
		string areaDescription="";
		if (!discovered) {areaDescription="Undiscovered";}
		else 
		{
			/*
			if (hasHorde)
			{
				areaDescription+=hordeEncounter.lootDescription;
			}
			else*/
			{
				if (hasEncounter)
				{
					if (!scouted) {areaDescription="Not scouted";}
					else
					{
						areaDescription+=regionalEncounter.lootDescription+"\n\n";
						//Describe all potential loot
						areaDescription+="May contain:\n";
						foreach (InventoryItem.LootMetatypes metatype in regionalEncounter.chestTypes.probabilities.Keys)
						{
							areaDescription+="-"+InventoryItem.GetLootMetatypeDescription(metatype)+"\n";
						}
						areaDescription+="Enemies: "+regionalEncounter.enemyDescription+"\n";
						
						//if (isHive) {areaDescription+="\nHive";}
						areaDescription+="\nExploration threat: "+CalculateThreatLevel(0);
						areaDescription+="\nRest threat:"+GetCampingThreat();
					}
					areaDescription+="\nTemperature: "+GetTemperature();
					areaDescription+="\nRequired team size: "+regionalEncounter.minRequiredMembers+"-"+regionalEncounter.maxAllowedMembers;
					//if (PartyManager.mainPartyManager.selectedMembers.Count>0)

				}
				else {textExists=false;}
			}
		}
		if (PartyManager.mainPartyManager.selectedMembers.Count>0)
		{
			MapRegion cursorRegion=PartyManager.mainPartyManager.selectedMembers[0].currentRegion;
			if (cursorRegion.connections.ContainsKey(this))
			{
				if (textExists) areaDescription+="\n";
				textExists=true;
				areaDescription+="Move cost:"+cursorRegion.connections[this].moveCost;
				if (cursorRegion.connections[this].isIntercity) areaDescription+=" gas or 100 fatigue";
				else areaDescription+=" fatigue";
				
			}
		}
		
		if (textExists)	TooltipManager.main.CreateTooltip(areaDescription,this.transform);
	}

	public void ShowCampTooltip()
	{
		string tooltipText="You have secured a hideout in this area.\n";
		tooltipText+="You can forify, burn fuel and craft items here.\n";
		tooltipText+="Resting here restores more fatigue";
		TooltipManager.main.CreateTooltip(tooltipText,campToken.transform);
	}

	public void ShowCarTooltip()
	{
		string tooltipText="Your car is parked here";
		tooltipText+="\nThe car allows you to travel to other towns using gas";
		tooltipText+="\nWhen you travel, all items in this area will be moved with you";
		TooltipManager.main.CreateTooltip(tooltipText,carToken.transform);
	}

	public void StopTooltips()
	{
		TooltipManager.main.StopAllTooltips();
	}

	/*
	void OnMouseEnter()
	{
		//TooltipManager.main.CreateTooltip("Map thing",this.transform,true);
	}*
	
	void OnMouseOver()
	{
		drawMouseoverText=false;//true;
		
	}
	
	void OnMouseExit()
	{
		drawMouseoverText=false;
	}
	
	void OnGUI()
	{
		if (drawMouseoverText && !EventSystem.current.IsPointerOverGameObject())
		{
			float height=60;
			Vector3 myScreenPos=Camera.main.WorldToScreenPoint(transform.position);
			bool textExists=true;
			string areaDescription="";
			if (!discovered) {areaDescription="Undiscovered";}
			else 
			{
				if (hasHorde)
				{
					areaDescription+=hordeEncounter.lootDescription;
				}
				else
				{
					if (hasEncounter)
					{
						if (!scouted) {areaDescription="Not scouted";}
						else
						{
							areaDescription+=regionalEncounter.lootDescription+"\n";
							areaDescription+="Enemies: "+regionalEncounter.enemyDescription+"\n";
							areaDescription+="Ambush threat: "+threatLevel;
							//if (isHive) {areaDescription+="\nHive";}
						}
					}
					else {textExists=false;}
				}
			}
			
			if (textExists)
			{
				Rect textRect=new Rect(myScreenPos.x+50,Screen.height-myScreenPos.y-height*0.5f,200,height);
				GUI.Box(textRect,areaDescription);
			}
		}	
	}*/
}
