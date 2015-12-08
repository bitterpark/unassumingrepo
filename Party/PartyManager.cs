using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PartyManager : MonoBehaviour {

	public static PartyManager mainPartyManager;//mainPlayerState;
	
	public int dayTime;//=0;
	public int timePassed=0;
	
	public int mapCoordX;//=0;
	public int mapCoordY;//=0;
	
	//public bool hasCook=false;
	
	public List<PartyMember> partyMembers;
	public List<PartyMemberSelector> selectors;
	public StatusEffectDrawer effectTokenPrefab;
	Dictionary<int,List<StatusEffectDrawer>> statusEffectTokens;
	
	public PartyStatusCanvasHandler statusCanvas;
	public PartyMemberCanvasHandler partyMemberCanvasPrefab;
	Dictionary<PartyMember,PartyMemberCanvasHandler> partyMemberCanvases; 
	
	const int moveFatigueCost=10;
	//const int encounterFatigueCost=10;
	
	public int ammo
	{
		get {return _ammo;}
		set 
		{
			_ammo=value;
			if (_ammo<0) {_ammo=0;}
		}
	}
	int _ammo;
	
	//public int partyVisibilityMod;
	
	List<InventoryItem> partyInventory;
	public List<InventoryItem> GetPartyInventory() {return partyInventory;}
	
	public delegate void InventoryChangedDeleg();
	public static event InventoryChangedDeleg InventoryChanged;
	
	public delegate void TimePassedDeleg(int hours);
	public static event TimePassedDeleg TimePassed;
	
	//public PartyMemberSelector selectorPrefab;
	
	//Setup start of game
	void SetDefaultState()
	{
		//partyStatusCanvas.gameObject.SetActive(true);
		statusCanvas.EnableStatusDisplay();
		partyMemberCanvases=new Dictionary<PartyMember, PartyMemberCanvasHandler>();
		dayTime=5;
		
		mapCoordX=0;
		mapCoordY=0;
		//partyVisibilityMod=0;
		//foodSupply=0;//2;
		ammo=0;//500;//5;
		
		partyMembers=new List<PartyMember>();
		selectors=new List<PartyMemberSelector>();
		//statusEffectTokens=new Dictionary<int, List<StatusEffectDrawer>>();
		
		AddNewPartyMember(new PartyMember());
		AddNewPartyMember(new PartyMember());
		//AddNewPartyMember(new PartyMember());
		//AddNewPartyMember(new PartyMember());
		//AddNewPartyMember(new PartyMember());
		//AddNewPartyMember(new PartyMember());
		partyInventory=new List<InventoryItem>();
		partyInventory.Add(new FoodBig());
		partyInventory.Add (new FoodSmall());
		//partyInventory.Add(new SettableTrap());
		//partyInventory.Add(new SettableTrap());
		//partyInventory.Add(new SettableTrap());
		partyInventory.Add(new Knife());
		partyInventory.Add (new Knife());
		partyInventory.Add (new Axe());
		partyInventory.Add (new Axe());
		//partyInventory.Add(new AmmoBox());
		//partyInventory.Add (new AssaultRifle());
		//partyInventory.Add (new AssaultRifle());
		//partyInventory.Add (new SettableTrap());
		//partyInventory.Add (new SettableTrap());
		//partyInventory.Add (new Flashlight());
		//Do this to setup proper background color
		PassTime(1);
	}
	
	public void PassTime(int hoursPassed)
	{
		timePassed+=hoursPassed;
		dayTime=(int)Mathf.Repeat(dayTime+hoursPassed,24);
		//foreach (PartyMember member in partyMembers) {member.hunger+=10*hoursPassed;}
		if (TimePassed!=null) {TimePassed(hoursPassed);}
		
		//Adjust camera bg color
		float lightBottomThreshold=0.5f;
		float noonLightLevelBonus=0.5f;
		float newb=lightBottomThreshold+noonLightLevelBonus-(noonLightLevelBonus/12)*(Mathf.Abs(dayTime-12));//Mathf.Repeat(Camera.main.backgroundColor.b-0.083//+0.0416f*hoursPassed,1);
		Camera.main.backgroundColor=new Color(0,0,newb);
	}
	
	//Debug
	
	void Update() 
	{
		if (Input.GetKeyDown(KeyCode.M)) 
		{
			RemovePartyMember(partyMembers[2]);
			/*
			for (int i=0; i<30; i++)
			{
			partyMembers[0].GetMeleeAttackDamage();
			}*/
		}
	}
	
	public int PartyMemberCount() {return partyMembers.Count;}
	public PartyMember GetPartyMember(int index) {return partyMembers[index];}
	
	public void AddNewPartyMember(PartyMember newMember)
	{
		partyMembers.Add (newMember);
		PartyMemberCanvasHandler newPartyMemberCanvas=Instantiate(partyMemberCanvasPrefab);
		newPartyMemberCanvas.AssignPartyMember(newMember);
		newPartyMemberCanvas.transform.SetParent(statusCanvas.transform.FindChild("PartyMembersLayoutGroup"),false);
		partyMemberCanvases.Add(newMember,newPartyMemberCanvas);
		statusCanvas.NewNotification(newMember.name+" has joined the party");
	}
	
	public void RemovePartyMember(PartyMember removedMember)
	{
		PartyMemberCanvasHandler deletedHandler=partyMemberCanvases[removedMember];
		partyMemberCanvases.Remove(removedMember);
		GameObject.Destroy(deletedHandler.gameObject);
		partyMembers.Remove(removedMember);
		if (partyMembers.Count==0) {GameManager.main.EndCurrentGame(false);}//StartCoroutine(DoGameOver(false));}
		else
		{
			//this is required for the vertical layout formation to update on time (damn bugs)
			foreach (PartyMemberCanvasHandler memberCanvas in partyMemberCanvases.Values) 
			{
				memberCanvas.GetComponent<Canvas>().enabled=false;
				memberCanvas.GetComponent<Canvas>().enabled=true;
			}
			
			foreach (PartyMember member in partyMembers) 
			{
				if (member.relationships.ContainsKey(removedMember))
				{
					member.morale-=40;
					member.RemoveRelatonship(removedMember);
				}
			}
		}
	}
	
	public bool ConfirmMapMovement(int x, int y)
	{
		bool moveSuccesful=false;
		//cancel if not enough stamina to move
		/*
		foreach (PartyMember member in partyMembers) 
		{
			if (member.stamina<1) {return moveSuccesful;}
		}*/
		int moveLength=Mathf.Abs(x-mapCoordX)+Mathf.Abs(y-mapCoordY);//Mathf.Max (Mathf.Abs(x-mapCoordX),Mathf.Abs(y-mapCoordY));
		if (moveLength==1)
		{
			moveSuccesful=true;
		}
		return moveSuccesful;
	}
	
	//for regular map movement
	//public bool MovePartyToMapCoords(int x, int y) {return MovePartyToMapCoords(x,y,false);}
	
	//for event-based map jumps
	public void MovePartyToMapCoords(int x, int y)
	{
		/*
		bool moveSuccesful=false;
		//cancel if not enough stamina to move
		foreach (PartyMember member in partyMembers) 
		{
			if (member.stamina<1 && !overrideMoveLimits) {return moveSuccesful;}
		}
		int moveLength=Mathf.Abs(x-mapCoordX)+Mathf.Abs(y-mapCoordY);//Mathf.Max (Mathf.Abs(x-mapCoordX),Mathf.Abs(y-mapCoordY));
		if (moveLength==1 | overrideMoveLimits)
		{*/
			int moveLength=Mathf.Abs(x-mapCoordX)+Mathf.Abs(y-mapCoordY);
			mapCoordX=x;
			mapCoordY=y;
			if (moveLength>0) 
			{
				foreach(PartyMember member in partyMembers) {member.ChangeFatigue(moveFatigueCost);}
				PassTime(1);
			}
			
			
			//foodSupply-=1;
			//stamina-=1;
			
			//dayTime=(int)Mathf.Repeat(dayTime+1,24);
			
			
			//moveSuccesful=true;
		//}
		//return moveSuccesful;
	}
	/*
	public void EnterPartyIntoEncounter(List<PartyMember> encounterMembers)
	{
		PassTime(1);
		foreach (PartyMember member in encounterMembers) member.ChangeFatigue(encounterFatigueCost);
	}*/
	/*
	public void MemberLeavesEncounter(PartyMember leavingMember)
	{
		//PassTime(1);
		//foreach (PartyMember member in encounterMembers) member.ChangeFatigue(encounterFatigueCost);
		leavingMember.ChangeFatigue(encounterFatigueCost);
	}*/
	
	public void GainItems(InventoryItem newItem)
	{
		partyInventory.Add(newItem);
		InventoryChanged();
	}	
	
	public void RemoveItems(InventoryItem lostItem)
	{
		partyInventory.Remove(lostItem);
		InventoryChanged();
	}
	
	public void Rest()
	{
		/*
		foreach (PartyMember member in partyMembers)
		{
			//foodSupply-=1;
			if (member.hunger<100)
			{
				member.stamina+=Mathf.RoundToInt(member.maxStamina*0.3f);
				member.health+=2;
			}
			else
			{
				member.health-=2;
				member.stamina+=Mathf.RoundToInt(member.maxStamina*0.1f);
			}
		}*/
		foreach (PartyMember member in partyMembers) {member.RestEffect();}
		PassTime(6);
	}
	
	public void AddPartyMemberStatusEffect(PartyMember member, StatusEffect effect)//(int memberIndex, StatusEffect effect)
	{
		if (partyMembers.Contains(member))
		{
			//member.activeStatusEffects.Add(effect);
			if (member.AddStatusEffect(effect)) partyMemberCanvases[member].AddStatusEffectToken(effect);
		}
	}
	
	public void RemovePartyMemberStatusEffect(PartyMember member, StatusEffect effect)//(int memberIndex, StatusEffect effect)
	{
		if (partyMembers.Contains(member))
		{
			member.activeStatusEffects.Remove(effect);
			partyMemberCanvases[member].RemoveStatusEffectToken(effect);
		}
	}
	/*
	public void DamagePartyMember(PartyMember member, int dmg)
	{
		//PartyMember affected=partyMembers[memberIndex];
		member.health-=dmg;
		if (member.health==0) 
		{
			RemovePartyMember(member);
		}
	}*/
	/*
	public bool HealPartyMember(int memberIndex, int amountHealed)
	{
		return HealPartyMember(partyMembers[memberIndex],amountHealed);
	}*/
	/*
	public bool HealPartyMember (PartyMember member, int amountHealed)
	{
		bool healingSuccesful=false;
		{
			if (member.health<member.healthMax)
			{
				member.health+=amountHealed;
				healingSuccesful=true;
			}
		}
		return healingSuccesful;
	}*/
	/*
	public bool FeedPartyMember(PartyMember member, int amountFed)
	{
		bool feedSuccesful=false;
		if (member.hunger>0)
		{
			//member.health+=amountHealed;
			member.hunger-=amountFed;
			feedSuccesful=true;
		}
		return feedSuccesful;
	}*/
	/*
	public bool partyMemberDoPunch(PartyMember member)
	{
		bool strongPunch=false;
		if (member.stamina>0) 
		{
			member.stamina-=1;
			strongPunch=true;
		}
		return strongPunch;
	}*/
	
	/*
	void DrawPartyMemberStats()
	{
		float startX=partyStatsStartX;
		float startY=partyStatsStartY;
		//float offsetY=0;
		float sizeX=100;
		float sizeY=25;
		
		Rect itemRect=new Rect(startX,startY,sizeX,sizeY);
		int i=0;
		foreach(PartyMember member in partyMembers)
		{
			GUI.Box(itemRect,member.name);
			itemRect.y+=sizeY+17;
			Vector3 newSelectorPos=Camera.main.ScreenToWorldPoint(new Vector2(itemRect.x+sizeX*0.5f,Screen.height-itemRect.y));
			newSelectorPos.z=-2;
			//adjust selectors
			selectors[i].transform.position=newSelectorPos;//Camera.main.ScreenToWorldPoint(new Vector2(500,Screen.height-500));//itemRect.x,Screen.height-itemRect.y));
			//adjust status effect tokens
			if (statusEffectTokens[i].Count>0)
			{
				Vector3 newTokenPos=newSelectorPos+new Vector3(25,0,0);
				foreach(StatusEffectDrawer drawer in statusEffectTokens[i]) 
				{
					drawer.transform.position=newTokenPos;
					newTokenPos+=new Vector3(25,0,0);
				}
			}
			itemRect.y+=17;
			GUI.Box(itemRect,"Health:"+member.health);
			itemRect.y+=sizeY+3;
			GUI.Box(itemRect,"Stamina:"+member.stamina);
			itemRect.y+=sizeY+3;
			GUI.Box(itemRect,"Hunger:"+member.hunger);
			itemRect.y+=sizeY+3;
			i++;
		}
		//Camera.main.ScreenToWorldPoint(new Vector2(partyMemberStartX+partyMemberXGap*i,Screen.height-(partyMemberStartY)))
	}*/
	
	public void GameOverCleanup()
	{
		TimePassed=null;
		List<PartyMember> buffer=new List<PartyMember>();
		buffer.AddRange(partyMembers.ToArray());
		foreach (PartyMember member in buffer) {RemovePartyMember(member);}
		statusCanvas.DisableStatusDisplay();
	}
	
	void Start()
	{
		mainPartyManager=this;//mainPlayerState=this;
		GameManager.GameStart+=SetDefaultState;
		GameManager.GameOver+=GameOverCleanup;
	}
	
	void OnGUI()
	{
		if (GameManager.main.gameStarted)
		{
			//GUI.Box(timeOfDayRect,dayTime.ToString()+":00");
			//GUI.Box(playerXCoordRect,"Map x:"+mapCoordX.ToString());
			//GUI.Box(playerYCoordRect,"Map y:"+mapCoordY.ToString());
			//GUI.Box (healthRect,"Health:"+health.ToString());
			//GUI.Box (foodSupplyRect,"Food:"+foodSupply.ToString());
			//GUI.Box (ammoSupplyRect,"Ammo:"+ammo);
			//GUI.Box (staminaRect,"Stamina:"+stamina);
			//DrawPartyMemberStats();
			
			Rect partyScreenButtonRect=new Rect(5,300,50,20);
			/*
			if (GUI.Button(partyScreenButtonRect,"Party")) {drawPartyScreen=!drawPartyScreen;}
			if (drawPartyScreen)
			{
				PartyScreenManager.mainPSManager.DrawPartyScreen();	
			}*/
		}
		/*
		if (gameOver)
		{
			string endMessage="Your party died";
			if (gameWin) {endMessage="You were rescued!";}
			GUI.Box(new Rect(Screen.width*0.5f-25f,Screen.height*0.5f-50f,100,50),endMessage);
		}*/
	}
	
	
}
