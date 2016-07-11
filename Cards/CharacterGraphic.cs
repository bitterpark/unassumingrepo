using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharacterGraphic : MonoBehaviour {

	public FloatingText floatingTextPrefab;
	public VisualCardGraphic visualCardPrefab;
	public CombatCardGraphic combatCardPrefab;

	public Text nameText;
	public Text healthText;
	public Text armorText;
	public Text staminaText;
	public Text ammoText;
	public Image portrait;

	public Transform appliedCardsGroup;
	public Transform characterHandGroup;

	bool floatingTextIsOut = false;

	Character assignedCharacter;

	bool hasTurn = false;

	int ammo = 0;
	int stamina = 0;
	int armor = 0;

	public CombatDeck characterDeck;
	public List<CombatCard> characterHand = new List<CombatCard>();

	public List<CharacterStipulationCard> activeStipulationCards = new List<CharacterStipulationCard>();

	public enum Resource {Health,Armor,Stamina,Ammo};

	public delegate void TurnStartDeleg();
	public event TurnStartDeleg ECharacterTurnStarted;

	public void AssignCharacter(Character newChar)
	{
		assignedCharacter = newChar;
		//CAUTION - Enemy char graphics do not have this button in their prefab, so trying to call this on a non-merc char graphic
		//will result in an exception
		if (newChar.GetType()==typeof(PartyMember))
		{
			portrait.GetComponent<Button>().onClick.AddListener(
				()=>InventoryScreenHandler.mainISHandler.ToggleSelectedMember(assignedCharacter as PartyMember));
		}
		nameText.text = newChar.GetName();
		healthText.text = newChar.GetHealth().ToString();
		portrait.sprite = newChar.GetPortrait();

		SetStartStamina();
		SetStartAmmo();
		SetStartArmor();

		characterDeck = assignedCharacter.GetCombatDeck();
		characterHand.AddRange(characterDeck.DrawCards(CardsScreen.startingHandSize));
	}

	public void DrawCardsToHand()
	{
		DrawCardsToHand(CardsScreen.startingHandSize);
	}
	public void DrawCardsToHand(int count)
	{
		characterHand.AddRange(characterDeck.DrawCards(count));
	}

	public void DiscardCurrentHand()
	{
		characterDeck.DiscardCards(characterHand.ToArray());
		characterHand.Clear();
	}

	public virtual void DisplayHand(bool interactable)
	{
		foreach (CombatCard card in characterHand)
		{
			CombatCardGraphic newCardGraphic = Instantiate(combatCardPrefab);
			newCardGraphic.AssignCard(card);
			newCardGraphic.transform.SetParent(characterHandGroup, false);
			newCardGraphic.GetComponent<Button>().interactable = interactable;
		}
	}

	public void SetHandInteractivity(bool interactable)
	{
		foreach (CombatCardGraphic card in characterHandGroup.GetComponentsInChildren<CombatCardGraphic>())
		{
			card.GetComponent<Button>().interactable=interactable;
		}
	}

	public void ClearDisplayedHand()
	{
		foreach (CombatCardGraphic card in characterHandGroup.GetComponentsInChildren<CombatCardGraphic>())
		{
			GameObject.Destroy(card.gameObject);
		}
	}

	public List<CombatCardGraphic> GetHandGraphics()
	{
		List<CombatCardGraphic> graphics = new List<CombatCardGraphic>();
		foreach (CombatCardGraphic graphic in characterHandGroup.GetComponentsInChildren<CombatCardGraphic>())
			graphics.Add(graphic);
		return graphics;
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
		staminaText.text = stamina.ToString();
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


	public void PlaceCharacterCard(CharacterStipulationCard playedCard)
	{
		VisualCardGraphic cardGraphic = Instantiate(visualCardPrefab);
		cardGraphic.AssignCard(playedCard);
		cardGraphic.transform.SetParent(appliedCardsGroup, false);
		cardGraphic.transform.SetAsFirstSibling();
		playedCard.ActivateCard(this);
		activeStipulationCards.Add(playedCard);
	}

	public void RemoveCharacterCard(CharacterStipulationCard removedCard)
	{
		removedCard.DeactivateCard();
		foreach (VisualCardGraphic graphic in appliedCardsGroup.GetComponentsInChildren<VisualCardGraphic>())
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
			RemoveCharacterCard(card);
		}
	}

	public bool HasTurn()
	{
		return hasTurn;
	}

	public virtual void StartedTurn()
	{
		DoTurnEventAndDrawCards();
		DisplayHand(false);
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
		hasTurn = true;
	}
	public void TurnDone()
	{
		hasTurn = false;
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
