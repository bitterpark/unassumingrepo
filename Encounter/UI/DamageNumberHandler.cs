using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageNumberHandler : MonoBehaviour 
{
	int damageNumber;
	float lifeTime=1f;
	
	public void AssignNumber(int number)
	{
		GetComponent<Text>().text=number.ToString();
	}
	
	void FixedUpdate()
	{
		if (lifeTime>0) 
		{
			lifeTime-=Time.deltaTime;
			transform.Translate(0,3,0);
		} else {GameObject.Destroy(this.gameObject);}
	}
}
