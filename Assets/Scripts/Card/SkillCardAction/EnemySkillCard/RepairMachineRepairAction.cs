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

		if (timePassed == 0)
		{
			BattleStat.Owner.AnimationController.RunAttackMotion();
		}
		timePassed += dt;
		if (timePassed > 0.15f && timePassed - dt < 0.15f)
		{
			var targets = Game.Instance.GetGameMode<BattleStageGameMode>().BattleFieldSystem.GetAllObjectOfType(ObjectType.Enemy);
			var notMaxHpList = new List<IBattleObject>();
			foreach (var target in targets)
			{
				if (target.UnitCardBattleStat.GetValueByValueType(UnitValueType.Hp) <
				    target.UnitCardBattleStat.GetValueByValueType(UnitValueType.MaxHp))
				{
					notMaxHpList.Add(target);
				}
			}

			var healTarget = notMaxHpList.GetRandomElement();

			if (healTarget != null)
			{
				healTarget.Heal(
					new HealInfo()
					{
						Sender = BattleStat.Owner,
						HealAmount = BattleStat.GetValueByValueType(SkillValueType.Heal)
					});
			}
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