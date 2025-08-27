using System;
using UnityEngine;

public class StockSkillAction : TacticsCardActionBase
{
	private float timePassed;
	private bool canceled;
	private float actionDuration;
	private GameObject fxPrefab;
	private IBattleObject target;

	public StockSkillAction(StockSkillActionSpec spec)
	{
		actionDuration = spec.actionDuration;
		fxPrefab = spec.fxPrefab;
	}

	public override object[] DescParams => new object[] { StatFallback.GetValueByValueType(CommonValueType.Draw) };

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		if (canceled)
		{
			routineDone = true;
			return;
		}

		routineDone = false;

		timePassed += dt;
		if (timePassed > 0f)
		{
			for (var i = 0; i < BattleStat.GetValueByValueType(CommonValueType.Draw); i++)
			{
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawPlayerCard();
			}
			routineDone = true;
		}
	}

	protected override void OnTrigger(object triggerInfo)
	{
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}