using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PartyManager : MonoBehaviour {

	public static PartyManager mainPartyManager;//mainPlayerState;
	
	public int dayTime;//=0;
	
	public int mapCoordX;//=0;
	public int mapCoordY;//=0;
	
	//deprecate this later
	public int foodSupply;//=2;
	
	public List<PartyMember> partyMembers;
	public List<PartyMemberSelector> selectors;
	public StatusEffectDrawer effectTokenPrefab;
	Dictionary<int,List<StatusEffectDrawer>> statusEffectTokens;
	
	public Canvas partyStatusCanvas;
	public PartyMemberCanvasHandler partyMemberCanvasPrefab;
	Dictionary<PartyMember,PartyMemberCanvasHandler> partyMemberCanvases; 
	
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
	
	public List<InventoryItem> partyInventory;
	public delegate void InventoryChangedDeleg();
	public static event InventoryChangedDeleg InventoryChanged;
	
	public delegate void TimePassedDeleg(int hours);
	public static event TimePassedDeleg TimePassed;
	
	public PartyMemberSelector selectorPrefab;
	
	Rect timeOfDayRect=new Rect(5,10,100,25);
	
	Rect playerXCoordRect=new Rect(5,45,100,25);
	Rect playerYCoordRect=new Rect(5,80,100,25);
	
	//Rect healthRect=new Rect(5,120,100,25);
	Rect foodSupplyRect=new Rect(5,120,100,25);//new Rect(5,150,100,25);
	Rect ammoSupplyRect=new Rect(5,150,100,25);//new Rect(5,180,100,25);
	//Rect staminaRect=new Rect(5,210,100,25);
	float partyStatsStartX=5;
	float partyStatsStartY=185; //150+25+5
	
	//bool drawPartyScreen=false;
	
	//Rect gameOverMessageRect=new Rect(Screen.height*0);
	bool gameOver=false;
	
	//Setup start of game
	void SetDefaultState()
	{
		partyStatusCanvas.gameObject.SetActive(true);
		partyMemberCanvases=new Dictionary<PartyMember, PartyMemberCanvasHandler>();
		dayTime=0;
		
		mapCoordX=0;
		mapCoordY=0;
		
		foodSupply=0;//2;
		ammo=5;
		
		partyMembers=new List<PartyMember>();
		selectors=new List<PartyMemberSelector>();
		statusEffectTokens=new Dictionary<int, List<StatusEffectDrawer>>();
		
		AddNewPartyMember(new PartyMember("Guy1",null));
		AddNewPartyMember(new PartyMember("Guy2",null));
		
		partyInventory=new List<InventoryItem>();
		partyInventory.Add(new Food());
		partyInventory.Add (new Food());
		partyInventory.Add (new Medkit());
		AddPartyMemberStatusEffect(partyMembers[0],new Bleed(partyMembers[0]));
	}
	
	IEnumerator DoGameOver()
	{
		//Rect messageRect=new Rect();
		gameOver=true;
		partyStatusCanvas.gameObject.SetActive(false);
		GameManager.mainGameManager.EndCurrentGame();
		yield return new WaitForSeconds(2);
		gameOver=false;	
		yield break;
	}
	
	public void PassTime(int hoursPassed)
	{
		dayTime=(int)Mathf.Repeat(dayTime+hoursPassed,24);
		//foreach (PartyMember member in partyMembers) {member.hunger+=10*hoursPassed;}
		if (TimePassed!=null) {TimePassed(hoursPassed);}
		
		//Adjust camera bg color
		float lightBottomThreshold=0.5f;
		float noonLightLevelBonus=0.5f;
		float newb=lightBottomThreshold+noonLightLevelBonus-(noonLightLevelBonus/12)*(Mathf.Abs(dayTime-12));//Mathf.Repeat(Camera.main.backgroundColor.b-0.083//+0.0416f*hoursPassed,1);
		Camera.main.backgroundColor=new Color(0,0,newb);
	}
	
	public int PartyMemberCount() {return partyMembers.Count;}
	public PartyMember GetPartyMember(int index) {return partyMembers[index];}
	
	public void AddNewPartyMember(PartyMember newMember)
	{
		//PartyMemberSelector newSelector=Instantiate(selectorPrefab) as PartyMemberSelector;
		partyMembers.Add (newMember);
		//selectors.Add (newSelector);
		//newSelector.member=newMember;
		statusEffectTokens.Add(partyMembers.IndexOf(newMember),new List<StatusEffectDrawer>());
		
		PartyMemberCanvasHandler newPartyMemberCanvas=Instantiate(partyMemberCanvasPrefab);
		newPartyMemberCanvas.AssignPartyMember(newMember);
		newPartyMemberCanvas.transform.SetParent(partyStatusCanvas.transform.FindChild("PartyMembersLayoutGroup"),false);
		partyMemberCanvases.Add(newMember,newPartyMemberCanvas);
	}
	
	public void RemovePartyMember(PartyMember removedMember)
	{
		PartyMemberCanvasHandler deletedHandler=partyMemberCanvases[removedMember];
		partyMemberCanvases.Remove(removedMember);
		GameObject.Destroy(deletedHandler.gameObject);
		partyMembers.Remove(removedMember);
		if (partyMembers.Count==0) {StartCoroutine(DoGameOver());}
	}
	
	public bool ConfirmMapMovement(int x, int y)
	{
		bool moveSuccesful=false;
		//cancel if not enough stamina to move
		foreach (PartyMember member in partyMembers) 
		{
			if (member.stamina<1) {return moveSuccesful;}
		}
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
			mapCoordX=x;
			mapCoordY=y;
			//foodSupply-=1;
			//stamina-=1;
			foreach(PartyMember member in partyMembers) {member.stamina-=1;}
			//dayTime=(int)Mathf.Repeat(dayTime+1,24);
			PassTime(1);
			
			//moveSuccesful=true;
		//}
		//return moveSuccesful;
	}
	
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
		foreach (PartyMember member in partyMembers)
		{
			//foodSupply-=1;
			if (member.hunger<100)
			{
				member.stamina=member.maxStamina;
				member.health+=5;
			}
			else
			{
				member.health-=5;
				member.stamina=5;
			}
		}
		PassTime(3);
	}
	
	public void AddPartyMemberStatusEffect(PartyMember member, StatusEffect effect)//(int memberIndex, StatusEffect effect)
	{
		member.activeStatusEffects.Add(effect);
		partyMemberCanvases[member].AddStatusEffectToken(effect);
	}
	
	public void RemovePartyMemberStatusEffect(PartyMember member, StatusEffect effect)//(int memberIndex, StatusEffect effect)
	{
		
		member.activeStatusEffects.Remove(effect);
		partyMemberCanvases[member].RemoveStatusEffectToken(effect);
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
	
	void Start()
	{
		mainPartyManager=this;//mainPlayerState=this;
		GameManager.GameStart+=SetDefaultState;
	}
	
	void OnGUI()
	{
		if (GameManager.mainGameManager.gameStarted)
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
		
		if (gameOver)
		{
			GUI.Box(new Rect(Screen.width*0.5f-25f,Screen.height*0.5f-50f,100,50),"Your party died");
		}
	}
	
	
}
