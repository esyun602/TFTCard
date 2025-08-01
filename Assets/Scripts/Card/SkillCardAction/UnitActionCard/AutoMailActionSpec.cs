using UnityEngine;

[CreateAssetMenu]
public class AutoMailActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new AutoMailAction(this);
	}
}