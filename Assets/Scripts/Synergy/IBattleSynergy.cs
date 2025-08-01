using System.Collections.Generic;

public interface IBattleSynergy : ISynergy
{
	public void Activate();
	public void Deactivate();
	public void AddMember(IBattleObject obj);
	public void RemoveMember(IBattleObject obj);
}

public static class IBattleSynergyExtensions
{
	public static void AddMembers(this IBattleSynergy synergy, List<IBattleObject> obj)
	{
		foreach (var bo in obj)
		{
			synergy.AddMember(bo);
		}
	}

	public static void RemoveMembers(this IBattleSynergy synergy, List<IBattleObject> obj)
	{
		foreach (var bo in obj)
		{
			synergy.RemoveMember(bo);
		}
	} 
}