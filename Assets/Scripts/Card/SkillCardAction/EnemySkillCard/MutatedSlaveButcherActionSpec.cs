using System.Collections.Generic;

public class MutatedSlaveButcherActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new MutatedSlaveButcherAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
	}
}