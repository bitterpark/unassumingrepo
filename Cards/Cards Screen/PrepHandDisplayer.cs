using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PrepHandDisplayer : MonoBehaviour 
{

	public PrepCardGraphic prepCardPrefab;

	public Transform mainHandGroup;

	public void DisplayPrepHand(List<PrepCard> cardsInHand)
	{
		foreach (PrepCard card in cardsInHand)
		{
			PrepCardGraphic newCardGraphic = Instantiate(prepCardPrefab);
			newCardGraphic.AssignCard(card);
			newCardGraphic.transform.SetParent(mainHandGroup, false);
		}
	}

	public void HidePrepHand()
	{
		foreach (PrepCardGraphic card in mainHandGroup.GetComponentsInChildren<PrepCardGraphic>())
		{
			GameObject.Destroy(card.gameObject);
		}
	}

	public void SetPrepHandInteractivity(bool interactable)
	{
		foreach (PrepCardGraphic card in mainHandGroup.GetComponentsInChildren<PrepCardGraphic>())
		{
			card.GetComponent<Button>().interactable = interactable;
		}
	}
}
