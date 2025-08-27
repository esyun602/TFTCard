using UnityEngine;

public class BagSkillCard : BagUICard
{
    public override ICard TargetCard => TargetSkillCard;

    protected override void InitializeInfo()
    {
        infoHandler.Initialize(TargetSkillCard, TargetSkillCard.Stat);
    }

    public SkillCardBase TargetSkillCard { get; private set; }

    public void Initialize(SkillCardBase targetCard, Vector3 targetPos)
    {
        this.cardPosInfo = new BagCardPosInfo(targetPos, null);
        this.TargetSkillCard = targetCard;

        InitializeRoutine();
    }
}