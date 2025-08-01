using UnityEngine;

[CreateAssetMenu]
public class SteamPackActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new SteamPackAction(this);
	}
}