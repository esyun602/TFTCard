using System;
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

public class SynergySystem
{
	private Dictionary<Synergy, List<IBattleObject>> synergyBattleObjectMap;

	public void Initialize()
	{
		synergyBattleObjectMap = new();
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

			NoticeSystem.Instance.Publish(new SynergyInfoUpdateNotice(synergy, objList.Count));
		}
	}

	public void Dispose()
	{
	}
}