public struct DamageInfo
{
	public IBattleObject Sender { get; set; }
	public int Dmg { get; set; }
}

public struct HealInfo
{
	public IBattleObject Sender { get; set; }
	public int HealAmount { get; set; }
	
}

public interface IDamagedBehaviour
{
	public void AttachTo(IBattleObject obj);
	public void DetachFrom(IBattleObject obj);
	public void Damage(DamageInfo damageInfo);
	public void Heal(HealInfo healInfo);
	public void Die(IBattleObject sender);
}