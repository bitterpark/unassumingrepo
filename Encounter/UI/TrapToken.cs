using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrapToken : MonoBehaviour, IAttackAnimation 
{	
	bool readyToDispose=false;
	EncounterRoom assignedRoom;
	Trap assignedTrap;
	
	public void AssignTrap(Trap trap, EncounterRoom room)
	{
		assignedRoom=room;
		assignedTrap=trap;
	}
	
	#region AttackAnimation implementation
	
	public IEnumerator AttackAnimation(IGotHitAnimation targetAnimation)
	{
		/*
		foreach (Text child in GetComponentsInChildren<Text>())
		{
			child.transform.
		}*/
		//print ("starting attack animation!");
		transform.localScale=new Vector3(1.6f,1.6f,1f);
		//print ("trap token doing yeld return waitforseconds");
		//see if it transfers control back to encounter canvas handler here or on yield break
		if (targetAnimation!=null) yield return StartCoroutine(targetAnimation.GotHitAnimation());
		else yield return new WaitForSeconds(0.6f);
		//print ("yeld return waitforseconds done, finishing scaling and yield break");
		//print ("yielding attack animation!");
		transform.localScale=new Vector3(1f,1f,1f);
		readyToDispose=true;
		yield break;
	}
	
	#endregion
	
	//This makes sure the trap will play it's attack animation in full before being deleted
	public void BeginDispose()
	{
		StartCoroutine(DisposeTrap());
	}
	IEnumerator DisposeTrap() 
	{
		while (!readyToDispose) yield return new WaitForFixedUpdate();
		if (assignedRoom==null) {throw new System.Exception("trying to dispose a trap with no assigned room!");}
		EncounterCanvasHandler.main.roomButtons[assignedRoom.GetCoords()].RemoveTrap(assignedTrap);
		
	}
}
