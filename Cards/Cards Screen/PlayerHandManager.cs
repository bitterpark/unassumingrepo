using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHandManager : MonoBehaviour 
{
	MissionCharacterManager characterManager;

	public void EnableManager(MissionCharacterManager characterManager)
	{
		this.characterManager = characterManager;
	}

	public void DrawCardsForActiveMercs()
	{
		ClearDisplayedHand();
		List<CombatCard> playerHand = new List<CombatCard>();

		foreach (CharacterGraphic character in characterManager.GetMercGraphics())
		{
			MercGraphic merc = character as MercGraphic;
			merc.TryDrawNewCardToHand();
		}

		//handDisplayer.DisplayHand(true, playerHand);
	}

	public void SetPlayerHandInteractivity(bool interactive)
	{
		foreach (CharacterGraphic character in characterManager.GetMercGraphics())
		{
			MercGraphic merc = character as MercGraphic;
			merc.SetMyHandInteractivity(interactive);
		}
	}

	public void ClearDisplayedHand()
	{
		foreach (CharacterGraphic character in characterManager.GetMercGraphics())
		{
			MercGraphic merc = character as MercGraphic;
			merc.HideMyHand();
		}
	}
	
}
