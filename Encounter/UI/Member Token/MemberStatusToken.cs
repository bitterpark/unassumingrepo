using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public abstract class MemberStatusToken : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		TooltipManager.main.CreateTooltip(GenerateTooltipText(),this.transform);
	}

	#endregion


	#region IPointerExitHandler implementation
	public void OnPointerExit (PointerEventData eventData)
	{
		TooltipManager.main.StopAllTooltips();
	}
	#endregion
	
	protected abstract string GenerateTooltipText();
	
}






