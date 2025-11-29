using System.Collections.Generic;

public class BrassMonsterPrepareActionSpec : SkillCardActionSpec
{
	public string HorizontalCardName { get; private set; }
	public string VerticalCardName { get; private set; }
	public override SkillCardActionBase CreateCardAction()
	{
		return new BrassMonsterPrepareAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		HorizontalCardName = "BrassMonsterSmash";
		VerticalCardName = "BrassMonsterSmash:Vertical";
	}
}