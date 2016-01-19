using UnityEngine;
using System.Collections;

public class Trap 
{
	
	int damage=5;
	EncounterRoom assignedRoom;
	public TrapToken assignedToken;
	bool sprung=false;
	
	public virtual void SetOff()
	{
		if (!EncounterCanvasHandler.main.encounterOngoing) {throw new System.Exception("Trap set off outside of an encounter!");}
		
		if (assignedRoom.hasEnemies && !sprung) 
		{
			if (assignedToken==null) {throw new System.Exception("Trying to set off a trap with no token assigned!");}
			//GameManager.DebugPrint("begginning trap setoff");
			EncounterCanvasHandler.main.RegisterDamage
			(damage,assignedRoom.enemiesInRoom[0].bodyParts[0],false,assignedRoom.enemiesInRoom[0],assignedToken);
			sprung=true;
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
