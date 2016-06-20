using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FinancesTab : MonoBehaviour,TownTab
{
	public Transform expensesGroup;

	public Text expensePrefab;

	public Text balanceText;

	public void OpenTab()
	{
		gameObject.SetActive(true);
		DisplayExpenses();
	}

	void DisplayExpenses()
	{
		AddExpense("Crew: $"+TownManager.dailyPayPerCrew+"x"+TownManager.main.GetCrew()+" = -$"+TownManager.main.CalculateCrewExpenses());
		balanceText.text = "Balance = $"+TownManager.main.GetDailyBalance();
	}

	void AddExpense(string expenseText)
	{
		Text newExpenseText = Instantiate(expensePrefab);
		newExpenseText.text = expenseText;
		newExpenseText.transform.SetParent(expensesGroup, false);
	}


	void ClearExpenses()
	{
		foreach (Text textComponent in expensesGroup.GetComponentsInChildren<Text>())
		{
			if (textComponent.tag == "UI Clearable") GameObject.Destroy(textComponent.gameObject);
		}
	}

	public void CloseTab()
	{
		ClearExpenses();
		gameObject.SetActive(false);
	}
}
