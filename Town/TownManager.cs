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

	public const int mercenaryHireCost = 100;
	public const int mercenaryHireDuration = daysPerGameWeek;
	List<PartyMember> mercenaryHireList = new List<PartyMember>();
	List<PartyMember> previouslyHiredMercPool = new List<PartyMember>();
	int hirableMercsPoolSize = 6;
	public const int weeklyPayPerCrew = 90;

	public List<TownBuilding> buildings;

	public List<InventoryItem> itemsOnSale;

	bool enginesBuilt=false;
	bool marketUnlocked=false;
	bool workshopsUnlocked = false;

	int dailyMercHealRate = 0;

	float marketPriceMult = 1;

	public const int daysPerGameWeek = 3;

	List<FutureTransaction> expenseTransactions;
	List<FutureTransaction> incomeTransactions;

	public struct FutureTransaction
	{
		public int moneyDelta;
		public string transactionText;
		
		public FutureTransaction(int delta, string text)
		{
			moneyDelta = delta;
			transactionText = text;
		}
	}

	public void NewGameState()
	{
		LockMarket();
		LockWorkshops();

		incomeTransactions = new List<FutureTransaction>();
		expenseTransactions = new List<FutureTransaction>();

		dailyMercHealRate = 0;
		enginesBuilt = false;

		buildings = new List<TownBuilding>();
		buildings.Add(new EngineeringBay());
		buildings.Add(new CommCenter());
		buildings.Add(new CargoBay());
		buildings.Add(new SickBay());
		buildings.Add(new Engines());

		SetMoney(startingMoney);
		SetCrew(startingCrew);
		previouslyHiredMercPool.Clear();
		UpdateMercenaryHireList();

		itemsOnSale = new List<InventoryItem>();
		itemsOnSale.Add(new Pipe());
		itemsOnSale.Add(new Axe());
		itemsOnSale.Add(new Knife());
		itemsOnSale.Add(new AssaultRifle());
		itemsOnSale.Add(new NineM());
		itemsOnSale.Add(new Shotgun());
		itemsOnSale.Add(new ComputerParts());

		MapManager.main.GetTown().StashItem(new AssaultRifle());
		MapManager.main.GetTown().StashItem(new Stim());
		MapManager.main.GetTown().StashItem(new AmmoPouch());
		MapManager.main.GetTown().StashItem(new ArmorVest());
	}

	public virtual void UpdateMercenaryHireList()
	{
		mercenaryHireList.Clear();
		int returningMercsCount = Random.Range(1, Mathf.Min(3,hirableMercsPoolSize));
		for (int i = 0; i < returningMercsCount; i++)
		{
			//print("");
			if (previouslyHiredMercPool.Count == 0) 
				break;
			
			PartyMember randomlyPickedMerc=previouslyHiredMercPool[Random.Range(0,previouslyHiredMercPool.Count)];
			mercenaryHireList.Add(randomlyPickedMerc);
			//previouslyHiredMercPool.Remove(randomlyPickedMerc);
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
		previouslyHiredMercPool.Remove(hiredMerc);
	}
	public void MercenaryContractFinished(PartyMember finishedMerc)
	{
		previouslyHiredMercPool.Add(finishedMerc);
	}

	public void IncrementHireableMercsPoolSize(int delta)
	{
		hirableMercsPoolSize += delta;	
	}

	public int GetCrew() { return crew; }

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
		{
			BuildingsCrewUpdate();
			CalculateCrewExpenses();
		}
	}
	

	public virtual void BuildingsCrewUpdate()
	{
		foreach (TownBuilding building in buildings)
		{
			building.UpdateActiveStatus();
		}
	}

	public void AddFutureExpense(int expenseMagnitude, string text)
	{
		string financeLine = text + " = -$" + expenseMagnitude;
		expenseTransactions.Add(new FutureTransaction(-expenseMagnitude, financeLine));
	}
	public List<FutureTransaction> GetFutureExpenses()
	{
		return expenseTransactions;
	}

	public void AddFutureIncome(int incomeMagnitude, string text)
	{
		string financeLine = text + " = $" + incomeMagnitude;
		incomeTransactions.Add(new FutureTransaction(incomeMagnitude, financeLine));
	}
	public List<FutureTransaction> GetFutureIncomes()
	{
		return incomeTransactions;
	}


	public void SetMoney(int newMoneyBalance)
	{
		money = newMoneyBalance;
	}

	void DoEndOfWeekChanges()
	{
		UpdateMercenaryHireList();
		
		money = GetExpectedWeeklyBalance();
		expenseTransactions.Clear();
		incomeTransactions.Clear();

		if (money < 0)
			IncrementCrew(-noMoneyCrewPenalty);
		CalculateCrewExpenses();
	}

	public int GetExpectedWeeklyBalance()
	{
		int finalBalance=money;

		foreach (FutureTransaction expense in expenseTransactions)
			finalBalance += expense.moneyDelta;
		
		foreach (FutureTransaction income in incomeTransactions)
			finalBalance += income.moneyDelta;
			
		return finalBalance;
	}

	void CalculateCrewExpenses()
	{
		expenseTransactions.Clear();
		int nextWeeksCrewExpenses = crew * weeklyPayPerCrew;
		string crewExpensesText = "Crew: $" + weeklyPayPerCrew + "x" + crew;
		AddFutureExpense(nextWeeksCrewExpenses, crewExpensesText);
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

	public void UnlockWorkshops()
	{
		workshopsUnlocked = true;
		TownScreen.main.UnlockWorkshopsTab();
	}
	public void LockWorkshops()
	{
		workshopsUnlocked = false;
		TownScreen.main.LockWorkshopsTab();
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
		PartyManager.ENewWeekStart += DoEndOfWeekChanges;
	}
}
