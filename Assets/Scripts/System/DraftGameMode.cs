public class DraftGameMode : IGameMode
{
	public bool LoadComplete { get; private set; }
	public void Initialize()
	{
		LoadComplete = false;
		Game.Instance.SceneHandler.SetTransitionToNewScene(OnTransitionDone);
	}

	private void OnTransitionDone()
	{
		Game.Instance.UIManager.GenerateUI<StartDraft>(new StartDraftGenState()
		{
			DraftCount = 3,
			CardPerDraft = 2,
			DoneAction = ReturnToMap
		});
		LoadComplete = true;
	}

	public void ReturnToMap()
	{
		Game.Instance.ChangeGameMode(new FlowGameMode());
	}
	
	public void Dispose()
	{
		
	}
}