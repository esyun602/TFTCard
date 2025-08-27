using System;
using System.Collections.Generic;
using UnityEngine;

public class TacticsCardSpec : SkillCardSpec
{
	public static TacticsCardSpec Create(Dictionary<string, object> param)
	{
		var spec = new TacticsCardSpec();
		spec.Parse(param);
		
		return spec;
	}
}