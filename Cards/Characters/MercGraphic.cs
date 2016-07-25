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
		int size = CardsScreen.startingHandSize;
		currentCharacterDeck = new CombatDeck();
		CombatCard[] startingDeckCards = assignedMerc.GetCombatDeck().DrawCards(size).ToArray();
		currentCharacterDeck.AddCards(startingDeckCards);
		assignedMerc.GetCombatDeck().DiscardCards(startingDeckCards);
	}

	public override void StartedTurn()
	{
		base.StartedTurn();
		DisplayMyHand(true);
	}

	public void SetPortraitClickable(bool clickable)
	{
		portrait.raycastTarget = clickable;
		portrait.GetComponent<Button>().interactable = clickable;
	}

}
