using System;
using MessageSystem;
using UnityEngine;
//todo; 임시구현
public class EventStage : StageBase
{
    private EventStageSpec spec;
    protected override void OnLoad()
    {
        Game.Instance.UIManager.GenerateUI<EventPanel>(new EventPanelGenState(spec.EventName, StageDone));
        BgmManager.Instance.ChangeBgm("story");
    }
    
    private void StageDone()
    {
        NoticeSystem.Instance.Publish(new StageClearNotice());
        Game.Instance.ChangeGameMode(new FlowGameMode());
    }

    public EventStage(StageSpec stageSpec) : base(stageSpec)
    {
        if (stageSpec is not EventStageSpec espec) throw new ArgumentException();
        spec = espec;
    }
}