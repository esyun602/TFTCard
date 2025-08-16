using System;
using System.Collections.Generic;
using UnityEngine;


public enum UseType
{
	Targeting,
	Global,
}

public class SkillCardSpec : ICardSpec
{
	public string Name { get; private set; }
	public string NameKey{ get; private set; }
	public string DescKey{ get; private set; }
	public string StatSpecName{ get; private set; }
	public Sprite CardResource{ get; private set; }
	public string ActionSpecName{ get; private set; }
	public UseType CardUseType{ get; private set; }
	public bool IsUnitAction { get; private set; }

	private SkillCardSpec()
	{
		
	}

	public static SkillCardSpec Create(Dictionary<string, object> param)
	{
		var spec = new SkillCardSpec();
		spec.Name = param.GetString(nameof(Name));
		spec.NameKey = param.GetString(nameof(NameKey));
		spec.DescKey = param.GetString(nameof(DescKey));
		
		spec.StatSpecName = param.GetString(nameof(StatSpecName));
		//todo: fix
		spec.CardResource = Resources.Load<Sprite>("Sprites/" + param.GetString(nameof(CardResource)));
		spec.ActionSpecName = param.GetString(nameof(ActionSpecName));
		spec.CardUseType = Enum.TryParse(param.GetString(nameof(CardUseType)), out UseType result) ? result : UseType.Global;
		spec.IsUnitAction = param.GetBool(nameof(IsUnitAction));
		
		return spec;
	}
}