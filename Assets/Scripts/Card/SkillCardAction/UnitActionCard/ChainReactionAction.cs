using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainReactionAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;

	public ChainReactionAction(ChainReactionActionSpec spec) : base(spec)
	{
	}

	public override IEnumerable<ITile> Targets
	{
		get
		{
			var gameMode = Game.Instance.GetGameMode<BattleStageGameMode>();
			return gameMode.BattleFieldSystem
				.GetAllObjectOfType(ObjectType.Enemy).Where(x =>
					x.UnitCardBattleStat.GetValueByValueType(UnitValueType.Catalyst) > 0).Select(x => gameMode.BattleStage.BattleMap.GetTileOfBattleObject(x));
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
			var enumerator = Game.Instance.GetGameMode<BattleStageGameMode>().BattleFieldSystem
				.GetAllObjectOfType(ObjectType.Enemy).Where(x =>
					x.UnitCardBattleStat.GetValueByValueType(UnitValueType.Catalyst) > 0);

			foreach (var bo in enumerator)
			{
				bo.Damage(new DamageInfo()
				{
					Sender = BattleStat.Owner,
					Dmg = BattleStat.GetValueByValueType(UnitValueType.Attack),
					DamageType = DamageType.Bomb
				});
			}
			
			routineDone = true;
		}
	}

	protected override void OnTrigger()
	{
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}