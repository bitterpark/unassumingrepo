using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public interface HandDisplayingObject
{
	void DisplayHand(bool interactable, List<CombatCard> cardsInHand);
	void SetHandInteractivity(bool interactable);
	void HideDisplayedHand();
	List<CombatCardGraphic> GetHandGraphics();

}

public class HandDisplayer : MonoBehaviour,HandDisplayingObject {

	public CombatCardGraphic combatCardPrefab;	
	public Transform mainHandGroup;

	CharacterGraphic handOwner;
	int hoveredCardSiblingIndex = 0;

	public void EnableHandDisplayer(CharacterGraphic owner)
	{
		handOwner = owner;
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
		CombatCardGraphic newCardGraphic = Instantiate(combatCardPrefab);
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
		foreach (CombatCardGraphic card in mainHandGroup.GetComponentsInChildren<CombatCardGraphic>())
		{
			card.GetComponent<Button>().interactable = interactable;
		}
	}

	public List<CombatCardGraphic> GetHandGraphics()
	{
		List<CombatCardGraphic> graphics = new List<CombatCardGraphic>();
		foreach (CombatCardGraphic graphic in mainHandGroup.GetComponentsInChildren<CombatCardGraphic>())
			graphics.Add(graphic);
		return graphics;
	}

	public void HandCardClicked(CombatCardGraphic cardGraphic)
	{
		CombatManager.main.TryPlayCombatCard(cardGraphic,handOwner);
	}

	public void HandCardHoverStart(CombatCardGraphic cardGraphic)
	{
		hoveredCardSiblingIndex = cardGraphic.transform.GetSiblingIndex();
		mainHandGroup.GetComponent<ContentSizeFitter>().enabled = false;
		mainHandGroup.GetComponent<HorizontalLayoutGroup>().enabled = false;
		cardGraphic.transform.SetAsLastSibling();
		cardGraphic.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
	}

	public void HandCardHoverEnd(CombatCardGraphic cardGraphic)
	{
		cardGraphic.transform.localScale = new Vector3(1f, 1f, 1f);
		cardGraphic.transform.SetSiblingIndex(hoveredCardSiblingIndex);
		mainHandGroup.GetComponent<HorizontalLayoutGroup>().enabled = true;
		mainHandGroup.GetComponent<ContentSizeFitter>().enabled = true;

	}

}
