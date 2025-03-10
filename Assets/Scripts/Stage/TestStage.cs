
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
		Game.Instance.GetPlayer().CardList.Add(new Card(cardSpec));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardSpec));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardSpec));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardSpec));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardSpec));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardSpec));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardSpec));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardSpec));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardSpec));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardSpec));
		
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