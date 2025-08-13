
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StageSpec
{
	public string StageName { get; private set; }
	public MapData MapData { get; private set; }
	public float CamSize { get; private set; }
	public abstract IStage InstantiateStage();

	protected StageSpec()
	{
	}
	
	public static StageSpec Create(Dictionary<string, object> param)
	{
		var className = param.GetString("ClassName") + "Spec";
		var type = Type.GetType(className);
		var spec = (StageSpec)Activator.CreateInstance(type ?? throw new InvalidOperationException());
		
		spec.StageName = param.GetString(nameof(StageName));
		
		var mapName = param.GetString(nameof(MapData));
		spec.MapData = Resources.Load<MapData>("Map/" + mapName);
		
		spec.CamSize = param.GetFloat(nameof(CamSize));
		spec.Initialize(param);

		return spec;
	}
	protected abstract void Initialize(Dictionary<string, object> param);
}