using System;
using UnityEngine;

public class ReposeSkillAction : SkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private float actionDuration;
	private GameObject fxPrefab;
	private IBattleObject target;
	
	public ReposeSkillAction(ReposeSkillActionSpec spec)
	{
		actionDuration = spec.actionDuration;
		fxPrefab = spec.fxPrefab;
	}
	
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
			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.CardMoveCount++;
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