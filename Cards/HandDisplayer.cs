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
	public PrepCardGraphic prepCardPrefab;

	public void DisplayPrepHand(List<PrepCard> cardsInHand)
	{
		foreach (PrepCard card in cardsInHand)
		{
			PrepCardGraphic newCardGraphic = Instantiate(prepCardPrefab);
			newCardGraphic.AssignCard(card);
			newCardGraphic.transform.SetParent(transform, false);
		}
	}


	public void HidePrepHand()
	{
		foreach (PrepCardGraphic card in transform.GetComponentsInChildren<PrepCardGraphic>())
		{
			GameObject.Destroy(card.gameObject);
		}
	}

	public void SetPrepHandInteractivity(bool interactable)
	{
		foreach (PrepCardGraphic card in transform.GetComponentsInChildren<PrepCardGraphic>())
		{
			card.GetComponent<Button>().interactable = interactable;
		}
	}

	public void DisplayHand(bool interactable, List<CombatCard> cardsInHand)
	{
		foreach (CombatCard card in cardsInHand)
		{
			CombatCardGraphic newCardGraphic = Instantiate(combatCardPrefab);
			newCardGraphic.AssignCard(card);
			newCardGraphic.transform.SetParent(transform, false);
			newCardGraphic.GetComponent<Button>().interactable = interactable;
		}
	}

	public void SetHandInteractivity(bool interactable)
	{
		foreach (CombatCardGraphic card in transform.GetComponentsInChildren<CombatCardGraphic>())
		{
			card.GetComponent<Button>().interactable = interactable;
		}
	}

	public void HideDisplayedHand()
	{
		foreach (CombatCardGraphic card in transform.GetComponentsInChildren<CombatCardGraphic>())
		{
			GameObject.Destroy(card.gameObject);
		}
	}

	public List<CombatCardGraphic> GetHandGraphics()
	{
		List<CombatCardGraphic> graphics = new List<CombatCardGraphic>();
		foreach (CombatCardGraphic graphic in transform.GetComponentsInChildren<CombatCardGraphic>())
			graphics.Add(graphic);
		return graphics;
	}
	
}
