using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AttackLocationSelector : MonoBehaviour 
{
	static AttackLocationSelector currentSelector;
	public static void DestroySelectors()
	{
		if (currentSelector!=null) GameObject.Destroy(currentSelector.gameObject);
	}
	
	public Text selectButtonBasePrefab;
	
	public void EnableNewAttackSelector(EnemyTokenHandler enemyToken, bool ranged)
	{
		transform.SetParent(enemyToken.transform,false);
		GetComponent<Canvas>().overrideSorting=true;
		GetComponent<Canvas>().sortingOrder=10;
		//Create attack selection buttons
		foreach (EnemyBodyPart part in enemyToken.assignedEnemy.body.GetHealthyParts())
		{
			//Adjust hit chance for enemy bodypart modifier, and clamp between 0 and 1
			PartyMember attackingMember=EncounterCanvasHandler.main.selectedMember;
			float adjustedHitChance=Mathf.Clamp
			(attackingMember.GetCurrentAttackHitChance(EncounterCanvasHandler.main.memberTokens[attackingMember].rangedMode)
			+part.currentHitchanceShare-enemyToken.assignedEnemy.body.dodgeChance,0,1f);
			Text newButtonBase=Instantiate(selectButtonBasePrefab);
			foreach (Text buttonText in newButtonBase.GetComponentsInChildren<Text>())
			{
				buttonText.text=part.name+"("+part.hp+")- "+Mathf.RoundToInt((adjustedHitChance*100f)).ToString()+"%";
			}
			//newButtonBase.text=
			//newButtonBase.GetComponentInChildren<Text>().text=newButtonBase.text;
			newButtonBase.transform.SetParent(transform,false);
			EnemyBodyPart selectorPart=part;
			newButtonBase.GetComponentInChildren<Button>().onClick.AddListener(()=>
			{
				EncounterCanvasHandler.main.AttackOnEnemy(enemyToken.assignedEnemy,ranged,selectorPart,adjustedHitChance);
				DestroySelectors();
			});
		}
		DestroySelectors();
		currentSelector=this;
	}
	
	void Update()
	{
		if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Space)) DestroySelectors();
	}
}
