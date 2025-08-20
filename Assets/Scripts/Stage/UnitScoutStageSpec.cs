using System.Collections.Generic;

public class UnitScoutStageSpec : StageSpec
{
    public override IStage InstantiateStage()
    {
        return new UnitScoutStage();
    }

    protected override void Initialize(Dictionary<string, object> param)
    {
    }
}