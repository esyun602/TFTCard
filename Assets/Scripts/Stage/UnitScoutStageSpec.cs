using System.Collections.Generic;

public class UnitScoutStageSpec : StageSpec
{
    public override IStage InstantiateStage()
    {
        return new UnitScoutStage(this);
    }

    public override StageType StageType => StageType.EventStage;

    protected override void Initialize(Dictionary<string, object> param)
    {
    }
}