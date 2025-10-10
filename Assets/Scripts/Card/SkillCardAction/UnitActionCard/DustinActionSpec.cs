using System.Collections.Generic;

public class DustinActionSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new DustinAction(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
    }
}