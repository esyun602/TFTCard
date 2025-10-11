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

		timePassed += dt;
		routineDone = currentRoutine.Invoke(dt);
	}

	private Func<float, bool> currentRoutine;

	private bool AddBurnPhase(float dt)
	{
		if (timePassed > 0.2f)
		{
			target.UnitCardBattleStat.AddValueByValueType(UnitValueType.Burn, BattleStat.GetValueByValueType(SkillValueType.BurnAdd));
			currentRoutine = AddAttackPhase;
			timePassed = 0f;
		}
		return false;
	}

	private bool AddAttackPhase(float dt)
	{
		if (timePassed > 0.5f)
		{
			target.UnitCardBattleStat.AddValueByValueType(UnitValueType.Attack, BattleStat.GetValueByValueType(UnitValueType.Attack));
			currentRoutine = PostPhase;
			timePassed = 0f;
		}
		return false;
	}

	private bool PostPhase(float dt)
	{
		if (timePassed > 0.8f)
		{
			return true;
		}

		return false;
	}


	protected override void OnTrigger()
	{
		timePassed = 0f;
		target = ActionUtils.GetTargetObjectWithTargetingInfo(triggerInfo);
		currentRoutine = AddBurnPhase;
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}