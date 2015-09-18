using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartyScreenManager : MonoBehaviour 
{
	public static PartyScreenManager mainPSManager;
	
	public InventoryItemToken itemTokenPrefab;
	List<InventoryItemToken> itemTokens=new List<InventoryItemToken>();
	InventoryItemToken rangedWeaponToken;
	InventoryItemToken meleeWeaponToken;
	
	public bool showScreen=false;
	PartyMember selectedMember;
	
	public void InventoryItemClicked(InventoryItemToken clickedToken)
	{
		if (!clickedToken.isWeaponSlot) {clickedToken.myItem.UseAction(selectedMember);}
		else 
		{
			Weapon clickedWeapon=clickedToken.myItem as Weapon;
			clickedWeapon.Unequip(selectedMember);
		}
	}
	
	public void MemberClicked(PartyMember clickedMember)
	{
		if (clickedMember==selectedMember) {StopDrawingPartyScreen();}
		else 
		{
			if (!EncounterManager.mainEncounterManager.combatOngoing)
			{
				StopDrawingPartyScreen();
				selectedMember=clickedMember;
				StartDrawingPartyScreen();
			}
		}
	}
	
	void RefreshItemTokens()
	{
		if (showScreen)
		{
			DeleteItemTokens();
			CreateItemTokens();
		}
	}
	
	void CreateItemTokens()
	{
		foreach(InventoryItem item in PartyManager.mainPartyManager.partyInventory) 
		{
			InventoryItemToken newToken=Instantiate(itemTokenPrefab) as InventoryItemToken;
			newToken.myItem=item;
			itemTokens.Add (newToken);
		}
		
		//Camera.main.ScreenToWorldPoint(new Vector2(inventoryStartGUICoords.x+(xSize+xPad)*i,Screen.height-inventoryStartGUICoords.y));
		//Vector3 rangedWeaponIconPos=new Vector3();
		rangedWeaponToken=Instantiate(itemTokenPrefab) as InventoryItemToken;
		rangedWeaponToken.myItem=selectedMember.equippedRangedWeapon;
		rangedWeaponToken.isWeaponSlot=true;
		//Vector3 meleeWeaponIconPos=new Vector3();
		meleeWeaponToken=Instantiate(itemTokenPrefab) as InventoryItemToken;
		meleeWeaponToken.myItem=selectedMember.equippedMeleeWeapon;
		meleeWeaponToken.isWeaponSlot=true;
	}
	
	void DeleteItemTokens()
	{
		foreach (InventoryItemToken token in itemTokens) {GameObject.Destroy(token.gameObject);}
		itemTokens.Clear();
		if (rangedWeaponToken!=null) GameObject.Destroy(rangedWeaponToken.gameObject);
		rangedWeaponToken=null;
		if (meleeWeaponToken!=null) GameObject.Destroy(meleeWeaponToken.gameObject);
		meleeWeaponToken=null;
	}
	
	
	
	public void StartDrawingPartyScreen()
	{
		/*
		if (itemTokens.Count==0) 
		{
			foreach(InventoryItem item in PartyManager.mainPartyManager.partyInventory) 
			{
				InventoryItemToken newToken=Instantiate(itemTokenPrefab) as InventoryItemToken;
				newToken.myItem=item;
				itemTokens.Add (newToken);
			}
		}*/
		CreateItemTokens();
		showScreen=true;
	}
	
	public void DrawPartyScreen()
	{
		//main bg
		float partyScreenWidth=800;
		float partyScreenHeight=350;
		Rect partyScreenRect=new 
		Rect(Screen.width*0.6f-partyScreenWidth*0.5f,Screen.height*0.5f-partyScreenHeight*0.5f,partyScreenWidth,partyScreenHeight);
		
		//framing bg rect
		//GUI.Box(partyScreenRect,"");
		
		//Name box
		Rect selectedMemberNamePos=new Rect(partyScreenRect);
		selectedMemberNamePos.x+=50;
		selectedMemberNamePos.y+=25;
		selectedMemberNamePos.width=80;
		selectedMemberNamePos.height=25;
		GUI.Box(selectedMemberNamePos,selectedMember.name);
		
		//Ranged weapon
		Vector3 newWeaponTokenPos=Camera.main.ScreenToWorldPoint(
		new Vector2 (selectedMemberNamePos.x+5+15,Screen.height-(selectedMemberNamePos.y+selectedMemberNamePos.height+15)));
		newWeaponTokenPos.z=-2;
		rangedWeaponToken.transform.position=newWeaponTokenPos;
		//Melee weapon
		newWeaponTokenPos=Camera.main.ScreenToWorldPoint(
		new Vector2 (selectedMemberNamePos.x+5+30+15+15,Screen.height-(selectedMemberNamePos.y+selectedMemberNamePos.height+15)));
		newWeaponTokenPos.z=-2;
		meleeWeaponToken.transform.position=newWeaponTokenPos;
		
		//inventory
		Rect inventoryRect=new Rect(partyScreenRect);
		inventoryRect.x+=partyScreenRect.width*0.10f;
		inventoryRect.y+=partyScreenRect.height*0.5f;
		inventoryRect.height*=0.4f;
		inventoryRect.width*=0.8f;
		//GUI.Box(inventoryRect,"");
		
		Vector2 inventoryStartGUICoords=new Vector2(inventoryRect.x+10,inventoryRect.y+10);
		float xSize=35*0.5f;
		float xPad=10;
		
		int i=0;
		foreach(InventoryItemToken token in itemTokens)
		{
			Vector3 newTokenPos=Camera.main.ScreenToWorldPoint(new Vector2(inventoryStartGUICoords.x+(xSize+xPad)*i,Screen.height-inventoryStartGUICoords.y));
			newTokenPos.z=-2;
			token.transform.position=newTokenPos;
			i++;
		}
	}
	
	public void StopDrawingPartyScreen()
	{
		DeleteItemTokens();
		selectedMember=null;
		showScreen=false;
	}
	
	void Start()
	{
		mainPSManager=this;
		PartyManager.InventoryChanged+=RefreshItemTokens;
	}
	
	void OnGUI()
	{
		if (GameManager.mainGameManager.gameStarted && showScreen)
		{
			if (EncounterManager.mainEncounterManager.combatOngoing) {StopDrawingPartyScreen();}
			else {DrawPartyScreen();}
		}
	}
}
