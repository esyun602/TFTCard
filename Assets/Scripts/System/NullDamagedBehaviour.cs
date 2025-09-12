public class NullDamagedBehaviour : IDamagedBehaviour
{
	public static readonly NullDamagedBehaviour Instance = new NullDamagedBehaviour();

	private NullDamagedBehaviour()
	{
		
	}

	public void AttachTo(IBattleObject obj)
	{
	}

	public void DetachFrom(IBattleObject obj)
	{
	}

	public void Damage(DamageInfo damageInfo)
	{
	}

	public void Heal(HealInfo healInfo)
	{
	}

	public void Die(IBattleObject sender)
	{
	}
}