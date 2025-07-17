public class DraftGameMode : IGameMode
{
	public void Initialize()
	{
		Game.Instance.SceneHandler.SetTransitionToNewScene(OnTransitionDone);
	}

	private void OnTransitionDone()
	{
		Game.Instance.UIManager.GenerateUI<StartDraft>();
	}
	
	public void Dispose()
	{
		
	}
}