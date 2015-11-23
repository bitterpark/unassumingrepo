using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingTextHandler : MonoBehaviour 
{
	int damageNumber;
	float lifeTime=1f;
	
	public void AssignNumber(int number)
	{
		GetComponent<Text>().text=number.ToString();
	}
	public void AssignText(string newText) {GetComponent<Text>().text=newText;}
	
	void FixedUpdate()
	{
		if (lifeTime>0) 
		{
			lifeTime-=Time.deltaTime;
			transform.Translate(0,3,0);
		} else {GameObject.Destroy(this.gameObject);}
	}
}
