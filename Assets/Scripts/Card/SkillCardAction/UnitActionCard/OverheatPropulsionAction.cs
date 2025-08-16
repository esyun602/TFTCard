using System;
using UnityEngine;

public class OverheatPropulsionAction : UnitSkillCardActionBase
{
    private float timePassed;
    private bool canceled;
    private float actionDuration;
    private GameObject fxPrefab;
    private IBattleObject target;

    public OverheatPropulsionAction(OverheatPropulsionActionSpec spec)
    {
        actionDuration = spec.actionDuration;
        fxPrefab = spec.fxPrefab;
    }

    public override object[] DescParams => new object[] { StatFallback.GetValueByValueType(BattleValueType.Burn), StatFallback.GetValueByValueType(BattleValueType.Attack) };

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
            battleStat.Owner.UnitCardBattleStat.AddBuff(new BurnBuff(battleStat.GetValueByValueType(BattleValueType.Burn)));
            target.Damage(
                new DamageInfo()
                {
                    Sender = battleStat.Owner,
                    Dmg = StatFallback.GetValueByValueType(BattleValueType.Attack)
                });

            routineDone = true;
        }
    }

    protected override void OnTrigger(object triggerInfo)
    {
        timePassed = 0f;
        if (triggerInfo is not TargetingActionTriggerInfo ti)
        {
            throw new ArgumentException();
        }

        target = ti.Target;
    }

    protected override void OnCancel()
    {
        canceled = true;
    }
}