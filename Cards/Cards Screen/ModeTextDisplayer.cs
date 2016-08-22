using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModeTextDisplayer : MonoBehaviour {

	public Image currentModeText;
	
	public void DisplayCenterMessage(string message)
	{
		currentModeText.gameObject.SetActive(true);
		currentModeText.GetComponentInChildren<Text>().text = message;
	}
	public void HideCenterMessage()
	{
		currentModeText.gameObject.SetActive(false);
	}
}
