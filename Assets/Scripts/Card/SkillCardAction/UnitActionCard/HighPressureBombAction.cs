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

	public override object[] DescParams => new object[]{StatFallback.GetValueByValueType(UnitValueType.Attack), StatFallback.GetValueByValueType(UnitValueType.Attack) / 2};

	public override IEnumerable<ITile> Targets
	{
		get
		{
			var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.BattleMap;
			return map.GetAllTilesInRow(ActionUtils.GetTargetObjectWithTargetingInfo(triggerInfo), ObjectType.Enemy);
		}
	}

	public HighPressureBombAction(HighPressureBombActionSpec spec) : base(spec)
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
			var dmg = BattleStat.GetValueByValueType(UnitValueType.Attack);

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
				Sender = BattleStat.Owner,
				Dmg = dmg
			});
			foreach (var obj in targetList)
			{
				obj.Damage(new DamageInfo()
				{
					Sender = BattleStat.Owner,
					Dmg = dmg / 2
				});
			}
			
			routineDone = true;
		}
	}

	protected override void OnTrigger()
	{
		timePassed = 0f;
		target = ActionUtils.GetTargetObjectWithTargetingInfo(triggerInfo);
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}