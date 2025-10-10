using MessageSystem;

public class BuffLevelChangeNotice : Notice
{
	public BuffLevelChangeNotice(IBuff buff, IBattleObject target, int diff)
	{
		Buff = buff;
		Target = target;
		Diff = diff;
	}

	public IBattleObject Target { get; }
	public IBuff Buff { get; }
	public int Diff { get; }
	
}