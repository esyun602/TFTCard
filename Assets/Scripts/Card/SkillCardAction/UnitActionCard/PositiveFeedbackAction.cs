using System;
using System.Collections.Generic;

public class PositiveFeedbackAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public override bool CanUse(ITile targetTile)
	{
		return base.CanUse(targetTile) && targetTile.TileType == ObjectType.Enemy;
	}
	public override IEnumerable<ITile> Targets => ActionUtils.GetTargetTileWithTargetingInfo(triggerInfo);

	public PositiveFeedbackAction(PositiveFeedbackActionSpec spec) : base(spec)
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
		if (timePassed > 0f && timePassed - dt <= 0f)
		{
			target.UnitCardBattleStat.AddBuff(new CatalystBuff(BattleStat.GetValueByValueType(SkillValueType.CatalystAdd)));
			BattleStat.AddValueByValueType(SkillValueType.CatalystAdd, BattleStat.GetValueByValueType(UnitValueType.Attack));
			
		}
		else if (timePassed > 0.5f)
		{
			target.Damage(new DamageInfo()
			{
				Sender = BattleStat.Owner,
				Dmg = 0
				
			});
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