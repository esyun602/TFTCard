using System;

public class PanaceaAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public PanaceaAction(PanaceaActionSpec spec)
	{
	}

	public override object[] DescParams { get; }

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
			target.DamagedBehaviour.Heal(
				new HealInfo()
				{
					Sender = BattleStat.Owner,
					HealAmount =
						target.UnitCardBattleStat.GetValueByValueType(UnitValueType.MaxHp)
				});
			
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