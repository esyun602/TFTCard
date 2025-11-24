using System.Collections.Generic;
using System.Linq;

public class DerrickAction : UnitSkillCardActionBase
{
    private float timePassed;
    private bool canceled;
    private IBattleObject target;

    public override bool CanUse(ITile targetTile)
    {
        return base.CanUse(targetTile) && targetTile.TileType == ObjectType.Enemy;
    }

    public override IEnumerable<ITile> Targets
    {
        get
        {
            var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.BattleMap;
            return map.GetAllTilesInCol(ActionUtils.GetTargetObjectWithTargetingInfo(triggerInfo));
        }
    }

    public DerrickAction(DerrickActionSpec spec) : base(spec)
    {
    }

    //todo: 데미지 약간 지연? or 타일 정렬 약간 지연?
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
            var dmg = BattleStat.GetValueByValueType(UnitValueType.Attack);

            var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.BattleMap;

            var targetList = map.GetAllTilesInCol(target).Select(x => map.GetBattleObjectOfTile(x)).Where(x => x != null);
            
            foreach (var targetObj in targetList)
            {
                targetObj.Damage(new DamageInfo()
                {
                    DamageType = DamageType.Bomb,
                    Sender = BattleStat.Owner,
                    Dmg = dmg + BattleStat.GetValueByValueType(SkillValueType.BombDamage)
                });
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