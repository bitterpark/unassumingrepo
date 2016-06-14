using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck<T> where T:class
{

	List<T> drawPile=new List<T>();
	List<T> discardPile=new List<T>();

	public List<T> GetDeckCards()
	{
		List<T> allCards = new List<T>();
		allCards.AddRange(drawPile);
		allCards.AddRange(discardPile);
		return allCards;
	}

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
			drawnCards.Add(DrawCard());
		}
		return drawnCards;
	}

	public T DrawCard()
	{
		if (drawPile.Count < 1)
		{
			if (drawPile.Count + discardPile.Count < 1) throw new System.Exception("Trying to draw more cards than exist in the deck!");
			Reshuffle();
		}
		T drawnCard=drawPile[drawPile.Count-1];
		drawPile.RemoveAt(drawPile.Count - 1);
		return drawnCard;
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

public class CombatDeck : Deck<CombatCard>
{
	public void Populate(System.Type cardType)
	{
		Populate(cardType, 1);
	}
	public void Populate(System.Type cardType, int count)
	{
		Populate(CombatCard.GetMultipleCards(cardType, this, count));
	}
}
