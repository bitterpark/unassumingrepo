using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AttackModeToggler : MonoBehaviour 
{
	public Sprite rangedSprite;
	public Sprite meleeSprite;
	
	public void SetMode(bool isRanged)
	{
		if (isRanged) {GetComponent<Image>().sprite=rangedSprite;}
		else {GetComponent<Image>().sprite=meleeSprite;}
	}
}
