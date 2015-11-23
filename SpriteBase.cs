﻿using UnityEngine;
using System.Collections;

public class SpriteBase : MonoBehaviour {
	
	public static SpriteBase mainSpriteBase;
	
	
	public Sprite nineMSprite;
	public Sprite shotgunSprite;
	public Sprite assaultRifleSprite;
	
	public Sprite pipeSprite;
	public Sprite knifeSprite;
	public Sprite axeSprite;
	
	public Sprite ammoBoxSprite;
	public Sprite medkitSprite;
	public Sprite bandageSprite;
	public Sprite perishableFoodSprite;
	public Sprite foodSprite;
	public Sprite flashlightSprite;
	public Sprite armorvestSprite;
	public Sprite radioSprite;
	public Sprite settableTrapSprite;
	
	
	public Sprite genericEnemySprite;
	
	public Sprite bleedSprite;
	public Sprite phasedOutSprite;
	
	// Use this for initialization
	void Start () 
	{
		mainSpriteBase=this;
	}
	

}
