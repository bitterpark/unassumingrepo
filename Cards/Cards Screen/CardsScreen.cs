using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardsScreen : MonoBehaviour 
{
	public static CardsScreen main;
	
	public HandDisplayer enemyHandObject;
	public PrepHandDisplayer prepHandObject;
	public RewardCardDisplayer rewardDisplayer;
	public RoomCardDisplayer roomChoiceDisplayer;
	public MissionCharacterManager characterManager;
	public PrepHandManager prepManager;
	public ModeTextDisplayer modeTextDisplayer;
	public CombatCardTargeter combatCardTargeter;
	public AICombatCardSelector aiCardSelector;
	public MissionUIToggler uiToggler;
	public CombatManager combatManager;

	static bool missionOngoing = false;

	Encounter playedEncounter;

	public static bool IsMissionOngoing()
	{
		return missionOngoing;
	}

	public void OpenScreen(Encounter newEncounter, PartyMember[] team)
	{
		missionOngoing = true;
		GetComponent<Canvas>().enabled = true;
		playedEncounter=newEncounter;
		rewardDisplayer.EnableDisplayer(this,newEncounter);
		roomChoiceDisplayer.EnableDisplayer(this, newEncounter);
		characterManager.EnableCharacterManager(combatManager);
		characterManager.SpawnMercsOnMissionStart(team);
		prepManager.EnablePrepDisplayer(combatManager, characterManager, uiToggler, modeTextDisplayer);
		combatCardTargeter.EnableCombatCardTargeter(combatManager, characterManager, uiToggler, modeTextDisplayer);
		aiCardSelector.EnableCombatCardSelector(combatManager);
		uiToggler.EnableUIToggler(this,combatManager);
		combatManager.EnableCombatManager(this, characterManager, uiToggler, prepManager, modeTextDisplayer, combatCardTargeter, aiCardSelector);

		ProgressThroughEncounter();
	}

	public void MissionEndButtonPressed()
	{
		CloseScreen();
	}

	public void CloseScreen()
	{
		missionOngoing = false;
		modeTextDisplayer.HideCenterMessage();
		uiToggler.SetFinishButtonEnabled(false);

		characterManager.ResetSelectionArrow();
		characterManager.DoMissionEndCleanup();
		
		GetComponent<Canvas>().enabled = false;
		PartyManager.mainPartyManager.AdvanceMapTurn();
	}

	void ProgressThroughEncounter()
	{
		if (!playedEncounter.IsFinished())
			roomChoiceDisplayer.ShowNextRoomSelection();
		else
		{
			uiToggler.DisableTurnoverButton();
			uiToggler.SetFinishButtonEnabled(true);
			modeTextDisplayer.DisplayCenterMessage("Run complete!");
		}
	}


	public void RoomCardSelected(RoomCard selectedCard)
	{
		BeginCombatMode(selectedCard);
	}

	void BeginCombatMode(RoomCard selectedCard)
	{
		combatManager.SetupCombat(selectedCard);
	}

	public void CombatWon()
	{
		rewardDisplayer.ShowRewards();
	}

	public void RewardSelectionFinished()
	{
		ProgressThroughEncounter();
	}

	public int GetCurrentEnemyCount()
	{
		return characterManager.GetEnemyCount();
	}

	public int GetCurrentMercCount()
	{
		return characterManager.GetMercCount();
	}

	public bool ShowingEncounter()
	{
		return missionOngoing;
	}

	void Start() 
	{ 
		main = this;
		
	}

}
