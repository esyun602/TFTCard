using System;
using System.Collections.Generic;

public enum IconCategory
{
	HpRelevant = 0,
	AttackRelevant = 1,
	Special = 2,
}
public class KeywordInfo
{
	public string Name { get; private set; }
	public ValueType ValueType { get; private set; }
	public string IconResource { get; private set; }
	public int Importance { get; private set; }
	public string NameKey { get; private set; }
	public string DescKey { get; private set; }
	public IconCategory IconCategory { get; private set; }
	public string PoolName { get; private set; }
	public string ReducePoolName { get; private set; }
	public string SfxName { get; private set; }
	public string ReduceSfxName { get; private set; }
	
	private KeywordInfo()
	{
		
	}
	
	public static KeywordInfo Create(Dictionary<string, object> param)
	{
		var info = new KeywordInfo();
		info.Name = param.GetString(nameof(Name));
		if (ValueType.TryParse(param.GetString(nameof(ValueType)), out var valueType))
		{
			info.ValueType = valueType;
		}

		info.IconResource = param.GetString(nameof(IconResource));
		info.Importance = param.GetInt(nameof(Importance));
		info.NameKey = param.GetString(nameof(NameKey));
		info.DescKey = param.GetString(nameof(DescKey));
		info.PoolName = param.GetString(nameof(PoolName));
		info.ReducePoolName = param.GetString(nameof(ReducePoolName));
		info.SfxName = param.GetString(nameof(SfxName));
		info.ReduceSfxName = param.GetString(nameof(ReduceSfxName));
		if (Enum.TryParse(param.GetString(nameof(IconCategory)), out IconCategory iconCategory))
		{
			info.IconCategory = iconCategory;
		}
		
		return info;
	}
}