using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MemberMapToken : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler 
{
	
	PartyMember assignedMember;
	public bool moved
	{
		get {return _moved;}
		set 
		{
			_moved=value;
			if (_moved)
			{
				Deselect();
			}
		}
	}
	bool _moved=false;
	
	public void AssignPartyMember(PartyMember assigned)
	{
		assignedMember=assigned;
		GetComponent<Image>().color=assigned.color;
		MoveToken(MapManager.main.GetRegion(assignedMember.worldCoords).transform);
	}
	
	public void Select()
	{
		if (!MapScoutingHandler.scoutingDialogOngoing)
		{
			PartyManager.mainPartyManager.selectedMembers.Add(assignedMember);
			GetComponent<Image>().color=Color.blue;
		}
	}
	
	public void Deselect()
	{
		if (PartyManager.mainPartyManager.selectedMembers.Contains(assignedMember))
		{
			PartyManager.mainPartyManager.selectedMembers.Remove(assignedMember);
			GetComponent<Image>().color=assignedMember.color;
		}
	}

	public void MoveToken(Transform newRegion)
	{
		transform.SetParent(newRegion,false);
		transform.position=newRegion.position;
	}


	#region IPointerClickHandler implementation	
	public void OnPointerClick (PointerEventData eventData)
	{
		if (!moved) PartyManager.mainPartyManager.MapMemberClicked(assignedMember);
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
