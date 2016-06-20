using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameEvent
{
	public abstract string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers);// {return null;}
	public virtual bool PreconditionsMet(MapRegion eventRegion, List<PartyMember> movedMembers) {return true;}
	public virtual bool AllowMapMove() {return true;}
	public virtual List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers) {return null;}
	public virtual string DoChoice(string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers) {return null;}
	public bool repeatable=false;
}

public struct EventChoice
{
	public string choiceTxt;
	public bool grayedOut;
	public EventChoice(string txt, bool gray)
	{
		choiceTxt=txt;
		grayedOut=gray;
	}
	public EventChoice(string txt)
	{
		choiceTxt=txt;
		grayedOut=false;
	}
}

public class NewSurvivor:GameEvent
{
	string eventDescription="You come across another survivor, who asks to join your group";
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) {return eventDescription;}
	
	public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<EventChoice> choicesList=new List<EventChoice>();
		choicesList.Add(new EventChoice("Accept them"));
		choicesList.Add(new EventChoice("Reject them"));
		return choicesList;
	}
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		
		switch (choiceString)
		{
			case"Accept them": 
			{
				PartyMember newGuy=new PartyMember();
				PartyManager.mainPartyManager.AddNewPartyMember(newGuy);//.partyMembers.Add (newGuy);
				break;
			}
			case"Reject them":
			{
				eventResult="The survivor leaves";
				break;
			}
		}
		return eventResult;
	}
}
/*
public class FoodSpoilage:GameEvent
{
	string eventDescription="You discover some of your perishable food is about to expire.\nYou can try to eat it now and risk food poisoning, or just throw it away";
	
	public override string GetDescription(MapRegion eventRegion) {return eventDescription;}
	
	public override bool PreconditionsMet ()
	{
		bool conditionsAreMet=false;
		int partyFoodCount=0;
		foreach (InventoryItem item in PartyManager.mainPartyManager.GetPartyInventory())
		{
			if (item.GetType()==typeof(FoodSmall) || item.GetType()==typeof(FoodBig)) {partyFoodCount+=1;}
			if (partyFoodCount>=2) {conditionsAreMet=true; break;}
		}
		return conditionsAreMet;
	}
	
	public override List<string> GetChoices(MapRegion eventRegion)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Eat it");
		choicesList.Add("Throw it out");
		return choicesList;
	}
	public override string DoChoice (string choiceString,MapRegion eventRegion)
	{
		string eventResult=null;
		
		switch (choiceString)
		{
		case"Eat it": 
			{
				//success
				if (Random.value>0.5f)
				{
					eventResult="Realizing you can't be picky at a time like this, the party sets out eating old eggs, bread, meat products and other persihables. Aside from some minor stomach gurgling, nobody suffers any ill effects and everyone is energized by the meal.\n\nFood is halved, everyone is fed";	
					foreach (PartyMember member in eventRegion.localPartyMembers)
					{
						member.SetHunger(0);
					}
					
					List<InventoryItem> foodItems=new List<InventoryItem>();
					foreach (InventoryItem item in PartyManager.mainPartyManager.GetPartyInventory())
					{
						if (item.GetType()==typeof(FoodBig) || item.GetType()==typeof(FoodSmall)) 
						{foodItems.Add(item as InventoryItem);}
					}
					int foodCount=Mathf.RoundToInt(foodItems.Count*0.5f);
					for (int i=0; i<foodCount; i++)
					{
						PartyManager.mainPartyManager.RemoveItems(foodItems[i]);
					}
				}
				else
				{
					
					eventResult="Eating unrefrigerated goods is a risky move, and eventually the party succumbs to food poisoning, ending up worse than they started\n\nFood is halved, -5 health for everyone";
					
					foreach (PartyMember member in eventRegion.localPartyMembers)
					{
						if (member.health-5>0) {member.health-=5;}
						else {member.health=1;}
						//PartyManager.mainPartyManager.foodSupply-=1;
					}
					//PartyManager.mainPartyManager.foodSupply=Mathf.RoundToInt(PartyManager.mainPartyManager.foodSupply*0.5f);
					List<InventoryItem> foodItems=new List<InventoryItem>();
					foreach (InventoryItem item in PartyManager.mainPartyManager.GetPartyInventory())
					{
						if (item.GetType()==typeof(FoodBig) || item.GetType()==typeof(FoodSmall)) 
						{foodItems.Add(item as InventoryItem);}
					}
					int foodCount=Mathf.RoundToInt(foodItems.Count*0.5f);
					for (int i=0; i<foodCount; i++)
					{
						PartyManager.mainPartyManager.RemoveItems(foodItems[i]);
					}
				}
				break;
			}
		case"Throw it out":
			{
				eventResult="Deciding not to risk it, you throw out the dodgy foods\n\n-50% perishable food";
				//PartyManager.mainPartyManager.foodSupply=Mathf.RoundToInt(PartyManager.mainPartyManager.foodSupply*0.5f);
				List<InventoryItem> foodItems=new List<InventoryItem>();
				foreach (InventoryItem item in PartyManager.mainPartyManager.GetPartyInventory())
				{
					if (item.GetType()==typeof(FoodBig) || item.GetType()==typeof(FoodSmall)) 
					{ foodItems.Add(item as InventoryItem);}
				}
				int foodCount=Mathf.RoundToInt(foodItems.Count*0.5f);
				for (int i=0; i<foodCount; i++)
				{
					PartyManager.mainPartyManager.RemoveItems(foodItems[i]);
				}
				break;
			}
		}
		return eventResult;
	}
}*/


/*
public class MonsterAttack:GameEvent
{
	string eventDescription="As your party moves, you start to hear suspicious noises close by.\nThey grow and shrink in intensity and change direction several times as you go, putting everyone on their guard.\nFinally, the source reveals itself: a monster!";
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) {return eventDescription;}
	public override List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Fight it");
		choicesList.Add("Try to run");
		//Check to see if party has ranged weapons
		bool partyHasGuns=false;
		foreach (PartyMember member in movedMembers) 
		{
			if (member.equippedRangedWeapon!=null) {partyHasGuns=true; break;}
		}
		if (PartyManager.mainPartyManager.ammo>0 && partyHasGuns) 
		{choicesList.Add("Shoot it");}
		return choicesList;
	}
		
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		int fightDamage=25;
		int runFatigue=20;
		int fightFatigue=-5;
		
		switch (choiceString)
		{
		case"Fight it": 
			{
				//success
				if (Random.value>0.5f)
				{
					eventResult="Seeing the creature so close, you decide it's too late to run.\nSurging with adrenaline, the party charges the monster with their weapons drawn! Surprised, the terrifying creature wilts under a rain of blows, dead\n\n"+runFatigue+" fatigue for everyone";
					foreach (PartyMember member in movedMembers)
					{
						member.ChangeFatigue(fightFatigue);//stamina-=1;
					}
				}
				else
				{
					PartyMember hurtMember=movedMembers[Random.Range(0,movedMembers.Count)];
					eventResult=hurtMember.name+" is the first to charge the creature, and bears the brunt of its ferocious attacks. The others run in to help and finish it off, but "+hurtMember.name+" is in a bad shape.\n\n-"+fightDamage+" health for "+hurtMember.name;
					//foreach (PartyMember member in eventRegion.localPartyMembers)
					//{
						int realDamage=fightDamage;
						if (hurtMember.health<realDamage) realDamage=Mathf.Max(hurtMember.health-1,1);
						hurtMember.TakeDamage(realDamage,false);
						//PartyManager.mainPartyManager.foodSupply-=1;
					//}
					//PartyManager.mainPartyManager.foodSupply=Mathf.RoundToInt(PartyManager.mainPartyManager.foodSupply*0.5f);
				}
				break;
			}
			case"Try to run":
			{
				eventResult="Surprised by the attack, the party turns tail. You run with your hearts in your ears, only stopping once you're about to keel over from exhaustion. You're not sure how long you ran, but the creature is nowhere in sight\n\n"+runFatigue+" fatigue for everyone";
				
				foreach (PartyMember member in movedMembers)
				{
					member.ChangeFatigue(runFatigue);
				}
				break;
			}
		
			case"Shoot it":
			{
				eventResult="You decide that saved ammo is of no use when you're dead, and the creature disappears in a hail of gunfire.\n\n-10 Ammo";
				PartyManager.mainPartyManager.ammo-=10;
				break;
			}
		}
		return eventResult;
	}
}*/
/*
public class LostInAnomaly:GameEvent
{
	string eventDescription="While making their way to the next region, the party turns a corner in a dark, enclosed archway.\nUpon walking out the other side, everyone realizes it's a completely different territory. You emerge into a couryard of tall buildings, impossibly dense and close to eachother.\nThis couldn't be the destination you were trying to reach";
	
	public override string GetDescription(MapRegion eventRegion) {return eventDescription;}
	public override bool AllowMapMove (){return false;}
	
	public override List<string> GetChoices(MapRegion eventRegion)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Press on");
		choicesList.Add("Try to retrace your steps");
		return choicesList;
	}
	
	public override string DoChoice (string choiceString, MapRegion eventRegion)
	{
		string eventResult=null;
		
		switch (choiceString)
		{
		case"Press on": 
		{
			//success
			eventResult="This couldn't possibly be the place you were trying to reach before. The layout doesn't make sense, someone would have noticed such tall buildings from further away. Whatever happened to you all in that archway, you decide not to tempt fate and explore this new area instead";
			MapManager mpManager=MapManager.main;
			mpManager.TeleportToRegion(mpManager.GetRegion(new Vector2(Random.Range(0,mpManager.mapWidth),Random.Range(0,mpManager.mapHeight))));
			break;
		}
		case"Try to retrace your steps":
		{
				if (Random.value<0.8f)
				{
					eventResult="You file back into the mysterious archway, and it seems to take several minutes longer than it did when you walked in the other direction. However, when you finally emerge, you find yourselves much further back, in the area you started from. Someone notices the archway behind you no longer rounds a corner...";
					PartyManager manager=PartyManager.mainPartyManager;
					MapManager.main.TeleportToRegion(MapManager.main.GetRegion(manager.mapCoordX,manager.mapCoordY));
				}
				else
				{
					eventResult="As you turn around and walk back through, the archway seems to take several more turns than it did last time, eventually terminating in a different region still. As you look back, the archway looks short and straight, and the area on the other side looks nothing like where you just came from. You decide not to take any more chances";
					MapManager mpManager=MapManager.main;
					mpManager.TeleportToRegion(mpManager.GetRegion(new Vector2(Random.Range(0,mpManager.mapWidth),Random.Range(0,mpManager.mapHeight))));
				}
			break;
		}
		}
		return eventResult;
	}
}*/

