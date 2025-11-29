using System.Collections.Generic;

public class BrassMonsterSmashActionSpec : SkillCardActionSpec
{
	public bool IsVertical { get; set; }
	public override SkillCardActionBase CreateCardAction()
	{
		return new BrassMonsterSmashAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		IsVertical = param.GetBool(nameof(IsVertical));
	}
}