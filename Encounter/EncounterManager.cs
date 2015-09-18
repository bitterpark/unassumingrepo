using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EncounterManager : MonoBehaviour {

	public static EncounterManager mainEncounterManager;
	
	public bool encounterOngoing=false;
	
	//Dictionary<Vector2,EncounterRoom> encounterMap=new Dictionary<Vector2,EncounterRoom>();
	List<EncounterRoomDrawer> encounterRoomObjects=new List<EncounterRoomDrawer>();
	Encounter currentEncounter=null;
	
	public EncounterRoomDrawer encounterRoomPrefab;
	public GameObject encounterPlayerTokenPrefab;
	
	public bool combatOngoing=false;
	public PartyMemberToken partyMemberBattleTokenPrefab;
	Dictionary<PartyMember,PartyMemberToken> battleTokens=new Dictionary<PartyMember,PartyMemberToken>();
	public StatusEffectDrawer combatStatusTokenPrefab;
	List<StatusEffectDrawer> statusEffectTokens=new List<StatusEffectDrawer>();
	
	//public int selectedMemberIndex=0;
	public PartyMember selectedMember=null;
	GameObject encounterPlayerToken=null;
	
	public int encounterPlayerX;
	public int encounterPlayerY;
	
	Rect encounterBoxRect=new Rect(150,100,400,400);
	
	string lastMessage="";
	int messageCount=0;
	
	public GameObject apartmentPreset;
	
	public void StartNewEncounter(MapRegion encounterRegion)
	{
		encounterOngoing=true;
		GenerateEncounterObjects(encounterRegion.regionalEncounter);
		//GenerateEncounterPresetObjects();
		currentEncounter=encounterRegion.regionalEncounter;
		
		foreach (EncounterRoomDrawer drawer in encounterRoomObjects) 
		{
			if (drawer.roomInfo.isExit) 
			{	
				RoomClicked(drawer); 
				break;
			}
		}
		
	}
	
	void EndEncounter()
	{
		foreach (EncounterRoomDrawer clearedRoomObject in encounterRoomObjects)
		{
			GameObject.Destroy(clearedRoomObject.gameObject);
		}
		GameObject.Destroy(encounterPlayerToken);
		encounterRoomObjects.Clear();
		encounterOngoing=false;
	}
	
	void GenerateEncounterObjects(Encounter newEncounter)
	{
		encounterPlayerX=0;
		encounterPlayerY=0;
		
		int encounterSizeX=newEncounter.encounterSizeX;//3;
		int encounterSizeY=newEncounter.encounterSizeY;//3;
		
		Vector2 encounterMapTopLeft=Camera.main.ScreenToWorldPoint(new Vector2(encounterBoxRect.x+50,Screen.height-(encounterBoxRect.y+300)));//(GUIUtility.GUIToScreenPoint(new Vector2(encounterBoxRect.x,encounterBoxRect.y)));
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
	
	public void RoomClicked(EncounterRoomDrawer roomDrawer)
	{
		MovePartyToRoom(roomDrawer);
		//apply visibility range
		int visionRange=3;
		if (PartyManager.mainPartyManager.dayTime<9 | PartyManager.mainPartyManager.dayTime>21) 
		{
			visionRange=1;
			foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers) 
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
			if (Mathf.Abs(encounterPlayerX-cycledRoom.xCoord)<=visionRange && Mathf.Abs(encounterPlayerY-cycledRoom.yCoord)<=visionRange) 
			{cycledRoom.isVisible=true;}
			else {cycledRoom.isVisible=false;}
		}
	}
	
	void MovePartyToRoom(EncounterRoomDrawer roomDrawer)
	{
		int moveDistance=0;	
		EncounterRoom room=roomDrawer.roomInfo;
		moveDistance=Mathf.Abs(encounterPlayerX-room.xCoord)+Mathf.Abs(encounterPlayerY-room.yCoord);//Mathf.Max(Mathf.Abs(encounterPlayerX-room.xCoord),Mathf.Abs(encounterPlayerY-room.yCoord));
		if (moveDistance<=1 && !currentEncounter.encounterMap[new Vector2(encounterPlayerX,encounterPlayerY)].hasEnemies)
		{
			Vector3 newTokenPos=roomDrawer.transform.position;
			newTokenPos.z=encounterPlayerToken.transform.position.z;
			encounterPlayerToken.transform.position=newTokenPos;
			encounterPlayerX=room.xCoord;
			encounterPlayerY=room.yCoord;
			
			EnemiesMove();
			//start combat if necessary
			if (room.hasEnemies) 
			{
				DisplayNewMessage("You encounter a "+room.enemyInRoom.name+"!");
				//selectedMemberIndex=0;
				selectedMember=PartyManager.mainPartyManager.partyMembers[0];
			}
		}
	}
	
	public void PartyTokenClicked(PartyMember member)
	{
		//selectedMemberIndex=memberIndex;
		selectedMember=member;
		foreach (PartyMemberToken token in battleTokens.Values) 
		{
			if (token.drawnMember==selectedMember) {token.selected=true;} else {token.selected=false;}
		}
	}
	/*
	public int DamagePlayerCharacter(PartyMember member, int dmg)
	{
		//PartyManager.mainPartyManager.DamagePartyMember(member,dmg);
		return member.TakeDamage(dmg);
	}*/
	
	void TurnOver(EncounterRoom room)
	{
		if (room.hasEnemies)
		{
			EncounterEnemy enemy=room.enemyInRoom;
			enemy.TurnAction();
			
			//Check which players survived after enemy turn
			if (PartyManager.mainPartyManager.partyMembers.Count==0)
			{
				foreach (PartyMemberToken token in battleTokens.Values)
				{
					GameObject.Destroy(token.gameObject);
				}
				combatOngoing=false;
				encounterOngoing=false;
			}
			else
			{	
				PartyMemberToken[] tokenCopyBuffer=new PartyMemberToken[battleTokens.Values.Count];
				battleTokens.Values.CopyTo(tokenCopyBuffer,0);
			
				foreach (PartyMemberToken token in tokenCopyBuffer) 
				{
					if (!PartyManager.mainPartyManager.partyMembers.Contains(token.drawnMember))
					{
						battleTokens.Remove(token.drawnMember);
						GameObject.Destroy(token.gameObject);
					}
					else
					{
						token.turnTaken=false;
						selectedMember=PartyManager.mainPartyManager.partyMembers[0];
						if (token.drawnMember==selectedMember) {token.selected=true;} else {token.selected=false;}
					}
				}
			}
		}
	}
	
	void EnemiesMove()
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
				if (!upRoom.hasEnemies) {availableRooms.Add(upRoom);}
			}
			//down
			Vector2 downCords=new Vector2(enemyRoom.xCoord,enemyRoom.yCoord+1);
			if (currentEncounter.encounterMap.ContainsKey(downCords)) 
			{
				EncounterRoom downRoom=currentEncounter.encounterMap[downCords];
				if (!downRoom.hasEnemies) {availableRooms.Add(downRoom);}
			}
			//left
			Vector2 leftCords=new Vector2(enemyRoom.xCoord-1,enemyRoom.yCoord);
			if (currentEncounter.encounterMap.ContainsKey(leftCords)) 
			{
				EncounterRoom leftRoom=currentEncounter.encounterMap[leftCords];
				if (!leftRoom.hasEnemies) {availableRooms.Add(leftRoom);}
			}
			//right
			Vector2 rightCords=new Vector2(enemyRoom.xCoord+1,enemyRoom.yCoord);
			if (currentEncounter.encounterMap.ContainsKey(rightCords)) 
			{
				EncounterRoom rightRoom=currentEncounter.encounterMap[rightCords];
				if (!rightRoom.hasEnemies) {availableRooms.Add(rightRoom);}
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
		StatusEffectDrawer newDrawer=Instantiate(combatStatusTokenPrefab) as StatusEffectDrawer;
		newDrawer.assignedEffect=effect;
		statusEffectTokens.Add(newDrawer);
	}
	public void RemoveEnemyStatusEffect(StatusEffect effect)
	{
		StatusEffectDrawer drawer=null;
		foreach (StatusEffectDrawer iterDrawer in statusEffectTokens) 
		{
			if (iterDrawer.assignedEffect==effect) {drawer=iterDrawer; break;}
		}
		statusEffectTokens.Remove(drawer);
		GameObject.Destroy(drawer.gameObject);
	}
	
	
	void DisplayEncounter(EncounterRoom currentRoom)
	{
		
		Rect encounterTextRect=new Rect(200,150,300,40);
		Rect encounterMessageRect=new Rect(200,200,300,40);
		//Rect encounterMessageTwoRect=new Rect (200,240,300,40);
		
		string encountertext=currentRoom.description;//encounterMap[new Vector2(encounterPlayerX,encounterPlayerY)].description;
		
		if (!currentRoom.hasEnemies) 
		{
			DisplayEncounterExploration(currentRoom);
			combatOngoing=false;
		}
		else 
		{
			DisplayEncounterCombat(currentRoom);
			combatOngoing=true;
		}
		
		GUI.Box(encounterTextRect,encountertext);
		GUI.Box (encounterMessageRect,lastMessage);
		//GUI.Box (encounterMessageTwoRect,lastMessage);
	}
	
	void DisplayEncounterExploration(EncounterRoom currentRoom)
	{
		Rect encounterLootButton=new Rect(200,250,100,30);
		Rect waitButton=new Rect(200,285,100,30);
		//clears battle tokens once on end of battle
		if (battleTokens.Count>0) 
		{
			foreach (PartyMemberToken token in battleTokens.Values) {GameObject.Destroy(token.gameObject);}
			battleTokens.Clear();
		}
		if (statusEffectTokens.Count>0)
		{
			foreach (StatusEffectDrawer token in statusEffectTokens) {GameObject.Destroy(token.gameObject);}
			statusEffectTokens.Clear();
		}
		
		if (currentRoom.isExit)
		{
			if (GUI.Button(encounterLootButton,"Exit")) {EndEncounter();}
		}
		else
		{
			if (currentRoom.hasLoot)
			{
				if (GUI.Button(encounterLootButton,"Loot")) 
				{
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
						}
					}
					EnemiesMove();
				}
			}
		}
		if (GUI.Button(waitButton,"Wait")) {EnemiesMove();}
	}
	
	void DisplayEncounterCombat(EncounterRoom currentRoom)
	{
		//combat buttons
		Rect punchButton=new Rect(300,290,100,30);
		Rect shootButton=new Rect(300,330,100,30);
		Rect waitButton=new Rect (300,370,100,30);
		//enemy indicators
		Rect enemyName=new Rect(400,290,100,30);
		Rect enemyHp=new Rect(400,330,100,30);
		if (statusEffectTokens.Count>0)
		{
			Vector3 newTokenPos=Camera.main.ScreenToWorldPoint(new Vector2(enemyName.x+enemyName.width,Screen.height-(enemyName.y+enemyName.height*0.5f)));
			newTokenPos.z=-2;
			foreach (StatusEffectDrawer drawer in statusEffectTokens)
			{
				newTokenPos.x+=15;
				drawer.transform.position=newTokenPos;
			}
		}
		
		float partyMemberStartX=250;
		float partyMemberStartY=260;
		
		Rect selectedPartyMemberName=new Rect(200,290,100,30);
		Rect selectedPartyMemberHp=new Rect(200,330,100,30);
		
		//first create tokens
		if (battleTokens.Count==0)  
		{
			selectedMember=PartyManager.mainPartyManager.partyMembers[0];//Index=0;
			float partyMemberXGap=20;
			for (int i=0; i<PartyManager.mainPartyManager.partyMembers.Count; i++)
			{
				Vector2 tokensTopLeft=Camera.main.ScreenToWorldPoint(new Vector2(partyMemberStartX+partyMemberXGap*i,Screen.height-(partyMemberStartY)));
				PartyMemberToken newToken=Instantiate(partyMemberBattleTokenPrefab,tokensTopLeft,Quaternion.identity) as PartyMemberToken;
				newToken.drawnMember=PartyManager.mainPartyManager.partyMembers[i];//Index=i;//drawnMemberIndex;
				newToken.turnTaken=false;
				if (newToken.drawnMember==selectedMember) {newToken.selected=true;}
				//if (i==0) {newToken.selected=true;}
				battleTokens.Add(newToken.drawnMember,newToken);
			}
		}
		
		//show selected party member's stats
		//PartyMember selectedMember=PartyManager.mainPartyManager.partyMembers[selectedMemberIndex];
		GUI.Box(selectedPartyMemberName,selectedMember.name);
		GUI.Box (selectedPartyMemberHp,"Health:"+selectedMember.health);
		
		//current enemy stats
		GUI.Box(enemyName,currentRoom.enemyInRoom.name);
		GUI.Box(enemyHp,"Health:"+currentRoom.enemyInRoom.health);
		
		bool actionTaken=false;
		//Shoot button
		if (selectedMember.equippedRangedWeapon!=null && PartyManager.mainPartyManager.ammo>0)
		{
			if (GUI.Button(shootButton,"Shoot"))
			{	
				int actualDmg=currentRoom.DamageEnemy(selectedMember.RangedAttack(),true);//selectedMember.equippedRangedWeapon.GetDamage(),true);//.DamageEnemy(dmg);
				DisplayNewMessage("You shoot the "+currentRoom.enemyInRoom.name+" for "+actualDmg+"!");
				/*
				//currentRoom.DamageEnemy(selectedMember.equippedRangedWeapon.GetDamage());
				PartyManager.mainPartyManager.ammo-=1;
				//(currentRoom);*/
				actionTaken=true;
			}
		}
		// Punch button
		if (GUI.Button(punchButton,"Hit"))
		{
			string messageStart=null;
			//int dmg=0;
			/*
			if (PartyManager.mainPartyManager.partyMemberDoPunch(selectedMember))
			{
				if (selectedMember.equippedMeleeWeapon!=null) {dmg=selectedMember.equippedMeleeWeapon.GetDamage();}
				else {dmg=1;}
				messageStart=selectedMember.name+" hits the "+currentRoom.enemyInRoom.name;
			}
			else 
			{
				dmg=1;
				messageStart=selectedMember.name+"'s weak attack hits "+currentRoom.enemyInRoom.name;
			}*/
			int actualDmg=currentRoom.DamageEnemy(selectedMember.MeleeAttack(),false);//.DamageEnemy(dmg);
			//DisplayNewMessage(messageStart+" for "+actualDmg+"!");
			DisplayNewMessage(selectedMember.name+" hits the "+currentRoom.enemyInRoom.name+" for "+actualDmg+"!");
			actionTaken=true;
		}
		// Wait button
		if (GUI.Button(waitButton,"Wait"))
		{
			actionTaken=true;
		}
		
		
		if (actionTaken) 
		{
			battleTokens[selectedMember].turnTaken=true;
			battleTokens[selectedMember].selected=false;
			currentRoom.enemyInRoom.RoundAction();
			
			bool actionsRemain=false;
			foreach(PartyMemberToken token in battleTokens.Values)
			{
				if (!token.turnTaken) 
				{
					selectedMember=token.drawnMember;
					token.selected=true;
					actionsRemain=true;
					break;
				}
			}
			if (!actionsRemain) {TurnOver(currentRoom);}
		}
	}
	
	public void DisplayNewMessage(string message)
	{
		StopCoroutine("AddTimedMessage");
		StartCoroutine("AddTimedMessage",message);
	}
	
	IEnumerator AddTimedMessage(string message)
	{
		lastMessage+=message+"\n";
		/*
		messageCount+=1;
		
		if (messageCount>2)
		{
			lastMessage=lastMessage.Remove(0,lastMessage.IndexOf("\n")+1);
			messageCount=2;
		}*/
		while (lastMessage.Contains("\n"))
		{
			yield return new WaitForSeconds(1f);
			lastMessage=lastMessage.Remove(0,lastMessage.IndexOf("\n")+1);
		}
		lastMessage="";
		//messageCount=0;
		yield break;	
	}
	
	void Start() 
	{
		mainEncounterManager=this;
		GameManager.GameOver+=EndEncounter;
	}
	
	void OnGUI()
	{
		if (encounterOngoing)
		{
			//if (
			//GUI.Box(encounterBoxRect,"");
			DisplayEncounter(currentEncounter.encounterMap[new Vector2(encounterPlayerX,encounterPlayerY)]);
		}
	}
}
