using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public abstract class CharacterGraphic : MonoBehaviour {

	public FloatingText floatingTextPrefab;
	public CharStipulationCardGraphic stipulationCardPrefab;

	public Text nameText;
	public Text healthText;
	public Text armorText;
	public Text staminaText;
	public Text ammoText;
	public Image portrait;
	public Image background;

	public Transform appliedCardsGroup;

	bool floatingTextIsOut = false;

	protected Character assignedCharacter;

	protected CombatDeck currentCharacterDeck;
	protected List<CombatCard> characterHand = new List<CombatCard>();

	List<CharacterStipulationCard> activeStipulationCards = new List<CharacterStipulationCard>();

	public enum Resource {Health,Armor,Stamina,Ammo};

	public delegate void TurnStartDeleg();
	public event TurnStartDeleg ECharacterTurnStarted;

	public delegate void TookDamageDeleg();
	public event TookDamageDeleg ETookDamage;

	bool hasTurn = false;

	int ammo = 0;
	int stamina = 0;
	int armor = 0;
	bool meleeAttacksAreFree = false;
	bool rangedAttacksAreFree = false;
	bool rangedAttacksCostStamina = false;

	HandDisplayingObject myHandDisplayer;

	public void AssignCharacter(Character newChar)
	{
		assignedCharacter = newChar;
		//CAUTION - Enemy char graphics do not have this button in their prefab, so trying to call this on a non-merc char graphic
		//will result in an exception
		if (newChar.GetType() == typeof(PartyMember))
		{
			portrait.GetComponent<Button>().onClick.AddListener(
				() => InventoryScreenHandler.mainISHandler.ToggleSelectedMember(assignedCharacter as PartyMember));
			myHandDisplayer = CardsScreen.main.playerHandObject;
		}
		else
			myHandDisplayer = CardsScreen.main.enemyHandObject;

		nameText.text = newChar.GetName();
		healthText.text = newChar.GetHealth().ToString();
		portrait.sprite = newChar.GetPortrait();

		SetStartStamina();
		SetStartAmmo();
		SetStartArmor();

		//currentCharacterDeck = assignedCharacter.GetCombatDeck();
	}

	public abstract void GenerateCombatStartDeck();

	public void AddCardsToCurrentDeck(params CombatCard[] cards)
	{
		currentCharacterDeck.AddCards(cards);
	}
	public void RemoveCardsFromCurrentDeck(params CombatCard[] cards)
	{
		currentCharacterDeck.RemoveCards(cards);
	}

	public void DrawCardsToHand()
	{
		DrawCardsToHand(CardsScreen.startingHandSize);
	}
	public void DrawCardsToHand(int count)
	{
		characterHand.AddRange(currentCharacterDeck.DrawCards(count));
	}

	public void RemoveCardFromHand(CombatCard card)
	{
		characterHand.Remove(card);
	}

	public void DiscardCurrentHand()
	{
		currentCharacterDeck.DiscardCards(characterHand.ToArray());
		characterHand.Clear();
	}

	public void DisplayMyHand(bool interactable)
	{
		myHandDisplayer.DisplayHand(interactable, characterHand);
	}

	public void SetMyHandInteractivity(bool interactable)
	{
		myHandDisplayer.SetHandInteractivity(interactable);
	}

	public void HideMyDisplayedHand()
	{
		myHandDisplayer.HideDisplayedHand();
	}

	public List<CombatCardGraphic> GetHandGraphics()
	{
		return myHandDisplayer.GetHandGraphics();
	}

	public Character GetCharacter()
	{
		return assignedCharacter;
	}

	public System.Type GetCharacterType()
	{
		return assignedCharacter.GetType();
	}


	public int GetEffectiveHitpoints()
	{
		return GetHealth() + GetArmor();
	}

	public int GetHealth()
	{
		return assignedCharacter.GetHealth();
	}

	public void TakeDamage(int damage)
	{
		TakeDamage(damage, false);
	}

	public void TakeDamage(int damage, bool ignoreArmor)
	{
		if (ETookDamage != null)
			ETookDamage();

		int damageToHealth = damage;
		if (!ignoreArmor)
		{
			if (armor > 0)
			{
				damageToHealth = damage - armor;
				if (damageToHealth < 0)
					damageToHealth = 0;
				int damageToArmor = damage - damageToHealth;
				IncrementArmor(-damageToArmor);
			}
		}
		if (damageToHealth > 0)
		{
			IncrementHealth(-damageToHealth);
		}
	}

	public void ResetCharacterResource(Resource resource)
	{
		if (resource == Resource.Health)
			throw new System.Exception("Canno reset character health!");
		if (resource == Resource.Stamina)
			SetStartStamina();
		if (resource == Resource.Armor)
			SetStartArmor();
		if (resource == Resource.Ammo)
			SetStartAmmo();
	}

	public void IncrementCharacterResource(Resource resource, int delta)
	{
		if (resource == Resource.Health)
			IncrementHealth(delta);
		if (resource == Resource.Stamina)
			IncrementStamina(delta);
		if (resource == Resource.Armor)
			IncrementArmor(delta);
		if (resource == Resource.Ammo)
			IncrementAmmo(delta);
	}

	public void IncrementHealth(int delta)
	{
		assignedCharacter.IncrementHealth(delta);
		StartCoroutine("AddFloatingTextToQueue", new FloatingTextInfo(delta, Resource.Health));
		healthText.text = assignedCharacter.GetHealth().ToString();
	}

	public int GetArmor()
	{
		return armor;
	}

	public void IncrementArmor(int delta)
	{
		StartCoroutine("AddFloatingTextToQueue", new FloatingTextInfo(delta, Resource.Armor));
		SetArmor(armor + delta);
	}

	public void SetStartArmor()
	{
		SetArmor(assignedCharacter.GetStartArmor());
	}

	public void SetArmor(int newArmor)
	{
		armor = newArmor;
		if (armor < 0) 
			armor = 0;
		armorText.text = armor.ToString();
	}

	public int GetStamina()
	{
		return stamina;
	}

	public void IncrementStamina(int delta)
	{
		StartCoroutine("AddFloatingTextToQueue", new FloatingTextInfo(delta, Resource.Stamina));
		SetStamina(stamina + delta);
	}

	public void SetStartStamina()
	{
		SetStamina(assignedCharacter.GetMaxStamina());
	}

	public void SetStamina(int newStamina)
	{
		stamina = newStamina;
		if (stamina < 0) stamina = 0;
		if (stamina > assignedCharacter.GetMaxStamina())
			SetStartStamina();
		staminaText.text = stamina.ToString() + "/" + assignedCharacter.GetMaxStamina();
	}

	public void SetStartAmmo()
	{
		SetAmmo(assignedCharacter.GetStartAmmo());
	}

	public void IncrementAmmo(int delta) 
	{
		StartCoroutine("AddFloatingTextToQueue",new FloatingTextInfo(delta,Resource.Ammo));
		SetAmmo(ammo + delta); 
	}

	public int GetAmmo()
	{
		return ammo;
	}

	public void SetAmmo(int newAmmo)
	{
		ammo = newAmmo;
		if (ammo < 0) ammo = 0;
		ammoText.text = ammo.ToString();
	}


	public void DoCleanupAfterCharacterDeath()
	{
		RemoveAllCharacterCards();
	}


	public void TryPlaceCharacterStipulationCard(CharacterStipulationCard playedCard)
	{
		foreach (CharacterStipulationCard card in activeStipulationCards)
		{
			if (card.GetType() == playedCard.GetType())
			{
				RemoveCharacterStipulationCard(card);
				break;
			}
		}

		CharStipulationCardGraphic cardGraphic = Instantiate(stipulationCardPrefab);
		cardGraphic.AssignCard(playedCard);
		cardGraphic.transform.SetParent(appliedCardsGroup, false);
		cardGraphic.transform.SetAsFirstSibling();
		playedCard.ActivateCard(this);
		activeStipulationCards.Add(playedCard);
	}

	public void RemoveCharacterStipulationCard(CharacterStipulationCard removedCard)
	{
		removedCard.DeactivateCard();
		foreach (CharStipulationCardGraphic graphic in appliedCardsGroup.GetComponentsInChildren<CharStipulationCardGraphic>())
		{
			if (graphic.assignedCard == removedCard)
			{
				GameObject.Destroy(graphic.gameObject);
				break;
			}
		}
		activeStipulationCards.Remove(removedCard);
	}

	public void RemoveAllCharacterCards()
	{
		foreach (CharacterStipulationCard card in new List<CharacterStipulationCard>(activeStipulationCards))
		{
			RemoveCharacterStipulationCard(card);
		}
	}

	public bool HasTurn()
	{
		return hasTurn;
	}

	public virtual void StartedTurn()
	{
		DoTurnEventAndDrawCards();
	}

	protected void DoTurnEventAndDrawCards()
	{
		if (ECharacterTurnStarted != null)
			ECharacterTurnStarted();
		DiscardCurrentHand();
		DrawCardsToHand();
	}

	public void GiveTurn()
	{
		SetTurnPossibility(true);
	}
	public void RemoveTurn()
	{
		SetTurnPossibility(false);
	}
	public void TurnDone()
	{
		HideMyDisplayedHand();
		SetTurnPossibility(false);
	}
	void SetTurnPossibility(bool hasTurn)
	{
		this.hasTurn = hasTurn;
		if (hasTurn)
			background.color = Color.white;
		else
			background.color = Color.gray;
	}

	public void SetMeleeAttacksFree(bool free)
	{
		meleeAttacksAreFree = free;
	}

	public void SetRangedAttacksFree(bool free)
	{
		rangedAttacksAreFree = free;
	}

	public void SetRangedAttacksCostStamina(bool costStamina)
	{
		rangedAttacksCostStamina = costStamina;
	}

	public bool CharacterMeetsCardRequirements(CombatCard card)
	{
		int staminaReq = card.staminaCost;
		int ammoReq = card.ammoCost;

		if (card.GetType().BaseType==typeof(MeleeCard) && meleeAttacksAreFree)
			staminaReq = 0;

		if (card.GetType().BaseType == typeof(RangedCard))
		{
			if (rangedAttacksAreFree)
				ammoReq = 0;
			else
				if (rangedAttacksCostStamina)
				{
					staminaReq += ammoReq;
					ammoReq = 0;
				}
		}

		if (GetStamina() < staminaReq)
			return false;
		if (GetAmmo() < ammoReq)
			return false;
		if (!card.SpecialPrerequisitesMet(this))
			return false;

		return true;
	}

	public void SubtractCardCostsFromResources(int ammoCost, int staminaCost, int takeDamageCost, int removeHealthCost)
	{
		if (!rangedAttacksAreFree)
		{
			if (rangedAttacksCostStamina)
			{
				staminaCost += ammoCost;
				ammoCost = 0;
			}
			IncrementAmmo(-ammoCost);
		}
		
		if (!meleeAttacksAreFree)
			IncrementStamina(-staminaCost);

		TakeDamage(takeDamageCost);
		TakeDamage(removeHealthCost, true);
	}

	struct FloatingTextInfo
	{
		public int value;
		public Resource resource;

		public FloatingTextInfo(int newValue, Resource newResource)
		{
			value = newValue;
			resource = newResource;
		}
	}

	IEnumerator AddFloatingTextToQueue(FloatingTextInfo info)
	{
		float readyTextTimeout = 0.4f;
		
		if (floatingTextIsOut)
			yield return new WaitForSeconds(readyTextTimeout);

		CreateFloatingText(info.value,info.resource);

		floatingTextIsOut = true;
		yield return new WaitForSeconds(readyTextTimeout);
		floatingTextIsOut = false;
		yield break;
	}

	void CreateFloatingText(int value, Resource resourceType)
	{
		if (value != 0)
		{
			FloatingText newText = Instantiate(floatingTextPrefab);
			Transform textStartpoint = null;
			if (resourceType == Resource.Health)
				textStartpoint = healthText.transform;
			if (resourceType == Resource.Armor)
				textStartpoint = armorText.transform;
			if (resourceType == Resource.Stamina)
				textStartpoint = staminaText.transform;
			if (resourceType == Resource.Ammo)
				textStartpoint = ammoText.transform;

			newText.AssignFloatingText(value, textStartpoint, resourceType);
		}
	}

	public void HandWidgetHoverStart() 
	{ 
		CardsScreen.main.PeekCharacterHand(this); 
	}

	public void HandWidgetHoverEnd() 
	{ 
		CardsScreen.main.StopPeekingCharacterHand(this);
	}
}
