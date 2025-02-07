using MessageSystem;
using UnityEngine;

public class CardHandPosUpdatedNotice : Notice
{
	public CardHandPosUpdatedNotice(Vector3 targetPos, Quaternion targetRotation, Vector3 hoverTargetPos)
	{
		this.TargetPos = targetPos;
		this.TargetRotation = targetRotation;
		this.HoverTargetPos = hoverTargetPos;
	}

	public Vector3 HoverTargetPos { get; }

	public Vector3 TargetPos { get; }
	public Quaternion TargetRotation { get; }
}