using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EncounterManager : MonoBehaviour {

	/*
	public static EncounterManager mainEncounterManager;
	
	public bool encounterOngoing=false;
	
	//Dictionary<Vector2,EncounterRoom> encounterMap=new Dictionary<Vector2,EncounterRoom>();
	List<EncounterRoomDrawer> encounterRoomObjects=new List<EncounterRoomDrawer>();
	public Encounter currentEncounter=null;
	
	public EncounterRoomDrawer encounterRoomPrefab;
	public GameObject encounterPlayerTokenPrefab;
	
	public bool combatOngoing=false;
	//public PartyMemberToken partyMemberBattleTokenPrefab;
	//Dictionary<PartyMember,PartyMemberToken> battleTokens=new Dictionary<PartyMember,PartyMemberToken>();
	//public StatusEffectDrawer combatStatusTokenPrefab;
	//List<StatusEffectDrawer> statusEffectTokens=new List<StatusEffectDrawer>();
	
	public static int encounterMaxPlayerCount=2;
	
	//public int selectedMemberIndex=0;
	public PartyMember selectedMember=null;
	GameObject encounterPlayerToken=null;
	
	public List<PartyMember> encounterMembers=new List<PartyMember>();
	
	public int encounterPlayerX;
	public int encounterPlayerY;
	
	Rect encounterBoxRect=new Rect(150,100,400,400);
	
	string lastMessage="";
	int messageCount=0;
	
	public EncounterCanvasHandler handler;
	
	public void StartNewEncounter(Encounter newEncounter, List<PartyMember> membersOnMission)
	{
		//print ("starting new encounter");
		encounterOngoing=true;
		//do party bonds
		encounterMembers.Clear();
		encounterMembers.AddRange(membersOnMission.ToArray());
		foreach (PartyMember encounterMember in encounterMembers) {encounterMember.EncounterStartTrigger(encounterMembers);}
		currentEncounter=newEncounter;
		GenerateEncounterObjects(newEncounter);
		//GenerateEncounterPresetObjects();
		
		handler.EnableRender();//handler.enabled=true;
		
		foreach (EncounterRoomDrawer drawer in encounterRoomObjects) 
		{
			if (drawer.roomInfo.isEntrance) 
			{	
				MovePartyToRoom(drawer.roomInfo);
				break;
			}
		}
	}
	
	public void ExitEncounter()
	{
		PartyManager.mainPartyManager.PassTime(1);
		EndEncounter();
	}
	
	void EndEncounter()
	{
		handler.DisableRender();//handler.enabled=false;
		foreach (EncounterRoomDrawer clearedRoomObject in encounterRoomObjects)
		{
			GameObject.Destroy(clearedRoomObject.gameObject);
		}
		GameObject.Destroy(encounterPlayerToken);
		encounterRoomObjects.Clear();
		encounterOngoing=false;
		combatOngoing=false;
	}
	
	void GenerateEncounterObjects(Encounter newEncounter)
	{
		encounterPlayerX=0;
		encounterPlayerY=0;
		
		//int encounterSizeX=newEncounter.encounterSizeX;//3;
		//int encounterSizeY=newEncounter.encounterSizeY;//3;
		
		Vector2 encounterMapTopLeft=Camera.main.ScreenToWorldPoint(new Vector2(Screen.width*0.28f,Screen.height*(1-0.33f)));//Camera.main.ScreenToWorldPoint(new Vector2(encounterBoxRect.x+50,Screen.height-(encounterBoxRect.y+300)));//(GUIUtility.GUIToScreenPoint(new Vector2(encounterBoxRect.x,encounterBoxRect.y)));
		float elementSizeX=20;
		float elementSizeY=20;
		float elementGapX=3;
		float elementGapY=3;

		int i=0;
		int j=0;
		EncounterRoomDrawer exitRoomDrawer=null;
		foreach (EncounterRoom room in newEncounter.encounterMap.Values)
		{
			i=room.yCoord;
			j=room.xCoord;
			
			Vector2 newRoomPos=encounterMapTopLeft+new Vector2((elementSizeX+elementGapX)*j,-(elementSizeY+elementGapY)*i);
			EncounterRoomDrawer newRoomDrawer=Instantiate(encounterRoomPrefab,newRoomPos,Quaternion.identity) as EncounterRoomDrawer;
			newRoomDrawer.SetEncounterRoomInfo(newEncounter.encounterMap[new Vector2(j,i)]);
			encounterRoomObjects.Add (newRoomDrawer);
			
			if (room.isExit)//j==encounterPlayerX && i==encounterPlayerY) 
			{
				//if (encounterPlayerToken!=null) {}
				encounterPlayerX=j;
				encounterPlayerY=i;
				encounterPlayerToken=Instantiate(encounterPlayerTokenPrefab,newRoomDrawer.transform.position+new Vector3(0,0,-2),Quaternion.identity) as GameObject;
			}
		}
	}
	
	public void RoomClicked(EncounterRoom room)//EncounterRoomDrawer roomDrawer)
	{
		int moveDistance=0;	
		//EncounterRoom room=roomDrawer.roomInfo;
		moveDistance=Mathf.Abs(encounterPlayerX-room.xCoord)+Mathf.Abs(encounterPlayerY-room.yCoord);//Mathf.Max(Mathf.Abs(encounterPlayerX-room.xCoord),Mathf.Abs(encounterPlayerY-room.yCoord));
		if (moveDistance<=1 && !currentEncounter.encounterMap[new Vector2(encounterPlayerX,encounterPlayerY)].hasEnemies)
		{
			MovePartyToRoom(room);//roomDrawer);
		}
	}
	
	void MovePartyToRoom(EncounterRoom room)//EncounterRoomDrawer roomDrawer)
	{
		//EncounterRoom room=roomDrawer.roomInfo;
		//Vector3 newTokenPos=roomDrawer.transform.position;
		//newTokenPos.z=encounterPlayerToken.transform.position.z;
		//encounterPlayerToken.transform.position=newTokenPos;
		encounterPlayerX=room.xCoord;
		encounterPlayerY=room.yCoord;
			
		EnemiesMove();
		RevealPartyVision();
		handler.AssignRoom(room);
		//start combat if necessary
		CheckRoomForEnemies();
	}
	
	void CheckRoomForEnemies()
	{
		EncounterRoom room=currentEncounter.encounterMap[new Vector2(encounterPlayerX,encounterPlayerY)];
		if (room.hasEnemies) 
		{
			DisplayNewMessage("You encounter a "+room.enemyInRoom.name+"!");
			combatOngoing=true;
			selectedMember=encounterMembers[0];
			//handler.EnableCombat(encounterMembers);
			//selectedMemberIndex=0;
			
		}
	}
	
	//reveals stuff around current party room
	void RevealPartyVision()
	{
		//apply visibility range
		int visionRange=3;
		if (PartyManager.mainPartyManager.dayTime<9 | PartyManager.mainPartyManager.dayTime>21) 
		{
			visionRange=1;
			foreach (PartyMember member in encounterMembers) 
			{
				if (member.hasLight) 
				{
					visionRange+=1;
					break;
				}
			}
		}
		foreach (EncounterRoom cycledRoom in currentEncounter.encounterMap.Values)
		{
			
			cycledRoom.isVisible=false;
		}
		//Do los-based reveal
		for (int i=-visionRange; i<=visionRange; i++)
		{
			for (int j=-visionRange; j<=visionRange; j++)
			{
				if (Mathf.Abs(i)+Mathf.Abs(j)==visionRange)
				{
					//print ("starting line: ("+encounterPlayerX+";"+encounterPlayerY+")->("+(encounterPlayerX+j)+";"+(encounterPlayerY+i)+")");
					//Bresenham vision line
					List<Vector2> lineCoords=new List<Vector2>();
					BresenhamLines.Line(encounterPlayerX,encounterPlayerY,encounterPlayerX+j,encounterPlayerY+i,(int x, int y)=>
					{
						int storedX=x;
						int storedY=y;
						bool noBlock=true;
						if (!currentEncounter.encounterMap.ContainsKey(new Vector2(x,y))) {noBlock=false;}	
						else 
						{
							if (currentEncounter.encounterMap[new Vector2(x,y)].isWall) {noBlock=false;}
							else {noBlock=true;}
							lineCoords.Add(new Vector2(storedX,storedY));
						}
						//print ("x:"+x);
						//print ("y:"+y);
						return noBlock;
					});
					foreach (Vector2 roomCoords in lineCoords) {currentEncounter.encounterMap[roomCoords].isVisible=true;}
				}
			}
		}
	}
	
	public void SetSelectedMember(PartyMember member)
	{
		selectedMember=member;
	}
	
	public void MemberHit()
	{
		EncounterRoom currentRoom=currentEncounter.encounterMap[new Vector2(encounterPlayerX,encounterPlayerY)];
		string messageStart=null;
		int actualDmg=currentRoom.DamageEnemy(selectedMember.MeleeAttack(),false);//.DamageEnemy(dmg);
		//DisplayNewMessage(messageStart+" for "+actualDmg+"!");
		DisplayNewMessage(selectedMember.name+" hits the "+currentRoom.enemyInRoom.name+" for "+actualDmg+"!");
		//handler.StartDamageNumber(actualDmg);
		if (!currentRoom._hasEnemies) {combatOngoing=false;}
		else {TurnOver();}
	}
	//for waiting out your turn in combat
	public void MemberWait()
	{
		TurnOver();
	}
	
	public void MemberShoot()
	{
		EncounterRoom currentRoom=currentEncounter.encounterMap[new Vector2(encounterPlayerX,encounterPlayerY)];
		int actualDmg=currentRoom.DamageEnemy(selectedMember.RangedAttack(),true);//selectedMember.equippedRangedWeapon.GetDamage(),true);//.DamageEnemy(dmg);
		DisplayNewMessage("You shoot the "+currentRoom.enemyInRoom.name+" for "+actualDmg+"!");
		if (!currentRoom._hasEnemies) {combatOngoing=false;}
		else 
		{
			//handler.StartDamageNumber(actualDmg);
			TurnOver();
		}
	}
	
	public void TurnOver()
	{
		EncounterRoom currentRoom=currentEncounter.encounterMap[new Vector2(encounterPlayerX,encounterPlayerY)];
		currentRoom.enemyInRoom.TurnAction();
	}
	
	public void NoActionsLeft()
	{
		RoundOver();
	}
	
	void RoundOver()
	{
		EncounterRoom room=currentEncounter.encounterMap[new Vector2(encounterPlayerX,encounterPlayerY)];
		
		if (room.hasEnemies)
		{
			EncounterEnemy enemy=room.enemyInRoom;
			enemy.RoundAction();
			
			//Check which players survived after enemy turn
			List<PartyMember> memberBuffer=new List<PartyMember>();
			memberBuffer.AddRange(encounterMembers.ToArray());
			
			foreach (PartyMember member in memberBuffer)
			{
				if (!PartyManager.mainPartyManager.partyMembers.Contains(member)) 
				{encounterMembers.Remove(member);}
			}
			//{	
				
			//}
			if (encounterMembers.Count==0)
			{
				EndEncounter();
			}
		}
		else
		{
			combatOngoing=false;
		}
	}
	
	//used for Wait button callback and other things
	public void EnemiesMove()
	{
		// enemy move block	
		//first - find each room with enemies
		List<EncounterRoom> enemiesRooms=new List<EncounterRoom>();
		foreach (EncounterRoom cycledRoom in currentEncounter.encounterMap.Values)
		{
			if (cycledRoom.hasEnemies) {enemiesRooms.Add (cycledRoom);}
		}
		
		//next - move enemies to other rooms, one by one
		foreach (EncounterRoom enemyRoom in enemiesRooms)
		{
			//a) make sure players did not move in the same room (prevents party and enemy "phasing through" eachother
			if (!(encounterPlayerX==enemyRoom.xCoord && encounterPlayerY==enemyRoom.yCoord))
			{	
			//b) determine rooms available for move
			List<EncounterRoom> availableRooms=new List<EncounterRoom>();
			
			//up
			Vector2 upCords=new Vector2(enemyRoom.xCoord,enemyRoom.yCoord-1);
			if (currentEncounter.encounterMap.ContainsKey(upCords)) 
			{
				EncounterRoom upRoom=currentEncounter.encounterMap[upCords];
				if (!upRoom.hasEnemies && !upRoom.isWall) {availableRooms.Add(upRoom);}
			}
			//down
			Vector2 downCords=new Vector2(enemyRoom.xCoord,enemyRoom.yCoord+1);
			if (currentEncounter.encounterMap.ContainsKey(downCords)) 
			{
				EncounterRoom downRoom=currentEncounter.encounterMap[downCords];
				if (!downRoom.hasEnemies && !downRoom.isWall) {availableRooms.Add(downRoom);}
			}
			//left
			Vector2 leftCords=new Vector2(enemyRoom.xCoord-1,enemyRoom.yCoord);
			if (currentEncounter.encounterMap.ContainsKey(leftCords)) 
			{
				EncounterRoom leftRoom=currentEncounter.encounterMap[leftCords];
				if (!leftRoom.hasEnemies && !leftRoom.isWall) {availableRooms.Add(leftRoom);}
			}
			//right
			Vector2 rightCords=new Vector2(enemyRoom.xCoord+1,enemyRoom.yCoord);
			if (currentEncounter.encounterMap.ContainsKey(rightCords)) 
			{
				EncounterRoom rightRoom=currentEncounter.encounterMap[rightCords];
				if (!rightRoom.hasEnemies && !rightRoom.isWall) {availableRooms.Add(rightRoom);}
			}
			//c) randomly pick one to move to
			if (availableRooms.Count>0 && Random.value<enemyRoom.enemyInRoom.moveChance) 
			{
				EncounterRoom destRoom=availableRooms[Random.Range(0,availableRooms.Count)];
				enemyRoom.hasEnemies=false;
				destRoom.hasEnemies=true;
				destRoom.enemyInRoom=enemyRoom.enemyInRoom;
				enemyRoom.enemyInRoom=null;
			}
			}
		}
	}
	
	public void AddEnemyStatusEffect(StatusEffect effect)
	{
		handler.AddEnemyStatusEffect(effect);
		//StatusEffectDrawer newDrawer=Instantiate(combatStatusTokenPrefab) as StatusEffectDrawer;
		//newDrawer.assignedEffect=effect;
		//statusEffectTokens.Add(newDrawer);
	}
	public void RemoveEnemyStatusEffect(StatusEffect effect)
	{
		handler.RemoveEnemyStatusEffect(effect);
		//StatusEffectDrawer drawer=null;
	}
	
	
	
	//For Loot button
	public void LootRoom()
	{
			EncounterRoom currentRoom=currentEncounter.encounterMap[new Vector2(encounterPlayerX,encounterPlayerY)];
			
			currentRoom.hasLoot=false;
			
			float randomRes=Random.value;
			Encounter.LootItems pickedUpItem=Encounter.LootItems.Ammo; //the compiler made me assign this, it should be unassigned/null
			bool lootFound=false;
			foreach(float chance in currentEncounter.lootChances.Keys)
			{
				if (randomRes<=chance) {pickedUpItem=currentEncounter.lootChances[chance]; lootFound=true;}
			}
			
			if (!lootFound)
			{
				DisplayNewMessage("You find nothing useful");
			}
			else
			{
				switch (pickedUpItem)
				{
				case Encounter.LootItems.Ammo:
				{
					PartyManager.mainPartyManager.ammo+=10;
					DisplayNewMessage("You find some ammo");
					break;
				}
				case Encounter.LootItems.Food:
				{
					//PartyManager.mainPartyManager.foodSupply+=1;
					PartyManager.mainPartyManager.GainItems(new Food());
					DisplayNewMessage("You find some food");
					break;
				}
				case Encounter.LootItems.Medkits:
				{
					PartyManager.mainPartyManager.GainItems(new Medkit());
					DisplayNewMessage("You find a medkit");
					break;
				}
				case Encounter.LootItems.Flashlight:
				{
					PartyManager.mainPartyManager.GainItems(new Flashlight());
					DisplayNewMessage("You find a flashlight");
					break;
				}
				case Encounter.LootItems.ArmorVest:
				{
					PartyManager.mainPartyManager.GainItems(new ArmorVest());
					DisplayNewMessage("You find an armor vest");
					break;
				}
				case Encounter.LootItems.Knife:
				{
					PartyManager.mainPartyManager.GainItems(new Knife());
					DisplayNewMessage("You find a knife");
					break;
				}
				case Encounter.LootItems.Axe:
				{
					PartyManager.mainPartyManager.GainItems(new Axe());
					DisplayNewMessage("You find an axe");
					break;
				}
				case Encounter.LootItems.NineM:
				{
					PartyManager.mainPartyManager.GainItems(new NineM());
					DisplayNewMessage("You find a 9mm pistol");
					break;
				}
				case Encounter.LootItems.AssaultRifle:
				{
					PartyManager.mainPartyManager.GainItems(new AssaultRifle());
					DisplayNewMessage("You find an assault rifle");
					break;
				}
				case Encounter.LootItems.Radio:
				{
					PartyManager.mainPartyManager.GainItems(new Radio());
					DisplayNewMessage("You find a radio");
					break;
				}
				}
			}
			EnemiesMove();
			CheckRoomForEnemies();
	}
	
	public void WaitInRoom()
	{
		EnemiesMove();
		CheckRoomForEnemies();
	}
	
	public void DisplayNewMessage(string message)
	{
		StopCoroutine("AddTimedMessage");
		StartCoroutine("AddTimedMessage",message);
	}
	
	IEnumerator AddTimedMessage(string message)
	{
		lastMessage+=message+"\n";
		while (lastMessage.Contains("\n"))
		{
			handler.SetMessage(lastMessage);
			yield return new WaitForSeconds(1f);
			lastMessage=lastMessage.Remove(0,lastMessage.IndexOf("\n")+1);
		}
		lastMessage="";
		handler.SetMessage(lastMessage);
		//messageCount=0;
		yield break;	
	}
	
	void Start() 
	{
		mainEncounterManager=this;
		GameManager.GameOver+=EndEncounter;
	}*/
}
