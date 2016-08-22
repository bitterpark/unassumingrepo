using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PrepCardManager : MonoBehaviour {

	public static PrepCardManager main;
	
	public Button beginCombatButton;
	public Button characterPreppedButton;

	public Transform prepCardsDisplayArea;

	public CombatCardGraphic combatCardPrefab;

	enum PrepMode { SelectClassCard, SelectWeaponCard };
	PrepMode currentPrepMode;

	CombatManager combatManager;
	MissionCharacterManager characterManager;
	HandDisplayer enemyHandObject;
	PrepHandDisplayer prepHandObject;
	ModeTextDisplayer modeTextDisplayer;
	MissionUIToggler uiToggler;

	CharacterGraphic selectedCharacter;

	public void EnablePrepDisplayer(CombatManager combatManager, MissionCharacterManager characterManager, HandDisplayer enemyHandObject, PrepHandDisplayer prepHandObject,
		MissionUIToggler uiToggler,ModeTextDisplayer modeTextDisplayer)
	{
		this.combatManager = combatManager;
		this.characterManager = characterManager;
		this.prepHandObject = prepHandObject;
		this.enemyHandObject = enemyHandObject;
		this.modeTextDisplayer = modeTextDisplayer;
		this.uiToggler = uiToggler;
		main = this;
	}

	public void BeginPrepCardsPhase()
	{
		beginCombatButton.gameObject.SetActive(true);
		beginCombatButton.onClick.AddListener(FinishCombatPrepPhase);
		StartCoroutine("SelectNextCharacterToPrep");
	}

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
	}

	void ShowSelectionOfPrepCards(MercGraphic character)
	{
		HideAvailablePrepCards();
		if (currentPrepMode == PrepMode.SelectClassCard)
			prepHandObject.DisplayPrepHand(character.GetMercsClassPrepCards());
		if (currentPrepMode == PrepMode.SelectWeaponCard)
			prepHandObject.DisplayPrepHand(character.GetMercsWeaponPrepCards());
	}

	public void PlayPrepCard(PrepCardGraphic cardGraphic)
	{
		StartCoroutine("PrepCardPlaying", cardGraphic);
	}

	IEnumerator PrepCardPlaying(PrepCardGraphic cardGraphic)
	{
		prepHandObject.SetPrepHandInteractivity(false);
		cardGraphic.transform.SetParent(prepCardsDisplayArea, false);
		cardGraphic.PlayAssignedCard(selectedCharacter);
		if (currentPrepMode == PrepMode.SelectClassCard)
		{
			TransferPrepFromCharacterCardToWeaponCard();
		}
		yield break;
	}

	void TransferPrepFromCharacterCardToWeaponCard()
	{
		currentPrepMode = PrepMode.SelectWeaponCard;
		MercGraphic selectedMerc = selectedCharacter as MercGraphic;
		ShowSelectionOfPrepCards(selectedMerc);
	}

	void CharacterPrepFinished()
	{
		characterPreppedButton.onClick.RemoveAllListeners();
		characterPreppedButton.gameObject.SetActive(false);
		beginCombatButton.gameObject.SetActive(true);
		HideAvailablePrepCards();
		HidePrepDisplayDeck();
		selectedCharacter.RemoveTurn();
		StartCoroutine("SelectNextCharacterToPrep");
	}

	void HideAvailablePrepCards()
	{
		prepHandObject.HidePrepHand();
	}

	void HidePrepDisplayDeck()
	{
		foreach (CardGraphic graphic in prepCardsDisplayArea.GetComponentsInChildren<CardGraphic>())
			GameObject.Destroy(graphic.gameObject);
	}

	void FinishCombatPrepPhase()
	{
		StopCoroutine("SelectNextCharacterToPrep");
		modeTextDisplayer.HideCenterMessage();
		beginCombatButton.onClick.RemoveAllListeners();
		beginCombatButton.gameObject.SetActive(false);
		combatManager.FinishPrepCardsPhase();
		
	}
}
