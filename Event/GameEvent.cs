using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameEvent
{
	public abstract string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers);// {return null;}
	public virtual bool PreconditionsMet(MapRegion eventRegion, List<PartyMember> movedMembers) {return true;}
	public virtual bool AllowMapMove() {return true;}
	public virtual List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers) {return null;}
	public virtual string DoChoice(string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers) {return null;}
	public bool repeatable=false;
}

public struct EventChoice
{
	public string choiceTxt;
	public bool grayedOut;
	public EventChoice(string txt, bool gray)
	{
		choiceTxt=txt;
		grayedOut=gray;
	}
	public EventChoice(string txt)
	{
		choiceTxt=txt;
		grayedOut=false;
	}
}