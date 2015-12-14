using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BarricadeToken : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, IPointerDownHandler /*,IBeginDragHandler,IDragHandler,IEndDragHandler, IInitializePotentialDragHandler*/
{
	public static BarricadeToken barricadeTokenBeingDragged;
	static bool dragPermitted=false;
	public Text myHealthText;
	public RoomButtonHandler assignedRoomButton;
	
	public void AssignBarricade(RoomButtonHandler roomWithBarricade)
	{
		//myHealthText.text=assignedBarricade.health.ToString();
		assignedRoomButton=roomWithBarricade;
		UpdateHealth(assignedRoomButton.assignedRoom.barricadeInRoom.health);
	}
	
	public void AssignNewRoomButton(RoomButtonHandler newRoomButton)
	{
		assignedRoomButton=newRoomButton;
	}
	
	public void SetHidden(bool hidden)
	{
		if (hidden)
		{
			GetComponent<Image>().enabled=false;
			myHealthText.enabled=false;
		}
		else
		{
			GetComponent<Image>().enabled=true;
			myHealthText.enabled=true;
		}
	}
	public void UpdateHealth(int newHealth) 
	{
		myHealthText.text=newHealth.ToString();
	}
	/*
	public void TokenClicked()
	{
		EncounterCanvasHandler.main.BarricadeBashClicked(assignedRoomButton);
	}*/

	#region IPointerDownHandler implementation

	public void OnPointerDown (PointerEventData eventData)
	{
		EncounterCanvasHandler.main.BarricadeBashClicked(assignedRoomButton);
	}

	#endregion
	
	#region IPointerEnterHandler implementation
	
	public void OnPointerEnter (PointerEventData eventData)
	{
		string tooltipText="Blocks entrance into the room";
		Vector2 selectedMemberCoords=EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember];
		if ((selectedMemberCoords-assignedRoomButton.GetRoomCoords()).magnitude<=1) tooltipText+="\nClick: break barricade(1)";
		TooltipManager.main.CreateTooltip(tooltipText,this.transform);
	}
	
	#endregion
	
	#region IPointerExitHandler implementation
	
	public void OnPointerExit (PointerEventData eventData)
	{
		TooltipManager.main.StopAllTooltips();
	}
	
	#endregion
	/*
	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		if (dragPermitted)
		{
			GetComponent<Image>().raycastTarget=false;
			transform.SetParent(EncounterCanvasHandler.main.transform);
			barricadeTokenBeingDragged=this;
		}
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		if (dragPermitted) 
		{
			//this represents the 3x3 draggable area limit
			float dragDistanceLimit=assignedRoomButton.GetComponent<RectTransform>().rect.width*1.3f;
			Vector3 centerRoomPos
			=EncounterCanvasHandler.main.roomButtons[EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember]]
			.transform.position;
			
			if (Mathf.Abs(Input.mousePosition.x-centerRoomPos.x)<=dragDistanceLimit) 
			transform.position=new Vector3(Input.mousePosition.x,transform.position.y);
			if (Mathf.Abs(Input.mousePosition.y-centerRoomPos.y)<=dragDistanceLimit) 
			transform.position=new Vector3(transform.position.x,Input.mousePosition.y);
			//if (Mathf.Abs(Input.mousePosition.x-centerRoomPos.x)<=dragDistanceLimit
			//&&  Mathf.Abs(Input.mousePosition.y-centerRoomPos.y)<=dragDistanceLimit)	
			//{transform.position=Input.mousePosition;}
		}
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		if (dragPermitted)
		{
			GetComponent<Image>().raycastTarget=true;
			barricadeTokenBeingDragged=null;
			transform.SetParent(assignedRoomButton.transform,false);
			transform.position=transform.parent.position;
		}
		dragPermitted=false;
	}

	#endregion

	#region IInitializePotentialDragHandler implementation

	public void OnInitializePotentialDrag (PointerEventData eventData)
	{
		int draggingMemberX=(int)EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember].x;
		int draggingMemberY=(int)EncounterCanvasHandler.main.memberCoords[EncounterCanvasHandler.main.selectedMember].y;
		//if (Mathf.Abs(draggingMemberX-assignedRoomButton.roomX)+Mathf.Abs(draggingMemberY-assignedRoomButton.roomY)==1)
		float xDiff=Mathf.Abs(draggingMemberX-assignedRoomButton.roomX);
		float yDiff=Mathf.Abs(draggingMemberY-assignedRoomButton.roomY);
		if ((xDiff>0 || yDiff>0) && (xDiff<2 && yDiff<2))
		{dragPermitted=true;}
		else {dragPermitted=false;}
	}

	#endregion*/
}
