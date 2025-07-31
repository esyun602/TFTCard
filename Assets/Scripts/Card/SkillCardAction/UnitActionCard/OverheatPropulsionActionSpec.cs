using UnityEngine;

[CreateAssetMenu]
public class OverheatPropulsionActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	
	public override SkillCardActionBase CreateCardAction()
	{
		return new OverheatPropulsionAction(this);
	}
}