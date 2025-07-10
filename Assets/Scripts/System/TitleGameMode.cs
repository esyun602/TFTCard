public class TitleGameMode : IGameMode
{
	public void Initialize()
	{
		Game.Instance.SceneHandler.SetTransitionToNewScene(OnTransitionDone);
	}

	private void OnTransitionDone()
	{
		Game.Instance.UIManager.GenerateUI<MainMenuPanel>(new MainMenuPanelGenState()
		{
			GameStartAction = StartGame,
		});
	}

	private void StartGame()
	{
		//todo: 일단 바로 맵 씬으로
		//todo: transition 추가해서 수정
		//todo: transition 시 uimanager 동작도 체크
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(0)));
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(1)));
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(2)));
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(3)));
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(4)));
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(5)));
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(6)));
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(7)));
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(8)));
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(9)));
		Game.Instance.GetPlayer().CurrentPlayInfo.DeckCardList.Add(new SkillCard(GameDataSystem.Instance.GetGameData<CardData>().GetSkillCardSpecById(0)));

		Game.Instance.ChangeGameMode(new MapGameMode());
	}
	
	public void Dispose()
	{
		Game.Instance.UIManager.HideUI<MainMenuPanel>();
	}
}