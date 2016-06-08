using UnityEngine;
using System.Collections;

public class SpriteBase : MonoBehaviour {
	
	public static SpriteBase mainSpriteBase;
	
	//Ranged
	public Sprite nineMSprite;
	public Sprite shotgunSprite;
	public Sprite assaultRifleSprite;
	public Sprite pipegunSprite;
	//Melee
	public Sprite pipeSprite;
	public Sprite knifeSprite;
	public Sprite axeSprite;
	//Items
	//Usable
	public Sprite ammoBoxSprite;
	public Sprite bulletSprite;
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
	public Sprite backpackSprite;
	//Equippable
	public Sprite flashlightSprite;
	public Sprite armorvestSprite;
	//Crafting ingredients
	public Sprite gunpowderSprite;
	public Sprite scrapSprite;
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
	public Sprite brokenArmsSprite;
	public Sprite brokenLegsSprite;

	//Event sprites
	public Sprite gasStationSprite;

	//Building icons
	public Sprite barIcon;
	public Sprite padlockIcon;

	public Color[] possibleMemberColors;

	//New stuff
	public Sprite mercPortrait;
	public Sprite enemyPortrait;

	public Sprite bite;
	public Sprite venom;

	// Use this for initialization
	void Start () 
	{
		mainSpriteBase=this;
	}
	

}
