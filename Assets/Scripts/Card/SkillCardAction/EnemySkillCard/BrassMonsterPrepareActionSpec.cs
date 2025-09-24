using System.Collections.Generic;

public class BrassMonsterPrepareActionSpec : SkillCardActionSpec
{
	public string TargetCardName { get; private set; }
	public override SkillCardActionBase CreateCardAction()
	{
		return new BrassMonsterPrepareAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		TargetCardName = "BrassMonsterSmash";
	}
}