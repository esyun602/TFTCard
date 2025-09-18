using System.Collections.Generic;

public class PowerRepeaterAllocateAction : UnitSkillCardActionBase
{
	private float timePassed = 0f;

	public PowerRepeaterAllocateAction(PowerRepeaterAllocateActionSpec spec) : base(spec)
	{
	}

	public override object[] DescParams => new object[]
		{ (BattleStat?.Owner.Name ?? Stat.Owner.Name), StatFallback.GetValuesByValueType(UnitValueType.Attack) };

	public override IEnumerable<ITile> Targets => new ITile[] { };

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		routineDone = false;

		timePassed += dt;
		if (timePassed > 0.15f && timePassed - dt < 0.15f)
		{
			var bos = Game.Instance.GetGameMode<BattleStageGameMode>().BattleFieldSystem
				.GetAllObjectOfType(BattleStat.Owner.ObjectType);
			foreach (var bo in bos)
			{
				bo.UnitCardBattleStat.AddValueByValueType(UnitValueType.Attack, BattleStat.GetValueByValueType(UnitValueType.Attack));
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