using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public abstract class CharacterGraphic : MonoBehaviour {

	public FloatingText floatingTextPrefab;
	public CharStipulationCardGraphic stipulationCardPrefab;

	public HandManager handManager;

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

	List<CharacterStipulationCard> activeStipulationCards = new List<CharacterStipulationCard>();

	public enum Resource {Health,Armor,Stamina,Ammo};


	public delegate void TookDamageDeleg();
	public event TookDamageDeleg ETookDamage;
	public delegate void RangedBlockAssignedDeleg();
	public event RangedBlockAssignedDeleg ERangedBlockAssigned;
	public delegate void MeleeBlockAssignedDeleg();
	public event MeleeBlockAssignedDeleg EMeleeBlockAssigned;

	bool hasTurn = false;

	int ammo = 0;
	int stamina = 0;
	int currentMaxStamina = 0;
	int armor = 0;

	bool canRegenStaminaNextRound = true;

	bool meleeAttacksAreFree = false;
	bool rangedAttacksAreFree = false;
	bool rangedAttacksCostStamina = false;

	public bool meleeBlockActive = false;
	public bool rangedBlockActive = false;

	int rangedAttacksAmmoCostReduction = 0;


	public void AssignCharacter(Character newChar)
	{
		assignedCharacter = newChar;
		//CAUTION - Enemy char graphics do not have this button in their prefab, so trying to call this on a non-merc char graphic
		//will result in an exception
		if (newChar.GetType() == typeof(PartyMember))
		{
			portrait.GetComponent<Button>().onClick.AddListener(
				() => InventoryScreenHandler.mainISHandler.ToggleSelectedMember(assignedCharacter as PartyMember));
		}

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
		handManager.AddCardsToAssignedDeck(cards);
	}
	public void RemoveCardsFromCurrentDeck(params CombatCard[] cards)
	{
		
		currentCharacterDeck.RemoveCards(cards);
	}


	public List<CombatCard> DrawCardsFromCharacterDeck(int count)
	{
		return currentCharacterDeck.DrawCards(count,true);
	}

	public CombatDeck GetCharactersCombatDeck()
	{
		return assignedCharacter.GetCombatDeck();//currentCharacterDeck;
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
		if (ETookDamage != null && GetHealth()>0)
			ETookDamage();
	}

	public void ResetCharacterResource(Resource resource)
	{
		if (resource == Resource.Health)
			throw new System.Exception("Cannot reset character health!");
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
		if (delta < 0)
			canRegenStaminaNextRound = false;
		SetStamina(stamina + delta);
	}

	public void SetStartStamina()
	{
		SetMaxStamina(assignedCharacter.GetMaxStamina());
		RegenStamina();
	}

	void RegenStamina()
	{
		if (stamina<currentMaxStamina)
			SetStamina(currentMaxStamina);
	}

	public int GetMaxStamina()
	{
		return currentMaxStamina;
	}

	public void IncrementMaxStamina(int delta)
	{
		SetMaxStamina(currentMaxStamina + delta);
	}

	public void SetMaxStamina(int newMaxStamina)
	{
		currentMaxStamina = Mathf.Max(newMaxStamina, 0);
		UpdateStaminaText();
	}

	public void SetStamina(int newStamina)
	{
		stamina = newStamina;
		if (stamina < 0) 
			stamina = 0;
		//if (stamina > currentMaxStamina)
			//stamina = currentMaxStamina;
		UpdateStaminaText();
	}

	void UpdateStaminaText()
	{
		staminaText.text = stamina.ToString() + "/" + currentMaxStamina;
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

	public void GiveTurn()
	{
		SetTurnPossibility(true);
	}
	public virtual void RemoveTurn()
	{
		SetTurnPossibility(false);
	}

	public void RoundStarted()
	{
		if (canRegenStaminaNextRound)
			RegenStamina();
		else
			canRegenStaminaNextRound = true;
	}
	public void TurnFinished()
	{
		RemoveTurn();
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

	public void SetAllBlocks(bool active)
	{
		SetMeleeBlock(active);
		SetRangedBlock(active);
	}

	public void SetMeleeBlock(bool active)
	{
		meleeBlockActive = active;
		if (meleeBlockActive && EMeleeBlockAssigned != null)
			EMeleeBlockAssigned();
	}
	public void SetRangedBlock(bool active)
	{
		rangedBlockActive = active;
		if (rangedBlockActive && ERangedBlockAssigned != null)
			ERangedBlockAssigned();
	}

	public void ChangeRangedAttacksAmmoCostReduction(int costReductionDelta)
	{
		rangedAttacksAmmoCostReduction = Mathf.Min(0, rangedAttacksAmmoCostReduction+costReductionDelta);
	}

	public void SetRangedAttacksCostStamina(bool costStamina)
	{
		rangedAttacksCostStamina = costStamina;
	}

	public bool CharacterMeetsCardRequirements(CombatCardGraphic cardGraphic)
	{
		return CharacterMeetsCardRequirements(cardGraphic.GetAssignedCard());
	}

	public bool CharacterMeetsCardRequirements(CombatCard card)
	{
		if (!hasTurn)
			return false;
		
		int staminaReq = card.staminaCost;
		int ammoReq = card.ammoCost;
		
		if (card.GetType().BaseType==typeof(MeleeCard) && meleeAttacksAreFree)
			staminaReq = 0;
		

		if (card.GetType().BaseType == typeof(RangedCard))
		{
			if (rangedAttacksAreFree)
				ammoReq = 0;

			else
			{
				ammoReq = Mathf.Max(0,ammoReq-rangedAttacksAmmoCostReduction);
				if (rangedAttacksCostStamina)
				{
					staminaReq += ammoReq;
					ammoReq = 0;
				}
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

	public void RemovePlayedCombatCardFromHand(CombatCard playedCard)
	{
		handManager.RemoveCardFromHand(playedCard);
	}

	public bool IsCardBlocked(CombatCard card)
	{
		if (card.GetType() == typeof(MeleeCard) && meleeBlockActive)
			return true;
		if (card.GetType() == typeof(RangedCard) && rangedBlockActive)
			return true;

		return false;
	}

	public void SubtractCardCostsFromResources(bool useUpStamina, int ammoCost, int staminaCost,int maxStaminaCost, int takeDamageCost, int removeHealthCost)
	{
		if (!rangedAttacksAreFree)
		{
			ammoCost = Mathf.Max(0,ammoCost-rangedAttacksAmmoCostReduction);
			if (rangedAttacksCostStamina)
			{
				staminaCost += ammoCost;
				ammoCost = 0;
			}
			IncrementAmmo(-ammoCost);
		}

		if (!meleeAttacksAreFree)
		{
			if (useUpStamina)
				IncrementStamina(-GetStamina());
			else
				IncrementStamina(-staminaCost);
		}
		TakeDamage(takeDamageCost);
		TakeDamage(removeHealthCost, true);
		IncrementMaxStamina(-maxStaminaCost);
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
	/*
	public void HandWidgetHoverStart() 
	{
		MissionUIToggler.main.PeekCharacterHand(this); 
	}

	public void HandWidgetHoverEnd() 
	{ 
		MissionUIToggler.main.StopPeekingCharacterHand(this);
	}*/
}
