using System.Collections.Generic;

public class BrassMonsterSmashAction : UnitSkillCardActionBase
{
	private float timePassed = 0f;

	public BrassMonsterSmashAction(BrassMonsterSmashActionSpec spec) : base(spec)
	{
	}

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
		if (timePassed > 0.2f && timePassed - dt < 0.2f)
		{
			var targetTile = GetTarget();
			if (targetTile != null)
			{
				var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
				var target = map.GetBattleObjectOfTile(targetTile);

				if (target?.ObjectType.IsHostile(BattleStat.Owner.ObjectType) == true)
				{
					target.Damage(new DamageInfo()
					{
						DamageType = DamageType.NormalAttack,
						Sender = BattleStat.Owner,
						Dmg = BattleStat.GetValueByValueType(UnitValueType.Attack) + BattleStat.GetValueByValueType(UnitValueType.Hp)
					});
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
		BattleStat.Owner.AnimationController.RunAttackMotion();
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
	}
}