public class CacheInAnomaly:GameEvent
{	
	string eventDescription="";
	int fatiguePenalty=2;
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		string noticingPartyMemberName=movedMembers[Random.Range(0,movedMembers.Count)].name;
		eventDescription="As you explore the area, you come across a patch of road filled with empty cars.\nTheir roofs stand at waist height relative to you, the cars are piled together as if they all slid from their parking spots into an unseen sinkhole.\nBubbling water covers the entire area below them, obscuring the ground and emanating warm steam.\nAs you take in this strange sight,"+noticingPartyMemberName+" notices some food and ammo stashed in an open bed truck amid the cars";
		
		return eventDescription;
	}
	public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<EventChoice> choicesList=new List<EventChoice>();
		foreach (PartyMember member in movedMembers)
		{
			choicesList.Add(new EventChoice("Send "+member.name+" to navigate the car roofs and the supplies"));
		}
		choicesList.Add(new EventChoice("Leave it and move on"));
		return choicesList;
	}
	
	public override string DoChoice (string choiceString,MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;

		if (choiceString=="Leave it and move on")
		eventResult="You decide not to test whatever strange phenomenon caused this pileup, and leave the supplies. Better safe than sorry.";

		if (eventResult==null)
		{
			foreach (PartyMember member in movedMembers)
			{
				if (choiceString=="Send "+member.name+" to navigate the car roofs and the supplies")
				{
					if (Random.value<0.6f)
					{
						eventResult=member.name+" volunteers to try and move across. The roofs are moist and slippery, and "+member.name+" loses balance several times, but manages to recover and safely reaches the supplies. Throwing the bags over,"+member.name+" carefully makes it back across, covered in sweat but unharmed\n\nFood, Ammo\n\n"+fatiguePenalty+" fatigue for "+member.name;
						eventRegion.StashItem(new FoodSmall());
						eventRegion.StashItem(new FoodSmall());
						eventRegion.StashItem(new FoodBig());
						eventRegion.StashItem(new FoodBig());
						eventRegion.StashItem(new AmmoBox());
						//PartyManager.mainPartyManager.GainItems(new FoodSmall());
						//PartyManager.mainPartyManager.GainItems(new FoodBig());
						//PartyManager.mainPartyManager.GainItems(new FoodBig());
						//PartyManager.mainPartyManager.ammo+=10;
						member.ChangeFatigue(fatiguePenalty);
					}
					else
					{
						int damage=25;
						eventResult=member.name+" navigates the first few cars with surprising nimbleness and grace, but an inclined hatchback roof knocks him off-balance.\n"+member.name+" manages to grab an upper trunk of another nearby car at the last second, but his left foot dips into the drink all the way to the ankle. A terrible scream follows, but a surge of adrenaline causes "+member.name+" to pull himself up. With one foot badly boiled, he abandons the supplies, and somehow manages to crawl back to safety.\n\n";
						eventResult+=damage+" leg damage for "+member.name;
						//if (member.health<damage) {damage=member.health-1;}
						member.TakeDamage(damage,false,PartyMember.BodyPartTypes.Legs);
						member.ChangeFatigue(fatiguePenalty);
						//volunteer.ChangeFatigue(fatiguePenalty);
					}
					break;
				}
			}
		}

		return eventResult;
	}

	public CacheInAnomaly()
	{
		repeatable=false;
	}
}

public class MedicalCache:GameEvent
{	
	string eventDescription="";
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="Wandering down a street, you spot a notice glued to one of the lampposts. It calls for evacuation and lists the address of the nearest emergency service post.\n They are probably gone by now, but they may have left some supplies behind. You can spend time to try and find the address, or ignore it and move on";
		//GameManager.DebugPrint("Region coords is:"+eventRegion.GetCoords());
		return eventDescription;
	}
	public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<EventChoice> choicesList=new List<EventChoice>();
		choicesList.Add(new EventChoice("Try to find the address"));
		choicesList.Add(new EventChoice("Ignore it"));
		return choicesList;
	}
	
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		int fatigueChangeSuccess=2;
		int fatigueChangeFailure=2;
		
		switch (choiceString)
		{
		case"Try to find the address": 
			{
				//success
				if (Random.value<0.5f)
				{
					eventResult="After an hour of wandering through deserted streets and alleyways, you find the remains of the emeregency camp in an emptied out parking lot.\nIt looks like they left in a hurry: a few tents stand abandoned amid discarded cellophane bags and other refuse.\nSearching the tents, you discover forgotten medical supplies\n\n"+fatigueChangeSuccess+" fatigue to everyone\n\n 2 medkits  2 bandages gained";
					eventRegion.StashItem(new Medkit());
					eventRegion.StashItem(new Medkit());
					eventRegion.StashItem(new Bandages());
					eventRegion.StashItem(new Pills());
					eventRegion.StashItem(new Pills());
					foreach (PartyMember member in movedMembers)
					{
						member.ChangeFatigue(fatigueChangeSuccess);
					}
				}
				else
				{
					eventResult="You search the winding streets and alleyways.\n The layout of this area seems strangely twisted, the street signs hard to find.\nAfter runnning into several dead ends and a few passageways inexplicably blocked by random debris, you find yourself looping back to where you started and give the search up.\n\n"+fatigueChangeFailure+" fatigue to everyone";
					foreach (PartyMember member in movedMembers)
					{
						member.ChangeFatigue(fatigueChangeFailure);
					}
				}
				break;
			}
		case"Ignore it":
			{
				eventResult="Even if you can locate the emergency post, there's no guarantee anything useful is still there, so you decide to ignore it.";
				break;
			}
		}
		return eventResult;
	}

	public MedicalCache()
	{
		repeatable=false;
	}
}

