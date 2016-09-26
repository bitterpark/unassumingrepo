using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHandManager : HandManager
{
	public static PlayerHandManager main;
	
	const int startingHandSize = 6;
	const int extraDrawPerTurn = 3;
	const int maxHandSize = 10;
	MissionCharacterManager characterManager;

	//public HandManager commonHandManager;

	public void EnableManager(MissionCharacterManager characterManager)
	{
		this.characterManager = characterManager;
		EnableAttachedHandDisplayer();
		main = this;
	}

	public void DrawCombatStartHand()
	{
		if (cardsInHand.Count>0)
			DiscardCurrentHand();
		CreateCombatStartDeck();
		DrawNewCardsToCommonHand(startingHandSize-extraDrawPerTurn);
	}

	void CreateCombatStartDeck()
	{
		List<CombatCard> allCombatCards = GenericCombatCards.GetDefaultCommonDeckCards();//new List<CombatCard>();
		/*
		foreach (CharacterGraphic character in characterManager.GetMercGraphics())
		{
			MercGraphic merc = character as MercGraphic;
			CombatDeck newDeck = merc.GetCharactersCombatDeck();
			allCombatCards.AddRange(newDeck.GetDeckCards());
		}*/

		CombatDeck commonDeck = new CombatDeck();
		commonDeck.AddCards(allCombatCards.ToArray());
		AssignDeck(commonDeck);
	}

	public void DrawNewCardsToCommonHand(int cardsCount)
	{
		int newCardsDrawn = cardsCount;
		if (cardsInHand.Count + newCardsDrawn > maxHandSize)
			newCardsDrawn = maxHandSize - cardsInHand.Count;

		if (newCardsDrawn > 0)
		{
			HideDisplayedHand();
			DrawCardsToHand(newCardsDrawn);
			DisplayHand(true);
		}
	}

	public void NewPlayerTurnStart()
	{
		DrawNewCardsToCommonHand(extraDrawPerTurn);
	}

	public void DiscardLeftmostCards(int discardCount)
	{
		HideDisplayedHand();
		DiscardCardsFromHand(discardCount, true);
		DisplayHand(true);

	}

	//deprecated
	public void DrawCardsForActiveMercs(int drawCount)
	{
		HideDisplayedHand();
		List<CombatCard> playerHand = new List<CombatCard>();

		foreach (CharacterGraphic character in characterManager.GetMercGraphics())
		{
			MercGraphic merc = character as MercGraphic;
			merc.TryDrawNewCardsToHand(drawCount);
		}
	}
}
