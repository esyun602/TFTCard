
using System.Collections;
using UnityEngine;
using Coroutine;
using WaitForSeconds = Coroutine.WaitForSeconds;

public class TestStage : StageBase
{
	private CardSpec cardSpec;
	public TestStage(CardSpec cardSpec, StageSpec stageSpec) : base(stageSpec)
	{
		this.cardSpec = cardSpec;
	}
	
	protected override void OnLoad()
	{
		base.OnLoad();
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new Card(GameDataSystem.Instance.GetGameData<CardData>().GetSpecById(0)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new Card(GameDataSystem.Instance.GetGameData<CardData>().GetSpecById(1)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new Card(GameDataSystem.Instance.GetGameData<CardData>().GetSpecById(2)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new Card(GameDataSystem.Instance.GetGameData<CardData>().GetSpecById(3)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new Card(GameDataSystem.Instance.GetGameData<CardData>().GetSpecById(4)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new Card(GameDataSystem.Instance.GetGameData<CardData>().GetSpecById(5)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new Card(GameDataSystem.Instance.GetGameData<CardData>().GetSpecById(6)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new Card(GameDataSystem.Instance.GetGameData<CardData>().GetSpecById(7)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new Card(GameDataSystem.Instance.GetGameData<CardData>().GetSpecById(8)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new Card(GameDataSystem.Instance.GetGameData<CardData>().GetSpecById(9)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new Card(GameDataSystem.Instance.GetGameData<CardData>().GetSpecById(10)));
		
	}

	protected override void OnStart()
	{
		CoroutineManager.Instance.StartCoroutine(Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().StageGameObject, TestStartRoutine());
	}

	private IEnumerator TestStartRoutine()
	{
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawCard();
		yield return new WaitForSeconds(1.0f);
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawCard();
		yield return new WaitForSeconds(1.0f);
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawCard();
		yield return new WaitForSeconds(1.0f);
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawCard();
		yield return new WaitForSeconds(1.0f);
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawCard();
		yield return new WaitForSeconds(1.0f);
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawCard();
		yield return new WaitForSeconds(1.0f);
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawCard();
		yield return new WaitForSeconds(1.0f);
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawCard();
		yield return new WaitForSeconds(1.0f);
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawCard();
		yield return new WaitForSeconds(1.0f);
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawCard();
		yield return new WaitForSeconds(1.0f);
	}


	protected override void OnUnLoad()
	{
		base.OnUnLoad();
	}

}