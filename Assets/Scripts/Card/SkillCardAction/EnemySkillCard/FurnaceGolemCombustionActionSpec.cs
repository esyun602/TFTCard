using System.Collections.Generic;

public class FurnaceGolemCombustionActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new FurnaceGolemCombustionAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
	}
}