using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VisualCardGraphic :CardGraphic 
{
	public void AssignCard(Card newCard)
	{
		base.UpdateBasicVisuals(newCard);
	}
		
}
