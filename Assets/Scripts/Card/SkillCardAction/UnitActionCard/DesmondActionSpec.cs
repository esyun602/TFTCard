using System.Collections.Generic;

public class DesmondActionSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new DesmondAction(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
    }
}