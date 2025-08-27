using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkillCardSpec : SkillCardSpec
{
	public static UnitSkillCardSpec Create(Dictionary<string, object> param)
	{
		var spec = new UnitSkillCardSpec();
		spec.Parse(param);
		
		return spec;
	}
}