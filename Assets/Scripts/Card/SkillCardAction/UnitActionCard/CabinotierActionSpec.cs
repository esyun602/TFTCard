using System.Collections.Generic;

public class CabinotierActionSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new CabinotierAction(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
    }
}