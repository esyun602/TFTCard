using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[Serializable]
public class Constant : GameData
{
	public AnimationCurve CardReturnAnimationCurve;
	public AnimationCurve CardFollowingSpeedCurve;
	public AnimationCurve HandCardVerticalOffsetCurve;
	[SerializeField]
	private List<AnimationCurve> mapEdgeCurveList;

	public List<List<AnimationCurve>> MapEdgeCurveList = new();
	public  const float MapEdgeCurveModifier = 150f;
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
	
	public const float AttackYPos = 50f;
	public const float HighlightYPos = 60f;
	public const float SelectYPos = 70f;

	public const float StageCameraHeight = 200f;

	public static readonly Vector3 HandColliderSize = new Vector3(0.7f, 1f, 0.01f);
	public static readonly Vector3 HandHoverColliderSize = new Vector3(1f, 1f, 0.01f);

	public const int PlayerHandMax = 10;
	public static string AllyCardDefaultFrameName { get; private set; }
	public static string EnemyCardDefaultFrameName { get; private set; }
	public static string BossCardDefaultFrameName { get; private set; }
	public static Dictionary<SynergyTier, Sprite> BagSynergyTierFrame { get; private set; }
	public static Dictionary<SynergyTier, Sprite> BattleSynergyTierFrame { get; private set; }
	
	public override void Initialize()
	{
		var param = GameDataSystem.Instance.GameDataParams["ConstantData"][0];

		AllyCardDefaultFrameName = param.GetString(nameof(AllyCardDefaultFrameName));
		EnemyCardDefaultFrameName = param.GetString(nameof(EnemyCardDefaultFrameName));
		BossCardDefaultFrameName = param.GetString(nameof(BossCardDefaultFrameName));
		DefaultEnergy = param.GetInt(nameof(DefaultEnergy));
		DefaultMinEnergy = param.GetInt(nameof(DefaultMinEnergy));
		DefaultMaxEnergy = param.GetInt(nameof(DefaultMaxEnergy));

		BagSynergyTierFrame = new();
		BagSynergyTierFrame[SynergyTier.Bronze] = Resources.Load<Sprite>("Sprites/BagSynergyFrame/" + param.GetString("BronzeSynergyFrame"));
		BagSynergyTierFrame[SynergyTier.Silver] = Resources.Load<Sprite>("Sprites/BagSynergyFrame/" + param.GetString("SilverSynergyFrame"));
		BagSynergyTierFrame[SynergyTier.Gold] = Resources.Load<Sprite>("Sprites/BagSynergyFrame/" + param.GetString("GoldSynergyFrame"));
		BagSynergyTierFrame[SynergyTier.Disabled] = Resources.Load<Sprite>("Sprites/BagSynergyFrame/" + param.GetString("DisabledSynergyFrame"));

		BattleSynergyTierFrame = new();
		BattleSynergyTierFrame[SynergyTier.Bronze] = Resources.Load<Sprite>("Sprites/BattleSynergyFrame/" + param.GetString("BronzeSynergyFrame"));
		BattleSynergyTierFrame[SynergyTier.Silver] = Resources.Load<Sprite>("Sprites/BattleSynergyFrame/" + param.GetString("SilverSynergyFrame"));
		BattleSynergyTierFrame[SynergyTier.Gold] = Resources.Load<Sprite>("Sprites/BattleSynergyFrame/" + param.GetString("GoldSynergyFrame"));
		BattleSynergyTierFrame[SynergyTier.Disabled] = Resources.Load<Sprite>("Sprites/BattleSynergyFrame/" + param.GetString("DisabledSynergyFrame"));

		for (var i = 0; i < mapEdgeCurveList.Count; i++)
		{
			if (i % 2 == 0)
			{
				MapEdgeCurveList.Add(new());
				MapEdgeCurveList[^1].Add(mapEdgeCurveList[i]);
			}
			else
			{
				MapEdgeCurveList[^1].Add(mapEdgeCurveList[i]);
			}
		}
	}

	public static string GetFullSynergyName(string name, SynergyTier tier)
	{
		switch (tier)
		{
			case SynergyTier.Gold:
				return "<color=#fffdd7>" + name + "</color>";
			case SynergyTier.Silver:
				return "<color=#d7ffff>" + name + "</color>";
			case SynergyTier.Bronze:
				return "<color=#ffdcd7>" + name + "</color>";
			case SynergyTier.Disabled:
				return "<color=#d7ffff>" + name + "</color>";
			default:
				return name;
		}
	}

	public static string GetFullSynergyTotalCount(string name, SynergyTier tier)
	{
		switch (tier)
		{
			case SynergyTier.Gold:
				return "<color=#87754d>" + name + "</color>";
			case SynergyTier.Silver:
				return "<color=#588286>" + name + "</color>";
			case SynergyTier.Bronze:
				return "<color=#866458>" + name + "</color>";
			case SynergyTier.Disabled:
				return "<color=#588486>" + name + "</color>";
			default:
				return name;
		}
	}
	
	public static string GetFullSynergyCount(string name, SynergyTier tier)
	{
		switch (tier)
		{
			case SynergyTier.Gold:
				return "<color=#ffe786>" + name + "</color>";
			case SynergyTier.Silver:
				return "<color=#adf1ff>" + name + "</color>";
			case SynergyTier.Bronze:
				return "<color=#f9c0b1>" + name + "</color>";
			case SynergyTier.Disabled:
				return "<color=#719c9c>" + name + "</color>";
			default:
				return name;
		}
	}

	public override void Dispose()
	{
	}
}