using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck<T> where T:class
{

	List<T> drawPile=new List<T>();
	List<T> discardPile=new List<T>();

	public void Populate(params T[] cards)
	{
		Populate(true, cards);
	}

	public void Populate(bool shuffle, params T[] cards)
	{
		foreach (T card in cards) AddToTop(card);
		if (shuffle) Shuffle();
	}
	//Top is considered index=n, bottom - index=0
	public void AddToTop(T card) { drawPile.Add(card); }
	public void AddToBottom(T card) { drawPile.Insert(0, card); }

	public List<T> DrawCards(int number)
	{
		List<T> drawnCards = new List<T>();

		if (drawPile.Count < number)
		{
			if (drawPile.Count + discardPile.Count < number) throw new System.Exception("Trying to draw more cards than exist in the deck!");
			Reshuffle();
		}

		for (int i = 0; i < number; i++)
		{
			drawnCards.Add(drawPile[drawPile.Count-1]);
			drawPile.RemoveAt(drawPile.Count-1);
		}
		return drawnCards;
	}

	public void DiscardCards(params T[] discarded)
	{
		foreach (T card in discarded) discardPile.Add(card);
	}

	public void Reshuffle()
	{
		DiscardCards(drawPile.ToArray());
		drawPile.Clear();
		Populate(discardPile.ToArray());
		discardPile.Clear();
		Shuffle();
	}

	public void Shuffle()
	{
		List<T> buffer = new List<T>(drawPile);
		drawPile.Clear();
		foreach (T card in buffer)
		{
			if (Random.value < 0.5f) AddToTop(card);
			else AddToBottom(card);
		}
	}
}
