using UnityEngine;

public abstract class SkillCardActionData : ScriptableObject
{
	public abstract SkillCardActionBase CreateCardAction();
}