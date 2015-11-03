using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MemberTokenHandler : MonoBehaviour 
{
	public PartyMember myMember;
	public GameObject mySelectedArrow;
	public Image myImage;
	public AttackModeToggler myToggler;
	//public GameObject rangedModeIcon;
	//public GameObject meleeModeIcon;
	public Text healthText;
	public Text nameText;
	public Text staminaText;
	
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
	
	public bool actionTaken
	{
		get {return _actionTaken;}
		set 
		{
			_actionTaken=value; 
			if (_actionTaken) {Deselect();}
			DetermineColor();
		}
	}
	bool _actionTaken=false;
	
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
	}
	
	public void Clicked()
	{
		if (!actionTaken) EncounterCanvasHandler.main.ClickMember(this);
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
		if (!actionTaken)
		{
			myImage.GetComponent<Button>().interactable=true;
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
			myImage.GetComponent<Button>().interactable=false; 
			mySelectedArrow.SetActive(false);
		}//GetComponent<Button>().image.color=Color.gray;}
	}
	
	public void UpdateTokenPos(RoomButtonHandler newRoomHandler)
	{
		newRoomHandler.AttachMemberToken(transform);
	}
	
	public void AnimateAttack() 
	{
		StartCoroutine(AttackAnimation());
	}
	
	IEnumerator AttackAnimation()
	{
		/*
		foreach (Text child in GetComponentsInChildren<Text>())
		{
			child.transform.
		}*/
		myImage.transform.localScale=new Vector3(1.6f,1.6f,1f);
		yield return new WaitForSeconds(0.6f);
		myImage.transform.localScale=new Vector3(1f,1f,1f);
		yield break;
	}
	
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
		TooltipManager.main.CreateTooltip(text,transform);
	}
	
	public void MouseLeave() {TooltipManager.main.StopAllTooltips();}
	
	void Update()
	{
		healthText.text=myMember.health.ToString();
		staminaText.text=myMember.stamina.ToString();
		//if (Input.GetMouseButtonDown(1)) {ToggleRangedMode();}
	}
}
