using UnityEngine;

[CreateAssetMenu]
public class FlamethrowerActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	public override SkillCardActionBase CreateCardAction()
	{
		return new FlamethrowerAction(this);
	}
}