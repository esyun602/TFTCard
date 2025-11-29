using System;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private float actionDuration;
	private GameObject fxPrefab;
	private IBattleObject target;

	public FlamethrowerAction(FlamethrowerActionSpec spec) : base(spec)
	{
		actionDuration = spec.actionDuration;
		fxPrefab = spec.fxPrefab;
	}

	public override IEnumerable<ITile> Targets
	{
		get
		{
			yield return GetTarget();
		}
	}

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		routineDone = false;

		if (timePassed == 0)
		{
			BattleStat.Owner.AnimationController.RunAttackMotion();
		}
		timePassed += dt;
		if (timePassed > 0.2f && timePassed - dt < 0.2f)
		{
			var targetTile = GetTarget();
			if (targetTile != null)
			{
				var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
				var target = map.GetBattleObjectOfTile(targetTile);

				if (target?.ObjectType.IsHostile(BattleStat.Owner.ObjectType) == true)
				{
					target.UnitCardBattleStat.AddValueByValueType(UnitValueType.Burn, BattleStat.GetValueByValueType(UnitValueType.Attack));
				}
			}
		}
		else if (timePassed > 1f)
		{
			routineDone = true;
		}
	}
	
	private ITile GetTarget()
	{
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;

		return map.GetAttackTargetTile(BattleStat.Owner);
	}

	protected override void OnTrigger()
	{
		timePassed = 0f;
	}


	protected override void OnCancel()
	{
		canceled = true;
	}
}