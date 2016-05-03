using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vectrosity;

public class RoomButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler//, IDropHandler
{

	public int roomX;
	public int roomY;

	public float enemySpawnProbability
	{
		get {return _enemySpawnProbability;}
		set 
		{
			_enemySpawnProbability=Mathf.Clamp(value,0,1);
			UpdateVisuals();
		}
	}
	float _enemySpawnProbability=0;

	const float nonredColorDampingMult=0.5f;

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

	public BarricadeToken currentBarricadeToken=null;
	
	Dictionary<InventoryItem, GameObject> floorItemTokens=new Dictionary<InventoryItem, GameObject>();
	
	bool isWall;
	bool isExit;
	bool isEntrance;
	public bool isVisible;
	bool isWithinHearingRange;
	bool isDiscovered;
	//public bool hasEnemies;
	bool hasLoot;
	bool lootIsLocked;
	bool canBarricade;
	
	public EncounterRoom assignedRoom=null;
	
	public Transform enemiesGroup;
	public Transform enemiesGroupBack;
	public Transform enemiesGroupFront;

	public List<EnemyTokenHandler> enemiesInFront=new List<EnemyTokenHandler>();
	public List<EnemyTokenHandler> enemiesInBack=new List<EnemyTokenHandler>();
	public int GetTotalEnemyCount() {return enemiesInFront.Count+enemiesInBack.Count;}
	public List<EnemyTokenHandler> GetAllEnemiesInRoom() 
	{
		List<EnemyTokenHandler> allEnemyTokens=new List<EnemyTokenHandler>();
		allEnemyTokens.AddRange(new List<EnemyTokenHandler>(enemiesInFront));
		allEnemyTokens.AddRange(new List<EnemyTokenHandler>(enemiesInBack));
		return allEnemyTokens;
	}
	public bool RoomHasEnemies() {return (enemiesInFront.Count>0 || enemiesInBack.Count>0);}

	public Transform membersGroup;
	public Transform membersGroupBack;
	public Button moveToBackButton;
	public Transform membersGroupFront;
	public Button moveToFrontButton;

	public List<MemberTokenHandler> membersInFront=new List<MemberTokenHandler>();
	public List<MemberTokenHandler> membersInBack=new List<MemberTokenHandler>();
	public int GetTotalMembersInRoom() {return membersInFront.Count+membersInBack.Count;}

	public Color wallColor;
	
	public VectorUI aimLinePrefab;
	protected static VectorUI aimLine;

	public EnemyTokenHandler SpawnNewEnemyInRoom()
	{
		EnemyTokenHandler newEnemyToken=Instantiate(EncounterCanvasHandler.main.enemyTokenPrefab);
		newEnemyToken.AssignEnemy(EncounterEnemy.GetEnemy(assignedRoom.parentEncounter.encounterEnemyType,GetRoomCoords()));
		return newEnemyToken;
	}

	public void AttachEnemyToken(Transform tokenTransform)
	{
		enemiesGroup.gameObject.SetActive(true);
		//If an enemy moves into a room with no members present - center the token, else - set up token anchoring for battle mode
		UpdateMemberGroup();

		EnemyTokenHandler enemyToken=tokenTransform.GetComponent<EnemyTokenHandler>();

		if (tokenTransform.GetComponent<EnemyTokenHandler>().assignedEnemy.inFront)
		{
			//If enemy is a frontliner, set to front
			tokenTransform.SetParent(enemiesGroupFront,false);
			if (enemiesInBack.Contains(enemyToken)) enemiesInBack.Remove(enemyToken);
			enemiesInFront.Add(enemyToken);
		}
		else
		{
			tokenTransform.SetParent(enemiesGroupBack,false);
			if (enemiesInFront.Contains(enemyToken)) enemiesInFront.Remove(enemyToken);
			enemiesInBack.Add(enemyToken);
		}
	}

	public void EnemyTokenDestroyed(EnemyTokenHandler token)
	{
		if (enemiesInFront.Contains(token)) enemiesInFront.Remove(token);
		if (enemiesInBack.Contains(token)) enemiesInBack.Remove(token);
	}

	public void AttachMemberToken(Transform tokenTransform)
	{
		membersGroup.gameObject.SetActive(true);
		//If a member moves into a room with no enemies present - center the token, else - set up token anchoring for battle mode
		if (enemiesGroupBack.childCount==0 && enemiesGroupFront.childCount==0) 
		{
			enemiesGroup.gameObject.SetActive(false);
			//membersGroup.GetComponent<VerticalLayoutGroup>().childAlignment=TextAnchor.MiddleCenter;
		}
		else
		{
			//enemiesGroup.GetComponent<VerticalLayoutGroup>().childAlignment=TextAnchor.LowerCenter;
			//membersGroup.GetComponent<VerticalLayoutGroup>().childAlignment=TextAnchor.UpperCenter;
		}
		MoveMemberToFront(tokenTransform.GetComponent<MemberTokenHandler>());
	}

