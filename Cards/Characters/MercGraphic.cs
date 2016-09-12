using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MercGraphic : CharacterGraphic
{
	public PrepHandDisplayer prepHandDisplayer;
	
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

	public void StartPrepDisplay()
	{
		prepHandDisplayer.StartPrepHandDisplay();
	}
	
	public void FinishPrepDisplay()
	{
		prepHandDisplayer.HidePrepHand();
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
		int size = 4;
		currentCharacterDeck = new CombatDeck();
		CombatCard[] startingDeckCards = assignedMerc.GetCombatDeck().DrawCards(size).ToArray();
		currentCharacterDeck.AddCards(startingDeckCards);
		handManager.AssignDeck(currentCharacterDeck);
		assignedMerc.GetCombatDeck().DiscardCards(startingDeckCards);
	}


	public void TryDrawNewCardsToHand(int drawCount)
	{
		if (HasTurn())
		{
			handManager.HideDisplayedHand();
			handManager.DrawCardsToHand(drawCount);
			handManager.DisplayHand(true);
		}
	}

	public void DiscardMyHand()
	{
		handManager.DiscardCurrentHand();
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
		handManager.HideDisplayedHand();
	}

}
