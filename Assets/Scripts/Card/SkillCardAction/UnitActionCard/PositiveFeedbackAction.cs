using System;

public class PositiveFeedbackAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public override bool CanUse(ITile targetTile)
	{
		return base.CanUse(targetTile) && targetTile.TileType == ObjectType.Enemy;
	}

	public override object[] DescParams => new object[] { StatFallback.GetValueByValueType(CommonValueType.CatalystAdd) };

	public PositiveFeedbackAction(PositiveFeedbackActionSpec spec)
	{
	}

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
			target.UnitCardBattleStat.AddBuff(new CatalystBuff(BattleStat.GetValueByValueType(CommonValueType.CatalystAdd)));
			BattleStat.AddValueByValueType(CommonValueType.CatalystAdd, 1);
			
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