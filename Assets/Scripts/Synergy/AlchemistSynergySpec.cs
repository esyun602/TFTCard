using UnityEngine;

[CreateAssetMenu]
public class AlchemistSynergySpec : SynergySpec
{
	public override bool TryGenerateGlobalSynergyInstance(out IGlobalSynergy globalSynergy)
	{
		globalSynergy = null;
		return false;
	}

	public override bool TryGenerateBattleSynergyInstance(out IBattleSynergy battleSynergy)
	{
		battleSynergy = new AlchemistSynergy(this);
		return true;
	}
}