using System;
using System.Collections.Generic;

public interface IBattleObjectStat : IStat, IDisposable
{
	public List<SynergyCategory> SynergyList { get; }
	public void AddOption(IOption option);
	public bool RemoveOption(IOption option);
	public void RemoveAllOption();
	public IOption GetOption<T>() where T : IOption;
	public void AddBuff(IBuff buff);
	public bool RemoveBuff(IBuff buff);
	public void RemoveAllBuff();
	public IBuff GetBuff<T>() where T : IBuff;
	public void AddSynergy(SynergyCategory synergyCategory);
	public bool RemoveSynergy(SynergyCategory synergyCategory);
	public void Purify();
}

public static class IBattleObjectStatExtensions
{
	public static bool RemoveOption<T>(this IBattleObjectStat stat) where T : IOption
	{
		var option = stat.GetOption<T>();
		if (option != null)
		{
			return stat.RemoveOption(option);
		}

		return false;
	}
	
	public static bool RemoveBuff<T>(this IBattleObjectStat stat) where T : IBuff
	{
		var option = stat.GetBuff<T>();
		if (option != null)
		{
			return stat.RemoveBuff(option);
		}

		return false;
	}
}