using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardGraphic : MonoBehaviour {

	public Image image;
	public Text cardName;
	public Text description;

	protected void UpdateBasicVisuals(Card newCard)
	{
		cardName.text = newCard.name;
		image.sprite = newCard.image;
		description.text = newCard.description;
	}
}
