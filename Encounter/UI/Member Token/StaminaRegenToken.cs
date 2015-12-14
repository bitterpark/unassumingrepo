using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StaminaRegenToken:MemberStatusToken
{
	protected override string GenerateTooltipText ()
	{
		return "Stamina regen enabled";
	}
	
	public void StaminaRegenStatusChanged(bool regenIsOn)
	{
		if (regenIsOn) GetComponent<Image>().enabled=true;
		else GetComponent<Image>().enabled=false;
	}
}
