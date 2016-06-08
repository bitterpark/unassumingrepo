using UnityEngine;
using System.Collections;
using UnityEngine.UI;

interface TownTab
{
	void CloseTab();
}

public class TownScreen : MonoBehaviour {

	public static TownScreen main;

	public Button mapButton;
	public Button workshopsButton;
	public Button mercenariesButton;
	public Button buildingsButton;
	public Button marketButton;

	public Transform tabContentGroup;

	public WorkshopTab workshopTab;
	public MercenariesTab mercenariesTab;
	public BuildingsTab buildingsTab;
	public MarketTab marketTab;

	TownTab currentTab = null;

	// Use this for initialization
	void Start () 
	{
		main = this;

		buildingsButton.onClick.AddListener(() => { OpenBuildingsTab(); });
		workshopsButton.onClick.AddListener(() => { OpenWorkshopsTab();});
		marketButton.onClick.AddListener(() => { OpenMarketTab(); });
		mercenariesButton.onClick.AddListener(()=>{OpenMercenariesTab();});
		mapButton.onClick.AddListener(() => { ReturnToMap();});

		gameObject.SetActive(false);
	}

	public void OpenTownScreen()
	{
		gameObject.SetActive(true);
	}

	void OpenBuildingsTab()
	{
		CleanupCurrentTab();
		currentTab = buildingsTab;
		buildingsTab.OpenTab();
	}

	void OpenWorkshopsTab()
	{
		CleanupCurrentTab();
		currentTab = workshopTab;
		workshopTab.OpenTab();
	}

	void OpenMarketTab()
	{
		CleanupCurrentTab();
		currentTab = marketTab;
		marketTab.OpenTab();
	}

	void OpenMercenariesTab()
	{
		CleanupCurrentTab();
		currentTab = mercenariesTab;
		mercenariesTab.OpenTab();
	}

	void ReturnToMap()
	{
		CloseTownScreen();
	}

	void CloseTownScreen()
	{
		CleanupCurrentTab();
		gameObject.SetActive(false);
	}

	void CleanupCurrentTab()
	{
		if (currentTab != null) currentTab.CloseTab();
		currentTab = null;
	}
}
