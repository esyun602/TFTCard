using System.Collections.Generic;

public class RepairMachineRepairAction : UnitSkillCardActionBase
{
	private float timePassed = 0f;
	public RepairMachineRepairAction(RepairMachineRepairActionSpec spec) : base(spec)
	{
	}

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
					HealAmount = BattleStat.GetValueByValueType(SkillValueType.Heal)
				});
		}
		else if (timePassed > 1.5f)
		{
			routineDone = true;
		}
	}

	protected override void OnTrigger()
	{
		BattleStat.Owner.AnimationController.RunAttackMotion();
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		
	}
}