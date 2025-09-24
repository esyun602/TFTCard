using System.Collections.Generic;

public class ScrapMonsterHeavySwingActionSpec : SkillCardActionSpec
{
	public string TargetCardName { get; private set; }
	public override SkillCardActionBase CreateCardAction()
	{
		return new ScrapMonsterHeavySwingAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		TargetCardName = "ScrapMonsterSmite";
	}
}