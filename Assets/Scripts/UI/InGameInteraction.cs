
public class InGameInteraction : UIInstance
{
	public override UIType UIType => UIType.SceneUI;
	protected override void Init(object param)
	{
		
	}

	public void OpenBagPanel()
	{
		Game.Instance.UIManager.GenerateUI<PlayerBagPanel>();
	}
}