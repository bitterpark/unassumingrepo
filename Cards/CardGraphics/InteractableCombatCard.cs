using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface ICombatCard
{
	CombatCard GetAssignedCard();
	Transform GetTransform();
}

public class InteractableCombatCard :InteractableCard, ICombatCard
{
	HandDisplayer handDisplayer;
	CombatCard assignedCard;

	public void AssignCard(CombatCard newCard, HandDisplayer handDisplayer)
	{
		assignedCard = newCard;
		SetupButton();
		this.handDisplayer = handDisplayer;
		GetComponent<CombatCardGraphic>().AssignCard(newCard,true);
	}

	public void DescriptionClicked()
	{
		CardClicked();
	}

	protected override void CardClicked()
	{
		if (GetComponent<Button>().IsInteractable() && handDisplayer != null)
			handDisplayer.HandCardClicked(this);
	}


	public CombatCard GetAssignedCard()
	{
		return assignedCard;
	}
	public Transform GetTransform()
	{
		return transform;
	}
}
