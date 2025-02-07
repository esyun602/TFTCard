
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DeckSystem
{
	public PlayerHand PlayerHand { get; } = new();

	private List<BattleCardObjectInHand> deck = new();

	private Vector3 deckPos = Vector3.zero;
	
	public void Initialize()
	{
		PlayerHand.Initialize();
		foreach (var card in Game.Instance.GetPlayer().CardList)
		{
			var cardObject = BattleCardObjectInHand.Instantiate(card, new BattleStat(card.Stat));
			deck.Add(cardObject);
		}
	}

	public void Dispose()
	{
		PlayerHand.Dispose();	
	}

	//todo: 없을 때 예외 체크
	public void DrawCard()
	{
		var targetCard = deck[^1];
		targetCard.Activate();
		deck.RemoveAt(deck.Count - 1);
		PlayerHand.AddCard(targetCard);
	}
}