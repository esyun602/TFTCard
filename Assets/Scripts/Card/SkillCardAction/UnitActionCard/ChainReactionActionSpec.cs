using UnityEngine;

[CreateAssetMenu]
public class ChainReactionActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new ChainReactionAction(this);
	}
}