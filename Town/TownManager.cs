using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TownManager : MonoBehaviour {

	public static TownManager main;

	public int money;

	List<PartyMember> mercenaryHireList = new List<PartyMember>();
	const int requiredMercHireListSize = 5;

	public void MercenaryHired(PartyMember hiredMerc) { mercenaryHireList.Remove(hiredMerc);}
	public void MercenaryContractFinished(PartyMember finishedMerc) { mercenaryHireList.Add(finishedMerc); }

	public List<PartyMember> GetHireableMercenariesList()
	{
		return mercenaryHireList;
	}

	void UpdateMercenaryHireList()
	{
		while (mercenaryHireList.Count < requiredMercHireListSize)
		{
			mercenaryHireList.Add(new PartyMember(MapManager.main.mapRegions[0]));
		}
	}

	public List<TownBuilding> buildings;

	public List<InventoryItem> itemsOnSale;

	public void NewGameState()
	{
		money = 3000;
		UpdateMercenaryHireList();
		buildings = new List<TownBuilding>();
		buildings.Add(new Bar());
		buildings.Add(new Bar());

		itemsOnSale = new List<InventoryItem>();
		itemsOnSale.Add(new FoodBig());
		itemsOnSale.Add(new Scrap());
		itemsOnSale.Add(new Knife());
		itemsOnSale.Add(new AssaultRifle());
	}

	// Use this for initialization
	void Start () 
	{
		main = this;
	}
}
