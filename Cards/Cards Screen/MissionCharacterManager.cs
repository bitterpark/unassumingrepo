using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public interface ICharacterResourceManipulator
{
	void ResetAllMercsResource(CharacterGraphic.Resource resource);

	void IncrementAllCharactersResource(CharacterGraphic.Resource resource, int delta);

	void IncrementAllMercsResource(CharacterGraphic.Resource resource, int delta);

	void IncrementAllEnemiesResource(CharacterGraphic.Resource resource, int delta);

	void DamageAllMercs(int damage);

	void DamageAllMercs(int damage, bool ignoreArmor);

	void DamageOpposingTeam(int damage);

	void DamageAllCharacters(int damage);

	void DamageAllEnemies(int damage);

	void DamageAllEnemies(int damage, bool ignoreArmor);

}

public class MissionCharacterManager : MonoBehaviour, ICharacterResourceManipulator 
{
	public static ICharacterResourceManipulator main;

	public Transform selectionArrow;

	public Transform enemiesGroup;
	public Transform mercsGroup;

	public MercGraphic mercGraphicPrefab;
	public EnemyGraphic enemyGraphicPrefab;

	const int staminaRegen = 1;

	List<CharacterGraphic> mercGraphics = new List<CharacterGraphic>();
	List<CharacterGraphic> enemyGraphics = new List<CharacterGraphic>();

	CombatManager combatManager;

	public void EnableCharacterManager(CombatManager combatManager)
	{
		main = this;
		this.combatManager = combatManager;
	}

	public void SpawnMercsOnMissionStart(PartyMember[] mercs)
	{
		foreach (PartyMember merc in mercs)
		{
			AddMercenary(merc);
		}
	}

	public void PrepAllCharactersForCombatStart(params EncounterEnemy[] enemies)
	{
		foreach (EncounterEnemy enemy in enemies)
			AddEnemy(enemy);

		foreach (CharacterGraphic enemy in enemyGraphics)
		{
			enemy.SetStartStamina();
			enemy.GiveTurn();
			enemy.GenerateCombatStartDeck();
		}
		foreach (CharacterGraphic merc in mercGraphics)
		{
			merc.SetStartStamina();
			merc.SetStartAmmo();
			merc.GiveTurn();
			merc.GenerateCombatStartDeck();
		}
	}

	public void DoMissionEndCleanup()
	{
		RemoveAllMercs();
		RemoveAllEnemies();
	}


	public void DoCharacterDeathCleanup(CharacterGraphic killedCharGraphic)
	{
		killedCharGraphic.DoCleanupAfterCharacterDeath();

		if (killedCharGraphic.GetCharacterType() == typeof(PartyMember))
			RemoveMercenary(killedCharGraphic);
		else
			RemoveEnemy(killedCharGraphic);
	}


	public void AddMercenary(Mercenary newMerc)
	{
		MercGraphic newCharGraphic = Instantiate(mercGraphicPrefab);
		newCharGraphic.AssignCharacter(newMerc);
		newCharGraphic.transform.SetParent(mercsGroup, false);
		mercGraphics.Add(newCharGraphic);
		newCharGraphic.SetStartArmor();
	}

	public void RemoveMercenary(CharacterGraphic removedMerc)
	{
		if (mercGraphics.Contains(removedMerc))
		{
			mercGraphics.Remove(removedMerc);
			GameObject.Destroy(removedMerc.gameObject);
		}
	}

	void RemoveAllMercs()
	{
		foreach (CharacterGraphic graphic in new List<CharacterGraphic>(mercGraphics))
			RemoveMercenary(graphic);
	}

	public CharacterGraphic GetFirstMerc()
	{
		if (mercGraphics.Count == 0)
			throw new System.Exception("Trying to get first merc when no mercs are present!");

		return mercGraphics[0];
	}

	public List<CharacterGraphic> GetMercGraphics()
	{
		return mercGraphics;
	}

	public int GetMercCount()
	{
		return mercGraphics.Count;
	}


