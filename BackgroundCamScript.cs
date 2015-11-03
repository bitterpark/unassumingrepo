using UnityEngine;
using System.Collections;

public class BackgroundCamScript : MonoBehaviour {

	public Camera foregroundCam;
	public static BackgroundCamScript mainCamScript;
	
	void Start() {mainCamScript=this;}
	
	void OnGUI(){
		// [all your other gui stuff]
		/*
		if (Event.current.type == EventType.Repaint){
			foregroundCam.Render();
		}*/
	}
}
