﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardsScreen : MonoBehaviour 
{
	public delegate void RoundOverDeleg();
	public static event RoundOverDeleg ERoundIsOver;

	bool rangedAttacksAllowed = true;
	public void SetRangedAttacksRestriction(bool allowed)
	{
		rangedAttacksAllowed = allowed;
	}

	const float cardPlayAnimationTime = 1.5f;

	public Transform enemiesGroup;
	public Transform enemyHand;
	public Transform mercsGroup;
	public Transform playerHand;
	public Transform roomCardsGroup;

	public Transform encounterDeckGroup;

	public Transform centerPlayArea;

	public CombatCardGraphic combatCardPrefab;
	public VisualCardGraphic visualCardPrefab;
	public RewardCardGraphic rewardCardPrefab;
	public CharacterGraphic mercGraphicPrefab;
	public CharacterGraphic enemyGraphicPrefab;
	public EncounterCardGraphic encounterCardPrefab;
	
	public CharacterGraphic selectedCharacter;

	public static CardsScreen main;

	public enum TurnStatus {Player,Enemy};
	public TurnStatus turnStatus;

	public Transform selectionArrow;

	public Image selectModeMessage;
	public Button skipTurnButton;
	public Button discardHandButton;
	public Button finishButton;

	public List<CharacterGraphic> mercGraphics=new List<CharacterGraphic>();
	public List<CharacterGraphic> enemyGraphics = new List<CharacterGraphic>();

	//public Deck<EncounterCard> encounterDeck = new Deck<EncounterCard>();
	//List<EncounterCard> encounter = new List<EncounterCard>();
	//Deck<RewardCard> rewards;

	Dictionary<Character, Deck<CombatCard>> characterDecks = new Dictionary<Character, Deck<CombatCard>>();
	Dictionary<Character, List<CombatCard>> characterHands = new Dictionary<Character, List<CombatCard>>();

	public List<RoomCard> activeRoomCards = new List<RoomCard>();

	const int handSize = 3;
	const int staminaRegen = 1;
	const int enemyTeamSize = 4;

	bool handPeekingAllowed = false;

	Encounter playedEncounter;

	bool encounterOngoing = false;

	public void OpenScreen(Encounter newEncounter, PartyMember[] team)
	{
		encounterOngoing = true;
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
		encounterOngoing = false;
		selectModeMessage.gameObject.SetActive(false);
		finishButton.gameObject.SetActive(false);
		
		selectionArrow.SetParent(this.transform, false);
		RemoveAllRoomCards();
		ClearDisplayedHand(playerHand);
		ClearDisplayedHand(enemyHand);
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
		if (!playedEncounter.IsFinished()) ShowEncounterSelection();
		else
		{
			selectModeMessage.gameObject.SetActive(true);
			selectModeMessage.GetComponentInChildren<Text>().text = "Run complete";
			finishButton.gameObject.SetActive(true);
		}
	}

	/*
	IEnumerator ProgressThroughEncounter()
	{
		if (encounter.Count > 0)
		{
			EncounterCard nextEncounterCard = encounter[0];
			encounter.Remove(nextEncounterCard);
			ShowEncounterDeck();
			VisualCardGraphic nextCardGraphic = Instantiate(visualCardPrefab);
			nextCardGraphic.AssignCard(nextEncounterCard);
			PutCardToCenter(nextCardGraphic);
			yield return new WaitForSeconds(cardPlayAnimationTime);
			GameObject.Destroy(nextCardGraphic.gameObject);
			HideEncounterDeck();
			StartCombat(GenerateRandomEnemyEncounter(3));
		}
		else CloseScreen();
		yield break;
	}*/

	public void PlayEncounterCard(EncounterCardGraphic playedCardGraphic)
	{
		EncounterCard playedCard = playedCardGraphic.assignedCard;
		GameObject.Destroy(playedCardGraphic.gameObject);
		ClearEncounterSelection();
		StartCombat(playedCard, playedEncounter.GenerateEnemies(enemyTeamSize));
	}

	void ShowEncounterSelection()
	{
		foreach (EncounterCard room in playedEncounter.GetRoomSelection(2))
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


	void StartCombat(EncounterCard playedRoom ,params EncounterEnemy[] enemies)
	{
		handPeekingAllowed = true;
		foreach (EncounterEnemy enemy in enemies) AddEnemy(enemy);

		//Deal out all hands
		foreach (CharacterGraphic enemy in enemyGraphics)
		{
			enemy.SetStartStamina();
			enemy.SetStartArmor();
			characterHands[enemy.GetCharacter()].AddRange(characterDecks[enemy.GetCharacter()].DrawCards(handSize));
		}
			foreach (CharacterGraphic merc in mercGraphics)
		{
			merc.SetStartStamina();
			merc.SetStartArmor();
			characterHands[merc.GetCharacter()].AddRange(characterDecks[merc.GetCharacter()].DrawCards(handSize));
		}

		foreach (RoomCard card in playedRoom.GetRoomCards())
		{
			PlaceRoomCard(card);
		}

		StartPlayerTurn();
	}

	void StartPlayerTurn()
	{
		foreach (CharacterGraphic merc in mercGraphics)
		{
			merc.GiveTurn();
		}
		List<CombatCardGraphic> playerHandCards=new List<CombatCardGraphic>(playerHand.GetComponentsInChildren<CombatCardGraphic>());
		foreach (CombatCardGraphic card in playerHandCards) card.GetComponent<Button>().interactable = true;
		turnStatus = TurnStatus.Player;
		SelectCharacter(mercGraphics[0]);
	}

	public void PlayerTurnProcess(CombatCardGraphic cardGraphic)
	{
		CombatCard playedCard = cardGraphic.assignedCard;
		if (CardRequirementsMet(playedCard))
		{
			discardHandButton.interactable = false;
			if (playedCard.targetType == CombatCard.TargetType.Select)
			{
				StartCoroutine("CharacterTargetSelect", cardGraphic);
			}
			else
			{
				AssignCardTargets(cardGraphic.assignedCard);
				StartCoroutine("CombatCardPlayed", cardGraphic);
			}
		}
	}

	IEnumerator CharacterTargetSelect(CombatCardGraphic playedCardGraphic)
	{
		CharacterGraphic targetChar = null;
		selectModeMessage.gameObject.SetActive(true);
		handPeekingAllowed = false;
		selectModeMessage.GetComponentInChildren<Text>().text = "Select enemy";
		foreach (CombatCardGraphic card in playerHand.GetComponentsInChildren<CombatCardGraphic>()) 
			card.GetComponent<Button>().interactable = false;
		while (true)
		{
			if (Input.GetMouseButton(1)) break;

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
				{
					if (!mercGraphics.Contains(targetChar)) break;
				}
			}
			yield return new WaitForEndOfFrame();
		}
		selectModeMessage.gameObject.SetActive(false);
		if (targetChar != null)
		{
			playedCardGraphic.assignedCard.targetChars.Add(targetChar);
			yield return StartCoroutine("CombatCardPlayed", playedCardGraphic);
		}
		handPeekingAllowed = true;
		foreach (CombatCardGraphic card in playerHand.GetComponentsInChildren<CombatCardGraphic>()) card.GetComponent<Button>().interactable = true;
	}

	void StartEnemyTurn()
	{
		foreach (CharacterGraphic enemy in enemyGraphics)
		{
			enemy.GiveTurn();
		}
		foreach (CombatCardGraphic card in playerHand.GetComponentsInChildren<CombatCardGraphic>())
		{
			card.GetComponent<Button>().interactable = false;
		}
		turnStatus = TurnStatus.Enemy;
		SelectCharacter(enemyGraphics[0]);
		StartCoroutine(EnemyTurnProcess());
	}

	IEnumerator EnemyTurnProcess()
	{
		List<CombatCard> playableCards = SortOutPlayableEnemyCards();
		while (playableCards.Count > 0)
		{
			//yield return new WaitForSeconds(cardPlayAnimationTime);
			if (!PlayerWonOrLost())
			{
				CombatCard playedCard = playableCards[Random.Range(0, playableCards.Count)];
				AssignCardTargets(playedCard);

				CombatCardGraphic playedCardGraphic = null;
				foreach (CombatCardGraphic cardGraphic in enemyHand.GetComponentsInChildren<CombatCardGraphic>())
					if (cardGraphic.assignedCard == playedCard) { playedCardGraphic = cardGraphic; break; }

				if (playedCardGraphic == null) throw new System.Exception("No graphic found for played card!");

				yield return StartCoroutine("CombatCardPlayed", playedCardGraphic);
				//If enemy died after playing a card, finish turn
				if (selectedCharacter == null) break;
				playableCards = SortOutPlayableEnemyCards();
			}
			else
				yield break;
		}
		TransferTurn();
		yield break;
	}

	List<CombatCard> SortOutPlayableEnemyCards()
	{
		List<CombatCard> playableCards = new List<CombatCard>();

		if (characterHands[selectedCharacter.GetCharacter()].Count > 0)
		{
			foreach (CombatCard card in characterHands[selectedCharacter.GetCharacter()])
			{
				if (CardRequirementsMet(card))
					playableCards.Add(card);
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
		List<CharacterGraphic> targetGroup;
		if (turnStatus == TurnStatus.Player) targetGroup = enemyGraphics;
		else targetGroup = mercGraphics;

		if (playedCard.targetType == CombatCard.TargetType.None)
		{
			return;
		}
		if (playedCard.targetType == CombatCard.TargetType.Select)
		{
			playedCard.targetChars.Add(targetGroup[Random.Range(0, targetGroup.Count)]);
		}
		if (playedCard.targetType == CombatCard.TargetType.Random)
		{
			playedCard.targetChars.Add(targetGroup[Random.Range(0,targetGroup.Count)]);
		}
		if (playedCard.targetType == CombatCard.TargetType.All)
		{
			playedCard.targetChars.AddRange(targetGroup);
		}
		if (playedCard.targetType == CombatCard.TargetType.Weakest)
		{
			CharacterGraphic weakestCharGraphic = targetGroup[0];
			foreach (CharacterGraphic charGraphic in targetGroup)
			{
				if (charGraphic.GetHealth() < weakestCharGraphic.GetHealth()) weakestCharGraphic = charGraphic;
			}
			playedCard.targetChars.Add(weakestCharGraphic);
		}
		if (playedCard.targetType == CombatCard.TargetType.Strongest)
		{
			CharacterGraphic strongestCharGraphic = targetGroup[0];
			foreach (CharacterGraphic charGraphic in targetGroup)
			{
				if (charGraphic.GetHealth()+charGraphic.GetArmor() 
					> strongestCharGraphic.GetHealth()+strongestCharGraphic.GetArmor()) strongestCharGraphic = charGraphic;
			}
			playedCard.targetChars.Add(strongestCharGraphic);
		}
	}


	IEnumerator CombatCardPlayed(CombatCardGraphic playedCardGraphic)
	{
		DisablePlayerActionsDuringCardPlay();
		CombatCard playedCard = playedCardGraphic.assignedCard;
		if (playedCard.targetType==CombatCard.TargetType.None)
 			PutCardToCharacter(playedCardGraphic, selectedCharacter);
		if (playedCard.targetType==CombatCard.TargetType.All)
			PutCardToCenter(playedCardGraphic);
		if (playedCard.targetType!=CombatCard.TargetType.None && playedCard.targetType!=CombatCard.TargetType.All)  
			PutCardToCharacter(playedCardGraphic, playedCard.targetChars[0]);

		yield return new WaitForSeconds(cardPlayAnimationTime);
		GameObject.Destroy(playedCardGraphic.gameObject);

		playedCard.userCharGraphic = selectedCharacter;
		characterHands[selectedCharacter.GetCharacter()].Remove(playedCard);
		playedCard.originDeck.DiscardCards(playedCard);
		playedCard.PlayCard();
		

		//If neither all mercs nor all enemies died as a result of the turn, continue turn order as usual
		/*
		if (!CheckIfPlayerWonOrLost())
		{
			if (selectedCharacter.GetCharacter().GetHealth() > 0) 
			{	
				int cardsMissing = handSize - characterHands[selectedCharacter.GetCharacter()].Count;
				characterHands[selectedCharacter.GetCharacter()].AddRange(characterDecks[selectedCharacter.GetCharacter()].DrawCards(cardsMissing)); 
			}
			TransferTurn();
		}*/
		if (turnStatus == TurnStatus.Player)
			ReenablePlayerActions();
		yield break;
	}

	void DisablePlayerActionsDuringCardPlay()
	{
		handPeekingAllowed = false;
		foreach (CombatCardGraphic graphic in playerHand.GetComponentsInChildren<CombatCardGraphic>())
		{
			graphic.GetComponent<Button>().interactable = false;
		}
	}

	void ReenablePlayerActions()
	{
		handPeekingAllowed = true;
		foreach (CombatCardGraphic graphic in playerHand.GetComponentsInChildren<CombatCardGraphic>())
		{
			graphic.GetComponent<Button>().interactable = true;
		}
	}

	void TransferTurn()
	{

		if (!PlayerWonOrLost())
		{
			if (selectedCharacter!=null)
			{
				int cardsMissing = handSize - characterHands[selectedCharacter.GetCharacter()].Count;
				characterHands[selectedCharacter.GetCharacter()].AddRange(characterDecks[selectedCharacter.GetCharacter()].DrawCards(cardsMissing));
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
				StartNewRound();
				if (!PlayerWonOrLost())
					StartPlayerTurn();
			}
		}
	}

	void StartNewRound()
	{
		RoundStaminaRegen();
		if (ERoundIsOver != null) ERoundIsOver();
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
				EndCombat();
			}
		}
		return winOrLossHappened;
	}

	void SelectCharacter(CharacterGraphic newSelectedCharacter)
	{
		selectedCharacter = newSelectedCharacter;
		
		ClearDisplayedHand(playerHand);
		ClearDisplayedHand(enemyHand);
		if (turnStatus == TurnStatus.Player)
		{
			//Set anchor to bottom left corner
			selectionArrow.GetComponent<RectTransform>().anchorMin = Vector2.zero;
			selectionArrow.GetComponent<RectTransform>().anchorMax = Vector2.zero;
			selectionArrow.SetParent(newSelectedCharacter.transform, false);
			selectionArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f,20f);//GetComponent<RectTransform>().Translate(-20f,-10f,0);//Translate(0, 20f, 0);

			skipTurnButton.interactable = true;
			discardHandButton.interactable = true;
			handPeekingAllowed = true;
			DisplayHand(playerHand, characterHands[newSelectedCharacter.GetCharacter()].ToArray());
		}
		else
		{
			//Set anchor to upper left corner
			selectionArrow.GetComponent<RectTransform>().anchorMin = new Vector2(0,1f);
			selectionArrow.GetComponent<RectTransform>().anchorMax = new Vector2(0,1f);
			selectionArrow.SetParent(newSelectedCharacter.transform, false);
			selectionArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, -20f);//selectionArrow.GetComponent<RectTransform>().Translate(20f, -10f, 0);

			handPeekingAllowed = false;
			skipTurnButton.interactable = false;
			discardHandButton.interactable = false;
			DisplayHand(enemyHand, characterHands[newSelectedCharacter.GetCharacter()].ToArray());
		}
	}

	public void DiscardHand()
	{
		characterDecks[selectedCharacter.GetCharacter()].DiscardCards(characterHands[selectedCharacter.GetCharacter()].ToArray());
		characterHands[selectedCharacter.GetCharacter()].Clear();
		characterHands[selectedCharacter.GetCharacter()].AddRange(characterDecks[selectedCharacter.GetCharacter()].DrawCards(handSize));
		SkipCharacterTurn();
	}

	public void SkipCharacterTurn()
	{
		TransferTurn();
	}

	void EndCombat()
	{
		handPeekingAllowed = false;
		RemoveAllRoomCards();
		ClearDisplayedHand(enemyHand);
		ClearDisplayedHand(playerHand);
		foreach (List<CombatCard> hand in characterHands.Values)
		{
			if (hand.Count > 0) hand[0].originDeck.DiscardCards(hand.ToArray());
			hand.Clear();
		}
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
			newGraphic.transform.SetParent(playerHand, false);
		}
	}

	public void RewardCardPlay(RewardCardGraphic cardGraphic)
	{
		StartCoroutine("RewardCardPlaying", cardGraphic);
	}

	IEnumerator RewardCardPlaying(RewardCardGraphic cardGraphic)
	{
		foreach (RewardCardGraphic graphic in playerHand.GetComponentsInChildren<RewardCardGraphic>())
		{
			graphic.GetComponent<Button>().interactable = false;
		}
		PutCardToCenter(cardGraphic);
		cardGraphic.assignedCard.PlayCard();
		yield return new WaitForSeconds(cardPlayAnimationTime);
		GameObject.Destroy(cardGraphic.gameObject);
		HideRewards();
		ProgressThroughEncounter();
		yield break;
	}

	void HideRewards()
	{
		ClearDisplayedHand(playerHand);
	}

	public void CharacterKilled(CharacterGraphic killedCharGraphic)
	{
		if (selectedCharacter == killedCharGraphic)
		{
			selectedCharacter = null;
			selectionArrow.SetParent(transform, false);
		}
		if (killedCharGraphic.GetCharacterType() == typeof(PartyMember)) RemoveMercenary(killedCharGraphic);
		else RemoveEnemy(killedCharGraphic);
	}

	void AddMercenary(Character newMerc)
	{
		CharacterGraphic newCharGraphic = Instantiate(mercGraphicPrefab);
		newCharGraphic.AssignCharacter(newMerc);
		newCharGraphic.transform.SetParent(mercsGroup,false);
		mercGraphics.Add(newCharGraphic);
		characterDecks.Add(newMerc, newMerc.GetCombatDeck());
		characterHands.Add(newMerc,new List<CombatCard>());
	}

	void RemoveMercenary(CharacterGraphic removedMerc)
	{
		mercGraphics.Remove(removedMerc);
		characterDecks.Remove(removedMerc.GetCharacter());
		characterHands.Remove(removedMerc.GetCharacter());
		GameObject.Destroy(removedMerc.gameObject);
	}

	void AddEnemy(Character newEnemy)
	{
		CharacterGraphic newCharGraphic = Instantiate(enemyGraphicPrefab);
		newCharGraphic.AssignCharacter(newEnemy);
		newCharGraphic.transform.SetParent(enemiesGroup, false);
		enemyGraphics.Add(newCharGraphic);

		characterDecks.Add(newEnemy, newEnemy.GetCombatDeck());
		characterHands.Add(newEnemy, new List<CombatCard>());
	}

	void RemoveEnemy(CharacterGraphic removedEnemy)
	{
		enemyGraphics.Remove(removedEnemy);
		characterDecks.Remove(removedEnemy.GetCharacter());
		characterHands.Remove(removedEnemy.GetCharacter());
		GameObject.Destroy(removedEnemy.gameObject);
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

	void PlaceRoomCard(RoomCard playedCard)
	{
		VisualCardGraphic cardGraphic = Instantiate(visualCardPrefab);
		cardGraphic.AssignCard(playedCard);
		cardGraphic.transform.SetParent(roomCardsGroup.transform, false);
		playedCard.ActivateCard();
		activeRoomCards.Add(playedCard);
	}

	void RemoveRoomCard(RoomCard removedCard)
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
		foreach (RoomCard card in new List<RoomCard>(activeRoomCards))
		{
			RemoveRoomCard(card);
		}
	}

	public void PeekCharacterHand(Character shownCharacter)
	{
		if (handPeekingAllowed && shownCharacter!=selectedCharacter.GetCharacter())
		{
			ClearDisplayedHand(playerHand);
			ClearDisplayedHand(enemyHand);

			Transform displayedHand;
			if (shownCharacter.GetType() == typeof(PartyMember)) displayedHand = playerHand;
			else displayedHand = enemyHand;
			DisplayHand(displayedHand, characterHands[shownCharacter].ToArray());
			foreach (CombatCardGraphic graphic in displayedHand.GetComponentsInChildren<CombatCardGraphic>()) 
				graphic.GetComponent<Button>().interactable = false;
		}
	}

	public void StopPeekingCharacterHand()
	{
		if (handPeekingAllowed)
		{
			ClearDisplayedHand(playerHand);
			ClearDisplayedHand(enemyHand);
			if (selectedCharacter.GetCharacter().GetType() == typeof(PartyMember))
				DisplayHand(playerHand, characterHands[selectedCharacter.GetCharacter()].ToArray());
			else DisplayHand(enemyHand, characterHands[selectedCharacter.GetCharacter()].ToArray());
		}
	}

	void DisplayHand(Transform hand, params CombatCard[] cards)
	{
		foreach (CombatCard card in cards)
		{
			CombatCardGraphic newCardGraphic = Instantiate(combatCardPrefab);
			newCardGraphic.AssignCard(card);
			newCardGraphic.transform.SetParent(hand, false);
			if (hand == enemyHand) newCardGraphic.GetComponent<Button>().interactable = false;
		}
	}

	void ClearDisplayedHand(Transform hand)
	{
		foreach (CardGraphic card in hand.GetComponentsInChildren<CardGraphic>())
		{
			GameObject.Destroy(card.gameObject);
		}
	}

	void PutCardToCenter(CardGraphic playedCardGraphic)
	{
		playedCardGraphic.transform.SetParent(centerPlayArea, false);
	}

	void PutCardToCharacter(CardGraphic playedCardGraphic, CharacterGraphic character)
	{
		playedCardGraphic.transform.SetParent(character.appliedCardsGroup, false);
	}

	public bool ShowingEncounter()
	{
		return encounterOngoing;
	}

	void Start() 
	{ 
		main = this;
		skipTurnButton.onClick.AddListener(()=>SkipCharacterTurn());
		discardHandButton.onClick.AddListener(() => DiscardHand());
		finishButton.onClick.AddListener(() => CloseScreen());
		
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			//PartyMember[] newTeam = new PartyMember[1];
			//newTeam[0]= new PartyMember();
			//OpenScreen(new Encounter(true), newTeam);
			EndCombat();
		}
	}

}
