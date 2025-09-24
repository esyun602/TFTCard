using System.Collections.Generic;
using UnityEngine;

public class SteamEngineProtectionActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new SteamEngineProtectionAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
	}
}