using System.Collections.Generic;
using UnityEngine;

public abstract class SkillCardActionSpec
{
	public abstract SkillCardActionBase CreateCardAction();

	public void Initialize(Dictionary<string, object> param)
	{
		DescKey = param.GetString(nameof(DescKey));

		OnInitialize(param);
	}
	protected abstract void OnInitialize(Dictionary<string, object> param);
	public string DescKey { get; private set; }
}