public class SurvivorRescue:GameEvent
{	
	string eventDescription="";
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="You stumble across a survivor fighting for his life against an otherwolrdly creature. Occupied with their struggle, neither of them notice your party.\nThe survivor doesn't look like he can hold out much longer";
		return eventDescription;
	}
	public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<EventChoice> choicesList=new List<EventChoice>();
		choicesList.Add(new EventChoice("Charge in to help"));
		choicesList.Add(new EventChoice("Slip away while the creature is distracted"));
		return choicesList;
	}
	
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		int failPartyDamage=10;
		int newMemberHealthPenalty=20;
		int ignoreMoralePenalty=-20;
		
		switch (choiceString)
		{
		case"Charge in to help": 
			{
				//success
				if (Random.value<0.5f)
				{
					PartyMember newGuy=new PartyMember();
					PartyManager.mainPartyManager.AddNewPartyMember(newGuy);
					PartyMember.BodyPartTypes damagedPart=newGuy.TakeRandomPartDamage(newMemberHealthPenalty,true);
					eventResult="You charge the creature before it can deliver another blow to the distressed survivor. Taken by surprise, it quickly succumbs to your attacks.\nThe survivor is shaken, but alive.\n\n"+newGuy.name+" Joins your party."+"\n\n"+newGuy.name+" takes "+newMemberHealthPenalty+" "+damagedPart+" damage";

					//newGuy.health+=newMemberHealthPenalty;
				}
				else
				{
					
					eventResult="Before you can close the distance, the creature reacts and lunges at you!\nAmid a flurry of vicious attacks, you barely manage to fight it off with the help of the survivor.\n\n";
					//Order is
					PartyMember.BodyPartTypes partType;
					foreach (PartyMember member in movedMembers) 
					{
						partType=member.TakeRandomPartDamage(failPartyDamage,true);
						eventResult+=member.name+" takes "+failPartyDamage+" "+partType+" damage\n";
					}//member.health+=failPartyDamage;}
					//important
                    PartyMember newGuy = new PartyMember();
                    eventResult +="\n\n"+newGuy.name + " joins your party\n";
					PartyManager.mainPartyManager.AddNewPartyMember(newGuy);

                    partType = newGuy.TakeRandomPartDamage(newMemberHealthPenalty, true);
                    eventResult += newGuy.name + " takes " + newMemberHealthPenalty + " " + partType + " damage\n";
					//newGuy.health+=newMemberHealthPenalty;
				}
				break;
			}
		case"Slip away while the creature is distracted":
			{
				eventResult="You quickly pass the fight by and leave the survivor to his fate. Eventually, the sounds of struggle behind you turn to silence\n\n"+ignoreMoralePenalty+" morale for everyone";
				foreach (PartyMember member in movedMembers) {member.morale+=ignoreMoralePenalty;}
				break;
			}
		}
		return eventResult;
	}
    public SurvivorRescue()
    {
        repeatable = false;
    }
}

public class SearchForSurvivor:GameEvent
{	
	string eventDescription="";
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="Suddenly, your surroundings echo a voice calling out for survivors. It doesn't seem to react when you try to shout back, continuing to cry out in uneven intervals.\nIt sounds fairly distant, but you can just barely make out the direction to search";
		return eventDescription;
	}
	public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<EventChoice> choicesList=new List<EventChoice>();
		choicesList.Add(new EventChoice("Search for the voice"));
		choicesList.Add(new EventChoice("Ignore it and move on"));
		return choicesList;
	}
	
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		int ignoreMoralePenalty=10;
		int fatigueChangeFailure=1;
		
		switch (choiceString)
		{
		case"Search for the voice": 
			{
				//success
				if (Random.value<0.5f)
				{
					eventResult="The voice grows louder as you close in on the source, and eventually starts responding to your calls, quickly guiding you to another survivor\n\nA new survivor joins your party";
					PartyMember newGuy=new PartyMember();
					PartyManager.mainPartyManager.AddNewPartyMember(newGuy);
				}
				else
				{
					eventResult="You try to home in on the voice, but it seems to change direction and intensity several times, until, finally, it goes quiet.\n\n"+fatigueChangeFailure+" fatigue for everyone";
					foreach (PartyMember member in movedMembers)
					{
						member.ChangeFatigue(fatigueChangeFailure);
					}
				}
				break;
			}
		case"Ignore it and move on":
			{
				eventResult="You decide not to take any chances searching for strangers and move on.\n\n-"+ignoreMoralePenalty+" morale for everyone";
				foreach (PartyMember member in movedMembers) {member.morale-=ignoreMoralePenalty;}
				break;
			}
		}
		return eventResult;
	}
}
/*
public class LowMoraleSpiral:GameEvent
{	
	string eventDescription="";
	int moraleThreshold=30;
	int moralePenalty=-15;
	PartyMember affectedMember;

	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="As morale wavers, some people start to slide into hopelessness\n\n";
		PartyMember affectedMember=null;
		foreach (PartyMember member in movedMembers)
		{
			if (member.morale<=moraleThreshold) {affectedMember=member; break;}
		}
		eventDescription+=affectedMember.name+" is trying to hold it together, but you can see the fear in his eyes, his body language.\n Sometimes, one crack in the shell is all it takes...\n\n"+moralePenalty+" morale for "+affectedMember.name;
		affectedMember.morale+=moralePenalty;
		return eventDescription;
	}
	
	public override bool PreconditionsMet (MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		bool conditionsAreMet=false;
		foreach (PartyMember member in movedMembers)
		{
			if (member.morale<=moraleThreshold) {conditionsAreMet=true; break;}
		}
		return conditionsAreMet;
	}
}*/

public class LowMoraleSteal:GameEvent
{	
	string eventDescription="";
	int moraleThreshold=35;
	InventoryItem stolenItem=null;
	PartyMember inventoryMember=null;

	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="Something seems wrong during asset review";
		eventDescription+="\nSome items are missing. Stolen?";
		eventDescription+="\n\n-"+stolenItem.itemName+" is gone";

		//If an inventory member was found - take the item from that member's inventory, otherwise - take it from local region
		if (inventoryMember!=null) inventoryMember.carriedItems.Remove(stolenItem);
		else eventRegion.TakeStashItem(stolenItem);

		return eventDescription;
	}
	
	public override bool PreconditionsMet (MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		bool thiefMemberFound=false;
		foreach (PartyMember member in movedMembers)
		{
			if (stolenItem==null)
			{
				foreach (InventoryItem item in member.carriedItems)
				{
					if (item.GetType()==typeof(FoodBig) | item.GetType()==typeof(FoodSmall) | item.GetType()==typeof(FoodCooked))
					{
						stolenItem=item;
						inventoryMember=member;
						break;
					}
				}
			}
			if (member.morale<=moraleThreshold && member.isKleptomaniac) thiefMemberFound=true;
		}
		if (thiefMemberFound && stolenItem==null)
		{
			foreach (InventoryItem item in eventRegion.GetStashedItems())
			{
				if (item.GetType()==typeof(FoodBig) | item.GetType()==typeof(FoodSmall) | item.GetType()==typeof(FoodCooked))
				{
					stolenItem=item;
				}
			}
		}
		return (thiefMemberFound && stolenItem!=null);
	}
}

public class LowMoraleFight:GameEvent
{	
	string eventDescription="";
	int moraleThreshold=20;
	int healthPenalty=10;
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="A few terse words send pent up tension boiling over, sparking a fistfight between two survivors!\n";

		PartyMember angryMember=null;
		PartyMember normalMember=null;
		foreach (PartyMember member in movedMembers)
		{
			
			if ((member.morale<=moraleThreshold | member.isViolent) && angryMember==null) 
			{
				angryMember=member;
			}
			else normalMember=member;
			if (angryMember!=null && normalMember!=null) {break;}
		}
		if (angryMember==null | normalMember==null) {throw new System.Exception("Null members for LowMoraleFight!");}
		PartyMember.BodyPartTypes angryDamagedPart=angryMember.TakeRandomPartDamage(healthPenalty,true);
		PartyMember.BodyPartTypes normalDamagedPart=normalMember.TakeRandomPartDamage(healthPenalty,true);
		eventDescription+=angryMember.name+" and "+normalMember.name+" tear into eachother with frustrated viciousness!\nEventually the fight breaks up, but not before they could do some damage.\n\n"+healthPenalty+" "+angryDamagedPart+" damage for "+angryMember.name;
		eventDescription+="\n"+healthPenalty+" "+normalDamagedPart+" damage for "+normalMember.name;
		return eventDescription;
	}

	public override bool PreconditionsMet (MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		bool conditionsAreMet=false;
		if (movedMembers.Count>1)
		{
			foreach (PartyMember member in movedMembers)
			{
				if (member.morale<=moraleThreshold | member.isViolent) {conditionsAreMet=true; break;}
			}
		}
		return conditionsAreMet;
	}
}

public class LowMoraleQuit:GameEvent
{
	string eventDescription="";
	int moraleThreshold=10;
	PartyMember leavingMember;
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		PartyManager.mainPartyManager.RemovePartyMember(leavingMember,false);

