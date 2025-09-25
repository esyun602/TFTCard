using System;
using System.Collections.Generic;

public class SteamPackAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public SteamPackAction(SteamPackActionSpec spec) : base(spec)
	{
	}

	public override bool CanUse(ITile targetTile)
	{
		return base.CanUse(targetTile) && targetTile.TileType == BattleStat.Owner.ObjectType;
	}
	public override IEnumerable<ITile> Targets => ActionUtils.GetTargetTileWithTargetingInfo(triggerInfo);

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
			target.UnitCardBattleStat.AddValueByValueType(UnitValueType.Burn, BattleStat.GetValueByValueType(SkillValueType.BurnAdd));
			target.UnitCardBattleStat.AddValueByValueType(UnitValueType.Attack, BattleStat.GetValueByValueType(UnitValueType.Attack));
			
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