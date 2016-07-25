using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EncounterEnemy: Character 
{
	public static EncounterEnemy CreateEnemyOfSetPowerLevel(System.Type enemyType,PowerLevel powerLevel)
	{
		EncounterEnemy newEnemy=(EncounterEnemy)System.Activator.CreateInstance(enemyType);
		if (powerLevel == PowerLevel.Normal)
			newEnemy.NormalConstructor();
		if (powerLevel == PowerLevel.Tough)
			newEnemy.ToughConstructor();
		return newEnemy;
	}


	//NON-STATIC
	public string name;
	public int health
	{
		get {return _health;}
		set 
		{
			_health=value;
		}
	}
	int _health;

	public int xCoord;
	public int yCoord;

	public Color color;
	//protected int barricadeBashStrength=0;

	public Sprite GetSprite() {return SpriteBase.mainSpriteBase.genericEnemySprite;}
	
	public Vector2 GetCoords() {return new Vector2(xCoord,yCoord);}
	public void SetCoords(Vector2 newCoords) {xCoord=(int)newCoords.x; yCoord=(int)newCoords.y;}
	
	public EncounterEnemy() {}

	public enum PowerLevel {Normal, Tough};

	public EncounterEnemy(PowerLevel powerLevel)
	{
		if (powerLevel==PowerLevel.Tough)
			ToughConstructor();
		else
			NormalConstructor();
	}

	protected abstract void NormalConstructor();
	protected abstract void ToughConstructor();

	//NEW STUFF
	protected CombatDeck basicCombatDeck = new CombatDeck();
	protected int stamina=0;
	protected int ammo = 0;
	protected int armor = 0;
	protected List<EnemyVariationCard> variationCards = new List<EnemyVariationCard>();

	public List<EnemyVariationCard> GetRandomVariationCards(int countRequired)
	{
		List<EnemyVariationCard> resultList = new List<EnemyVariationCard>(variationCards);
		if (variationCards.Count > 0)
		{
			if (countRequired > variationCards.Count)
				countRequired = variationCards.Count;
			while (resultList.Count > countRequired)
				resultList.RemoveAt(Random.Range(0, resultList.Count));
		}
		return resultList;
	}

	public string GetName()
	{
		return name;
	}

	public int GetHealth()
	{
		return health;
	}

	public void IncrementHealth(int delta)
	{
		SetHealth(health + delta);
	}

	public void SetHealth(int newValue)
	{
		health = newValue;
	}

	public int GetStartArmor()
	{
		return armor;
	}

	public int GetMaxStamina()
	{
		return stamina;
	}

	public void SetStamina(int newStamina)
	{
		stamina = newStamina;
		if (stamina < 0) stamina = 0;
	}

	public int GetStartAmmo() { return ammo; }

	public Sprite GetPortrait()
	{
		return SpriteBase.mainSpriteBase.enemyPortrait;
	}

	public CombatDeck GetCombatDeck()
	{
		return basicCombatDeck;
	}

	public void TakeDamage(int damage) { health -= damage; }
}