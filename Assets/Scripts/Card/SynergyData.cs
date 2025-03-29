using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Synergy
{
	Test1,
	Test2
}

[CreateAssetMenu]
public class SynergyData : GameData
{
	[SerializeField] private List<SynergySpec> synergySpecList;
	private Dictionary<Synergy, SynergySpec> synergyMap;

	public override void Initialize()
	{
		synergyMap = new();
		foreach (var synergySpec in synergySpecList)
		{
			synergyMap[synergySpec.synergy] = synergySpec;
		}
	}

	public SynergySpec GetSynergySpec(Synergy synergy)
	{
		return synergyMap.GetValueOrDefault(synergy);
	}
	
	public override void Dispose()
	{
		synergyMap = null;
	}
}