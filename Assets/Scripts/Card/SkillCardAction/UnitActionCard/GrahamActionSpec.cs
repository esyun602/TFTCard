using System.Collections.Generic;

public class GrahamActionSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new GrahamAction(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
    }
}