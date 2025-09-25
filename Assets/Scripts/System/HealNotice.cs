using MessageSystem;

public class HealNotice : Notice
{
    public HealNotice(HealInfo healInfo, IBattleObject target)
    {
        HealInfo = healInfo;
        Target = target;
    }

    public IBattleObject Target { get; private set; }
    public HealInfo HealInfo { get; private set; }
}