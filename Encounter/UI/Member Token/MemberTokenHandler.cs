using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface IAttackAnimation
{
	IEnumerator AttackAnimation(IGotHitAnimation targetAnimation);
}

public interface IGotHitAnimation
{
	IEnumerator GotHitAnimation();
}

public class MemberTokenHandler : MonoBehaviour, IAttackAnimation, IGotHitAnimation 
{
	//const int defenseModifier=3;
	
	static float attackAnimationTime=0.6f;
	static float gotHitAnimationTime=0.6f;
	
	public PartyMember myMember;
	public GameObject mySelectedArrow;
	public Image myImage;
	public AttackModeToggler myToggler;
	//public GameObject rangedModeIcon;
	//public GameObject meleeModeIcon;
	public Text healthText;
	public Text nameText;
	public Text staminaText;
	
	public ActionToken myActionToken;
	public MoveToken myMoveToken;
	public DefenceToken myDefenceToken;
	public StaminaRegenToken myStaminaRegenToken;
	
	public bool selected
	{
		get {return _selected;}
		set 
		{
			_selected=value;
			DetermineColor();
		}
	}	
	bool _selected=false;
	
	int currentAllowedMovesCount;
	int maxAllowedMovesCount;
	void RefreshMaxAllowedMovesCount()
	{
		maxAllowedMovesCount=2;
		if (myMember.legsBroken) maxAllowedMovesCount-=1;
		else if (myMember.isScout) maxAllowedMovesCount+=1;
		if (currentAllowedMovesCount>maxAllowedMovesCount) currentAllowedMovesCount=maxAllowedMovesCount;
	}
	bool moveTaken
	{
		get {return _moveTaken;}
		set 
		{
			_moveTaken=value;
			myMoveToken.MoveStatusChanged(_moveTaken,turnTaken,myMember.stamina>=myMember.staminaMoveCost);
			//myDefenceToken.DefenceStatusChanged(_moveTaken,turnTaken);
		}	
	}
	 
	bool _moveTaken=false;
	
	public bool turnTaken
	{
		get {return _turnTaken;}
		set 
		{
			_turnTaken=value; 
			if (_turnTaken) {Deselect();}
			myActionToken.ActionStatusChanged(_turnTaken);
			//myDefenceToken.DefenceStatusChanged(moveTaken,_turnTaken);
			myMoveToken.MoveStatusChanged(moveTaken,_turnTaken,myMember.stamina>=myMember.staminaMoveCost);
			DetermineColor();
		}
	}
	bool _turnTaken=false;
	//public bool defenceMode=false;
	/*
	public bool defenceMode
	{
		get{return _defenceMode;}
		set
		{
			_defenceMode=value;
			print ("Defence mode set to:"+_defenceMode);
			myDefenceToken.DefenceChanged(_defenceMode);
		}
	}
	bool _defenceMode=false;*/
	bool staminaRegenEnabled
	{
		get {return _staminaRegenEnabled;}
		set
		{
			_staminaRegenEnabled=value;
			myStaminaRegenToken.StaminaRegenStatusChanged(_staminaRegenEnabled);
		}
	}
	
	bool _staminaRegenEnabled=false;
	
	
	public bool rangedMode
	{
		get {return _rangedMode;}
		set 
		{
			_rangedMode=value;
			myToggler.SetMode(_rangedMode);
			/*
			if (_rangedMode) 
			{
				meleeModeIcon.SetActive(false);
				rangedModeIcon.SetActive(true);
			}
			else
			{
				meleeModeIcon.SetActive(true);
				rangedModeIcon.SetActive(false);
			}*/
		}
	}
	bool _rangedMode;
	
