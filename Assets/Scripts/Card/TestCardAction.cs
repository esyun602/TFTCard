using System.Collections;
using Coroutine;
using UnityEngine;

public class TestCardAction : CardActionBase
{
	private float timePassed = 0f;
	private float actionDuration;
	private GameObject fxPrefab;
	private GridSelector gridInfo;

	public override GridSelector AttackRangeInfo => gridInfo;

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		routineDone = false;
		
		timePassed += dt;
		if (timePassed > actionDuration)
		{
			routineDone = true;
		}
	}

	protected override void OnTrigger()
	{
		var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;

		foreach (var targetTile in map.GetTargetTiles(gridInfo, owner))
		{
			if (targetTile != null)
			{
				var target = map.GetBattleObjectOfTile(targetTile);
				Object.Instantiate(fxPrefab, targetTile.GetPosition(), Quaternion.identity);
				if (target?.ObjectType.IsHostile(owner.ObjectType) == true)
				{
					map.GetBattleObjectOfTile(targetTile).Damage(owner, owner.BattleStat.Attack);
				}
			}
		}
		

		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		throw new System.NotImplementedException();
	}

	public TestCardAction(TestCardActionData actionData)
	{
		actionDuration = actionData.actionDuration;
		fxPrefab = actionData.fxPrefab;
		gridInfo = actionData.actionRange;
	}
}