		eventDescription="Fed up with the group, "+leavingMember.name+" decides to try his luck alone\n\n";
		eventDescription+=leavingMember.name+" leaves the party";
		return eventDescription;
	}
	public override bool PreconditionsMet (MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		bool conditionsAreMet=false;
		if (PartyManager.mainPartyManager.partyMembers.Count>1)
		{
			foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
			{
				if (member.morale<=moraleThreshold) 
				{
					leavingMember=member;
					conditionsAreMet=true; 
					break;
				}
			}
		}
		return conditionsAreMet;
	}
}

//RELATIONSHIP EVENTS
public class LowMoraleEnmity:GameEvent
{	
	string eventDescription="";
	PartyMember lowestModMember=null;
	PartyMember secondLowestModMember=null;

	public override bool PreconditionsMet (MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		bool conditionsAreMet=false;

		int i=0;
		float lowestTotalRelationshipMod=0f;
		while (i<movedMembers.Count)
		{
			if (movedMembers[i].GetCurrentRelationshipMod()<0)
			{
				PartyMember firstPickedMember=movedMembers[i];
				//float lowestMod=movedMembers[i].GetCurrentRelationshipMod();

				foreach (PartyMember member in movedMembers)
				{
					if (member!=firstPickedMember && !firstPickedMember.relationships.ContainsKey(member) && member.GetCurrentRelationshipMod()<0)
					{
						if (firstPickedMember.GetCurrentRelationshipMod()+member.GetCurrentRelationshipMod()<lowestTotalRelationshipMod) 
						{
							lowestModMember=firstPickedMember;
							secondLowestModMember=member;
							lowestTotalRelationshipMod=firstPickedMember.GetCurrentRelationshipMod()+member.GetCurrentRelationshipMod();
						}
					}
				}
			}
			i+=1; 
		}
		if (lowestModMember!=null && secondLowestModMember!=null && Random.value<=Mathf.Abs(lowestTotalRelationshipMod)) conditionsAreMet=true;

		return conditionsAreMet;
	}

	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		PartyMember angryMember=lowestModMember;
		PartyMember normalMember=secondLowestModMember;
		/*
		foreach (PartyMember member in movedMembers)
		{
			
			if (member.morale<=moraleThreshold && angryMember==null) 
			{
				angryMember=member;
			} 
			else normalMember=member;
			if (angryMember!=null && normalMember!=null) {break;}
		}
		if (angryMember==null | normalMember==null) {throw new System.Exception("Null members for LowMoraleEnmity!");}
		*/
		eventDescription="Tension and frustration in the group escalates in a shouting match between two survivors!\n";
		eventDescription+=angryMember.name+" and "+normalMember.name+"'s argument devolves into personal attacks, as both forget the original root of their disagreement.\n When the exchange of accusastions finally settles, "+normalMember.name+" seems livid with indignation.";
		eventDescription+="\n\n"+normalMember.name+" has a grudge against "+angryMember.name;
		//if (normalMember.relationships.ContainsKey(angryMember)) {normalMember.RemoveRelatonship(angryMember);}
		normalMember.SetRelationship(angryMember,Relationship.RelationTypes.Enemy);
		return eventDescription;
	}
}

public class HighMoraleFriendship:GameEvent
{	
	string eventDescription="";
	PartyMember highestModMember=null;
	PartyMember secondHighestModMember=null;

	public override bool PreconditionsMet (MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		bool conditionsAreMet=false;

		int i=0;
		float highestTotalRelationshipMod=0f;
		while (i<movedMembers.Count)
		{
			if (movedMembers[i].GetCurrentRelationshipMod()>0)
			{
				PartyMember firstPickedMember=movedMembers[i];
				//float lowestMod=movedMembers[i].GetCurrentRelationshipMod();

				foreach (PartyMember member in movedMembers)
				{
					if (member!=firstPickedMember && !firstPickedMember.relationships.ContainsKey(member) && member.GetCurrentRelationshipMod()>0)
					{
						if (firstPickedMember.GetCurrentRelationshipMod()+member.GetCurrentRelationshipMod()>highestTotalRelationshipMod) 
						{
							highestModMember=firstPickedMember;
							secondHighestModMember=member;
							highestTotalRelationshipMod=firstPickedMember.GetCurrentRelationshipMod()+member.GetCurrentRelationshipMod();
						}
					}
				}
			}
			i+=1; 
		}
		if (highestModMember!=null && secondHighestModMember!=null && Random.value<=Mathf.Abs(highestTotalRelationshipMod)) conditionsAreMet=true;

		return conditionsAreMet;
	}

	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="Survivors engage in friendly conversation to pass the time\n";
		eventDescription+=highestModMember.name+" and "+secondHighestModMember.name+" seem to hit it off especially well, bonding over similar points of view and sharing old hobbies.";
		eventDescription+="\n\n"+secondHighestModMember.name+" became friends with "+highestModMember.name;
		//if (normalMember.relationships.ContainsKey(angryMember)) {normalMember.RemoveRelatonship(angryMember);}
		secondHighestModMember.SetRelationship(highestModMember,Relationship.RelationTypes.Friend);
		return eventDescription;
	}
}

//PERSISTENT EVENTS
public abstract class PersistentEvent:GameEvent
{
	public abstract string GetTooltipDescription();
	public abstract string GetScoutingDescription();
	public abstract Sprite GetRegionSprite();
	public override bool PreconditionsMet(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		return true;
	}
	public bool eventCompleted=false;
}

public class GasolineEvent:PersistentEvent
{
    string eventDescription="";

    int fatigueRequirement=4;

    public GasolineEvent()
    {
        repeatable=true;
    }

    public override Sprite GetRegionSprite(){return SpriteBase.mainSpriteBase.gasStationSprite;}

    public override string GetTooltipDescription()
	{
		string ttDescription="Gas station";
		if (!eventCompleted)
		{
			ttDescription+="\n\nYou can spend "+fatigueRequirement+" fatigue here to scavenge some gas";
		}
		else ttDescription+="\n\nYou have salvaged the gas from the station";
		return ttDescription;
	}

    public override string GetScoutingDescription()
	{
		string scoutingDescription="";
		if (!eventCompleted)
		{
			scoutingDescription="There is an abandoned gas station here, quiet and still. No working cars left, but you bet you could find a way to siphon some gas.";
		}
		else scoutingDescription="A gas station with nothing left but some stray garbage";
		return scoutingDescription;
	}

    public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
    {
        string noticingPartyMemberName=movedMembers[Random.Range(0,movedMembers.Count)].name;
        eventDescription="The station seems (thankfully) empty of all life. A quick search of the booth";
        eventDescription+="\nturns up nothing of value, but the tanks don't sound empty when tapped.";
        eventDescription+="\nYou have seen some special tools in a nearby shed, but they will take";
        eventDescription+="\ntime and effort to set up";
        //eventDescription="The buildings in this block seem almost normal, save for the cold blue lights"; 
        //eventDescription+="\nflickering on and off in some of the windows. You can't make out anything inside";
        //eventDescription+="\nYour party sets to work searching for supplies.";

        
        return eventDescription;
    }
    public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        List<EventChoice> choicesList=new List<EventChoice>();
        foreach (PartyMember presentMember in movedMembers)
        {
        	if (presentMember.CheckEnoughFatigue(fatigueRequirement))
			choicesList.Add(new EventChoice("Have "+presentMember.name+" siphon (-"+fatigueRequirement+" fatigue)"));
        }
		choicesList.Add(new EventChoice("Leave"));
        return choicesList;
    }
    
    public override string DoChoice (string choiceString,MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        string eventResult=null;
        int gasCanisterCount=1;
        foreach (PartyMember presentMember in movedMembers)
        {
			if (choiceString=="Have "+presentMember.name+" siphon (-"+fatigueRequirement+" fatigue)")
			{
				eventResult=presentMember.name+" sets to work, dragging out tools and twisting off locks.";
	            eventResult+="\nThe sounds of metal on metal ring out trough the eerie silence. The work is dirty";
	            eventResult+="\nand moments tense, but only the cold wind stirs the nearby ruins.";
	            List<InventoryItem> scavengedItems=InventoryItem.GenerateMapSalvageLoot(InventoryItem.LootMetatypes.Salvage);
	            eventResult+="\n\n"+fatigueRequirement+" fatigue for "+presentMember.name+"\n"+gasCanisterCount+" gas canisters";

	            presentMember.ChangeFatigue(fatigueRequirement);
	            for (int i=0; i<gasCanisterCount; i++)
	            {
	                eventRegion.StashItem(new Gasoline());
	            }
	            eventCompleted=true;
				eventRegion.visible=true;
	            break;
	        }
        }
		if (choiceString=="Leave") 
		{
			eventResult="You decide you have bigger priorities right now.";
			return eventResult;

		}
        return eventResult;
    }
}



