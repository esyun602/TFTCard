using UnityEngine;

[CreateAssetMenu]
public class TestCardActionData : CardActionData
{
	public float actionDuration;
	public GameObject fxPrefab;
	public override IAction CreateCardAction()
	{
		return new TestCardAction(this);
	}
	//todo: 추후 툴 개발
}