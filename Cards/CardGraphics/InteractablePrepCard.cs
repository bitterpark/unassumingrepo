using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractablePrepCard : InteractableCard {

	public PrepCardGraphic bigCardPrefab;
	PrepCard assignedCard;
	PrepHandDisplayer assignedDisplayer;

	public void AssignCard(PrepCard newCard, PrepHandDisplayer assignedDisplayer)
	{
		SetupButton();
		assignedCard = newCard;
		GetComponent<PrepCardGraphic>().AssignCard(newCard);
		this.assignedDisplayer = assignedDisplayer;
	}

	protected override Transform CreateBigCardGraphic()
	{
		PrepCardGraphic newBigCardGraphic = Instantiate(bigCardPrefab);
		newBigCardGraphic.AssignCard(assignedCard);
		return newBigCardGraphic.transform;
	}

	protected override void CardClicked()
	{
		if (GetComponent<Button>().IsInteractable() && assignedDisplayer != null)
			assignedDisplayer.PrepCardClicked(this);
	}

	public void PlayAssignedCard(CharacterGraphic playToCharacter)
	{
		assignedCard.PlayCard(playToCharacter);
	}
}
