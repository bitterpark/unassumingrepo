using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PartyStatusCanvasHandler : MonoBehaviour {

	bool displayEnabled=false;
	
	public Text mapXText;
	public Text mapYText;
	public Text timeText;
	public Text ammoText;
	
	public static PartyStatusCanvasHandler main;
	
	public NotePanelHandler notePanelPrefab;
	
	void Start() {main=this;}
	
	// Update is called once per frame
	void Update () 
	{
		if (displayEnabled)
		{
			mapXText.text="X:"+PartyManager.mainPartyManager.mapCoordX.ToString();
			mapYText.text="Y:"+PartyManager.mainPartyManager.mapCoordY.ToString();
			timeText.text="Time:"+PartyManager.mainPartyManager.dayTime.ToString()+":00";
			ammoText.text="Ammo:"+PartyManager.mainPartyManager.ammo.ToString();
		}
	}
	
	public void NewNotification(string notificationText)
	{
		//if (displayEnabled)
		{
			NotePanelHandler newPanel=Instantiate(notePanelPrefab);
			newPanel.AssignNote(notificationText);
			//newPanel.transform.SetParent(transform,false);
		}
	}
	
	public void EnableStatusDisplay()
	{
		GetComponent<Canvas>().enabled=true;
		displayEnabled=true;
	}
	public void DisableStatusDisplay()
	{
		GetComponent<Canvas>().enabled=false;
		displayEnabled=false;
	}
}
