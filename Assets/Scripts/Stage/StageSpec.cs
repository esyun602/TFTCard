
using System;
using System.Collections.Generic;
using UnityEngine;

//배틀 관련된 내용 삭제
public abstract class StageSpec
{
	public string StageName { get; private set; }
	public float CamSize { get; private set; }
	public abstract IStage InstantiateStage();
	public abstract StageType StageType { get; }

	protected StageSpec()
	{
	}
	
	public static StageSpec Create(Dictionary<string, object> param)
	{
		var className = param.GetString("ClassName") + "Spec";
		var type = Type.GetType(className);
		var spec = (StageSpec)Activator.CreateInstance(type ?? throw new InvalidOperationException());
		
		spec.StageName = param.GetString(nameof(StageName));
		
		spec.CamSize = param.GetFloat(nameof(CamSize));
		spec.Initialize(param);

		return spec;
	}
	protected abstract void Initialize(Dictionary<string, object> param);
}