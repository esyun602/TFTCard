using UnityEngine;

[CreateAssetMenu]
public class PrepareBombingActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new PrepareBombingAction(this);
	}
}