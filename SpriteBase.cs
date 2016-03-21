using UnityEngine;
using System.Collections;

public class SpriteBase : MonoBehaviour {
	
	public static SpriteBase mainSpriteBase;
	
	//Ranged
	public Sprite nineMSprite;
	public Sprite shotgunSprite;
	public Sprite assaultRifleSprite;
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
	public Sprite foodSprite;
	public Sprite foodSpriteSmall;
	public Sprite foodSpriteBig;
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
	//Misc
	public Sprite bedSprite;
	
	//Assigned task sprites
	public Sprite buildCampSprite;
	public Sprite restSprite;
	public Sprite noActionSprite;
	
	//Enemy sprites
	public Sprite genericEnemySprite;
	//Status effect sprites
	public Sprite bleedSprite;
	public Sprite phasedOutSprite;
	public Sprite brokenArmsSprite;
	public Sprite brokenLegsSprite;
	
	public Color[] possibleMemberColors;
	
	// Use this for initialization
	void Start () 
	{
		mainSpriteBase=this;
	}
	

}
