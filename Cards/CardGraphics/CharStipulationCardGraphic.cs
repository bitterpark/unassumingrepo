using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharStipulationCardGraphic : CardGraphic {

	public CharacterStipulationCard assignedCard;

	public void AssignCard(CharacterStipulationCard newCard)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;
	}
	
	public void ShowAddedCombatCardsTooltip()
	{
		if (assignedCard.addedCombatCards.Count>0)
			TooltipManager.main.CreateTooltip("", transform, assignedCard.addedCombatCards.ToArray());
	}
	public void StopShowingShowAddedCombatCardsTooltip()
	{
		TooltipManager.main.StopAllTooltips();
	}
}
