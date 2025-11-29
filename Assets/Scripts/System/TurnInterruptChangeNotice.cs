using MessageSystem;

public class TurnInterruptChangeNotice : Notice
{
	public TurnInterruptChangeNotice(int count)
	{
		Count = count;
	}

	public int Count { get; }
}