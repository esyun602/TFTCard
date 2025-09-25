using System.Collections.Generic;

public class NormanActionSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new NormanAction(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
    }
}