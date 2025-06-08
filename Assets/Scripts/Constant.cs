using UnityEngine;

[CreateAssetMenu]
public class Constant : GameData
{
	public AnimationCurve CardReturnAnimationCurve;
	public AnimationCurve CardFollowingSpeedCurve;
	public AnimationCurve HandCardVerticalOffsetCurve;
	public const float Epsilon = 0.001f;
	public const int DefaultEnergy = 3;

	public const float FieldYPos = 0.55f;
	public const float FieldMoveYPos = 2f;
	public const float FieldSwitchActYPos = 4f;
	public const float FieldHoverYPos = 6f;

	public const float HandCenterZOffset = 1.3f + 0.5f;
	public const float HandCenterYPos = 10f;
	public const float HandIndexYOffset = 1f;
	public const float HandHoverYPos = 30f;
	public const float HandCardDistance = 1.3f + 0.5f;
	public const float HoverZOffset = 1.7f;
	
	public const float SelectYPos = 40f;
	public const float AttackYPos = 50f;

	public readonly Vector3 HandColliderSize = new Vector3(0.7f, 1f, 0.01f);
	public readonly Vector3 HandHoverColliderSize = new Vector3(1f, 1f, 0.01f);

	public override void Initialize()
	{
	}

	public override void Dispose()
	{
	}
}