//TRADE EVENTS
public abstract class TradeEvent:PersistentEvent
{
    public Dictionary<PartyMember,List<InventoryItem>> foundMemberItems;
    public List<InventoryItem> foundAreaItems;
    public Dictionary<InventoryItem,int> rewardItems;

    public System.Type requiredItemType;
    public int requiredItemCount;

	public static bool TryFindRequiredItems(System.Type requiredItemType, int requiredCount,
	MapRegion eventRegion, List<PartyMember> memberList,
	out Dictionary<PartyMember,List<InventoryItem>> memberItems,out List<InventoryItem> areaItems)
    {
		memberItems=new Dictionary<PartyMember,List<InventoryItem>>();
    	areaItems=new List<InventoryItem>();
    	foreach (InventoryItem item in eventRegion.GetStashedItems())
    	{
    		if (item.GetType()==requiredItemType)
    		{
    			areaItems.Add(item);
    			requiredCount--;
    		}
    		if (requiredCount==0) break;
    	}
    	if (requiredCount>0)
    	{
    		foreach (PartyMember member in memberList)
    		{
    			List<InventoryItem> foundItems=new List<InventoryItem>();
				memberItems.Add(member,foundItems);
    			foreach (InventoryItem item in member.carriedItems)
    			{
    				if (item.GetType()==requiredItemType)
    				{
						foundItems.Add(item);
    					requiredCount--;
    				}
    				if (requiredCount==0) break;
    			}
    			if (requiredCount==0) break;
    		}
    	}
    	return (requiredCount==0);
    }
}
public class ScrapTrade:TradeEvent
{
	string eventDescription="";
	int fuelRewardCount=4;
	int foodRewardCount=4;

	public ScrapTrade()
    {
		rewardItems=new Dictionary<InventoryItem, int>();
    	float randomRoll=Random.value;
    	if (randomRoll>0.5f)
    	{
	        requiredItemType=typeof(Scrap);
	        requiredItemCount=3;
			rewardItems.Add(new Firewood(),fuelRewardCount);
		}
		else
		{
			requiredItemType=typeof(Pills);
	        requiredItemCount=3;
			rewardItems.Add(new FoodSmall(),foodRewardCount);
		}
		repeatable=false;
    }

	string GetResultDescription(System.Type requiredItem)
	{
		string eventResult="";

		if (requiredItem==typeof(Scrap))
		{
			eventResult="A tense-looking survivor hurriedly brings several handfuls of firewood out of the hideout.";
	        eventResult+="\nThe whole time, you can't shake the feeling of being watched from within.";
	        eventResult+="\nHe shakes your hand and briefly thanks you before running back, locking the door behind him.";
			
			eventResult+="\n\n"+fuelRewardCount+" firewood";
		}
		if (requiredItem==typeof(Pills))
		{
			eventResult="A tense-looking survivor hurriedly brings several handfuls of food out of the hideout.";
	        eventResult+="\nThe whole time, you can't shake the feeling of being watched from within.";
	        eventResult+="\nHe shakes your hand and briefly thanks you before running back, locking the door behind him.";

			eventResult+="\n\n"+foodRewardCount+" Junk Food";
		}
		return eventResult;
	}

    

    public override Sprite GetRegionSprite(){return SpriteBase.mainSpriteBase.gasStationSprite;}

    public override string GetTooltipDescription()
	{
		string ttDescription="Trade";
		if (!eventCompleted)
		{
			ttDescription+="\n\nSurvivors here will trade you";
			foreach (InventoryItem item in rewardItems.Keys)
			{
				ttDescription+=" "+rewardItems[item]+" "+item.itemName;
			}
			ttDescription+="\n for "+requiredItemCount+" "+requiredItemType;
		}
		else ttDescription+="\n\nThe survivors are gone from this hideout";
		return ttDescription;
	}

    public override string GetScoutingDescription()
	{
		string scoutingDescription="";
		if (!eventCompleted)
		{
			scoutingDescription="Survivors holed up here offer to trade you";
            foreach (InventoryItem item in rewardItems.Keys)
            {
                scoutingDescription += " " + rewardItems[item] + " " + item.itemName;
            }
            scoutingDescription += " for " + requiredItemCount + " " + requiredItemType;
			scoutingDescription+="\n";
			
		}
		else scoutingDescription="The hideout is trashed and empty. It looks like the survivors left, or got chased off.\nYou wonder what became of them.";
		return scoutingDescription;
	}

    public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
    {
        string noticingPartyMemberName=movedMembers[Random.Range(0,movedMembers.Count)].name;
        eventDescription="You find a barred-up apartment building that seems to have been converted into a safehouse.";
        eventDescription+="\nThe door is securely locked, but muffled voices are heard inside when you knock.";
        eventDescription+="\nFinally, just when you get ready to leave, a hoarse voice calls out to you and offers a trade.";

		eventDescription+="\n\n"+requiredItemCount+" "+requiredItemType+" for";
		eventDescription+="\n";
		foreach (InventoryItem item in rewardItems.Keys)
		{
			eventDescription+=" "+rewardItems[item]+" "+item.itemName;
		}
        //eventDescription="The buildings in this block seem almost normal, save for the cold blue lights"; 
        //eventDescription+="\nflickering on and off in some of the windows. You can't make out anything inside";
        //eventDescription+="\nYour party sets to work searching for supplies.";

        
        return eventDescription;
    }
    public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        List<EventChoice> choicesList=new List<EventChoice>();

        choicesList.Add(new EventChoice("Trade "+requiredItemCount+" "+requiredItemType
        ,!TryFindRequiredItems(requiredItemType,requiredItemCount,eventRegion,movedMembers,out foundMemberItems,out foundAreaItems)));
		choicesList.Add(new EventChoice("Leave"));
        return choicesList;
    }
    
    public override string DoChoice (string choiceString,MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        string eventResult=null;

			if (choiceString=="Trade "+requiredItemCount+" "+requiredItemType)
			{
				eventResult=GetResultDescription(requiredItemType);

	            foreach (PartyMember member in foundMemberItems.Keys)
	            {
	            	foreach (InventoryItem item in foundMemberItems[member])
	            	{
	            		member.RemoveCarriedItem(item);
	            	}
	            }
	            foreach (InventoryItem item in foundAreaItems)
	            {
	            	eventRegion.TakeStashItem(item);
	            }
	            foreach (InventoryItem item in rewardItems.Keys)
	            {
				for (int i=0; i<rewardItems[item]; i++) 
					{
						InventoryItem newRewardItem=System.Activator.CreateInstance(item.GetType()) as InventoryItem;
						eventRegion.StashItem(newRewardItem);
					}
	            }
	            eventCompleted=true;
	            eventRegion.visible=true;
	        }

		if (choiceString=="Leave") 
		{
			eventResult="You politely decline the trade and quickly leave.";
			return eventResult;
		}
        return eventResult;
    }
}

public class WoundedSurvivor:TradeEvent
{
    string eventDescription="";

	public WoundedSurvivor()
    {
		requiredItemType=typeof(Medkit);
        requiredItemCount=2;
        rewardItems=new Dictionary<InventoryItem, int>();
        repeatable=false;
    }

    public override Sprite GetRegionSprite(){return SpriteBase.mainSpriteBase.gasStationSprite;}

    public override string GetTooltipDescription()
	{
		string ttDescription="A badly wounded survivor is stuck here.";
		if (!eventCompleted)
		{
			ttDescription+="\n\nBring this person "+requiredItemCount+" "+requiredItemType+" to rescue them.";
		}
		else ttDescription+="\n\nThe former hideout of a wounded survivor.";
		return ttDescription;
	}

    public override string GetScoutingDescription()
	{
		string scoutingDescription="";
		if (!eventCompleted)
		{
			//scoutingDescription="A survivor is stuck in a dusty, twisted up nook of an empty building. He's trying to stay quiet, but his eyes light up with hope";
			//scoutingDescription+=" as you approach.";
			scoutingDescription="You hear a nearby noise as you explore one of the empty buildings. You prepare for a fight, but then the noise is followed by";
			scoutingDescription+=" muffled swearing. You follow the sounds, cautiously, to a dark corner with a survivor is sitting up against a wall.";
			scoutingDescription+=" He seems unable to move, his legs badly wounded.";
		}
		else scoutingDescription="The corner where the survivor was stranded is only filled with dust and rubble now.";
		return scoutingDescription;
	}

