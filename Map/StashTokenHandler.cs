using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class StashTokenHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{	
	#region IPointerEnterHandler implementation
	public void OnPointerEnter (PointerEventData eventData)
	{
		TooltipManager.main.StopAllTooltips();
		string tooltipDescription="Stashed items";
		foreach (InventoryItem item in GetComponentInParent<MapRegion>().GetStashedItems())
		{
			tooltipDescription+="\n-"+item.itemName;
		}
		
		TooltipManager.main.CreateTooltip(tooltipDescription,this.transform);
	}
	#endregion
	
	
	#region IPointerExitHandler implementation
	public void OnPointerExit (PointerEventData eventData)
	{
		TooltipManager.main.StopAllTooltips();
	}
	#endregion
}
