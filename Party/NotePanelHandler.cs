using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NotePanelHandler : MonoBehaviour 
{
	float notificationTime;
	public Text notificationText;
	
	public void AssignNote(string newText) {AssignNote(newText,2f);}
	
	public void AssignNote(string newText, float lifeTime)
	{
		notificationText.text=newText;
		notificationTime=lifeTime;
	}
	
	void Update()
	{
		notificationTime-=Time.deltaTime;
		if (notificationTime<=0) {GameObject.Destroy(this.gameObject);}
	}
}
