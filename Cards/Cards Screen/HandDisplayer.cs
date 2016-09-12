using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public interface HandDisplayingObject
{
	void DisplayHand(bool interactable, List<CombatCard> cardsInHand);
	void SetHandInteractivity(bool interactable);
	void HideDisplayedHand();
	List<InteractableCombatCard> GetHandGraphics();

}

public class HandDisplayer : MonoBehaviour,HandDisplayingObject {

	public InteractableCombatCard combatCardPrefab;	
	public Transform mainHandGroup;

	HandManager handManager;
	int hoveredCardSiblingIndex = 0;

	public void EnableHandDisplayer(HandManager manager)
	{
		handManager = manager;
	}
	

	public void DisplayHand(bool interactable, List<CombatCard> cardsInHand)
	{
		foreach (CombatCard card in cardsInHand)
		{
			AddNewCombatCardGraphic(card, interactable);
		}
	}

	void AddNewCombatCardGraphic(CombatCard card, bool interactable)
	{
		InteractableCombatCard newCardGraphic = Instantiate(combatCardPrefab);
		newCardGraphic.AssignCard(card,this);
		newCardGraphic.transform.SetParent(mainHandGroup, false);
		newCardGraphic.GetComponent<Button>().interactable = interactable;
	}

	public void HideDisplayedHand()
	{
		foreach (CombatCardGraphic card in mainHandGroup.GetComponentsInChildren<CombatCardGraphic>())
		{
			GameObject.Destroy(card.gameObject);
		}
	}

	public void SetHandInteractivity(bool interactable)
	{
		foreach (InteractableCombatCard card in mainHandGroup.GetComponentsInChildren<InteractableCombatCard>())
		{
			card.GetComponent<Button>().interactable = interactable;
		}
	}

	public List<InteractableCombatCard> GetHandGraphics()
	{
		List<InteractableCombatCard> graphics = new List<InteractableCombatCard>();
		foreach (InteractableCombatCard graphic in mainHandGroup.GetComponentsInChildren<InteractableCombatCard>())
			graphics.Add(graphic);
		return graphics;
	}

	public void HandCardClicked(ICombatCard cardGraphic)
	{
		handManager.TryPlayCardInHand(cardGraphic);
	}
	/*
	public void HandCardHoverStart(CombatCardGraphic cardGraphic)
	{
		hoveredCardSiblingIndex = cardGraphic.transform.GetSiblingIndex();
		mainHandGroup.GetComponent<ContentSizeFitter>().enabled = false;
		mainHandGroup.GetComponent<HorizontalLayoutGroup>().enabled = false;
		cardGraphic.transform.SetAsLastSibling();
		cardGraphic.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void HandCardHoverEnd(CombatCardGraphic cardGraphic)
	{
		cardGraphic.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
		cardGraphic.transform.SetSiblingIndex(hoveredCardSiblingIndex);
		mainHandGroup.GetComponent<HorizontalLayoutGroup>().enabled = true;
		mainHandGroup.GetComponent<ContentSizeFitter>().enabled = true;
	}*/

}
