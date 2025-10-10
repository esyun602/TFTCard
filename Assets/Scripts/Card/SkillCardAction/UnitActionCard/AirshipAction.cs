using System.Collections.Generic;
using System.Linq;

public class AirshipAction : UnitSkillCardActionBase
{
    private float timePassed = 0f;
    public AirshipAction(AirshipActionSpec spec) : base(spec)
    {
    }

    public override IEnumerable<ITile> Targets => new ITile[] { };
    protected override void OnUpdate(float dt, out bool routineDone)
    {
        routineDone = false;

        timePassed += dt;
        if (timePassed > 0.15f && timePassed - dt < 0.15f)
        {
            for (var i = 0; i < BattleStat.GetValueByValueType(SkillValueType.AttackCount); i++)
            {
                var target = Game.Instance.GetGameMode<BattleStageGameMode>().BattleFieldSystem.GetRandomBattleObject(BattleStat.Owner.ObjectType.GetOpposite());
                target?.Damage(new DamageInfo()
                {
                    DamageType = DamageType.Bomb,
                    Dmg = BattleStat.GetValueByValueType(UnitValueType.Attack),
                    Sender = BattleStat.Owner
                });
            }

            BattleStat.Owner.UnitCardBattleStat.RemoveBuff<ValueAddAttackBuff>();
            BattleStat.Owner.UnitCardBattleStat.AddValueByValueType(SkillValueType.AttackCount, -9999, 0);
        }
        else if (timePassed > 1.5f)
        {
            routineDone = true;
        }
    }

    protected override void OnTrigger()
    {
        BattleStat.Owner.AnimationController.RunAttackMotion();
        timePassed = 0f;
    }

    protected override void OnCancel()
    {
		
    }
}