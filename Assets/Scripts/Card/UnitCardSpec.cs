using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitCardSpec : ICardSpec
{
	public int Id { get; private set; }
	public string Name { get; private set; }
	public string NameKey { get; private set; }
	public string DescKey { get; private set; }
	public string StatSpecName { get; private set; }
	public Sprite CardResource { get; private set; }
	public string ActionSpecName { get; private set; }
	public string TargetSkillCardSpecName { get; private set; }

	private UnitCardSpec()
	{
	}
	
	public static UnitCardSpec Create(Dictionary<string, object> param)
	{
		var spec = new UnitCardSpec();
		spec.Id = param.GetInt(nameof(Id));
		spec.Name = param.GetString(nameof(Name));
		spec.NameKey = param.GetString(nameof(NameKey));
		spec.DescKey = param.GetString(nameof(DescKey));
		
		spec.StatSpecName = param.GetString(nameof(StatSpecName));
		//todo: fix
		spec.CardResource = Resources.Load<Sprite>("Sprites/" + param.GetString(nameof(CardResource)));
		spec.ActionSpecName = param.GetString(nameof(ActionSpecName));
		spec.TargetSkillCardSpecName = param.GetString(nameof(TargetSkillCardSpecName));

		return spec;
	}
}