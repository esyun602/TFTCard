
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

public class WaveSystem
{
	private int currentWaveIdx;
	private IUpdatableRoutine spawnNextWaveRoutine;
	private List<WaveSpec> waveData;
	private List<UnitCardInField> currentEnemyObjects;
	private List<UnitCardInField> currentSpawnedEnemyObjects;
	private BlockInputHandler blockInputHandler = new();
	private Transform waveParentTransform;
	private int WaveSpawnedTurnCount;
	
	public WaveSystem(List<WaveSpec> waveData)
	{
		this.waveData = waveData;
	}

	public void Initialize()
	{
		currentEnemyObjects = new();
		currentSpawnedEnemyObjects = new();
		spawnNextWaveRoutine = new UpdatableRoutine(UpdateSpawnNextWave, InitializeSpawnWaveRoutine);
		blockInputHandler.BlockInputs(InputBlockFlag.All, this);
		NoticeSystem.Instance.Subscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
		NoticeSystem.Instance.Subscribe<PlayerTurnEndNotice>(OnPlayerTurnEnd);		
		//todo: need inputsystem
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Subscribe<FieldCardSelectNotice>(OnFieldCardSelect);
		NoticeSystem.Instance.Subscribe<FieldCardSelectCancelNotice>(OnFieldCardSelectCancel);
		NoticeSystem.Instance.Subscribe<PlayerFieldCardMoveNotice>(OnPlayerFieldCardMove);
		NoticeSystem.Instance.Subscribe<SkillHandCardStartUseNotice>(OnCardStartUse);
		NoticeSystem.Instance.Subscribe<SkillHandCardEndUseNotice>(OnCardEndUse);
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

	public void SpawnInitialWave(out IUpdatableRoutine routine)
	{
		WaveSpawnedTurnCount = 1;
		currentWaveIdx = 0;
		SpawnWaveImpl(waveData[0]);
		
		spawnNextWaveRoutine.Initialize();
		routine = spawnNextWaveRoutine;
	}
	
	public bool TrySpawnNextWave(out IUpdatableRoutine routine)
	{
		if (currentWaveIdx >= waveData.Count - 1)
		{
			routine = null;
			return false;
		}

		WaveSpawnedTurnCount = Game.Instance.GetGameMode<BattleStageGameMode>().TurnSystem.CurrentTurnCount;
		var gridInfoList = waveData[++currentWaveIdx];

		SpawnWaveImpl(gridInfoList);

		spawnNextWaveRoutine.Initialize();
		routine = spawnNextWaveRoutine;
		return true;
	}

	private void InitializeSpawnWaveRoutine()
	{
		spawnTimePassed = 0f;
		foreach (var enemy in currentSpawnedEnemyObjects)
		{
			enemy.AnimationController.RunEnemySpawnAction();
		}
	}

	private void SpawnWaveImpl(WaveSpec spec)
	{
		currentSpawnedEnemyObjects.Clear();
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
		bool carryOver = false;
		foreach (var cellInfo in spec.CellList)
		{
			if (carryOver)
			{
				waveData[currentWaveIdx + 1].CellList.Add(cellInfo);
			}
			else
			{
				var cardSpec = GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecByName(cellInfo.UnitCardName);
				var tile = DetermineTargetTile(map.GetTileAt(cellInfo.Row, cellInfo.Col));
				if (tile == null)
				{
					carryOver = true;
					if (waveData.Count >= currentWaveIdx + 1)
					{
						var carryOverSpec = WaveSpec.CreateForCarryOver();
						waveData.Add(carryOverSpec);
					}
				
					waveData[currentWaveIdx + 1].CellList.Add(cellInfo);
					continue;
				}
				var card = UnitCardInField.Instantiate(cardSpec, tile, ObjectType.Enemy);
				card.transform.SetParent(waveParentTransform);
				currentSpawnedEnemyObjects.Add(card);
				currentEnemyObjects.Add(card);
				currentEnemyObjects[^1].UpdateBlockInput(blockInputHandler.BlockInput);
			}
		}
		
	}

	private ITile DetermineTargetTile(ITile tile)
	{
		var ret = DetermineTargetTileRow(tile);

		if (ret == null)
		{
			var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
			var (row, _) = map.GetTileCoord(tile);
			if (row == 0)
			{
				ret = DetermineTargetTileRow(map.GetUpwardTile(tile)) ?? DetermineTargetTileRow(map.GetUpwardTile(tile));
			}
			else
			{
				ret = DetermineTargetTileRow(map.GetDownwardTile(tile)) ?? DetermineTargetTileRow(map.GetUpwardTile(tile));
			}
		}
		
		return ret;
	}

	private ITile DetermineTargetTileRow(ITile tile)
	{
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
		if (map.GetBattleObjectOfTile(tile) == null)
		{
			return tile;
		}

		var ret = tile;
		while (ret != null && map.GetBattleObjectOfTile(ret) != null)
		{
			ret = map.GetBackwardTile(ret);
		}

		if (ret == null)
		{
			ret = tile;
			while (ret != null && map.GetBattleObjectOfTile(ret) != null)
			{
				ret = map.GetForwardTile(ret);
			}
		}
		
		return ret;
	}

	public bool IsSatisfySpawnWaveCondition()
	{
		if (IsInLastWave)
		{
			return false;
		}

		return waveData[currentWaveIdx + 1].PrepareTurn <=
		       Game.Instance.GetGameMode<BattleStageGameMode>().TurnSystem.CurrentTurnCount
		       - WaveSpawnedTurnCount;
	}

	public int LeftNextWaveTurn => IsInLastWave ? -1 : waveData[currentWaveIdx + 1].PrepareTurn 
	                               - (Game.Instance.GetGameMode<BattleStageGameMode>().TurnSystem.CurrentTurnCount
										- WaveSpawnedTurnCount);

	public bool IsInLastWave => currentWaveIdx == waveData.Count - 1;

	private float spawnTimePassed = 0f;
	private void UpdateSpawnNextWave(float dt, out bool done)
	{
		spawnTimePassed += dt;
		done = spawnTimePassed > 1f;
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
		NoticeSystem.Instance.Unsubscribe<PlayerTurnEndNotice>(OnPlayerTurnEnd);
		
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardStartUseNotice>(OnCardStartUse);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardEndUseNotice>(OnCardEndUse);
		NoticeSystem.Instance.Unsubscribe<FieldCardSelectNotice>(OnFieldCardSelect);
		NoticeSystem.Instance.Unsubscribe<FieldCardSelectCancelNotice>(OnFieldCardSelectCancel);
		NoticeSystem.Instance.Unsubscribe<PlayerFieldCardMoveNotice>(OnPlayerFieldCardMove);

		foreach (var obj in currentEnemyObjects)
		{
			obj.Dispose();
		}
	}
	
	private void OnHandCardSelect(SkillHandCardSelectNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.SelectedCard);
		PropagateBlockInputInfo();
	}

	private void OnHandCardSelectCancel(SkillHandCardSelectCancelNotice m)
	{
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.SelectedCard);
		PropagateBlockInputInfo();
	}

	private void OnCardStartUse(SkillHandCardStartUseNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.SelectedCard);
		PropagateBlockInputInfo();
	}

	private void OnCardEndUse(SkillHandCardEndUseNotice m)
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