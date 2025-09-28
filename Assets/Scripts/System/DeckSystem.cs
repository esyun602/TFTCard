
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
			var target = Mathf.Clamp(value, Game.Instance.GetPlayer().CurrentPlayInfo.MinEnergy,
				Game.Instance.GetPlayer().CurrentPlayInfo.MaxEnergy);
			
			energy = target;
			NoticeSystem.Instance.Publish(new EnergyChangeNotice(energy, target));
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
	public List<BattleCardObjectInHand> EnemyCardPool => enemyCardPool;
	private List<BattleCardObjectInHand> enemyDropCardList = new();
	public List<BattleCardObjectInHand> EnemyDropCardList => enemyDropCardList;
	private List<BattleCardObjectInHand> deck = new();
	public List<BattleCardObjectInHand> Deck => deck;
	private List<BattleCardObjectInHand> dropCardList = new();
	public List<BattleCardObjectInHand> DropCardList => dropCardList;

	private GameObject deckObject;

	private Vector3 deckPos = Vector3.zero;

	private BlockInputHandler blockInputHandler = new();
	public BlockInputHandler BlockInputHandler => blockInputHandler;
	
	public void Initialize()
	{
		var playInfo = Game.Instance.GetPlayer().CurrentPlayInfo;
		playInfo.NormalizeFieldDeployInfo();
		
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Subscribe<FieldCardSelectNotice>(OnFieldCardSelect);
		NoticeSystem.Instance.Subscribe<FieldCardSelectCancelNotice>(OnFieldCardSelectCancel);
		NoticeSystem.Instance.Subscribe<PlayerFieldCardMoveNotice>(OnPlayerFieldCardMove);
		NoticeSystem.Instance.Subscribe<SkillHandCardStartUseNotice>(OnCardStartUse);
		NoticeSystem.Instance.Subscribe<SkillHandCardEndUseNotice>(OnCardEndUse);
		NoticeSystem.Instance.Subscribe<SkillCardActionTriggerNotice>(OnSkillCardActionTrigger);
		NoticeSystem.Instance.Subscribe<SkillCardActionRoutineCompleteNotice>(OnSkillCardActionComplete);
		NoticeSystem.Instance.Subscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
		NoticeSystem.Instance.Subscribe<PlayerTurnEndNotice>(OnPlayerTurnEnd);
		PlayerHand.Initialize();
		PlayerField.Initialize();

		Energy = 0;
		CardMoveCount = 0;
		deckObject = new GameObject("Deck");
		//todo:fix?
		deckObject.transform.SetParent(Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().StageGameObject.transform);
		foreach (var card in Game.Instance.GetPlayer().CurrentPlayInfo.TacticsCardList)
		{
			GenerateTacticsCardInstance(card);
		}
		
		ShuffleDeck();
		blockInputHandler.BlockInputs(InputBlockFlag.All, this);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}
	
	private void OnSkillCardActionTrigger(SkillCardActionTriggerNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.TargetAction);
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
	}

	private void OnSkillCardActionComplete(SkillCardActionRoutineCompleteNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.TargetAction);
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
	
	//todo: battlestat owner set 관련된부분 살펴보기
	public UnitSkillCardInHand GenerateUnitSkillCardInstance(IBattleObject bo, UnitSkillCard skillCard, bool addToEnemyPool = false)
	{
		UnitSkillCardInHand obj;
		if (addToEnemyPool)
		{
			obj = UnitSkillCardInHand.InstantiateForEnemy(skillCard, new UnitSkillCardBattleStat(skillCard.UnitSkillCardStat, bo));
			
			obj.transform.SetParent(deckObject.transform);
			enemyCardPool.Add(obj);
		}
		else
		{
			obj = UnitSkillCardInHand.InstantiateForAlly(skillCard, new UnitSkillCardBattleStat(skillCard.UnitSkillCardStat, bo));

			obj.transform.SetParent(deckObject.transform);
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
		foreach (var cardObject in totalList)
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

		Energy += Game.Instance.GetPlayer().CurrentPlayInfo.EnergyPerTurn;
		for (var i = 0; i < Game.Instance.GetPlayer().CurrentPlayInfo.DeckDrawCount; i++)
		{
			DrawPlayerCard();
		}
		
		for (var i = 0; i < Game.Instance.GetPlayer().CurrentPlayInfo.EnemyDrawCount; i++)
		{
			DrawEnemyCard();
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
		
		PlayerHand.UpdateBlockFlags(blockInputHandler.BlockInput);
		PlayerField.UpdateBlockFlags(blockInputHandler.BlockInput);
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

	public void DrawPlayerCard(BattleCardObjectInHand card)
	{
		if (PlayerHand.CardList.Count >= Constant.PlayerHandMax)
		{
			return;
		}
		
		if(card == null) return;
		
		if (deck.Contains(card)) deck.Remove(card);
		else if (dropCardList.Contains(card)) dropCardList.Remove(card);
		else return;
		
		PlayerHand.CardList.Add(card);
	}

	public void ReturnHandCardToDeck(BattleCardObjectInHand card)
	{
		if (PlayerHand.CardList.Remove(card))
		{
			if (card.CardType == ObjectType.Ally)
			{
				deck.Add(card);
			}
			else if(card.CardType == ObjectType.Enemy)
			{
				enemyCardPool.Add(card);
			}
		}
	}

	public void ReturnHandCardToHand(BattleCardObjectInHand card)
	{
		if (PlayerHand.CardList.Remove(card))
		{
			PlayerHand.CardList.Add(card);
		}
	}
	
	public void DrawPlayerCard(SkillCardBase card)
	{
		DrawPlayerCard(GetSkillCardInstance(card));
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
		
		Game.Instance.GetGameMode<BattleStageGameMode>().TurnSystem.RegisterEnemyCard(targetCard);
	}

	//todo: hand 말고 다른 곳에서 버릴 때
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
		dropCardList.Remove(target);
		enemyCardPool.Remove(target);
		enemyDropCardList.Remove(target);
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

	public void OnAllyAdd(UnitCardInField ally)
	{
		foreach (var card in ally.TargetUnitCard.UnitSkillCard)
		{
			GenerateUnitSkillCardInstance(ally, card);
		}
		ShuffleDeck();
		
		PlayerField.AddToField(ally);
	}

	public void OnAllyRemove(UnitCardInField ally)
	{
		PlayerField.RemoveFromField(ally);

		foreach (var card in ally.TargetUnitCard.UnitSkillCard)
		{
			var obj = GetSkillCardInstance(card) as UnitSkillCardInHand;
		
			if (obj == null) return;
		
			//todo: fix
			obj.SetDeadState();
		}
		
		ShuffleDeck();
	}
	
	public void OnEnemyAdd(UnitCardInField enemy)
	{
		foreach (var card in enemy.TargetUnitCard.UnitSkillCard)
		{
			GenerateUnitSkillCardInstance(enemy, card, true);
			ShuffleEnemyDeck();
		}
	}

	public void OnEnemyRemove(UnitCardInField enemy)
	{
		foreach (var card in enemy.TargetUnitCard.UnitSkillCard)
		{
			var obj = GetSkillCardInstance(card) as UnitSkillCardInHand;

			if (obj == null) return;

			obj.SetDeadState();
			RemoveCard(obj);
		}
		
		ShuffleEnemyDeck();
	}

	public BattleCardObjectInHand GetSkillCardInstance(SkillCardBase skillCard)
	{
		return totalList.First(x => x.TargetCard == skillCard);
	}
}