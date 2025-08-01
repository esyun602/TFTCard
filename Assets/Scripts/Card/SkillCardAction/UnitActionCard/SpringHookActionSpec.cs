using UnityEngine;

[CreateAssetMenu]
public class SpringHookActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new SpringHookAction(this);
	}
}