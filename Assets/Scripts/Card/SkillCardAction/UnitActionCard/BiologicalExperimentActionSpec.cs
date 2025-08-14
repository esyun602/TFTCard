using System.Collections.Generic;
using UnityEngine;

public class BiologicalExperimentActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new BiologicalExperimentAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}