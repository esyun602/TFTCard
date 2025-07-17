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
		Game.Instance.GetPlayer().CurrentPlayInfo.DeckCardList.Add(new SkillCard(GameDataSystem.Instance.GetGameData<CardData>().GetSkillCardSpecById(0)));
		Game.Instance.GetPlayer().CurrentPlayInfo.DeckCardList.Add(new SkillCard(GameDataSystem.Instance.GetGameData<CardData>().GetSkillCardSpecById(0)));
		Game.Instance.GetPlayer().CurrentPlayInfo.DeckCardList.Add(new SkillCard(GameDataSystem.Instance.GetGameData<CardData>().GetSkillCardSpecById(1)));
		Game.Instance.GetPlayer().CurrentPlayInfo.DeckCardList.Add(new SkillCard(GameDataSystem.Instance.GetGameData<CardData>().GetSkillCardSpecById(1)));
		Game.Instance.GetPlayer().CurrentPlayInfo.DeckCardList.Add(new SkillCard(GameDataSystem.Instance.GetGameData<CardData>().GetSkillCardSpecById(2)));
		Game.Instance.GetPlayer().CurrentPlayInfo.DeckCardList.Add(new SkillCard(GameDataSystem.Instance.GetGameData<CardData>().GetSkillCardSpecById(2)));

		Game.Instance.ChangeGameMode(new DraftGameMode());
	}
	
	public void Dispose()
	{
		Game.Instance.UIManager.HideUI<MainMenuPanel>();
	}
}