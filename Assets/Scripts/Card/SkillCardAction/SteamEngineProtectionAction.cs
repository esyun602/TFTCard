using System;

public class SteamEngineProtectionAction : SkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public override bool CanUse(ITile targetTile)
	{
		var targetObject = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map
			.GetBattleObjectOfTile(targetTile);
		return base.CanUse(targetTile) && targetObject.ObjectType == ObjectType.Ally;
	}

	public override object[] DescParams => new object[] { StatFallback.GetValueByValueType(BattleValueType.Shield) };

	public SteamEngineProtectionAction(SteamEngineProtectionActionSpec spec)
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
			target.UnitCardBattleStat.AddValueByValueType(BattleValueType.Shield, battleStat.GetValueByValueType(BattleValueType.Shield));
			target.UnitCardBattleStat.AddSynergy(SynergyCategory.SteamEngine);
			
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