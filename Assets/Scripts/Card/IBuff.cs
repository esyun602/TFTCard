public enum BuffType
{
	Negative = 0,
	Positive = 1,
	
}

/// <summary>
/// 버프 하나는 value 하나를 control 하는 걸로 정의
/// </summary>
public interface IBuff
{
	public BuffType BuffType { get; }
	public BattleValueType ControlBattleValueType { get; }
	public int Level { get; }
	public void OnAdd(IBattleObject target);
	public void OnRemove();
	public bool TryStack(IBuff buff);
}