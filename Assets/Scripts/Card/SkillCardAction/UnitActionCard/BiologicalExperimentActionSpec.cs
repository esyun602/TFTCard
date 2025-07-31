using UnityEngine;

[CreateAssetMenu]
public class BiologicalExperimentActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new BiologicalExperimentAction(this);
	}
}