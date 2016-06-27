using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MercGraphic : CharacterGraphic
{
	public Text classText;

	public void AssignCharacter(Mercenary newChar)
	{
		base.AssignCharacter(newChar);
		portrait.color = newChar.GetColor();
		classText.text = newChar.GetClass();
	}
	
}
