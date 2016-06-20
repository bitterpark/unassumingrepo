using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingsTab : MonoBehaviour,TownTab {


	public BuildingInfo buildingInfoPrefab;
	public Transform buildingsGroup;


	public void OpenTab()
	{
		gameObject.SetActive(true);
		transform.SetAsLastSibling();
		RefreshBuildingList();
		BuildingBuildButton.EBuildingBuilt += RefreshBuildingList;
	}

	void RefreshBuildingList()
	{
		CleanupOldBuildingList();
		foreach (TownBuilding building in TownManager.main.buildings)
		{
			BuildingInfo newBuildingInfo = Instantiate(buildingInfoPrefab);
			newBuildingInfo.AssignBuilding(building);
			newBuildingInfo.transform.SetParent(buildingsGroup,false);
		}
	}

	void CleanupOldBuildingList()
	{
		foreach (BuildingInfo oldBuilding in buildingsGroup.GetComponentsInChildren<BuildingInfo>())//Button>())
		{
			GameObject.Destroy(oldBuilding.gameObject);
		}
	}


	public void CloseTab()
	{
		CleanupOldBuildingList();
		BuildingBuildButton.EBuildingBuilt -= RefreshBuildingList;
		gameObject.SetActive(false);
	}
}
