using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TraitUIHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	Trait assignedTrait;
	public Text nameText;
	bool drawMouseoverText=false;
	
	public void AssignTrait(Trait newTrait)
	{
		assignedTrait=newTrait;
		nameText.text=newTrait.name;
		GetComponent<Button>().onClick.RemoveAllListeners();
		if (assignedTrait.GetType().BaseType==typeof(Trait)) GetComponent<Button>().image.color=Color.white; 
		else 
		{
			Skill assignedSkill=assignedTrait as Skill;
			if (assignedSkill.learned) GetComponent<Button>().image.color=Color.white; 
			else
			{
				if (InventoryScreenHandler.mainISHandler.selectedMember.skillpoints<1) GetComponent<Button>().image.color=Color.gray;
				else
				{
					GetComponent<Button>().image.color=Color.green;
					GetComponent<Button>().onClick.AddListener(
						()=>
						{
						//if (InventoryScreenHandler.mainISHandler.selectedMember.skillpoints>=1)
						{
							//Order is important
							InventoryScreenHandler.mainISHandler.selectedMember.ActivateSkill(assignedSkill);
							InventoryScreenHandler.mainISHandler.selectedMember.skillpoints--;
							InventoryScreenHandler.mainISHandler.RefreshInventoryItems();
						}
					});
				}
			}
		}
	}
	
	//public void DrawMouseoverText() {TooltipManager.main.CreateTooltip(assignedPerk.GetMouseoverDescription(),transform);}
	//public void StopMouseoverText() {TooltipManager.main.StopAllTooltips();}//drawMouseoverText=false;}

	#region IPointerEnterHandler implementation
	public void OnPointerEnter (PointerEventData eventData)
	{
		if (assignedTrait.GetAddedCombatCards().Count > 0)
		{
			CombatCard[] addedCards = assignedTrait.GetAddedCombatCards().ToArray();
			TooltipManager.main.CreateTooltip(assignedTrait.GetMouseoverDescription(), transform, addedCards);
		}
		else 
			TooltipManager.main.CreateTooltip(assignedTrait.GetMouseoverDescription(),transform);
	}
	#endregion

	#region IPointerExitHandler implementation
	public void OnPointerExit (PointerEventData eventData)
	{
		TooltipManager.main.StopAllTooltips();
	}
	#endregion
}
