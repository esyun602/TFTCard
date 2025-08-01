using UnityEngine;

[CreateAssetMenu]
public class EquivalentExchangeActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new EquivalentExchangeAction(this);
	}
}