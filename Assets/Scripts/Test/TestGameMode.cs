using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class TestGameMode : IGameMode
{
	public void Initialize()
	{
		Game.Instance.GetPlayer().CurrentPlayInfo.CurrentFlowInfo =	GameDataSystem.Instance.GetGameData<FlowGenData>().GetFlowSpec("TestFlow").GenerateFlow();
		foreach (var head in Game.Instance.GetPlayer().CurrentPlayInfo.CurrentFlowInfo.GetHeads())
		{
			head.OpenNode();
		}
		
		Game.Instance.ChangeGameMode(new FlowGameMode());
	}

	public void Dispose()
	{
	}
}
#endif