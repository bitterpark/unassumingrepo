using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecipeIngredient : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
	public Image ingredientIcon;
	public Text requiredNumberText;
		
	InventoryItem ingredientItem=null;

	public void AssignIngredientItem(CraftRecipe.CraftableItems itemType, int requiredNumber)
	{
		ingredientItem = CraftRecipe.GetItemInstance(itemType);
		ingredientIcon.sprite=ingredientItem.GetItemSprite();
		requiredNumberText.text="x"+requiredNumber;
	}
	
	#region IPointerEnterHandler implementation
	public void OnPointerEnter (PointerEventData eventData)
	{
		if (ingredientItem!=null) TooltipManager.main.CreateTooltip(ingredientItem.GetMouseoverDescription(),this.transform); 
	}
	#endregion
	
	#region IPointerExitHandler implementation
	public void OnPointerExit (PointerEventData eventData){TooltipManager.main.StopAllTooltips();}
	#endregion
	
	
}
