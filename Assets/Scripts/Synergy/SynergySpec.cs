using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

public enum SynergyTier
{
	Disabled,
	Bronze,
	Silver,
	Gold,
}

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
	public List<SynergyTier> SynergyTier { get; private set; }

	public abstract bool TryGenerateGlobalSynergyInstance(out IGlobalSynergy globalSynergy);
	public abstract bool TryGenerateBattleSynergyInstance(out IBattleSynergy battleSynergy);

	public static SynergySpec Create(Dictionary<string, object> param)
	{
		var overrideClassName = param.GetString("OverrideClassName");
		var spec = (SynergySpec)Activator.CreateInstance(Type.GetType(string.IsNullOrEmpty(overrideClassName) ? param.GetString("SynergyCategory")  + "SynergySpec" : overrideClassName + "Spec") ?? throw new InvalidOperationException());

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

		var tierStr = param.GetStringArray(nameof(SynergyTier));
		spec.SynergyTier = new();
		foreach (var str in tierStr)
		{
			spec.SynergyTier.Add(Enum.Parse<SynergyTier>(str, true)); 
		}

		spec.Initialize(param);
		
		return spec;
	}

	protected virtual void Initialize(Dictionary<string, object> param)
	{
		
	}

	public int GetGrade(int currentCount)
	{
		for (var i = SynergyCountList.Length - 1; i >= 0; i--)
		{
			if (currentCount >= SynergyCountList[i])
			{
				return i + 1;
			}
		}

		return 0;
	}
}

public static class SynergySpecExtensions
{
	public static Sprite GetBagTierResource(this SynergySpec spec, int count)
	{
		return Constant.BagSynergyTierFrame[spec.GetCurrentSynergyTier(count)];
	}
		
	public static Sprite GetBattleTierResource(this SynergySpec spec, int count)
	{
		return Constant.BattleSynergyTierFrame[spec.GetCurrentSynergyTier(count)];
	}

	public static SynergyTier GetCurrentSynergyTier(this SynergySpec spec, int count)
	{
		if (count >= spec.SynergyCountList[^1])
		{
			return spec.SynergyTier[^1];
		}
		
		int left = 0;
		int right = spec.SynergyCountList.Length;
		while (left < right)
		{
			int mid = (left + right) / 2;
			if (spec.SynergyCountList[mid] <= count)
				left = mid + 1;
			else
				right = mid;
		}
		
		if (left == 0)
		{
			return SynergyTier.Disabled;
		}
		else
		{
			return spec.SynergyTier[left - 1];
		}
	}
}