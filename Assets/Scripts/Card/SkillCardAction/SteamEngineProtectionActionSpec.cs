using UnityEngine;

[CreateAssetMenu]
public class SteamEngineProtectionActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new SteamEngineProtectionAction(this);
	}
}