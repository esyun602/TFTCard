using System;
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

/// <summary>
/// todo: 배틀 중간중간 시너지 변동되는 것 반영하도록 수정 필요
/// </summary>
public class SynergySystem
{
	private Dictionary<SynergyCategory, List<IBattleObject>> synergyBattleObjectMap;
	private Dictionary<SynergyCategory, IBattleSynergy> synergyDict;

	public void Initialize()
	{
		synergyBattleObjectMap = new();
		synergyDict = new();
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

		foreach (var synergy in targetObject.UnitCardBattleStat.GetSynergyList())
		{
			if (!synergyBattleObjectMap.TryGetValue(synergy, out var objList))
			{
				objList = new List<IBattleObject>() { targetObject };
				synergyBattleObjectMap[synergy] = objList;
			}
			else
			{
				objList.Add(targetObject);
			}

			if (!synergyDict.TryGetValue(synergy, out var battleSynergy))
			{
				if (GameDataSystem.Instance.GetGameData<SynergyData>()
				    .GetSynergySpec(synergy).TryGenerateBattleSynergyInstance(out battleSynergy))
				{
					synergyDict[synergy] = battleSynergy;
				}
			}
			else
			{
				battleSynergy.Level++;
			}

			NoticeSystem.Instance.Publish(new SynergyInfoUpdateNotice(synergy, objList.Count));
		}
	}

	public void UnRegister(IBattleObject targetObject)
	{
		foreach (var synergy in targetObject.UnitCardBattleStat.GetSynergyList())
		{
			if (!synergyBattleObjectMap.TryGetValue(synergy, out var objList))
			{
#if UNITY_EDITOR
				throw new Exception();
#endif
			}
			else
			{
				objList.Remove(targetObject);
			}
			
			if (!synergyDict.TryGetValue(synergy, out var battleSynergy))
			{
#if UNITY_EDITOR
				throw new Exception();
#endif
			}
			else
			{
				battleSynergy.Level--;
			}

			NoticeSystem.Instance.Publish(new SynergyInfoUpdateNotice(synergy, objList.Count));
		}
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
	}

}