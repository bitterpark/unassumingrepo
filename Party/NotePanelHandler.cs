using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NotePanelHandler : MonoBehaviour 
{
	float notificationTime;
	public Text notificationText;
	
	public void AssignNote(string newText)
	{
		notificationText.text=newText;
		notificationTime=2f;
	}
	
	void Update()
	{
		notificationTime-=Time.deltaTime;
		if (notificationTime<=0) {GameObject.Destroy(this.gameObject);}
	}
}
