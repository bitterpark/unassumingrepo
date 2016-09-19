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
			playedCard = SelectPriorityCard(playableCards, character);
			print("Setting AI card user char");
			playedCard.SetUserChar(character);
			EnemyGraphic enemy = character as EnemyGraphic;
			//enemy.RemovePlayedCardFromHand(playedCard);
			
			return true;
		}
		else
		{
			playedCard = null;
			return false;
		}
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
				if (combatManager.EligibleCombatCardType(card) && character.CharacterMeetsCardRequirements(card))
					playableCards.Add(card);
			}
		}
		return playableCards;
	}

	CombatCard SelectPriorityCard(List<CombatCard> availableCards, CharacterGraphic character)
	{
		CombatCard priorityCard=null;
		
		int maxStaminaCost = 0;
		foreach (CombatCard card in availableCards)
		{
			int realCardStaminaCost = card.staminaCost;
			if (card.useUpAllStamina)
				realCardStaminaCost = character.GetStamina();
			if (realCardStaminaCost > maxStaminaCost)
			{
				priorityCard = card;
				maxStaminaCost = realCardStaminaCost;
			}
			else
			{
				if (realCardStaminaCost == maxStaminaCost)
				{
					if (Random.value > 0.5f)
						priorityCard = card;
				}
			}
		}
		//If no stamina-based card was chosen, choose an ammo-based card instead
		if (priorityCard == null)
		{
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
		}


		return priorityCard;
	}
}
