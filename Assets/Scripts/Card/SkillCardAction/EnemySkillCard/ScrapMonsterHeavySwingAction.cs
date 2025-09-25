using System.Collections.Generic;
using System.Linq;

public class ScrapMonsterHeavySwingAction : UnitSkillCardActionBase
{
	private float timePassed = 0f;
	private UnitSkillCardSpec targetCardSpec;

	public ScrapMonsterHeavySwingAction(ScrapMonsterHeavySwingActionSpec spec) : base(spec)
	{
		targetCardSpec = GameDataSystem.Instance.GetGameData<CardData>().GetUnitSkillCardSpecByName(spec.TargetCardName);
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
		if (timePassed > 0.15f && timePassed - dt < 0.15f)
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
			
			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.GenerateUnitSkillCardInstance(
				BattleStat.Owner, new UnitSkillCard(targetCardSpec, Stat.Owner), true);
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
		BattleStat.Owner.RunAttackMotion();
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
	}
}