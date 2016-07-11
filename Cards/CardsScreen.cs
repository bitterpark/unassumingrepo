using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardsScreen : MonoBehaviour 
{
	public delegate void RoundOverDeleg();
	public static event RoundOverDeleg ERoundIsOver;
	public delegate void SetMeleeTargetMercDeleg();
	public static event SetMeleeTargetMercDeleg ENewMeleeTargetMercSet;

	bool rangedAttacksAllowed = true;
	public void SetRangedAttacksRestriction(bool allowed)
	{
		rangedAttacksAllowed = allowed;
	}

	public const float cardPlayAnimationTime = 1.75f;

	public Transform enemiesGroup;
	//public Transform enemyHand;
	public Transform mercsGroup;
	//public Transform playerHand;
	public Transform roomCardsGroup;

	public Transform encounterDeckGroup;

	public Transform centerPlayArea;

	public CombatCardGraphic combatCardPrefab;
	public VisualCardGraphic visualCardPrefab;
	public RewardCardGraphic rewardCardPrefab;
	public MercGraphic mercGraphicPrefab;
	public CharacterGraphic enemyGraphicPrefab;
	public EncounterCardGraphic encounterCardPrefab;
	
	public CharacterGraphic selectedCharacter;
	CharacterGraphic meleeCardTargetOverrideMerc = null;

	public static CardsScreen main;

	public enum TurnStatus {Player,Enemy};
	public TurnStatus turnStatus;

	public Transform selectionArrow;

	public Transform mainPanel;

	public Image selectModeMessage;
	public Button skipTurnButton;
	public Button discardHandButton;
	public Button finishButton;

	public List<CharacterGraphic> mercGraphics=new List<CharacterGraphic>();
	public List<CharacterGraphic> enemyGraphics = new List<CharacterGraphic>();

	//public Deck<EncounterCard> encounterDeck = new Deck<EncounterCard>();
	//List<EncounterCard> encounter = new List<EncounterCard>();
	//Deck<RewardCard> rewards;

	//Dictionary<Character, Deck<CombatCard>> characterDecks = new Dictionary<Character, Deck<CombatCard>>();
	//Dictionary<Character, List<CombatCard>> characterHands = new Dictionary<Character, List<CombatCard>>();

	public List<RoomStipulationCard> activeRoomCards = new List<RoomStipulationCard>();

	public const int startingHandSize = 3;
	const int maxHandSize = 8;
	const int cardDrawPerTurn = 1;
	const int staminaRegen = 1;
	const int enemyTeamSize = 4;

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
		RemoveAllRoomCards();
	
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

	public void PlayEncounterCard(EncounterCardGraphic playedCardGraphic)
	{
		RoomCard playedCard = playedCardGraphic.assignedCard;
		GameObject.Destroy(playedCardGraphic.gameObject);
		ClearEncounterSelection();
		StartCombat(playedCard, playedEncounter.GenerateEnemies(enemyTeamSize));
	}

	void ShowNextRoomSelection()
	{
		foreach (RoomCard room in playedEncounter.GetRoomSelection(2))
		{
			EncounterCardGraphic cardGraphic = Instantiate(encounterCardPrefab);
			cardGraphic.AssignCard(room);
			cardGraphic.transform.SetParent(centerPlayArea, false);
		}
		/*
		foreach (EncounterCard card in encounter)
		{
			EncounterCardGraphic cardGraphic = Instantiate(encounterCardPrefab);
			cardGraphic.AssignCard(card);
			cardGraphic.transform.SetParent(centerPlayArea, false);
		}*/
	}

	void ClearEncounterSelection()
	{
		foreach (EncounterCardGraphic graphic in centerPlayArea.GetComponentsInChildren<EncounterCardGraphic>())
		{
			GameObject.Destroy(graphic.gameObject);
		}
	}


	void StartCombat(RoomCard playedRoom ,params EncounterEnemy[] enemies)
	{
		handPeekingAllowed = true;
		foreach (EncounterEnemy enemy in enemies) AddEnemy(enemy);

		//Deal out all hands
		foreach (CharacterGraphic enemy in enemyGraphics)
		{
			enemy.SetStartStamina();
			enemy.DrawCardsToHand(startingHandSize);
			//characterHands[enemy.GetCharacter()].AddRange(characterDecks[enemy.GetCharacter()].DrawCards(startingHandSize));
		}
		foreach (CharacterGraphic merc in mercGraphics)
		{
			merc.SetStartStamina();
			//merc.SetStartArmor();
			merc.DrawCardsToHand(startingHandSize);
		}
		/*
		foreach (RoomCard card in playedRoom.GetRoomCards())
		{
			PlaceRoomCard(card);
		}
		*/
		List<RoomStipulationCard> randomRoomCards = playedRoom.GetRandomRoomStipulationCards(1);
		foreach (RoomStipulationCard card in randomRoomCards)
			PlaceRoomCard(card);

		StartPlayerTurn();
	}

	void StartPlayerTurn()
	{
		foreach (CharacterGraphic merc in mercGraphics)
		{
			merc.GiveTurn();
		}
		turnStatus = TurnStatus.Player;
		SelectCharacter(mercGraphics[0]);
	}

	public void ClickCombatCard(CombatCardGraphic cardGraphic)
	{
		CombatCard playedCard = cardGraphic.assignedCard;
		if (CardRequirementsMet(playedCard))
		{
			StartCoroutine("PlayerTurnProcess",cardGraphic);
		}
	}

	IEnumerator PlayerTurnProcess(CombatCardGraphic cardGraphic)
	{
		CombatCard playedCard = cardGraphic.assignedCard;
		DisableTurnoverButtons();
		if (playedCard.targetType == CombatCard.TargetType.SelectEnemy || playedCard.targetType == CombatCard.TargetType.SelectFriendly)
		{
			yield return StartCoroutine("SelectCharacterTargetByPlayer", cardGraphic);
		}
		else
		{
			AssignCardTargets(cardGraphic.assignedCard);
			yield return StartCoroutine("CombatCardPlayed", cardGraphic);
		}
		//See if all enemies/players died during the card play
		/*
		if (!PlayerWonOrLost())
		{
			//See if the current merc died during his card play
			if (selectedCharacter == null) 
				TransferTurn();
		}*/
	}

	IEnumerator SelectCharacterTargetByPlayer(CombatCardGraphic playedCardGraphic)
	{
		CharacterGraphic targetChar = null;
		List<CharacterGraphic> targetGroup= null;
		string centerMessageText = "Select enemy";
		if (playedCardGraphic.assignedCard.targetType == CombatCard.TargetType.SelectEnemy)
		{
			targetGroup = enemyGraphics;
			centerMessageText = "Select enemy";
		}
		if (playedCardGraphic.assignedCard.targetType == CombatCard.TargetType.SelectFriendly)
		{
				targetGroup = mercGraphics;
				centerMessageText = "Select friendly";
		}
		if (targetGroup == null) 
			throw new System.Exception("Target group not found for character select");

		DisablePlayerActionsDuringCardPlay();
		DisplayCenterMessage(centerMessageText);

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
				PointerEventData pointerData = new PointerEventData(EventSystem.current);
				pointerData.position = Input.mousePosition;
				List<RaycastResult> raycastResults = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerData, raycastResults);

				foreach (RaycastResult result in raycastResults)
				{
					if (result.gameObject.GetComponentInParent<CharacterGraphic>() != null)//.GetComponent<CharacterGraphic>() != null)
					{
						targetChar = result.gameObject.GetComponentInParent<CharacterGraphic>();
						break;
					}
				}
				if (targetChar != null)
					if (targetGroup.Contains(targetChar)) break;
					else
						targetChar = null;
			}
			yield return new WaitForEndOfFrame();
		}

		HideCenterMessage();
		if (targetChar != null)
		{
			playedCardGraphic.assignedCard.targetChars.Add(targetChar);
			yield return StartCoroutine("CombatCardPlayed", playedCardGraphic);
		}
	}

	void StartEnemyTurn()
	{
		foreach (CharacterGraphic enemy in enemyGraphics)
		{
			enemy.GiveTurn();
		}
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

		if (selectedCharacter.characterHand.Count > 0)
		{
			foreach (CombatCardGraphic cardGraphic in selectedCharacter.GetHandGraphics())
			{
				if (CardRequirementsMet(cardGraphic.assignedCard))
					playableCards.Add(cardGraphic);
			}
		}
		return playableCards;
	}

	bool CardRequirementsMet(CombatCard card)
	{
		if (selectedCharacter.GetStamina() >= card.staminaCost
			&& selectedCharacter.GetAmmo() >= card.ammoCost)
		{
			if (card.cardType == CombatCard.CardType.Ranged_Attack && rangedAttacksAllowed)
			{
				return true;
			}
			if (card.cardType != CombatCard.CardType.Ranged_Attack) return true;
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
		//The following two are only used by the AI
		if (playedCard.targetType == CombatCard.TargetType.SelectEnemy)
		{
			playedCard.targetChars.Add(opposingGroup[Random.Range(0, opposingGroup.Count)]);
		}
		if (playedCard.targetType == CombatCard.TargetType.SelectFriendly)
		{
			playedCard.targetChars.Add(friendlyGroup[Random.Range(0, opposingGroup.Count)]);
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
		if (turnStatus == TurnStatus.Enemy)
		{
			if (meleeCardTargetOverrideMerc != null && playedCard.GetType().BaseType == typeof(MeleeCard))
			{
				playedCard.targetChars.Clear();
				playedCard.targetChars.Add(meleeCardTargetOverrideMerc);
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
		selectedCharacter.characterHand.Remove(playedCard);
		playedCard.originDeck.DiscardCards(playedCard);
		playedCard.PlayCard();
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
		selectedCharacter.SetHandInteractivity(false);
	}

	void ReenablePlayerActions()
	{
		handPeekingAllowed = true;
		selectedCharacter.SetHandInteractivity(true);
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
		if (!PlayerWonOrLost())
		{
			if (selectedCharacter!=null)
			{
				//int cardsMissing = startingHandSize - characterHands[selectedCharacter.GetCharacter()].Count;
				//characterHands[selectedCharacter.GetCharacter()].AddRange(characterDecks[selectedCharacter.GetCharacter()].DrawCards(cardDrawPerTurn));
				selectedCharacter.TurnDone();
			}
			
			if (turnStatus == TurnStatus.Player)
			{
				//ClearHand(playerHand);
				foreach (CharacterGraphic mercGraphic in mercGraphics)
					if (mercGraphic.HasTurn())
					{
						SelectCharacter(mercGraphic);
						return;
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
						return;
					}
				StartCoroutine(StartNewRound());
			}
		}
	}

	IEnumerator StartNewRound()
	{
		RoundStaminaRegen();
		if (ERoundIsOver != null) ERoundIsOver();
		CleanupCharactersWhoDied();

		if (!PlayerWonOrLost())
		{
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

	void SelectCharacter(CharacterGraphic newSelectedCharacter)
	{
		if (selectedCharacter != null)
			selectedCharacter.ClearDisplayedHand();
		
		selectedCharacter = newSelectedCharacter;
		/*
		if (characterHands[selectedCharacter.GetCharacter()].Count < maxHandSize)
		{	
			List<CombatCard> addedCards = characterDecks[selectedCharacter.GetCharacter()].DrawCards(cardDrawPerTurn);
			characterHands[selectedCharacter.GetCharacter()].AddRange(addedCards);
		}*/

		if (turnStatus == TurnStatus.Player)
		{
			SelectPlayersCharacter();
		}
		else
		{
			SelectEnemyCharacter();
		}
		selectedCharacter.StartedTurn();
	}

	void SelectPlayersCharacter()
	{
		//Set anchor to bottom left corner
		selectionArrow.GetComponent<RectTransform>().anchorMin = Vector2.zero;
		selectionArrow.GetComponent<RectTransform>().anchorMax = Vector2.zero;
		selectionArrow.SetParent(selectedCharacter.transform, false);
		selectionArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, 20f);//GetComponent<RectTransform>().Translate(-20f,-10f,0);//Translate(0, 20f, 0);

		ReenablePlayerActions();
		EnableTurnoverButtons();
	}

	void SelectEnemyCharacter()
	{
		//Set anchor to upper left corner
		selectionArrow.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1f);
		selectionArrow.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1f);
		selectionArrow.SetParent(selectedCharacter.transform, false);
		selectionArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, -20f);//selectionArrow.GetComponent<RectTransform>().Translate(20f, -10f, 0);

		DisableTurnoverButtons();
		handPeekingAllowed = false;
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
	}

	void EndCombatAfterWin()
	{
		foreach (CharacterGraphic mercGraphic in mercGraphics)
		{
			mercGraphic.SetStartArmor();
			mercGraphic.RemoveAllCharacterCards();
		}
		handPeekingAllowed = false;
		RemoveAllRoomCards();
		selectedCharacter.ClearDisplayedHand();
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
		CharacterGraphic newCharGraphic = Instantiate(enemyGraphicPrefab);
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

	public void PlaceRoomCard(RoomStipulationCard playedCard)
	{
		VisualCardGraphic cardGraphic = Instantiate(visualCardPrefab);
		cardGraphic.AssignCard(playedCard);
		cardGraphic.transform.SetParent(roomCardsGroup.transform, false);
		playedCard.ActivateCard();
		activeRoomCards.Add(playedCard);
	}

	public void RemoveRoomCard(RoomStipulationCard removedCard)
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

	void RemoveAllRoomCards()
	{
		foreach (RoomStipulationCard card in new List<RoomStipulationCard>(activeRoomCards))
		{
			RemoveRoomCard(card);
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

	public void PeekCharacterHand(CharacterGraphic shownCharacter)
	{
		handPeekingAllowed = false; //TEMPORARY TESTING MEASURE, CHANGE THIS LATER!!!
		if (handPeekingAllowed && shownCharacter!=selectedCharacter)
		{
			selectedCharacter.ClearDisplayedHand();
			shownCharacter.DisplayHand(false);
		}
	}

	public void StopPeekingCharacterHand(CharacterGraphic shownCharacter)
	{
		handPeekingAllowed = false; //TEMPORARY TESTING MEASURE, CHANGE THIS LATER!!!
		if (handPeekingAllowed)
		{
			shownCharacter.ClearDisplayedHand();
			selectedCharacter.DisplayHand(true);
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
