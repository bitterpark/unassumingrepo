using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomCardGraphic :CardGraphic 
{
	public RoomCard assignedCard;
	public Text enemyCountText;
	
	public void AssignCard(RoomCard newCard)
	{
		base.UpdateBasicVisuals(newCard);
		GetComponent<Button>().onClick.AddListener(() => CardClicked());
		assignedCard = newCard;
		enemyCountText.text = assignedCard.GetEnemyCount().ToString();
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
			CardsScreen.main.PlayRoomCard(this);
	}

}
