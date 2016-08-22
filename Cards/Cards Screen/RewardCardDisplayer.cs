using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RewardCardDisplayer : MonoBehaviour 
{
	public RewardCardGraphic rewardCardPrefab;
	public Transform rewardCardsPlayArea;

	CardsScreen mainCardsScreen;
	Encounter currentMission;

	public void EnableDisplayer(CardsScreen mainCardsScreen, Encounter currentMission)
	{
		this.mainCardsScreen = mainCardsScreen;
		this.currentMission = currentMission;
	}

	public void ShowRewards()
	{
		RewardCard[] rewardsSelection;
		if (!currentMission.IsFinished())
		{
			rewardsSelection = currentMission.GetRewardCardSelection(2);
			currentMission.DiscardRewardCards(rewardsSelection);
		}
		else
		{
			rewardsSelection = currentMission.GetMissionEndRewards();
		}
		foreach (RewardCard reward in rewardsSelection)
		{
			RewardCardGraphic newGraphic = Instantiate(rewardCardPrefab);
			newGraphic.AssignCard(reward,this);
			newGraphic.transform.SetParent(rewardCardsPlayArea, false);
		}
	}

	public void RewardCardPlay(RewardCardGraphic cardGraphic)
	{
		StartCoroutine("RewardCardPlaying", cardGraphic);
	}

	IEnumerator RewardCardPlaying(RewardCardGraphic cardGraphic)
	{
		foreach (RewardCardGraphic graphic in rewardCardsPlayArea.GetComponentsInChildren<RewardCardGraphic>())
			graphic.GetComponent<Button>().interactable = false;

		//PutCardToCenter(cardGraphic);
		cardGraphic.PlayAssignedCard();
		//yield return new WaitForSeconds(cardPlayAnimationTime);
		//GameObject.Destroy(cardGraphic.gameObject);
		HideRewards();
		mainCardsScreen.RewardSelectionFinished();
		yield break;
	}

	void HideRewards()
	{
		foreach (RewardCardGraphic graphic in rewardCardsPlayArea.GetComponentsInChildren<RewardCardGraphic>())
		{
			GameObject.Destroy(graphic.gameObject);
		}
	}
}
