using UnityEngine;
using System.Collections;

public class HearingTokenHandler : MonoBehaviour 
{
	void Start()
	{
		EncounterCanvasHandler.main.ERoundIsOver+=DisposeToken;
	}
	
	void OnDestroy() 
	{
		EncounterCanvasHandler.main.ERoundIsOver-=DisposeToken;
	}
	
	public void DisposeToken()
	{
		GameObject.Destroy(this.gameObject);
	}
	
}
