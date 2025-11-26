using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EquivalentExchangeAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;

	public EquivalentExchangeAction(EquivalentExchangeActionSpec spec) : base(spec)
	{
	}
	public override IEnumerable<ITile> Targets => Enumerable.Empty<ITile>();

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
			var totalEnemyList = Game.Instance.GetGameMode<BattleStageGameMode>().BattleFieldSystem.GetAllObjectOfType(ObjectType.Enemy);
			var catalystCountList = new List<int>();
			
			for (var i = 0; i < totalEnemyList.Count; i++)
			{
				catalystCountList.Add(0);
			}

			for (var i = 0; i < BattleStat.GetValueByValueType(UnitValueType.Attack); i++)
			{
				catalystCountList[Random.Range(0, catalystCountList.Count)]++;
			}
			
			for (var i = 0; i < catalystCountList.Count; i++)
			{
				if(catalystCountList[i] == 0) continue;
				totalEnemyList[i].UnitCardBattleStat.AddValueByValueType(UnitValueType.Catalyst, catalystCountList[i]); 
			}
			
			Game.Instance.GetGameMode<BattleStageGameMode>().BattleFieldSystem.GetRandomBattleObject(ObjectType.Ally).UnitCardBattleStat.AddValueByValueType(UnitValueType.Catalyst, BattleStat.GetValueByValueType(UnitValueType.Attack));
			
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