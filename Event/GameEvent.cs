using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameEvent
{
	public abstract string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers);// {return null;}
	public virtual bool PreconditionsMet(MapRegion eventRegion, List<PartyMember> movedMembers) {return true;}
	public virtual bool AllowMapMove() {return true;}
	public abstract List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers);// {return null;}
	public abstract string DoChoice(string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers);// {return null;}
}

public class NewSurvivor:GameEvent
{
	string eventDescription="You come across another survivor, who asks to join your group";
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) {return eventDescription;}
	

	public override List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Accept them");
		choicesList.Add("Reject them");
		return choicesList;
	}
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		
		switch (choiceString)
		{
			case"Accept them": 
			{
				PartyMember newGuy=new PartyMember(eventRegion);
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

public class AmbushEvent:GameEvent
{
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
		return desc;
	}
	public override List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Continue");
		return choicesList;
	}
	
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		
		switch (choiceString)
		{
			case"Continue": 
			{
				int minHealthLoss=3;
				int maxHealthLoss=10;
				int actualHealthLoss=Random.Range(minHealthLoss,maxHealthLoss+1);
				if (movedMembers.Count==1) eventResult=movedMembers[0].name+" loses "+actualHealthLoss+" health";
				else  eventResult="Everyone loses "+actualHealthLoss+" health";
				foreach (PartyMember member in movedMembers){member.TakeDamage(actualHealthLoss,false);}//.health-=actualHealthLoss;}
				break;
			}
		}
		return eventResult;
	}
}

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
}
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
	int fatiguePenalty=10;
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		string noticingPartyMemberName=movedMembers[Random.Range(0,movedMembers.Count)].name;
		eventDescription="As you explore the area, you come across a patch of road filled with empty cars.\nTheir roofs stand at waist height relative to you, the cars are piled together as if they all slid from their parking spots into an unseen sinkhole.\nBubbling water covers the entire area below them, obscuring the ground and emanating warm steam.\nAs you take in this strange sight,"+noticingPartyMemberName+" notices some food and ammo stashed in an open bed truck amid the cars";
		
		return eventDescription;
	}
	public override List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Try to navigate the car roofs to reach the supplies");
		choicesList.Add("Leave it and move on");
		return choicesList;
	}
	
	public override string DoChoice (string choiceString,MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		
		switch (choiceString)
		{
		case"Try to navigate the car roofs to reach the supplies": 
			{
				PartyMember volunteer=movedMembers[Random.Range(0,movedMembers.Count)];
				//success
				if (Random.value<0.5f)
				{
					eventResult=volunteer.name+" volunteers to try and move across. The roofs are moist and slippery, and "+volunteer.name+" loses balance several times, but manages to recover and safely reaches the supplies. Loaded with bags,"+volunteer.name+" somehow makes it back across, covered in sweat but unharmed\n\n+2 food, +10 ammo\n\n"+fatiguePenalty+" fatigue for "+volunteer.name;
					eventRegion.StashItem(new FoodSmall());
					eventRegion.StashItem(new FoodBig());
					eventRegion.StashItem(new FoodBig());
					//PartyManager.mainPartyManager.GainItems(new FoodSmall());
					//PartyManager.mainPartyManager.GainItems(new FoodBig());
					//PartyManager.mainPartyManager.GainItems(new FoodBig());
					PartyManager.mainPartyManager.ammo+=10;
					volunteer.ChangeFatigue(fatiguePenalty);
				}
				else
				{
					int damage=25;
					eventResult=volunteer.name+" navigates the first few cars with surprising nimbleness and grace, but an inclined hatchback roof knocks them off-balance.\n"+volunteer.name+" manages to grab an upper trunk of another nearby car at the last second, but their left foot dips into the drink all the way to their ankle. A terrible scream follows, but a surge of adrenaline causes "+volunteer.name+" to pull themselves up. With one foot badly boiled, they abandon the supplies, and somehow manage to crawl back to safety.\n\n-"+damage+" health for "+volunteer.name;
					if (volunteer.health<damage) {damage=volunteer.health-1;}
					volunteer.TakeDamage(damage,false);
					//volunteer.ChangeFatigue(fatiguePenalty);
				}
				break;
			}
		case"Leave it and move on":
			{
				eventResult="You decide not to expose yourselves to whatever strange phenomenon caused this pileup, and leave the supplies. Better safe than sorry.";
				break;
			}
		}
		return eventResult;
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
	public override List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Try to find the address");
		choicesList.Add("Ignore it");
		return choicesList;
	}
	
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		int fatigueChangeSuccess=5;
		int fatigueChangeFailure=20;
		
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
					eventRegion.StashItem(new Bandages());
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
}

