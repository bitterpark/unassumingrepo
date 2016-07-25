using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardsScreen : MonoBehaviour 
{
	
	public const float cardPlayAnimationTime = 2.25f;

	public static CardsScreen main;

	public Transform enemiesGroup;
	public Transform mercsGroup;
	public Transform roomCardsGroup;
	public Transform encounterDeckGroup;
	public Transform centerPlayArea;
	public Transform prepCardsDisplayArea;

	public Transform selectionArrow;
	public Transform mainPanel;
	public Image selectModeMessage;
	public Button beginCombatButton;
	public Button characterPreppedButton;
	public Button skipTurnButton;
	public Button discardHandButton;
	public Button finishButton;
	
	//Currently unused
	public HandDisplayer enemyHandObject;
	public HandDisplayer playerHandObject;

	public CombatCardGraphic combatCardPrefab;
	public VisualCardGraphic visualCardPrefab;
	public RewardCardGraphic rewardCardPrefab;
	
	public MercGraphic mercGraphicPrefab;
	public EnemyGraphic enemyGraphicPrefab;
	public RoomCardGraphic encounterCardPrefab;

	public delegate void RoundOverDeleg();
	public static event RoundOverDeleg ERoundIsOver;
	public delegate void SetMeleeTargetMercDeleg();
	public static event SetMeleeTargetMercDeleg ENewMeleeTargetMercSet;
	public delegate void SetMeleeTargetEnemyDeleg();
	public static event SetMeleeTargetEnemyDeleg ENewMeleeTargetEnemySet;
	public delegate void SetRangedTargetMercDeleg();
	public static event SetRangedTargetMercDeleg ENewRangedTargetMercSet;
	public delegate void SetRangedTargetEnemyDeleg();
	public static event SetRangedTargetEnemyDeleg ENewRangedTargetEnemySet;

	enum TurnStatus { Player, Enemy };
	TurnStatus turnStatus;

	CharacterGraphic selectedCharacter;

	List<CharacterGraphic> mercGraphics = new List<CharacterGraphic>();
	List<CharacterGraphic> enemyGraphics = new List<CharacterGraphic>();
	List<RoomStipulationCard> activeRoomCards = new List<RoomStipulationCard>();

	CharacterGraphic meleeCardTargetOverrideMerc = null;
	CharacterGraphic meleeCardTargetOverrideEnemy = null;
	CharacterGraphic rangedCardTargetOverrideMerc = null;
	CharacterGraphic rangedCardTargetOverrideEnemy = null;

	public const int startingHandSize = 4;
	const int maxHandSize = 8;
	const int cardDrawPerTurn = 1;
	const int staminaRegen = 1;
	const int enemyTeamSize = 4;

	bool rangedAttacksAllowed = true;

	bool handPeekingAllowed = false;

	Encounter playedEncounter;

	public static bool missionOngoing = false;

	public void OpenScreen(Encounter newEncounter, PartyMember[] team)
	{
		missionOngoing = true;
		GetComponent<Canvas>().enabled = true;
		playedEncounter=newEncounter;
		foreach (PartyMember merc in team)
		{
			AddMercenary(merc);
		}
		ProgressThroughEncounter();
	}

	void CloseScreen()
	{
		missionOngoing = false;
		HideCenterMessage();
		finishButton.gameObject.SetActive(false);
		
		selectionArrow.SetParent(this.transform, false);
		RemoveAllRoomStipulationCards();
	
		foreach (CharacterGraphic graphic in new List<CharacterGraphic>(mercGraphics)) 
			RemoveMercenary(graphic);
		foreach (CharacterGraphic graphic in new List<CharacterGraphic>(enemyGraphics)) 
		{ 
			RemoveEnemy(graphic);
		}
		GetComponent<Canvas>().enabled = false;
		PartyManager.mainPartyManager.AdvanceMapTurn();
	}

	void ProgressThroughEncounter()
	{
		if (!playedEncounter.IsFinished()) 
			ShowNextRoomSelection();
		else
		{
			DisableTurnoverButtons();
			DisplayCenterMessage("Run complete!");
			finishButton.gameObject.SetActive(true);
		}
	}

	public void PlayRoomCard(RoomCardGraphic playedCardGraphic)
	{
		RoomCard playedCard = playedCardGraphic.assignedCard;
		GameObject.Destroy(playedCardGraphic.gameObject);
		ClearRoomSelection();
		StartCombat(playedCard, playedCard.GetEnemies());
	}

	void ShowNextRoomSelection()
	{
		foreach (RoomCard room in playedEncounter.GetRoomSelection(2))
		{
			RoomCardGraphic cardGraphic = Instantiate(encounterCardPrefab);
			cardGraphic.AssignCard(room);
			cardGraphic.transform.SetParent(centerPlayArea, false);
		}
	}

	void ClearRoomSelection()
	{
		foreach (RoomCardGraphic graphic in centerPlayArea.GetComponentsInChildren<RoomCardGraphic>())
		{
			GameObject.Destroy(graphic.gameObject);
		}
	}


	void StartCombat(RoomCard playedRoom ,params EncounterEnemy[] enemies)
	{
		handPeekingAllowed = true;
		foreach (EncounterEnemy enemy in enemies) 
			AddEnemy(enemy);

		//Set starting stats
		foreach (CharacterGraphic enemy in enemyGraphics)
			enemy.SetStartStamina();
		foreach (CharacterGraphic merc in mercGraphics)
			merc.SetStartStamina();

		List<RoomStipulationCard> randomRoomStipulationCards = playedRoom.GetRandomRoomStipulationCards(1);
		foreach (RoomStipulationCard card in randomRoomStipulationCards)
			PlaceRoomStipulationCard(card);

		DoCombatPrepPhase();
	}

	enum PrepMode { SelectClassCard, SelectWeaponCard };
	PrepMode currentPrepMode;

	void DoCombatPrepPhase()
	{
		foreach (CharacterGraphic merc in mercGraphics)
		{
			merc.GiveTurn();
			merc.GenerateCombatStartDeck();
		}
		foreach (CharacterGraphic enemy in enemyGraphics)
		{
			enemy.GiveTurn();
			enemy.GenerateCombatStartDeck();
		}

		beginCombatButton.gameObject.SetActive(true);
		beginCombatButton.onClick.AddListener(FinishCombatPrepPhase);
		StartCoroutine("SelectNextCharacterToPrep");
	}

	IEnumerator SelectNextCharacterToPrep()
	{
		CharacterGraphic targetChar = null;
		bool selectFriendly = true;
		string centerMessageText = "Select character";

		DisablePlayerActionsDuringCardPlay();
		DisplayCenterMessage(centerMessageText);

		SetMercPortraitsEnabled(false);
		while (true)
		{
			if (Input.GetMouseButton(0))
			{
				if (RaycastForCharacter(selectFriendly, out targetChar))
				{
					if (targetChar.HasTurn())
						break;
				}
			}
			yield return new WaitForEndOfFrame();
		}

		SetMercPortraitsEnabled(true);
		HideCenterMessage();
		MercGraphic targetMerc = targetChar as MercGraphic;
		selectedCharacter = targetMerc;
		HighlightSelectedPlayersCharacter();
		CharacterSelectedForPrep(targetMerc);
	}

	

	void CharacterSelectedForPrep(MercGraphic character)
	{
		characterPreppedButton.gameObject.SetActive(true);
		characterPreppedButton.onClick.AddListener(CharacterPrepFinished);
		beginCombatButton.gameObject.SetActive(false);
		List<CombatCard> startingCards = character.GetCurrentDeckCards();

		foreach (CombatCard card in startingCards)
		{
			CombatCardGraphic newCardGraphic = Instantiate(combatCardPrefab);
			newCardGraphic.AssignCard(card);
			newCardGraphic.SetInteractable(false);
			newCardGraphic.transform.SetParent(prepCardsDisplayArea, false);
		}

		currentPrepMode = PrepMode.SelectClassCard;
		ShowSelectionOfPrepCards(character);
	}

	void ShowSelectionOfPrepCards(MercGraphic character)
	{
		HideAvailablePrepCards();
		if (currentPrepMode==PrepMode.SelectClassCard)
			playerHandObject.DisplayPrepHand(character.GetMercsClassPrepCards());
		if (currentPrepMode == PrepMode.SelectWeaponCard)
			playerHandObject.DisplayPrepHand(character.GetMercsWeaponPrepCards());
	}

	public void PlayPrepCard(PrepCardGraphic cardGraphic)
	{
		StartCoroutine("PrepCardPlaying", cardGraphic);
	}

	IEnumerator PrepCardPlaying(PrepCardGraphic cardGraphic)
	{
		playerHandObject.SetPrepHandInteractivity(false);
		cardGraphic.transform.SetParent(prepCardsDisplayArea, false);
		cardGraphic.PlayAssignedCard(selectedCharacter);
		if (currentPrepMode == PrepMode.SelectClassCard)
		{
			TransferPrepFromCharacterCardToWeaponCard();
		}
		yield break;
	}

	void TransferPrepFromCharacterCardToWeaponCard()
	{
		currentPrepMode = PrepMode.SelectWeaponCard;
		MercGraphic selectedMerc = selectedCharacter as MercGraphic;
		ShowSelectionOfPrepCards(selectedMerc);
	}

	void CharacterPrepFinished()
	{
		characterPreppedButton.onClick.RemoveAllListeners();
		characterPreppedButton.gameObject.SetActive(false);
		beginCombatButton.gameObject.SetActive(true);
		HideAvailablePrepCards();
		HidePrepDisplayDeck();
		selectedCharacter.RemoveTurn();
		StartCoroutine("SelectNextCharacterToPrep");
	}

	void HideAvailablePrepCards()
	{
		playerHandObject.HidePrepHand();
	}

	void HidePrepDisplayDeck()
	{
		foreach (CardGraphic graphic in prepCardsDisplayArea.GetComponentsInChildren<CardGraphic>())
			GameObject.Destroy(graphic.gameObject);
	}

	void FinishCombatPrepPhase()
	{
		StopCoroutine("SelectNextCharacterToPrep");
		beginCombatButton.onClick.RemoveAllListeners();
		beginCombatButton.gameObject.SetActive(false);
		GiveAllChractersTurns();
		StartPlayerTurn();
	}

	void StartPlayerTurn()
	{
		turnStatus = TurnStatus.Player;
		StartCoroutine(SelectNextCharacterToPlay());
	}

	IEnumerator SelectNextCharacterToPlay()
	{
		CharacterGraphic targetChar = null;
		bool selectFriendly = true;
		string centerMessageText = "Select character";

		DisablePlayerActionsDuringCardPlay();
		DisplayCenterMessage(centerMessageText);

		SetMercPortraitsEnabled(false);

		while (true)
		{
			if (Input.GetMouseButton(0))
			{
				if (RaycastForCharacter(selectFriendly, out targetChar))
				{
					if (targetChar.HasTurn())
						break;
				}
			}
			yield return new WaitForEndOfFrame();
		}

		SetMercPortraitsEnabled(true);
		HideCenterMessage();
		SelectCharacter(targetChar);
	}

	public void ClickCombatCard(CombatCardGraphic cardGraphic)
	{
		CombatCard playedCard = cardGraphic.assignedCard;
		if (CardRequirementsMet(playedCard))
		{
			PlayerCardTargetAssignment(cardGraphic);
		}
	}

	void PlayerCardTargetAssignment(CombatCardGraphic cardGraphic)
	{
		CombatCard playedCard = cardGraphic.assignedCard;
		DisableTurnoverButtons();
		if (playedCard.targetType == CombatCard.TargetType.SelectFriendly | playedCard.targetType == CombatCard.TargetType.SelectFriendlyOther)
		{
			StartCoroutine("SelectCardTargetCharacterByPlayer", cardGraphic);
			return;
		}

		if (playedCard.targetType == CombatCard.TargetType.SelectEnemy)
		{
			if (playedCard.GetType().BaseType == typeof(MeleeCard) && meleeCardTargetOverrideEnemy != null)
			{
				AssignCardTargets(cardGraphic.assignedCard);
				StartCoroutine("CombatCardPlayed", cardGraphic);
				return;
			}
			if (playedCard.GetType().BaseType == typeof(RangedCard) && rangedCardTargetOverrideEnemy != null)
			{
				AssignCardTargets(cardGraphic.assignedCard);
				StartCoroutine("CombatCardPlayed", cardGraphic);
				return;
			}

			StartCoroutine("SelectCardTargetCharacterByPlayer", cardGraphic);
			return;
		}
		if (playedCard.targetType != CombatCard.TargetType.SelectFriendly 
			&& playedCard.targetType!=CombatCard.TargetType.SelectFriendlyOther
			&& playedCard.targetType!=CombatCard.TargetType.SelectEnemy)
		{
			AssignCardTargets(cardGraphic.assignedCard);
			StartCoroutine("CombatCardPlayed", cardGraphic);
		}
	}

	IEnumerator SelectCardTargetCharacterByPlayer(CombatCardGraphic playedCardGraphic)
	{
		CharacterGraphic targetChar = null;
		bool selectFriendly=false;
		bool selectOtherFriendly = false;
		string centerMessageText = "Select enemy";
		if (playedCardGraphic.assignedCard.targetType == CombatCard.TargetType.SelectEnemy)
		{
			selectFriendly=false;
			centerMessageText = "Select enemy";
		}
		if (playedCardGraphic.assignedCard.targetType == CombatCard.TargetType.SelectFriendly)
		{
			selectFriendly=true;
			SetMercPortraitsEnabled(false);
			centerMessageText = "Select friendly";
		}
		if (playedCardGraphic.assignedCard.targetType == CombatCard.TargetType.SelectFriendlyOther)
		{
			selectFriendly = true;
			selectOtherFriendly = true;
			SetMercPortraitsEnabled(false);
			centerMessageText = "Select friendly";
		}

		DisablePlayerActionsDuringCardPlay();
		DisplayCenterMessage(centerMessageText);

		bool targetFound = false;

		while (true)
		{
			if (Input.GetMouseButton(1))
			{
				EnableTurnoverButtons();
				ReenablePlayerActions();
				break;
			}
			if (Input.GetMouseButton(0))
			{
				if (selectOtherFriendly)
				{
					if (RaycastForOtherFriendlyCharacter(selectedCharacter,out targetChar))
					{
						targetFound = true;
						break;
					}
				}
				else
				if (RaycastForCharacter(selectFriendly, out targetChar))
				{
					targetFound = true;
					break;
				}
			}
			yield return new WaitForEndOfFrame();
		}

		if (selectFriendly)
			SetMercPortraitsEnabled(true);
		HideCenterMessage();
		if (targetFound)
		{
			playedCardGraphic.assignedCard.targetChars.Add(targetChar);
			yield return StartCoroutine("CombatCardPlayed", playedCardGraphic);
		}
	}

	bool RaycastForOtherFriendlyCharacter(CharacterGraphic sourceCharacter,out CharacterGraphic foundChar)
	{
		if (RaycastForCharacter(true, out foundChar))
		{
			if (foundChar != sourceCharacter)
				return true;
			else
				return false;
		}
		return false;
	}

	bool RaycastForCharacter(bool friendly, out CharacterGraphic foundChar)
	{
		foundChar = null;

		List<CharacterGraphic> targetGroup;
		if (friendly)
			targetGroup = mercGraphics;
		else
			targetGroup = enemyGraphics;

		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		pointerData.position = Input.mousePosition;
		List<RaycastResult> raycastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerData, raycastResults);

		bool found = false;
		foreach (RaycastResult result in raycastResults)
		{
			if (result.gameObject.GetComponentInParent<CharacterGraphic>() != null)//.GetComponent<CharacterGraphic>() != null)
			{
				foundChar = result.gameObject.GetComponentInParent<CharacterGraphic>();
				if (targetGroup.Contains(foundChar))
					found = true;
			}
		}
		return found;
	}

	void SetMercPortraitsEnabled(bool enabled)
	{
		foreach (CharacterGraphic graphic in mercGraphics)
		{
			MercGraphic mercGraphic = graphic as MercGraphic;
			mercGraphic.SetPortraitClickable(enabled);
		}
	}

	void StartEnemyTurn()
	{
		
		turnStatus = TurnStatus.Enemy;
		DisableTurnoverButtons();
		SelectCharacter(enemyGraphics[0]);
		StartCoroutine(EnemyTurnProcess());
	}

	IEnumerator EnemyTurnProcess()
	{
		List<CombatCardGraphic> playableCards = SortOutPlayableEnemyCards();
		/*
		while (playableCards.Count > 0)
		{*/
			//yield return new WaitForSeconds(cardPlayAnimationTime);
		if (!PlayerWonOrLost())
		{
			if (playableCards.Count > 0)
			{
				CombatCardGraphic playedCardGraphic=playableCards[Random.Range(0, playableCards.Count)];
				CombatCard playedCard = playedCardGraphic.assignedCard;
				AssignCardTargets(playedCard);

				yield return StartCoroutine("CombatCardPlayed", playedCardGraphic);
				//If enemy died after playing a card, finish turn
				//if (selectedCharacter == null) break;
				//playableCards = SortOutPlayableEnemyCards();
			}
			else
				TransferTurn();
		}
		yield break;
	}

	List<CombatCardGraphic> SortOutPlayableEnemyCards()
	{
		List<CombatCardGraphic> playableCards = new List<CombatCardGraphic>();
		List<CombatCardGraphic> cardsInHand = selectedCharacter.GetHandGraphics();

		if (cardsInHand.Count > 0)
		{
			foreach (CombatCardGraphic cardGraphic in cardsInHand)
			{
				if (CardRequirementsMet(cardGraphic.assignedCard))
					playableCards.Add(cardGraphic);
			}
		}
		return playableCards;
	}

	bool CardRequirementsMet(CombatCard card)
	{
		if (selectedCharacter.CharacterMeetsCardRequirements(card))
		{
			if (card.cardType == CombatCard.CardType.Ranged_Attack && rangedAttacksAllowed)
				return true;
			if (card.cardType != CombatCard.CardType.Ranged_Attack) 
				return true;

			return false;
		}
		return false;
	}

	void AssignCardTargets(CombatCard playedCard)
	{
		List<CharacterGraphic> opposingGroup;
		List<CharacterGraphic> friendlyGroup;
		if (turnStatus == TurnStatus.Player)
		{
			opposingGroup = enemyGraphics;
			friendlyGroup = mercGraphics;
		}
		else
		{
			opposingGroup = mercGraphics;
			friendlyGroup = enemyGraphics;
		}

		if (playedCard.targetType == CombatCard.TargetType.None)
		{
			return;
		}
		//The following two are usually only used by the AI (unless a melee override target is set)
		if (playedCard.targetType == CombatCard.TargetType.SelectEnemy)
		{
			playedCard.targetChars.Add(opposingGroup[Random.Range(0, opposingGroup.Count)]);
		}
		if (playedCard.targetType == CombatCard.TargetType.SelectFriendly)
		{
			playedCard.targetChars.Add(friendlyGroup[Random.Range(0, friendlyGroup.Count)]);
		}
		if (playedCard.targetType == CombatCard.TargetType.SelectFriendlyOther)
		{
			List<CharacterGraphic> friendlyOthers = new List<CharacterGraphic>(friendlyGroup);
			friendlyOthers.Remove(selectedCharacter);
			playedCard.targetChars.Add(friendlyOthers[Random.Range(0, friendlyOthers.Count)]);
		}
		if (playedCard.targetType == CombatCard.TargetType.Random)
		{
			playedCard.targetChars.Add(opposingGroup[Random.Range(0,opposingGroup.Count)]);
		}
		if (playedCard.targetType == CombatCard.TargetType.AllEnemies)
		{
			playedCard.targetChars.AddRange(opposingGroup);
		}
		if (playedCard.targetType == CombatCard.TargetType.AllFriendlies)
		{
			playedCard.targetChars.AddRange(friendlyGroup);
		}
		if (playedCard.targetType == CombatCard.TargetType.Weakest)
		{
			CharacterGraphic weakestCharGraphic = opposingGroup[0];
			foreach (CharacterGraphic charGraphic in opposingGroup)
			{
				if (charGraphic.GetEffectiveHitpoints() == weakestCharGraphic.GetEffectiveHitpoints())
				{
					if (Random.value < 0.5f)
						weakestCharGraphic = charGraphic;
				}
				else
				{
					if (charGraphic.GetEffectiveHitpoints() < weakestCharGraphic.GetEffectiveHitpoints())
						weakestCharGraphic = charGraphic;
				}
			}
			playedCard.targetChars.Add(weakestCharGraphic);
		}
		if (playedCard.targetType == CombatCard.TargetType.Strongest)
		{
			CharacterGraphic strongestCharGraphic = opposingGroup[0];
			foreach (CharacterGraphic charGraphic in opposingGroup)
			{
				if (charGraphic.GetEffectiveHitpoints() == strongestCharGraphic.GetEffectiveHitpoints())
				{
					if (Random.value < 0.5f)
						strongestCharGraphic = charGraphic;
				}
				else
				{
					if (charGraphic.GetEffectiveHitpoints() > strongestCharGraphic.GetEffectiveHitpoints())
						strongestCharGraphic = charGraphic;
				}
			}
			playedCard.targetChars.Add(strongestCharGraphic);
		}

		if (playedCard.GetType().BaseType == typeof(MeleeCard))
			TryOverrideMeleeCardTarget(playedCard);
		if (playedCard.GetType().BaseType == typeof(RangedCard))
			TryOverrideRangedCardTarget(playedCard);
	}

	void TryOverrideRangedCardTarget(CombatCard playedRangedCard)
	{
		if (playedRangedCard.targetType == CombatCard.TargetType.SelectEnemy
			|| playedRangedCard.targetType == CombatCard.TargetType.Random
			|| playedRangedCard.targetType == CombatCard.TargetType.Strongest
			|| playedRangedCard.targetType == CombatCard.TargetType.Weakest)
		{
			if (turnStatus == TurnStatus.Enemy)
			{
				if (rangedCardTargetOverrideMerc != null)
				{
					playedRangedCard.targetChars.Clear();
					playedRangedCard.targetChars.Add(rangedCardTargetOverrideMerc);
				}
			}
			if (turnStatus == TurnStatus.Player)
			{
				if (rangedCardTargetOverrideEnemy != null)
				{
					playedRangedCard.targetChars.Clear();
					playedRangedCard.targetChars.Add(rangedCardTargetOverrideEnemy);
				}
			}
		}
	}

	void TryOverrideMeleeCardTarget(CombatCard playedMeleeCard)
	{
		if (playedMeleeCard.targetType == CombatCard.TargetType.SelectEnemy
			|| playedMeleeCard.targetType == CombatCard.TargetType.Random
			|| playedMeleeCard.targetType == CombatCard.TargetType.Strongest
			|| playedMeleeCard.targetType == CombatCard.TargetType.Weakest)
		{
			if (turnStatus == TurnStatus.Enemy)
			{
				if (meleeCardTargetOverrideMerc != null)
				{
					playedMeleeCard.targetChars.Clear();
					playedMeleeCard.targetChars.Add(meleeCardTargetOverrideMerc);
				}
			}
			if (turnStatus == TurnStatus.Player)
			{
				if (meleeCardTargetOverrideEnemy != null)
				{
					playedMeleeCard.targetChars.Clear();
					playedMeleeCard.targetChars.Add(meleeCardTargetOverrideEnemy);
				}
			}
		}
	}

	IEnumerator CombatCardPlayed(CombatCardGraphic playedCardGraphic)
	{
		DisablePlayerActionsDuringCardPlay();
		CombatCard playedCard = playedCardGraphic.assignedCard;
		CharacterGraphic graphicToPlayCardTo = null;
		if (playedCard.targetChars.Count>0)
			graphicToPlayCardTo = playedCard.targetChars[0];

		//Actual effects
		playedCard.userCharGraphic = selectedCharacter;
		selectedCharacter.RemoveCardFromHand(playedCard);
		playedCard.PlayCard();
		playedCard.originDeck.DiscardCards(playedCard);
		//Visualize card play
		if (playedCard.targetType==CombatCard.TargetType.None)
 			PutCardToCharacter(playedCardGraphic, selectedCharacter);
		if (playedCard.targetType == CombatCard.TargetType.AllEnemies || playedCard.targetType == CombatCard.TargetType.AllFriendlies)
			PutCardToCenter(playedCardGraphic);
		if (playedCard.targetType!=CombatCard.TargetType.None && playedCard.targetType!=CombatCard.TargetType.AllEnemies
			&& playedCard.targetType != CombatCard.TargetType.AllFriendlies)
			PutCardToCharacter(playedCardGraphic, graphicToPlayCardTo);

		yield return new WaitForSeconds(cardPlayAnimationTime);
		GameObject.Destroy(playedCardGraphic.gameObject);

		CleanupCharactersWhoDied();

		if (turnStatus == TurnStatus.Player)
			ReenablePlayerActions();

		TransferTurn();
		yield break;
	}

	void CleanupCharactersWhoDied()
	{
		List<CharacterGraphic> charactersInScene = new List<CharacterGraphic>();
		charactersInScene.AddRange(mercGraphics);
		charactersInScene.AddRange(enemyGraphics);
		foreach (CharacterGraphic character in charactersInScene)
		{
			if (character.GetHealth() <= 0)
				CharacterKilled(character);
		}
	}

	void DisablePlayerActionsDuringCardPlay()
	{
		handPeekingAllowed = false;
		if (selectedCharacter!=null)
			selectedCharacter.SetMyHandInteractivity(false);
	}

	void ReenablePlayerActions()
	{
		handPeekingAllowed = true;
		if (selectedCharacter != null)
			selectedCharacter.SetMyHandInteractivity(true);
	}

	void EnableTurnoverButtons()
	{
		discardHandButton.gameObject.SetActive(true);
		skipTurnButton.gameObject.SetActive(true);
	}
	void DisableTurnoverButtons()
	{
		discardHandButton.gameObject.SetActive(false);
		skipTurnButton.gameObject.SetActive(false);
	}

	void TransferTurn()
	{
		StartCoroutine(TurnTransferRoutine());
	}

	IEnumerator TurnTransferRoutine()
	{
		if (!PlayerWonOrLost())
		{
			if (selectedCharacter != null)
			{
				//int cardsMissing = startingHandSize - characterHands[selectedCharacter.GetCharacter()].Count;
				//characterHands[selectedCharacter.GetCharacter()].AddRange(characterDecks[selectedCharacter.GetCharacter()].DrawCards(cardDrawPerTurn));
				selectedCharacter.TurnDone();
				//Wait for one frame to make sure previous character's cards are disposed
				yield return new WaitForFixedUpdate();
			}

			if (turnStatus == TurnStatus.Player)
			{
				//ClearHand(playerHand);
				foreach (CharacterGraphic mercGraphic in mercGraphics)
					if (mercGraphic.HasTurn())
					{
						StartCoroutine(SelectNextCharacterToPlay());
						yield break;
					}
				StartEnemyTurn();
			}
			else
			{
				//ClearHand(enemyHand);
				foreach (CharacterGraphic enemyGraphic in enemyGraphics)
					if (enemyGraphic.HasTurn())
					{
						SelectCharacter(enemyGraphic);
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
		RoundStaminaRegen();
		if (ERoundIsOver != null) ERoundIsOver();
		CleanupCharactersWhoDied();

		if (!PlayerWonOrLost())
		{
			GiveAllChractersTurns();
			DisplayCenterMessage("New round");
			yield return new WaitForSeconds(cardPlayAnimationTime);
			HideCenterMessage();
			StartPlayerTurn();
		}
		yield break;
	}

	void RoundStaminaRegen()
	{
		foreach (CharacterGraphic mercGraphic in mercGraphics)
		{
			mercGraphic.IncrementStamina(staminaRegen);
		}
		foreach (CharacterGraphic enemyGraphic in enemyGraphics)
		{
			enemyGraphic.IncrementStamina(staminaRegen);
		}
	}

	bool PlayerWonOrLost()
	{
		bool winOrLossHappened = false;
		if (mercGraphics.Count == 0)
		{
			winOrLossHappened = true;
			CloseScreen();
		}
		else
		{
			if (enemyGraphics.Count == 0)
			{
				winOrLossHappened = true;
				EndCombatAfterWin();
			}
		}
		return winOrLossHappened;
	}

	void GiveAllChractersTurns()
	{
		foreach (CharacterGraphic merc in mercGraphics)
		{
			merc.GiveTurn();
		}
		foreach (CharacterGraphic enemy in enemyGraphics)
		{
			enemy.GiveTurn();
		}
	}


	void SelectCharacter(CharacterGraphic newSelectedCharacter)
	{
		selectedCharacter = newSelectedCharacter;

		if (turnStatus == TurnStatus.Player)
		{
			PlayersCharacterSelectedInCombat();
		}
		else
		{
			EnemyCharacterSelectedInCombat();
		}
		selectedCharacter.StartedTurn();
	}

	void PlayersCharacterSelectedInCombat()
	{
		HighlightSelectedPlayersCharacter();
		ReenablePlayerActions();
		EnableTurnoverButtons();
	}

	void HighlightSelectedPlayersCharacter()
	{
		//Set anchor to bottom left corner
		selectionArrow.GetComponent<RectTransform>().anchorMin = Vector2.zero;
		selectionArrow.GetComponent<RectTransform>().anchorMax = Vector2.zero;
		selectionArrow.SetParent(selectedCharacter.transform, false);
		selectionArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, 20f);
	}

	void EnemyCharacterSelectedInCombat()
	{
		HighlightSelectedEnemyCharacter();
		DisableTurnoverButtons();
		handPeekingAllowed = false;
	}

	void HighlightSelectedEnemyCharacter()
	{
		//Set anchor to upper left corner
		selectionArrow.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1f);
		selectionArrow.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1f);
		selectionArrow.SetParent(selectedCharacter.transform, false);
		selectionArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, -20f);
	}

	public void DiscardHandPressed()
	{
		selectedCharacter.DiscardCurrentHand();
		selectedCharacter.DrawCardsToHand(startingHandSize);
		SkipCharacterTurn();
	}

	public void SkipCharacterTurn()
	{
		TransferTurn();
		DisableTurnoverButtons();
	}

	void EndCombatAfterWin()
	{
		foreach (CharacterGraphic mercGraphic in mercGraphics)
		{
			mercGraphic.SetStartArmor();
			mercGraphic.RemoveAllCharacterCards();
		}
		handPeekingAllowed = false;
		RemoveAllRoomStipulationCards();
		selectedCharacter.HideMyDisplayedHand();
		foreach (CharacterGraphic merc in mercGraphics)
		{
			merc.DiscardCurrentHand();
		}
		DisableTurnoverButtons();
		ShowRewards();
	}
	

	void ShowRewards()
	{
		RewardCard[] rewardsSelection;
		if (!playedEncounter.IsFinished())
		{
			rewardsSelection = playedEncounter.GetRewardCardSelection(2);
			playedEncounter.DiscardRewardCards(rewardsSelection);
		}
		else
		{
			rewardsSelection = playedEncounter.GetMissionEndRewards();
		}
		foreach (RewardCard reward in rewardsSelection)
		{
			RewardCardGraphic newGraphic = Instantiate(rewardCardPrefab);
			newGraphic.AssignCard(reward);
			newGraphic.transform.SetParent(centerPlayArea, false);
		}
	}

	public void RewardCardPlay(RewardCardGraphic cardGraphic)
	{
		StartCoroutine("RewardCardPlaying", cardGraphic);
	}

	IEnumerator RewardCardPlaying(RewardCardGraphic cardGraphic)
	{
		foreach (RewardCardGraphic graphic in centerPlayArea.GetComponentsInChildren<RewardCardGraphic>())
			graphic.GetComponent<Button>().interactable = false;

		PutCardToCenter(cardGraphic);
		cardGraphic.assignedCard.PlayCard();
		//yield return new WaitForSeconds(cardPlayAnimationTime);
		//GameObject.Destroy(cardGraphic.gameObject);
		HideRewards();
		ProgressThroughEncounter();
		yield break;
	}

	void HideRewards()
	{
		foreach (RewardCardGraphic graphic in centerPlayArea.GetComponentsInChildren<RewardCardGraphic>())
		{
			GameObject.Destroy(graphic.gameObject);
		}
	}

	void CharacterKilled(CharacterGraphic killedCharGraphic)
	{
		if (selectedCharacter == killedCharGraphic)
		{
			killedCharGraphic.HideMyDisplayedHand();
			NullifySelectedCharacter();
		}
		killedCharGraphic.DoCleanupAfterCharacterDeath();

		if (killedCharGraphic.GetCharacterType() == typeof(PartyMember)) 
			RemoveMercenary(killedCharGraphic);
		else 
			RemoveEnemy(killedCharGraphic);
	}

	void NullifySelectedCharacter()
	{
		selectedCharacter = null;
		selectionArrow.SetParent(transform, false);
	}

	void AddMercenary(Mercenary newMerc)
	{
		MercGraphic newCharGraphic = Instantiate(mercGraphicPrefab);
		newCharGraphic.AssignCharacter(newMerc);
		newCharGraphic.transform.SetParent(mercsGroup,false);
		mercGraphics.Add(newCharGraphic);
		newCharGraphic.SetStartArmor();
	}

	void RemoveMercenary(CharacterGraphic removedMerc)
	{
		if (mercGraphics.Contains(removedMerc))
		{
			mercGraphics.Remove(removedMerc);
			GameObject.Destroy(removedMerc.gameObject);
		}
	}

	void AddEnemy(Character newEnemy)
	{
		EnemyGraphic newCharGraphic = Instantiate(enemyGraphicPrefab);
		newCharGraphic.AssignCharacter(newEnemy);
		newCharGraphic.transform.SetParent(enemiesGroup, false);
		enemyGraphics.Add(newCharGraphic);

		newCharGraphic.SetStartArmor();
	}

	void RemoveEnemy(CharacterGraphic removedEnemy)
	{
		if (enemyGraphics.Contains(removedEnemy))
		{
			enemyGraphics.Remove(removedEnemy);
			GameObject.Destroy(removedEnemy.gameObject);
		}
	}

	public void ResetAllMercsResource(CharacterGraphic.Resource resource)
	{
		ResetResourceForGroup(resource,mercGraphics);
	}

	void ResetResourceForGroup(CharacterGraphic.Resource resource, List<CharacterGraphic> groupList)
	{
		foreach (CharacterGraphic graphic in groupList)
			graphic.ResetCharacterResource(resource);
	}

	public void IncrementAllCharactersResource(CharacterGraphic.Resource resource, int delta)
	{
		IncrementAllMercsResource(resource, delta);
		IncrementAllEnemiesResource(resource, delta);//
	}

	public void IncrementAllMercsResource(CharacterGraphic.Resource resource, int delta)
	{
		IncrementResourceForGroup(resource, delta, mercGraphics);
	}

	public void IncrementAllEnemiesResource(CharacterGraphic.Resource resource, int delta)
	{
		IncrementResourceForGroup(resource, delta, enemyGraphics);
	}

	void IncrementResourceForGroup(CharacterGraphic.Resource resource, int delta, List<CharacterGraphic> groupList)
	{
		foreach (CharacterGraphic graphic in groupList)
			graphic.IncrementCharacterResource(resource, delta);
	}

	public void DamageAllMercs(int damage)
	{
		DamageAllMercs(damage, false);
	}
	public void DamageAllMercs(int damage, bool ignoreArmor)
	{
		foreach (CharacterGraphic graphic in new List<CharacterGraphic>(mercGraphics))
		{
			graphic.TakeDamage(damage, ignoreArmor);
		}
	}

	public void DamageOpposingTeam(int damage)
	{
		if (turnStatus == TurnStatus.Player)
			DamageAllEnemies(damage);
		else
			DamageAllMercs(damage);
	}

	public void DamageAllCharacters(int damage)
	{
		DamageAllMercs(damage);
		DamageAllEnemies(damage);
	}

	public void DamageAllEnemies(int damage)
	{
		DamageAllEnemies(damage, false);
	}
	public void DamageAllEnemies(int damage, bool ignoreArmor)
	{
		foreach (CharacterGraphic graphic in new List<CharacterGraphic>(enemyGraphics))
		{
			graphic.TakeDamage(damage, ignoreArmor);
		}
	}

	public void PlaceRoomStipulationCard(RoomStipulationCard playedCard)
	{
		VisualCardGraphic cardGraphic = Instantiate(visualCardPrefab);
		cardGraphic.AssignCard(playedCard);
		cardGraphic.transform.SetParent(roomCardsGroup.transform, false);
		playedCard.ActivateCard();
		activeRoomCards.Add(playedCard);
	}

	public void RemoveRoomStipulationCard(RoomStipulationCard removedCard)
	{
		removedCard.DeactivateCard();
		foreach (VisualCardGraphic graphic in roomCardsGroup.GetComponentsInChildren<VisualCardGraphic>())
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


	public void SetNewMeleeTargetMerc(CharacterGraphic newTargetMerc)
	{
		if (!mercGraphics.Contains(newTargetMerc))
			throw new System.Exception("Trying to set a melee target merc that doesn't exist in mission!");
		//Important to do the event first
		if (ENewMeleeTargetMercSet != null)
			ENewMeleeTargetMercSet();
		ClearMeleeTargetMerc();
		meleeCardTargetOverrideMerc = newTargetMerc;
		
	}
	public void ClearMeleeTargetMerc()
	{
		meleeCardTargetOverrideMerc = null;
	}

	public void SetNewMeleeTargetEnemy(CharacterGraphic newTargetEnemy)
	{
		if (!enemyGraphics.Contains(newTargetEnemy))
			throw new System.Exception("Trying to set a melee target enemy that doesn't exist in mission!");
		//Important to do the event first
		if (ENewMeleeTargetEnemySet != null)
			ENewMeleeTargetEnemySet();
		ClearMeleeTargetEnemy();
		meleeCardTargetOverrideEnemy = newTargetEnemy;

	}
	public void ClearMeleeTargetEnemy()
	{
		meleeCardTargetOverrideEnemy = null;
	}

	public void SetNewRangedTargetEnemy(CharacterGraphic newTargetEnemy)
	{
		if (!enemyGraphics.Contains(newTargetEnemy))
			throw new System.Exception("Trying to set a range target enemy that doesn't exist in mission!");
		//Important to do the event first
		if (ENewRangedTargetEnemySet != null)
			ENewRangedTargetEnemySet();
		ClearRangedTargetEnemy();
		rangedCardTargetOverrideEnemy = newTargetEnemy;

	}
	public void ClearRangedTargetEnemy()
	{
		rangedCardTargetOverrideEnemy = null;
	}

	public void SetNewRangedTargetMerc(CharacterGraphic newTargetMerc)
	{
		if (!mercGraphics.Contains(newTargetMerc))
			throw new System.Exception("Trying to set a range target merc that doesn't exist in mission!");
		//Important to do the event first
		if (ENewRangedTargetMercSet != null)
			ENewRangedTargetMercSet();
		ClearRangedTargetMerc();
		rangedCardTargetOverrideMerc = newTargetMerc;

	}
	public void ClearRangedTargetMerc()
	{
		rangedCardTargetOverrideMerc = null;
	}

	public void SetRangedAttacksRestriction(bool allowed)
	{
		rangedAttacksAllowed = allowed;
	}


	public int GetCurrentEnemyCount()
	{
		return enemyGraphics.Count;
	}

	public int GetCurrentMercCount()
	{
		return mercGraphics.Count;
	}


	public void PeekCharacterHand(CharacterGraphic shownCharacter)
	{
		handPeekingAllowed = false; //TEMPORARY TESTING MEASURE, CHANGE THIS LATER!!!
		if (handPeekingAllowed && shownCharacter!=selectedCharacter)
		{
			selectedCharacter.HideMyDisplayedHand();
			shownCharacter.DisplayMyHand(false);
		}
	}

	public void StopPeekingCharacterHand(CharacterGraphic shownCharacter)
	{
		handPeekingAllowed = false; //TEMPORARY TESTING MEASURE, CHANGE THIS LATER!!!
		if (handPeekingAllowed)
		{
			shownCharacter.HideMyDisplayedHand();
			selectedCharacter.DisplayMyHand(true);
		}
	}


	void DisplayCenterMessage(string message)
	{
		selectModeMessage.gameObject.SetActive(true);
		selectModeMessage.GetComponentInChildren<Text>().text = message;
	}
	void HideCenterMessage()
	{
		selectModeMessage.gameObject.SetActive(false);
	}

	void PutCardToCenter(CardGraphic playedCardGraphic)
	{
		playedCardGraphic.transform.SetParent(centerPlayArea, false);
	}

	void PutCardToCharacter(CardGraphic playedCardGraphic, CharacterGraphic character)
	{
		playedCardGraphic.transform.SetParent(character.appliedCardsGroup, false);
		//playedCardGraphic.transform.SetParent(mainPanel);
		//playedCardGraphic.transform.SetAsLastSibling();
	}

	public bool ShowingEncounter()
	{
		return missionOngoing;
	}

	void Start() 
	{ 
		main = this;
		skipTurnButton.onClick.AddListener(()=>SkipCharacterTurn());
		discardHandButton.onClick.AddListener(() => DiscardHandPressed());
		finishButton.onClick.AddListener(() => CloseScreen());
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			//PartyMember[] newTeam = new PartyMember[1];
			//newTeam[0]= new PartyMember();
			//OpenScreen(new Encounter(true), newTeam);
			EndCombatAfterWin();
		}
	}

}
