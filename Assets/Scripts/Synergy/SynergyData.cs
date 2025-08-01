using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//todo: flag?
public enum SynergyCategory
{
	SteamEngine,
	Alchemist
}

[CreateAssetMenu]
public class SynergyData : GameData
{
	[SerializeField] private List<SynergySpec> synergySpecList;
	private Dictionary<SynergyCategory, SynergySpec> synergyMap;

	public override void Initialize()
	{
		synergyMap = new();
		foreach (var synergySpec in synergySpecList)
		{
			synergyMap[synergySpec.synergyCategory] = synergySpec;
		}
	}

	public SynergySpec GetSynergySpec(SynergyCategory synergyCategory)
	{
		return synergyMap.GetValueOrDefault(synergyCategory);
	}
	
	public override void Dispose()
	{
		synergyMap = null;
	}
}