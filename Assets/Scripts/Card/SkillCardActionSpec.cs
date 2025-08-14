using System.Collections.Generic;
using UnityEngine;

public abstract class SkillCardActionSpec
{
	public abstract SkillCardActionBase CreateCardAction();
	public abstract void Initialize(Dictionary<string, object> param);
}