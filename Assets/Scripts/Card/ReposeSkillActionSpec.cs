using UnityEngine;

[CreateAssetMenu]
public class ReposeSkillActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	public override SkillCardActionBase CreateCardAction()
	{
		return new ReposeSkillAction(this);
	}
}