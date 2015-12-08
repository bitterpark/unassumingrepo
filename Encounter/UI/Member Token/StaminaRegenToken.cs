using UnityEngine;
using System.Collections;

public class StaminaRegenToken:MemberStatusToken
{
	protected override string GenerateTooltipText ()
	{
		return "Stamina regen enabled";
	}
}
