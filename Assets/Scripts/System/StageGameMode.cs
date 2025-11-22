
using System;
using System.Collections;
using Coroutine;
using MessageSystem;

public class StageGameMode : IGameMode
{
	public bool LoadComplete { get; private set; }
	private readonly IStage currentStage;

	public StageGameMode(IStage targetStage)
	{
		currentStage = targetStage;
	}
	
	public IStage GetCurrentStage()
	{
		return currentStage;
	}

	public void Initialize()
	{
		LoadComplete = false;
		Game.Instance.SceneHandler.SetTransitionToNewScene(OnTransitionDone);
	}

	private void OnTransitionDone()
	{
		currentStage.Load();
		//todo: fix
		OnInitialize();
		CoroutineManager.Instance.StartCoroutine(StartStage());
		LoadComplete = true;
	}

	//todo: 싹다 코루틴으로?
	private IEnumerator StartStage()
	{
		yield return StageStartRoutine();
		currentStage.Start();
		OnStageStart();
	}

	public void ClearStage()
	{
		SfxManager.Instance.Play2D("win_battle");
		BgmManager.Instance.ChangeBgm("");
		NoticeSystem.Instance.Publish(new StageClearNotice());
	}

	public void GameOver()
	{
		SfxManager.Instance.Play2D("game_over");
		BgmManager.Instance.ChangeBgm("");
		NoticeSystem.Instance.Publish(new GameOverNotice());
	}

	protected virtual IEnumerator StageStartRoutine()
	{
		yield return null;
	}

	protected virtual void OnInitialize()
	{
		
	}

	protected virtual void OnStageStart()
	{
		
	}

	public void Dispose()
	{
		currentStage.UnLoad();
		OnDispose();
	}

	protected virtual void OnDispose()
	{
		
	}
}