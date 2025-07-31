using MessageSystem;

public class DamageNotice : ContextNotice
{
	public DamageInfo DamageInfo { get; }
	public IBattleObject Target { get; }
	
	public DamageNotice(DamageInfo info, IBattleObject target)
	{
		DamageInfo = info;
		Target = target;
	}
}