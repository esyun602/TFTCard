using System.Collections.Generic;

public class DawsonActionPlusSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new DawsonActionPlus(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
    }
}