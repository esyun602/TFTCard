
using System;
using MessageSystem;

/// <summary>
/// 턴 시작 시 - 카드 사용 가능해짐
///           - todo: 카드 이동 가능해짐
/// 턴 종료 시 - 카드 사용 막음
///           - todo: 카드 이동 막음
/// </summary>

public class PlayerTurn : ITurnObject, IDisposable
{
	private bool playerActionDone;
	
	public void Initialize()
	{
		NoticeSystem.Instance.Subscribe<HandCardEndUseNotice>(OnCardUse);
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<HandCardEndUseNotice>(OnCardUse);
	}

	private void OnCardUse(HandCardEndUseNotice m)
	{
		playerActionDone = true;
		NoticeSystem.Instance.PublishSync(new PlayerTurnEndNotice(this));
	}

	public void StartTurn()
	{
		playerActionDone = false;
		NoticeSystem.Instance.Publish(new PlayerTurnStartNotice(this));
	}

	public void UpdateFrame(float dt, out bool routineDone)
	{
		routineDone = playerActionDone;
	}

	//todo: fix
	public float TurnSpeed => 10f;
	public void AddChain(IUpdatableRoutine routine)
	{
		throw new NotImplementedException();
	}

	public void RemoveChain(IUpdatableRoutine routine)
	{
		throw new NotImplementedException();
	}
}