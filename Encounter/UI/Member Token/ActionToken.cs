using UnityEngine;
using System.Collections;

public class ActionToken:MemberStatusToken
{
	protected override string GenerateTooltipText ()
	{
		return "Action available";
	}
}
