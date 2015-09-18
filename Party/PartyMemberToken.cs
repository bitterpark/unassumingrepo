using UnityEngine;
using System.Collections;

//Party member combat token
public class PartyMemberToken : MonoBehaviour {
	
	//public int drawnMemberIndex;
	public PartyMember drawnMember;
	
	public bool selected
	{
		get {return _selected;}
		set 
		{
			_selected=value;
			if (_selected) {ChangeColor(Color.cyan);}//renderer.material.SetColor("_Color",Color.cyan);}//renderer.material.color=Color.cyan;}
			else {turnTaken=turnTaken;}
		}
	
	}
	bool _selected=false;
	
	public bool turnTaken
	{
		get {return _turnTaken;}
		set 
		{
			_turnTaken=value; 
			if (_turnTaken) {ChangeColor(Color.gray);}//renderer.material.SetColor("_Color",Color.gray);}//renderer.material.color=Color.gray;}
			else 
			{
				if (!selected) ChangeColor(Color.white);//renderer.material.SetColor("_Color",Color.white);//renderer.material.color=Color.white;
			}
		}
	
	}
	bool _turnTaken=false;
	
	void OnMouseDown()
	{
		if (!turnTaken) {EncounterManager.mainEncounterManager.PartyTokenClicked(drawnMember);}
		//print ("Selected:"+drawnMemberIndex);
	}
	
	void ChangeColor(Color c)
	{
		gameObject.GetComponent<SpriteRenderer>().color=c;
	}
}
