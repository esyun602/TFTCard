using System.Collections.Generic;

public class VacuumTubeLumpAmplifyActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new VacuumTubeLumpAmplifyAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}