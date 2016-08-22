using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICombatCardSelector : MonoBehaviour {

	CombatManager combatManager;

	public void EnableCombatCardSelector(CombatManager combatManager)
	{
		this.combatManager = combatManager;
	}

	public bool TrySelectCardToPlay(out CombatCard playedCard, CharacterGraphic character)
	{
		List<CombatCard> playableCards = SortOutPlayableEnemyCards(character);
		if (playableCards.Count > 0)
		{
			playedCard = SelectPriorityCard(playableCards);
			playedCard.SetUserChar(character);
			EnemyGraphic enemy = character as EnemyGraphic;
			enemy.RemovePlayedCardFromHand(playedCard);
			return true;
		}
		else
		{
			playedCard = null;
			return false;
		}
	}
	
	CombatCard SelectPriorityCard(List<CombatCard> availableCards)
	{
		CombatCard priorityCard=null;
		int maxAmmoCost = 0;
		foreach (CombatCard card in availableCards)
		{
			if (card.ammoCost > maxAmmoCost)
			{
				priorityCard = card;
				maxAmmoCost = card.ammoCost;
			}
			else
			{
				if (card.ammoCost == maxAmmoCost)
				{
					if (Random.value > 0.5f)
						priorityCard = card;
				}
			}
		}
		//If no ammo-based card was chosen, choose a stamina-based card instead
		if (maxAmmoCost == 0)
		{
			int maxStaminaCost = 0;
			foreach (CombatCard card in availableCards)
			{
				if (card.staminaCost > maxStaminaCost)
				{
					priorityCard = card;
					maxStaminaCost = card.staminaCost;
				}
				else
				{
					if (card.staminaCost == maxStaminaCost)
					{
						if (Random.value > 0.5f)
							priorityCard = card;
					}
				}
			}
		}
		return priorityCard;
	}

	List<CombatCard> SortOutPlayableEnemyCards(CharacterGraphic character)
	{
		List<CombatCard> playableCards = new List<CombatCard>();
		EnemyGraphic enemy = character as EnemyGraphic;
		List<CombatCard> cardsInHand = enemy.GetCharacterHand();

		if (cardsInHand.Count > 0)
		{
			foreach (CombatCard card in cardsInHand)
			{
				if (combatManager.CombatCardRequirementsMet(card) && character.CharacterMeetsCardRequirements(card))
					playableCards.Add(card);
			}
		}
		return playableCards;
	}
}