	public void AssignMember(PartyMember member)
	{
		myMember=member;
		nameText.text=member.name;
		myImage.color=member.color;
		RefreshMaxAllowedMovesCount();
		currentAllowedMovesCount=maxAllowedMovesCount;
		//myDefenceToken.DefenceStatusChanged(moveTaken,turnTaken);
		myActionToken.ActionStatusChanged(turnTaken);
		myMoveToken.MoveStatusChanged(moveTaken,turnTaken,myMember.stamina>=myMember.staminaMoveCost);
		myStaminaRegenToken.StaminaRegenStatusChanged(staminaRegenEnabled);
	}
	
	public bool TryHitAssignedMember(int damage, out int realDamage, out int staminaDamage, out PartyMember.BodyPartTypes hitPart)
	{
		//int extraArmorMod=0;
		//if (defenceMode) {extraArmorMod=damage;}
		/*
		if (myMember.stamina>=staminaDamage) 
		{
			extraArmorMod=damage; 
			myMember.stamina-=staminaDamage;
		}*/
		staminaDamage=0;//else staminaDamage=0;
		realDamage=0;
		//staminaRegenEnabled=false;
		//int realDmg=myMember.TakeDamage(damage,extraArmorMod,true);
		//return realDmg;
		bool hitConnected=myMember.TryTakeDamage(damage,ref realDamage, out hitPart);
		return hitConnected;
	}
	
	public void FinishTurn(bool turnSkipped)
	{
		//if (turnSkipped && !moveTaken) 
		//{
			//defenceMode=true;
		//}
		if (!turnSkipped) {staminaRegenEnabled=false;}
		turnTaken=true;
	}
	
	public void NextTurn()
	{
		int staminaRegenAmount=myMember.staminaRegen;
		if (staminaRegenEnabled) myMember.stamina+=staminaRegenAmount;
		moveTaken=false;
		turnTaken=false;
		RefreshMaxAllowedMovesCount();
		currentAllowedMovesCount=maxAllowedMovesCount;
		//defenceMode=false;
		//staminaRegenEnabled=true;
		//print ("next turn switched!");
	}
	/*
	public bool CanMove()
	{
		
	}*/
	
	public bool TryMove(out bool doTurnOver)
	{
		doTurnOver=false;
		//This is necessary incase member legs get crippled as he moves out with 2 moves left
		currentAllowedMovesCount-=1;
		RefreshMaxAllowedMovesCount();
		//Stamina cost is currently 0, deprecate this later
		int moveStaminaCost=myMember.staminaMoveCost;
		if (myMember.stamina>=moveStaminaCost) 
		{
			myMember.stamina-=moveStaminaCost;
			staminaRegenEnabled=false;
			if (currentAllowedMovesCount<=0) 
			{
				doTurnOver=true;
			}
			if (currentAllowedMovesCount<=1) moveTaken=true;
			return true;
		}
		else return false;
	}
	
	public void Clicked()
	{
		if (!turnTaken) EncounterCanvasHandler.main.ClickMember(this);
	}
	
	public void Select()
	{
		selected=true;
	}
	
	public void Deselect()
	{
		selected=false;
		rangedMode=false;
	}
	
	void DetermineColor()
	{
		if (!turnTaken)
		{
			//myImage.GetComponent<Button>().interactable=true;
			if (_selected) 
			{
				
				//myImage.GetComponent<Button>().image.color=Color.blue;
				mySelectedArrow.SetActive(true);
			}
			else 
			{
				myImage.GetComponent<Button>().image.color=myMember.color;
				mySelectedArrow.SetActive(false);
			}
		}
		else 
		{
			//myImage.GetComponent<Button>().interactable=false; 
			mySelectedArrow.SetActive(false);
		}//GetComponent<Button>().image.color=Color.gray;}
	}
	
	public void UpdateTokenPos(RoomButtonHandler newRoomHandler)
	{
		newRoomHandler.AttachMemberToken(transform);
	}
	
