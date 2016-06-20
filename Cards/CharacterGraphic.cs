using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterGraphic : MonoBehaviour {

	public Text nameText;
	public Text healthText;
	public Text armorText;
	public Text staminaText;
	public Text ammoText;
	public Image portrait;

	public Transform appliedCardsGroup;

	Character assignedCharacter;

	bool hasTurn = false;

	int ammo = 0;
	int stamina = 0;
	int armor = 0;

	public void AssignCharacter(Character newChar)
	{
		assignedCharacter = newChar;
		//CAUTION - Enemy char graphics do not have this button in their prefab, so trying to call this on a non-merc char graphic
		//will result in an exception
		if (newChar.GetType()==typeof(PartyMember))
		{
			portrait.GetComponent<Button>().onClick.AddListener(
				()=>InventoryScreenHandler.mainISHandler.MapOrEncounterToggleSelectedMember(assignedCharacter as PartyMember));
		}
		nameText.text = newChar.GetName();
		healthText.text = newChar.GetHealth().ToString();
		portrait.sprite = newChar.GetPortrait();
		SetStamina(newChar.GetStartStamina());
		SetAmmo(newChar.GetAmmo());
		SetArmor(newChar.GetStartArmor());
	}

	public Character GetCharacter()
	{
		return assignedCharacter;
	}

	public System.Type GetCharacterType()
	{
		return assignedCharacter.GetType();
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
		int totalDamage = damage;
		if (!ignoreArmor)
		{
			if (armor > 0)
			{
				totalDamage -= armor;
				IncrementArmor(-damage);
			}
		}
		if (totalDamage > 0)
		{
			TakeHealthDamage(totalDamage);
		}
	}

	public void TakeHealthDamage(int damage)
	{
		assignedCharacter.TakeDamage(damage);
		if (assignedCharacter.GetHealth() <= 0)
		{
			CardsScreen.main.CharacterKilled(this);
		}
		else
			healthText.text = assignedCharacter.GetHealth().ToString();
	}

	public int GetArmor()
	{
		return armor;
	}

	public void IncrementArmor(int delta)
	{
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
		//assignedCharacter.SetStamina(assignedCharacter.GetStartStamina() + delta);
		SetStamina(stamina + delta);
	}

	public void SetStartStamina()
	{
		SetStamina(assignedCharacter.GetStartStamina());
	}

	public void SetStamina(int newStamina)
	{
		stamina = newStamina;
		if (stamina < 0) stamina = 0;
		staminaText.text = stamina.ToString();
	}

	public void IncrementAmmo(int delta) { SetAmmo(ammo + delta); }

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

	public bool HasTurn()
	{
		return hasTurn;
	}

	public void GiveTurn()
	{
		hasTurn = true;
	}
	public void TurnDone()
	{
		hasTurn = false;
	}

	public void HandWidgetHoverStart() { CardsScreen.main.PeekCharacterHand(assignedCharacter); }

	public void HandWidgetHoverEnd() { CardsScreen.main.StopPeekingCharacterHand();}

}
