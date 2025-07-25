using MessageSystem;

public class DamageNotice : ContextNotice
{
	public IBattleObject Sender { get; }
	public IBattleObject Target { get; }
	public int Damage { get; }
	
	public DamageNotice(IBattleObject sender, IBattleObject target, int damage)
	{
		Sender = sender;
		Target = target;
		Damage = damage;
	}
}