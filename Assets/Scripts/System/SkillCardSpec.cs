using System;
using System.Collections.Generic;
using UnityEngine;


public enum UseType
{
	Targeting,
	Global,
}

public abstract class SkillCardSpec : ICardSpec
{
	public int Id { get; private set; }
	public string Name { get; protected set; }
	public string NameKey{ get; protected set; }
	public string DescKey{ get; protected set; }
	public string StatSpecName{ get; protected set; }
	public Sprite CardResource{ get; protected set; }
	public string ActionSpecName{ get; protected set; }
	public UseType CardUseType{ get; protected set; }
	
	protected void Parse(Dictionary<string, object> param)
	{
		Id = param.GetInt(nameof(Id));
		Name = param.GetString(nameof(Name));
		NameKey = param.GetString(nameof(NameKey));
		DescKey = param.GetString(nameof(DescKey));
		
		StatSpecName = param.GetString(nameof(StatSpecName));
		//todo: fix
		CardResource = Resources.Load<Sprite>("Sprites/" + param.GetString(nameof(CardResource)));
		ActionSpecName = param.GetString(nameof(ActionSpecName));
		CardUseType = Enum.TryParse(param.GetString(nameof(CardUseType)), out UseType result) ? result : UseType.Global;
	}
}