public class SurvivorRescue:GameEvent
{	
	string eventDescription="";
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="You stumble across a survivor fighting for his life against an otherwolrdly creature. Occupied with their struggle, neither of them notice your party.\nThe survivor doesn't look like he can hold out much longer";
		return eventDescription;
	}
	public override List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Charge in to help");
		choicesList.Add("Slip away while the creature is distracted");
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
					PartyMember newGuy=new PartyMember(eventRegion);
					PartyManager.mainPartyManager.AddNewPartyMember(newGuy);
					eventResult="You charge the creature before it can deliver another blow to the distressed survivor. Taken by surprise, it quickly succumbs to your attacks.\nThe survivor is shaken, but alive.\n\n"+newGuy.name+" Joins your party."+"\n\n"+newGuy.name+" takes "+newMemberHealthPenalty+" damage";
					newGuy.TakeDamage(newMemberHealthPenalty,false);
					//newGuy.health+=newMemberHealthPenalty;
				}
				else
				{
					PartyMember newGuy=new PartyMember(eventRegion);
					eventResult="Before you can close the distance, the creature reacts and lunges at you!\nAmid a flurry of vicious attacks, you barely manage to fight it off with the help of the survivor.\n\n"+newGuy.name+" joins your party\n\nEveryone takes "+failPartyDamage+" damage";
					//Order is
					foreach (PartyMember member in movedMembers) {member.TakeDamage(failPartyDamage,false);}//member.health+=failPartyDamage;}
					//important
					PartyManager.mainPartyManager.AddNewPartyMember(newGuy);
					newGuy.TakeDamage(failPartyDamage,false);
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
}