	//Check to see if this member can attack a specific enemy in encounter
	public bool AttackIsPossible(ref bool isRanged, EncounterEnemy targetEnemy)
	{
		bool enemyReachable=false;
		
		//if (!moveTaken)
		{
			isRanged=rangedMode;
			int attackRange;
			if (isRanged) {attackRange=100;}
			else {attackRange=0;}
			
			int myX=(int)EncounterCanvasHandler.main.memberCoords[myMember].x;
			int myY=(int)EncounterCanvasHandler.main.memberCoords[myMember].y;
			int enemyX=(int)targetEnemy.GetCoords().x;
			int enemyY=(int)targetEnemy.GetCoords().y;
			
			
			//See if clicked enemy passes attack range check
			if (Mathf.Abs(myX-enemyX)+Mathf.Abs(myY-enemyY)
			    <=attackRange) 
			{
				enemyReachable=true;
				//Second - see if any walls are blocking member (for ranged attacks)
				if (attackRange>0)
				{
					
					BresenhamLines.Line(myX,myY,enemyX,enemyY,(int x, int y)=>
					                    {
						//bool visionClear=true;
						if (EncounterCanvasHandler.main.currentEncounter.encounterMap[new Vector2(x,y)].isWall) {enemyReachable=false;}
						return enemyReachable;
					});
				}
			}
		}
		return enemyReachable;
	}

	#region AttackAnimation implementation

	public IEnumerator AttackAnimation(IGotHitAnimation targetAnimation)
	{
		/*
		foreach (Text child in GetComponentsInChildren<Text>())
		{
			child.transform.
		}*/
		//print ("starting attack animation!");
		if (myImage!=null) myImage.transform.localScale=new Vector3(1.6f,1.6f,1f);
		else yield break;
		if (targetAnimation!=null) yield return StartCoroutine(targetAnimation.GotHitAnimation());
		else yield return new WaitForSeconds(attackAnimationTime);
		//print ("yielding attack animation!");
		if (myImage!=null) myImage.transform.localScale=new Vector3(1f,1f,1f);
		yield break;
	}

	#endregion

	#region IGotHitAnimation implementation

	public IEnumerator GotHitAnimation ()
	{
		if (myImage!=null) myImage.transform.localScale=new Vector3(0.8f,0.8f,1f);
		else yield break;
		yield return new WaitForSeconds(gotHitAnimationTime);
		//print ("yielding attack animation!");
		if (myImage!=null) myImage.transform.localScale=new Vector3(1f,1f,1f);
		yield break;
	}

	#endregion
	
	/*
	public void AnimateAttack() 
	{
		StartCoroutine(AttackAnimation());
	}
	
	IEnumerator DebugTest()
	{
		print ("test started, finish in 2 seconds");
		yield return new WaitForSeconds(2f);
		print ("test finished successfully");
		yield break;
	}
	*/
	
	
	public void ToggleRangedMode ()
	{
		if (rangedMode) {rangedMode=false;}
		else 
		{
			if (myMember.equippedRangedWeapon!=null && PartyManager.mainPartyManager.ammo>0) 
			{rangedMode=true;}
		}
	}
	
	public void MouseEnter()
	{
		string text=myMember.name;
		foreach (Perk perk in myMember.perks){text+="\n-"+perk.name;}
		foreach (PartyMember.BodyPartTypes part in myMember.memberBodyParts.currentParts.Keys)//.currentParts.Keys)
		{text+="\n"+part.ToString()+" "+(myMember.memberBodyParts.GetPartHitChance(part)*100).ToString()+"%";}
		TooltipManager.main.CreateTooltip(text,transform);
	}
	
	public void MouseLeave() {TooltipManager.main.StopAllTooltips();}
	
	void Update()
	{
		//healthText.text=myMember.health.ToString();
		healthText.text=myMember.memberBodyParts.currentParts[PartyMember.BodyPartTypes.Hands].health
		+"|"+myMember.memberBodyParts.currentParts[PartyMember.BodyPartTypes.Legs].health
		+"|"+myMember.memberBodyParts.currentParts[PartyMember.BodyPartTypes.Vitals].health;
		staminaText.text=myMember.stamina.ToString();
		//if (Input.GetMouseButtonDown(1)) {ToggleRangedMode();}
	}
}
