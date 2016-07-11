using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MercenariesTab : MonoBehaviour, TownTab {

	public MercenaryHireInfo mercInfoPrefab;
	public Transform mercGroup;

	public void OpenTab()
	{
		gameObject.SetActive(true);
		transform.SetAsLastSibling();
		RefreshMercList();
	}

	public void RefreshMercList()
	{
		CleanupOldMercList();
		//Refresh crafting recipes
		List<PartyMember> availableMercs=TownManager.main.GetHireableMercenariesList(); //= CraftRecipe.GetGenericRecipes(false);
		foreach (PartyMember merc in availableMercs)
		{
			MercenaryHireInfo newMercInfo = Instantiate(mercInfoPrefab);
			newMercInfo.transform.SetParent(mercGroup,false);
			newMercInfo.AssignMercenary(merc);
			newMercInfo.parentMercTab = this;
		}
	}

	void CleanupOldMercList()
	{
		foreach (MercenaryHireInfo oldMercInfo in mercGroup.GetComponentsInChildren<MercenaryHireInfo>())
		{ GameObject.Destroy(oldMercInfo.gameObject); }
	}

	public void CloseTab()
	{
		gameObject.SetActive(false);
		CleanupOldMercList();
	}
}
