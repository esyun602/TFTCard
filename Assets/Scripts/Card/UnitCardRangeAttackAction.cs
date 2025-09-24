using System.Collections.Generic;
using UnityEngine;

public class UnitCardRangeAttackAction : UnitCardActionBase
{
	private float timePassed = 0f;
	private float actionDuration;
	private GameObject fxPrefab;
	//private GridSelector gridInfo;

	//public override GridSelector AttackRangeInfo => gridInfo;

	public override object[] DescParams => new object[] { owner.TargetUnitCard.Name, owner.UnitCardBattleStat.GetValueByValueType(UnitValueType.Attack) };

	public override IEnumerable<ITile> Targets
	{
		get
		{
			yield return GetTarget();
		}
	}

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		routineDone = false;

		timePassed += dt;
		if (timePassed > 0.15f && timePassed - dt < 0.15f)
		{
			var targetTile = GetTarget();
			if (targetTile != null)
			{
				var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
				var target = map.GetBattleObjectOfTile(targetTile);
				if (fxPrefab != null)
				{
					Object.Instantiate(fxPrefab, targetTile.GetPosition() + Vector3.up, Quaternion.identity);
				}
				
				if (target?.ObjectType.IsHostile(owner.ObjectType) == true)
				{
					map.GetBattleObjectOfTile(targetTile).Damage(
						new DamageInfo()
						{
							Sender = owner,
							DamageType = DamageType.NormalAttack,
							Dmg = owner.UnitCardBattleStat.GetValueByValueType(UnitValueType.Attack)
						});
				}
			}
		}
		else if (timePassed > 1.5f)
		{
			routineDone = true;
		}
	}

	private ITile GetTarget()
	{
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;

		return map.GetRangeAttackTargetTile(owner);
	}

	protected override void OnTrigger()
	{
		owner.RunAttackMotion();
		
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		throw new System.NotImplementedException();
	}

	public UnitCardRangeAttackAction(UnitCardRangeAttackActionSpec actionSpec) : base(actionSpec)
	{
		actionDuration = actionSpec.actionDuration;
		fxPrefab = actionSpec.fxPrefab;
		//gridInfo = actionData.actionRange;
	}
}