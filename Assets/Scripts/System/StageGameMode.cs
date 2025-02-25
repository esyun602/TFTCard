
using System.Collections;
using Coroutine;

public abstract class StageGameMode : IGameMode
{
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
		currentStage.Load();
		OnInitialize();
		
		//todo: fix
		CoroutineManager.Instance.StartCoroutine(StartStage());
	}

	//todo: 싹다 코루틴으로?
	private IEnumerator StartStage()
	{
		yield return StageStartRoutine();
		currentStage.Start();
		OnStageStart();
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