    public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
    {
        string noticingPartyMemberName=movedMembers[Random.Range(0,movedMembers.Count)].name;
        eventDescription="The survivor's eyes light up as you approach him.";
        eventDescription+="\nHe whispers, begging you to help him.";
        //eventDescription="The buildings in this block seem almost normal, save for the cold blue lights"; 
        //eventDescription+="\nflickering on and off in some of the windows. You can't make out anything inside";
        //eventDescription+="\nYour party sets to work searching for supplies.";

        
        return eventDescription;
    }
    public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        List<EventChoice> choicesList=new List<EventChoice>();
        foreach (PartyMember presentMember in movedMembers)
        {
			choicesList.Add(new EventChoice("Use "+requiredItemCount+" "+requiredItemType+" to help the survivor"
			,!TryFindRequiredItems(requiredItemType,requiredItemCount,eventRegion,movedMembers,out foundMemberItems, out foundAreaItems)));
        }
		choicesList.Add(new EventChoice("Leave"));
        return choicesList;
    }
    
    public override string DoChoice (string choiceString,MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        string eventResult=null;
		if (choiceString=="Use "+requiredItemCount+" "+requiredItemType+" to help the survivor")
		{
			PartyMember newMember=new PartyMember();
			PartyManager.mainPartyManager.AddNewPartyMember(newMember);
			newMember.TakeDamage(14,false,PartyMember.BodyPartTypes.Legs);



			foreach (InventoryItem item in foundAreaItems)
			{
				eventRegion.TakeStashItem(item);
			}
			foreach (PartyMember member in foundMemberItems.Keys)
			{
				foreach (InventoryItem item in foundMemberItems[member]) member.RemoveCarriedItem(item);
			}

			eventResult="You patch up the survivor as best you can. His legs are still badly hurt, but he manages to stagger back to his feet.";
			eventResult+="\nHe thanks you and introduces himself: "+newMember.name;
	        eventResult+="\n\n"+newMember.name+" joins the group.";
	        eventResult+="\n -"+requiredItemCount+" "+requiredItemType;

	        eventCompleted=true;
			eventRegion.visible=true;
	    }
		if (choiceString=="Leave") 
		{
			eventResult="You don't have medical supplies to spare, and have to leave the survivor to his fate. You can feel his desperate stare on your back";
			eventResult+=" as you walk away.";
			return eventResult;
		}
        return eventResult;
    }
}

//!-INSTRUMENTAL EVENTS-!
//EXPLORE EVENTS
public class TownMove:GameEvent
{
	string eventDescription="";
	int gasCost=MapManager.townToTownGasCost;
	int fatiguePenaltyCost=10;
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="You decide it's time to move on to the next town";
		eventDescription+="\n\n";
		if (PartyManager.mainPartyManager.gas>=gasCost && movedMembers[0].currentRegion.hasCar)
		eventDescription+="The car can carry all the items in the town center to the next town";
		eventDescription+="\n\n(Party members outside the town center will be abandoned)";
		return eventDescription;
	}
	
	public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<EventChoice> choicesList=new List<EventChoice>();
		bool carGreyedOut=true;
		if (PartyManager.mainPartyManager.gas>=gasCost && movedMembers[0].currentRegion.hasCar) carGreyedOut=false; 
		choicesList.Add(new EventChoice("Take the car (-"+gasCost+" gas)",carGreyedOut));
		choicesList.Add(new EventChoice("Go on foot ("+fatiguePenaltyCost+" fatigue for everyone)"));
		choicesList.Add(new EventChoice("Cancel"));
		return choicesList;
	}
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		
		if (choiceString=="Take the car (-"+gasCost+" gas)") 
		{
				//success
				eventResult="You load up your meager supplies, refill the tank and begin driving down the silent, empty road";
				PartyManager.mainPartyManager.gas-=gasCost;
				//Abandon members left behind
				foreach (PartyMember member in new List<PartyMember>(PartyManager.mainPartyManager.partyMembers))
				{
					if (!movedMembers.Contains(member)) PartyManager.mainPartyManager.RemovePartyMember(member);
				}
				//Remove car from previous region
				MapRegion startingRegion=movedMembers[0].currentRegion;
				startingRegion.SetCar(false);
				foreach (InventoryItem item in new List<InventoryItem>(startingRegion.GetStashedItems()))
				{
					startingRegion.TakeStashItem(item);
					eventRegion.StashItem(item);
				}
				//Put car in the next region
				MapManager.main.MoveMembersToRegion(eventRegion,movedMembers.ToArray());
				eventRegion.SetCar(true);
				return eventResult;
		}
		if (choiceString=="Go on foot ("+fatiguePenaltyCost+" fatigue for everyone)") 
		{
			//success
			eventResult="You take only what you can carry, and prepare for a long, gruelling walk to the next town";
			foreach (PartyMember member in new List<PartyMember>(PartyManager.mainPartyManager.partyMembers))
			{
				if (!movedMembers.Contains(member)) PartyManager.mainPartyManager.RemovePartyMember(member);
				else member.SetFatigue(fatiguePenaltyCost);
			}
			MapManager.main.MoveMembersToRegion(eventRegion,movedMembers.ToArray());
			return eventResult;
		}
		if (choiceString=="Cancel") 
		{
			//success
			eventResult="You decide to stay awhile longer before braving the road";
			return eventResult;

		}
		return eventResult;
	}
	
}



public class AmbushEvent:GameEvent
{
    const int requiredAmmo=5;
    int fightHealthPenaltyMin=10;
    int fightHealthPenaltyMax=15;
    int stayBehindPenaltyMin=20;
    int stayBehindPenaltyMax=35;
    PartyMember shooter=null;

    public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
    {
        string desc="";
        if (movedMembers.Count==1) desc=movedMembers[0].name+" has been ambushed by monsters!";
        else
        {
            for (int i=0; i<movedMembers.Count-1; i++)
            {
                if (i>0) desc+=", ";
                desc+=movedMembers[i].name;
            }
            desc+=" and "+movedMembers[movedMembers.Count-1].name+" have been ambushed by monsters!";
        }
        desc+="\n\n What should they do?";
        return desc;
    }
    public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        List<EventChoice> choicesList=new List<EventChoice>();
        //choicesList.Add("Try to run!");
		choicesList.Add(new EventChoice("Fight!"));
		bool shootGrayedOut=true;
        foreach (PartyMember member in movedMembers)
        {
			if (movedMembers.Count>1) 
			choicesList.Add(new EventChoice("Have "+member.name+" hold them off!"));

            if (member.equippedRangedWeapon!=null && PartyManager.mainPartyManager.ammo>=requiredAmmo)
            {
            	shootGrayedOut=false;
                shooter=member;
            }
        }
		choicesList.Add(new EventChoice("Shoot them!("+requiredAmmo+" ammo)",shootGrayedOut)); 
        return choicesList;
    }
    
    public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        string eventResult=null;
        /*
        switch (choiceString)
        {
            case"Fight!":
            {
                
                break;
            }
            case"Shoot them!("+requiredAmmo+" ammo)": 
            {
                eventResult=shooter.name+" decides that saved ammo is of no use to the dead and opens fire! The abominations fall one by one, screams and gunshots ringing through the warped, empty streets. This time, everyone is spared\n\n-"+requiredAmmo+" Ammo";
                PartyManager.mainPartyManager.ammo-=requiredAmmo;
                break;
            }
        }*/
        if (choiceString=="Fight!")
        {
            int fightHealthPenalty=Random.Range(fightHealthPenaltyMin,fightHealthPenaltyMax+1);
            eventResult="With the creatures so close, it's too late to run.\nSurging with adrenaline, the party charges the monsters with weapons drawn! The violence is brief, but brutal\n";
            foreach (PartyMember member in movedMembers)
            {
            	PartyMember.BodyPartTypes damagedPart=member.TakeRandomPartDamage(fightHealthPenalty,true);
            	eventResult+="\n"+fightHealthPenalty+" "+damagedPart+" damage to "+member.name;
            }
        }
        if (choiceString=="Shoot them!("+requiredAmmo+" ammo)")
        {
            eventResult=shooter.name+" decides that saved ammo is of no use to the dead and opens fire! The abominations fall one by one, screams and gunshots ringing through the warped, empty streets. This time, everyone is spared\n\n-"+requiredAmmo+" Ammo";
            PartyManager.mainPartyManager.ammo-=requiredAmmo;
        }
        
        foreach (PartyMember member in movedMembers)
        {
            if (choiceString=="Have "+member.name+" hold them off!")
            {
                int stayBehindHealthPenalty=Random.Range(stayBehindPenaltyMin,stayBehindPenaltyMax+1);
                string partyPronounTwo="others";
                string partyPronoun="companions";
                if (movedMembers.Count==2) 
                {   
                    partyPronounTwo="other";
                    partyPronoun="companion";
                }
                eventResult=member.name+" hesitates for a moment, before rushing past "+partyPronoun+" and into danger. The sounds of struggle left behind spur on the "+partyPronounTwo+" as they run, wondering if "+member.name+" will make it...";
				PartyMember.BodyPartTypes damagedPart=member.TakeRandomPartDamage(stayBehindHealthPenalty,true);
                eventResult+="\n\n"+stayBehindHealthPenalty+" "+damagedPart+" health for "+member.name;

                break;
            }
        }
        return eventResult;
    }
}

