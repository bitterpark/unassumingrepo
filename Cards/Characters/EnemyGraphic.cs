using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGraphic : CharacterGraphic 
{

	public void PlayVariationCards(int count)
	{
		EncounterEnemy assignedEnemy = assignedCharacter as EncounterEnemy;
		List<CharacterStipulationCard> variationCards = assignedEnemy.GetRandomVariationCards(count);
		foreach (CharacterStipulationCard card in variationCards)
		{
			TryPlaceCharacterStipulationCard(card);
		}
	}

	public override void StartedTurn()
	{
		base.StartedTurn();
		DisplayMyHand(false);
	}

	public override void GenerateCombatStartDeck()
	{
		currentCharacterDeck = assignedCharacter.GetCombatDeck();
	}
}
