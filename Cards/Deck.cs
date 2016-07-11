using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck<T> where T:class
{

	List<T> drawPile=new List<T>();
	List<T> discardPile=new List<T>();
	List<T> allCards = new List<T>();

	public List<T> GetDeckCards()
	{
		//List<T> allCards = new List<T>();
		//allCards.AddRange(drawPile);
		//allCards.AddRange(discardPile);
		return allCards;
	}

	public virtual void AddCards(params T[] cards)
	{
		allCards.AddRange(cards);
		PopulateDraw(cards);
	}

	public void RemoveCards(T[] removedCards)
	{
		foreach (T card in removedCards)
		{
			if (!allCards.Contains(card))
				throw new System.Exception("Attempting to remove cards that are not in the deck!");
			allCards.Remove(card);
			drawPile.Remove(card);
			discardPile.Remove(card);
		}
	}

	protected void PopulateDraw(params T[] cards)
	{
		PopulateDraw(true, cards);
	}

	protected void PopulateDraw(bool shuffle, params T[] cards)
	{
		foreach (T card in cards) 
			PutOnTopOfDrawpile(card);
		if (shuffle) 
			Shuffle();
	}
	//Top is considered index=n, bottom - index=0
	public void PutOnTopOfDrawpile(T card) 
	{ 
		drawPile.Add(card); 
	}
	public void PutAtBottomOfDrawpile(T card) 
	{ 
		drawPile.Insert(0, card); 
	}

	

	public List<T> DrawCards(int number)
	{
		List<T> drawnCards = new List<T>();

		for (int i = 0; i < number; i++)
		{
			T card;
			if (DrawCard(out card))
				drawnCards.Add(card);
		}
		return drawnCards;
	}

	public bool DrawCard(out T drawnCard)
	{
		if (drawPile.Count < 1)
		{
			if (drawPile.Count + discardPile.Count < 1)
			{
				drawnCard = null;
				return false;
			}
			Reshuffle();
		}
		drawnCard=drawPile[drawPile.Count-1];
		drawPile.RemoveAt(drawPile.Count - 1);
		return true;
	}

	public void DiscardCards(params T[] discarded)
	{
		foreach (T card in discarded) discardPile.Add(card);
	}

	public void Reshuffle()
	{
		DiscardCards(drawPile.ToArray());
		drawPile.Clear();
		PopulateDraw(discardPile.ToArray());
		discardPile.Clear();
		Shuffle();
	}

	public void Shuffle()
	{
		List<T> buffer = new List<T>(drawPile);
		drawPile.Clear();
		foreach (T card in buffer)
		{
			if (Random.value < 0.5f) PutOnTopOfDrawpile(card);
			else PutAtBottomOfDrawpile(card);
		}
	}
}

public class CombatDeck : Deck<CombatCard>
{
	public override void AddCards(params CombatCard[] cards)
	{
		foreach (CombatCard card in cards)
			card.originDeck = this;
		base.AddCards(cards);
	}
	
	public void AddCards(System.Type cardType)
	{
		AddCards(cardType, 1);
	}
	public void AddCards(System.Type cardType, int count)
	{
		CombatCard[] newCards = CombatCard.GetMultipleCards(cardType, count);
		foreach (CombatCard card in newCards)
		{
			card.originDeck = this;
		}
		AddCards(newCards);
	}
}
