using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICombatManager
{
	CombatManager.TurnStatus GetTurnStatus();
	PlayerHandManager GetPlayerHandManager();
	bool TryPlayCombatCard(ICombatCard card,CharacterGraphic cardPlayer);
	void PlaceRoomStipulationCard(RoomStipulationCard card);
	void RemoveRoomStipulationCard(RoomStipulationCard card);
}

public interface ICombatRulesHandler
{
	void SetRangedAttacksRestriction(bool allowed);
}

public class CombatManager : MonoBehaviour,ICombatManager,ICombatRulesHandler {

	public delegate void RoundOverDeleg();
	public static event RoundOverDeleg ERoundIsOver;

	public static ICombatManager main;
	public static ICombatRulesHandler rulesHandler;

	public VisualCardGraphic visualCardPrefab;
	public CombatCardGraphic combatCardPrefab;

	public Transform centerPlayArea;
	public Transform roomStipulationCardsGroup;

	public const int enemyHandSize = 8;
	public const int mercStartingHandSize = 4;
	public const int mercExtraCardDrawPerTurn = 1;

	public const float cardPlayAnimationTime = 2.25f;

	public PlayerHandManager playerHandManager;

	const int maxHandSize = 8;
	const int cardDrawPerTurn = 1;

	CharacterGraphic selectedCharacter;

	List<RoomStipulationCard> activeRoomCards = new List<RoomStipulationCard>();

	bool rangedAttacksAllowed = true;

	public enum TurnStatus { Player, Enemy };
	TurnStatus turnStatus;

	
	CardsScreen cardsScreen;
	MissionCharacterManager characterManager;
	
	MissionUIToggler uiToggler;
	PrepHandManager prepManager;
	ModeTextDisplayer modeTextDisplayer;
	CombatCardTargeter combatCardTargeter;
	AICombatCardSelector aiCardSelector;

	public void EnableCombatManager(CardsScreen cardsScreen,
	MissionCharacterManager characterManager,
	MissionUIToggler uiToggler,
	PrepHandManager prepDisplayer,
	ModeTextDisplayer modeTextDisplayer,
	CombatCardTargeter combatCardTargeter,
	AICombatCardSelector aiCardSelector)
	{
		this.cardsScreen=cardsScreen;
		this.characterManager = characterManager;
		this.uiToggler = uiToggler;
		this.prepManager = prepDisplayer;
		this.modeTextDisplayer = modeTextDisplayer;
		this.combatCardTargeter = combatCardTargeter;
		this.aiCardSelector = aiCardSelector;
		playerHandManager.EnableManager(characterManager);
	}
	
	public void SetupCombat(RoomCard playedRoom)
	{
		EncounterEnemy[] enemies = playedRoom.GetEnemies();
		uiToggler.SetAllowHandPeeking(true);
		characterManager.PrepAllCharactersForCombatStart(enemies);

		List<RoomStipulationCard> randomRoomStipulationCards = playedRoom.GetRandomRoomStipulationCards(1);
		foreach (RoomStipulationCard card in randomRoomStipulationCards)
			PlaceRoomStipulationCard(card);

		playerHandManager.DrawCombatStartHand();
		playerHandManager.SetPlayerHandInteractivity(false);
		prepManager.BeginPrepCardsPhase();
	}

	public void FinishPrepCardsPhase()
	{
		characterManager.GiveAllCharactersTurns();
		playerHandManager.SetPlayerHandInteractivity(true);
		StartPlayerTurn();
	}


	public void CharacterKilled(CharacterGraphic killedCharGraphic)
	{
		if (selectedCharacter == killedCharGraphic)
			NullifySelectedCharacter();

		characterManager.DoCharacterDeathCleanup(killedCharGraphic);
	}

	void NullifySelectedCharacter()
	{
		selectedCharacter = null;
		characterManager.ResetSelectionArrow();
	}

	//COMBAT STUFF START
	void StartPlayerTurn()
	{
		turnStatus = TurnStatus.Player;
		uiToggler.EnableTurnoverButton();
		playerHandManager.NewPlayerTurnStart();
	}

	void AdvancePlayerTurn()
	{
		uiToggler.EnableTurnoverButton();
		//playerHandManager.DrawCardsForActiveMercs(1);
	}

	public bool TryPlayCombatCard(ICombatCard cardObject,CharacterGraphic cardPlayer)
	{
		CombatCard playedCard = cardObject.GetAssignedCard();
		if (EligibleCombatCardType(playedCard) && cardPlayer.CharacterMeetsCardRequirements(playedCard))
		{
			uiToggler.DisableTurnoverButton();
			combatCardTargeter.CombatCardPlayStarted(cardObject, cardPlayer);
			return true;
		}
		else
			return false;
	}

