using System.Collections.Generic;

public class CollectorActionSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new CollectorAction(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
		
    }
}