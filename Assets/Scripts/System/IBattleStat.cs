using System;
using System.Collections.Generic;

public interface IBattleObjectStat : IStat, IDisposable
{
	public void Revive();
	public List<SynergyCategory> SynergyList { get; }
	public void AddOption(IOption option);
	public bool RemoveOption(IOption option);
	public void RemoveAllOption();
	public IOption GetOption<T>() where T : IOption;
	public void AddBuff(IBuff buff, object requester = null);
	public bool RemoveBuff<T>(object requester = null) where T : IBuff;
	public void RemoveAllBuff();
	public IBuff GetBuff<T>(object requester = null) where T : IBuff;
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
}