	public void CombatCardPlayStarted(ICombatCard playedCard)
	{
		StartCoroutine("CombatCardPlayed", playedCard);
	}



	void StartEnemyTurn()
	{
		turnStatus = TurnStatus.Enemy;
		SelectEnemyCharacter(characterManager.GetFirstEnemy());
		StartCoroutine(EnemyTurnProcess());
	}

	IEnumerator EnemyTurnProcess()
	{
		if (!PlayerWonOrLost())
		{
			CombatCard playedCard;
			if (aiCardSelector.TrySelectCardToPlay(out playedCard, selectedCharacter))
			{
				combatCardTargeter.AssignCardTargets(playedCard, selectedCharacter);
				CombatCardGraphic cardGraphic=InstantiateCombatCardGraphic(playedCard);
				CombatCardPlayStarted(cardGraphic);
			}
			else
				TransferTurn();
		}
		yield break;
	}


	public bool EligibleCombatCardType(CombatCard card)
	{
		if (card.cardType == CombatCard.CardType.Ranged_Attack && rangedAttacksAllowed)
			return true;
		if (card.cardType != CombatCard.CardType.Ranged_Attack)
			return true;

		return false;
	}

	IEnumerator CombatCardPlayed(ICombatCard playedCardObject)
	{
		uiToggler.DisablePlayerActionsDuringCardPlay();
		CombatCard playedCard = playedCardObject.GetAssignedCard();
		CharacterGraphic userGraphic = playedCard.GetUserChar();
		
		//Visualize card play
		if (playedCard.targetType == CombatCard.TargetType.None)
			PutCardToCharacter(playedCardObject, userGraphic);
		if (playedCard.targetType == CombatCard.TargetType.AllEnemies || playedCard.targetType == CombatCard.TargetType.AllFriendlies)
			PutCardToCenter(playedCardObject);
		if (playedCard.targetType != CombatCard.TargetType.None && playedCard.targetType != CombatCard.TargetType.AllEnemies
			&& playedCard.targetType != CombatCard.TargetType.AllFriendlies)
		{
			CharacterGraphic targetGraphic = null;
			targetGraphic = playedCard.targetChars[0];
			PutCardToCharacter(playedCardObject, targetGraphic);
		}
		//Actual effects
		playedCard.PlayCard();
		yield return new WaitForSeconds(cardPlayAnimationTime);
		GameObject.Destroy(playedCardObject.GetTransform().gameObject);

		if (userGraphic.GetHealth() > 0)
			userGraphic.TurnFinished();

		characterManager.CleanupCharactersWhoDied();

		if (turnStatus == TurnStatus.Player)
			uiToggler.ReenablePlayerActions();

		TransferTurn();
		yield break;
	}

	void TransferTurn()
	{
		StartCoroutine(TurnTransferRoutine());
	}

	IEnumerator TurnTransferRoutine()
	{
		if (!PlayerWonOrLost())
		{
			//Make sure the current character's hand is properly disposed
			yield return new WaitForFixedUpdate();
			
			if (turnStatus == TurnStatus.Player)
			{
				foreach (CharacterGraphic mercGraphic in characterManager.GetMercGraphics())
					if (mercGraphic.HasTurn())
					{
						AdvancePlayerTurn();
						yield break;
					}
				EndPlayerTurn();
			}
			else
			{
				//ClearHand(enemyHand);
				foreach (CharacterGraphic enemyGraphic in characterManager.GetEnemyGraphics())
					if (enemyGraphic.HasTurn())
					{
						SelectEnemyCharacter(enemyGraphic);
						StartCoroutine(EnemyTurnProcess());
						yield break;
					}
				StartCoroutine(StartNewRound());
			}
		}
		yield break;
	}

	IEnumerator StartNewRound()
	{
		//characterManager.DoRoundStaminaRegen();
		if (ERoundIsOver != null) 
			ERoundIsOver();
		characterManager.CleanupCharactersWhoDied();

		if (!PlayerWonOrLost())
		{
			characterManager.DoRoundStartForAllCharacters();
			modeTextDisplayer.DisplayCenterMessage("New round");
			yield return new WaitForSeconds(cardPlayAnimationTime);
			modeTextDisplayer.HideCenterMessage();
			StartPlayerTurn();
		}
		yield break;
	}

