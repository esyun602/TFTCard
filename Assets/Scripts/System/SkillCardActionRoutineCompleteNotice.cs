using MessageSystem;

public class SkillCardActionRoutineCompleteNotice : Notice
{
	public SkillCardActionRoutineCompleteNotice(IAction targetAction)
	{
		TargetAction = targetAction;
	}

	public IAction TargetAction { get; }
}