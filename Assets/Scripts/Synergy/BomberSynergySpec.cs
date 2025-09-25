public class BomberSynergySpec : SynergySpec
{
    public override bool TryGenerateGlobalSynergyInstance(out IGlobalSynergy globalSynergy)
    {
        globalSynergy = null;
        return false;
    }

    public override bool TryGenerateBattleSynergyInstance(out IBattleSynergy battleSynergy)
    {
        battleSynergy = new BomberSynergy(this);
        return true;
    }
}