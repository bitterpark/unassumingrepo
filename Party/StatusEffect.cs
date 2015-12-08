using UnityEngine;
using System.Collections;

public abstract class StatusEffect 
{
	public abstract Sprite effectSprite {get;}
	public abstract string effectName {get;}
	
	public abstract string GetMouseoverDescription();
	public virtual void TurnOverEffect() {}
	public virtual void TimePassEffect(int hoursPassed) {}
	public virtual void CleanupEffect() {}
	public virtual void StackEffect() {}
	public bool canStack=false;
}

public class Bleed:StatusEffect
{
	string _name="Bleeding";
	public override string effectName 
	{
		get {return _name;}
	}
	public override Sprite effectSprite 
	{
		get {return SpriteBase.mainSpriteBase.bleedSprite;}
	}
	//int affectedPartyMemberIndex;
	PartyMember affectedMember;
	int hoursDuration=1;
	int hoursPassed=0;
	int bleedDmg=5;
	
	public override void TimePassEffect(int timePassed)
	{
		for (int i=0; i<timePassed; i++)
		{
			//affectedPartyMember.health-=bleedDmg*timePassed;
			//PartyManager.mainPartyManager.DamagePartyMember(affectedMember,bleedDmg);
			affectedMember.TakeDamage(bleedDmg,false);
			hoursPassed+=1;
			if (hoursPassed==hoursDuration) 
			{
				//PartyManager.TimePassed-=TimePassEffect;
				//PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(affectedMember,this);
				CureBleed();
				break;
			}
		}
	}
	
	public void CureBleed()
	{
		PartyStatusCanvasHandler.main.NewNotification(affectedMember.name+" has stopped bleeding");
		PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(affectedMember,this);
		CleanupEffect();
	}
	
	public override void CleanupEffect()
	{
		PartyManager.TimePassed-=TimePassEffect;
	}
	
	public override void StackEffect ()
	{
		hoursDuration+=hoursDuration;
	}
	
	public override string GetMouseoverDescription ()
	{
		return _name+"\n-"+bleedDmg+" health every hour\nDuration:"+(hoursDuration-hoursPassed)+" hours";
	}
	
	public Bleed (PartyMember member) 
	{
		//affectedPartyMemberIndex=affectedMemberIndex;
		affectedMember=member;
		PartyManager.TimePassed+=TimePassEffect;
		canStack=true;
	}
}

public class PhasedOut:StatusEffect
{
	string _name="Phased out";
	public override string effectName 
	{
		get {return _name;}
	}
	public override Sprite effectSprite 
	{
		get {return SpriteBase.mainSpriteBase.phasedOutSprite;}
	}
	
	public override string GetMouseoverDescription ()
	{
		return _name+"\nCannot be harmed";
	}
}