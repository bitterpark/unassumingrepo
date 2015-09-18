using UnityEngine;
using System.Collections;

public class SpriteBase : MonoBehaviour {
	
	public static SpriteBase mainSpriteBase;
	
	
	public Sprite nineMSprite;
	public Sprite assaultRifleSprite;
	
	public Sprite pipeSprite;
	public Sprite knifeSprite;
	public Sprite axeSprite;
	
	
	public Sprite medkitSprite;
	public Sprite foodSprite;
	public Sprite flashlightSprite;
	public Sprite armorvestSprite;
	
	
	public Sprite bleedSprite;
	public Sprite phasedOutSprite;
	
	// Use this for initialization
	void Start () 
	{
		mainSpriteBase=this;
	}
	

}
