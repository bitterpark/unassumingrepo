using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomCardGraphic :CardGraphic 
{
	
	public Text enemyCountText;
	RoomCard assignedCard;
	RoomCardDisplayer displayer;

	public void AssignCard(RoomCard newCard, RoomCardDisplayer displayer)
	{
		base.UpdateBasicVisuals(newCard);
		this.displayer = displayer;

		GetComponent<Button>().onClick.AddListener(() => CardClicked());
		assignedCard = newCard;
		enemyCountText.text = assignedCard.GetEnemyCount().ToString();
	}

	public RoomCard GetAssignedCard()
	{
		return assignedCard;
	}

	public int GetRoomEnemyCount()
	{
		return assignedCard.GetEnemyCount();
	}

	public void ShowRoomTooltip()
	{
		RoomStipulationCard spawnedRoomStipulationCard;//
		if (assignedCard.TryGetRespectiveStipulationCard(out spawnedRoomStipulationCard))
			TooltipManager.main.CreateTooltip("", transform, spawnedRoomStipulationCard);
	}

	public void StopShowingRoomTooltip()
	{
		TooltipManager.main.StopAllTooltips();
	}

	public void CardClicked()
	{
		if (GetComponent<Button>().IsInteractable())
			displayer.PlayRoomCard(this);
	}

}
