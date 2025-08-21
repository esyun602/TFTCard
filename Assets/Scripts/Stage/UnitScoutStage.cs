using MessageSystem;
using UnityEngine;
//todo; 임시구현
public class UnitScoutStage : IStage
{
    public StageType StageType => StageType.EventStage;
    public GameObject StageGameObject { get; private set; }
    public void Load()
    {
        StageGameObject = new GameObject("ScoutStage");
        Game.Instance.UIManager.GenerateUI<StartDraft>(new StartDraftGenState()
        {
            DraftCount = 1,
            CardPerDraft = 3,
            DoneAction = StageDone
        });
    }

    public void Start()
    {
    }

    public void End()
    {
    }

    public void StageDone()
    {
        NoticeSystem.Instance.Publish(new StageClearNotice());
        Game.Instance.ChangeGameMode(new MapGameMode());
    }
    
    public void UnLoad()
    {
    }
}