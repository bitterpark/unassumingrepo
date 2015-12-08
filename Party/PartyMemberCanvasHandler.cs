using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PartyMemberCanvasHandler : MonoBehaviour {

	PartyMember assignedMember;
	public Text nameText;
	public Text healthText;
	public Text staminaText;
	public Text hungerText;
	public Text moraleText;
	public Text fatigueText;
	public Button memberSelector;
	Dictionary<StatusEffect,StatusEffectImageHandler> statusEffectTokens=new Dictionary<StatusEffect, StatusEffectImageHandler>();
	public StatusEffectImageHandler tokenPrefab;
	
	public void AssignPartyMember(PartyMember assigned) {assignedMember=assigned;}
	
	public void AddStatusEffectToken(StatusEffect effect)
	{
		StatusEffectImageHandler newStatusToken=Instantiate(tokenPrefab) as StatusEffectImageHandler;
		newStatusToken.transform.SetParent(transform.FindChild("Status Tokens Group"),false);
		newStatusToken.AssignStatusEffect(effect);
		statusEffectTokens.Add(effect,newStatusToken);
	}
	public void RemoveStatusEffectToken(StatusEffect effect)
	{
		StatusEffectImageHandler removedToken=statusEffectTokens[effect];
		statusEffectTokens.Remove(effect);
		GameObject.Destroy(removedToken.gameObject);
	}
	
	
	void Start() 
	{
		memberSelector.GetComponent<Image>().color=assignedMember.color;
		memberSelector.onClick.AddListener(()=>InventoryScreenHandler.mainISHandler.AssignSelectedMember(assignedMember));
	}//PartyScreenManager.mainPSManager.MemberClicked(assignedMember));}
	
	void Update()
	{
		nameText.text=""+assignedMember.name;
		healthText.text="Health:"+assignedMember.health;
		staminaText.text="Stamina:"+assignedMember.stamina;
		hungerText.text="Hunger:"+assignedMember.GetHunger();
		moraleText.text="Morale:"+assignedMember.morale;
		fatigueText.text="Fatigue:"+assignedMember.GetFatigue();//
	}
}
