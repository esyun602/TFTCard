using MessageSystem;

public class SkillCardActionTriggerNotice : Notice
{
	public SkillCardActionTriggerNotice(IAction targetAction)
	{
		TargetAction = targetAction;
	}

	public IAction TargetAction { get; }
}