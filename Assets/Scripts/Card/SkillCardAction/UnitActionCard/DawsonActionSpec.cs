using System.Collections.Generic;

public class DawsonActionSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new DawsonAction(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
    }
}