	bool PlayerWonOrLost()
	{
		bool winOrLossHappened = false;
		if (characterManager.GetMercCount() == 0)
		{
			winOrLossHappened = true;
			EndCombatAfterLoss();
		}
		else
		{
			if (characterManager.GetEnemyCount() == 0)
			{
				winOrLossHappened = true;
				EndCombatAfterWin();
			}
		}
		return winOrLossHappened;
	}

	public void SelectEnemyCharacter(CharacterGraphic newSelectedCharacter)
	{
		selectedCharacter = newSelectedCharacter;
		EnemyCharacterSelectedInCombat();
		EnemyGraphic enemy = selectedCharacter as EnemyGraphic;
		enemy.StartedTurn();
	}


	void PlayersCharacterSelectedInCombat()
	{
		characterManager.HighlightSelectedPlayersCharacter(selectedCharacter);
		uiToggler.ReenablePlayerActions();
		uiToggler.EnableTurnoverButton();
	}

	void EnemyCharacterSelectedInCombat()
	{
		characterManager.HighlightSelectedEnemyCharacter(selectedCharacter);
		uiToggler.DisableTurnoverButton();
		uiToggler.SetAllowHandPeeking(false);
	}

	public CombatCardGraphic InstantiateCombatCardGraphic(CombatCard card)
	{
		CombatCardGraphic newGraphic = Instantiate(combatCardPrefab);
		newGraphic.AssignCard(card);
		return newGraphic;
	}
	
	public void PutCardToCenter(ICombatCard playedCardObject)
	{
		playedCardObject.GetTransform().SetParent(centerPlayArea, false);
	}

	void PutCardToCharacter(ICombatCard playedCardObject, CharacterGraphic character)
	{
		playedCardObject.GetTransform().SetParent(character.appliedCardsGroup, false);
		playedCardObject.GetTransform().SetAsLastSibling();
	}

	public void EndTurnButtonPressed()
	{
		EndPlayerTurn();
	}

	void EndPlayerTurn()
	{
		characterManager.RemoveAllMercTurns();
		uiToggler.DisableTurnoverButton();
		playerHandManager.ClearDisplayedHand();
		StartEnemyTurn();
		
	}

	void EndCombatAfterWin()
	{
		foreach (CharacterGraphic mercGraphic in characterManager.GetMercGraphics())
		{
			mercGraphic.SetStartArmor();
			mercGraphic.RemoveAllCharacterCards();
			//mercGraphic.DiscardCurrentHand();
		}
		uiToggler.SetAllowHandPeeking(false);
		uiToggler.DisableTurnoverButton();
		RemoveAllRoomStipulationCards();
		playerHandManager.ClearDisplayedHand();

		cardsScreen.CombatWon();
	}

	void EndCombatAfterLoss()
	{
		RemoveAllRoomStipulationCards();
		cardsScreen.CloseScreen();
	}

	public void PlaceRoomStipulationCard(RoomStipulationCard playedCard)
	{
		VisualCardGraphic cardGraphic = Instantiate(visualCardPrefab);
		cardGraphic.AssignCard(playedCard);
		cardGraphic.transform.SetParent(roomStipulationCardsGroup.transform, false);
		playedCard.ActivateCard();
		activeRoomCards.Add(playedCard);
	}

	public void RemoveRoomStipulationCard(RoomStipulationCard removedCard)
	{
		removedCard.DeactivateCard();
		foreach (VisualCardGraphic graphic in roomStipulationCardsGroup.GetComponentsInChildren<VisualCardGraphic>())
		{
			if (graphic.assignedCard == removedCard)
			{
				GameObject.Destroy(graphic.gameObject);
				break;
			}
		}
		activeRoomCards.Remove(removedCard);
	}

	void RemoveAllRoomStipulationCards()
	{
		foreach (RoomStipulationCard card in new List<RoomStipulationCard>(activeRoomCards))
		{
			RemoveRoomStipulationCard(card);
		}
	}

	public void SetRangedAttacksRestriction(bool allowed)
	{
		rangedAttacksAllowed = allowed;
	}

	public TurnStatus GetTurnStatus()
	{
		return turnStatus;
	}

	public PlayerHandManager GetPlayerHandManager()
	{
		return playerHandManager;
	}

	public CharacterGraphic GetSelectedCharacter()
	{
		return selectedCharacter;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			foreach (CharacterGraphic enemy in new List<CharacterGraphic>(characterManager.GetEnemyGraphics()))
			{
				CharacterKilled(enemy);
			}
			EndCombatAfterWin();
		}
	}

	void Start()
	{
		main = this;
		rulesHandler=this;
	}
}
