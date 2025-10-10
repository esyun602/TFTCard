using System.Collections.Generic;

public class DerrickActionSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new DerrickAction(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
    }
}