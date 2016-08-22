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

	const float horizontalOffsetFromParentEdge = 10f;
	const float verticalOffsetFromScreenEdge = 20f;
	float tooltipWidth = 80f;
	const int maxCardsPerRow = 4;

	public CombatCardGraphic combatCardGraphicPrefab;
	public RewardCardGraphic rewardCardGraphicPrefab;
	public VisualCardGraphic visualCardGraphicPrefab;

	public void DisplayValuesAndCombatCards(string text, Transform tooltipParent, CombatCard[] displayCards)
	{
		cardsDisplayGroup.gameObject.SetActive(true);
		cardsDisplayGroup.GetComponent<GridLayoutGroup>().constraintCount = Mathf.Min(displayCards.Length, maxCardsPerRow);
		foreach (CombatCard card in displayCards)
		{
			AddCombatCard(card);
		}
		AssignDisplayValues(text, tooltipParent);
	}

	public void DisplayValuesAndRewardCards(string text, Transform tooltipParent, RewardCard[] displayCards)
	{
		cardsDisplayGroup.gameObject.SetActive(true);
		cardsDisplayGroup.GetComponent<GridLayoutGroup>().constraintCount = Mathf.Min(displayCards.Length, maxCardsPerRow);
		foreach (RewardCard card in displayCards)
		{
			AddRewardCard(card);
		}
		AssignDisplayValues(text, tooltipParent);
	}

	public void DisplayValuesAndVisualCards(string text, Transform tooltipParent, Card[] displayCards)
	{
		cardsDisplayGroup.gameObject.SetActive(true);
		cardsDisplayGroup.GetComponent<GridLayoutGroup>().constraintCount = Mathf.Min(displayCards.Length, maxCardsPerRow);
		foreach (Card card in displayCards)
		{
			AddVisualCard(card);
		}
		AssignDisplayValues(text, tooltipParent);
	}

	public void AssignDisplayValues(string text, Transform tooltipParent)
	{
		SetupTextComponent(text);
		SetPositionAndSortOrder(tooltipParent,text);
	}


	void SetPositionAndSortOrder(Transform tooltipParent, string text)
	{
		GetComponent<Canvas>().worldCamera = Camera.main;
		widthMeasuringText.text = text;
		transform.SetParent(tooltipParent, false);

		Canvas.ForceUpdateCanvases();
		SetWidth();
		SetHorizontalPositionToLeftOrRight();
		ClampVerticalPositionToScreen();
		
		GetComponent<Canvas>().enabled = true;
		GetComponent<Canvas>().overrideSorting = true;
		GetComponent<Canvas>().sortingOrder = 100;
	}

	void SetWidth()
	{
		float cardsContentWidth = cardsDisplayGroup.GetComponent<GridLayoutGroup>().preferredWidth;
		float textContentWidth = Mathf.Min(widthMeasuringText.GetComponent<RectTransform>().rect.width, maxTooltipWidth);
		tooltipWidth = Mathf.Max(cardsContentWidth, textContentWidth);
		contentWrapper.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tooltipWidth);
	}

	void SetHorizontalPositionToLeftOrRight()
	{
		RectTransform.Edge tooltipSide;
		
		float newX = transform.parent.position.x + (transform.parent.GetComponent<RectTransform>().rect.width) + tooltipWidth + horizontalOffsetFromParentEdge;
		if (Camera.main.WorldToScreenPoint(new Vector3(newX, transform.parent.position.y, 0)).x > Screen.width)
			tooltipSide = RectTransform.Edge.Left;
		else
			tooltipSide = RectTransform.Edge.Right;

		GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(tooltipSide, -(tooltipWidth + horizontalOffsetFromParentEdge), tooltipWidth);
	}
	void ClampVerticalPositionToScreen()
	{
		float cardsContentHeight = cardsDisplayGroup.GetComponent<GridLayoutGroup>().preferredHeight;
		float textContentHeight = tooltipText.preferredHeight;
		float tooltipHeight = cardsContentHeight + textContentHeight;

		float lowestWorldYPoint = transform.parent.position.y - tooltipHeight*0.5f;
		float lowestScreenYPoint = Camera.main.WorldToScreenPoint(new Vector3(transform.parent.position.x, lowestWorldYPoint, 0)).y;
		if (lowestScreenYPoint <= 0)
			transform.position += new Vector3(0, Mathf.Abs(lowestScreenYPoint) + verticalOffsetFromScreenEdge);

		float highestWorldYPoint = transform.parent.position.y + tooltipHeight * 0.5f;
		float highestScreenYPoint = Camera.main.WorldToScreenPoint(new Vector3(transform.parent.position.x, highestWorldYPoint, 0)).y;
		if (highestScreenYPoint >= Screen.height)
			transform.position -= new Vector3(0,(verticalOffsetFromScreenEdge - Screen.height) + highestScreenYPoint);
	}


	void SetupTextComponent(string text)
	{
		if (text != "")
			tooltipText.text = text;
		else
			tooltipText.enabled=false;
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
		cardGraphic.AssignCardForDisplayOnly(addedCard);
		cardGraphic.GetComponent<Image>().raycastTarget = false;
		cardGraphic.transform.SetParent(cardsDisplayGroup, false);
	}
	void AddVisualCard(Card addedCard)
	{
		VisualCardGraphic cardGraphic = Instantiate(visualCardGraphicPrefab);
		cardGraphic.AssignCard(addedCard);
		cardGraphic.GetComponent<Image>().raycastTarget = false;
		cardGraphic.transform.SetParent(cardsDisplayGroup, false);
	}

}
