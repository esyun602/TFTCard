
using System.Collections;
using UnityEngine;
using Coroutine;
using WaitForSeconds = Coroutine.WaitForSeconds;

public class TestStage : StageBase
{
	private UnitCardSpec unitCardSpec;
	public TestStage(UnitCardSpec unitCardSpec, StageSpec stageSpec) : base(stageSpec)
	{
		this.unitCardSpec = unitCardSpec;
	}
	
	protected override void OnLoad()
	{
		base.OnLoad();
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(0)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(1)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(2)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(3)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(4)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(5)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(6)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(7)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(8)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(9)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new SkillCard(GameDataSystem.Instance.GetGameData<CardData>().GetSkillCardSpecById(0)));
		
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