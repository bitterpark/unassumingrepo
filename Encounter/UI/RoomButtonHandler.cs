using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vectrosity;

public class RoomButtonHandler : MonoBehaviour, IPointerEnterHandler//, IDropHandler
{

	public int roomX;
	public int roomY;
	public Vector2 GetRoomCoords() {return new Vector2(roomX,roomY);}
	
	public GameObject lootToken;
	public GameObject lockToken;
	public GameObject exitToken;
	public GameObject barricadeBuildToken;
	
	public Transform barricadeGroup;
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
	public bool isVisible;
	bool isWithinHearingRange;
	bool isDiscovered;
	public bool hasEnemies;
	bool hasLoot;
	bool lootIsLocked;
	bool canBarricade;
	
	public EncounterRoom assignedRoom=null;
	
	public Transform enemiesGroup;
	public Transform membersGroup;

	public Color wallColor;
	
	public VectorUI aimLinePrefab;
	protected static VectorUI aimLine;
	
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
		roomX=newRoom.xCoord;
		roomY=newRoom.yCoord;
		if (!newRoom.hideImage)
		{
			assignedRoom=newRoom;
			//because unassigned bool==false, this is necessary
			isWall=assignedRoom.isWall;
			isExit=assignedRoom.isExit;
			isDiscovered=assignedRoom.isDiscovered;
			hasEnemies=assignedRoom.hasEnemies;
			hasLoot=assignedRoom.hasLoot;
			lootIsLocked=assignedRoom.lootIsLocked;
			canBarricade=assignedRoom.canBarricade;
			
			if (assignedRoom.trapInRoom!=null) SetTrap(assignedRoom.trapInRoom,false);
			if (assignedRoom.barricadeInRoom!=null) {SpawnNewBarricadeToken();}
			//this is FOR DEBUG PURPOSES ONLY!!!
			
			
			isVisible=false;
			isWithinHearingRange=false;
			//isVisible=assignedRoom.isVisible;
			GetComponent<Button>().onClick.AddListener(()=>EncounterCanvasHandler.main.RoomClicked(this));
			
			foreach (InventoryItem item in newRoom.floorItems) {AddFloorItem(item);}
		}
		else GetComponent<Image>().enabled=false;
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
	
	void SpawnNewBarricadeToken()
	{
		BarricadeToken newToken=Instantiate(barricadePrefab);
		newToken.transform.SetParent(barricadeGroup,false);
		newToken.transform.position=barricadeGroup.position;
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
		BarricadeToken myBarricadeToken=GetComponentInChildren<BarricadeToken>();
		EncounterCanvasHandler.main.SendFloatingMessage(bashStrength.ToString(),this.transform,Color.black);
		if (assignedRoom.barricadeInRoom==null) {DespawnBarricadeToken();}
		else {myBarricadeToken.UpdateHealth(assignedRoom.barricadeInRoom.health);}
	}
	
	public int AttackEnemyInRoom(int damage, BodyPart attackedPart, EncounterEnemy attackedEnemy, bool isRanged)
	{
		return assignedRoom.DamageEnemy(damage,attackedPart,attackedEnemy,isRanged);
	}
	
