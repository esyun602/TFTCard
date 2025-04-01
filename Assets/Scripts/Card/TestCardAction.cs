using System.Collections;
using Coroutine;
using UnityEngine;

public class TestCardAction : CardActionBase
{
	private float timePassed = 0f;
	private float actionDuration;
	private GameObject fxPrefab;
	private GridSelector gridInfo;
	
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
		var (col, row) = map.GetTileCoord(map.GetTileOfBattleObject(owner));

		foreach (var cell in gridInfo.selectedCells)
		{
			var modifier = (1, 1);
			if (owner.ObjectType == ObjectType.Enemy && !gridInfo.isAbsolute)
			{
				modifier = (-1, 1);
			}
			var targetTile = map.GetTileAt(row + modifier.Item1 * cell.row, col + modifier.Item2 * cell.col);

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