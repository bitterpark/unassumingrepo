using UnityEngine;
using System.Collections;

public class MainMenu
{
	public bool DrawMainMenu()
	{
		bool resBool=false;
		if (GUI.Button(new Rect(Screen.width*0.5f,Screen.height*0.5f,100,20),"New Game"))
		{resBool=true;}
		return resBool;
	}
}
