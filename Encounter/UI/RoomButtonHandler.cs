using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoomButtonHandler : MonoBehaviour {

	public int roomX;
	public int roomY;
	public Vector2 GetRoomCoords() {return new Vector2(roomX,roomY);}
	
	public GameObject lootToken;
	public GameObject lockToken;
	public GameObject exitToken;
	
	public Transform itemsGroup;
	public Transform actorsGroup;
	public Image floorItemPrefab;
	
	Dictionary<InventoryItem, GameObject> floorItemTokens=new Dictionary<InventoryItem, GameObject>();
	
	/*
	public bool isWall
	{
		get {return _isWall;}
		set 
		{
			_isWall=value;
			UpdateVisuals();
		}
	}
	bool _isWall;
	
	public bool isExit
	{
		get {return _isExit;}
		set 
		{
			if (_isExit!=value)
			{
				_isExit=value;
				UpdateVisuals();
			}
		}
	}
	public bool _isExit;
	*/
	bool isWall;
	bool isExit;
	bool isEntrance;
	bool isVisible;
	bool isDiscovered;
	bool hasEnemies;
	bool hasLoot;
	bool lootIsLocked;
	/*
	public bool isVisible
	{
		get {return _isVisible;}
		set
		{
			if (_isVisible!=value)
			{
				_isVisible=value;
				UpdateVisuals();
			}
		}
	}
	
	public bool _isVisible;
	
	public bool hasEnemies
	{
		get {return _hasEnemies;}
		set 
		{
			if (_hasEnemies!=value)
			{
				_hasEnemies=value;
				//UpdateVisuals();
			}
		}
		
	}
	public bool _hasEnemies;
	
	
	public bool hasLoot
	{
		get {return _hasLoot;}
		set 
		{
			if (_hasLoot!=value)
			{
				_hasLoot=value;
				UpdateVisuals();
			}
		}
		
	}
	public bool _hasLoot;
	
	public bool lootIsLocked
	{
		get {return _lootIsLocked;}
		set 
		{
			if (_lootIsLocked!=value)
			{
				_lootIsLocked=value;
				UpdateVisuals();
			}
		}
		
	}
	bool _lootIsLocked;
	*/
	
	public EncounterRoom assignedRoom=null;
	
	public Transform enemiesGroup;
	public Transform membersGroup;

	
	public void AttachEnemyToken(Transform tokenTransform)
	{
		enemiesGroup.gameObject.SetActive(true);
		//If an enemy moves into a room with no members present - center the token, else - set up token anchoring for battle mode
		if (membersGroup.childCount==0) 
		{
			membersGroup.gameObject.SetActive(false);
			//enemiesGroup.GetComponent<VerticalLayoutGroup>().childAlignment=TextAnchor.MiddleCenter;
		}
		else
		{
			//enemiesGroup.GetComponent<VerticalLayoutGroup>().childAlignment=TextAnchor.LowerCenter;
			//membersGroup.GetComponent<VerticalLayoutGroup>().childAlignment=TextAnchor.UpperCenter;
		}
		tokenTransform.SetParent(enemiesGroup,false);
	}
	public void AttachMemberToken(Transform tokenTransform)
	{
		membersGroup.gameObject.SetActive(true);
		//If a member moves into a room with no enemies present - center the token, else - set up token anchoring for battle mode
		if (enemiesGroup.childCount==0) 
		{
			enemiesGroup.gameObject.SetActive(false);
			//membersGroup.GetComponent<VerticalLayoutGroup>().childAlignment=TextAnchor.MiddleCenter;
		}
		else
		{
			//enemiesGroup.GetComponent<VerticalLayoutGroup>().childAlignment=TextAnchor.LowerCenter;
			//membersGroup.GetComponent<VerticalLayoutGroup>().childAlignment=TextAnchor.UpperCenter;
		}
		tokenTransform.SetParent(membersGroup,false);
	}
	
	public void AssignRoom(EncounterRoom newRoom)
	{
		assignedRoom=newRoom;
		//because unassigned bool==false, this is necessary
		isWall=assignedRoom.isWall;
		isExit=assignedRoom.isExit;
		isDiscovered=assignedRoom.isDiscovered;
		hasEnemies=assignedRoom.hasEnemies;
		hasLoot=assignedRoom.hasLoot;
		lootIsLocked=assignedRoom.lootIsLocked;
		
		//this is FOR DEBUG PURPOSES ONLY!!!
		roomX=newRoom.xCoord;
		roomY=newRoom.yCoord;
		
		//isVisible=assignedRoom.isVisible;
		GetComponent<Button>().onClick.AddListener(()=>EncounterCanvasHandler.main.RoomClicked(this));
		
		foreach (InventoryItem item in newRoom.floorItems) {AddFloorItem(item);}
	}
	
	public void SetVisibility(bool visible)
	{
		isVisible=visible;
		if (isVisible) 
		{
			assignedRoom.isDiscovered=true;
			isDiscovered=true;
		}
		UpdateVisuals();
	}
	
	public int AttackEnemyInRoom(int damage, EncounterEnemy attackedEnemy, bool isRanged)
	{
		return assignedRoom.DamageEnemy(damage,attackedEnemy,isRanged);
	}
	
	//These methods are the go-between to external scripts and assigned EncounterRoom
	public void MoveEnemyInRoom(EncounterEnemy enemy)
	{
		assignedRoom.MoveEnemyIn(enemy);
		if (assignedRoom.hasEnemies!=hasEnemies) 
		{
			hasEnemies=assignedRoom.hasEnemies;
			UpdateVisuals();
		}
	}
	public void MoveEnemyOutOfRoom(EncounterEnemy enemy)
	{
		assignedRoom.MoveEnemyOut(enemy);
		if (assignedRoom.hasEnemies!=hasEnemies) 
		{
			hasEnemies=assignedRoom.hasEnemies;
			UpdateVisuals();
		}
	}
	
	public void ExitTokenClicked()
	{
		EncounterCanvasHandler.main.ExitClicked(this);
	}
	
	public void LockTokenClicked()
	{
		EncounterCanvasHandler.main.BashClicked(this);
	}
	
	public void BashLock(int bashStrength)
	{
		assignedRoom.BashLock(bashStrength);
		if (assignedRoom.lootIsLocked==false){lootIsLocked=assignedRoom.lootIsLocked;}
		UpdateVisuals();
	}
	
	public void LootTokenClicked()
	{
		EncounterCanvasHandler.main.LootClicked(this);
	}
	
	public void LootRoom()
	{
		//Compiler made me assign this
		InventoryItem.LootItems itemType=InventoryItem.LootItems.Ammo;
		if (assignedRoom.LootRoom(out itemType))
		{
			InventoryItem newItem=InventoryItem.GetLootingItem(itemType);
			assignedRoom.AddFloorItem(newItem);
			AddFloorItem(newItem);
		}
		if (assignedRoom.hasLoot!=hasLoot) 
		{
			hasLoot=assignedRoom.hasLoot;
			UpdateVisuals();
		}
	}
	
	public void PickUpFloorItem(InventoryItem item)
	{
		assignedRoom.RemoveFloorItem(item);
		RemoveFloorItem(item);
	}
	
	public void DropItemOnFloor(InventoryItem item)
	{
		assignedRoom.AddFloorItem(item);
		AddFloorItem(item);
	}
	
	void AddFloorItem(InventoryItem item)
	{
		Image newFloorItem=Instantiate(floorItemPrefab);
		newFloorItem.sprite=item.GetItemSprite();
		newFloorItem.transform.SetParent(itemsGroup,false);
		newFloorItem.GetComponent<Button>().onClick.AddListener(()=>
		{
			EncounterCanvasHandler encounterHandler=EncounterCanvasHandler.main;
			if (GetRoomCoords()==encounterHandler.memberCoords[encounterHandler.selectedMember] && encounterHandler.selectedMember.CanPickUpItem())
			{
				encounterHandler.selectedMember.carriedItems.Add(item);
				PickUpFloorItem(item);
			} 
		}
		);
		
		floorItemTokens.Add (item,newFloorItem.gameObject);
	}
	void RemoveFloorItem(InventoryItem item)
	{
		GameObject.Destroy(floorItemTokens[item]);
		floorItemTokens.Remove(item);
	}
	
	//public void LootingTokenPressed() {EncounterCanvasHandler.main.LootingTokenClicked(this);}
	
	void UpdateVisuals()
	{
		if (isWall)
		{
			GetComponent<Button>().image.color=Color.black;
			lootToken.SetActive(false);
			lockToken.SetActive(false);
			exitToken.SetActive(false);
		}
		else
		{
			if (hasLoot) {lootToken.SetActive(true);}
			else lootToken.SetActive(false);
			if (lootIsLocked) 
			{
				lockToken.SetActive(true);
				Text lockTokenStrengthText=lockToken.GetComponentInChildren<Text>();
				if (lockTokenStrengthText!=null) lockTokenStrengthText.text=assignedRoom.lockStrength.ToString();
				lootToken.SetActive(false);
			}
			else lockToken.SetActive(false);
			
			if (!isVisible) 
			{
				//switch off Enemy Group and Member Group to hide enemies in fog of war
				actorsGroup.gameObject.SetActive(false);
				if (!isDiscovered) 
				{
					GetComponent<Button>().image.color=Color.gray;
					itemsGroup.gameObject.SetActive(false);
				}
				else {GetComponent<Button>().image.color=new Color32(172,172,172,255);}
			}
			else
			{
				GetComponent<Button>().image.color=Color.white;
				//Switch on Enemy Group and Member Group
				actorsGroup.gameObject.SetActive(true);
				itemsGroup.gameObject.SetActive(true);
			}	
			if (isExit) {exitToken.SetActive(true);}//GetComponent<Button>().image.color=Color.green;}
		}
	}
	
	/*
	void Update()
	{
		if (assignedRoom!=null) 
		{
			//print ("updating");
			isWall=assignedRoom.isWall;
			isExit=assignedRoom.isExit;
			hasEnemies=assignedRoom.hasEnemies;
			hasLoot=assignedRoom.hasLoot;
			lootIsLocked=assignedRoom.lootIsLocked;
			isVisible=assignedRoom.isVisible;
			
		}
	}*/
}
