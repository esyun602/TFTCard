using System;
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

/// <summary>
/// todo: 배틀 중간중간 시너지 변동되는 것 반영하도록 수정 필요
/// </summary>
public class SynergySystem
{
	private Dictionary<SynergyCategory, HashSet<IBattleObject>> synergyBattleObjectMap;
	private Dictionary<SynergyCategory, IBattleSynergy> synergyDict;

	public void Initialize()
	{
		synergyBattleObjectMap = new();
		synergyDict = new();
		
		NoticeSystem.Instance.Subscribe<StatSynergyAddNotice>(OnSynergyAdd);
		NoticeSystem.Instance.Subscribe<StatSynergyRemoveNotice>(OnSynergyRemove);
		
	}

	public T GetSynergyInstance<T>(SynergyCategory synergyCategory) where T : IBattleSynergy
	{
		if (synergyDict.TryGetValue(synergyCategory, out var instance) && instance is T ret)
		{
			return ret;
		}

		return default;
	}
	
	private void OnSynergyAdd(StatSynergyAddNotice m)
	{
		foreach (var synergy in m.AddedSynergyList)
		{
			AddSynergyToObject(synergy, m.Target);
		}
	}
	
	private void OnSynergyRemove(StatSynergyRemoveNotice m)
	{
		foreach (var synergy in m.RemovedSynergyList)
		{
			RemoveSynergyFromObject(synergy, m.Target);
		}
	}


	public void Register(IBattleObject targetObject)
	{
		if (targetObject.ObjectType != ObjectType.Ally)
		{
#if UNITY_EDITOR
			throw new Exception();
#endif
			return;
		}

		foreach (var synergy in targetObject.UnitCardBattleStat.SynergyList)
		{
			AddSynergyToObject(synergy, targetObject);
		}
	}
	
	public void UnRegister(IBattleObject targetObject)
	{
		foreach (var synergy in targetObject.UnitCardBattleStat.SynergyList)
		{
			RemoveSynergyFromObject(synergy, targetObject);
		}
	}

	private void AddSynergyToObject(SynergyCategory category, IBattleObject target)
	{
		if (!synergyBattleObjectMap.TryGetValue(category, out var objList))
		{
			objList = new HashSet<IBattleObject>() { target };
			synergyBattleObjectMap[category] = objList;
		}
		else
		{
			//이미 있다면 그냥 리턴
			if (!objList.Add(target)) return;
		}

		if (!synergyDict.TryGetValue(category, out var battleSynergy))
		{
			if (GameDataSystem.Instance.GetGameData<SynergyData>()
			    .GetSynergySpec(category).TryGenerateBattleSynergyInstance(out battleSynergy))
			{
				synergyDict[category] = battleSynergy;
			}
		}

		battleSynergy.AddMember(target);

		NoticeSystem.Instance.Publish(new SynergyInfoUpdateNotice(category, objList.Count));
	}

	private void RemoveSynergyFromObject(SynergyCategory category, IBattleObject target)
	{
		if (!synergyBattleObjectMap.TryGetValue(category, out var objList))
		{
#if UNITY_EDITOR
			throw new Exception();
#endif
		}
		else
		{
			objList.Remove(target);
		}
			
		if (!synergyDict.TryGetValue(category, out var battleSynergy))
		{
#if UNITY_EDITOR
			throw new Exception();
#endif
		}
		
		battleSynergy.RemoveMember(target);

		NoticeSystem.Instance.Publish(new SynergyInfoUpdateNotice(category, objList.Count));
	}
	
	public void ActivateSynergies()
	{
		foreach (var kvp in synergyDict)
		{
			kvp.Value.Activate();
		}	
	}

	public void DeactivateSynergies()
	{
		foreach (var kvp in synergyDict)
		{
			kvp.Value.Deactivate();
		}
	}

	public void Dispose()
	{
		DeactivateSynergies();
		NoticeSystem.Instance.Unsubscribe<StatSynergyAddNotice>(OnSynergyAdd);
		NoticeSystem.Instance.Unsubscribe<StatSynergyRemoveNotice>(OnSynergyRemove);
	}

}