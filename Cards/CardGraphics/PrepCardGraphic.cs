using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PrepCardGraphic : CardGraphic {

	PrepCard assignedCard;
	
	public void AssignCard(PrepCard newCard)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;
		GetComponent<Button>().onClick.AddListener(CardClicked);
	}

	public void SetInteractable(bool interactable)
	{
		GetComponent<Button>().interactable = interactable;
	}

	public void PlayAssignedCard(CharacterGraphic playToCharacter)
	{
		assignedCard.PlayCard(playToCharacter);
	}

	public void CardClicked()
	{
		if (GetComponent<Button>().IsInteractable())
			PrepCardManager.main.PlayPrepCard(this);
	}

	public void ShowAddedCombatCardsTooltip()
	{
		List<CombatCard> addedCombatCards=assignedCard.GetAddedCombatCards();
		if (addedCombatCards.Count > 0)
			TooltipManager.main.CreateTooltip("", transform, addedCombatCards.ToArray());
	}
	public void StopShowingShowAddedCombatCardsTooltip()
	{
		TooltipManager.main.StopAllTooltips();
	}
}