public class CleanupEvent:GameEvent
{
    const int requiredAmmo=5;
    int stayBehindPenaltyMin=18;
    int stayBehindPenaltyMax=25;
    int requiredTrapsCount=1;
    int trapAmbientThreatReduction=1;

    int fatigueRequired=2;

	Dictionary<InventoryItem,PartyMember> usedMemberTraps=new Dictionary<InventoryItem,PartyMember>();
	List<InventoryItem> usedLocationTraps=new List<InventoryItem>();

    public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
    {
        string desc="You decide to try and make the area safer to scavenge.";
        desc+="\n\n In order to do that, you...";
        return desc;
    }
    public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        List<EventChoice> choicesList=new List<EventChoice>();
        //choicesList.Add("Try to run!");
        bool trapsGreyedOut=true;
        bool fatigueOptionGreyed=true;
        //Try to find traps in member inventory, and check fatigue
		foreach (PartyMember member in movedMembers)
	    {
	    	//Find traps
	        foreach (InventoryItem item in member.carriedItems)
	        {
				if (item.GetType()==typeof(SettableTrap)) usedMemberTraps.Add(item,member);
				if (usedMemberTraps.Count+usedLocationTraps.Count==requiredTrapsCount) 
	            {
					trapsGreyedOut=false;
	                break;
	            }   
	        }
	        //Find fatigue options
			choicesList.Add(new EventChoice("Have "+member.name+" find safer routes ("+fatigueRequired+" fatigue)",!member.CheckEnoughFatigue(fatigueRequired)));
	    }
      	
        if (trapsGreyedOut)
        {
			//Try to find enough traps in region inventory
	        foreach(InventoryItem item in eventRegion.GetStashedItems())
	        {
				if (item.GetType()==typeof(SettableTrap)) usedLocationTraps.Add(item);
				if (usedLocationTraps.Count==requiredTrapsCount)
				{
					trapsGreyedOut=false;
					break;
				}
	        }
        }
		choicesList.Add(new EventChoice("...Trap the area (-"+requiredTrapsCount+" traps)",trapsGreyedOut));

		choicesList.Add(new EventChoice("Cancel"));
        return choicesList;
    }
    
    public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        string eventResult=null;

		if (choiceString=="...Trap the area (-"+requiredTrapsCount+" traps)")
        {
			eventResult="You get to work setting a series of deadly traps in tight spaces, luring dangerous creatures in one by one.\n\nAmbush threat reduced.";
			foreach (InventoryItem trap in usedLocationTraps) {eventRegion.TakeStashItem(trap);}
			foreach (InventoryItem trap in usedMemberTraps.Keys) {usedMemberTraps[trap].RemoveCarriedItem(trap);}
            eventRegion.ambientThreatNumber-=1;
        }

		if (choiceString=="Cancel") 
		{
			//success
			eventResult="You decide not to brave the area right now.";
			return eventResult;
		}

        if (eventResult==null)
        {
	        foreach (PartyMember member in movedMembers)
	        {
				if (choiceString=="Have "+member.name+" find safer routes ("+fatigueRequired+" fatigue)")
				{
					eventResult=member.name+" scouts new paths through the ruined streets, finding ways around large groups of monsters and odd anomalies.";
					eventResult+="\n\n-"+fatigueRequired+" for "+member.name+", exploration threat lowered.";
					member.ChangeFatigue(fatigueRequired);
					eventRegion.ambientThreatNumber-=1;
					break;
				}
	        }
		}
        return eventResult;
    }
}

//CAMP EVENTS
//COLD EVENTS
//This fires if a camp is simply Cold
public class MemberIsCold:GameEvent
{	
	string eventDescription="";
	int healthPenalty=15;
	PartyMember coldMember=null;
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		coldMember=movedMembers[Random.Range(0,movedMembers.Count)];
		//success
		//PartyMember.BodyPartTypes damagedPart=coldMember.TakeRandomPartDamage(healthPenalty,true);
		PartyManager.mainPartyManager.AddPartyMemberStatusEffect(coldMember,new Cold());

		eventDescription="The air is cold and crisp. You can see your own breath. Life is slowly leaking out of your body.\n";
		eventDescription+="Due to low temperatures and a lack of heating, "+coldMember.name+" freezes and becomes sick!\n\n";//+healthPenalty+" "+damagedPart+" damage for "+coldMember.name;
		return eventDescription;
	}

	public override bool PreconditionsMet (MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		bool conditionsAreMet=false;
		switch (eventRegion.localTemperature)
		{
			case MapRegion.TemperatureRating.Cold: {conditionsAreMet=true; break;}
		}
		return conditionsAreMet;
	}
}
//This fires if a camp is Freezing
public class MembersAreFreezing:GameEvent
{	
	string eventDescription="";
	int healthPenalty=15;
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		//coldMember=movedMembers[Random(0,movedMembers.Count)];
		eventDescription="Your nostrils stick to the septum when you inhale. The air is so cold it's hard to breathe. It cuts deep into the bones.\n";
		eventDescription="The freezing cold of the camp slowly drains life from all present.\n";

		foreach (PartyMember member in movedMembers)
		{
			//PartyMember.BodyPartTypes damagedPartType=member.TakeRandomPartDamage(healthPenalty,false);
			eventDescription+="\n"+member.name+" becomes sick!";//+" takes "+healthPenalty+" "+damagedPartType+" damage";
			PartyManager.mainPartyManager.AddPartyMemberStatusEffect(member,new Cold());
		}

		return eventDescription;
	}
	public override bool PreconditionsMet (MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		bool conditionsAreMet=false;
		switch (eventRegion.localTemperature)
		{
			case MapRegion.TemperatureRating.Freezing: {conditionsAreMet=true; break;}
		}
		return conditionsAreMet;
	}
}

//ATTACK EVENTS
public class AttackOnCamp:GameEvent
{
    const int requiredAmmo=5;
    int fightHealthPenaltyMin=10;
    int fightHealthPenaltyMax=15;
    int stayBehindPenaltyMin=20;
    int stayBehindPenaltyMax=35;
    PartyMember shooter=null;

    public override bool PreconditionsMet(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		//The cold events use a different way of handling probability
		float triggerProbability=0;
		switch (eventRegion.GetCampingThreat())
		{
			case MapRegion.ThreatLevels.High: {triggerProbability=0.9f; break;}
			case MapRegion.ThreatLevels.Medium: {triggerProbability=0.6f; break;}
			case MapRegion.ThreatLevels.Low: {triggerProbability=0.3f; break;}
		}
		if (Random.value<=triggerProbability) return true;
		else return false;
	}

    public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
    {
        string desc="";
        for (int i=0; i<movedMembers.Count; i++)
        {
        	if (i<movedMembers.Count-1) desc+=movedMembers[i].name+",";
        	else 
        	{
        		if (i>0) desc+=" and ";
        		desc+=movedMembers[i].name+"'s";
        	}
        }
        desc+=" camp is attacked by monsters!";
        return desc;
    }
    public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        List<EventChoice> choicesList=new List<EventChoice>();
        //choicesList.Add("Try to run!");
		choicesList.Add(new EventChoice("Fight!"));
		//bool shootGreyedOut=true;

