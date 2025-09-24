using MessageSystem;

public class UnitBattleValueChangeNotice : Notice
{
	public UnitBattleValueChangeNotice(UnitValueType type, int prevValue, int changedValue, UnitCardBattleStat stat)
	{
		Type = type;
		PrevValue = prevValue;
		ChangedValue = changedValue;
		Stat = stat;
	}

	public UnitCardBattleStat Stat { get; private set; }
	public int PrevValue { get; private set; }
	public int ChangedValue { get; private set; }
	public UnitValueType Type { get; private set; }
	public int Diff => ChangedValue - PrevValue;
}