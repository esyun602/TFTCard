
using MessageSystem;

public class MapGameMode : IGameMode
{
	public void Initialize()
	{
		NoticeSystem.Instance.Subscribe<MapNodeSelectNotice>(StartTestStage);
		Game.Instance.UIManager.GenerateUI<MapPanel>();
	}

	//todo: 메서드 type을 이렇게 나눌 필요가 있나?
	private void StartTestStage(MapNodeSelectNotice notice)
	{
		var stage = notice.TargetSpec.InstantiateStage();
		Game.Instance.ChangeGameMode(new BattleStageGameMode(((TestStageSpec)notice.TargetSpec).WaveData, stage));
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<MapNodeSelectNotice>(StartTestStage);
		Game.Instance.UIManager.HideUI<MapPanel>();
	}
}