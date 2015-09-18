using UnityEngine;
using System.Collections;

public abstract class Perk
{
	public abstract string GetName();
	public abstract void ActivatePerk(PartyMember member);
	public abstract string GetMouseoverDescription();
}

public class Tough:Perk
{
	int healthUpgrade=25;
	string name="Toughness";
	
	public override string GetName (){return name;}
	public override void ActivatePerk(PartyMember member){member.maxHealth+=healthUpgrade;}
	public override string GetMouseoverDescription (){return "This character is tough as nails\n+"+healthUpgrade+" max health";}
}

public class Endurance:Perk
{
	int staminaUpgrade=5;
	string name="Endurance";
	
	public override string GetName (){return name;}
	public override void ActivatePerk(PartyMember member){member.maxStamina+=staminaUpgrade;}
	public override string GetMouseoverDescription (){return "This character lasts longer\n+"+staminaUpgrade+" max stamina";}
}

public class LeanEater:Perk
{
	int hungerReduction=2;
	string name="Lean Eater";
	
	public override string GetName () {return name;}
	public override void ActivatePerk(PartyMember member) {member.hungerIncreasePerHour-=hungerReduction;}
	public override string GetMouseoverDescription () {return "This character needs less food\n+"+hungerReduction+" hunger reduction";}
}
