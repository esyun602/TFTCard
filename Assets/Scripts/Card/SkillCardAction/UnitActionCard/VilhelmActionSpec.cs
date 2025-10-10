using System.Collections.Generic;

public class VilhelmActionSpec : SkillCardActionSpec
{
    public override SkillCardActionBase CreateCardAction()
    {
        return new VilhelmAction(this);
    }

    protected override void OnInitialize(Dictionary<string, object> param)
    {
		
    }
}