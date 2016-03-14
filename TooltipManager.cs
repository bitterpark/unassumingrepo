using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour 
{
	public static TooltipManager main;
	
	public GameObject tooltipPrefab;
	static GameObject activeTooltip=null;
	float edgeOffsetSize=5f;
	float tooltipWidth=80f;
	
	void Start() {main=this;}
	
	public void CreateTooltip(string tooltipText, Transform tooltipParent)
	{
		StopAllTooltips();
		activeTooltip=Instantiate(tooltipPrefab);
		foreach (Text textComponent in activeTooltip.GetComponentsInChildren<Text>()) 
		{
			textComponent.text=tooltipText;
			//CAUTION! - this width is not the actual width of the tooltip, the bg image is this + manually set margin!!!
			tooltipWidth=textComponent.preferredWidth;
		}
		//activeTooltip.GetComponent<Text>().text=tooltipText;
		//activeTooltip.GetComponentInChildren<Text>().text=tooltipText;
		activeTooltip.transform.SetParent(tooltipParent,false);
		//tooltipWidth=activeTooltip.GetComponent<RectTransform>().rect.width;
		//print ("width is:"+activeTooltip.GetComponent<RectTransform>().rect.width);
		RectTransform.Edge tooltipSide=RectTransform.Edge.Right;
		
		//print ("Object position:"+tooltipParent.position);
		/*
		print ("Object screen position:"+Camera.main.WorldToScreenPoint(tooltipParent.position-new Vector3(Screen.width*0.5f,Screen.height*0.5f,0)));
		print ("Tooltip edge position:"+(Camera.main.WorldToScreenPoint(tooltipParent.position-new Vector3(Screen.width*0.5f,Screen.height*0.5f,0)).x
		+tooltipParent.GetComponent<RectTransform>().rect.width
		+tooltipWidth+edgeOffsetSize));
		print ("parent width:"+tooltipParent.GetComponent<RectTransform>().rect.width);
		print ("tooltip width and offset:"+(tooltipWidth+edgeOffsetSize));*/
		//print ("Screen width:"+Screen.width);
		if (Camera.main.WorldToScreenPoint(tooltipParent.position-new Vector3(Screen.width*0.5f,Screen.height*0.5f,0)).x
		+tooltipParent.GetComponent<RectTransform>().rect.width+tooltipWidth+edgeOffsetSize>Screen.width) 
		tooltipSide=RectTransform.Edge.Left;//.GetComponent<RectTransform>().rect.xMax+tooltipWidth+edgeOffsetSize>Screen.width)tooltipSide=RectTransform.Edge.Right;
		activeTooltip.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(tooltipSide,-(tooltipWidth+edgeOffsetSize),tooltipWidth);
		//if (realScaling) activeTooltip.transform.localScale/=80;
		activeTooltip.GetComponent<Canvas>().enabled=true;
		activeTooltip.GetComponent<Canvas>().overrideSorting=true;
		activeTooltip.GetComponent<Canvas>().sortingOrder=100;
	}	
	
	public void StopAllTooltips() 
	{
		if (activeTooltip!=null) GameObject.Destroy(activeTooltip);
	}
}
