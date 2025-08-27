
using System;
using System.Collections.Generic;
using System.Linq;
using MessageSystem;
using Unity.Mathematics;
using Unity.VisualScripting;
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
	public int CardMoveCount { get; set; }
	public PlayerHand PlayerHand { get; } = new();
	public PlayerField PlayerField { get; } = new();

	private IEnumerable<BattleCardObjectInHand> totalList
	{
		get
		{
			foreach (var cardInstance in deck)
			{
				yield return cardInstance;
			}

			foreach (var cardInstance in dropCardList)
			{
				yield return cardInstance;
			}

			foreach (var cardInstance in PlayerHand.CardList)
			{
				yield return cardInstance;
			}

			foreach (var cardInstance in enemyCardPool)
			{
				yield return cardInstance;
			}
			
			foreach (var cardInstance in enemyDropCardList)
			{
				yield return cardInstance;
			}
		}
	}

	private List<BattleCardObjectInHand> enemyCardPool = new();
	private List<BattleCardObjectInHand> enemyDropCardList = new();
	private List<BattleCardObjectInHand> deck = new();
	private List<BattleCardObjectInHand> dropCardList = new();

	private GameObject deckObject;

	private Vector3 deckPos = Vector3.zero;

	private BlockInputHandler blockInputHandler = new();
	public BlockInputHandler BlockInputHandler => blockInputHandler;
	
	public void Initialize()
	{
		var playInfo = Game.Instance.GetPlayer().CurrentPlayInfo;
		playInfo.NormalizeFieldDeployLocationInfo();
		
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Subscribe<FieldCardSelectNotice>(OnFieldCardSelect);
		NoticeSystem.Instance.Subscribe<FieldCardSelectCancelNotice>(OnFieldCardSelectCancel);
		NoticeSystem.Instance.Subscribe<PlayerFieldCardMoveNotice>(OnPlayerFieldCardMove);
		NoticeSystem.Instance.Subscribe<SkillHandCardStartUseNotice>(OnCardStartUse);
		NoticeSystem.Instance.Subscribe<SkillHandCardEndUseNotice>(OnCardEndUse);
		NoticeSystem.Instance.Subscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
		NoticeSystem.Instance.Subscribe<PlayerTurnEndNotice>(OnPlayerTurnEnd);
		PlayerHand.Initialize();
		PlayerField.Initialize();
		
		Energy = Game.Instance.GetPlayer().CurrentPlayInfo.MaxEnergy;
		CardMoveCount = 0;
		deckObject = new GameObject("Deck");
		//todo:fix?
		deckObject.transform.SetParent(Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().StageGameObject.transform);
		foreach (var card in Game.Instance.GetPlayer().CurrentPlayInfo.TacticsCardList)
		{
			GenerateTacticsCardInstance(card);
		}
		
		foreach (var card in Game.Instance.GetPlayer().CurrentPlayInfo.UnitSkillCardList)
		{
			GenerateUnitSkillCardInstance(card);
		}
		
		ShuffleDeck();
		blockInputHandler.BlockInputs(InputBlockFlag.All, this);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}

	private BattleCardObjectInHand GenerateTacticsCardInstance(TacticsCard skillCard, bool addToEnemyPool = false)
	{
		var obj = TacticsCardInHand.Instantiate(skillCard, new TacticsCardBattleStat(skillCard.Stat));

		obj.transform.SetParent(deckObject.transform);
		if (addToEnemyPool)
		{
			enemyCardPool.Add(obj);
		}
		else
		{
			deck.Add(obj);
		}

		return obj;
	}
	
	private BattleCardObjectInHand GenerateUnitSkillCardInstance(UnitSkillCard skillCard, bool addToEnemyPool = false)
	{
		var obj = UnitSkillCardInHand.Instantiate(skillCard, new UnitSkillCardBattleStat(skillCard.UnitSkillCardStat));

		obj.transform.SetParent(deckObject.transform);
		if (addToEnemyPool)
		{
			enemyCardPool.Add(obj);
		}
		else
		{
			deck.Add(obj);
		}

		return obj;
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
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardStartUseNotice>(OnCardStartUse);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardEndUseNotice>(OnCardEndUse);
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
		for (var i = 0; i < Game.Instance.GetPlayer().CurrentPlayInfo.DeckDrawCount; i++)
		{
			DrawPlayerCard();
		}
		
		for (var i = 0; i < Game.Instance.GetPlayer().CurrentPlayInfo.EnemyDrawCount; i++)
		{
			DrawEnemyCard();
		}

		if (PlayerHand.HasEnemyCard)
		{
			blockInputHandler.BlockInputs(InputBlockFlag.TurnEnd, this);
		}
	}

	private void OnPlayerTurnEnd(PlayerTurnEndNotice m)
	{
		DropAllCards();
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.PlayerTurnObject);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}
	
	private void OnHandCardSelect(SkillHandCardSelectNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.SelectedCard);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}

	private void OnHandCardSelectCancel(SkillHandCardSelectCancelNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.SelectedCard);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}

	private void OnCardStartUse(SkillHandCardStartUseNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.SelectedCard);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}

	private void OnCardEndUse(SkillHandCardEndUseNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.SelectedCard);

		if (!PlayerHand.HasEnemyCard)
		{
			blockInputHandler.RestoreInputs(InputBlockFlag.TurnEnd, this);
		}
		
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}
	
	public void SpawnAllyUnits()
	{
		var playInfo = Game.Instance.GetPlayer().CurrentPlayInfo;
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
		var deployInfos = playInfo.FieldDeployLocationInfo;
		deployInfos.Sort((x, y) => x.Col == y.Col ? x.Row.CompareTo(y.Row) : y.Col.CompareTo(x.Col));
		
		//todo: 소환순서..
		foreach (var info in deployInfos)
		{
			var card = UnitCardInField.Instantiate(info.TargetCard, map.GetTileAt(info.Row, info.Col), ObjectType.Ally);
				
			PlayerField.AddToField(card);
			
			card.UpdateBlockInput(blockInputHandler.BlockInput);
		}
	}
	
	//todo: 없을 때 예외 체크
	public void DrawPlayerCard()
	{
		if (PlayerHand.CardList.Count >= Constant.PlayerHandMax)
		{
			return;
		}
		
		if (deck.Count == 0)
		{
			if (dropCardList.Count != 0)
			{
				(deck, dropCardList) = (dropCardList, deck);
				ShuffleDeck();
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

	//todo: 핸드를 구분할 건지 정해야함
	public void DrawEnemyCard()
	{
		if (PlayerHand.CardList.Count >= Constant.PlayerHandMax)
		{
			return;
		}
		
		if (enemyCardPool.Count == 0)
		{
			if (enemyDropCardList.Count != 0)
			{
				(enemyCardPool, enemyDropCardList) = (enemyDropCardList, enemyCardPool);
				ShuffleEnemyDeck();
			}
			else
			{
				return;
			}
		}
		var targetCard = enemyCardPool[^1];
		targetCard.Activate();
		enemyCardPool.RemoveAt(enemyCardPool.Count - 1);
		PlayerHand.AddCard(targetCard);
	}

	public void DropCard(BattleCardObjectInHand target)
	{
		if (PlayerHand.CardList.Count == 0)
		{
			return;
		}
		
		PlayerHand.RemoveCard(target);
		if ((target.Stat as UnitSkillCardBattleStat)?.Owner?.ObjectType == ObjectType.Enemy)
		{
			enemyDropCardList.Add(target);
		}
		else
		{
			dropCardList.Add(target);
		}
		target.Deactivate();
	}

	public void RemoveCard(BattleCardObjectInHand target)
	{
		PlayerHand.RemoveCard(target);
		target.Deactivate();
		
		//todo:fix
		deck.Remove(target);
		enemyCardPool.Remove(target);
	}

	public void ShuffleDeck()
	{
		deck.Shuffle();
	}

	public void ShuffleEnemyDeck()
	{
		enemyCardPool.Shuffle();
	}
	
	public void DropAllCards()
	{
		for (var i = PlayerHand.CardList.Count - 1; i >= 0; i--)
		{
			DropCard(PlayerHand.CardList[i]);
		}
	}

	public void OnEnemyAdd(UnitCardInField enemy)
	{
		GenerateUnitSkillCardInstance(enemy.TargetUnitCard.UnitSkillCard, true);
		ShuffleEnemyDeck();
	}

	public void OnEnemyRemove(UnitCardInField enemy)
	{
		RemoveCard(GetSkillCardInstance(enemy.TargetUnitCard.UnitSkillCard));
		ShuffleEnemyDeck();
	}

	public BattleCardObjectInHand GetSkillCardInstance(SkillCardBase skillCard)
	{
		return totalList.First(x => x.IsInstanceOf(skillCard));
	}
}