using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameEvent
{
	public abstract string GetDescription();// {return null;}
	public virtual bool PreconditionsMet() {return true;}
	public virtual bool AllowMapMove() {return true;}
	public abstract List<string> GetChoices();// {return null;}
	public abstract string DoChoice(string choiceString);// {return null;}
}

public class NewSurvivor:GameEvent
{
	string eventDescription="You come across another survivor, who asks to join your group";
	
	public override string GetDescription() {return eventDescription;}
	
	public override List<string> GetChoices()
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Accept them");
		choicesList.Add("Reject them");
		return choicesList;
	}
	public override string DoChoice (string choiceString)
	{
		string eventResult=null;
		
		switch (choiceString)
		{
			case"Accept them": 
			{
				PartyMember newGuy=new PartyMember("Newdude",null);
				PartyManager.mainPartyManager.AddNewPartyMember(newGuy);//.partyMembers.Add (newGuy);
				eventResult=newGuy.name+" joins your party!";
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

public class FoodSpoilage:GameEvent
{
	string eventDescription="You discover some of your perishable food is about to expire. You can try to eat it now and risk food poisoning, or just throw it away";
	
	public override string GetDescription() {return eventDescription;}
	
	public override bool PreconditionsMet ()
	{
		bool conditionsAreMet=false;
		int partyFoodCount=0;
		foreach (InventoryItem item in PartyManager.mainPartyManager.partyInventory)
		{
			if (item.GetType()==typeof(Food)) {partyFoodCount+=1;}
			if (partyFoodCount>=2) {conditionsAreMet=true; break;}
		}
		return conditionsAreMet;
	}
	
	public override List<string> GetChoices()
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Eat it");
		choicesList.Add("Throw it out");
		return choicesList;
	}
	public override string DoChoice (string choiceString)
	{
		string eventResult=null;
		
		switch (choiceString)
		{
		case"Eat it": 
			{
				//success
				if (Random.value>0.5f)
				{
					eventResult="Realizing you can't be picky at a time like this, the party sets out eating old eggs, bread, meat products and other persihables. Aside from some minor stomach cramps, nobody suffers any ill effects and everyone is refreshed by the most decadent meal in days.\n\nFood is halved, everyone is fed";	
					foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
					{
						member.hunger=0;
					}
					
					List<Food> foodItems=new List<Food>();
					foreach (InventoryItem item in PartyManager.mainPartyManager.partyInventory)
					{
						if (item.GetType()==typeof(Food)) {/*foodCount+=1;*/ foodItems.Add(item as Food);}
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
					
					foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
					{
						if (member.health-5>0) {member.health-=5;}
						else {member.health=1;}
						//PartyManager.mainPartyManager.foodSupply-=1;
					}
					//PartyManager.mainPartyManager.foodSupply=Mathf.RoundToInt(PartyManager.mainPartyManager.foodSupply*0.5f);
					List<Food> foodItems=new List<Food>();
					foreach (InventoryItem item in PartyManager.mainPartyManager.partyInventory)
					{
						if (item.GetType()==typeof(Food)) {/*foodCount+=1;*/ foodItems.Add(item as Food);}
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
				eventResult="Deciding not to risk it, you throw out the dodgy foods\n\n-50% Food";
				//PartyManager.mainPartyManager.foodSupply=Mathf.RoundToInt(PartyManager.mainPartyManager.foodSupply*0.5f);
				List<Food> foodItems=new List<Food>();
				foreach (InventoryItem item in PartyManager.mainPartyManager.partyInventory)
				{
					if (item.GetType()==typeof(Food)) {/*foodCount+=1;*/ foodItems.Add(item as Food);}
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
	
}

public class MonsterAttack:GameEvent
{
	string eventDescription="As your party moves, you start to hear suspicious noises close by. They grow and shrink in intensity and change direction several times as you go, putting everyone on their guard. Finally, the source reveals itself: a monster!";
	
	public override string GetDescription() {return eventDescription;}
	public override List<string> GetChoices()
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Fight it");
		choicesList.Add("Try to run");
		//Check to see if party has ranged weapons
		bool partyHasGuns=false;
		foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers) 
		{
			if (member.equippedRangedWeapon!=null) {partyHasGuns=true; break;}
		}
		if (PartyManager.mainPartyManager.ammo>0 && partyHasGuns) 
		{choicesList.Add("Shoot it");}
		return choicesList;
	}
		
	public override string DoChoice (string choiceString)
	{
		string eventResult=null;
		
		switch (choiceString)
		{
		case"Fight it": 
			{
				//success
				if (Random.value>0.5f)
				{
					eventResult="Seeing the creature so close, you decide it's too late to run. Surging with adrenaline, the party charges the monster with their weapons drawn! Surprised, the terrifying creature wilts under a rain of blows, dead\n\n -1 Stamina for everyone";
					foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
					{
						member.stamina-=1;
					}
				}
				else
				{
					PartyMember hurtMember=PartyManager.mainPartyManager.partyMembers[Random.Range(0,PartyManager.mainPartyManager.partyMembers.Count)];
					eventResult=hurtMember.name+" is the first to charge the creature, and bears the brunt of its ferocious attacks. The others run in to help and finish it off, but "+hurtMember.name+" is in a bad shape.\n\n-25% health for "+hurtMember.name;
					//foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
					//{
						if (hurtMember.health-25>0) {hurtMember.health-=25;}
						else {hurtMember.health=1;}
						//PartyManager.mainPartyManager.foodSupply-=1;
					//}
					PartyManager.mainPartyManager.foodSupply=Mathf.RoundToInt(PartyManager.mainPartyManager.foodSupply*0.5f);
				}
				break;
			}
			case"Try to run":
			{
				eventResult="Surprised by the attack, the party turns tail. You run with your hearts in your ears, only stopping once you're about to keel over from exhaustion. You're not sure how long you ran, but the creature is nowhere in sight\n\n-5 Stamina for everyone";
				
				foreach (PartyMember member in PartyManager.mainPartyManager.partyMembers)
				{
					member.stamina-=5;
				}
				break;
			}
		
			case"Shoot it":
			{
				eventResult="Saved ammo is of no use when you're dead, and the creature disappears in a hail of gunfire.\n\n-10 Ammo";
				PartyManager.mainPartyManager.ammo-=10;
				break;
			}
		}
		return eventResult;
	}
}

public class LostInAnomaly:GameEvent
{
	string eventDescription="While making their way to the next region, the party turns a corner in a dark, enclosed archway. Upon walking out the other side, everyone realizes it's a completely different territory. They emerge into a couryard of tall buildings, impossibly dense and close to eachother. This couldn't be the destination you were trying to reach";
	
	public override string GetDescription() {return eventDescription;}
	public override bool AllowMapMove (){return false;}
	
	public override List<string> GetChoices()
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Press on");
		choicesList.Add("Try to retrace your steps");
		return choicesList;
	}
	
	public override string DoChoice (string choiceString)
	{
		string eventResult=null;
		
		switch (choiceString)
		{
		case"Press on": 
		{
			//success
			eventResult="This couldn't possibly be the place you were trying to reach before. The layout doesn't make sense, someone would have noticed such tall buildings from further away. Whatever happened to you all in that archway, you decide not to tempt fate and explore this new area instead";
			MapManager mpManager=MapManager.mainMapManager;
			mpManager.TeleportToRegion(mpManager.GetRegion(new Vector2(Random.Range(0,mpManager.mapWidth),Random.Range(0,mpManager.mapHeight))));
			break;
		}
		case"Try to retrace your steps":
		{
				if (Random.value<0.8f)
				{
					eventResult="You file back into the mysterious archway, and it seems to take several minutes longer than it did when you walked in the other direction. However, when you finally emerge, you find yourselves much further back, in the area you started from. Someone notices the archway behind you no longer rounds a corner...";
					PartyManager manager=PartyManager.mainPartyManager;
					MapManager.mainMapManager.TeleportToRegion(MapManager.mainMapManager.GetRegion(manager.mapCoordX,manager.mapCoordY));
				}
				else
				{
					eventResult="As you turn around and walk back through, the archway seems to take several more turns than it did last time, eventually terminating in a different region still. As you look back, the archway looks short and straight, and the area on the other side looks nothing like where you just came from. You decide not to take any more chances";
					MapManager mpManager=MapManager.mainMapManager;
					mpManager.TeleportToRegion(mpManager.GetRegion(new Vector2(Random.Range(0,mpManager.mapWidth),Random.Range(0,mpManager.mapHeight))));
				}
			break;
		}
		}
		return eventResult;
	}
}

public class CacheInAnomaly:GameEvent
{	
	string eventDescription="";
	
	public override string GetDescription() 
	{
		string noticingPartyMemberName=PartyManager.mainPartyManager.partyMembers[Random.Range(0,PartyManager.mainPartyManager.partyMembers.Count)].name;
		eventDescription="As you explore the surroundings looking for places to explore and loot, you come across a strange patch of road filled with empty cars. Their roofs stand at waist height relative to you, piled together as if they all slid from their parking spots into a sinkhole. The ground below them is covered in what looks like boiling water, emanating warmth and light steam. As you take in this strange sight,"+noticingPartyMemberName+"notices some food and supplies in an open bed truck amid the cars";
		
		return eventDescription;
	}
	public override List<string> GetChoices()
	{
		List<string> choicesList=new List<string>();
		choicesList.Add("Try to navigate the car roofs to reach the supplies");
		choicesList.Add("Leave it and move on");
		return choicesList;
	}
	
	public override string DoChoice (string choiceString)
	{
		string eventResult=null;
		
		switch (choiceString)
		{
		case"Try to navigate the car roofs to reach the supplies": 
			{
				PartyMember volunteer=PartyManager.mainPartyManager.partyMembers[Random.Range(0,PartyManager.mainPartyManager.partyMembers.Count)];
				//success
				if (Random.value<0.5f)
				{
					eventResult=volunteer.name+" volunteers to try and move across. The roofs are moist and slippery, and "+volunteer.name+" loses balance several times, but manages to recover and safely reaches the supplies. Loaded with bags,"+volunteer.name+" somehow makes it back across, covered in sweat but unharmed\n\n+2 food, +10 ammo, -1 stamina for "+volunteer.name;
					PartyManager.mainPartyManager.partyInventory.Add (new Food());
					PartyManager.mainPartyManager.partyInventory.Add (new Food());
					PartyManager.mainPartyManager.ammo+=10;
					volunteer.stamina-=1;
				}
				else
				{
					int damage=25;
					eventResult=volunteer.name+" navigates the first few cars with surprising nimbleness and grace, but an inclined hatchback roof knocks them off-balance. "+volunteer.name+" manages to grab an upper trunk of another nearby car at the last second, but their left food dips into the drink all the way to their ankle. A terrible scream follows, and a surge of adrenaline causes "+volunteer.name+" to pull themselves up into safety. With one foot badly burnt, they turn back and somehow manage to crawl back to safety.\n\n-"+damage+" health for "+volunteer.name;
					if (volunteer.health<damage) {damage=volunteer.health-1;}
					volunteer.TakeDamage(damage,false);
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
