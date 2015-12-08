using UnityEngine;
using System.Collections;

public class DefenceToken:MemberStatusToken
{
	protected override string GenerateTooltipText ()
	{
		return "Defence enabled";
	}
}
