public class SecretSynergySpec : SynergySpec
{
    public override bool TryGenerateGlobalSynergyInstance(out IGlobalSynergy globalSynergy)
    {
        globalSynergy = new SecretSynergy(this);
        return true;
    }

    public override bool TryGenerateBattleSynergyInstance(out IBattleSynergy battleSynergy)
    {
        battleSynergy = null;
        return false;
    }
}