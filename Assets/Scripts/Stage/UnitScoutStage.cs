using MessageSystem;
using UnityEngine;
//todo; 임시구현
public class UnitScoutStage : IStage
{
    public GameObject StageGameObject { get; private set; }
    public IMap Map { get; }
    public void Load()
    {
        StageGameObject = new GameObject("ScoutStage");
        Game.Instance.UIManager.GenerateUI<StartDraft>(new StartDraftGenState()
        {
            DraftCount = 1,
            CardPerDraft = 3
        });
    }

    public void Start()
    {
    }

    public void End()
    {
    }

    public void UnLoad()
    {
        NoticeSystem.Instance.Publish(new StageClearNotice());
    }
}