using MessageSystem;
using UnityEngine;
//todo; 임시구현
public class ShopStage : StageBase
{
	protected override void OnLoad()
	{
		Game.Instance.UIManager.GenerateUI<ShopUIPanel>(new ShopUIPanelGenState()
		{
			doneAction = StageDone
		});
		BgmManager.Instance.ChangeBgm("shop");
	}
    
	private void StageDone()
	{
		NoticeSystem.Instance.Publish(new StageClearNotice());
		Game.Instance.ChangeGameMode(new FlowGameMode());
	}

	public ShopStage(StageSpec stageSpec) : base(stageSpec)
	{
	}
}