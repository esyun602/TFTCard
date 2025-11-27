using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainReactionAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;


	public ChainReactionAction(ChainReactionActionSpec spec) : base(spec)
	{
	}

	public override IEnumerable<ITile> Targets
	{
		get
		{
			target = ActionUtils.GetTargetObjectWithTargetingInfo(triggerInfo);
			var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
			return GetTargets().Select(x => map.GetTileOfBattleObject(x));
		}
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
			var enumerator = GetTargets();

			foreach (var bo in enumerator)
			{
				bo.Damage(new DamageInfo()
				{
					Sender = BattleStat.Owner,
					Dmg = BattleStat.GetValueByValueType(UnitValueType.Attack) + BattleStat.GetValueByValueType(SkillValueType.BombDamage),
					DamageType = DamageType.Bomb
				});
			}
			
			routineDone = true;
		}
	}

	private IEnumerable<IBattleObject> GetTargets()
	{
		var q = new Queue<IBattleObject>();
		var targets = new List<IBattleObject>();
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
		targets.Add(target);

		if (target.UnitCardBattleStat.GetValueByValueType(UnitValueType.Catalyst) != 0)
		{
			q.Enqueue(target);
		}

		while (q.Count != 0)
		{
			var toCheck = q.Dequeue();
			var tile = map.GetTileOfBattleObject(toCheck);
			var (row, _) = map.GetTileCoord(tile);

			var nextCandidate = new List<IBattleObject>();
			if (row != 0)
			{
				var dTile = map.GetDownwardTile(tile);
				nextCandidate.Add(map.GetBattleObjectOfTile(dTile));
			}

			if (row != 2)
			{
				var uTile = map.GetUpwardTile(tile);
				nextCandidate.Add(map.GetBattleObjectOfTile(uTile));
			}

			if (map.GetOrderInRow(tile) != 1)
			{
				var fTile = map.GetForwardTile(tile);
				nextCandidate.Add(map.GetBattleObjectOfTile(fTile));
			}

			if (map.GetOrderInRow(tile) != 4)
			{
				var bTile = map.GetBackwardTile(tile);
				nextCandidate.Add(map.GetBattleObjectOfTile(bTile));
			}

			foreach (var candidate in nextCandidate)
			{
				if(candidate == null) continue;
				
				if (candidate.UnitCardBattleStat.GetValueByValueType(UnitValueType.Catalyst) != 0 
				    && !targets.Contains(candidate))
				{
					targets.Add(candidate);
					q.Enqueue(candidate);
				}
			}
		}

		return targets;
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