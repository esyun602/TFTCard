using System.Collections.Generic;

public class FurnaceGolemCombustionAction : UnitSkillCardActionBase
{
	private float timePassed = 0f;

	public FurnaceGolemCombustionAction(FurnaceGolemCombustionActionSpec spec) : base(spec)
	{
	}

	public override object[] DescParams => new object[]
		{ (BattleStat?.Owner.Name ?? Stat.Owner.Name), StatFallback.GetValuesByValueType(UnitValueType.Attack) };

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
		if (timePassed > 0.15f && timePassed - dt < 0.15f)
		{
			var targetTile = GetTarget();
			if (targetTile != null)
			{
				var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
				var target = map.GetBattleObjectOfTile(targetTile);
				
				if (target?.ObjectType.IsHostile(BattleStat.Owner.ObjectType) == true)
				{
					map.GetBattleObjectOfTile(targetTile).UnitCardBattleStat.AddValueByValueType(UnitValueType.Burn, BattleStat.GetValueByValueType(UnitValueType.Attack));
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
	}

	protected override void OnCancel()
	{
	}
}