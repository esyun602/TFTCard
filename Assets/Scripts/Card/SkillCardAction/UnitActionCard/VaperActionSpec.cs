using System.Collections.Generic;

public class VaperActionSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new VaperAction(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
    }
}