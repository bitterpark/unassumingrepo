using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Encounter 
{
	public string lootDescription="";
	public string enemyDescription="";
	
	public int encounterSizeX=3;
	public int encounterSizeY=3;
	
	public enum LootTypes {Apartment,Warehouse,Store,Police,Hospital};
	public LootTypes encounterLootType;
	public enum LootItems {Ammo,Medkits,Food,Knife,NineM,AssaultRifle,Axe,ArmorVest,Flashlight};
	
	//key is dropchance percentage
	public Dictionary<float,LootItems> lootChances;
	
	public enum EnemyTypes {Flesh,Quick,Slime,Muscle,Transient,Gasser,Spindler};
	public EnemyTypes encounterEnemyType;  
	
	public Dictionary<Vector2,EncounterRoom> encounterMap=new Dictionary<Vector2,EncounterRoom>();
	
	
	
	
	
	public Encounter ()
	{
		float lootRoll=Random.Range(0f,5f);
		if (lootRoll<=5)
		{
			encounterLootType=LootTypes.Hospital; lootDescription="An empty clinic";
			lootChances=new Dictionary<float, LootItems>();
			lootChances.Add (0.4f,LootItems.Medkits);
		}
		if (lootRoll<=4)
		{
			encounterLootType=LootTypes.Police; lootDescription="A deserted police station";
			lootChances=new Dictionary<float, LootItems>();
			lootChances.Add (0.5f,LootItems.Ammo);
			lootChances.Add (0.2f,LootItems.NineM);
			lootChances.Add (0.1f,LootItems.AssaultRifle);
		}
		if (lootRoll<=3) 
		{
			encounterLootType=LootTypes.Apartment; lootDescription="An apartment building";
			lootChances=new Dictionary<float, LootItems>();
			//Add largest number first
			lootChances.Add (0.3f,LootItems.Knife);
			//one per level - 0.2-0.4
			//lootChances.Add (0.4f,LootItems.Medkits);
			//one per level - 0-0.2
			lootChances.Add (0.2f,LootItems.Food);
			//lootChances.Add (0.5f,LootItems.Medkits);
		}
		if (lootRoll<=2) 
		{
			encounterLootType=LootTypes.Store; lootDescription="An abandoned store";
			lootChances=new Dictionary<float, LootItems>();
			lootChances.Add (0.6f,LootItems.Food);
		}
		if (lootRoll<=1) 
		{
			encounterLootType=LootTypes.Warehouse; lootDescription="A warehouse";
			lootChances=new Dictionary<float, LootItems>();
			//lootChances.Add (0.7f,LootItems.NineM);
			lootChances.Add (0.85f,LootItems.ArmorVest);
			lootChances.Add (0.75f,LootItems.Axe);
			lootChances.Add (0.65f,LootItems.Knife);
			//0.4-0.6 range
			lootChances.Add (0.5f,LootItems.Flashlight);
			// 00-0.4 range
			lootChances.Add (0.3f,LootItems.Food);
			
		}
		
		float enemiesRoll=Random.Range(0f,7f);
		if (enemiesRoll<=7) {encounterEnemyType=EnemyTypes.Spindler; enemyDescription="spindlers";}
		if (enemiesRoll<=6) {encounterEnemyType=EnemyTypes.Gasser; enemyDescription="gas sacs";}
		if (enemiesRoll<=5) {encounterEnemyType=EnemyTypes.Transient; enemyDescription="transients";}
		if (enemiesRoll<=4) {encounterEnemyType=EnemyTypes.Muscle; enemyDescription="muscle masses";}
		if (enemiesRoll<=3) {encounterEnemyType=EnemyTypes.Flesh; enemyDescription="flesh masses";}
		if (enemiesRoll<=2) {encounterEnemyType=EnemyTypes.Quick; enemyDescription="quick masses";}
		if (enemiesRoll<=1) {encounterEnemyType=EnemyTypes.Slime; enemyDescription="slime masses";}
		
		//description="Zombie-infested warehouse";
		//GenerateEncounter();
		GenerateEncounterFromPrefabMap(PrefabAssembler.assembler.SetupEncounterMap(this,encounterLootType));
	}
	
	void GenerateEncounterFromPrefabMap(List<EncounterRoom> prefabMap)
	{
		encounterMap.Clear();
		foreach (EncounterRoom room in prefabMap)
		{
			encounterMap.Add (new Vector2(room.xCoord,room.yCoord),room);
			//newRoom.xCoord=j;
			//newRoom.yCoord=i;
			if (Random.value<0.3f) 
			{
				room.hasEnemies=true;
			} else {room.hasEnemies=false;}
		}
	}
	
	void GenerateEncounter()
	{	
		encounterMap.Clear();
		
		int encounterSizeX=3;
		int encounterSizeY=3;
		
		//Vector2 encounterMapTopLeft=Camera.main.ScreenToWorldPoint(new Vector2(encounterBoxRect.x+50,Screen.height-(encounterBoxRect.y+300)));//(GUIUtility.GUIToScreenPoint(new Vector2(encounterBoxRect.x,encounterBoxRect.y)));
		float elementSizeX=20;
		float elementSizeY=20;
		float elementGapX=3;
		float elementGapY=3;
		
		
		for (int i=0; i<encounterSizeY; i++)
		{
			for (int j=0; j<encounterSizeX;j++)
			{
				//Vector2 newRoomPos=encounterMapTopLeft+new Vector2((elementSizeX+elementGapX)*j,-(elementSizeY+elementGapY)*i);
				EncounterRoom newRoom=new EncounterRoom(this);//Instantiate(encounterRoomPrefab,newRoomPos,Quaternion.identity) as EncounterRoom;
				encounterMap.Add (new Vector2(j,i),newRoom);
				newRoom.xCoord=j;
				newRoom.yCoord=i;
				if (Random.value<0.5) 
				{
					newRoom.hasEnemies=true;
				} else {newRoom.hasEnemies=false;}
				/*
				if (j==encounterPlayerX && i==encounterPlayerY) 
				{
					//if (encounterPlayerToken!=null) {}
					encounterPlayerToken=Instantiate(encounterPlayerTokenPrefab,newRoom.transform.position+new Vector3(0,0,-2),Quaternion.identity) as GameObject;
				}*/
			}
		}
		
		//Add exit last
		encounterMap[new Vector2(0,0)].isExit=true;
		
	}
}
