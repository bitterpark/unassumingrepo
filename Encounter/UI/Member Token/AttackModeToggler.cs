using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AttackModeToggler : MonoBehaviour ,IPointerEnterHandler,IPointerExitHandler
{
	public Sprite rangedSprite;
	public Sprite meleeSprite;
	enum AttackMode {Ranged, Melee};
	AttackMode currentMode=AttackMode.Melee;
	
	public void SetMode(bool isRanged)
	{
		if (isRanged) 
		{
			GetComponent<Image>().sprite=rangedSprite;
			currentMode=AttackMode.Ranged;
		}
		else 
		{
			GetComponent<Image>().sprite=meleeSprite;
			currentMode=AttackMode.Melee;
		}
	}

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		string tooltipText="";
		if (currentMode==AttackMode.Melee) tooltipText="Using melee";
		if (currentMode==AttackMode.Ranged) tooltipText="Using ranged";
		TooltipManager.main.CreateTooltip(tooltipText,transform);
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		TooltipManager.main.StopAllTooltips();
	}

	#endregion
}
