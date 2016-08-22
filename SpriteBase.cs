using UnityEngine;
using System.Collections;

public class SpriteBase : MonoBehaviour {
	
	public static SpriteBase mainSpriteBase;

	//Items
	//Usable
	
	public Sprite medkitSprite;
	public Sprite bandageSprite;
	public Sprite perishableFoodSprite;
	public Sprite pillsSprite;
    public Sprite alcohol;
    //Usable encounter items
    public Sprite firecracker;
    public Sprite coffee;
	//Food
	public Sprite foodSprite;
	public Sprite foodSpriteSmall;
	public Sprite foodSpriteBig;
	public Sprite foodSpriteCooked;
	//
	public Sprite radioSprite;
	public Sprite settableTrapSprite;
	public Sprite gasolineSprite;
	
	//Equippable
	public Sprite flashlightSprite;
	public Sprite armorvestSprite;
	//Crafting ingredients
	public Sprite gunpowderSprite;
	//Camp items
	public Sprite bedSprite;
	public Sprite fuelSprite;
	public Sprite campBarricadeSprite;
	public Sprite toolsSprite;

	//Assigned task sprites
	public Sprite buildCampSprite;
	public Sprite restSprite;
	public Sprite noActionSprite;
	
	//Enemy sprites
	public Sprite genericEnemySprite;
	//Status effect sprites
	public Sprite bleedSprite;
	public Sprite phasedOutSprite;
	public Sprite coldSprite;
	

	//Event sprites
	public Sprite gasStationSprite;

	//Building icons
	public Sprite barIcon;
	public Sprite padlockIcon;

	public Color[] possibleMemberColors;

	//New stuff
	public Sprite mercPortrait;
	public Sprite enemyPortrait;

	public Sprite arm;
	public Sprite leg;

	public Sprite pistol;
	public Sprite shotgun;
	public Sprite assaultRifle;
	public Sprite pipegun;

	public Sprite ammoBox;
	public Sprite backpack;
	public Sprite skull;
	public Sprite droplet;
	public Sprite crosshair;
	public Sprite fire;
	public Sprite cover;
	public Sprite arrow;
	public Sprite lateralArrows;
	public Sprite bullet;
	public Sprite bullets;
	public Sprite flamingBullet;
	public Sprite snowflake;
	public Sprite bomb;
	public Sprite sack;
	public Sprite wrench;
	public Sprite roseOfWinds;
	public Sprite screwdriver;
	public Sprite medicalCross;
	public Sprite radio;
	public Sprite meat;
	public Sprite rest;
	public Sprite lightning;
	public Sprite lockIcon;
	public Sprite cloud;
	public Sprite key;
	public Sprite compass;
	public Sprite anchor;
	public Sprite door;
	public Sprite flag;
	public Sprite armor;
	public Sprite rock;
	public Sprite buckshot;
	public Sprite pipe;
	public Sprite knife;
	public Sprite axe;
	public Sprite money;
	public Sprite medal;

	// Use this for initialization
	void Start () 
	{
		mainSpriteBase=this;//
	}
	

}
