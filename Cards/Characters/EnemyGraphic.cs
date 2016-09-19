using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGraphic : CharacterGraphic 
{

	public override void GenerateCombatStartDeck()
	{
		currentCharacterDeck = new CombatDeck();
		handManager.AssignDeck(currentCharacterDeck);
		ApplyVariationCardsToBasicDeck(1);
		
	}

	void ApplyVariationCardsToBasicDeck(int count)
	{
		EncounterEnemy assignedEnemy = assignedCharacter as EncounterEnemy;
		List<EnemyVariationCard> variationCards = assignedEnemy.GetRandomVariationCards(count);
		CombatCard[] basicDeckCards = assignedCharacter.GetCombatDeck().GetDeckCards().ToArray();
		foreach (EnemyVariationCard card in variationCards)
		{
			card.AddEnemysBasicCombatDeck(basicDeckCards);
			TryPlaceCharacterStipulationCard(card);
		}
	}

	public virtual void StartedTurn()
	{
		handManager.DiscardCurrentHand();
		handManager.DrawCardsToHand(CombatManager.enemyHandSize);
	}

	public List<CombatCard> GetCharacterHand()
	{
		return handManager.GetHand();
	}
	/*
	public void RemovePlayedCardFromHand(CombatCard card)
	{
		handManager.RemoveCardFromHand(card);
	}*/

}
