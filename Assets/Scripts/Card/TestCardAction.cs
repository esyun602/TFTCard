using UnityEngine;

public class TestCardAction : CardActionBase
{
	private float timePassed = 0f;
	private float actionDuration;
	private GameObject fxPrefab;
	
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

		var targetTile = map.GetTileAt(row+2, col);
		if (targetTile != null)
		{
			GameObject.Instantiate(fxPrefab, targetTile.GetPosition(), Quaternion.identity);
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
	}
}