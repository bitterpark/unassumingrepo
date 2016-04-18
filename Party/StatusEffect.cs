using UnityEngine;
using System.Collections;

public abstract class StatusEffect 
{
	public abstract Sprite effectSprite {get;}
	public abstract string effectName {get;}
	
	public abstract string GetMouseoverDescription();
	public virtual void TurnOverEffect() {}
	public virtual void TimePassEffect(int hoursPassed) {}
	public virtual void ActivateEffect(PartyMember member) {}
	public virtual void CleanupEffect() {}
	public virtual void StackEffect() {}
	public bool canStack=false;
}

//MEMBER EFFECTS
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

	public override void ActivateEffect(PartyMember member)
	{
		affectedMember=member;
		PartyManager.ETimePassed+=TimePassEffect;
	}

	public void CureBleed()
	{
		PartyStatusCanvasHandler.main.NewNotification(affectedMember.name+" has stopped bleeding");
		PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(affectedMember,this);
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
		return _name+"\n-"+bleedDmg+" vitals every rest\nDuration:"+(hoursDuration-hoursPassed);
	}
	
	public Bleed () 
	{
		//affectedPartyMemberIndex=affectedMemberIndex;
		canStack=true;
	}
}

public class Cold:StatusEffect
{
	string _name="Cold";
	int maxFatiguePenalty=3;
	float cureChancePerHour=0.25f;

	//int damageModDelta=-40;


	public override void ActivateEffect(PartyMember member)
	{
		affectedMember=member;
		//affectedMember.meleeDamageMod+=damageModDelta;
		affectedMember.fatigueRestoreSleep-=maxFatiguePenalty;
		affectedMember.fatigueRestoreWait-=maxFatiguePenalty;
		//GameManager.DebugPrint("Cold activated!");
		affectedMember.morale-=15;
		PartyManager.ETimePassed+=TimePassEffect;
	}

	public override string effectName 
	{
		get {return _name;}
	}
	public override Sprite effectSprite 
	{
		get {return SpriteBase.mainSpriteBase.coldSprite;}
	}
	//int affectedPartyMemberIndex;
	PartyMember affectedMember;


	public void CureCold()
	{
		//PartyStatusCanvasHandler.main.NewNotification(affectedMember.name+" has stopped bleeding");
		PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(affectedMember,this);
		//affectedMember.meleeDamageMod-=damageModDelta;
		affectedMember.fatigueRestoreSleep+=maxFatiguePenalty;
		affectedMember.fatigueRestoreWait+=maxFatiguePenalty;
		affectedMember.morale+=15;
	}

	public override void TimePassEffect(int timePassed)
	{
		for (int i=0; i<timePassed; i++)
		{
			if (Random.value<=cureChancePerHour) 
			{
				//PartyManager.TimePassed-=TimePassEffect;
				//PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(affectedMember,this);
				CureCold();
				break;
			}
		}
	}

	public override void CleanupEffect()
	{
		PartyManager.ETimePassed-=TimePassEffect;
	}
	public override void StackEffect ()
	{
		affectedMember.morale-=10;
	}
	
	
	public override string GetMouseoverDescription ()
	{
		return _name+"\nFatigue restore reduced by "+maxFatiguePenalty;//"\n Damage dealt reduced by "+damageModDelta;
	}

	public Cold () 
	{
		canStack=true;
	}
}

public class BrokenArmsMember:StatusEffect
{
	string _name="Broken arms";
	int craftFatiguePenalty=1;

	//int damageModDelta=-40;
	float hitChanceChange=0.4f;

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

	public override void ActivateEffect(PartyMember member)
	{
		affectedMember=member;
		//affectedMember.meleeDamageMod+=damageModDelta;
		affectedMember.meleeHitchanceMod-=hitChanceChange;
		affectedMember.morale-=15;
		affectedMember.currentFatigueCraftPenalty=craftFatiguePenalty;
	}

	public void CureArms()
	{
		//PartyStatusCanvasHandler.main.NewNotification(affectedMember.name+" has stopped bleeding");
		PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(affectedMember,this);
		//affectedMember.meleeDamageMod-=damageModDelta;
		affectedMember.meleeHitchanceMod+=hitChanceChange;
		affectedMember.morale+=15;
		affectedMember.currentFatigueCraftPenalty=0;
	}
	
	
	public override string GetMouseoverDescription ()
	{
		return _name+"\n Hit chande reduced by "+hitChanceChange+"\nCrafting fatigue cost increased by "+craftFatiguePenalty;//"\n Damage dealt reduced by "+damageModDelta;
	}
}

public class BrokenLegsMember:StatusEffect
{
	string _name="Broken legs";
	int fatigueMovePenalty=1;

	
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

	public override void ActivateEffect(PartyMember member)
	{
		affectedMember=member;
		affectedMember.legsBroken=true;
		affectedMember.morale-=15;
		affectedMember.currentFatigueMovePenalty=fatigueMovePenalty;
	}

	public void CureLegs()
	{
		//PartyStatusCanvasHandler.main.NewNotification(affectedMember.name+" has stopped bleeding");
		PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(affectedMember,this);
		affectedMember.legsBroken=false;
		affectedMember.morale+=15;
		affectedMember.currentFatigueMovePenalty=0;
	}

	
	public override string GetMouseoverDescription ()
	{
		return _name+"\n Reduced move speed\nMap movement cost increased by "+fatigueMovePenalty;
	}
}

//ENEMY EFFECTS
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