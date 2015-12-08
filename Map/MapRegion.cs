using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MapRegion : MonoBehaviour
{
	public int xCoord=0;
	public int yCoord=0;
	
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
	
	public GameObject campTokenPrefab;
	
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
	
	public enum ThreatLevels {Low,Medium,High};
	public ThreatLevels threatLevel=ThreatLevels.Low;
	
	public bool hasEncounter=false;
	
	public bool hasCamp=false;
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
	Horde _hordeEncounter;
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
	
	bool drawMouseoverText=false;
	
	void SetSprite()
	{
		if (!_discovered)
		{
			GetComponent<SpriteRenderer>().sprite=undiscoveredLocSprite;
		}
		else
		{
			/*
			if (hasHorde && visible)
			{
				GetComponent<SpriteRenderer>().sprite=hordeSprite;
			}*/
			//else
			{
				if (hasEncounter)
				{
					if (scouted)
					{
						switch (regionalEncounter.encounterAreaType)
						{
							case Encounter.AreaTypes.Hospital: {GetComponent<SpriteRenderer>().sprite=hospitalSprite; break;}
							case Encounter.AreaTypes.Apartment: {GetComponent<SpriteRenderer>().sprite=apartmentSprite; break;}
							case Encounter.AreaTypes.Store: {GetComponent<SpriteRenderer>().sprite=storeSprite; break;}
							case Encounter.AreaTypes.Warehouse:{GetComponent<SpriteRenderer>().sprite=warehouseSprite; break;}
							case Encounter.AreaTypes.Police: {GetComponent<SpriteRenderer>().sprite=policeStationSprite; break;}
							case Encounter.AreaTypes.Endgame: {GetComponent<SpriteRenderer>().sprite=radioStationSprite; break;}
						}
					}
					else
					{
						GetComponent<SpriteRenderer>().sprite=encounterSprite;
					}
				}
				else
				{
					GetComponent<SpriteRenderer>().sprite=emptyLocSprite;
				}
			}
			
			if (isHive) GetComponent<SpriteRenderer>().color=Color.red;
			if (visible)
			{
				if (hasHorde) {GetComponent<SpriteRenderer>().color=Color.red;} 
				else {GetComponent<SpriteRenderer>().color=Color.white;}
			}
			else
			{
				GetComponent<SpriteRenderer>().color=Color.gray;
			}
		}
	}
	
	public void GenerateEncounter(bool isEndgame)
	{
		hasEncounter=true;
		if (isEndgame) 
		{
			regionalEncounter=new Encounter(true);
			threatLevel=ThreatLevels.High;
		}
		else 
		{
			regionalEncounter=new Encounter();
			float threatRoll=Random.value;
			if (threatRoll<1f) threatLevel=ThreatLevels.Low;
			if (threatRoll<0.7f) threatLevel=ThreatLevels.Medium;
			if (threatRoll<0.3f) threatLevel=ThreatLevels.High;
		}
	}
	
	public void GenerateHorde(int emptySigVar)
	{
		if (Random.value<0.1f) 
		{
			MapManager.mainMapManager.AddHorde(new Vector2(xCoord,yCoord),regionalEncounter.encounterEnemyType);
			//GameManager.DebugPrint("New horde added, maxX:");
		}
	}
	
	public void SetUpCamp()
	{
		hasCamp=true;
		GameObject campToken=Instantiate(campTokenPrefab);
		campToken.transform.SetParent(this.transform);//,true);
		campToken.transform.position=this.transform.position;
	}
	
	//void Start() {transform.Rotate(new Vector3(-90,0,0));}
	void Start() {SetSprite();}
	
	void OnMouseDown()
	{
		if (!EventSystem.current.IsPointerOverGameObject()) MapManager.mainMapManager.RegionClicked(this);
	}
	
	void OnMouseOver()
	{
		drawMouseoverText=true;
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
	}
}
