using System;
using System.Buffers;
using System.Collections.Generic;

public class BiologicalExperimentAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public BiologicalExperimentAction(BiologicalExperimentActionSpec spec) : base(spec)
	{
	}



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
		if (timePassed > 0.2f && timePassed - dt < 0.2f)
		{
			var targetTile = GetTarget();
			if (targetTile != null)
			{
				var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
				var target = map.GetBattleObjectOfTile(targetTile);

				if (target?.ObjectType.IsHostile(BattleStat.Owner.ObjectType) == true)
				{
					target.Damage(new DamageInfo()
					{
						Sender = BattleStat.Owner,
						Dmg = BattleStat.GetValueByValueType(UnitValueType.Attack)
					});
					target.UnitCardBattleStat.AddBuff(new CatalystBuff(BattleStat.GetValueByValueType(SkillValueType.CatalystAdd)));
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
		BattleStat.Owner.AnimationController.RunAttackMotion();
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}