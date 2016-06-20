using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RewardCardGraphic : CardGraphic 
{
	public Text effectDescription;

	public RewardCard assignedCard;

	public Transform rewardItemsGroup;
	public RewardCardItem itemPrefab;

	public void AssignCard(RewardCard newCard)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;
		effectDescription.text = newCard.effectDescription;
		if (assignedCard.rewardItems.Count > 0)
		{
			DisplayRewardItems();
		}
		GetComponent<Button>().onClick.AddListener(() => CardClicked());
	}

	void DisplayRewardItems()
	{
		rewardItemsGroup.gameObject.SetActive(true);
		foreach (InventoryItem item in assignedCard.rewardItems)
		{
			RewardCardItem newItem = Instantiate(itemPrefab);
			newItem.AssignItem(item);
			newItem.transform.SetParent(rewardItemsGroup);
		}
	}

	void CardClicked()
	{
		CardsScreen.main.RewardCardPlay(this);
	}
	
}
