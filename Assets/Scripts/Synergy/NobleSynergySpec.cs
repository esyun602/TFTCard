public class NobleSynergySpec : AddOptionBattleSynergySpec
{
    public override bool TryGenerateGlobalSynergyInstance(out IGlobalSynergy globalSynergy)
    {
        globalSynergy = null;
        return false;
    }

    public override bool TryGenerateBattleSynergyInstance(out IBattleSynergy battleSynergy)
    {
        battleSynergy = new NobleSynergy(this);
        return true;
    }
}