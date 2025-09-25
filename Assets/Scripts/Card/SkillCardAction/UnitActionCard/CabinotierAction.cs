using System.Collections.Generic;
using System.Linq;

public class CabinotierAction : UnitSkillCardActionBase
{
    private float timePassed;
    private bool canceled;

    public CabinotierAction(CabinotierActionSpec spec) : base(spec)
    {
    }
    public override IEnumerable<ITile> Targets => Enumerable.Empty<ITile>();

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
            var deckSystem = Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem;
            foreach (var handCard in deckSystem.PlayerHand.CardList)
            {
                //todo: 영구적? 일시적?
                handCard.Stat.AddValueByValueType(SkillValueType.Cost, BattleStat.GetValueByValueType(UnitValueType.Attack), 0);
            }
        }
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