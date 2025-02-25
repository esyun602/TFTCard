
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
		currentWaveIdx = -1;
	}

	private void OnPlayerTurnStart(PlayerTurnStartNotice m)
	{
		//todo:fix
		if (!blockInputHandler.HasRequest(m.PlayerTurnObject))
		{
			blockInputHandler.RestoreInputs(InputBlockFlag.All, this);
			
			foreach (var enemy in currentEnemyObjects)
			{
				enemy.UpdateBlockInput(blockInputHandler.BlockInput);
			}
			
			return;
		}
		blockInputHandler.RestoreInputs(InputBlockFlag.All, m.PlayerTurnObject);
	}

	private void OnPlayerTurnEnd(PlayerTurnEndNotice m)
	{
		blockInputHandler.BlockInputs(InputBlockFlag.All, m.PlayerTurnObject);
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
			currentEnemyObjects.Add(BattleCardObjectInField.Instantiate(cellInfo.cardObject, map.GetTileAt(cellInfo.row, cellInfo.col), ObjectType.Enemy));
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
	}
}