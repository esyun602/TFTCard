
using System.Collections.Generic;
using MessageSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR;

public class DeckSystem
{
	public PlayerHand PlayerHand { get; } = new();
	public PlayerField PlayerField { get; } = new();

	private List<BattleCardObjectInHand> deck = new();

	private GameObject deckObject;

	private Vector3 deckPos = Vector3.zero;

	private BlockInputHandler blockInputHandler = new();
	
	
	public void Initialize()
	{
		NoticeSystem.Instance.Subscribe<HandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Subscribe<HandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Subscribe<FieldCardSelectNotice>(OnFieldCardSelect);
		NoticeSystem.Instance.Subscribe<FieldCardSelectCancelNotice>(OnFieldCardSelectCancel);
		NoticeSystem.Instance.Subscribe<PlayerFieldCardMoveNotice>(OnPlayerFieldCardMove);
		NoticeSystem.Instance.Subscribe<HandCardStartUseNotice>(OnCardStartUse);
		NoticeSystem.Instance.Subscribe<HandCardEndUseNotice>(OnCardEndUse);
		NoticeSystem.Instance.Subscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
		NoticeSystem.Instance.Subscribe<PlayerTurnEndNotice>(OnPlayerTurnEnd);
		PlayerHand.Initialize();
		PlayerField.Initialize();
		deckObject = new GameObject("Deck");
		//todo:fix?
		deckObject.transform.SetParent(Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().StageGameObject.transform);
		foreach (var card in Game.Instance.GetPlayer().CurrentPlayInfo.CardList)
		{
			var cardObject = BattleCardObjectInHand.Instantiate(card, new BattleStat(card.Stat));
			cardObject.transform.SetParent(deckObject.transform);
			deck.Add(cardObject);
		}
		blockInputHandler.BlockInputs(InputBlockFlag.All, this);
	}

	private void OnPlayerFieldCardMove(PlayerFieldCardMoveNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.Target);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}

	private void OnFieldCardSelect(FieldCardSelectNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.SelectedCard);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}

	private void OnFieldCardSelectCancel(FieldCardSelectCancelNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.SelectedCard);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}


	public void Dispose()
	{
		PlayerHand.Dispose();
		PlayerField.Dispose();
		foreach (var cardObject in deck)
		{
			cardObject.Dispose();
		}
		NoticeSystem.Instance.Unsubscribe<HandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Unsubscribe<HandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Unsubscribe<HandCardStartUseNotice>(OnCardStartUse);
		NoticeSystem.Instance.Unsubscribe<HandCardEndUseNotice>(OnCardEndUse);
		NoticeSystem.Instance.Unsubscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
		NoticeSystem.Instance.Unsubscribe<PlayerTurnEndNotice>(OnPlayerTurnEnd);
	}

	private void OnPlayerTurnStart(PlayerTurnStartNotice m)
	{
		//todo:fix
		if (!blockInputHandler.HasRequest(m.PlayerTurnObject))
		{
			blockInputHandler.RestoreInputs(InputBlockFlag.All, this);
			
			PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
			
			return;
		}
		//todo: hand, field 플래그 분리
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.PlayerTurnObject);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}

	private void OnPlayerTurnEnd(PlayerTurnEndNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.PlayerTurnObject);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}
	
	private void OnHandCardSelect(HandCardSelectNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.SelectedCard);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}

	private void OnHandCardSelectCancel(HandCardSelectCancelNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.SelectedCard);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}

	private void OnCardStartUse(HandCardStartUseNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.SelectedCard);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}

	private void OnCardEndUse(HandCardEndUseNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.SelectedCard);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
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