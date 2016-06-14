using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VisualCardGraphic :CardGraphic 
{
	public Card assignedCard;
	
	public void AssignCard(Card newCard)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;
	}
		
}
