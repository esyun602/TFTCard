using System;
using MessageSystem;
using UnityEngine;
//todo; 임시구현
public class EventStage : StageBase
{
    private EventStageSpec spec;
    protected override void OnLoad()
    {
        Action doneAction;
        var targetEvent = GameDataSystem.Instance.GetGameData<EventData>().GetEvent(spec.EventName);
        
        switch (targetEvent.GameEventType)
        {
            case GameEventType.Story:
                doneAction = StageDone;
                break;
            case GameEventType.Ending:
                doneAction = Game.Instance.ResetProgressInfo;
                break;
            default:
                doneAction = StageDone;
                break;
        }
        
        Game.Instance.UIManager.GenerateUI<EventPanel>(new EventPanelGenState(targetEvent, doneAction));
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