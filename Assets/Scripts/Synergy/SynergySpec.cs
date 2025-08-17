using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

//todo: 접근성 수정
public abstract class SynergySpec
{
	public SynergyCategory SynergyCategory { get; private set; } 
	public Sprite TargetSprite { get; private set; }
	public string SynergyNameKey { get; private set; }
	public string CommonDescKey { get; private set; }
	public int[] SynergyCountList { get; private set; }
	public string[] DescKey { get; private set; }
	public Color SymbolColor { get; private set; }

	public abstract bool TryGenerateGlobalSynergyInstance(out IGlobalSynergy globalSynergy);
	public abstract bool TryGenerateBattleSynergyInstance(out IBattleSynergy battleSynergy);

	public static SynergySpec Create(Dictionary<string, object> param)
	{
		var spec = (SynergySpec)Activator.CreateInstance(Type.GetType(param.GetString("SynergyCategory") + "SynergySpec") ?? throw new InvalidOperationException());

		spec.SynergyCategory =
			Enum.TryParse(param.GetString(nameof(SynergyCategory)), out SynergyCategory category)
				? category
				: throw new InvalidOperationException();
		//todo: fix
		spec.TargetSprite = Resources.Load<Sprite>("Sprites/" + param.GetString(nameof(TargetSprite)));
		spec.SynergyNameKey = param.GetString(nameof(SynergyNameKey));
		spec.CommonDescKey = param.GetString(nameof(CommonDescKey));
		spec.SynergyCountList = param.GetIntArray(nameof(SynergyCountList));
		spec.DescKey = param.GetStringArray(nameof(DescKey));
		if (ColorUtility.TryParseHtmlString(param.GetString(nameof(SymbolColor)), out var color))
		{
			spec.SymbolColor = color;
		}

		return spec;
	}
}