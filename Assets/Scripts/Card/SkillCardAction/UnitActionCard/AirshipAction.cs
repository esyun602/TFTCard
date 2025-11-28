using System.Collections.Generic;
using System.Linq;

public class AirshipAction : UnitSkillCardActionBase
{
    private int currentIdx;
    private float timePassed = 0f;
    public AirshipAction(AirshipActionSpec spec) : base(spec)
    {
    }

    public override IEnumerable<ITile> Targets => new ITile[] { };
    protected override void OnUpdate(float dt, out bool routineDone)
    {
        routineDone = false;
        
        timePassed += dt;
        if (timePassed > 0.5f && timePassed - dt < 0.5f && currentIdx < BattleStat.GetValueByValueType(SkillValueType.AttackCount))
        {
            var target = Game.Instance.GetGameMode<BattleStageGameMode>().BattleFieldSystem.GetRandomBattleObject(BattleStat.Owner.ObjectType.GetOpposite());
            target?.Damage(new DamageInfo()
            {
                DamageType = DamageType.Bomb,
                Dmg = BattleStat.GetValueByValueType(UnitValueType.Attack),
                Sender = BattleStat.Owner
            });
            
            if (++currentIdx < BattleStat.GetValueByValueType(SkillValueType.AttackCount))
            {
                timePassed = 0;
            }
            else
            {
                BattleStat.Owner.UnitCardBattleStat.RemoveBuff<ValueAddAttackBuff>();
                BattleStat.Owner.UnitCardBattleStat.AddValueByValueType(SkillValueType.AttackCount, -9999, 0);
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
        currentIdx = 0;
        BattleStat.Owner.AnimationController.RunAttackMotion();
    }

    protected override void OnCancel()
    {
		
    }
}