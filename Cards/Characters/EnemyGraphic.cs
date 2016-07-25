using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGraphic : CharacterGraphic 
{

	public override void GenerateCombatStartDeck()
	{
		currentCharacterDeck = new CombatDeck();
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

	public override void StartedTurn()
	{
		base.StartedTurn();
		DisplayMyHand(false);
	}

	
}
