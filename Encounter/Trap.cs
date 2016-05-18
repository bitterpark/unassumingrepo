using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Trap 
{
	
	int trapDamage=0;
	EncounterRoom assignedRoom;
	public TrapToken assignedToken;
	bool sprung=false;
	
	public virtual void SetOff(EncounterEnemy activatingEnemy)
	{
		if (!EncounterCanvasHandler.main.encounterOngoing) {throw new System.Exception("Trap set off outside of an encounter!");}
		
		if (EncounterCanvasHandler.main.roomButtons[assignedRoom.GetCoords()] && !sprung) 
		{
			if (assignedToken==null) {throw new System.Exception("Trying to set off a trap with no token assigned!");}
			//GameManager.DebugPrint("begginning trap setoff");
			EnemyBodyPart attackedPart=null;
			bool attackSuccessful=false;
			if (activatingEnemy.body.TryGetBodyPart(EnemyBodyPart.PartTypes.Legs,out attackedPart)) attackSuccessful=true;
			else if (activatingEnemy.body.TryGetBodyPart(EnemyBodyPart.PartTypes.Vitals,out attackedPart)) attackSuccessful=true;
			
			if (attackSuccessful)
			{
				EncounterCanvasHandler.main.RegisterDamage(trapDamage,attackedPart,false,activatingEnemy,assignedToken,!assignedToken.GetComponent<Image>().enabled);
				sprung=true;
				assignedToken.BeginDispose();
			}
			else throw new System.Exception("Trap set off but could not find parts to damage!");
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
	
	public Trap (EncounterRoom room, int damage)
	{
		assignedRoom=room;
		trapDamage=damage;
	}
}
