using System;
using UnityEngine;

[Serializable]
public class Constant : GameData
{
	public AnimationCurve CardReturnAnimationCurve;
	public AnimationCurve CardFollowingSpeedCurve;
	public AnimationCurve HandCardVerticalOffsetCurve;
	public const float Epsilon = 0.001f;
	public static int DefaultEnergy { get; private set; }
	public static int DefaultMinEnergy { get; private set; }
	public static int DefaultMaxEnergy { get; private set; }

	public const float FieldYPos = 0.55f;
	public const float FieldMoveYPos = 2f;
	public const float FieldSwitchActYPos = 4f;
	public const float FieldHoverYPos = 6f;

	public const float HandCenterZOffset = 1.3f;
	public const float HandCenterYPos = 10f;
	public const float HandIndexYOffset = 1f;
	public const float HandHoverYPos = 30f;
	
	public const float SelectYPos = 40f;
	public const float AttackYPos = 50f;
	public const float HighlightYPos = 60f;

	public const float StageCameraHeight = 200f;

	public static readonly Vector3 HandColliderSize = new Vector3(0.7f, 1f, 0.01f);
	public static readonly Vector3 HandHoverColliderSize = new Vector3(1f, 1f, 0.01f);

	public const int PlayerHandMax = 10;
	public static string AllyCardDefaultFrameName { get; private set; }
	public static string EnemyCardDefaultFrameName { get; private set; }
	public static string BossCardDefaultFrameName { get; private set; }
	
	public override void Initialize()
	{
		var param = GameDataSystem.Instance.GameDataParams["ConstantData"][0];

		AllyCardDefaultFrameName = param.GetString(nameof(AllyCardDefaultFrameName));
		EnemyCardDefaultFrameName = param.GetString(nameof(EnemyCardDefaultFrameName));
		BossCardDefaultFrameName = param.GetString(nameof(BossCardDefaultFrameName));
		DefaultEnergy = param.GetInt(nameof(DefaultEnergy));
		DefaultMinEnergy = param.GetInt(nameof(DefaultMinEnergy));
		DefaultMaxEnergy = param.GetInt(nameof(DefaultMaxEnergy));
	}

	public override void Dispose()
	{
	}
}