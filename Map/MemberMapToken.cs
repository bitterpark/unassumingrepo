using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MemberMapToken : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler 
{
	public Image actionStatusToken;
	public Text nameText;
	
	public PartyMember assignedMember;
	public bool moved
	{
		get {return _moved;}
		set 
		{
			_moved=value;
			if (_moved)
			{
				actionStatusToken.enabled=false;
			} 
			else 
			{
				actionStatusToken.enabled=true;
				actionStatusToken.sprite=SpriteBase.mainSpriteBase.noActionSprite;
			}
		}
	}
	bool _moved=false;
	
	public void AssignPartyMember(PartyMember assigned)
	{
		assignedMember=assigned;
		nameText.text=assigned.name;
		GetComponent<Image>().color=assigned.color;
		MoveToken(MapManager.main.GetRegion(assignedMember.worldCoords).transform);
	}
	
	public void Select()
	{
		if (!MapScoutingHandler.scoutingDialogOngoing)
		{
			PartyManager.mainPartyManager.AddSelectedMember(assignedMember);//.selectedMembers.Add(assignedMember);
			GetComponent<Image>().color=Color.blue;
		}
	}
	
	public void Deselect()
	{
		if (PartyManager.mainPartyManager.selectedMembers.Contains(assignedMember))
		{
			PartyManager.mainPartyManager.RemoveSelectedMember(assignedMember);
			GetComponent<Image>().color=assignedMember.color;
		}
	}

	public void MoveToken(Transform newRegion)
	{
		transform.SetParent(newRegion,false);
		transform.position=newRegion.position;
	}
	
	public void NewTaskSet(Sprite taskSprite)
	{
		moved=true;
		actionStatusToken.enabled=true;
		actionStatusToken.sprite=taskSprite;
	}
	public void TaskRemoved()
	{
		moved=false;
	}

	#region IPointerClickHandler implementation	
	public void OnPointerClick (PointerEventData eventData)
	{
		PartyManager.mainPartyManager.MapMemberClicked(assignedMember);
		//throw new System.NotImplementedException ();
	}

	#endregion

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		TooltipManager.main.StopAllTooltips();
	}

	#endregion
}
