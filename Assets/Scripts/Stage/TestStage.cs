
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
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecById(4)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new SkillCard(GameDataSystem.Instance.GetGameData<CardData>().GetSkillCardSpecById(0)));
		Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new SkillCard(GameDataSystem.Instance.GetGameData<CardData>().GetSkillCardSpecById(1)));
		
	}

	protected override void OnStart()
	{
		CoroutineManager.Instance.StartCoroutine(Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().StageGameObject, TestStartRoutine());
	}

	private IEnumerator TestStartRoutine()
	{
		yield return new WaitForSeconds(3.0f);
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