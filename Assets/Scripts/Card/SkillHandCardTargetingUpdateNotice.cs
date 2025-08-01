using MessageSystem;
using UnityEngine;

public class SkillHandCardTargetingUpdateNotice : Notice
{
	public SkillHandCardTargetingUpdateNotice(Vector2 position)
	{
		Position = position;
	}

	public Vector2 Position { get; private set; }
}