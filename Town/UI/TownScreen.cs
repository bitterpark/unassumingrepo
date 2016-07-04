using UnityEngine;
using System.Collections;
using UnityEngine.UI;

interface TownTab
{
	void CloseTab();
}

public class TownScreen : MonoBehaviour {

	public static TownScreen main;

	public Button financeButton;
	public Button storageButton;
	public Button mapButton;
	public Button workshopsButton;
	public Button mercenariesButton;
	public Button buildingsButton;
	public Button marketButton;

	public Transform tabContentGroup;

	public FinancesTab financesTab;
	public StorageTab storageTab;
	public WorkshopTab workshopTab;
	public MercenariesTab mercenariesTab;
	public BuildingsTab buildingsTab;
	public MarketTab marketTab;

	TownTab currentTab = null;

	void Start () 
	{
		main = this;

		GameManager.GameOver += CloseTownScreen;

		financeButton.onClick.AddListener(() => OpenFinancesTab());
		storageButton.onClick.AddListener(() => OpenStorageTab());
		buildingsButton.onClick.AddListener(() => { OpenBuildingsTab(); });
		workshopsButton.onClick.AddListener(() => { OpenWorkshopsTab();});
		marketButton.onClick.AddListener(() => { OpenMarketTab(); });
		mercenariesButton.onClick.AddListener(()=>{OpenMercenariesTab();});
		mapButton.onClick.AddListener(() => { ReturnToMap();});

		gameObject.SetActive(false);
	}

	public void UnlockMarketTab()
	{
		marketButton.gameObject.SetActive(true);
	}

	public void LockMarketTab()
	{
		marketButton.gameObject.SetActive(false);
	}

	public void UnlockWorkshopsTab()
	{
		workshopsButton.gameObject.SetActive(true);
	}

	public void LockWorkshopsTab()
	{
		workshopsButton.gameObject.SetActive(false);
	}

	public void OpenTownScreen()
	{
		gameObject.SetActive(true);
		OpenDefaultTab();
	}

	void OpenDefaultTab()
	{
		OpenFinancesTab();
	}

	void OpenFinancesTab()
	{
		CleanupCurrentTab();
		currentTab = financesTab;
		financesTab.OpenTab();
	}

	void OpenStorageTab()
	{
		CleanupCurrentTab();
		currentTab = storageTab;
		storageTab.OpenTab();
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
