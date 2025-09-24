using System.Collections.Generic;

public class PowerRepeaterAllocateActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new PowerRepeaterAllocateAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
	}
}