	public void AddEnemy(Character newEnemy)
	{
		EnemyGraphic newCharGraphic = Instantiate(enemyGraphicPrefab);
		newCharGraphic.AssignCharacter(newEnemy);
		newCharGraphic.transform.SetParent(enemiesGroup, false);
		enemyGraphics.Add(newCharGraphic);

		newCharGraphic.SetStartArmor();
	}

	public void RemoveEnemy(CharacterGraphic removedEnemy)
	{
		if (enemyGraphics.Contains(removedEnemy))
		{
			enemyGraphics.Remove(removedEnemy);
			GameObject.Destroy(removedEnemy.gameObject);
		}
	}

	void RemoveAllEnemies()
	{
		foreach (CharacterGraphic graphic in new List<CharacterGraphic>(enemyGraphics))
		{
			RemoveEnemy(graphic);
		}
	}

	public CharacterGraphic GetFirstEnemy()
	{
		if (enemyGraphics.Count == 0)
			throw new System.Exception("Trying to get first enemy when no enemies are present!");

		return enemyGraphics[0];
	}

	public List<CharacterGraphic> GetEnemyGraphics()
	{
		return enemyGraphics;
	}
	public int GetEnemyCount()
	{
		return enemyGraphics.Count;
	}


	public bool RaycastForOtherFriendlyCharacter(CharacterGraphic sourceCharacter, out CharacterGraphic foundChar)
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

	public bool RaycastForCharacter(bool friendly, out CharacterGraphic foundChar)
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

	public void SetMercPortraitsEnabled(bool enabled)
	{
		foreach (CharacterGraphic graphic in mercGraphics)
		{
			MercGraphic mercGraphic = graphic as MercGraphic;
			mercGraphic.SetPortraitClickable(enabled);
		}
	}

	public void CleanupCharactersWhoDied()
	{
		List<CharacterGraphic> charactersInScene = new List<CharacterGraphic>();
		charactersInScene.AddRange(mercGraphics);
		charactersInScene.AddRange(enemyGraphics);
		foreach (CharacterGraphic character in charactersInScene)
		{
			if (character.GetHealth() <= 0)
				combatManager.CharacterKilled(character);
		}
	}


	public void HighlightSelectedPlayersCharacter(CharacterGraphic selectedCharacter)
	{
		//Set anchor to bottom left corner
		selectionArrow.GetComponent<RectTransform>().anchorMin = Vector2.zero;
		selectionArrow.GetComponent<RectTransform>().anchorMax = Vector2.zero;
		selectionArrow.SetParent(selectedCharacter.transform, false);
		selectionArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, 20f);
	}

	public void HighlightSelectedEnemyCharacter(CharacterGraphic selectedCharacter)
	{
		//Set anchor to upper left corner
		selectionArrow.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1f);
		selectionArrow.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1f);
		selectionArrow.SetParent(selectedCharacter.transform, false);
		selectionArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, -20f);
	}

	public void GiveAllChractersTurns()
	{
		GiveAllMercsTurns();
		GiveAllEnemiesTurns();
	}

	public void GiveAllMercsTurns()
	{
		foreach (CharacterGraphic merc in mercGraphics)
		{
			merc.GiveTurn();
		}
	}
	public void GiveAllEnemiesTurns()
	{
		foreach (CharacterGraphic enemy in enemyGraphics)
		{
			enemy.GiveTurn();
		}
	}

	public void RemoveAllMercTurns()
	{
		foreach (CharacterGraphic merc in mercGraphics)
		{
			merc.RemoveTurn();
		}
	}

	public void DoRoundStaminaRegen()
	{
		IncrementAllCharactersResource(CharacterGraphic.Resource.Stamina, staminaRegen);
	}

	public void ResetAllMercsResource(CharacterGraphic.Resource resource)
	{
		ResetResourceForGroup(resource, mercGraphics);
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
		if (combatManager.GetTurnStatus() == CombatManager.TurnStatus.Player)
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


	public void ResetSelectionArrow()
	{
		selectionArrow.SetParent(this.transform, false);
	}

}
