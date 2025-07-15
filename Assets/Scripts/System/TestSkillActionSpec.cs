using UnityEngine;

[CreateAssetMenu]
public class TestSkillActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	
	public override SkillCardActionBase CreateCardAction()
	{
		return new TestSkillAction(this);
	}
}