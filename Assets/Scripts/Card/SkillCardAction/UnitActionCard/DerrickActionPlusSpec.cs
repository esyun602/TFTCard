using System.Collections.Generic;

public class DerrickActionPlusSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new DerrickActionPlus(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
    }
}