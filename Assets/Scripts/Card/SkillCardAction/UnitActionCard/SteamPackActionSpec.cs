using System.Collections.Generic;
using UnityEngine;

public class SteamPackActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new SteamPackAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}