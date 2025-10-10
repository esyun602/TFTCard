using System;

[Flags]
public enum BuffType
{
	Definite = 1,
	Negative =  0<<1,
	Positive = 1<<1,
	BlockOptionAdd = 1<<2
}

public static class BuffTypeExtensions
{
	public static bool IsAll(this BuffType type, BuffType type2)
	{
		return (type & type2) == type2;
	}
	
	public static bool IsAny(this BuffType type, BuffType type2)
	{
		return (type & type2) != 0;
	}
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