		bool partyHasRangedWeapons=false;
        foreach (PartyMember member in movedMembers)
        {
			if (movedMembers.Count>1) choicesList.Add(new EventChoice("Have "+member.name+" hold them off!"));
			if (member.equippedRangedWeapon!=null && PartyManager.mainPartyManager.ammo>=requiredAmmo)
            {
				partyHasRangedWeapons=true;
                shooter=member;
            }
            //If party member does not have a ranged weapon equipped, search his inventory
            if (!partyHasRangedWeapons)
            {
            	foreach (InventoryItem item in member.carriedItems) 
            	{
            		if (item.GetType().BaseType==typeof(RangedWeapon)) 
            		{
            			partyHasRangedWeapons=true;
            			shooter=member;
            		}
            	}
            }
        }
        //If none of the members have ranged weapons equipped or carrying them, check local inventory
        if (!partyHasRangedWeapons)
        {
        	foreach (InventoryItem item in eventRegion.GetStashedItems())
        	{
				if (item.GetType().BaseType==typeof(RangedWeapon)) 
            	{
            		partyHasRangedWeapons=true;
            		shooter=movedMembers[0];
            	}
            }
        }
		choicesList.Add(new EventChoice("Shoot them!("+requiredAmmo+" ammo)",!(partyHasRangedWeapons && PartyManager.mainPartyManager.ammo>=requiredAmmo)));
        return choicesList;
    }



    public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        string eventResult=null;
        /*
        switch (choiceString)
        {
            case"Fight!":
            {
                
                break;
            }
            case"Shoot them!("+requiredAmmo+" ammo)": 
            {
                eventResult=shooter.name+" decides that saved ammo is of no use to the dead and opens fire! The abominations fall one by one, screams and gunshots ringing through the warped, empty streets. This time, everyone is spared\n\n-"+requiredAmmo+" Ammo";
                PartyManager.mainPartyManager.ammo-=requiredAmmo;
                break;
            }
        }*/
		if (choiceString=="Fight!")
        {
            int fightHealthPenalty=Random.Range(fightHealthPenaltyMin,fightHealthPenaltyMax+1);
            eventResult="With the creatures so close, it's too late to run.\nSurging with adrenaline, the party charges the monsters with weapons drawn! The violence is brief, but brutal\n";
            foreach (PartyMember member in movedMembers)
            {
            	PartyMember.BodyPartTypes damagedPart=member.TakeRandomPartDamage(fightHealthPenalty,true);
            	eventResult+="\n"+fightHealthPenalty+" "+damagedPart+" damage to "+member.name;
            }
        }
        if (choiceString=="Shoot them!("+requiredAmmo+" ammo)")
        {
            eventResult=shooter.name+" decides that saved ammo is of no use to the dead and opens fire! The abominations fall one by one, screams and gunshots ringing through the warped, empty streets. This time, everyone is spared\n\n-"+requiredAmmo+" Ammo";
            PartyManager.mainPartyManager.ammo-=requiredAmmo;
        }
        
        foreach (PartyMember member in movedMembers)
        {
            if (choiceString=="Have "+member.name+" hold them off!")
            {
                int stayBehindHealthPenalty=Random.Range(stayBehindPenaltyMin,stayBehindPenaltyMax+1);
                string partyPronounTwo="others";
                string partyPronoun="companions";
                if (movedMembers.Count==2) 
                {   
                    partyPronounTwo="other";
                    partyPronoun="companion";
                }
                eventResult=member.name+" hesitates for a moment, before rushing past "+partyPronoun+" and into danger. The sounds of struggle left behind spur on the "+partyPronounTwo+" as they run, wondering if "+member.name+" will make it...";
				PartyMember.BodyPartTypes damagedPart=member.TakeRandomPartDamage(stayBehindHealthPenalty,true);
                eventResult+="\n\n"+stayBehindHealthPenalty+" "+damagedPart+" health for "+member.name;

                break;
            }
        }
        return eventResult;
    }
}

//SCAVENGE EVENTS
public class ScavengeEventOne:GameEvent
{
    string eventDescription="";
    
    public ScavengeEventOne()
    {
        repeatable=true;
    }
    
    public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
    {
        string noticingPartyMemberName=movedMembers[Random.Range(0,movedMembers.Count)].name;
        eventDescription="As your party scouts, the streets wind, clash and loop on themselves. Some buildings"; 
        eventDescription+="\nare mashed together and conjoined. Others are bisected apart with impossible";
        eventDescription+="\nprecision, interiors displayed in neat rows.";
        eventDescription+="\nYour party sets to work searching for supplies.";
        
        return eventDescription;
    }
    public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        List<EventChoice> choicesList=new List<EventChoice>();
		choicesList.Add(new EventChoice("Scavenge"));
        return choicesList;
    }
    
    public override string DoChoice (string choiceString,MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        string eventResult=null;
        int gasCanisterCount=1;
        
        switch (choiceString)
        {
            case"Scavenge":
            {
                eventResult="Safer-looking ruins are inspected, rubble cleared and plywood pried off, as you loot";
                eventResult+="\namid distant echoes of otherworldly sounds.\nA safe search yields a modest bounty";
                List<InventoryItem> scavengedItems=InventoryItem.GenerateMapSalvageLoot(InventoryItem.LootMetatypes.Salvage);
                eventResult+="\n\n";
                bool firstAdded=true;
                foreach (InventoryItem item in scavengedItems)
                {
                    if (!firstAdded) eventResult+=", ";
                    eventResult+=item.itemName;
                    eventRegion.StashItem(item);
                    firstAdded=false;
                }
                break;
            }
        }
        return eventResult;
    }
}


public class CarFindEvent:GameEvent
{
    string eventDescription="";
    
    public CarFindEvent()
    {
        repeatable=true;
    }
    
    public override bool PreconditionsMet (MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        bool allowEvent=true;
        //if (eventRegion.hasGasoline) allowEvent=false;
        //else 
        {
            if (eventRegion.townCenter==null)
            {
                if (eventRegion.hasCar) allowEvent=false;
            }
            else if (eventRegion.townCenter.hasCar) allowEvent=false;
        }
        return allowEvent;
    }
    
    public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
    {
        string noticingPartyMemberName=movedMembers[Random.Range(0,movedMembers.Count)].name;
        eventDescription="The buildings in this block seem almost normal, save for the cold blue lights"; 
        eventDescription+="\nflickering on and off in some of the windows. You can't make out anything inside";
        eventDescription+="\nYour party sets to work searching for supplies.";
        
        return eventDescription;
    }
    public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        List<EventChoice> choicesList=new List<EventChoice>();
		choicesList.Add(new EventChoice("Scavenge"));
        return choicesList;
    }
    
    public override string DoChoice (string choiceString,MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        string eventResult=null;
        int gasCanisterCount=1;
        
        switch (choiceString)
        {
        case"Scavenge":
            {
                eventResult="The few cars that still remain on warped streets are in various states of disrepair,";
                eventResult+="\nbut you get lucky and stumble on one that's still functioning, with barely enough gas.";
                eventResult+="\nin the tank to bring it back to the town's center.";
                
                if (eventRegion.townCenter==null) eventRegion.SetCar(true);
                else eventRegion.townCenter.SetCar(true);   
                break;
            }
        }
        return eventResult;
    }
}

public class GameWinEvent:GameEvent
{
    string eventDescription="";

    public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
    {
        string noticingPartyMemberName=movedMembers[Random.Range(0,movedMembers.Count)].name;
        eventDescription="As the night of the seventh day gives way to the first crack of dawn, the week ends."; 
        eventDescription+="\nYou look at the skyline tensely, wondering if something might have happened to the";
        eventDescription+="\nscheduled rescue effort. You've fought hard to survive that long, and you're not sure";
        eventDescription+="\nyou have any more fight left in you.";

		eventDescription="Finally, a small dot appears on the horizon, slowly morphing into a transport chopper as it goes.";
		eventDescription+="\nSeveral more soon follow, one breaking off in your direction while the others fly on past.\n";
		eventDescription+="\nYou awaken from your reverie to run towards it, as it goes in for landing a few streets away from your camp.";
		eventDescription+="\nGunfire rings out in the morning air as you run, machineguns securing the area from enemies roused by the noise.";
		eventDescription+="\nThey almost take your head off before they notice you're not a hostile. Soldiers in armor and protective gear";
		eventDescription+="\ndisembark into the action, a few covering while two rush towards you, roughly grabbing you and heading back";
		eventDescription+="\nto their helicopter. You are put on a bench and left alone, listening to the gunfire outside while a few other";
		eventDescription+="\nsurvivors are dragged in, and the chopper finally takes off. You dully watch the island flash past the";
		eventDescription+="\nwindows, wondering where you are being taken.";

        return eventDescription;
    }
    public override List<EventChoice> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        List<EventChoice> choicesList=new List<EventChoice>();
		choicesList.Add(new EventChoice("Continue"));
        return choicesList;
    }
    
    public override string DoChoice (string choiceString,MapRegion eventRegion, List<PartyMember> movedMembers)
    {
        string eventResult=null;
        
        switch (choiceString)
        {
        	case"Continue":
            {
                eventResult="";
                GameManager.main.EndCurrentGame(true);
				EventCanvasHandler.main.CloseChoiceScreen();
                break;
            }
        }
        return eventResult;
    }
}