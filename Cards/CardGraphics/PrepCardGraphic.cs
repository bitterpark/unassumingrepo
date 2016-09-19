using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PrepCardGraphic : CardGraphic {

	PrepCard assignedCard;

	public void AssignCard(PrepCard newCard)
	{
		AssignCard(newCard, false);
	}

	public void AssignCard(PrepCard newCard, bool showDescriptionTooltip)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;
		description.raycastTarget = showDescriptionTooltip;
	}

	public void ShowAddedCombatCardsTooltip()
	{
		List<CombatCard> addedCombatCards=assignedCard.GetAddedCombatCards();
		if (addedCombatCards.Count > 0)
			TooltipManager.main.CreateTooltip("", transform, addedCombatCards.ToArray());
		else
		{
			CharacterStipulationCard placedStipulationCard = assignedCard.GetPlacedStipulationCard();
			if (placedStipulationCard!=null)
				TooltipManager.main.CreateTooltip("", transform,placedStipulationCard);
		}
	}
	public void StopShowingShowAddedCombatCardsTooltip()
	{
		TooltipManager.main.StopAllTooltips();
	}
}
