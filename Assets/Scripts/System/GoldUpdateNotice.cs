using MessageSystem;

public class GoldUpdateNotice : Notice
{
	public GoldUpdateNotice(int value)
	{
		Value = value;
	}

	public int Value { get; set; }
}