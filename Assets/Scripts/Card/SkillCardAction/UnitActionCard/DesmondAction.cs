using System.Collections.Generic;
using System.Linq;

public class DesmondAction : UnitSkillCardActionBase
{
    private float timePassed;
    private bool canceled;
    private IBattleObject target;

    public DesmondAction(DesmondActionSpec spec) : base(spec)
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
            target.Damage(new DamageInfo()
            {
                DamageType = DamageType.NormalAttack,
                Dmg = BattleStat.GetValueByValueType(UnitValueType.Attack),
                Sender = BattleStat.Owner
            });
            
            if (target.IsDead())
            {
                BattleStat.Owner.UnitCardBattleStat.AddValueByValueType(UnitValueType.Attack, BattleStat.GetValuesByValueType(SkillValueType.AttackAdd)[0]);
            }
            else
            {
                BattleStat.Owner.UnitCardBattleStat.AddValueByValueType(UnitValueType.Attack, BattleStat.GetValuesByValueType(SkillValueType.AttackAdd)[1]);
            }
        }
        else if (timePassed > 1.5f)
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