using System;

public class BiologicalExperimentAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public BiologicalExperimentAction(BiologicalExperimentActionSpec spec)
	{
	}

	public override object[] DescParams  => new object[] { StatFallback.GetValueByValueType(BattleValueType.Attack), StatFallback.GetValueByValueType(BattleValueType.Catalyst) };

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		if (canceled)
		{
			routineDone = true;
			return;
		}

		routineDone = false;

		timePassed += dt;
		if (timePassed > 0f)
		{
			//todo: owner가 있으면 그냥 스탯에 owner 스탯을 합쳐버리는 방향으로 수정
			target.Damage(new DamageInfo()
			{
				Sender = battleStat.Owner,
				Dmg = battleStat.GetValueByValueType(BattleValueType.Attack)
			});
			target.UnitCardBattleStat.AddBuff(new CatalystBuff(battleStat.GetValueByValueType(BattleValueType.Catalyst)));
			
			routineDone = true;
		}
	}

	protected override void OnTrigger(object triggerInfo)
	{
		timePassed = 0f;
		if (triggerInfo is not TargetingActionTriggerInfo ti)
		{
			throw new ArgumentException();
		}

		target = ti.Target;
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}