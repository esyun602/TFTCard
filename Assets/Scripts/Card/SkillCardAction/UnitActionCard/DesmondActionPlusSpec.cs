using System.Collections.Generic;

public class DesmondActionPlusSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new DesmondActionPlus(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
    }
}