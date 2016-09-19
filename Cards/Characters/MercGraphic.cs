using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MercGraphic : CharacterGraphic
{
	public PrepHandDisplayer prepHandDisplayer;
	
	public Text classText;
	Mercenary assignedMerc;

	public void AssignMerc(Mercenary newMerc,HandManager newHandManager)
	{
		base.AssignCharacter(newMerc);
		//handManager.EnableAttachedHandDisplayer(this);
		portrait.color = newMerc.GetColor();
		classText.text = newMerc.GetClass();
		assignedMerc = newMerc;
		handManager = newHandManager;
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
	//deprecated
	public override void GenerateCombatStartDeck()
	{
		int size = 4;
		currentCharacterDeck = new CombatDeck();
		CombatCard[] startingDeckCards = assignedMerc.GetCombatDeck().DrawCards(size).ToArray();
		currentCharacterDeck.AddCards(startingDeckCards);
		handManager.AssignDeck(currentCharacterDeck);
		assignedMerc.GetCombatDeck().DiscardCards(startingDeckCards);
	}

	//deprecated
	public void TryDrawNewCardsToHand(int drawCount)
	{
		if (HasTurn())
		{
			handManager.HideDisplayedHand();
			handManager.DrawCardsToHand(drawCount);
			handManager.DisplayHand(true);
		}
	}
	//deprecated
	public void DiscardMyHand()
	{
		handManager.DiscardCurrentHand();
	}
	//deprecated
	public void HideMyHand()
	{
		handManager.HideDisplayedHand();
	}
	//deprecated
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
		//handManager.HideDisplayedHand();
	}

}
