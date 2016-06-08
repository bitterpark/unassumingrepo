using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardsScreen : MonoBehaviour 
{
	const float cardPlayAnimationTime = 1f;

	public Transform enemiesGroup;
	public Transform enemyHand;
	public Transform mercsGroup;
	public Transform playerHand;

	public Transform encounterDeckGroup;

	public Transform cardPlaySpot;

	public CombatCardGraphic combatCardPrefab;
	public VisualCardGraphic visualCardPrefab;
	public RewardCardGraphic rewardCardPrefab;
	public CharacterGraphic mercGraphicPrefab;
	public CharacterGraphic enemyGraphicPrefab;
	
	public CharacterGraphic selectedCharacter;

	public static CardsScreen main;

	public enum TurnStatus {Player,Enemy};
	public TurnStatus turnStatus;

	public Transform selectionArrow;

	public Image selectModeMessage;
	public Button skipTurnButton;
	public Button discardHandButton;

	public List<CharacterGraphic> mercGraphics=new List<CharacterGraphic>();
	public List<CharacterGraphic> enemyGraphics = new List<CharacterGraphic>();

	//public Deck<EncounterCard> encounterDeck = new Deck<EncounterCard>();
	List<EncounterCard> encounter = new List<EncounterCard>();
	Deck<RewardCard> rewards;

	Dictionary<Character, Deck<CombatCard>> characterDecks = new Dictionary<Character, Deck<CombatCard>>();
	Dictionary<Character, List<CombatCard>> characterHands = new Dictionary<Character, List<CombatCard>>();

	const int handSize = 4;

	bool handPeekingAllowed = false;

	public void OpenScreen()
	{
		GetComponent<Canvas>().enabled = true;
		AddMercenary(new PartyMember(MapManager.main.mapRegions[0]));
		AddMercenary(new PartyMember(MapManager.main.mapRegions[0]));

		encounter.Clear();
		encounter.Add(new Hallway());
		encounter.Add(new EngineRoom());
		rewards = new Deck<RewardCard>();
		rewards.Populate(new CashStash(),new CashStash(), new AmmoStash(), new AmmoStash(), new AmmoStash(), new AmmoStash());
		rewards.Shuffle();
		StartCoroutine(ProgressThroughEncounter());
	}

	IEnumerator ProgressThroughEncounter()
	{
		if (encounter.Count > 0)
		{
			EncounterCard nextEncounterCard = encounter[0];
			encounter.Remove(nextEncounterCard);
			ShowEncounterDeck();
			VisualCardGraphic nextCardGraphic = Instantiate(visualCardPrefab);
			nextCardGraphic.AssignCard(nextEncounterCard);
			PutCardInCenterSpot(nextCardGraphic);
			yield return new WaitForSeconds(cardPlayAnimationTime);
			GameObject.Destroy(nextCardGraphic.gameObject);
			HideEncounterDeck();
			StartCombat(GenerateRandomEnemyEncounter(3));
		}
		else CloseScreen();
		yield break;
	}

	EncounterEnemy[] GenerateRandomEnemyEncounter(int enemiesCount)
	{
		//Dictionary<System.Type, int> enemyWeights;
		List<System.Type> enemyTypes = new List<System.Type>();
		enemyTypes.Add(typeof(Stinger));
		enemyTypes.Add(typeof(Skitter));
		enemyTypes.Add(typeof(Bugzilla));
		EncounterEnemy[] resultList = new EncounterEnemy[enemiesCount];
		for (int i = 0; i < enemiesCount; i++)
		{
			resultList[i] = (EncounterEnemy)System.Activator.CreateInstance(enemyTypes[Random.Range(0,enemyTypes.Count)]);
		}
		return resultList;
	}

	void ShowEncounterDeck()
	{
		foreach (EncounterCard card in encounter)
		{
			VisualCardGraphic cardGraphic = Instantiate(visualCardPrefab);
			cardGraphic.AssignCard(card);
			cardGraphic.transform.SetParent(playerHand, false);
		}
	}

	void HideEncounterDeck()
	{
		//foreach (VisualCardGraphic card in playerHand.GetComponentsInChildren<VisualCardGraphic>()) GameObject.Destroy(card.gameObject);
		ClearDisplayedHand(playerHand);
	}

	void CloseScreen()
	{
		selectionArrow.SetParent(this.transform,false);
		ClearDisplayedHand(playerHand);
		ClearDisplayedHand(enemyHand);
		foreach (CharacterGraphic graphic in new List<CharacterGraphic>(mercGraphics)) RemoveMercenary(graphic);
		foreach (CharacterGraphic graphic in new List<CharacterGraphic>(enemyGraphics)) RemoveEnemy(graphic);
		GetComponent<Canvas>().enabled = false;
	}

	void PutCardInCenterSpot(CardGraphic playedCardGraphic)
	{
		playedCardGraphic.transform.SetParent(cardPlaySpot, false);
	}

	void PutCardToCharacter(CardGraphic playedCardGraphic, CharacterGraphic character)
	{
		playedCardGraphic.transform.SetParent(character.appliedCardsGroup,false);
	}

	public void CharacterKilled(CharacterGraphic killedCharGraphic)
	{
		if (killedCharGraphic.assignedCharacter.GetType() == typeof(PartyMember)) RemoveMercenary(killedCharGraphic);
		else RemoveEnemy(killedCharGraphic);
	}

	void StartCombat(params EncounterEnemy[] enemies)
	{
		foreach (EncounterEnemy enemy in enemies) AddEnemy(enemy);
		//characterHands[mercGraphics[0].assignedCharacter].AddRange(characterDecks[mercGraphics[0].assignedCharacter].DrawCards(4));
		//Deal out all hands
		foreach (CharacterGraphic enemy in enemyGraphics)
			characterHands[enemy.assignedCharacter].AddRange(characterDecks[enemy.assignedCharacter].DrawCards(handSize));
		foreach (CharacterGraphic merc in mercGraphics)
			characterHands[merc.assignedCharacter].AddRange(characterDecks[merc.assignedCharacter].DrawCards(handSize));
		StartPlayerTurn();
	}


	void StartPlayerTurn()
	{
		foreach (CharacterGraphic mercGraphic in mercGraphics) 
		{ mercGraphic.GiveTurn(); mercGraphic.SetStamina(mercGraphic.assignedCharacter.GetStartStamina()); }
		List<CombatCardGraphic> playerHandCards=new List<CombatCardGraphic>(playerHand.GetComponentsInChildren<CombatCardGraphic>());
		foreach (CombatCardGraphic card in playerHandCards) card.GetComponent<Button>().interactable = true;
		turnStatus = TurnStatus.Player;
		SelectCharacter(mercGraphics[0]);
	}

	public void PlayerCombatCardPlay(CombatCardGraphic cardGraphic)
	{
		CombatCard playedCard = cardGraphic.assignedCard;
		if (selectedCharacter.stamina >= playedCard.staminaCost
			&& selectedCharacter.assignedCharacter.GetHealth() >= playedCard.ammoCost)
		{
			discardHandButton.interactable = false;
			if (playedCard.targetType == CombatCard.TargetType.Character)
			{
				if (turnStatus == TurnStatus.Player) StartCoroutine("CharacterTargetCombatCardPlaying", cardGraphic);
			}
			else StartCoroutine("CombatCardPlayed", cardGraphic);
		}
	}

	IEnumerator CharacterTargetCombatCardPlaying(CombatCardGraphic playedCardGraphic)
	{
		CharacterGraphic targetChar = null;
		selectModeMessage.gameObject.SetActive(true);
		handPeekingAllowed = false;
		selectModeMessage.GetComponentInChildren<Text>().text = "Select enemy";
		foreach (CombatCardGraphic card in playerHand.GetComponentsInChildren<CombatCardGraphic>()) card.GetComponent<Button>().interactable = false;
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
			playedCardGraphic.assignedCard.targetCharGraphic = targetChar;
			yield return StartCoroutine("CombatCardPlayed", playedCardGraphic);
		}
		handPeekingAllowed = true;
		foreach (CombatCardGraphic card in playerHand.GetComponentsInChildren<CombatCardGraphic>()) card.GetComponent<Button>().interactable = true;
	}

	void StartEnemyTurn()
	{
		foreach (CharacterGraphic enemyGraphic in enemyGraphics)
		{ enemyGraphic.GiveTurn(); enemyGraphic.SetStamina(enemyGraphic.assignedCharacter.GetStartStamina()); }
		foreach (CombatCardGraphic card in playerHand.GetComponentsInChildren<CombatCardGraphic>()) card.GetComponent<Button>().interactable = false;
		turnStatus = TurnStatus.Enemy;
		//print("Enemy turn started");
		SelectCharacter(enemyGraphics[0]);
		StartCoroutine(EnemyTurnProcess());
	}

	IEnumerator EnemyTurnProcess()
	{
		yield return new WaitForSeconds(cardPlayAnimationTime);
		List<CombatCard> playableCards = SortOutPlayableEnemyCards();
		while (playableCards.Count > 0)
		{
			CombatCard playedCard = playableCards[Random.Range(0, playableCards.Count)];

			if (playedCard.targetType == CombatCard.TargetType.Character) playedCard.targetCharGraphic = mercGraphics[Random.Range(0, mercGraphics.Count)];

			CombatCardGraphic playedCardGraphic = null;
			foreach (CombatCardGraphic cardGraphic in enemyHand.GetComponentsInChildren<CombatCardGraphic>())
				if (cardGraphic.assignedCard == playedCard) { playedCardGraphic = cardGraphic; break; }

			if (playedCardGraphic == null) throw new System.Exception("No graphic found for played card!");

			yield return StartCoroutine("CombatCardPlayed", playedCardGraphic);
			playableCards = SortOutPlayableEnemyCards();
		}
		TransferTurn();
		yield break;
	}

	List<CombatCard> SortOutPlayableEnemyCards()
	{
		List<CombatCard> playableCards = new List<CombatCard>();

		if (characterHands[selectedCharacter.assignedCharacter].Count > 0)
		{
			foreach (CombatCard card in characterHands[selectedCharacter.assignedCharacter])
			{
				if (selectedCharacter.stamina >= card.staminaCost
					&& selectedCharacter.assignedCharacter.GetHealth() >= card.ammoCost)
					playableCards.Add(card);
			}
		}
		return playableCards;
	}

	IEnumerator CombatCardPlayed(CombatCardGraphic playedCardGraphic)
	{
		handPeekingAllowed = false;
		CombatCard playedCard = playedCardGraphic.assignedCard;
		if (playedCard.targetType==CombatCard.TargetType.None) PutCardInCenterSpot(playedCardGraphic);
		if (playedCard.targetType == CombatCard.TargetType.Character) PutCardToCharacter(playedCardGraphic, playedCard.targetCharGraphic);

		yield return new WaitForSeconds(cardPlayAnimationTime);
		GameObject.Destroy(playedCardGraphic.gameObject);

		playedCard.userCharGraphic = selectedCharacter;
		playedCard.PlayCard();
		characterHands[selectedCharacter.assignedCharacter].Remove(playedCard);
		playedCard.originDeck.DiscardCards(playedCard);

		//If neither all mercs nor all enemies died as a result of the turn, continue turn order as usual
		/*
		if (!CheckIfPlayerWonOrLost())
		{
			if (selectedCharacter.assignedCharacter.GetHealth() > 0) 
			{	
				int cardsMissing = handSize - characterHands[selectedCharacter.assignedCharacter].Count;
				characterHands[selectedCharacter.assignedCharacter].AddRange(characterDecks[selectedCharacter.assignedCharacter].DrawCards(cardsMissing)); 
			}
			TransferTurn();
		}*/
		if (turnStatus == TurnStatus.Player) handPeekingAllowed = true;
		yield break;
	}

	void TransferTurn()
	{

		if (!CheckIfPlayerWonOrLost())
		{
			if (selectedCharacter.assignedCharacter.GetHealth() > 0)
			{
				int cardsMissing = handSize - characterHands[selectedCharacter.assignedCharacter].Count;
				characterHands[selectedCharacter.assignedCharacter].AddRange(characterDecks[selectedCharacter.assignedCharacter].DrawCards(cardsMissing));
			}

			selectedCharacter.TurnDone();
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
				StartPlayerTurn();
			}
		}
	}

	bool CheckIfPlayerWonOrLost()
	{
		bool winOrLossHappened = false;
		if (enemyGraphics.Count == 0)
		{
			winOrLossHappened = true;
			EndCombat();
		}
		if (mercGraphics.Count == 0)
		{
			winOrLossHappened = true;
			CloseScreen();
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
			
			skipTurnButton.interactable = true;
			discardHandButton.interactable = true;
			handPeekingAllowed = true;
			DisplayHand(playerHand, characterHands[newSelectedCharacter.assignedCharacter].ToArray());
		}
		else
		{
			//Set anchor to upper left corner
			selectionArrow.GetComponent<RectTransform>().anchorMin = new Vector2(0,1f);
			selectionArrow.GetComponent<RectTransform>().anchorMax = new Vector2(0,1f);
			selectionArrow.SetParent(newSelectedCharacter.transform, false);

			handPeekingAllowed = false;
			skipTurnButton.interactable = false;
			discardHandButton.interactable = false;
			DisplayHand(enemyHand, characterHands[newSelectedCharacter.assignedCharacter].ToArray());
		}
	}

	public void DiscardHand()
	{
		characterDecks[selectedCharacter.assignedCharacter].DiscardCards(characterHands[selectedCharacter.assignedCharacter].ToArray());
		characterHands[selectedCharacter.assignedCharacter].Clear();
		characterHands[selectedCharacter.assignedCharacter].AddRange(characterDecks[selectedCharacter.assignedCharacter].DrawCards(handSize));
		SkipCharacterTurn();
	}

	public void SkipCharacterTurn()
	{
		TransferTurn();
	}

	void EndCombat()
	{
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
		foreach (RewardCard reward in rewards.DrawCards(3))
		{
			RewardCardGraphic newGraphic = Instantiate(rewardCardPrefab);
			newGraphic.AssignCard(reward);
			newGraphic.transform.SetParent(playerHand, false);
			rewards.DiscardCards(reward);
		}
	}

	public void RewardCardPlay(RewardCardGraphic cardGraphic)
	{
		StartCoroutine("RewardCardPlaying", cardGraphic);
	}

	IEnumerator RewardCardPlaying(RewardCardGraphic cardGraphic)
	{
		PutCardInCenterSpot(cardGraphic);
		cardGraphic.assignedCard.PlayCard();
		yield return new WaitForSeconds(cardPlayAnimationTime);
		GameObject.Destroy(cardGraphic.gameObject);
		HideRewards();
		StartCoroutine(ProgressThroughEncounter());
		yield break;
	}

	void HideRewards()
	{
		ClearDisplayedHand(playerHand);
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
		characterDecks.Remove(removedMerc.assignedCharacter);
		characterHands.Remove(removedMerc.assignedCharacter);
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
		characterDecks.Remove(removedEnemy.assignedCharacter);
		characterHands.Remove(removedEnemy.assignedCharacter);
		GameObject.Destroy(removedEnemy.gameObject);
	}

	public void PeekCharacterHand(Character shownCharacter)
	{
		if (handPeekingAllowed && shownCharacter!=selectedCharacter.assignedCharacter)
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
			if (selectedCharacter.assignedCharacter.GetType() == typeof(PartyMember))
				DisplayHand(playerHand, characterHands[selectedCharacter.assignedCharacter].ToArray());
			else DisplayHand(enemyHand, characterHands[selectedCharacter.assignedCharacter].ToArray());
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

	void Start() 
	{ 
		main = this;
		skipTurnButton.onClick.AddListener(()=>SkipCharacterTurn());
		discardHandButton.onClick.AddListener(() => DiscardHand());
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.C)) OpenScreen(); 
	}

}
