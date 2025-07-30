//todo: negative를 별도로 둘건지 결정 필요
public class ValueAddAttackBuff : IBuff
{
	private IBattleObject target;
	public BuffType BuffType => Level > 0 ? BuffType.Positive : BuffType.Negative;
	public BattleValueType ControlBattleValueType => BattleValueType.Attack;
	private int attackAddValue;
	public int Level => attackAddValue;
	

	public ValueAddAttackBuff(int attackAddValue)
	{
		this.attackAddValue = attackAddValue;
	}
	
	public void OnAdd(IBattleObject target)
	{
		this.target = target;
	}

	public void OnRemove()
	{
	}

	public bool TryStack(IBuff buff)
	{
		if (buff is ValueAddAttackBuff)
		{
			attackAddValue += buff.Level;
			if (attackAddValue == 0)
			{
				target.UnitCardBattleStat.RemoveBuff(this);
			}
			return true;
		}

		return false;
	}
}