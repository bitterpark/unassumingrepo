using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionUIToggler : MonoBehaviour {

	public static MissionUIToggler main;
	
	public Button skipTurnButton;
	public Button finishButton;

	bool handPeekingAllowed = false;

	CardsScreen cardsScreen;
	CombatManager combatManager;

	public void EnableUIToggler(CardsScreen cardsScreen,CombatManager combatManager)
	{
		this.cardsScreen = cardsScreen;
		this.combatManager = combatManager;
	}


	public void SetAllowHandPeeking(bool allow)
	{
		handPeekingAllowed = allow;
	}
	/*
	public void PeekCharacterHand(CharacterGraphic shownCharacter)
	{
		CharacterGraphic selectedCharacter = combatManager.GetSelectedCharacter();
		handPeekingAllowed = false; //TEMPORARY TESTING MEASURE, CHANGE THIS LATER!!!
		if (handPeekingAllowed && shownCharacter != selectedCharacter)
		{
			selectedCharacter.HideMyDisplayedHand();
			shownCharacter.DisplayMyHand(false);
		}
	}

	public void StopPeekingCharacterHand(CharacterGraphic shownCharacter)
	{
		CharacterGraphic selectedCharacter = combatManager.GetSelectedCharacter();
		handPeekingAllowed = false; //TEMPORARY TESTING MEASURE, CHANGE THIS LATER!!!
		if (handPeekingAllowed)
		{
			shownCharacter.HideMyDisplayedHand();
			selectedCharacter.DisplayMyHand(true);
		}
	}
	*/

	public void DisablePlayerActionsDuringCardPlay()
	{
		PlayerHandManager handManager = CombatManager.main.GetPlayerHandManager();
		handManager.SetPlayerHandInteractivity(false);
	}

	public void ReenablePlayerActions()
	{
		PlayerHandManager handManager = CombatManager.main.GetPlayerHandManager();
		handManager.SetPlayerHandInteractivity(true);
	}

	public void EnableTurnoverButton()
	{
		skipTurnButton.gameObject.SetActive(true);
	}

	public void DisableTurnoverButton()
	{
		skipTurnButton.gameObject.SetActive(false);
	}

	public void SetFinishButtonEnabled(bool enabled)
	{
		finishButton.gameObject.SetActive(enabled);
	}

	void Start()
	{
		skipTurnButton.onClick.AddListener(() => combatManager.EndTurnButtonPressed());
		finishButton.onClick.AddListener(() => cardsScreen.MissionEndButtonPressed());
		main = this;
	}
}
