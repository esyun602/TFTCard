
using System;
using System.Collections;
using Coroutine;
using MessageSystem;

/// <summary>
/// 턴 시작 시 - 카드 사용 가능해짐
///           - todo: 카드 이동 가능해짐
/// 턴 종료 시 - 카드 사용 막음
///           - todo: 카드 이동 막음
/// </summary>

public class PlayerTurn : IDisposable
{
	private bool turnStartRoutineDone;
	private bool playerActionDone;
	private IUpdatableRoutine startRoutine;
	private IUpdatableRoutine turnRoutine;
	private IUpdatableRoutine currentRoutine;
	public IUpdatableRoutine UpdatableCurrentRoutine => currentRoutine;
	//one-base
	public int CurrentTurnCount { get; private set; }
	public void Initialize()
	{
		startRoutine = new UpdatableRoutine(UpdateTurnStart, () => CoroutineManager.Instance.StartCoroutine(TurnStartRoutine()));
		turnRoutine = new UpdatableRoutine(UpdateTurn);
		currentRoutine = startRoutine;
		CurrentTurnCount = 0;
	}

	public void Dispose()
	{
	}
	
	public void EndTurn()
	{
		playerActionDone = true;
	}


	public void StartTurn()
	{
		CurrentTurnCount++;
		turnStartRoutineDone = false;
		
		var waveSystem = Game.Instance.GetGameMode<BattleStageGameMode>().WaveSystem;
		if (Game.Instance.GetGameMode<BattleStageGameMode>().WaveSystem.IsSatisfySpawnWaveCondition())
		{
			if(waveSystem.TrySpawnNextWave(out var routine))
			{
				currentRoutine = routine;
				routine.AddChain(startRoutine);
			}
		}
		else
		{
			currentRoutine = startRoutine;
			currentRoutine.Initialize();
		}
	}

	private void UpdateTurnStart(float dt, out bool routineDone)
	{
		if (turnStartRoutineDone)
		{
			routineDone = true;
			playerActionDone = false;
			currentRoutine = turnRoutine;
			currentRoutine.Initialize();
			NoticeSystem.Instance.Publish(new PlayerTurnStartNotice(this));
			return;
		}

		routineDone = false;
	}

	private IEnumerator TurnStartRoutine()
	{
		turnStartRoutineDone = false;
		yield return new WaitForSeconds(1.0f);
		turnStartRoutineDone = true;
	}
	
	private void UpdateTurn(float dt, out bool routineDone)
	{
		routineDone = playerActionDone;
	}
}