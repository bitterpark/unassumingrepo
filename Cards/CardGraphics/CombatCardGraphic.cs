using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CombatCardGraphic : CardGraphic
{

	public GameObject staminaCost;
	public GameObject ammoCost;
	public GameObject healthDamage;
	public GameObject staminaDamage;
	public Text cardTypeText;


	public CombatCard assignedCard;

	public void AssignCard(CombatCard newCard)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;

		cardTypeText.text = assignedCard.cardType.ToString().Replace('_',' ');

		if (newCard.staminaCost > 0)
		{
			staminaCost.SetActive(true);
			staminaCost.GetComponentInChildren<Text>().text = newCard.staminaCost.ToString();
		}
		else staminaCost.SetActive(false);
		if (newCard.ammoCost > 0)
		{
			ammoCost.SetActive(true);
			ammoCost.GetComponentInChildren<Text>().text = newCard.ammoCost.ToString();
		}
		else ammoCost.SetActive(false);
		if (newCard.healthDamage > 0)
		{
			healthDamage.SetActive(true);
			healthDamage.GetComponentInChildren<Text>().text = newCard.healthDamage.ToString();
		}
		else healthDamage.SetActive(false);
		if (newCard.staminaDamage > 0)
		{
			staminaDamage.SetActive(true);
			staminaDamage.GetComponentInChildren<Text>().text = newCard.staminaDamage.ToString();
		}
		else staminaDamage.SetActive(false);

		GetComponent<Button>().onClick.AddListener(() => { CardClicked(); });
	}

	public void CardClicked()
	{
		CardsScreen.main.PlayerTurnProcess(this);
	}
}
