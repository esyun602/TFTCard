using System;
using System.Collections.Generic;
using MessageSystem;

//todo: 옵션, 버프도 데이터 추가하고 BattleSynergyBase로 묶기
public class SteamEngineSynergy : IBattleSynergy
{
	private HashSet<IBattleObject> memberList;
	private Action<IBattleObject> onMemberAdd;
	private Action<IBattleObject> onMemberRemove;
	private SynergySpec spec;

	public SteamEngineSynergy(SynergySpec spec)
	{
		this.spec = spec;
		memberList = new();
	}

	public int Level => spec.GetGrade(memberList.Count);

	public void Activate()
	{
		onMemberAdd = AddOptionToObject;
		onMemberRemove = RemoveOptionFromObject;
		foreach (var member in memberList)
		{
			AddOptionToObject(member);
		}
	}

	public void Deactivate()
	{
		onMemberAdd = null;
		onMemberRemove = null;
	}

	private void AddOptionToObject(IBattleObject obj)
	{
		obj.UnitCardBattleStat.AddOption(new SteamEngineOption(Level));
	}

	private void RemoveOptionFromObject(IBattleObject obj)
	{
		obj.UnitCardBattleStat.RemoveOption<SteamEngineOption>();
	}

	public void AddMember(IBattleObject obj)
	{
		if (memberList.Add(obj))
		{
			onMemberAdd?.Invoke(obj);
		}
#if UNITY_EDITOR
		else
		{
			throw new ArgumentException();
		}
#endif
	}

	public void RemoveMember(IBattleObject obj)
	{
		if (memberList.Remove(obj))
		{
			onMemberRemove?.Invoke(obj);
		}
#if UNITY_EDITOR
		else
		{
			throw new ArgumentException();
		}
#endif
	}
}