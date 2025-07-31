using System;

public class HighPressureBombAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public override bool CanUse(ITile targetTile)
	{
		return base.CanUse(targetTile) && targetTile.TileType == ObjectType.Enemy;
	}

	public HighPressureBombAction(HighPressureBombActionSpec spec)
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
			var dmg = stat.Owner.UnitCardBattleStat.GetValueByValueType(BattleValueType.Attack);
			target.Damage(new DamageInfo()
			{
				Sender = stat.Owner,
				Dmg = dmg
			});

			var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.BattleMap;

			var (row, col) = map.GetTileCoord(map.GetTileOfBattleObject(target));

			for (var i = 4; i <= 7; i++)
			{
				if(i == col) continue;
				var obj = map.GetBattleObjectOfTile(map.GetTileAt(row, i));
				if (obj != null)
				{
					obj.Damage(new DamageInfo()
					{
						Sender = stat.Owner,
						Dmg = dmg / 2
					});
				}
			}
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