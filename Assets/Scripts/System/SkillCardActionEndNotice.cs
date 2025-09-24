using MessageSystem;

public class SkillCardActionEndNotice : Notice
{
	public SkillCardActionEndNotice(IAction targetAction)
	{
		TargetAction = targetAction;
	}

	public IAction TargetAction { get; }
}