using System.Collections.Generic;
using System.Linq;

public class CollectorAction : UnitSkillCardActionBase
{
    private float timePassed;
    private bool canceled;

    public CollectorAction(CollectorActionSpec spec) : base(spec)
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
            var drawCount = BattleStat.GetValueByValueType(UnitValueType.Attack);
            var deckSystem = Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem;
            for (var i = 0; i < drawCount; i++)
            {
                var card = deckSystem.DropCardList.GetRandomElement();
                deckSystem.DrawPlayerCard(card);
            }
			
            routineDone = true;
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