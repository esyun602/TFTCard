using System;
using System.Collections.Generic;

public class AlchemistSynergy : IBattleSynergy
{
	private HashSet<IBattleObject> memberList;
	private Action<IBattleObject> onMemberAdd;
	private Action<IBattleObject> onMemberRemove;
	//public SynergySpec Spec { get; }

	public AlchemistSynergy(SynergySpec spec)
	{
		//Spec = spec;
		memberList = new();
	}

	public int Level { get; set; }

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
		obj.UnitCardBattleStat.AddOption(new AlchemistOption(Level));
	}

	private void RemoveOptionFromObject(IBattleObject obj)
	{
		obj.UnitCardBattleStat.RemoveOption<AlchemistOption>();
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