	public void MoveMemberToFront(MemberTokenHandler memberToken)
	{
		memberToken.transform.SetParent(membersGroupFront,false);
		memberToken.transform.GetComponent<MemberTokenHandler>().inFront=true;
		if (membersInBack.Contains(memberToken)) membersInBack.Remove(memberToken);
		membersInFront.Add(memberToken);
		SelectedMemberMovesUpdate(memberToken);
	}
	public void MoveMemberToBack(MemberTokenHandler memberToken)
	{
		memberToken.transform.SetParent(membersGroupBack,false);
		memberToken.transform.GetComponent<MemberTokenHandler>().inFront=false;
		if (membersInFront.Contains(memberToken)) membersInFront.Remove(memberToken);
		membersInBack.Add(memberToken);
		SelectedMemberMovesUpdate(memberToken);
	}

	public void SelectedMemberMovesUpdate(MemberTokenHandler selectedMemberHandler)
	{
			if (selectedMemberHandler.currentAllowedMovesCount>0)
			{
				if (selectedMemberHandler.inFront) 
				{	
					moveToFrontButton.gameObject.SetActive(false);
					moveToBackButton.gameObject.SetActive(true);
				}
				else
				{
					moveToFrontButton.gameObject.SetActive(true);
					moveToBackButton.gameObject.SetActive(false);
				}
			}
			else
			{
				moveToFrontButton.gameObject.SetActive(false);
				moveToBackButton.gameObject.SetActive(false);
			}
	}

	public void MemberGone(MemberTokenHandler memberToken)
	{
		if (membersInFront.Contains(memberToken)) membersInFront.Remove(memberToken);
		else if (membersInBack.Contains(memberToken)) membersInBack.Remove(memberToken);
		//if ()
		UpdateMemberGroup();
	}

	public void UpdateMemberGroup()
	{
		if (membersInFront.Count==0 && membersInBack.Count==0 && assignedRoom.barricadeInRoom==null) membersGroup.gameObject.SetActive(false);
		MemberTokenHandler selectedMember=EncounterCanvasHandler.main.memberTokens[EncounterCanvasHandler.main.selectedMember];
		if (!membersInFront.Contains(selectedMember) && !membersInBack.Contains(selectedMember))
		{ 
			moveToBackButton.gameObject.SetActive(false);
			moveToFrontButton.gameObject.SetActive(false);
		}
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
			//hasEnemies=assignedRoom.hasEnemies;
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

			moveToBackButton.onClick.AddListener(()=>EncounterCanvasHandler.main.MoveMemberWithinRoomClicked(this,true));
			moveToFrontButton.onClick.AddListener(()=>EncounterCanvasHandler.main.MoveMemberWithinRoomClicked(this,false));

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
		newToken.transform.SetParent(membersGroupFront,false);
		newToken.transform.position=barricadeGroup.position;
		newToken.AssignBarricade(this);
		currentBarricadeToken=newToken;
	}
	
	void DespawnBarricadeToken()
	{
		if (GetComponentInChildren<BarricadeToken>()!=null)
		{
			GameObject.Destroy(GetComponentInChildren<BarricadeToken>().gameObject);
			currentBarricadeToken=null;
		}
	}
	
	public void BashBarricade(int bashStrength)
	{
		assignedRoom.BashBarricade(bashStrength);
		//BarricadeToken myBarricadeToken=GetComponentInChildren<BarricadeToken>();
		//if (isVisible) EncounterCanvasHandler.main.SendFloatingMessage(bashStrength.ToString(),this.transform,Color.black);
		if (assignedRoom.barricadeInRoom==null) {DespawnBarricadeToken();}
		else {currentBarricadeToken.UpdateHealth(assignedRoom.barricadeInRoom.health);}
	}
	/*
	public int AttackEnemyInRoom(int damage, BodyPart attackedPart, EncounterEnemy attackedEnemy, bool isRanged)
	{
		return assignedRoom.DamageEnemy(damage,attackedPart,attackedEnemy,isRanged);
	}*/
	//Currently unused
	//These methods are the go-between to external scripts and assigned EncounterRoom
	public void MoveEnemyInRoom(EncounterEnemy enemy)
	{
		//assignedRoom.MoveEnemyIn(enemy);
		//Set off traps
		if (assignedRoom.trapInRoom!=null) 
		{
			assignedRoom.trapInRoom.SetOff(enemy);
		}
		/*
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
		}*/
	}
	//Currently unused
	/*
	public void MoveEnemyOutOfRoom(EncounterEnemy enemy)
	{
		assignedRoom.MoveEnemyOut(enemy);

		if (assignedRoom.hasEnemies!=hasEnemies) 
		{
			hasEnemies=assignedRoom.hasEnemies;
			UpdateVisuals();
		}
	}*/
	
