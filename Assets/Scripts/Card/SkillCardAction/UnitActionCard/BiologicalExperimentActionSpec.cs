using System.Collections.Generic;
using UnityEngine;

public class BiologicalExperimentActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new BiologicalExperimentAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}