using MessageSystem;
using UnityEngine;
//todo; 임시구현
public class UnitScoutStage : StageBase
{
    protected override void OnLoad()
    {
        Game.Instance.UIManager.GenerateUI<StartDraft>(new StartDraftGenState()
        {
            DraftCount = 1,
            CardPerDraft = 3,
            DoneAction = StageDone
        });
    }
    
    private void StageDone()
    {
        NoticeSystem.Instance.Publish(new StageClearNotice());
        Game.Instance.ChangeGameMode(new FlowGameMode());
    }

    public UnitScoutStage(StageSpec stageSpec) : base(stageSpec)
    {
    }
}