public class SearchForSurvivor:GameEvent
{	
	string eventDescription="";
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="Suddenly, your surroundings echo a voice calling out for survivors. It doesn't seem to react when you try to shout back, continuing to cry out in uneven intervals.\nIt sounds fairly distant, but you can just barely make out the direction to search";
		return eventDescription;
	}
	public override List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Search for the voice");
		choicesList.Add("Ignore it and move on");
		return choicesList;
	}
	
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		int ignoreMoralePenalty=10;
		int fatigueChangeFailure=10;
		
		switch (choiceString)
		{
		case"Search for the voice": 
			{
				//success
				if (Random.value<0.5f)
				{
					eventResult="The voice grows louder as you close in on the source, and eventually starts responding to your calls, quickly guiding you to another survivor\n\nA new survivor joins your party";
					PartyMember newGuy=new PartyMember(eventRegion);
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

public class LowMoraleSpiral:GameEvent
{	
	string eventDescription="";
	int moraleThreshold=30;
	int moralePenalty=-15;
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="As morale wavers, some people start to slide into hopelessness";
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
	
	public override List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Continue");
		return choicesList;
	}
	
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		PartyMember affectedMember=null;
		foreach (PartyMember member in movedMembers)
		{
			if (member.morale<=moraleThreshold) {affectedMember=member; break;}
		}
		if (affectedMember==null) {throw new System.Exception("No low morale member found for MoraleSpiral");}
		
		switch (choiceString)
		{
		case"Continue": 
			{
				//success
				eventResult=affectedMember.name+" is trying to hold it together, but you can see the fear in his eyes, his body language.\n Sometimes, one crack in the shell is all it takes...\n\n"+moralePenalty+" morale for "+affectedMember.name;
				affectedMember.morale+=moralePenalty;
				break;
			}
		}
		return eventResult;
	}
}

public class LowMoraleFight:GameEvent
{	
	string eventDescription="";
	int moraleThreshold=20;
	int healthPenalty=10;
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="A few terse words send pent up tension boiling over, sparking a fistfight between two survivors!";
		return eventDescription;
	}
	public override bool PreconditionsMet (MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		bool conditionsAreMet=false;
		if (movedMembers.Count>1)
		{
			foreach (PartyMember member in movedMembers)
			{
				if (member.morale<=moraleThreshold) {conditionsAreMet=true; break;}
			}
		}
		return conditionsAreMet;
	}
	
	public override List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Continue");
		return choicesList;
	}
	
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		PartyMember angryMember=null;
		PartyMember normalMember=null;
		foreach (PartyMember member in movedMembers)
		{
			
			if (member.morale<=moraleThreshold && angryMember==null) 
			{
				angryMember=member;
			}
			else normalMember=member;
			if (angryMember!=null && normalMember!=null) {break;}
		}
		if (angryMember==null | normalMember==null) {throw new System.Exception("Null members for LowMoraleFight!");}
		
		switch (choiceString)
		{
		case"Continue": 
			{
				//success
				eventResult=angryMember.name+" and "+normalMember.name+" tear into eachother with desperate, frustrated viciousness!\nEventually the fight gets broken up, but not before they could do some damage.\n\n"+healthPenalty+" health for "+angryMember.name+" and "+normalMember.name;
				//angryMember.health+=healthPenalty;
				//normalMember.health+=healthPenalty;
				angryMember.TakeDamage(healthPenalty,false);
				normalMember.TakeDamage(healthPenalty,false);
				break;
			}
		}
		return eventResult;
	}
}

public class LowMoraleEnmity:GameEvent
{	
	string eventDescription="";
	int moraleThreshold=15;
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="Tension and frustration in the group escalates in a shouting match between two survivors!";
		return eventDescription;
	}
	public override bool PreconditionsMet (MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		bool conditionsAreMet=false;
		if (movedMembers.Count>1)
		{
			foreach (PartyMember member in movedMembers)
			{
				if (member.morale<=moraleThreshold) {conditionsAreMet=true; break;}
			}
		}
		return conditionsAreMet;
	}
	
	public override List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Continue");
		return choicesList;
	}
	
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		PartyMember angryMember=null;
		PartyMember normalMember=null;
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
		
		switch (choiceString)
		{
		case"Continue": 
			{
				//success
				eventResult=angryMember.name+" and "+normalMember.name+"'s argument devolves into personal attacks, as both forget the original root of their disagreement.\n When the exchange of accusastions finally settles, "+normalMember.name+" seems livid with indignation.\n\n"+normalMember.name+" has a grudge against "+angryMember.name;
				if (normalMember.relationships.ContainsKey(angryMember)) {normalMember.RemoveRelatonship(angryMember);}
				normalMember.SetRelationship(angryMember,Relationship.RelationTypes.Enemy);
				break;
			}
		}
		return eventResult;
	}
}

public class LowMoraleQuit:GameEvent
{
	string eventDescription="";
	int moraleThreshold=10;
	PartyMember leavingMember;
	
	public override string GetDescription(MapRegion eventRegion, List<PartyMember> movedMembers) 
	{
		eventDescription="Fed up with the group, "+leavingMember.name+" decides to try his luck alone";
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
	
	public override List<string> GetChoices(MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Continue");
		return choicesList;
	}
	
	public override string DoChoice (string choiceString, MapRegion eventRegion, List<PartyMember> movedMembers)
	{
		string eventResult=null;
		switch (choiceString)
		{
		case"Continue": 
			{
				//success
				eventResult=leavingMember.name+" leaves the party";
				PartyManager.mainPartyManager.RemovePartyMember(leavingMember);
				break;
			}
		}
		return eventResult;
	}
}
