
using System.Collections;
using UnityEngine;
using Coroutine;
using WaitForSeconds = Coroutine.WaitForSeconds;

public class TestStage : StageBase
{
	private CardData cardData;
	public TestStage(CardData cardData, StageData stageData) : base(stageData)
	{
		this.cardData = cardData;
	}
	
	protected override void OnLoad()
	{
		base.OnLoad();
		Game.Instance.GetPlayer().CardList.Add(new Card(cardData));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardData));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardData));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardData));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardData));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardData));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardData));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardData));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardData));
		Game.Instance.GetPlayer().CardList.Add(new Card(cardData));
		
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