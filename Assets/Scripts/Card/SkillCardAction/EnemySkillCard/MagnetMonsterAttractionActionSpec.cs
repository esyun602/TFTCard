using System.Collections.Generic;

public class MagnetMonsterAttractionActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new MagnetMonsterAttractionAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
	}
}