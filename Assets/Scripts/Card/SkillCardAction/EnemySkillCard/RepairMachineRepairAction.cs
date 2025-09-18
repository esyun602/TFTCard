using System.Collections.Generic;

public class RepairMachineRepairAction : UnitSkillCardActionBase
{
	private float timePassed = 0f;
	public RepairMachineRepairAction(RepairMachineRepairActionSpec spec) : base(spec)
	{
	}

	public override object[] DescParams => new object[] { (BattleStat?.Owner.Name ?? Stat.Owner.Name) , StatFallback.GetValuesByValueType(CommonValueType.Heal) };
	public override IEnumerable<ITile> Targets => new ITile[] { };
	protected override void OnUpdate(float dt, out bool routineDone)
	{
		routineDone = false;

		timePassed += dt;
		if (timePassed > 0.15f && timePassed - dt < 0.15f)
		{
			var target = Game.Instance.GetGameMode<BattleStageGameMode>().BattleFieldSystem.GetRandomBattleObject(ObjectType.Enemy);
			
			target.Heal(
				new HealInfo()
				{
					Sender = BattleStat.Owner,
					HealAmount = BattleStat.GetValueByValueType(CommonValueType.Heal)
				});
		}
		else if (timePassed > 1.5f)
		{
			routineDone = true;
		}
	}

	protected override void OnTrigger()
	{
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		
	}
}