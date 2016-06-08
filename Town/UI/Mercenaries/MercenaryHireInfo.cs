using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class MercenaryHireInfo : MonoBehaviour {

	public Image mercPortrait;
	public Text mercNameText;
	public Text hireCostText;
	public TraitUIHandler traitPrefab;

	public Button hireButton;

	public Transform traitGroup;
	public Transform skillGroup;

	public MercenariesTab parentMercTab;

	PartyMember assignedMerc;
	const int hireCost = 300;
	const int hireDuration = 3;

	public void AssignMercenary(PartyMember merc)
	{
		assignedMerc = merc;
		mercPortrait.color = merc.color;
		mercNameText.text = merc.name;
		hireCostText.text = hireCost+" $";

		hireButton.onClick.AddListener(() => { HireButtonPressed();});
		hireButton.GetComponentInChildren<Text>().text="Hire ("+hireDuration+" days)";

		//Delete old traits
		foreach (Image oldTraitImage in traitGroup.GetComponentsInChildren<Image>())
		{
			GameObject.Destroy(oldTraitImage.gameObject);
		}
		//Delete old skills
		foreach (Image oldSkillImage in skillGroup.GetComponentsInChildren<Image>())
		{
			GameObject.Destroy(oldSkillImage.gameObject);
		}
		//Repopulate skills and traits	
		foreach (Trait memberTrait in assignedMerc.traits)
		{
			TraitUIHandler newPerkImage = Instantiate(traitPrefab);
			newPerkImage.AssignTrait(memberTrait);
			if (memberTrait.GetType().BaseType == typeof(Trait)) newPerkImage.transform.SetParent(traitGroup, false);
			else newPerkImage.transform.SetParent(skillGroup, false);
		}
	}

	void HireButtonPressed()
	{
		if (TownManager.main.money >= hireCost) 
		{
			TownManager.main.money -= hireCost;
			assignedMerc.hireDaysRemaining=hireDuration;
			PartyManager.mainPartyManager.AddNewPartyMember(assignedMerc);
			TownManager.main.MercenaryHired(assignedMerc);
			parentMercTab.RefreshMercList();
		}
	}
}
