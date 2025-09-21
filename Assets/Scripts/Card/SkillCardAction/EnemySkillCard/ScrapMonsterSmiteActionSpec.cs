using System.Collections.Generic;

public class ScrapMonsterSmiteActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new ScrapMonsterSmiteAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
	}
}