using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionToken:MemberStatusToken
{
	protected override string GenerateTooltipText ()
	{
		return "Action available";
	}
	
	public void ActionStatusChanged(bool actionTaken)
	{
		if (actionTaken) GetComponent<Image>().color=Color.red;//.enabled=false;
		else GetComponent<Image>().color=Color.green;//.enabled=true;
	}
}
