using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EncounterCardGraphic :CardGraphic 
{
	public RoomCard assignedCard;
	
	public void AssignCard(RoomCard newCard)
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
