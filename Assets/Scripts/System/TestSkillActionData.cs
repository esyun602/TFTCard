using UnityEngine;

[CreateAssetMenu]
public class TestSkillActionData : SkillCardActionData
{
	public float actionDuration;
	public GameObject fxPrefab;
	
	public override SkillCardActionBase CreateCardAction()
	{
		return new TestSkillAction(this);
	}
}