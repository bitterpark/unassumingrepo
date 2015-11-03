using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour 
{
	public static TooltipManager main;
	
	public GameObject tooltipPrefab;
	static GameObject activeTooltip;
	float edgeOffsetSize=5f;
	float tooltipWidth=80f;
	
	void Start() {main=this;}
	
	public void CreateTooltip(string tooltipText, Transform tooltipParent)
	{
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
		activeTooltip.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right,-(tooltipWidth+edgeOffsetSize),tooltipWidth);
		activeTooltip.GetComponent<Canvas>().enabled=true;
		activeTooltip.GetComponent<Canvas>().overrideSorting=true;
		activeTooltip.GetComponent<Canvas>().sortingOrder=100;
	}	
	
	public void StopAllTooltips() {GameObject.Destroy(activeTooltip);}
}
