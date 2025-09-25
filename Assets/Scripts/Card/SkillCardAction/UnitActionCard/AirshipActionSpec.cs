using System.Collections.Generic;

public class AirshipActionSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new AirshipAction(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
        
    }
}