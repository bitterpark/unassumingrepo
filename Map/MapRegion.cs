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
			/*
			if (_scouted)
			{
				if (hasEncounter)
				{
					switch (regionalEncounter.encounterLootType)
					{
						case Encounter.LootTypes.Apartment: {GetComponent<SpriteRenderer>().sprite=apartmentSprite; break;}
						case Encounter.LootTypes.Store: {GetComponent<SpriteRenderer>().sprite=storeSprite; break;}
						case Encounter.LootTypes.Warehouse:{GetComponent<SpriteRenderer>().sprite=warehouseSprite; break;}
					}
				}
			}*/
		}
	}
	
	public bool _scouted=false;
	
	public bool hasEncounter
	{
		get {return _hasEncounter;}
		set 
		{
			_hasEncounter=value;
			if (_hasEncounter) 
			{
				//GetComponent<SpriteRenderer>().sprite=encounterSprite;//.material.color=Color.blue;
				regionalEncounter=new Encounter();
			}
			//else {GetComponent<SpriteRenderer>().sprite=emptyLocSprite;}//.material.color=Color.gray;}
			SetSprite();
		}
	}
	public bool _hasEncounter=false;
	
	public Encounter regionalEncounter;
	
	void SetSprite()
	{
		if (!_discovered)
		{
			GetComponent<SpriteRenderer>().sprite=undiscoveredLocSprite;
		}
		else
		{
			if (_scouted)
			{
				if (hasEncounter)
				{
					switch (regionalEncounter.encounterLootType)
					{
						case Encounter.LootTypes.Hospital: {GetComponent<SpriteRenderer>().sprite=hospitalSprite; break;}
						case Encounter.LootTypes.Apartment: {GetComponent<SpriteRenderer>().sprite=apartmentSprite; break;}
						case Encounter.LootTypes.Store: {GetComponent<SpriteRenderer>().sprite=storeSprite; break;}
						case Encounter.LootTypes.Warehouse:{GetComponent<SpriteRenderer>().sprite=warehouseSprite; break;}
						case Encounter.LootTypes.Police: {GetComponent<SpriteRenderer>().sprite=policeStationSprite; break;}
					}
				}
			}
			else
			{
				if (_hasEncounter) 
				{
					GetComponent<SpriteRenderer>().sprite=encounterSprite;//.material.color=Color.blue;
					regionalEncounter=new Encounter();
				}
				else {GetComponent<SpriteRenderer>().sprite=emptyLocSprite;}
			}
		}
	}
	
	//void Start() {transform.Rotate(new Vector3(-90,0,0));}
	void Start() {SetSprite();}
	
	void OnMouseDown()
	{
		if (!EventSystem.current.IsPointerOverGameObject()) MapManager.mainMapManager.RegionClicked(this);
	}	
}
