using UnityEngine;
using System.Collections;


public interface IStatusEffectTokenInfo
{
	Sprite GetEffectSprite();
	string GetEffectName();
	string GetMouseoverDescription();
}

public abstract class MemberStatusEffect: IStatusEffectTokenInfo
{
	protected Sprite effectSprite;
	protected string effectName;

	#region IEffectTokenInfo implementation
	public Sprite GetEffectSprite() {return effectSprite;}
	public string GetEffectName() {return effectName;}
	public abstract string GetMouseoverDescription();
	#endregion

	public virtual void TimePassEffect() {}
	public virtual void ActivateEffect(PartyMember member) {}
	public virtual void CleanupEffect() {}
	public virtual void StackEffect() {}
	public bool canStack=false;
}

//MEMBER EFFECTS
public class Bleed:MemberStatusEffect
{
	
	//int affectedPartyMemberIndex;
	PartyMember affectedMember;
	int roundsDuration=5;
	//int hoursPassed=0;
	const int bleedDmg=1;
    int currentDmgPerRound;

	public override void TimePassEffect()
	{

		affectedMember.TakeDamage(bleedDmg,false);
		roundsDuration--;
		if (roundsDuration<=0) 
		{
			CureBleed();
		}
	}

	public override void ActivateEffect(PartyMember member)
	{
		affectedMember=member;
		//EncounterCanvasHandler.main.EMe
		//PartyManager.ETimePassed+=TimePassEffect;
		EncounterCanvasHandler.main.ERoundIsOver+=TimePassEffect;
		EncounterCanvasHandler.main.EEncounterExited+=EncounterExitEffect;
	}

	public void CureBleed()
	{
		if (PartyManager.mainPartyManager.partyMembers.Contains(affectedMember))
		{
			//PartyStatusCanvasHandler.main.NewNotification(affectedMember.name+" has stopped bleeding");
			PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(affectedMember,this);
		}
	}
	
	public override void CleanupEffect()
	{
		EncounterCanvasHandler.main.ERoundIsOver-=TimePassEffect;
		EncounterCanvasHandler.main.EEncounterExited-=EncounterExitEffect;
	}
	
	public override void StackEffect ()
	{
		roundsDuration+=roundsDuration;
        currentDmgPerRound += bleedDmg;
	}
	
	public override string GetMouseoverDescription ()
	{
		return effectName+"\n-"+bleedDmg+" vitals every turn\nDuration:"+roundsDuration;
	}

	public void EncounterExitEffect(PartyMember exitingMember)
	{
		if (affectedMember==exitingMember) while (roundsDuration>0) TimePassEffect();
	}
	
	public Bleed () 
	{
		effectName="Bleeding";
		effectSprite=SpriteBase.mainSpriteBase.bleedSprite;
		canStack=true;
        currentDmgPerRound = bleedDmg;
	}
}

public class Cold:MemberStatusEffect
{
	int maxFatiguePenalty=4;
	int sickMoralePenalty=10;
	int cureMoraleBonus=5;
	float cureChancePerCycle=0.25f;

	//int damageModDelta=-40;


	public override void ActivateEffect(PartyMember member)
	{
		affectedMember=member;
		//affectedMember.meleeDamageMod+=damageModDelta;
		affectedMember.fatigueRestoreSleep-=maxFatiguePenalty;
		affectedMember.fatigueRestoreWait-=maxFatiguePenalty;
		//GameManager.DebugPrint("Cold activated!");
		affectedMember.morale-=sickMoralePenalty;
		PartyManager.ETimePassed+=TimePassEffect;
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
		affectedMember.morale+=cureMoraleBonus;
	}

	public override void TimePassEffect()
	{

		if (Random.value<=cureChancePerCycle) 
		{
			CureCold();
		}
	}

	public override void CleanupEffect()
	{
		PartyManager.ETimePassed-=TimePassEffect;
	}
	public override void StackEffect ()
	{
		affectedMember.morale-=sickMoralePenalty;
	}
	
	
	public override string GetMouseoverDescription ()
	{
		return effectName+"\nFatigue restore reduced by "+maxFatiguePenalty;//"\n Damage dealt reduced by "+damageModDelta;
	}

