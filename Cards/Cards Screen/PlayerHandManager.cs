using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHandManager : HandManager
{	
	const int startingHandSize = 6;
	const int extraDrawPerTurn = 3;
	const int maxHandSize = 10;
	MissionCharacterManager characterManager;
	//public HandManager commonHandManager;

	public void EnableManager(MissionCharacterManager characterManager)
	{
		this.characterManager = characterManager;
		EnableAttachedHandDisplayer();
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
		List<CombatCard> allCombatCards = new List<CombatCard>();
		foreach (CharacterGraphic character in characterManager.GetMercGraphics())
		{
			MercGraphic merc = character as MercGraphic;
			CombatDeck newDeck = merc.GetCharactersCombatDeck();
			allCombatCards.AddRange(newDeck.GetDeckCards());
		}

		CombatDeck commonDeck = new CombatDeck();
		commonDeck.AddCards(allCombatCards.ToArray());
		AssignDeck(commonDeck);
	}

	void DrawNewCardsToCommonHand(int cardsCount)
	{
		HideDisplayedHand();
		DrawCardsToHand(cardsCount);
		DisplayHand(true);
	}

	public void NewPlayerTurnStart()
	{
		int newCardsDrawn = extraDrawPerTurn;
		if (cardsInHand.Count + newCardsDrawn > maxHandSize)
			newCardsDrawn = maxHandSize - cardsInHand.Count;
		
		if (newCardsDrawn>0)
			DrawNewCardsToCommonHand(extraDrawPerTurn);
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
