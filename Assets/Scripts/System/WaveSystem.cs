
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

public class WaveSystem
{
	private int currentWaveIdx;
	private IUpdatableRoutine spawnNextWaveRoutine;
	private List<WaveGrid> waveData;
	private List<BattleCardObjectInField> currentEnemyObjects;
	private BlockInputHandler blockInputHandler = new();
	private Transform waveParentTransform;
	
	public WaveSystem(List<WaveGrid> waveData)
	{
		this.waveData = waveData;
	}

	public void Initialize()
	{
		currentEnemyObjects = new();
		spawnNextWaveRoutine = new UpdatableRoutine(UpdateSpawn);
		blockInputHandler.BlockInputs(InputBlockFlag.All, this);
		NoticeSystem.Instance.Subscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
		NoticeSystem.Instance.Subscribe<PlayerTurnEndNotice>(OnPlayerTurnEnd);		
		//todo: need inputsystem
		NoticeSystem.Instance.Subscribe<HandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Subscribe<HandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Subscribe<FieldCardSelectNotice>(OnFieldCardSelect);
		NoticeSystem.Instance.Subscribe<FieldCardSelectCancelNotice>(OnFieldCardSelectCancel);
		NoticeSystem.Instance.Subscribe<PlayerFieldCardMoveNotice>(OnPlayerFieldCardMove);
		NoticeSystem.Instance.Subscribe<HandCardStartUseNotice>(OnCardStartUse);
		NoticeSystem.Instance.Subscribe<HandCardEndUseNotice>(OnCardEndUse);
		//
		
		currentWaveIdx = -1;
		waveParentTransform = new GameObject("EnemyWave").transform;
		waveParentTransform.SetParent(Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().StageGameObject.transform);
	}

	private void OnPlayerTurnStart(PlayerTurnStartNotice m)
	{
		//todo:fix
		if (!blockInputHandler.HasRequest(m.PlayerTurnObject))
		{
			blockInputHandler.RestoreInputs(InputBlockFlag.All ^ InputBlockFlag.Select, this);
			PropagateBlockInputInfo();
			
			return;
		}
		blockInputHandler.RestoreInputs(InputBlockFlag.All ^ InputBlockFlag.Select, m.PlayerTurnObject);
		PropagateBlockInputInfo();
	}

	private void OnPlayerTurnEnd(PlayerTurnEndNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.PlayerTurnObject);
		PropagateBlockInputInfo();
	}

	private void PropagateBlockInputInfo()
	{
		foreach (var enemy in currentEnemyObjects)
		{
			enemy.UpdateBlockInput(blockInputHandler.BlockInput);
		}
	}
	
	public bool TrySpawnNextWave(out IUpdatableRoutine routine)
	{
		if (currentWaveIdx >= waveData.Count - 1)
		{
			routine = null;
			return false;
		}
		var gridInfoList = waveData[++currentWaveIdx];
		var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
		foreach (var cellInfo in gridInfoList.cells)
		{
			var card = BattleCardObjectInField.Instantiate(cellInfo.cardObject, map.GetTileAt(cellInfo.row, cellInfo.col),
				ObjectType.Enemy);
			card.transform.SetParent(waveParentTransform);
			currentEnemyObjects.Add(card);
			currentEnemyObjects[^1].UpdateBlockInput(blockInputHandler.BlockInput);
		}
		
		spawnNextWaveRoutine.Initialize();
		routine = spawnNextWaveRoutine;
		return true;
	}

	private void UpdateSpawn(float dt, out bool done)
	{
		done = true;
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
		NoticeSystem.Instance.Unsubscribe<PlayerTurnEndNotice>(OnPlayerTurnEnd);
		
		NoticeSystem.Instance.Unsubscribe<HandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Unsubscribe<HandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Unsubscribe<HandCardStartUseNotice>(OnCardStartUse);
		NoticeSystem.Instance.Unsubscribe<HandCardEndUseNotice>(OnCardEndUse);
		NoticeSystem.Instance.Subscribe<FieldCardSelectNotice>(OnFieldCardSelect);
		NoticeSystem.Instance.Subscribe<FieldCardSelectCancelNotice>(OnFieldCardSelectCancel);
		NoticeSystem.Instance.Subscribe<PlayerFieldCardMoveNotice>(OnPlayerFieldCardMove);
	}
	
	private void OnHandCardSelect(HandCardSelectNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.SelectedCard);
		PropagateBlockInputInfo();
	}

	private void OnHandCardSelectCancel(HandCardSelectCancelNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.SelectedCard);
		PropagateBlockInputInfo();
	}

	private void OnCardStartUse(HandCardStartUseNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.SelectedCard);
		PropagateBlockInputInfo();
	}

	private void OnCardEndUse(HandCardEndUseNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.SelectedCard);
		PropagateBlockInputInfo();
	}
	
	private void OnPlayerFieldCardMove(PlayerFieldCardMoveNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.Target);
		PropagateBlockInputInfo();
	}

	private void OnFieldCardSelect(FieldCardSelectNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.SelectedCard);
		PropagateBlockInputInfo();
	}

	private void OnFieldCardSelectCancel(FieldCardSelectCancelNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.SelectedCard);
		PropagateBlockInputInfo();
	}


}