using UnityEngine;
using System.Collections;

public class MoveToken:MemberStatusToken
{
	protected override string GenerateTooltipText ()
	{
		return "Move available";
	}
}
