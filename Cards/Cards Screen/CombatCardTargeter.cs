using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CombatCardTargeter : MonoBehaviour {

	public static CombatCardTargeter main;

	public delegate void SetMeleeTargetMercDeleg();
	public static event SetMeleeTargetMercDeleg ENewMeleeTargetMercSet;
	public delegate void SetMeleeTargetEnemyDeleg();
	public static event SetMeleeTargetEnemyDeleg ENewMeleeTargetEnemySet;
	public delegate void SetRangedTargetMercDeleg();
	public static event SetRangedTargetMercDeleg ENewRangedTargetMercSet;
	public delegate void SetRangedTargetEnemyDeleg();
	public static event SetRangedTargetEnemyDeleg ENewRangedTargetEnemySet;

	//public PlayerHandManager playerHandManager;

	MissionCharacterManager characterManager;
	ModeTextDisplayer modeTextDisplayer;
	CombatManager combatManager;
	MissionUIToggler uiToggler;
	

	CharacterGraphic meleeCardTargetOverrideMerc = null;
	CharacterGraphic meleeCardTargetOverrideEnemy = null;
	CharacterGraphic rangedCardTargetOverrideMerc = null;
	CharacterGraphic rangedCardTargetOverrideEnemy = null;

	CharacterGraphic selectedCharacter;

	public void EnableCombatCardTargeter(CombatManager cardsScreen, MissionCharacterManager characterManager
		, MissionUIToggler uiToggler, ModeTextDisplayer modeTextDisplayer)
	{
		this.combatManager = cardsScreen;
		this.characterManager = characterManager;
		this.modeTextDisplayer = modeTextDisplayer;
		this.uiToggler = uiToggler;
		main = this;
	}

	public void CombatCardPlayStarted(ICombatCard cardObject)
	{
		//CombatCard playedCard = cardObject.GetAssignedCard();
		//playedCard.SetUserChar(cardPlayer);
		//PlayerCardTargetAssignment(cardObject, cardPlayer);
		StartCoroutine(SelectCardUserByPlayer(cardObject));
	}

	IEnumerator SelectCardUserByPlayer(ICombatCard playedCardGraphic)
	{
		CombatCard playedCard = playedCardGraphic.GetAssignedCard();

		CharacterGraphic cardUserChar = null;
		characterManager.SetMercPortraitsEnabled(false);
		string centerMessageText = "Select character";

		uiToggler.DisablePlayerActionsDuringCardPlay();
		modeTextDisplayer.DisplayCenterMessage(centerMessageText);

		bool characterFound = false;
		while (true)
		{
			if (Input.GetMouseButton(1))
			{
				uiToggler.EnableTurnoverButton();
				uiToggler.ReenablePlayerActions();
				break;
			}
			if (Input.GetMouseButton(0))
			{
				if (characterManager.RaycastForCharacter(true, out cardUserChar))
				{
					if (cardUserChar.CharacterMeetsCardRequirements(playedCard))
					{
						characterFound = true;
						break;
					}
				}
			}
			yield return new WaitForFixedUpdate();
		}

		characterManager.SetMercPortraitsEnabled(true);
		modeTextDisplayer.HideCenterMessage();

		if (characterFound)
		{
			while (Input.GetMouseButton(0))
				yield return new WaitForFixedUpdate();
			//playerHandManager.RemoveCardFromHand(playedCard);
			playedCard.SetUserChar(cardUserChar);
			PlayerCardTargetAssignment(playedCardGraphic,cardUserChar);
		}
	}

	void PlayerCardTargetAssignment(ICombatCard cardObject,CharacterGraphic selectedCharacter)
	{
		CombatCard playedCard = cardObject.GetAssignedCard();
		this.selectedCharacter = selectedCharacter;

		if (playedCard.targetType == CombatCard.TargetType.SelectFriendly | playedCard.targetType == CombatCard.TargetType.SelectFriendlyOther)
		{
			StartCoroutine("SelectCardTargetCharacterByPlayer", cardObject);
			return;
		}

		if (playedCard.targetType == CombatCard.TargetType.SelectEnemy)
		{
			if (playedCard.GetType().BaseType == typeof(MeleeCard) && meleeCardTargetOverrideEnemy != null)
			{
				AssignCardTargets(playedCard, selectedCharacter);
				combatManager.CombatCardPlayStarted(cardObject);
				return;
			}
			if (playedCard.GetType().BaseType == typeof(RangedCard) && rangedCardTargetOverrideEnemy != null)
			{
				AssignCardTargets(playedCard, selectedCharacter);
				combatManager.CombatCardPlayStarted(cardObject);
				return;
			}

			StartCoroutine("SelectCardTargetCharacterByPlayer", cardObject);
			return;
		}
		if (playedCard.targetType != CombatCard.TargetType.SelectFriendly
			&& playedCard.targetType != CombatCard.TargetType.SelectFriendlyOther
			&& playedCard.targetType != CombatCard.TargetType.SelectEnemy)
		{
			AssignCardTargets(playedCard, selectedCharacter);
			combatManager.CombatCardPlayStarted(cardObject);
		}
	}

	IEnumerator SelectCardTargetCharacterByPlayer(ICombatCard playedCardObject)
	{
		CombatCard playedCard = playedCardObject.GetAssignedCard();
		
		CharacterGraphic targetChar = null;
		bool selectFriendly = false;
		bool selectOtherFriendly = false;
		string centerMessageText = "Select enemy";
		if (playedCard.targetType == CombatCard.TargetType.SelectEnemy)
		{
			selectFriendly = false;
			centerMessageText = "Select enemy";
		}
		if (playedCard.targetType == CombatCard.TargetType.SelectFriendly)
		{
			selectFriendly = true;
			characterManager.SetMercPortraitsEnabled(false);
			centerMessageText = "Select friendly";
		}
		if (playedCard.targetType == CombatCard.TargetType.SelectFriendlyOther)
		{
			selectFriendly = true;
			selectOtherFriendly = true;
			characterManager.SetMercPortraitsEnabled(false);
			centerMessageText = "Select friendly";
		}

		uiToggler.DisablePlayerActionsDuringCardPlay();
		modeTextDisplayer.DisplayCenterMessage(centerMessageText);

		bool targetFound = false;

		while (true)
		{
			if (Input.GetMouseButton(1))
			{
				uiToggler.EnableTurnoverButton();
				uiToggler.ReenablePlayerActions();
				break;
			}
			if (Input.GetMouseButton(0))
			{
				if (selectOtherFriendly)
				{
					if (characterManager.RaycastForOtherFriendlyCharacter(selectedCharacter, out targetChar))
					{
						targetFound = true;
						break;
					}
				}
				else
					if (characterManager.RaycastForCharacter(selectFriendly, out targetChar))
					{
						targetFound = true;
						break;
					}
			}
			yield return new WaitForFixedUpdate();
		}

		if (selectFriendly)
			characterManager.SetMercPortraitsEnabled(true);
		modeTextDisplayer.HideCenterMessage();
		if (targetFound)
		{
			playedCard.targetChars.Add(targetChar);
			combatManager.CombatCardPlayStarted(playedCardObject);
		}
	}

	public void AssignCardTargets(CombatCard playedCard, CharacterGraphic selectedCharacter)
	{
		List<CharacterGraphic> opposingGroup;
		List<CharacterGraphic> friendlyGroup;
		if (combatManager.GetTurnStatus() == CombatManager.TurnStatus.Player)
		{
			opposingGroup = characterManager.GetEnemyGraphics();
			friendlyGroup = characterManager.GetMercGraphics();
		}
		else
		{
			opposingGroup = characterManager.GetMercGraphics();
			friendlyGroup = characterManager.GetEnemyGraphics();
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
			playedCard.targetChars.Add(opposingGroup[Random.Range(0, opposingGroup.Count)]);
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
			if (combatManager.GetTurnStatus() == CombatManager.TurnStatus.Enemy)
			{
				if (rangedCardTargetOverrideMerc != null)
				{
					playedRangedCard.targetChars.Clear();
					playedRangedCard.targetChars.Add(rangedCardTargetOverrideMerc);
				}
			}
			if (combatManager.GetTurnStatus() == CombatManager.TurnStatus.Player)
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
			if (combatManager.GetTurnStatus() == CombatManager.TurnStatus.Enemy)
			{
				if (meleeCardTargetOverrideMerc != null)
				{
					playedMeleeCard.targetChars.Clear();
					playedMeleeCard.targetChars.Add(meleeCardTargetOverrideMerc);
				}
			}
			if (combatManager.GetTurnStatus() == CombatManager.TurnStatus.Player)
			{
				if (meleeCardTargetOverrideEnemy != null)
				{
					playedMeleeCard.targetChars.Clear();
					playedMeleeCard.targetChars.Add(meleeCardTargetOverrideEnemy);
				}
			}
		}
	}

	public void SetNewMeleeTargetMerc(CharacterGraphic newTargetMerc)
	{
		if (!characterManager.GetMercGraphics().Contains(newTargetMerc))
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

	public bool HasMeleeTargetMerc()
	{
		return meleeCardTargetOverrideMerc != null;
	}

	public void SetNewMeleeTargetEnemy(CharacterGraphic newTargetEnemy)
	{
		if (!characterManager.GetEnemyGraphics().Contains(newTargetEnemy))
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

	public bool HasMeleeTargetEnemy()
	{
		return meleeCardTargetOverrideEnemy != null;
	}

	public void SetNewRangedTargetEnemy(CharacterGraphic newTargetEnemy)
	{
		if (!characterManager.GetEnemyGraphics().Contains(newTargetEnemy))
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
		if (!characterManager.GetMercGraphics().Contains(newTargetMerc))
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
}
