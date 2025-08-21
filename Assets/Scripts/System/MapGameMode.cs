
using MessageSystem;
using UnityEngine;

public class MapGameMode : IGameMode
{
	public void Initialize()
	{
		Game.Instance.SceneHandler.SetTransitionToNewScene(OnTransitionDone);
		NoticeSystem.Instance.Subscribe<MapNodeSelectNotice>(StartTestStage);
	}

	private void OnTransitionDone()
	{
		Game.Instance.UIManager.GenerateUI<MapPanel>();
		Game.Instance.UIManager.GenerateUI<InGameInteraction>();
		var curNode = Game.Instance.GetPlayer().CurrentPlayInfo.CurrentSelectedNode;
		if (curNode?.NodeState == MapNodeState.Cleared)
		{
			//클리어 루틴?
		}
	}

	//todo: 메서드 type을 이렇게 나눌 필요가 있나?
	private void StartTestStage(MapNodeSelectNotice notice)
	{
		var stage = notice.TargetInfo.TargetStageSpec.InstantiateStage();
		//todo:fix
		if (stage is BattleStage)
		{
			Game.Instance.ChangeGameMode(new BattleStageGameMode(stage));
		}
		else
		{
			Game.Instance.ChangeGameMode(new UnitScoutStageGameMode(stage));
		}
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<MapNodeSelectNotice>(StartTestStage);
		//todo: remove?
		Game.Instance.UIManager.HideUI<MapPanel>();
	}
}