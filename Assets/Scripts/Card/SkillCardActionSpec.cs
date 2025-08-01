using UnityEngine;

public abstract class SkillCardActionSpec : ScriptableObject
{
	public abstract SkillCardActionBase CreateCardAction();
}