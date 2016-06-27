using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour 
{
	
	public Text tooltipText;
	public Text widthMeasuringText;
	public Image contentWrapper;
	public Transform cardsDisplayGroup;

	const float maxTooltipWidth = 300;

	const float edgeOffsetSize = 5f;
	float tooltipWidth = 80f;
	const int maxCardsPerRow = 4;

	public CombatCardGraphic combatCardGraphicPrefab;
	public RewardCardGraphic rewardCardGraphicPrefab;

	public void AssignDisplayValues(string text, Transform tooltipParent, CombatCard[] displayCards)
	{
		cardsDisplayGroup.gameObject.SetActive(true);
		cardsDisplayGroup.GetComponent<GridLayoutGroup>().constraintCount = Mathf.Min(displayCards.Length, maxCardsPerRow);
		foreach (CombatCard card in displayCards)
		{
			AddCombatCard(card);
		}
		AssignDisplayValues(text, tooltipParent);
	}

	public void AssignDisplayValues(string text, Transform tooltipParent, RewardCard[] displayCards)
	{
		cardsDisplayGroup.gameObject.SetActive(true);
		cardsDisplayGroup.GetComponent<GridLayoutGroup>().constraintCount = Mathf.Min(displayCards.Length, maxCardsPerRow);
		foreach (RewardCard card in displayCards)
		{
			AddRewardCard(card);
		}
		AssignDisplayValues(text, tooltipParent);
	}

	public void AssignDisplayValues(string text, Transform tooltipParent)
	{
		
		SetPositionAndSortOrder(tooltipParent,text);
		AddText(text);
	}

	void SetPositionAndSortOrder(Transform tooltipParent, string text)
	{
		widthMeasuringText.text = text;
		
		transform.SetParent(tooltipParent, false);
		RectTransform.Edge tooltipSide = RectTransform.Edge.Right;
		Canvas.ForceUpdateCanvases();
		float cardsContentWidth = cardsDisplayGroup.GetComponent<GridLayoutGroup>().preferredWidth;
		float textContentWidth=Mathf.Min(widthMeasuringText.GetComponent<RectTransform>().rect.width,maxTooltipWidth);
		tooltipWidth=Mathf.Max(cardsContentWidth,textContentWidth);
		contentWrapper.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,tooltipWidth);

		if (Camera.main.WorldToScreenPoint(tooltipParent.position - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0)).x
		+ tooltipParent.GetComponent<RectTransform>().rect.width + tooltipWidth + edgeOffsetSize > Screen.width)
			tooltipSide = RectTransform.Edge.Left;
		GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(tooltipSide, -(tooltipWidth + edgeOffsetSize), tooltipWidth);
		GetComponent<Canvas>().enabled = true;
		GetComponent<Canvas>().overrideSorting = true;
		GetComponent<Canvas>().sortingOrder = 100;
	}

	void AddText(string text)
	{
		tooltipText.text = text;
	}

	void AddCombatCard(CombatCard addedCard)
	{
		CombatCardGraphic cardGraphic = Instantiate(combatCardGraphicPrefab);
		cardGraphic.AssignCard(addedCard);
		cardGraphic.GetComponent<Image>().raycastTarget = false;
		cardGraphic.transform.SetParent(cardsDisplayGroup, false);
	}
	void AddRewardCard(RewardCard addedCard)
	{
		RewardCardGraphic cardGraphic = Instantiate(rewardCardGraphicPrefab);
		cardGraphic.AssignCard(addedCard);
		cardGraphic.GetComponent<Image>().raycastTarget = false;
		cardGraphic.transform.SetParent(cardsDisplayGroup, false);
	}

}
