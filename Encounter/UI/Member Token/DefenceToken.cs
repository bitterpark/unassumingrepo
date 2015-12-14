using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DefenceToken:MemberStatusToken
{
	protected override string GenerateTooltipText ()
	{
		return "Defence enabled";
	}
	
	public void DefenceStatusChanged(bool moveTaken, bool actionTaken)
	{
		if (moveTaken | actionTaken) GetComponent<Image>().enabled=false;
		else GetComponent<Image>().enabled=true;
	}	
}
