using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryScreenHandler : MonoBehaviour 
{
	PartyMember selectedMember;
	public ItemImageHandler itemPrefab;
	public PerkTextHandler perkPrefab;
	
	public Text memberNameText;
	public static InventoryScreenHandler mainISHandler;
	
	void Start() 
	{
		mainISHandler=this;
		PartyManager.InventoryChanged+=InventoryChangeHandler;
		selectedMember=null;
	}
	
	public void InventoryChangeHandler() 
	{
		if (GetComponent<Canvas>().enabled==true) {RefreshInventoryItems();}
	}
	
	public void AssignSelectedMember(PartyMember newMember)
	{
		if (selectedMember!=newMember && !EncounterManager.mainEncounterManager.combatOngoing)
		{
			selectedMember=newMember;
			GetComponent<Canvas>().enabled=true;
			memberNameText.text=newMember.name;		
			RefreshInventoryItems();
		} 
		else 
		{
			selectedMember=null;
			GetComponent<Canvas>().enabled=false;
		}
	}
	
	void RefreshInventoryItems()
	{
		//Remove old weapons
		foreach (Button oldWeaponButton in transform.FindChild("Inventory Panel").FindChild("Weapons Group").GetComponentsInChildren<Button>()) 
		{
			GameObject.Destroy(oldWeaponButton.gameObject);
		}
	
		//Refresh melee slot
		ItemImageHandler newMeleeWeaponButton=Instantiate (itemPrefab) as ItemImageHandler;
		newMeleeWeaponButton.AssignItem(selectedMember.equippedMeleeWeapon);
		newMeleeWeaponButton.GetComponent<Button>().onClick.AddListener(()=>WeaponSlotClicked(selectedMember.equippedMeleeWeapon));
		newMeleeWeaponButton.transform.SetParent(transform.FindChild("Inventory Panel").FindChild("Weapons Group"),false);
		//GameObject.Destroy(transform.FindChild("Inventory Panel").FindChild("Weapons Group").gameObject);
		
		//Refresh ranged slot
		ItemImageHandler newRangedWeaponButton=Instantiate (itemPrefab) as ItemImageHandler;
		newRangedWeaponButton.AssignItem(selectedMember.equippedRangedWeapon);
		newRangedWeaponButton.GetComponent<Button>().onClick.AddListener(()=>WeaponSlotClicked(selectedMember.equippedRangedWeapon));
		newRangedWeaponButton.transform.SetParent(transform.FindChild("Inventory Panel").FindChild("Weapons Group"),false);
		//GameObject.Destroy(transform.FindChild("Inventory Panel").FindChild("Ranged Weapon Slot").gameObject);
		
		//Refresh current equipment
		foreach (Button oldItemButton in transform.FindChild("Inventory Panel").FindChild("Equipped Items").GetComponentsInChildren<Button>())
		{
			GameObject.Destroy(oldItemButton.gameObject);
		}	
		foreach (InventoryItem item in selectedMember.equippedItems)
		{
			ItemImageHandler newItemButton=Instantiate(itemPrefab);
			newItemButton.AssignItem(item);
			newItemButton.transform.SetParent(transform.FindChild("Inventory Panel").FindChild("Equipped Items"),false);
			newItemButton.GetComponent<Button>().onClick.AddListener(()=>InventoryItemClicked(newItemButton.assignedItem));
		}	
		
		//Refresh inventory items
		foreach (Button oldItemButton in transform.FindChild("Inventory Panel").FindChild("Common Inventory").GetComponentsInChildren<Button>())
		{
			GameObject.Destroy(oldItemButton.gameObject);
		}	
		foreach (InventoryItem item in PartyManager.mainPartyManager.partyInventory)
		{
			ItemImageHandler newItemButton=Instantiate(itemPrefab);
			newItemButton.AssignItem(item);
			newItemButton.transform.SetParent(transform.FindChild("Inventory Panel").FindChild("Common Inventory"),false);
			newItemButton.GetComponent<Button>().onClick.AddListener(()=>InventoryItemClicked(newItemButton.assignedItem));
		}
		
		//Refresh perks
		foreach (Image oldPerkImage in transform.FindChild("Inventory Panel").FindChild("Perks Group").GetComponentsInChildren<Image>())
		{
			GameObject.Destroy(oldPerkImage.gameObject);
		}	
		foreach (Perk memberPerk in selectedMember.perks)
		{
			PerkTextHandler newPerkImage=Instantiate(perkPrefab);
			newPerkImage.AssignPerk(memberPerk);
			newPerkImage.transform.SetParent(transform.FindChild("Inventory Panel").FindChild("Perks Group"),false);
		}
	}
	
	public void InventoryItemClicked(InventoryItem clickedItem)
	{
		clickedItem.UseAction(selectedMember);
	}
	
	public void WeaponSlotClicked(InventoryItem clickedItem)
	{
		if (clickedItem!=null)
		{
			Weapon clickedWeapon=clickedItem as Weapon;
			clickedWeapon.Unequip(selectedMember);
		}
	}
	
	void OnDestroy() {PartyManager.InventoryChanged-=InventoryChangeHandler;}
}
