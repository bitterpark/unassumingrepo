using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class CombatCardGraphic : CardGraphic, ICombatCard//, IPointerDownHandler
{

	public GameObject staminaCost;
	public GameObject ammoCost;
	public GameObject nonStaminaDamage;
	public GameObject damagePerStaminaPoint;
	public Text cardTypeText;

	public Color effectCardColor;
	public Color rangedCardColor;
	public Color meleeCardColor;

	CombatCard assignedCard;

	public void AssignCard(CombatCard newCard)
	{
		AssignCard(newCard,false);
	}

	public void AssignCard(CombatCard newCard, bool showStipulationCardTooltip)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;

		SetColor();
		SetTypeText();
		
		ShowCosts();
		ShowDamage();

		description.raycastTarget = showStipulationCardTooltip;
	}

	void SetColor()
	{
		if (assignedCard.GetType().BaseType == typeof(EffectCard))
			GetComponent<Image>().color = effectCardColor;
		if (assignedCard.GetType().BaseType == typeof(RangedCard))
			GetComponent<Image>().color = rangedCardColor;
		if (assignedCard.GetType().BaseType == typeof(MeleeCard))
			GetComponent<Image>().color = meleeCardColor;
	}

	void SetTypeText()
	{
		cardTypeText.text = assignedCard.cardType.ToString().Replace('_', ' ');
	}

	void ShowCosts()
	{
		
		if (assignedCard.staminaCost > 0)
		{
			staminaCost.SetActive(true);
			staminaCost.GetComponentInChildren<Text>().text = assignedCard.staminaCost.ToString();
		}
		else 
			staminaCost.SetActive(false);

		if (assignedCard.ammoCost > 0)
		{
			ammoCost.SetActive(true);
			ammoCost.GetComponentInChildren<Text>().text = assignedCard.ammoCost.ToString();
		}
		else 
			ammoCost.SetActive(false);
	}

	void ShowDamage()
	{
		if (assignedCard.damage > 0)
		{
			nonStaminaDamage.SetActive(true);
			nonStaminaDamage.GetComponentInChildren<Text>().text = assignedCard.damage.ToString();
		}
		else nonStaminaDamage.SetActive(false);

		if (assignedCard.damagePerStaminaPoint > 0)
		{
			damagePerStaminaPoint.SetActive(true);
			damagePerStaminaPoint.GetComponentInChildren<Text>().text = assignedCard.damagePerStaminaPoint.ToString();
		}
		else damagePerStaminaPoint.SetActive(false);
	}

	public void ShowStipulationCardTooltip()
	{
		if (assignedCard.addedStipulationCard != null)
			TooltipManager.main.CreateTooltip("", transform, assignedCard.addedStipulationCard);
	}
	public void StopShowingStipulationCardTooltip()
	{
		TooltipManager.main.StopAllTooltips();
	}

	public CombatCard GetAssignedCard()
	{
		return assignedCard;
	}

	public Transform GetTransform()
	{
		return transform;
	}
}
