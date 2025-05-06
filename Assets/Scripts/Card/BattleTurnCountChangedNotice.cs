using MessageSystem;

public class BattleTurnCountChangedNotice : Notice
{
	public BattleTurnCountChangedNotice(int prevValue, int changedValue, BattleStat stat)
	{
		PrevValue = prevValue;
		ChangedValue = changedValue;
		Stat = stat;
	}

	public BattleStat Stat { get; private set; }
	public int PrevValue { get; private set; }
	public int ChangedValue { get; private set; }
}