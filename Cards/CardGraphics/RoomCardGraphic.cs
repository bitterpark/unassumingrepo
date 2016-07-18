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

	void CardClicked()
	{
		CardsScreen.main.PlayRoomCard(this);
	}

}
