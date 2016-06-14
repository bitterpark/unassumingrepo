using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EncounterCardGraphic :CardGraphic 
{
	public EncounterCard assignedCard;
	
	public void AssignCard(EncounterCard newCard)
	{
		base.UpdateBasicVisuals(newCard);
		GetComponent<Button>().onClick.AddListener(() => CardClicked());
		assignedCard = newCard;
	}

	void CardClicked()
	{
		CardsScreen.main.PlayEncounterCard(this);
	}

}
