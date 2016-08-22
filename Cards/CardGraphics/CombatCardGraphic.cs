using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class CombatCardGraphic : CardGraphic, IPointerEnterHandler,IPointerExitHandler
{

	public GameObject staminaCost;
	public GameObject ammoCost;
	public GameObject healthDamage;
	public GameObject staminaDamage;
	public Text cardTypeText;

	public Color effectCardColor;
	public Color rangedCardColor;
	public Color meleeCardColor;

	CombatCard assignedCard;
	HandDisplayer handDisplayer;
	CharacterGraphic myHandOwner;


	public void AssignCard(CombatCard newCard)
	{
		AssignCard(newCard, null);
	}

	public void AssignCard(CombatCard newCard, HandDisplayer handDisplayer)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;
		this.handDisplayer = handDisplayer;

		SetColor();
		SetTypeText();
		
		ShowCosts();
		ShowDamage();

		GetComponent<Button>().onClick.AddListener(() => { CardClicked(); });
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
		else staminaCost.SetActive(false);

		if (assignedCard.ammoCost > 0)
		{
			ammoCost.SetActive(true);
			ammoCost.GetComponentInChildren<Text>().text = assignedCard.ammoCost.ToString();
		}
		else ammoCost.SetActive(false);
	}

	void ShowDamage()
	{
		if (assignedCard.damage > 0)
		{
			healthDamage.SetActive(true);
			healthDamage.GetComponentInChildren<Text>().text = assignedCard.damage.ToString();
		}
		else healthDamage.SetActive(false);

		if (assignedCard.staminaDamage > 0)
		{
			staminaDamage.SetActive(true);
			staminaDamage.GetComponentInChildren<Text>().text = assignedCard.staminaDamage.ToString();
		}
		else staminaDamage.SetActive(false);
	}


	public void SetInteractable(bool interactable)
	{
		GetComponent<Button>().interactable = interactable;
	}


	public void ShowStipulationCardTooltip()
	{
		//if (GetComponent<Button>().IsInteractable())
		//{
			if (assignedCard.addedStipulationCard != null)
				TooltipManager.main.CreateTooltip("", transform, assignedCard.addedStipulationCard);
		//}
	}
	public void StopShowingStipulationCardTooltip()
	{
		TooltipManager.main.StopAllTooltips();
	}

	public void CardClicked()
	{
		if (GetComponent<Button>().IsInteractable() && handDisplayer != null)
			handDisplayer.HandCardClicked(this);
	}

	public CombatCard GetAssignedCard()
	{
		return assignedCard;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (handDisplayer!=null)
			handDisplayer.HandCardHoverStart(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (handDisplayer != null)
			handDisplayer.HandCardHoverEnd(this);
	}
}
