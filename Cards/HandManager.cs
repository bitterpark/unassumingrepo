using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandManager: MonoBehaviour{

	public HandDisplayer handDisplayer;
	protected List<CombatCard> cardsInHand = new List<CombatCard>();
	protected CombatDeck assignedDeck;
	//CharacterGraphic handOwner;
	
	public void AssignDeck(CombatDeck deck)
	{
		assignedDeck = deck;
	}

	public void AddCardsToAssignedDeck(params CombatCard[] cards)
	{
		if (assignedDeck == null)
			throw new System.Exception("Cannot add cards: assigned deck is null!");
		assignedDeck.AddCards(cards);

	}

	public void RemoveCardsFromAssignedDeck(params CombatCard[] cards)
	{
		if (assignedDeck == null)
			throw new System.Exception("Cannot remove cards: assigned deck is null!");
		assignedDeck.RemoveCards(cards);

	}


	public void TryPlayCardInHand(ICombatCard cardObject)
	{
		CombatManager.main.StartCombatCardPlay(cardObject);
		//CombatCardTargeter.main.try
		//if (CombatManager.main.TryPlayCombatCard(cardObject, handOwner))
			//RemoveCardFromHand(cardObject.GetAssignedCard());
	}

	public void DrawCardsToHand(int count)
	{
		cardsInHand.AddRange(assignedDeck.DrawCards(count, true));
	}

	public void DiscardCardsFromHand(int count, bool startFromLeftmost)
	{
		if (startFromLeftmost)
			DiscardLeftmostHandCards(count);
		else
			DiscardRightmostHandCards(count);
	}

	void DiscardLeftmostHandCards(int discardCount)
	{
		for (int i = 0; i < discardCount; i++)
		{
			if (cardsInHand.Count > 0)
				DiscardCardFromHand(cardsInHand[0]);
			else
				break;
		}
	}
	void DiscardRightmostHandCards(int discardCount)
	{
		for (int i = 0; i < discardCount; i++)
		{
			if (cardsInHand.Count > 0)
				DiscardCardFromHand(cardsInHand[cardsInHand.Count-1]);
			else
				break;
		}
	}

	public void DiscardCurrentHand()
	{
		foreach (CombatCard card in new List<CombatCard>(cardsInHand))
			DiscardCardFromHand(card);
	}

	public void DiscardCardFromHand(CombatCard card)
	{
		cardsInHand.Remove(card);
		assignedDeck.DiscardCards(card);
	}

	

	public List<CombatCard> GetHand()
	{
		return cardsInHand;
	}

	public void EnableAttachedHandDisplayer()//CharacterGraphic handOwner)
	{
		//this.handOwner = handOwner;
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
