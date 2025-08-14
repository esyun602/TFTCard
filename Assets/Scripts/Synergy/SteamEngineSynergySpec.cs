using UnityEngine;

public class SteamEngineSynergySpec : SynergySpec
{
	public override bool TryGenerateGlobalSynergyInstance(out IGlobalSynergy globalSynergy)
	{
		globalSynergy = null;
		return false;
	}

	public override bool TryGenerateBattleSynergyInstance(out IBattleSynergy battleSynergy)
	{
		battleSynergy = new SteamEngineSynergy(this);
		return true;
	}
}