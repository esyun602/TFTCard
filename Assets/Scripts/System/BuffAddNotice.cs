using MessageSystem;

public class BuffAddNotice : Notice
{
	public BuffAddNotice(IBattleObject target, IBuff buff)
	{
		Target = target;
		Buff = buff;
	}

	public IBattleObject Target { get; }
	public IBuff Buff { get; }
}