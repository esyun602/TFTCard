using System;

public class AutoMailAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public override bool CanUse(ITile targetTile)
	{
		return base.CanUse(targetTile) && targetTile.TileType == ObjectType.Ally;
	}

	public override object[] DescParams => new object[] { StatFallback.GetValueByValueType(UnitValueType.Attack) };

	public AutoMailAction(AutoMailActionSpec spec)
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
			target.UnitCardBattleStat.AddValueByValueType(UnitValueType.Shield, BattleStat.GetValueByValueType(UnitValueType.Attack));
			target.UnitCardBattleStat.AddBuff(new ValueAddAttackBuff(1));
			
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