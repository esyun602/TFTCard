
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
		currentStage.Start();
	}

	protected virtual void OnInitialize()
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