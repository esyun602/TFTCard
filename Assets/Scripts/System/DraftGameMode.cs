public class DraftGameMode : IGameMode
{
	public void Initialize()
	{
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
	}

	public void ReturnToMap()
	{
		Game.Instance.ChangeGameMode(new MapGameMode());
	}
	
	public void Dispose()
	{
		
	}
}