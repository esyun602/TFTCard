using UnityEngine;

[CreateAssetMenu]
public class PanaceaActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new PanaceaAction(this);
	}
}