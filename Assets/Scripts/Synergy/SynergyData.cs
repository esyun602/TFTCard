using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

//todo: flag?
public enum SynergyCategory
{
	SteamEngine,
	Alchemist,
	Aeronaut,
	Noble,
	Secret,
	Clock,
	Assault,
	Guard,
	Strategy,
	Gunslinger,
	Bomber,
	Mechanic,
	Supporter,
	
}

public class SynergyData : GameData
{
	private List<SynergySpec> synergySpecList;
	private Dictionary<SynergyCategory, SynergySpec> synergyMap;

	public override void Initialize()
	{
		synergySpecList = new();
		var data = GameDataSystem.Instance.GameDataParams["SynergyData"];
		
		synergyMap = new();
		foreach (var spec in data)
		{
			var synergySpec = SynergySpec.Create(spec);
			synergyMap[synergySpec.SynergyCategory] = synergySpec;
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