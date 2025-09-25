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

    public override IEnumerable<ITile> Targets => ActionUtils.GetTargetTileWithTargetingInfo(triggerInfo);


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
            BattleStat.Owner.UnitCardBattleStat.AddBuff(new BurnBuff(BattleStat.GetValueByValueType(SkillValueType.BurnAdd)));
            target.Damage(
                new DamageInfo()
                {
                    Sender = BattleStat.Owner,
                    Dmg = StatFallback.GetValueByValueType(UnitValueType.Attack)
                });

            routineDone = true;
        }
    }


    protected override void OnTrigger()
    {
        timePassed = 0f;
        target = ActionUtils.GetTargetObjectWithTargetingInfo(triggerInfo);
    }

    protected override void OnCancel()
    {
        canceled = true;
    }
}