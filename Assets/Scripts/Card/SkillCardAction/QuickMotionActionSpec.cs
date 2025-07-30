using UnityEngine;

//todo: 스크립트 생성 자동화
[CreateAssetMenu]
public class QuickMotionActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	
	public override SkillCardActionBase CreateCardAction()
	{
		return new QuickMotionAction(this);
	}
}