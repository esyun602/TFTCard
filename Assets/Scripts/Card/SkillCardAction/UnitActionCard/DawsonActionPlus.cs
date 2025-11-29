using System.Collections.Generic;
using System.Linq;

public class DawsonActionPlus : UnitSkillCardActionBase
{
    private float timePassed = 0f;

    public DawsonActionPlus(DawsonActionPlusSpec spec) : base(spec)
    {
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
        if (timePassed > 0.15f && timePassed - dt < 0.15f)
        {
            var targetTile = GetTarget();
            if (targetTile != null)
            {
                var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
                var target = map.GetBattleObjectOfTile(targetTile);

                if (target?.ObjectType.IsHostile(BattleStat.Owner.ObjectType) == true)
                {
                    target.Damage(new DamageInfo()
                    {
                        DamageType = DamageType.NormalAttack,
                        Sender = BattleStat.Owner,
                        Dmg = BattleStat.GetValueByValueType(UnitValueType.Attack)
                    });
                }
                
                Game.Instance.GetGameMode<BattleStageGameMode>().BattleGlobalModifier.DropBlockAction.Enqueue(Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.ReturnHandCardToHand);
                
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
    }
}