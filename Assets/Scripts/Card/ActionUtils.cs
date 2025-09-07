using System;
using System.Collections.Generic;

public static class ActionUtils
{
	public static IEnumerable<ITile> GetTargetTileWithTargetingInfo(object triggerInfo)
	{
		if (triggerInfo is not TargetingActionTriggerInfo ti)
		{
			throw new ArgumentException();
		}

		var targetTile = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.BattleMap
			.GetTileOfBattleObject(ti.Target);

		yield return targetTile;
	}
	
	public static IEnumerable<IBattleObject> GetTargetObjectsWithTargetingInfo(object triggerInfo)
	{
		if (triggerInfo is not TargetingActionTriggerInfo ti)
		{
			throw new ArgumentException();
		}

		yield return ti.Target;
	}
		
	public static IBattleObject GetTargetObjectWithTargetingInfo(object triggerInfo)
	{
		if (triggerInfo is not TargetingActionTriggerInfo ti)
		{
			throw new ArgumentException();
		}

		return ti.Target;
	}
}