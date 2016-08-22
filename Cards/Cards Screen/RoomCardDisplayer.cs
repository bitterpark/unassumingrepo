using UnityEngine;
using System.Collections;

public class RoomCardDisplayer : MonoBehaviour {

	public RoomCardGraphic encounterCardPrefab;
	public Transform roomCardDisplayArea;

	CardsScreen cardsScreen;
	Encounter currentMission;

	public void EnableDisplayer(CardsScreen cardsScreen, Encounter currentMission)
	{
		this.cardsScreen = cardsScreen;
		this.currentMission = currentMission;
	}

	public void PlayRoomCard(RoomCardGraphic playedCardGraphic)
	{
		RoomCard playedCard = playedCardGraphic.GetAssignedCard();
		ClearRoomSelection();
		cardsScreen.RoomCardSelected(playedCard);
		
	}

	public void ShowNextRoomSelection()
	{
		foreach (RoomCard room in currentMission.GetRoomSelection(2))
		{
			RoomCardGraphic cardGraphic = Instantiate(encounterCardPrefab);
			cardGraphic.AssignCard(room,this);
			cardGraphic.transform.SetParent(roomCardDisplayArea, false);
		}
	}

	void ClearRoomSelection()
	{
		foreach (RoomCardGraphic graphic in roomCardDisplayArea.GetComponentsInChildren<RoomCardGraphic>())
		{
			GameObject.Destroy(graphic.gameObject);
		}
	}
}
