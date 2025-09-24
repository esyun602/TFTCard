using System.Collections.Generic;

public class RepairMachineRepairActionSpec: SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new RepairMachineRepairAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
	}
}