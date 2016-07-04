using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FinancesTab : MonoBehaviour,TownTab
{
	public Text incomeAndExpensePrefab;

	public Transform incomeGroup;
	public Transform expensesGroup;

	public Text currentBalanceText;
	public Text expectedWeeklyBalanceText;

	public void OpenTab()
	{
		gameObject.SetActive(true);
		DisplayExpectedWeeklyBalance();
	}

	void DisplayExpectedWeeklyBalance()
	{
		currentBalanceText.text="Current balance = $"+TownManager.main.money;
		
		foreach (TownManager.FutureTransaction income in TownManager.main.GetFutureIncomes())
			AddIncome(income.transactionText);
		
		foreach (TownManager.FutureTransaction expense in TownManager.main.GetFutureExpenses())
			AddExpense(expense.transactionText);

		expectedWeeklyBalanceText.text = "Expected weekly balance = $"+TownManager.main.GetExpectedWeeklyBalance();
	}

	void AddIncome(string incomeText)
	{
		AddLineToGroup(incomeText, incomeGroup);
	}

	void ClearIncome()
	{
		ClearLinesFromGroup(incomeGroup);
	}

	void AddExpense(string expenseText)
	{
		AddLineToGroup(expenseText, expensesGroup);
	}

	void ClearExpenses()
	{
		ClearLinesFromGroup(expensesGroup);
	}

	void AddLineToGroup(string text, Transform group)
	{
		Text newLineText = Instantiate(incomeAndExpensePrefab);
		newLineText.text = text;
		newLineText.transform.SetParent(group, false);
	}

	void ClearLinesFromGroup(Transform group)
	{
		foreach (Text textComponent in group.GetComponentsInChildren<Text>())
		{
			if (textComponent.tag == "UI Clearable") 
				GameObject.Destroy(textComponent.gameObject);
		}
	}

	

	public void CloseTab()
	{
		ClearExpenses();
		ClearIncome();
		gameObject.SetActive(false);
	}
}
