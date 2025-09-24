public enum BuffType
{
	DefiniteNegative = -1,
	Negative = 0,
	Positive = 1,
}

/// <summary>
/// 버프 하나는 value 하나를 control 하는 걸로 정의
/// </summary>
public interface IBuff
{
	public BuffType BuffType { get; }
	public UnitValueType ControlUnitValueType { get; }
	public int Level { get; }
	public void AddTo(IBattleObject target);
	public void RemoveFromObject();
	public bool TryStack(IBuff buff);
	//todo: 버프 데이터 빼기
	public string Keyword { get; }
}