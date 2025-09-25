using System.Collections.Generic;
using System.Linq;

public class AirshipAction : UnitSkillCardActionBase
{
    private float timePassed;
    private IBattleObject target;
    public override IEnumerable<ITile> Targets => Enumerable.Empty<ITile>();

    public AirshipAction(AirshipActionSpec spec) : base(spec)
    {
    }

    protected override void OnUpdate(float dt, out bool routineDone)
    {
        routineDone = true;
    }

    protected override void OnTrigger()
    {
    }
	
    protected override void OnCancel()
    {
    }
}