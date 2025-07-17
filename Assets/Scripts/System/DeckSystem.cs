
using System;
using System.Collections.Generic;
using MessageSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR;

public class DeckSystem
{
	//todo: command->log화?
	private int energy;
	public int Energy
	{
		get => energy;
		set
		{
			NoticeSystem.Instance.Publish(new EnergyChangeNotice(energy, value));
			energy = value;
		}
	}
	public PlayerHand PlayerHand { get; } = new();
	public PlayerField PlayerField { get; } = new();

	private List<BattleCardObjectInHand> deck = new();
	private List<BattleCardObjectInHand> dropCardList = new();

	private GameObject deckObject;

	private Vector3 deckPos = Vector3.zero;

	private BlockInputHandler blockInputHandler = new();
	public BlockInputHandler BlockInputHandler => blockInputHandler;
	
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
		
		Energy = Game.Instance.GetPlayer().CurrentPlayInfo.MaxEnergy;
		deckObject = new GameObject("Deck");
		//todo:fix?
		deckObject.transform.SetParent(Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().StageGameObject.transform);
		BattleCardObjectInHand cardObject;
		foreach (var card in Game.Instance.GetPlayer().CurrentPlayInfo.DeckCardList)
		{
			cardObject = card switch
			{
				SkillCard skillCard => SkillCardInHand.Instantiate(skillCard, new SkillCardBattleStat(skillCard.Stat))
			};

			cardObject.transform.SetParent(deckObject.transform);
			deck.Add(cardObject);
		}
		blockInputHandler.BlockInputs(InputBlockFlag.All, this);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
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
		NoticeSystem.Instance.Unsubscribe<FieldCardSelectNotice>(OnFieldCardSelect);
		NoticeSystem.Instance.Unsubscribe<FieldCardSelectCancelNotice>(OnFieldCardSelectCancel);
		NoticeSystem.Instance.Unsubscribe<PlayerFieldCardMoveNotice>(OnPlayerFieldCardMove);
		NoticeSystem.Instance.Unsubscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
		NoticeSystem.Instance.Unsubscribe<PlayerTurnEndNotice>(OnPlayerTurnEnd);
	}

	private void OnPlayerTurnStart(PlayerTurnStartNotice m)
	{
		//todo:fix
		if (!blockInputHandler.HasRequest(m.PlayerTurnObject))
		{
			blockInputHandler.RestoreInputs(InputBlockFlag.All, this);
		}
		else
		{
			//todo: hand, field 플래그 분리
			blockInputHandler.RestoreInputs(InputBlockFlag.All, m.PlayerTurnObject);
		}
		
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);

		Energy = Game.Instance.GetPlayer().CurrentPlayInfo.MaxEnergy;
		for (var i = 0; i < Game.Instance.GetPlayer().CurrentPlayInfo.DrawCount; i++)
		{
			DrawCard();
		}
	}

	private void OnPlayerTurnEnd(PlayerTurnEndNotice m)
	{
		DropAllCards();
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
	
	public void SpawnAllyUnits()
	{
		//충분히 배치를 해주지 않았다면 임의 배치
		var playInfo = Game.Instance.GetPlayer().CurrentPlayInfo;
		playInfo.NormalizeFieldDeployLocationInfo();
		
		var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
		var deployInfos = playInfo.FieldDeployLocationInfo;
		deployInfos.Sort((x, y) => x.Col == y.Col ? x.Row.CompareTo(y.Row) : y.Col.CompareTo(x.Col));
		
		//todo: 소환순서..
		foreach (var info in deployInfos)
		{
			var card = UnitCardInField.Instantiate(info.TargetCard.UnitCardStaticSpec, map.GetTileAt(info.Row, info.Col),
				ObjectType.Ally);
				
			PlayerField.AddToField(card);
			
			card.UpdateBlockInput(blockInputHandler.BlockInput);
		}
	}
	
	//todo: 없을 때 예외 체크
	public void DrawCard()
	{
		if (deck.Count == 0)
		{
			//todo: shuffle
			if (dropCardList.Count != 0)
			{
				(deck, dropCardList) = (dropCardList, deck);
			}
			else
			{
				return;
			}
		}
		var targetCard = deck[^1];
		targetCard.Activate();
		deck.RemoveAt(deck.Count - 1);
		PlayerHand.AddCard(targetCard);
	}

	public void DropCard(BattleCardObjectInHand target)
	{
		if (PlayerHand.CardList.Count == 0)
		{
			return;
		}
		
		PlayerHand.RemoveCard(target);
		dropCardList.Add(target);
		target.Deactivate();
	}
	
	public void DropAllCards()
	{
		for (var i = PlayerHand.CardList.Count - 1; i >= 0; i--)
		{
			DropCard(PlayerHand.CardList[i]);
		}
	}
}