using System;

public class VictoryPanelGenState
{
    public VictoryPanelGenState(Action returnToMapAction)
    {
        ReturnToMapAction = returnToMapAction;
    }

    public Action ReturnToMapAction { get; }
}

public class VictoryPanel : UIInstance
{
    public override UIType UIType => UIType.SceneUI;
    private VictoryPanelGenState genState;
    protected override void Init(object param)
    {
        if (param is not VictoryPanelGenState state)
        {
            throw new ArgumentException("param is not VictoryPanelGenState");
        }
        
        genState = state;
    }
	
    public void OnCloseClick()
    {
        Game.Instance.UIManager.GenerateUI<ShopUIPanel>(new ShopUIPanelGenState()
        {
            rollCount = 3,
            doneAction = genState.ReturnToMapAction
        });
        Hide();
    }
}