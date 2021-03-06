﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractablePrepCard : InteractableCard {

	PrepCard assignedCard;
	PrepHandDisplayer assignedDisplayer;

	public void AssignCard(PrepCard newCard, PrepHandDisplayer assignedDisplayer)
	{
		SetupButton();
		assignedCard = newCard;
		GetComponent<PrepCardGraphic>().AssignCard(newCard,true);
		this.assignedDisplayer = assignedDisplayer;
	}

	public void DescriptionClicked()
	{
		CardClicked();
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
