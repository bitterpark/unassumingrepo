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
	int hoursDuration=2;
	int hoursPassed=0;
	int bleedDmg=10;
	
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
		PartyManager.ETimePassed-=TimePassEffect;
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
		PartyManager.ETimePassed+=TimePassEffect;
		canStack=true;
	}
}

public class BrokenArmsMember:StatusEffect
{
	string _name="Broken arms";
	//int damageModDelta=-40;
	float hitChanceChange=0.4f;
	public BrokenArmsMember (PartyMember member) 
	{
		//affectedPartyMemberIndex=affectedMemberIndex;
		affectedMember=member;
		//affectedMember.meleeDamageMod+=damageModDelta;
		affectedMember.baseAttackHitChance-=hitChanceChange;
		affectedMember.morale-=15;
		//PartyManager.TimePassed+=TimePassEffect;
		//canStack=true;
	}
	
	public override string effectName 
	{
		get {return _name;}
	}
	public override Sprite effectSprite 
	{
		get {return SpriteBase.mainSpriteBase.brokenArmsSprite;}
	}
	//int affectedPartyMemberIndex;
	PartyMember affectedMember;
	
	public void CureArms()
	{
		//PartyStatusCanvasHandler.main.NewNotification(affectedMember.name+" has stopped bleeding");
		PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(affectedMember,this);
		//affectedMember.meleeDamageMod-=damageModDelta;
		affectedMember.baseAttackHitChance+=hitChanceChange;
		affectedMember.morale+=15;
	}
	
	
	public override string GetMouseoverDescription ()
	{
		return _name+"\n Hit chande reduced by "+hitChanceChange;//"\n Damage dealt reduced by "+damageModDelta;
	}
}

public class BrokenLegsMember:StatusEffect
{
	string _name="Broken legs";
	public BrokenLegsMember (PartyMember member) 
	{
		//affectedPartyMemberIndex=affectedMemberIndex;
		affectedMember=member;
		affectedMember.legsBroken=true;
		affectedMember.morale-=15;
		//PartyManager.TimePassed+=TimePassEffect;
		//canStack=true;
	}
	
	public override string effectName 
	{
		get {return _name;}
	}
	public override Sprite effectSprite 
	{
		get {return SpriteBase.mainSpriteBase.brokenLegsSprite;}
	}
	//int affectedPartyMemberIndex;
	PartyMember affectedMember;
	
	public void CureLegs()
	{
		//PartyStatusCanvasHandler.main.NewNotification(affectedMember.name+" has stopped bleeding");
		PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(affectedMember,this);
		affectedMember.legsBroken=false;
		affectedMember.morale+=15;
	}

	
	public override string GetMouseoverDescription ()
	{
		return _name+"\n Reduced move speed";
	}
}

//Enemy effects

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

public class NoLegs:StatusEffect
{
	int movementReduction=1;
	
	string _name="Broken legs";
	public override string effectName 
	{
		get {return _name;}
	}
	public override Sprite effectSprite 
	{
		get {return SpriteBase.mainSpriteBase.brokenLegsSprite;}
	}
	
	public override string GetMouseoverDescription ()
	{
		return _name+"\nReduced movement";
	}
	
	public NoLegs(EncounterEnemy affectedEnemy)
	{
		affectedEnemy.movesPerTurn-=movementReduction;
	}
}

public class NoArms:StatusEffect
{
	int staminaDamageReduction=1;
	float damageModMultiplier=0.5f;
	
	string _name="Broken arms";
	public override string effectName 
	{
		get {return _name;}
	}
	public override Sprite effectSprite 
	{
		get {return SpriteBase.mainSpriteBase.brokenArmsSprite;}
	}
	
	public override string GetMouseoverDescription ()
	{
		return _name+"\nReduced attack power";
	}
	
	public NoArms(EncounterEnemy affectedEnemy)
	{
		affectedEnemy.staminaDamage-=staminaDamageReduction;
		affectedEnemy.damageMod*=damageModMultiplier;
	}
}