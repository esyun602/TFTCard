using UnityEngine;

public class BagSkillCard : BagUICard
{
    public override ICard TargetCard => TargetSkillCard;

    protected override void InitializeInfo()
    {
        infoHandler.Initialize(TargetSkillCard, TargetSkillCard.Stat);
    }

    public SkillCard TargetSkillCard { get; private set; }

    public void Initialize(SkillCard targetCard, Vector3 targetPos)
    {
        this.cardPosInfo = new BagCardPosInfo(targetPos, null);
        this.TargetSkillCard = targetCard;

        InitializeRoutine();
    }
}