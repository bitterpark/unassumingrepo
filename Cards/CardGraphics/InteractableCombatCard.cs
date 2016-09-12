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

	public CombatCardGraphic bigCardPrefab;
	HandDisplayer handDisplayer;
	CombatCard assignedCard;

	public void AssignCard(CombatCard newCard, HandDisplayer handDisplayer)
	{
		assignedCard = newCard;
		SetupButton();
		this.handDisplayer = handDisplayer;
		GetComponent<CombatCardGraphic>().AssignCard(newCard,false);
	}

	protected override Transform CreateBigCardGraphic()
	{
		CombatCardGraphic newBigCardGraphic = Instantiate(bigCardPrefab);
		newBigCardGraphic.AssignCard(assignedCard);
		return newBigCardGraphic.transform;
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
