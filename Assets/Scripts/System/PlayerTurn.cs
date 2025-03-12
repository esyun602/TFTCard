
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
	public void Initialize()
	{
		NoticeSystem.Instance.Subscribe<TurnEndClickNotice>(OnTurnEndButtonClick);
		startRoutine = new UpdatableRoutine(UpdateTurnStart);
		turnRoutine = new UpdatableRoutine(UpdateTurn);
		currentRoutine = startRoutine;
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<TurnEndClickNotice>(OnTurnEndButtonClick);
	}

	private void OnTurnEndButtonClick(TurnEndClickNotice m)
	{
		EndTurn();
	}
	
	private void EndTurn()
	{
		playerActionDone = true;
		NoticeSystem.Instance.PublishSync(new PlayerTurnEndNotice(this));
	}


	public void StartTurn()
	{
		turnStartRoutineDone = false;
		currentRoutine = startRoutine;
		currentRoutine.Initialize();
		CoroutineManager.Instance.StartCoroutine(TurnStartRoutine());
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