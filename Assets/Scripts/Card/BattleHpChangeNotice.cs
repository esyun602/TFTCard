
using MessageSystem;

public class BattleHpChangeNotice : Notice
{
	public BattleHpChangeNotice(int prevValue, int changedValue, UnitCardBattleStat stat)
	{
		PrevValue = prevValue;
		ChangedValue = changedValue;
		Stat = stat;
	}

	public UnitCardBattleStat Stat { get; private set; }
	public int PrevValue { get; private set; }
	public int ChangedValue { get; private set; }
	
}