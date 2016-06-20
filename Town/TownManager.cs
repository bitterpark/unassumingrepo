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
	const int requiredMercHireListSize = 5;
	public const int dailyPayPerCrew = 10;
	
	public void MercenaryHired(PartyMember hiredMerc) { mercenaryHireList.Remove(hiredMerc);}
	public void MercenaryContractFinished(PartyMember finishedMerc) { mercenaryHireList.Add(finishedMerc); }

	public List<TownBuilding> buildings;

	public List<InventoryItem> itemsOnSale;

	bool barBuilt;

	public void NewGameState()
	{
		buildings = new List<TownBuilding>();
		SetMoney(startingMoney);
		SetCrew(startingCrew);
		UpdateMercenaryHireList();
		buildings.Add(new Bar());
		buildings.Add(new Bar());

		itemsOnSale = new List<InventoryItem>();
		itemsOnSale.Add(new FoodBig());
		itemsOnSale.Add(new Scrap());
		itemsOnSale.Add(new Scrap());
		itemsOnSale.Add(new Knife());
		itemsOnSale.Add(new AssaultRifle());
		itemsOnSale.Add(new Backpack());
	}

	public virtual void UpdateMercenaryHireList()
	{
		while (mercenaryHireList.Count < requiredMercHireListSize)
		{
			mercenaryHireList.Add(new PartyMember());
		}
	}

	public List<PartyMember> GetHireableMercenariesList()
	{
		return mercenaryHireList;
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

	public void BarBuilt()
	{
		//barBuilt=true;
		if (WinBuildingsAreBuilt())
			GameManager.main.EndCurrentGame(true);
	}

	bool WinBuildingsAreBuilt()
	{
		if (barBuilt) return true;
		else return false;
	}
	

	// Use this for initialization
	void Start () 
	{
		main = this;
		PartyManager.ETimePassed += DailyCashBalanceChange;
	}
}
