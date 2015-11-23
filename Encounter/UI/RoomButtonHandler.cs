using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomButtonHandler : MonoBehaviour, IDropHandler
{

	public int roomX;
	public int roomY;
	public Vector2 GetRoomCoords() {return new Vector2(roomX,roomY);}
	
	public GameObject lootToken;
	public GameObject lockToken;
	public GameObject exitToken;
	
	public Transform itemsGroup;
	public Transform actorsGroup;
	public Image floorItemPrefab;
	public GameObject heardEnemyPrefab;
	public TrapToken trapPrefab;
	public BarricadeToken barricadePrefab;
	
	Dictionary<InventoryItem, GameObject> floorItemTokens=new Dictionary<InventoryItem, GameObject>();
	
	bool isWall;
	bool isExit;
	bool isEntrance;
	bool isVisible;
	bool isWithinHearingRange;
	bool isDiscovered;
	bool hasEnemies;
	bool hasLoot;
	bool lootIsLocked;
	
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
		
		if (assignedRoom.trapInRoom!=null) SetTrap(assignedRoom.trapInRoom,false);
		if (assignedRoom.barricadeInRoom!=null) {SpawnNewBarricadeToken();}
		//this is FOR DEBUG PURPOSES ONLY!!!
		roomX=newRoom.xCoord;
		roomY=newRoom.yCoord;
		
		isVisible=false;
		isWithinHearingRange=false;
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
	
	public void SetHearing(bool inRange)
	{
		isWithinHearingRange=inRange;
	}
	
	public void SpawnNewBarricadeToken()
	{
		BarricadeToken newToken=Instantiate(barricadePrefab);
		newToken.transform.SetParent(this.transform,false);
		newToken.transform.position=transform.position;
		newToken.AssignBarricade(this);
	}
	
	void DespawnBarricadeToken()
	{
		if (GetComponentInChildren<BarricadeToken>()!=null)
		{
			GameObject.Destroy(GetComponentInChildren<BarricadeToken>().gameObject);
		}
	}
	
	public void BashBarricade(int bashStrength)
	{
		assignedRoom.BashBarricade(bashStrength);
		if (assignedRoom.barricadeInRoom==null) {DespawnBarricadeToken();}
		else {GetComponentInChildren<BarricadeToken>().UpdateHealth(assignedRoom.barricadeInRoom.health);}
	}
	
	public int AttackEnemyInRoom(int damage, EncounterEnemy attackedEnemy, bool isRanged)
	{
		return assignedRoom.DamageEnemy(damage,attackedEnemy,isRanged);
	}
	
	//These methods are the go-between to external scripts and assigned EncounterRoom
	public void MoveEnemyInRoom(EncounterEnemy enemy)
	{
		assignedRoom.MoveEnemyIn(enemy);
		//Set off traps
		if (assignedRoom.trapInRoom!=null) 
		{
			assignedRoom.trapInRoom.SetOff();
			//RemoveTrap(assignedRoom.trapInRoom);
			//assignedRoom.trapInRoom=null;
		}
		
		//Do hearing tokens
		if (isWithinHearingRange && !isVisible)
		{
			GameObject newObject=Instantiate(heardEnemyPrefab) as GameObject;
			newObject.transform.SetParent(this.transform,false);
			newObject.transform.position=transform.position;
		}
		
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
	
	public void SetTrap(Trap trap, bool addToAssignedRoom)
	{
		if (addToAssignedRoom) assignedRoom.trapInRoom=trap;
		TrapToken trapToken=Instantiate(trapPrefab);
		trap.assignedToken=trapToken;
		trapToken.transform.SetParent(transform,false);
		trapToken.transform.position=transform.position;
		trapToken.AssignTrap(trap,assignedRoom);
	}
	
	public void RemoveTrap(Trap trap)
	{
		if (assignedRoom.trapInRoom!=trap) {throw new System.Exception("Attempting to remove trap that does not exist in room!");}
		else
		{
			assignedRoom.trapInRoom=null;
			TrapToken myChildTrapToken=GetComponentInChildren<TrapToken>();
			if (myChildTrapToken!=null) GameObject.Destroy(myChildTrapToken.gameObject);
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
				//Hide barricades
				BarricadeToken assignedBarricadeToken=GetComponentInChildren<BarricadeToken>();
				if (assignedBarricadeToken!=null) {assignedBarricadeToken.SetHidden(true);}
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
				//Remove hearing tokens upon entering vision
				HearingTokenHandler attachedHaringToken=GetComponentInChildren<HearingTokenHandler>();
				if (attachedHaringToken!=null)
				{
					attachedHaringToken.DisposeToken();
				}
				//Show barricades
				BarricadeToken assignedBarricadeToken=GetComponentInChildren<BarricadeToken>();
				if (assignedBarricadeToken!=null) {assignedBarricadeToken.SetHidden(false);}
				//if (assignedBarricadeToken!=null) {assignedBarricadeToken.GetComponent<Image>(}
				//Set color
				GetComponent<Button>().image.color=Color.white;
				//Switch on Enemy Group and Member Group
				actorsGroup.gameObject.SetActive(true);
				itemsGroup.gameObject.SetActive(true);
			}	
			if (isExit) {exitToken.SetActive(true);}//GetComponent<Button>().image.color=Color.green;}
		}
	}

	#region IDropHandler implementation

	public void OnDrop (PointerEventData eventData)
	{
		BarricadeToken droppedToken=BarricadeToken.barricadeTokenBeingDragged;
		Vector2 draggingMemberCoords=EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember];
		if (droppedToken!=null)
		{
			/*
			if (Mathf.Abs(droppedToken.assignedRoomButton.roomX-roomX)+Mathf.Abs(droppedToken.assignedRoomButton.roomY-roomY)==1
			    && assignedRoom.barricadeInRoom==null)*/
			//Only permit dropping into rooms adjacent to the player
			float xDiff=Mathf.Abs(draggingMemberCoords.x-roomX);
			float yDiff=Mathf.Abs(draggingMemberCoords.y-roomY);
			if ((xDiff>0 || yDiff>0) && (xDiff<2 && yDiff<2)
			&& !isWall 
			&& assignedRoom.barricadeInRoom==null
			&& !hasEnemies
			&& !EncounterCanvasHandler.main.memberCoords.ContainsValue(GetRoomCoords()))
			{
				assignedRoom.barricadeInRoom=droppedToken.assignedRoomButton.assignedRoom.barricadeInRoom;
				droppedToken.assignedRoomButton.assignedRoom.barricadeInRoom=null;
				droppedToken.transform.SetParent(this.transform,false);
				droppedToken.AssignNewRoomButton(this);
				EncounterCanvasHandler.main.TurnoverSelectedMember();
				EncounterCanvasHandler.main.MakeNoise(GetRoomCoords(),3);
			}
		}
	}

	#endregion
}
