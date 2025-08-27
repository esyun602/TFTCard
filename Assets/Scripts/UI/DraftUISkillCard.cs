public class DraftUISkillCard : DraftUICard
{
    public override ICard TargetCard => tacticsCard;
    private TacticsCard tacticsCard;
    
    public override void OnInitialize(ICardSpec targetCard)
    {
        tacticsCard = new TacticsCard((TacticsCardSpec)targetCard);
        SetInfo();
    }
    
    private void SetInfo()
    {
        infoHandler.Initialize(TargetCard, tacticsCard.Stat);
    }
}