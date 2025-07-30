using UnityEngine;

[CreateAssetMenu]
public class FireArrowSkillActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	public override SkillCardActionBase CreateCardAction()
	{
		return new FireArrowSkillAction(this);
	}
}