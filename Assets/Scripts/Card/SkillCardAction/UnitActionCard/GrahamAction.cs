using System.Collections.Generic;
using System.Linq;

public class GrahamAction : UnitSkillCardActionBase
{
    private float timePassed;
    private bool canceled;

    public GrahamAction(GrahamActionSpec spec) : base(spec)
    {
    }

    public override IEnumerable<ITile> Targets
    {
        get
        {
            var airShipBo = GetAirshipBo();
            if (airShipBo == null || airShipBo.IsDead())
            {
                yield break;
            }

            yield return Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map.GetTileOfBattleObject(airShipBo);

        }
    }

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
            Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawPlayerCard();
            var airShip = GetAirshipBo();
            if (airShip != null && !airShip.IsDead())
            {
                airShip.UnitCardBattleStat.AddValueByValueType(SkillValueType.AttackCount, BattleStat.GetValueByValueType(UnitValueType.Attack));
            }
			
            routineDone = true;
        }
    }

    private IBattleObject GetAirshipBo()
    {
        var airShip = Game.Instance.GetPlayer().CurrentPlayInfo.GetGlobalSynergy<AeronautSynergy>(SynergyCategory.Aeronaut)?.AirShip;
        if (airShip == null) return null;
            
        var gameMode = Game.Instance.GetGameMode<BattleStageGameMode>();
        return gameMode.BattleFieldSystem.GetInstanceOf(airShip);
    }

    protected override void OnTrigger()
    {
        timePassed = 0f;
    }

    protected override void OnCancel()
    {
        canceled = true;
    }
}