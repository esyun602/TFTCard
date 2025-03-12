using MessageSystem;

public class EnergyChangeNotice : Notice
{
	public int PrevValue { get; }
	public int CurValue { get; }
	
	public EnergyChangeNotice(int prevValue, int curValue)
	{
		PrevValue = prevValue;
		CurValue = curValue;
	}
}