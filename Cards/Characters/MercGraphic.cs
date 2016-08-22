using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MercGraphic : CharacterGraphic
{
	public Text classText;
	Mercenary assignedMerc;

	public void AssignCharacter(Mercenary newChar)
	{
		base.AssignCharacter(newChar);
		handManager.EnableHandDisplayer(this);
		portrait.color = newChar.GetColor();
		classText.text = newChar.GetClass();
		assignedMerc = newChar;
	}

	public List<PrepCard> GetMercsWeaponPrepCards()
	{
		return assignedMerc.GetWeaponPrepCards();
	}

	public List<PrepCard> GetMercsClassPrepCards()
	{
		return assignedMerc.GetClassPrepCards();
	}

	public List<CombatCard> GetCurrentDeckCards()
	{
		return currentCharacterDeck.GetDeckCards();
	}
	public override void GenerateCombatStartDeck()
	{
		int size = CombatManager.startingHandSize;
		currentCharacterDeck = new CombatDeck();
		CombatCard[] startingDeckCards = assignedMerc.GetCombatDeck().DrawCards(size).ToArray();
		currentCharacterDeck.AddCards(startingDeckCards);
		handManager.AssignDeck(currentCharacterDeck);
		assignedMerc.GetCombatDeck().DiscardCards(startingDeckCards);
	}


	public void TryDrawNewCardToHand()
	{
		if (HasTurn())
		{
			handManager.HideDisplayedHand();
			handManager.DrawCardsToHand(1);
			handManager.DisplayHand(true);
		}
	}

	public void HideMyHand()
	{
		handManager.HideDisplayedHand();
	}

	public void SetMyHandInteractivity(bool interactive)
	{
		handManager.SetHandInteractivity(interactive);
	}


	public void SetPortraitClickable(bool clickable)
	{
		portrait.raycastTarget = clickable;
		portrait.GetComponent<Button>().interactable = clickable;
	}

	public override void RemoveTurn()
	{
		base.RemoveTurn();
		handManager.DiscardCurrentHand();
		handManager.HideDisplayedHand();
	}

}
