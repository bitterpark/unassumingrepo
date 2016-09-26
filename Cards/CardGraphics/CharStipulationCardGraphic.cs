using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharStipulationCardGraphic : CardGraphic,IBigCardSpawnable {

	public CharStipulationCardGraphic bigCardPrefab;
	public CharacterStipulationCard assignedCard;

	public void AssignCard(CharacterStipulationCard newCard)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;
	}
	
	public void ShowAddedCombatCardsTooltip()
	{
		if (assignedCard.addedCombatCards.Count > 0)
			StartCoroutine("SpawnTooltipOnBigCard");
	}

	IEnumerator SpawnTooltipOnBigCard()
	{
		Transform bigCardTransform;
		while (!GetComponent<MiniaturizedCard>().CanGetCreatedBigCardTransform(out bigCardTransform))
			yield return new WaitForEndOfFrame();
		TooltipManager.main.CreateTooltip("", bigCardTransform, assignedCard.addedCombatCards.ToArray());
	}

	public void StopShowingShowAddedCombatCardsTooltip()
	{
		StopCoroutine("SpawnTooltipOnBigCard");
		TooltipManager.main.StopAllTooltips();
	}

	public Transform CreateBigCardGraphic()
	{
		CharStipulationCardGraphic newBigCardGraphic = Instantiate(bigCardPrefab);
		newBigCardGraphic.AssignCard(assignedCard);
		return newBigCardGraphic.transform;
	}
}
