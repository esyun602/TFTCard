using System;
using System.Collections.Generic;
using UnityEngine;

public class OverheatPropulsionAction : UnitSkillCardActionBase
{
    private float timePassed;
    private bool canceled;
    private float actionDuration;
    private GameObject fxPrefab;
    private IBattleObject target;

    public OverheatPropulsionAction(OverheatPropulsionActionSpec spec) : base(spec)
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
					BattleStat.Owner.UnitCardBattleStat.AddValueByValueType(UnitValueType.Burn, BattleStat.GetValueByValueType(SkillValueType.BurnAdd));
					target.Damage(new DamageInfo()
					{
						DamageType = DamageType.NormalAttack,
						Sender = BattleStat.Owner,
						Dmg = BattleStat.GetValueByValueType(UnitValueType.Attack)
					});
				}
			}
		}
		else if (timePassed > 1.5f)
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
		BattleStat.Owner.AnimationController.RunAttackMotion();
		timePassed = 0f;
	}

    protected override void OnCancel()
    {
        canceled = true;
    }
}