using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RewardCardGraphic : CardGraphic 
{
	public Text effectDescription;

	public RewardCard assignedCard;

	public void AssignCard(RewardCard newCard)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;
		effectDescription.text = newCard.effectDescription;
		GetComponent<Button>().onClick.AddListener(() => CardClicked());
	}

	void CardClicked()
	{
		CardsScreen.main.RewardCardPlay(this);
	}
	
}
