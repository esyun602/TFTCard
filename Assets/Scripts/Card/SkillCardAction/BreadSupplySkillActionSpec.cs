using UnityEngine;

[CreateAssetMenu]
public class BreadSupplySkillActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	public override SkillCardActionBase CreateCardAction()
	{
		return new BreadSupplySkillAction(this);
	}
}