using MessageSystem;

public class BuffRemoveNotice : Notice
{
	public BuffRemoveNotice(IBattleObject target, IBuff buff)
	{
		Target = target;
		Buff = buff;
	}

	public IBattleObject Target { get; }
	public IBuff Buff { get; }
}