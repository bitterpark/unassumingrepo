using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandManager: MonoBehaviour{

	public HandDisplayer handDisplayer;
	List<CombatCard> cardsInHand = new List<CombatCard>();
	CombatDeck assignedDeck;
	CharacterGraphic handOwner;
	
	public void AssignDeck(CombatDeck deck)
	{
		assignedDeck = deck;
	}

	public void TryPlayCardInHand(ICombatCard cardObject)
	{
		if (CombatManager.main.TryPlayCombatCard(cardObject, handOwner))
			RemoveCardFromHand(cardObject.GetAssignedCard());
	}

	public void DrawCardsToHand(int count)
	{
		cardsInHand.AddRange(assignedDeck.DrawCards(count, true));
	}

	public void RemoveCardFromHand(CombatCard card)
	{
		cardsInHand.Remove(card);
	}

	public void DiscardCurrentHand()
	{
		assignedDeck.DiscardCards(cardsInHand.ToArray());
		cardsInHand.Clear();
	}

	public List<CombatCard> GetHand()
	{
		return cardsInHand;
	}

	public void EnableHandDisplayer(CharacterGraphic handOwner)
	{
		this.handOwner = handOwner;
		handDisplayer.EnableHandDisplayer(this);
	}

	/*
	void RefreshHand(bool interactable)
	{
		HideDisplayedHand();
		DisplayHand(interactable);
	}*/

	public void DisplayHand(bool interactable)
	{
		handDisplayer.DisplayHand(interactable, cardsInHand);
	}

	public void HideDisplayedHand()
	{
		handDisplayer.HideDisplayedHand();
	}

	public void SetHandInteractivity(bool interactable)
	{
		handDisplayer.SetHandInteractivity(interactable);
	}

}
