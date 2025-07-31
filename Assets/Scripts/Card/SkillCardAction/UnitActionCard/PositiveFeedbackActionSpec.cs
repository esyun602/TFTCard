using UnityEngine;

[CreateAssetMenu]
public class PositiveFeedbackActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new PositiveFeedbackAction(this);
	}
}