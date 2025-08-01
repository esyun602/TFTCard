using System;
using System.Collections.Generic;

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

	//todo: 데미지 약간 지연? or 타일 정렬 약간 지연?
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

			var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.BattleMap;

			var targetList = new List<IBattleObject>();
			var (row, col) = map.GetTileCoord(map.GetTileOfBattleObject(target));

			for (var i = 4; i <= 7; i++)
			{
				if(i == col) continue;
				var obj = map.GetBattleObjectOfTile(map.GetTileAt(row, i));
				if (obj != null)
				{
					targetList.Add(obj);
				}
			}
			
			target.Damage(new DamageInfo()
			{
				Sender = stat.Owner,
				Dmg = dmg
			});
			foreach (var obj in targetList)
			{
				obj.Damage(new DamageInfo()
				{
					Sender = stat.Owner,
					Dmg = dmg / 2
				});
			}
			
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