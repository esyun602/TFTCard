using System;
using System.Collections.Generic;

public class AutoMailAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public override bool CanUse(ITile targetTile)
	{
		return base.CanUse(targetTile) && targetTile.TileType == ObjectType.Ally;
	}
	public override IEnumerable<ITile> Targets => ActionUtils.GetTargetTileWithTargetingInfo(triggerInfo);

	public AutoMailAction(AutoMailActionSpec spec) : base(spec)
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
		if (timePassed > 0.2f && timePassed - dt < 0.2f)
		{
			target.UnitCardBattleStat.AddValueByValueType(UnitValueType.Shield, BattleStat.GetValueByValueType(UnitValueType.Attack));
			target.Damage(new DamageInfo()
			{
				DamageType = DamageType.Pierce,
				Dmg = BattleStat.GetValueByValueType(SkillValueType.Damage),
				Sender = BattleStat.Owner
			});
		}
		else if(timePassed > 1.5f)
		{
			routineDone = true;
		}
	}

	protected override void OnTrigger()
	{
		timePassed = 0f;
		target = ActionUtils.GetTargetObjectWithTargetingInfo(triggerInfo);
	}
	
	protected override void OnCancel()
	{
		canceled = true;
	}
}