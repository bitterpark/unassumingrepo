using UnityEngine;
using System.Collections;

public class PartyMemberSelector : MonoBehaviour {

	public PartyMember member;
	
	void OnMouseDown()
	{
		PartyScreenManager.mainPSManager.MemberClicked(member);
	}
}
