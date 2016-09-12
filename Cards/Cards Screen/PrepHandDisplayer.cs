using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PrepHandDisplayer : MonoBehaviour 
{

	public InteractablePrepCard prepCardPrefab;
	public MercGraphic handOwner;

	public Transform mainHandGroup;

	enum PrepMode { SelectClassCard, SelectWeaponCard };
	PrepMode currentPrepMode;

	public void StartPrepHandDisplay()
	{
		DisplayPrepHand(handOwner.GetMercsClassPrepCards());
		currentPrepMode = PrepMode.SelectClassCard;
	}

	public void PrepCardClicked(InteractablePrepCard clickedCardObject)
	{
		//cardGraphic.transform.SetParent(prepCardsDisplayArea, false);
		clickedCardObject.PlayAssignedCard(handOwner);
		TransferPrepMode();
	}

	void TransferPrepMode()
	{
		HidePrepHand();
		if (currentPrepMode == PrepMode.SelectClassCard)
		{
			currentPrepMode = PrepMode.SelectWeaponCard;
			DisplayPrepHand(handOwner.GetMercsWeaponPrepCards());
		}
			
	}

	void DisplayPrepHand(List<PrepCard> cardsInHand)
	{
		foreach (PrepCard card in cardsInHand)
		{
			InteractablePrepCard newCardObject = Instantiate(prepCardPrefab);
			newCardObject.AssignCard(card, this);
			newCardObject.transform.SetParent(mainHandGroup, false);
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
