using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PrepHandManager : MonoBehaviour {

	public static PrepHandManager main;
	
	public Button beginCombatButton;
	public Button characterPreppedButton;

	public Transform prepCardsDisplayArea;

	enum PrepMode { SelectClassCard, SelectWeaponCard };
	PrepMode currentPrepMode;

	CombatManager combatManager;
	MissionCharacterManager characterManager;
	ModeTextDisplayer modeTextDisplayer;
	MissionUIToggler uiToggler;

	CharacterGraphic selectedCharacter;

	public void EnablePrepDisplayer(CombatManager combatManager, MissionCharacterManager characterManager,
		MissionUIToggler uiToggler,ModeTextDisplayer modeTextDisplayer)
	{
		this.combatManager = combatManager;
		this.characterManager = characterManager;
		this.modeTextDisplayer = modeTextDisplayer;
		this.uiToggler = uiToggler;
		main = this;
	}

	public void BeginPrepCardsPhase()
	{
		beginCombatButton.gameObject.SetActive(true);
		beginCombatButton.onClick.AddListener(FinishCombatPrepPhase);

		List<CharacterGraphic> characters=characterManager.GetMercGraphics();

		foreach (CharacterGraphic character in characters)
		{
			MercGraphic merc = character as MercGraphic;
			merc.StartPrepDisplay();
		}

		//StartCoroutine("SelectNextCharacterToPrep");
	}
	/*
	IEnumerator SelectNextCharacterToPrep()
	{
		CharacterGraphic targetChar = null;
		bool selectFriendly = true;
		string centerMessageText = "Select character";

		uiToggler.DisablePlayerActionsDuringCardPlay();
		modeTextDisplayer.DisplayCenterMessage(centerMessageText);

		characterManager.SetMercPortraitsEnabled(false);
		while (true)
		{
			if (Input.GetMouseButton(0))
			{
				if (characterManager.RaycastForCharacter(selectFriendly, out targetChar))
				{
					if (targetChar.HasTurn())
						break;
				}
			}
			yield return new WaitForEndOfFrame();
		}

		characterManager.SetMercPortraitsEnabled(true);
		modeTextDisplayer.HideCenterMessage();
		MercGraphic targetMerc = targetChar as MercGraphic;
		selectedCharacter = targetMerc;
		characterManager.HighlightSelectedPlayersCharacter(selectedCharacter);
		CharacterSelectedForPrep(targetMerc);
	}


	/*
	void CharacterSelectedForPrep(MercGraphic character)
	{
		characterPreppedButton.gameObject.SetActive(true);
		characterPreppedButton.onClick.AddListener(CharacterPrepFinished);
		beginCombatButton.gameObject.SetActive(false);
		List<CombatCard> startingCards = character.GetCurrentDeckCards();

		foreach (CombatCard card in startingCards)
		{
			CombatCardGraphic newCardGraphic = Instantiate(combatCardPrefab);
			newCardGraphic.AssignCard(card);
			newCardGraphic.SetInteractable(false);
			newCardGraphic.transform.SetParent(prepCardsDisplayArea, false);
		}

		currentPrepMode = PrepMode.SelectClassCard;
		ShowSelectionOfPrepCards(character);
	}*/

	void FinishCombatPrepPhase()
	{
		List<CharacterGraphic> characters=characterManager.GetMercGraphics();
		
		foreach (CharacterGraphic character in characters)
		{
			MercGraphic merc = character as MercGraphic;
			merc.FinishPrepDisplay();
		}
		
		modeTextDisplayer.HideCenterMessage();
		beginCombatButton.onClick.RemoveAllListeners();
		beginCombatButton.gameObject.SetActive(false);
		combatManager.FinishPrepCardsPhase();
		
	}
}
