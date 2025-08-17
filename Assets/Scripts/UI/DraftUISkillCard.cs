public class DraftUISkillCard : DraftUICard
{
    public override void OnInitialize(ICardSpec targetCard)
    {
        TargetCard = new SkillCard((SkillCardSpec)targetCard);
        SetInfo();
    }
    
    private void SetInfo()
    {
        infoHandler.Initialize(TargetCard, ((SkillCard)TargetCard).Stat);
    }
}