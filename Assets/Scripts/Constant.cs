using UnityEngine;

[CreateAssetMenu]
public class Constant : GameData
{
	public AnimationCurve CardReturnAnimationCurve;
	public AnimationCurve CardFollowingSpeedCurve;
	public AnimationCurve HandCardVerticalOffsetCurve;
	public const float Epsilon = 0.001f;
	
	public override void Initialize()
	{
	}

	public override void Dispose()
	{
	}
}