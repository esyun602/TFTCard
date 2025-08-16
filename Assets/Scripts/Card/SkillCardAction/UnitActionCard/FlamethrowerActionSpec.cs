using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	public override SkillCardActionBase CreateCardAction()
	{
		return new FlamethrowerAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}