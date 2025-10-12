using System.Collections.Generic;

public class NormanAction : UnitSkillCardActionBase
{
    private float timePassed;
    private bool canceled;
    private IBattleObject target;

    public NormanAction(NormanActionSpec spec) : base(spec)
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
        if (timePassed > 0f)
        {
            target.Damage(new DamageInfo()
            {
                DamageType = DamageType.NormalAttack,
                Dmg = BattleStat.GetValueByValueType(UnitValueType.Attack),
                Sender = BattleStat.Owner
            });
            
            if (target.IsDead())
            {
                Game.Instance.GetPlayer().CurrentPlayInfo.GainGold(BattleStat.GetValueByValueType(SkillValueType.GoldAdd));
            }
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