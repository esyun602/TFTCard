using MessageSystem;

public class EnemyIconRemoveHoverNotice : Notice
{
    public EnemyIconRemoveHoverNotice(EnemySkillCardObject target)
    {
        Target = target;
    }

    public EnemySkillCardObject Target { get; }
}