public class GameOverPanel : UIInstance
{
	//todo: popup or dont destroy?
	public override UIType UIType => UIType.SceneUI;
	protected override void Init(object param)
	{
		
	}
	
	public void OnCloseClick()
	{
		Game.Instance.ResetProgressInfo();
		Hide();
	}
}