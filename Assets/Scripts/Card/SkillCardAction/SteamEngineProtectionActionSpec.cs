using System.Collections.Generic;
using UnityEngine;

public class SteamEngineProtectionActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new SteamEngineProtectionAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
	}
}