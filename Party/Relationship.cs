using UnityEngine;
using System.Collections;

public class Relationship 
{
	PartyMember relationSubject;
	RelationTypes relationType;
	
	public enum RelationTypes {Enemy,Friend};
	
	int moraleModifierStrength=20;
	
	public int OnMissionTogether()
	{
		int moraleModifier=0;
		if (relationType==RelationTypes.Friend) {moraleModifier=moraleModifierStrength;}
		if (relationType==RelationTypes.Enemy) {moraleModifier=-moraleModifierStrength;}
		return moraleModifier;
	}
	
	public string GetText() 
	{
		string text="";
		if (relationType==RelationTypes.Enemy) 
		{
			text=relationSubject.name+":Grudging";
		}
		if (relationType==RelationTypes.Friend) 
		{
			text=relationSubject.name+":Friendly";
		}
		return text;
	}
	
	public Relationship(PartyMember member, RelationTypes type)
	{
		relationSubject=member;
		relationType=type;
	}
	
}
