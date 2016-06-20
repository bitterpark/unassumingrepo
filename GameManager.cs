using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	bool gamestartPrepared=false;
	public bool gameStarted;
	MainMenu mainMenu;
	MapManager myMapManager;
	
	public static GameManager main;
	
	public delegate void GameOverDeleg();
	public static event GameOverDeleg GameOver;
	
	delegate void GameStartDeleg();
	static event GameStartDeleg GameStart;
	//public delegate void MapManagerStartDelegate();
	//public delegate void PartyManagerStartDelegate();
	
	public Canvas startMenuCanvas;
	
	bool gameOver=false;
	bool gameWin=false;
	
	public static void DebugPrint(string printed) {print (printed);}
	//public Canvas partyStatusCanvas;
	// Use this for initialization
	void Start () 
	{
		main=this;
		mainMenu=new MainMenu();
		myMapManager=gameObject.GetComponent<MapManager>();
		Button startButton=startMenuCanvas.transform.FindChild("Start Game Button").GetComponent<Button>();
		startButton.onClick.AddListener(()=>RegisterClick());
		startMenuCanvas.gameObject.SetActive(true);
		StartCoroutine(PrepareGamestartDelegate());
		//partyStatusCanvas.enabled=false;
	}
	
	IEnumerator PrepareGamestartDelegate()
	{
		//Determine the proper order of starting delegates here
		while (GameEventManager.mainEventManager==null) yield return new WaitForFixedUpdate();
		GameStart+=GameEventManager.mainEventManager.GamestartGenerateEventLists;
		while (MapManager.main==null) yield return new WaitForFixedUpdate();
		GameStart+=MapManager.main.GamestartMapSetup;
		while (PartyManager.mainPartyManager==null) yield return new WaitForFixedUpdate();
		GameStart+=PartyManager.mainPartyManager.SetDefaultState;
		while (TownManager.main == null) yield return new WaitForFixedUpdate();
		GameStart += TownManager.main.NewGameState;
		gamestartPrepared=true;
		//RegisterClick();
		yield break;
	}
	
	//lambda expression example
	void RegisterClick() 
	{
		if (this.gamestartPrepared)
		{
			gameStarted=true;
			startMenuCanvas.gameObject.SetActive(false);
			//partyStatusCanvas.gameObject.SetActive(true);
			
			//startMenuCanvas.enabled=false;
			StartNewGame();
		}
	}
	
	void StartNewGame()
	{
		if (GameStart!=null) GameStart();
	}
	
	public virtual void EndCurrentGame(bool win)
	{
		gameStarted=false;
		if (GameOver!=null) {GameOver();}
		startMenuCanvas.gameObject.SetActive(true);
		StartCoroutine(DoGameOver(win));
		//partyStatusCanvas.gameObject.SetActive(false);
		//startMenuCanvas.enabled=true;
		//partyStatusCanvas.enabled=false;
	}
	
	IEnumerator DoGameOver(bool win)
	{
		//Rect messageRect=new Rect();
		gameOver=true;
		gameWin=win;
		string endMessage;
		if (gameWin) endMessage="You have finished the spaceship!";
		else endMessage="Your crew abandoned you. Game over.";
		PartyStatusCanvasHandler.main.NewNotification(endMessage);
		//partyStatusCanvas.gameObject.SetActive(false);
		//EndCurrentGame();
		yield return new WaitForSeconds(2);
		gameOver=false;	
		gameWin=false;
		yield break;
	}
}
