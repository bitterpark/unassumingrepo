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
	public Sprite ammoBoxSprite;
	public Sprite medkitSprite;
	public Sprite bandageSprite;
	public Sprite perishableFoodSprite;
	public Sprite foodSprite;
	public Sprite foodSpriteSmall;
	public Sprite foodSpriteBig;
	public Sprite flashlightSprite;
	public Sprite armorvestSprite;
	public Sprite radioSprite;
	public Sprite settableTrapSprite;
	public Sprite bedSprite;
	public Sprite backpackSprite;
	
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
