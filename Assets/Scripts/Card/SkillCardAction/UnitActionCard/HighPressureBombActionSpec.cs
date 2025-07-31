using UnityEngine;

[CreateAssetMenu]
public class HighPressureBombActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new HighPressureBombAction(this);
	}
}