
using MessageSystem;
using UnityEngine;

public class FlowGameMode : IGameMode
{
	public bool LoadComplete { get; private set; }

	public void Initialize()
	{
		LoadComplete = false;
		Game.Instance.SceneHandler.SetTransitionToNewScene(OnTransitionDone);
		NoticeSystem.Instance.Subscribe<FlowNodeSelectNotice>(StartTestStage);
	}

	private void OnTransitionDone()
	{
		Game.Instance.UIManager.GenerateUI<FlowPanel>(new FlowPanelGenState()
		{
			FlowInfo = Game.Instance.GetPlayer().CurrentPlayInfo.CurrentFlowInfo
		});
		
		if (InGameInteraction.Instance == null)
		{
			Game.Instance.UIManager.GenerateUI<InGameInteraction>();
		}
		
		var curNode = Game.Instance.GetPlayer().CurrentPlayInfo.CurrentSelectedNode;
		if (curNode?.NodeState == FlowNodeState.Cleared)
		{
			//클리어 루틴?
		}

		BgmManager.Instance.ChangeBgm("flow");
		LoadComplete = true;
	}

	//todo: 메서드 type을 이렇게 나눌 필요가 있나?
	private void StartTestStage(FlowNodeSelectNotice notice)
	{
		var stage = notice.TargetInfo.TargetStageSpec.InstantiateStage();
		//todo:fix
		if (stage is BattleStage)
		{
			Game.Instance.ChangeGameMode(new BattleStageGameMode(stage));
		}
		else
		{
			Game.Instance.ChangeGameMode(new StageGameMode(stage));
		}
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<FlowNodeSelectNotice>(StartTestStage);
	}
}