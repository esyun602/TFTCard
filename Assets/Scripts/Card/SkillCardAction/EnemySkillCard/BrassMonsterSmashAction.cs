using System.Collections.Generic;
using System.Linq;

public class BrassMonsterSmashAction : UnitSkillCardActionBase
{
	private float timePassed = 0f;
	private BrassMonsterSmashActionSpec smashSpec;

	public BrassMonsterSmashAction(BrassMonsterSmashActionSpec spec) : base(spec)
	{
		smashSpec = spec;
	}

	public override IEnumerable<ITile> Targets => GetTargets();

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		routineDone = false;

		if (timePassed == 0)
		{
			BattleStat.Owner.AnimationController.RunAttackMotion();
		}
		timePassed += dt;
		if (timePassed > 0.2f && timePassed - dt < 0.2f)
		{
			var targetTiles = GetTargets();
			if (targetTiles != null)
			{
				var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;

				List<IBattleObject> targets = targetTiles.Select(x => map.GetBattleObjectOfTile(x)).Where(x => x != null).ToList();

				foreach (var target in targets)
				{
					if (target?.ObjectType.IsHostile(BattleStat.Owner.ObjectType) == true)
					{
						target.Damage(new DamageInfo()
						{
							DamageType = DamageType.NormalAttack,
							Sender = BattleStat.Owner,
							Dmg = BattleStat.GetValueByValueType(UnitValueType.Attack)
						});
					}
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

	private List<ITile> GetTargets()
	{
		var targetTile = GetTarget();
		if (targetTile == null) return null;
		
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;

		List<ITile> targets;
		if (smashSpec.IsVertical)
		{
			targets = map.GetAllTilesInCol(targetTile).ToList();
		}
		else
		{
			targets = map.GetAllTilesInRow(targetTile, ObjectType.Ally).ToList();
		}

		return targets;
	}

	protected override void OnTrigger()
	{
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
	}
}