	//These methods are the go-between to external scripts and assigned EncounterRoom
	public void MoveEnemyInRoom(EncounterEnemy enemy)
	{
		assignedRoom.MoveEnemyIn(enemy);
		//Set off traps
		if (assignedRoom.trapInRoom!=null) 
		{
			assignedRoom.trapInRoom.SetOff(enemy);
		}
		
		//Do hearing tokens
		if (isWithinHearingRange && !isVisible && GetComponentInChildren<HearingTokenHandler>()==null)
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
	
	public void BarricadingTokenClicked()
	{
		EncounterCanvasHandler.main.BarricadeBuildClicked(this);
	}
	
	public void BarricadeRoom()
	{
		bool spawnToken=false;
		if (assignedRoom.barricadeInRoom==null) spawnToken=true;
		assignedRoom.BuildBarricade();
		//order is important
		if (spawnToken) SpawnNewBarricadeToken();
		GetComponentInChildren<BarricadeToken>().UpdateHealth(assignedRoom.barricadeInRoom.health);
		canBarricade=assignedRoom.canBarricade;
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
		foreach (InventoryItem item in assignedRoom.LootRoom())
		{
			assignedRoom.AddFloorItem(item);
			AddFloorItem(item);
		}
		/*
		if (assignedRoom.LootRoom(out itemType))
		{
			InventoryItem newItem=InventoryItem.GetLootingItem(itemType);
			assignedRoom.AddFloorItem(newItem);
			AddFloorItem(newItem);
		}*/
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
			GetComponent<Button>().image.color=wallColor;
			lootToken.SetActive(false);
			lockToken.SetActive(false);
			exitToken.SetActive(false);
			barricadeBuildToken.SetActive(false);
		}
		else
		{
			if (hasLoot) lootToken.SetActive(true);
			else lootToken.SetActive(false);
			if (lootIsLocked) 
			{
				lockToken.SetActive(true);
				Text lockTokenStrengthText=lockToken.GetComponentInChildren<Text>();
				if (lockTokenStrengthText!=null) lockTokenStrengthText.text=assignedRoom.lockStrength.ToString();
				lootToken.SetActive(false);
			}
			else lockToken.SetActive(false);
			
			if (canBarricade) 
			{
				barricadeBuildToken.SetActive(true);
				Text barricadeMaterialsText=barricadeBuildToken.GetComponentInChildren<Text>();
				if (barricadeMaterialsText!=null) barricadeMaterialsText.text=assignedRoom.barricadeMaterials.ToString();
			}
			else barricadeBuildToken.SetActive(false);
			
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
				HearingTokenHandler attachedHearingToken=GetComponentInChildren<HearingTokenHandler>();
				if (attachedHearingToken!=null)
				{
					attachedHearingToken.DisposeToken();
				}
				//Show barricades
				BarricadeToken assignedBarricadeToken=GetComponentInChildren<BarricadeToken>();
				if (assignedBarricadeToken!=null) 
				{
					assignedBarricadeToken.SetHidden(false);
				}
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
	
	public void StartExitTooltip()
	{
		string tooltipText="Exit";
		Vector2 selectedMemberCoords=EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember];
		if ((selectedMemberCoords-GetRoomCoords()).magnitude==0) tooltipText+="\nClick: escape encounter";
		TooltipManager.main.CreateTooltip(tooltipText,exitToken.transform);
	}
	public void StartLootTooltip()
	{
		string tooltipText="Stash";
		Vector2 selectedMemberCoords=EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember];
		if ((selectedMemberCoords-GetRoomCoords()).magnitude==0) tooltipText+="\nClick: loot the stash";
		TooltipManager.main.CreateTooltip(tooltipText,lootToken.transform);
	}
	public void StartLootBashTooltip()
	{
		string tooltipText="Locked stash";
		Vector2 selectedMemberCoords=EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember];
		if ((selectedMemberCoords-GetRoomCoords()).magnitude==0) tooltipText+="\nClick: bash the lock(1)";
		TooltipManager.main.CreateTooltip(tooltipText,lockToken.transform);
	}
	public void StartBarricadeBuildTooltip()
	{
		string tooltipText="Furniture";
		Vector2 selectedMemberCoords=EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember];
		if ((selectedMemberCoords-GetRoomCoords()).magnitude==0) tooltipText+="\nClick: barricade room(3)";
		TooltipManager.main.CreateTooltip(tooltipText,barricadeBuildToken.transform);
	}
	public void StopTooltip() {TooltipManager.main.StopAllTooltips();}
	/*
	#region IDropHandler implementation

	public void OnDrop (PointerEventData eventData)
	{
		BarricadeToken droppedToken=BarricadeToken.barricadeTokenBeingDragged;
		Vector2 draggingMemberCoords=EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember];
		if (droppedToken!=null)
		{
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

	#endregion*/

	void Update()
	{
		// if selected member is switched, remove aimline
		if (Input.GetKeyDown(KeyCode.Q)) 
		{
			if (aimLine!=null)GameObject.Destroy(aimLine.gameObject);
		}
	}

	#region IPointerEnterHandler implementation
	public void OnPointerEnter (PointerEventData eventData)
	{
		//List<Vector2> lineCoords=new List<Vector2>();
		//print ("Room coords:"+transform.position);
		/*
		Vector3 newPoint;
		RectTransformUtility.ScreenPointToWorldPointInRectangle
		(GetComponent<RectTransform>()
		 ,GetComponent<RectTransform>().rect.center
		 ,Camera.main,
		 out newPoint);
		lineCoords.Add(newPoint);
		RectTransform otherRect=EncounterCanvasHandler.main.roomButtons[EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember]]
		.GetComponent<RectTransform>();
		RectTransformUtility.ScreenPointToWorldPointInRectangle
		(otherRect
		 ,otherRect.rect.center
		 ,Camera.main,
		 out newPoint);
		lineCoords.Add(newPoint);*/
		
		//lineCoords.Add(EncounterCanvasHandler.main.roomButtons[EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember]]
		//.GetComponent<RectTransform>().position);
		//List<Vector2> lineCoords=new List<Vector2>();
		//For brevity
		EncounterCanvasHandler handler=EncounterCanvasHandler.main;
		//Dispose the old aimline
		if (aimLine!=null)GameObject.Destroy(aimLine.gameObject);
		if (handler.memberTokens.ContainsKey(handler.selectedMember))
		{
			MemberTokenHandler currentToken=handler.memberTokens[handler.selectedMember];
			if (currentToken.rangedMode)
			{
				List<Vector2> linePointCoords=new List<Vector2>();
				List<Color32> lineSegmentColors=new List<Color32>();
				Color currentLineColor=Color.black;
				int selectedX=(int)handler.memberCoords[handler.selectedMember].x;
				int selectedY=(int)handler.memberCoords[handler.selectedMember].y;
				int endX=(int)GetRoomCoords().x;
				int endY=(int)GetRoomCoords().y;
				//Second - see if any walls are blocking member (for ranged attacks)
				//if (attackRange>0)
				//{
					BresenhamLines.Line(selectedX,selectedY,endX,endY,(int x, int y)=>
					{
						//Set color for the preceeding line segment
						if (handler.roomButtons[new Vector2(x,y)].assignedRoom.isWall) currentLineColor=Color.red;
						lineSegmentColors.Add(currentLineColor);
						//Set point coords
						linePointCoords.Add((Vector2)handler.roomButtons[new Vector2(x,y)].transform.position);
						return true;
						//bool visionClear=true;
						//if (handler.currentEncounter.encounterMap[new Vector2(x,y)].isWall) {enemyReachable=false;}
						//return enemyReachable;
					});
				//}
				//If there are enough points - plot the path of the shooting count algorithm
				if (linePointCoords.Count>1)
				{
					aimLine=Instantiate(aimLinePrefab);
					VectorLine aimVectorProperties=new VectorLine("Aim Vector",linePointCoords,2f);
					//This makes sure the number of entries in color array does not exceed the number of line segments
					//(Each new point sets color for the previous segment, first point's has no following segment and must be removed)
					lineSegmentColors.RemoveAt(0);
					aimVectorProperties.lineType=LineType.Continuous;
					aimVectorProperties.SetColors(lineSegmentColors);
					aimLine.AssignVectorLine(aimVectorProperties,transform.parent,true);//"Aim Vector",lineCoords,2f,Color.black,transform.parent);
				}
			}
		}
		/*
		lineCoords.Add((Vector2)transform.position);//GetComponent<RectTransform>().rect.position);
		lineCoords.Add((Vector2)EncounterCanvasHandler.main.roomButtons[EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember]]
		.transform.position);*///GetComponent<RectTransform>().rect.position);
		//lineCoords.Add(Vector3.zero);
		//testLine=new VectorLine("testLine",lineCoords,3f);
		//testLine.color=Color.black;
		/*
		VectorUI newThing=Instantiate(testThing);
		newThing.AssignVectorLine(testLine,transform.parent);
		myThing=newThing;*/
	}

	#endregion
}
