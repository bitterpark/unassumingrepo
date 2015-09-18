using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PartyStatusTextHandler : MonoBehaviour {

	public Text mapXText;
	public Text mapYText;
	public Text timeText;
	public Text ammoText;
	
	// Update is called once per frame
	void Update () 
	{
		mapXText.text="X:"+PartyManager.mainPartyManager.mapCoordX.ToString();
		mapYText.text="Y:"+PartyManager.mainPartyManager.mapCoordY.ToString();
		timeText.text="Time:"+PartyManager.mainPartyManager.dayTime.ToString()+":00";
		ammoText.text="Ammo:"+PartyManager.mainPartyManager.ammo.ToString();
	}
}
