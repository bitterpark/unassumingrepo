using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour 
{
	public static TooltipManager main;
	
	public Tooltip tooltipPrefab;
	static Tooltip activeTooltip=null;
	float edgeOffsetSize=5f;
	float tooltipWidth=80f;
	
	void Start() {main=this;}

	public void CreateItemTooltip(InventoryItem item, Transform parent)
	{
		if (item != null)
		{
			if (item.GetType().BaseType == typeof(InventoryItem))
				TooltipManager.main.CreateTooltip(item.GetMouseoverDescription(), parent);
			else
			{
				if (item.GetType().BaseType == typeof(EquippableItem))
				{
					EquippableItem castItem = item as EquippableItem;
					main.CreateTooltip(item.GetMouseoverDescription(), parent
						, castItem.addedCombatCards.ToArray());
				}
				if (item.GetType().BaseType.BaseType == typeof(Weapon))
				{
					Weapon castItem = item as Weapon;
					CreateTooltip(item.GetMouseoverDescription(), parent
						, castItem.addedCombatCards.ToArray());
				}
			}
		}
	}

	public void CreateTooltip(string tooltipText, Transform tooltipParent, CombatCard[] displayCards)
	{
		StopAllTooltips();
		activeTooltip = Instantiate(tooltipPrefab);
		activeTooltip.AssignDisplayValues(tooltipText, tooltipParent, displayCards);
	}
	public void CreateTooltip(string tooltipText, Transform tooltipParent, RewardCard[] displayCards)
	{
		StopAllTooltips();
		activeTooltip = Instantiate(tooltipPrefab);
		activeTooltip.AssignDisplayValues(tooltipText, tooltipParent, displayCards);
	}

	public void CreateTooltip(string tooltipText, Transform tooltipParent)
	{
		StopAllTooltips();
		activeTooltip=Instantiate(tooltipPrefab);
		activeTooltip.AssignDisplayValues(tooltipText,tooltipParent);
	}	
	
	public void StopAllTooltips() 
	{
		if (activeTooltip!=null) GameObject.Destroy(activeTooltip.gameObject);
	}
}
