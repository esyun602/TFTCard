using System;

public class MainMenuPanelGenState
{
	public Action GameStartAction { get; set; }
	public Action GameEndAction { get; set; }
}

public class MainMenuPanel : UIInstance
{
	public override UIType UIType { get; } = UIType.SceneUI;
	private MainMenuPanelGenState state;

	protected override void Init(object param)
	{
		if (param is MainMenuPanelGenState state)
		{
			this.state = state;
		}
	}

	public void OnStartButtonClick()
	{
		state.GameStartAction?.Invoke();
	}

	public void OnLoadButtonClick()
	{
	}

	public void OnOptionButtonClick()
	{
	}

	public void OnExitButtonClick()
	{
		state.GameEndAction?.Invoke();
	}
}