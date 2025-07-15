using MessageSystem;

public class BattleValueChangeNotice : Notice
{
	public BattleValueChangeNotice(ValueType type, int prevValue, int changedValue, UnitCardBattleStat stat)
	{
		Type = type;
		PrevValue = prevValue;
		ChangedValue = changedValue;
		Stat = stat;
	}

	public UnitCardBattleStat Stat { get; private set; }
	public int PrevValue { get; private set; }
	public int ChangedValue { get; private set; }
	public ValueType Type { get; private set; }
}