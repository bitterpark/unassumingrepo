using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RewardCardGraphic : CardGraphic 
{
	public Text effectDescription;

	RewardCard assignedCard;
	RewardCardDisplayer rewardDisplayer;

	public Transform rewardItemsGroup;
	public RewardCardItem itemPrefab;

	public void PlayAssignedCard()
	{
		assignedCard.PlayCard();
	}

	public void AssignCardForDisplayOnly(RewardCard newCard)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;
		effectDescription.text = newCard.effectDescription;
		if (assignedCard.rewardItems.Count > 0)
		{
			DisplayRewardItems();
		}
	}

	public void AssignCard(RewardCard newCard, RewardCardDisplayer rewardDisplayer)
	{
		base.UpdateBasicVisuals(newCard);
		assignedCard = newCard;
		effectDescription.text = newCard.effectDescription;
		if (assignedCard.rewardItems.Count > 0)
		{
			DisplayRewardItems();
		}

		this.rewardDisplayer = rewardDisplayer;
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
		rewardDisplayer.RewardCardPlay(this);
	}
	
}