	public void SetTrap(Trap trap, bool addToAssignedRoom)
	{
		if (addToAssignedRoom) assignedRoom.trapInRoom=trap;
		TrapToken trapToken=Instantiate(trapPrefab);
		trap.assignedToken=trapToken;
		enemiesGroup.gameObject.SetActive(true);
		trapToken.transform.SetParent(enemiesGroupFront,false);
		trapToken.transform.position=transform.position;
		trapToken.AssignTrap(trap,assignedRoom);
	}
	
	public void RemoveTrap(Trap trap)
	{
		if (assignedRoom.trapInRoom!=trap) {throw new System.Exception("Attempting to remove trap that does not exist in room!");}
		else
		{
			assignedRoom.trapInRoom=null;
			TrapToken myChildTrapToken=enemiesGroupFront.GetComponentInChildren<TrapToken>();
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

			//Find nonred damping (how red the color should be)

			//Color32 roomColor=new Color32(255,nonRedColorComponent,nonRedColorComponent,255);

			if (!isVisible) 
			{
				//Hide barricades
				BarricadeToken assignedBarricadeToken=GetComponentInChildren<BarricadeToken>();
				if (assignedBarricadeToken!=null) {assignedBarricadeToken.SetHidden(true);}
				//switch off Enemy Group and Member Group to hide enemies in fog of war
				actorsGroup.gameObject.SetActive(false); //!!!
				if (!isDiscovered) 
				{
					GetComponent<Button>().image.color=Color.gray;
					itemsGroup.gameObject.SetActive(false);
				}
				else 
				{
					byte baseColor=172;
					byte nonRedColorComponent=(byte)Mathf.RoundToInt(baseColor-baseColor*_enemySpawnProbability*nonredColorDampingMult);
					GetComponent<Button>().image.color=new Color32(baseColor,nonRedColorComponent,nonRedColorComponent,255);
				}
				//Hide traps
				foreach (TrapToken trap in GetComponentsInChildren<TrapToken>())
				{
					trap.GetComponent<Image>().enabled=false;
				}
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
				byte baseColor=255;
				byte nonRedColorComponent=(byte)Mathf.RoundToInt(baseColor-baseColor*_enemySpawnProbability*nonredColorDampingMult);
				GetComponent<Button>().image.color=new Color32(baseColor,nonRedColorComponent,nonRedColorComponent,255);
				//Switch on Enemy Group and Member Group
				actorsGroup.gameObject.SetActive(true);
				itemsGroup.gameObject.SetActive(true);
				//Show traps
				//Hide traps
				foreach (TrapToken trap in GetComponentsInChildren<TrapToken>())
				{
					trap.GetComponent<Image>().enabled=true;
				}
			}
				
			if (isExit) {exitToken.SetActive(true);}//GetComponent<Button>().image.color=Color.green;}
		}
	}
	
	public void StartExitTooltip()
	{
		string tooltipText="Exit";
		if (EncounterCanvasHandler.main.selectedMember!=null)
		{
			Vector2 selectedMemberCoords=EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember];
			if ((selectedMemberCoords-GetRoomCoords()).magnitude==0) tooltipText+="\nClick: escape encounter";
		}
		TooltipManager.main.CreateTooltip(tooltipText,exitToken.transform);
	}
	public void StartLootTooltip()
	{
		
		string tooltipText="Stash";
		if (EncounterCanvasHandler.main.selectedMember!=null)
		{	
			Vector2 selectedMemberCoords=EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember];
			if ((selectedMemberCoords-GetRoomCoords()).magnitude==0) tooltipText+="\nClick: loot the stash";
		}
		TooltipManager.main.CreateTooltip(tooltipText,lootToken.transform);
	}
	public void StartLootBashTooltip()
	{
		string tooltipText="Locked stash";
		if (EncounterCanvasHandler.main.selectedMember!=null)
		{
			Vector2 selectedMemberCoords=EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember];
			if ((selectedMemberCoords-GetRoomCoords()).magnitude==0) 
			tooltipText+="\nClick:"+EncounterCanvasHandler.main.selectedMember.GetMeleeAttackDescription();
		}
		TooltipManager.main.CreateTooltip(tooltipText,lockToken.transform);
	}
	public void StartBarricadeBuildTooltip()
	{
		string tooltipText="Furniture";
		if (EncounterCanvasHandler.main.selectedMember!=null)
		{
			Vector2 selectedMemberCoords=EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember];
			if ((selectedMemberCoords-GetRoomCoords()).magnitude==0) tooltipText+="\nClick: barricade room(3)";
		}
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
		if (isDiscovered && !isWall)
		{
			//Show enemy spawn chance
			string tooltipText="";
			tooltipText+="Enemy encounter chance - "+enemySpawnProbability;
			TooltipManager.main.CreateTooltip(tooltipText,transform);
		}

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
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit(PointerEventData eventData)
	{
		TooltipManager.main.StopAllTooltips();
	}

	#endregion
}
