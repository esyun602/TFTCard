
using MessageSystem;

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
		//todo: fix
		var waveSpecList = GameDataSystem.Instance.GetGameData<WaveData>().GetMultipleWaveSpec(((TestStageSpec)notice.TargetInfo.TargetStageSpec).WaveGridList);
		
		Game.Instance.ChangeGameMode(new BattleStageGameMode(waveSpecList, stage));
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<MapNodeSelectNotice>(StartTestStage);
		//todo: remove?
		Game.Instance.UIManager.HideUI<MapPanel>();
		Game.Instance.UIManager.RemoveUI<InGameInteraction>();
	}
}