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

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}