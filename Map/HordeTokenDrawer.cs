using UnityEngine;
using System.Collections;

public class HordeTokenDrawer : MonoBehaviour 
{
	public void MoveToDestination(MapRegion start,MapRegion newDest)
	{
		StopAllCoroutines();
		//from invisible area
		if (!start.visible)
		{
			// to visible area
			if (newDest.visible) 
			{
				StartCoroutine(MoveToNewDest(start,newDest));
				GetComponent<SpriteRenderer>().enabled=true;
			}
			else
			{
				JumpToNewDest(newDest);
				GetComponent<SpriteRenderer>().enabled=false;
			}
		}
		else
		{	
			StartCoroutine(MoveToNewDest(start,newDest));
			GetComponent<SpriteRenderer>().enabled=true;
			/*
			//from visible to visible
			if (newDest.visible) 
			{
				StartCoroutine(MoveToNewDest(newDest));
				GetComponent<SpriteRenderer>().enabled=true;
			}
			else //from visible to invisible
			{
				StartCoroutine(MoveToNewDest(n));
				GetComponent<SpriteRenderer>().enabled=true;
			}*/
				
		}
		
	}
	
	IEnumerator MoveToNewDest(MapRegion start,MapRegion newDest)
	{
		transform.position=start.transform.position;
		Vector3 endCoords=newDest.transform.position;
		float spd=100f;
		Vector3 moveVector=(endCoords-transform.position).normalized*spd;
		while (true)
		{
			Vector3 delta=moveVector*Time.deltaTime;
			if ((endCoords-transform.position).magnitude<=delta.magnitude) 
			{
				transform.position=endCoords;
				break;
			}
			else {transform.Translate(delta);}
			yield return new WaitForFixedUpdate();
		}
		if (!newDest.visible) {GetComponent<SpriteRenderer>().enabled=false;}
	}
	
	void JumpToNewDest(MapRegion newDest) 
	{
		transform.position=newDest.transform.position;
		//GetComponent<SpriteRenderer>().enabled=false;
	}
	
	void OnDestroy() {StopAllCoroutines();}
}
