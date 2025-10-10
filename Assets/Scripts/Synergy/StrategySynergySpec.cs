public class StrategySynergySpec : SynergySpec
{
    public override bool TryGenerateGlobalSynergyInstance(out IGlobalSynergy globalSynergy)
    {
        globalSynergy = null;
        return false;
    }

    public override bool TryGenerateBattleSynergyInstance(out IBattleSynergy battleSynergy)
    {
        battleSynergy = new StrategySynergy(this);
        return true;
    }
}