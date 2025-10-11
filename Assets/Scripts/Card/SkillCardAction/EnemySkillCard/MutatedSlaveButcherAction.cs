using System.Collections.Generic;
using UnityEngine;

public class MutatedSlaveButcherAction : UnitSkillCardActionBase
{
	private float timePassed = 0f;
	private float actionDuration;
	private GameObject fxPrefab;
	//private GridSelector gridInfo;

	//public override GridSelector AttackRangeInfo => gridInfo;

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

		if (timePassed == 0)
		{
			BattleStat.Owner.AnimationController.RunAttackMotion();
		}
		timePassed += dt;
		if (timePassed > 0.1f && timePassed - dt < 0.1f || 
		    timePassed > 0.2f && timePassed - dt < 0.2f ||
		    timePassed > 0.3f && timePassed - dt < 0.3f)
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
				
				if (target?.ObjectType.IsHostile(BattleStat.Owner.ObjectType) == true)
				{
					map.GetBattleObjectOfTile(targetTile).Damage(
						new DamageInfo()
						{
							Sender = BattleStat.Owner,
							DamageType = DamageType.NormalAttack,
							Dmg = BattleStat.Owner.UnitCardBattleStat.GetValueByValueType(UnitValueType.Attack)
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

		return map.GetAttackTargetTile(BattleStat.Owner);
	}

	protected override void OnTrigger()
	{
		
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		throw new System.NotImplementedException();
	}

	public MutatedSlaveButcherAction(MutatedSlaveButcherActionSpec actionSpec) : base(actionSpec)
	{
	}
}