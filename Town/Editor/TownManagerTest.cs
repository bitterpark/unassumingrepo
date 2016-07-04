using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;

public class TownManagerTest {

    [Test]
    public void DoesCrewDecreaseWhenNegativeBalanceIsReached()
    {
		/*
        //Arrange
		var mgr = NSubstitute.Substitute.For<TownManager>();
		mgr.When(x => x.UpdateMercenaryHireList()).DoNotCallBase();

		int startingCrew=50;
		mgr.SetCrew(startingCrew);
		mgr.SetMoney(-1);

        //Act
		mgr.WeeklyCashBalanceChange();

        //Assert
		Assert.Less(mgr.GetCrew(),startingCrew);//AreEqual(newGameObjectName, gameObject.name);
		 * */
    }

	[Test]
	public void IsGameOverCalledWhenCrewReachesZero()
	{
		/*
		//Arrange
		var gameManager = NSubstitute.Substitute.For<GameManager>();
		gameManager.When(x => x.EndCurrentGame(false)).DoNotCallBase();
		GameManager.main = gameManager;

		TownScreen.main = NSubstitute.Substitute.For<TownScreen>();

		var mgr = new TownManager();
		//mgr.When(x => x.UpdateMercenaryHireList()).DoNotCallBase();
		mgr.NewGameState();

		//Act
		mgr.IncrementCrew(-mgr.GetCrew());

		//Assert
		gameManager.Received().EndCurrentGame(false);
		 * */
	}
}
