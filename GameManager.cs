using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	public bool gameStarted;
	MainMenu mainMenu;
	MapManager myMapManager;
	
	public static GameManager mainGameManager;
	
	public delegate void GameOverDeleg();
	public static event GameOverDeleg GameOver;
	
	public delegate void GameStartDeleg();
	public static event GameStartDeleg GameStart;
	
	public Canvas startMenuCanvas;
	
	public static void DebugPrint(string printed) {print (printed);}
	//public Canvas partyStatusCanvas;
	// Use this for initialization
	void Start () 
	{
		mainGameManager=this;
		mainMenu=new MainMenu();
		myMapManager=gameObject.GetComponent<MapManager>();
		Button startButton=startMenuCanvas.transform.FindChild("Start Game Button").GetComponent<Button>();
		startButton.onClick.AddListener(()=>RegisterClick(startButton));
		startMenuCanvas.gameObject.SetActive(true);
		//partyStatusCanvas.enabled=false;
	}
	
	//lambda expression example
	void RegisterClick(Button btn) 
	{
		gameStarted=true;
		startMenuCanvas.gameObject.SetActive(false);
		//partyStatusCanvas.gameObject.SetActive(true);
		
		//startMenuCanvas.enabled=false;
		StartNewGame();
	}
	
	void Update() {if (Input.GetKeyDown(KeyCode.N)) EndCurrentGame();}
	/*
	void OnGUI()
	{
		if (!gameStarted)
		{
			gameStarted=mainMenu.DrawMainMenu();
			if (gameStarted) {StartNewGame();}
		}
	}*/
	
	void StartNewGame()
	{
		if (GameStart!=null) {GameStart();}
	}
	
	public void EndCurrentGame()
	{
		gameStarted=false;
		if (GameOver!=null) {GameOver();}
		startMenuCanvas.gameObject.SetActive(true);
		//partyStatusCanvas.gameObject.SetActive(false);
		//startMenuCanvas.enabled=true;
		//partyStatusCanvas.enabled=false;
	}
}
