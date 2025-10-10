using System.Collections.Generic;

public class NullBattleObjectStat : IBattleObjectStat
{
	public static readonly NullBattleObjectStat Instance = new NullBattleObjectStat();

	private NullBattleObjectStat()
	{
		
	}
	public int[] GetValuesByValueType(ValueType type)
	{
		return new int[] { 0 };
	}

	public void SetValuesByValueType(ValueType type, int[] newValues)
	{
	}

	public void Dispose()
	{
	}

	public void Revive()
	{
	}

	public List<SynergyCategory> SynergyList { get; } = new List<SynergyCategory>();

	public void AddOption(IOption option)
	{
	}

	public bool RemoveOption(IOption option)
	{
		return false;
	}

	public void RemoveAllOption()
	{
	}

	public IOption GetOption<T>() where T : IOption
	{
		return null;
	}

	public void AddBuff(IBuff buff, object requester = null)
	{
		
	}

	public bool RemoveBuff<T>(object requester = null) where T : IBuff
	{
		return false;
	}


	public void RemoveAllBuff()
	{
	}

	public IBuff GetBuff<T>(object requester = null) where T : IBuff
	{
		return null;
	}

	public void Purify()
	{
	}
}