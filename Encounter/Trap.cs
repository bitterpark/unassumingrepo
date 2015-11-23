using UnityEngine;
using System.Collections;

public class Trap 
{
	
	int damage=5;
	EncounterRoom assignedRoom;
	public TrapToken assignedToken;
	
	public virtual void SetOff()
	{
		if (!EncounterCanvasHandler.main.encounterOngoing) {throw new System.Exception("Trap set off outside of an encounter!");}
		
		
		if (assignedRoom.hasEnemies) 
		{
			if (assignedToken==null) {throw new System.Exception("Trying to set off a trap with no token assigned!");}
			//GameManager.DebugPrint("begginning trap setoff");
			EncounterCanvasHandler.main.RegisterDamage(damage,false,assignedRoom.enemiesInRoom[0],assignedToken);
			assignedToken.BeginDispose();
			//GameManager.DebugPrint("resuming trap script after trap setoff");
			//EncounterCanvasHandler.main.roomButtons[assignedRoom.GetCoords()].RemoveTrap(this);
			//EncounterCanvasHandler.main.roomButtons[assignedRoom.GetCoords()].AttackEnemyInRoom(damage,assignedRoom.enemiesInRoom[0],false);
		}
	}
	/*
	IEnumerator DisposalRoutine()
	{
		while (!assignedToken.readyToDispose) {yield return new WaitForFixedUpdate();}
		EncounterCanvasHandler.main.roomButtons[assignedRoom.GetCoords()].RemoveTrap(this);
	}*/
	
	public Trap (EncounterRoom room)
	{
		assignedRoom=room;
	}
}
