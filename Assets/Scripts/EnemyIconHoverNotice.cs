using MessageSystem;

public class EnemyIconHoverNotice : Notice
{
    public EnemyIconHoverNotice(EnemySkillCardObject target)
    {
        Target = target;
    }

    public EnemySkillCardObject Target { get; }
}