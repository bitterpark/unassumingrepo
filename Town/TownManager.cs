using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TownManager : MonoBehaviour {

	public static TownManager main;

	public int startingMoney;
	public int money;
	public int startingCrew=10;
	int crew;
	const int noMoneyCrewPenalty = 10;

	List<PartyMember> mercenaryHireList = new List<PartyMember>();
	List<PartyMember> previousHiredMercsPool = new List<PartyMember>();
	int hirableMercsPoolSize = 5;
	public const int dailyPayPerCrew = 10;

	public List<TownBuilding> buildings;

	public List<InventoryItem> itemsOnSale;

	bool enginesBuilt=false;
	bool marketUnlocked=false;

	int dailyMercHealRate = 0;

	float marketPriceMult = 1;

	public void NewGameState()
	{
		LockMarket();
		dailyMercHealRate = 0;
		enginesBuilt = false;
		buildings = new List<TownBuilding>();
		SetMoney(startingMoney);
		SetCrew(startingCrew);
		previousHiredMercsPool.Clear();
		UpdateMercenaryHireList();
		buildings.Add(new Habitation());
		buildings.Add(new CommCenter());
		buildings.Add(new CargoBay());
		buildings.Add(new SickBay());

		itemsOnSale = new List<InventoryItem>();
		itemsOnSale.Add(new Scrap());
		itemsOnSale.Add(new Scrap());
		itemsOnSale.Add(new Scrap());
		itemsOnSale.Add(new Scrap());
		itemsOnSale.Add(new Scrap());
		itemsOnSale.Add(new Scrap());
		itemsOnSale.Add(new ComputerParts());
		itemsOnSale.Add(new ComputerParts());
		itemsOnSale.Add(new ComputerParts());
		itemsOnSale.Add(new ComputerParts());
		itemsOnSale.Add(new ComputerParts());
		itemsOnSale.Add(new ComputerParts());
	}

	public virtual void UpdateMercenaryHireList()
	{
		int previouslyHiredMercsCount = Random.Range(1, Mathf.Min(3,hirableMercsPoolSize));
		for (int i = previouslyHiredMercsCount; i < previouslyHiredMercsCount; i++)
		{
			if (previousHiredMercsPool.Count == 0) break;
			PartyMember randomlyPickedMerc=previousHiredMercsPool[Random.Range(0,previousHiredMercsPool.Count)];
			mercenaryHireList.Add(randomlyPickedMerc);
			previousHiredMercsPool.Remove(randomlyPickedMerc);
		}
		
		while (mercenaryHireList.Count < hirableMercsPoolSize)
		{
			mercenaryHireList.Add(new PartyMember());
		}
	}

	public List<PartyMember> GetHireableMercenariesList()
	{
		return mercenaryHireList;
	}

	public void MercenaryHired(PartyMember hiredMerc)
	{
		mercenaryHireList.Remove(hiredMerc);
	}
	public void MercenaryContractFinished(PartyMember finishedMerc)
	{
		previousHiredMercsPool.Add(finishedMerc);
	}

	public void IncrementHireableMercsPoolSize(int delta)
	{
		hirableMercsPoolSize += delta;	
	}

	public void IncrementCrew(int delta)
	{
		SetCrew(crew + delta);
	}
	public void SetCrew(int newCrewCount)
	{
		crew = newCrewCount;
		if (crew <= 0)
			GameManager.main.EndCurrentGame(false);
		else
			BuildingsCrewUpdate();
	}
	public int GetCrew() { return crew; }

	public virtual void BuildingsCrewUpdate()
	{
		foreach (TownBuilding building in buildings)
		{
			building.UpdateActiveStatus();
		}
	}

	public void SetMoney(int newMoneyBalance)
	{
		money = newMoneyBalance;
	}

	public void DailyCashBalanceChange()
	{
		if (money < 0)
			IncrementCrew(-noMoneyCrewPenalty);
		money = GetDailyBalance();
	}

	public int GetDailyBalance()
	{
		return money - CalculateCrewExpenses();
	}

	public int CalculateCrewExpenses()
	{
		return crew * dailyPayPerCrew;
	}

	public void IncrementDailyMercHealRate(int delta)
	{
		SetDailyMercHealRate(dailyMercHealRate + delta);
	}

	public void SetDailyMercHealRate(int newHealRate)
	{
		dailyMercHealRate = newHealRate;
		if (dailyMercHealRate < 0)
			dailyMercHealRate = 0;
	}

	public int GetDailyMercHealRate()
	{
		return dailyMercHealRate;
	}

	public void IncrementMarketPriceMult(float delta)
	{
		marketPriceMult += delta;
	}
	public float GetMarketPriceMult()
	{
		return marketPriceMult;
	}

	public void UnlockMarket()
	{
		marketUnlocked = true;
		TownScreen.main.UnlockMarketTab();
	}

	public void LockMarket()
	{
		marketUnlocked = false;
		TownScreen.main.LockMarketTab();
	}

	public void EnginesBuilt()
	{
		//barBuilt=true;
		if (WinBuildingsAreBuilt())
			GameManager.main.EndCurrentGame(true);
	}

	bool WinBuildingsAreBuilt()
	{
		if (enginesBuilt) return true;
		else return false;
	}
	

	// Use this for initialization
	void Start () 
	{
		main = this;
		PartyManager.ETimePassed += DailyCashBalanceChange;
	}
}
