using MessageSystem;

public class BuffLevelChangeNotice : Notice
{
	public BuffLevelChangeNotice(IBuff buff, IBattleObject target)
	{
		Buff = buff;
		Target = target;
	}

	public IBattleObject Target { get; }
	public IBuff Buff { get; }
	
}