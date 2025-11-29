using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DustinAction : UnitSkillCardActionBase
{
    private float timePassed;
    private bool canceled;
    private IBattleObject target;

    public DustinAction(DustinActionSpec spec) : base(spec)
    {
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
        if (timePassed > 0.2f && timePassed - dt < 0.2f)
        {
            if (Random.value > 0.5f)
            {
                target.UnitCardBattleStat.AddValueByValueType(UnitValueType.Attack, BattleStat.GetValueByValueType(UnitValueType.Attack));
            }
            else
            {
                target.UnitCardBattleStat.AddValueByValueType(UnitValueType.Shield, BattleStat.GetValueByValueType(UnitValueType.Attack));
            }
        }
        else if (timePassed > 1f)
        {
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