	public Cold () 
	{
		effectName="Cold";
		effectSprite=SpriteBase.mainSpriteBase.coldSprite;
		canStack=true;
	}
}

public class BrokenArmsMember:MemberStatusEffect
{
	int craftFatiguePenalty=1;

	//int damageModDelta=-40;
	float hitChanceChange=0.4f;
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
		return effectName+"\n Hit chande reduced by "+hitChanceChange+"\nCrafting fatigue cost increased by "+craftFatiguePenalty;//"\n Damage dealt reduced by "+damageModDelta;
	}

	public BrokenArmsMember()
	{
		effectName="Broken arms";
		effectSprite=SpriteBase.mainSpriteBase.brokenArmsSprite;
	}
}

public class BrokenLegsMember:MemberStatusEffect
{
	int fatigueMovePenalty=1;
	float dodgeChanceMult=0.5f;

	//int affectedPartyMemberIndex;
	PartyMember affectedMember;

	public override void ActivateEffect(PartyMember member)
	{
		affectedMember=member;
		//affectedMember.legsBroken=true;
		affectedMember.SetDodgeMultiplier(dodgeChanceMult);
		affectedMember.morale-=15;
		affectedMember.currentFatigueMoveModifier+=fatigueMovePenalty;
	}

	public void CureLegs()
	{
		//PartyStatusCanvasHandler.main.NewNotification(affectedMember.name+" has stopped bleeding");
		PartyManager.mainPartyManager.RemovePartyMemberStatusEffect(affectedMember,this);
		//affectedMember.legsBroken=false;
        affectedMember.SetDodgeMultiplier(1);
		affectedMember.morale+=15;
		affectedMember.currentFatigueMoveModifier-=fatigueMovePenalty;
	}

	
	public override string GetMouseoverDescription ()
	{
		return effectName+"\n Dodge chance halved\nMap movement cost increased by "+fatigueMovePenalty;
	}

	public BrokenLegsMember()
	{
		effectName="Broken legs";
		effectSprite=SpriteBase.mainSpriteBase.brokenLegsSprite;
	}
}

//ENEMY EFFECTS
public abstract class EnemyStatusEffect:IStatusEffectTokenInfo
{
	protected Sprite effectSprite;
	protected string effectName;
	#region IEffectTokenInfo implementation
	public Sprite GetEffectSprite() {return effectSprite;}
	public string GetEffectName() {return effectName;}
	public abstract string GetMouseoverDescription();
	#endregion
}

public class PhasedOut:EnemyStatusEffect
{
	
	public override string GetMouseoverDescription ()
	{
		return effectName+"\nCannot be harmed";
	}
	public PhasedOut()
	{
		effectName="Phased out";
		effectSprite=SpriteBase.mainSpriteBase.phasedOutSprite;
	}
}

public class NoLegs:EnemyStatusEffect
{
	//int movementReduction=1;
	float dodgeChanceMult=0.5f;
	
	public override string GetMouseoverDescription ()
	{
		return effectName+"\nReduced dodge";
	}
	
	public NoLegs(EncounterEnemy affectedEnemy)
	{
		effectName="Broken legs";
		effectSprite=SpriteBase.mainSpriteBase.brokenLegsSprite;
		affectedEnemy.body.SetNewDodgeChance(affectedEnemy.body.dodgeChance*dodgeChanceMult);
	}
}

public class NoArms:EnemyStatusEffect
{
	int staminaDamageReduction=1;
	float damageModMultiplier=0.5f;
	
	public override string GetMouseoverDescription ()
	{
		return effectName+"\nDamage halved";
	}
	
	public NoArms(EncounterEnemy affectedEnemy)
	{
		effectName="Broken arms";
		effectSprite=SpriteBase.mainSpriteBase.brokenArmsSprite;
		
		affectedEnemy.staminaDamage-=staminaDamageReduction;
		affectedEnemy.damageMult=damageModMultiplier;
	}
}