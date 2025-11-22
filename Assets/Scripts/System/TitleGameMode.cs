public class TitleGameMode : IGameMode
{
	public bool LoadComplete { get; private set; }
	public void Initialize()
	{
		LoadComplete = false;
		Game.Instance.SceneHandler.SetTransitionToNewScene(OnTransitionDone);
	}

	private void OnTransitionDone()
	{
		Game.Instance.UIManager.GenerateUI<MainMenuPanel>(new MainMenuPanelGenState()
		{
			GameStartAction = StartGame,
		});
		BgmManager.Instance.ChangeBgm("main");
		LoadComplete = true;
	}

	private void StartGame()
	{
		//todo: 카드 추가 관련 위치 수정
		Game.Instance.GetPlayer().CurrentPlayInfo.AddCard(new TacticsCard(GameDataSystem.Instance.GetGameData<CardData>().GetTacticsCardSpecByName("Damage")));
		Game.Instance.GetPlayer().CurrentPlayInfo.AddCard(new TacticsCard(GameDataSystem.Instance.GetGameData<CardData>().GetTacticsCardSpecByName("Damage")));
		Game.Instance.GetPlayer().CurrentPlayInfo.AddCard(new TacticsCard(GameDataSystem.Instance.GetGameData<CardData>().GetTacticsCardSpecByName("Shield")));
		Game.Instance.GetPlayer().CurrentPlayInfo.AddCard(new TacticsCard(GameDataSystem.Instance.GetGameData<CardData>().GetTacticsCardSpecByName("Shield")));
		Game.Instance.GetPlayer().CurrentPlayInfo.AddCard(new TacticsCard(GameDataSystem.Instance.GetGameData<CardData>().GetTacticsCardSpecByName("Repose")));
		Game.Instance.GetPlayer().CurrentPlayInfo.AddCard(new TacticsCard(GameDataSystem.Instance.GetGameData<CardData>().GetTacticsCardSpecByName("Repose")));
		Game.Instance.GetPlayer().CurrentPlayInfo.AddCard(new TacticsCard(GameDataSystem.Instance.GetGameData<CardData>().GetTacticsCardSpecByName("FireArrow")));

		var info =	GameDataSystem.Instance.GetGameData<FlowGenData>().GetFlowSpec("EnemyFlow").GenerateFlow();
		foreach (var head in info.GetHeads())
		{
			head.OpenNode();
		}
		
		Game.Instance.GetPlayer().CurrentPlayInfo.CurrentFlowInfo = info;

		Game.Instance.ChangeGameMode(new DraftGameMode());
	}
	
	public void Dispose()
	{
		Game.Instance.UIManager.HideUI<MainMenuPanel>();
	}
}