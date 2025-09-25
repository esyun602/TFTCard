using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class TestGame : Game
{
	[SerializeField] private List<string> tacticsCardList;
	[SerializeField] private List<string> unitCardList;
	private void Start()
	{
		Invoke("StartTest", 0.5f);
	}

	private void StartTest()
	{
		Initialize();
		
		foreach (var cardName in tacticsCardList)
		{
			GetPlayer().CurrentPlayInfo.AddCard(new TacticsCard(GameDataSystem.Instance.GetGameData<CardData>().GetTacticsCardSpecByName(cardName)));
		}
		
		foreach (var cardName in unitCardList)
		{
			GetPlayer().CurrentPlayInfo.AddCard(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecByName(cardName)));
		}
		
		ChangeGameMode(new TestGameMode());
	}
}
#endif