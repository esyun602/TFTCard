using System.Collections.Generic;
using UnityEngine;

public class SteamPackActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new SteamPackAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}