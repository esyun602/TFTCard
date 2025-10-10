public class BurnImmuneBuff : BuffBase
{
    public BurnImmuneBuff()
    {
    }

    public override BuffType DefaultType => BuffType.Positive;
    public override UnitValueType ControlUnitValueType => UnitValueType.Burn;

    protected override void OnAdd()
    {
    }

    protected override void OnRemove()
    {
    }

    protected override bool TryStackImpl(IBuff buff)
    {
        var canStack = buff is BurnImmuneBuff;

        return canStack;
    }

    public override string Keyword => "BurnImmune";
}