using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

public interface IGlobalSynergy : ISynergy
{
	public void Initialize();
	public void AddMember(UnitCard target);
	public void RemoveMember(UnitCard target);
	public void Dispose();
}

public static class IGlobalSynergyExtensions
{
	public static void AddMembers(this IGlobalSynergy synergy, List<UnitCard> obj)
	{
		foreach (var bo in obj)
		{
			synergy.AddMember(bo);
		}
	}

	public static void RemoveMembers(this IGlobalSynergy synergy, List<UnitCard> obj)
	{
		foreach (var bo in obj)
		{
			synergy.RemoveMember(bo);
		}
	} 
}