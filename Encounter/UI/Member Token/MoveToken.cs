using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveToken:MemberStatusToken
{
	protected override string GenerateTooltipText ()
	{
		return "Move available";
	}
	
	public void MoveStatusChanged(bool moveTaken, bool turnTaken, bool enoughStamina)
	{
		if (moveTaken) GetComponent<Image>().color=Color.green;//.enabled=false;
		else GetComponent<Image>().color=Color.blue;//.enabled=true;
		if (!enoughStamina || turnTaken) GetComponent<Image>().color=Color.red;
	}
}
