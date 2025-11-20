using System.Collections.Generic;

public class EventStageSpec : StageSpec
{
    public override IStage InstantiateStage()
    {
        return new EventStage(this);
    }

    public override StageType StageType => StageType.EventStage;
    public string EventName { get; private set; }
    protected override void Initialize(Dictionary<string, object> param)
    {
        EventName = param.GetString(nameof(EventName));
    }
}