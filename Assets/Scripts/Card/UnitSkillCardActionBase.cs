public abstract class UnitSkillCardActionBase : SkillCardActionBase
{
	public override bool CanUse(ITile targetTile)
	{
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
		var tile = map.GetTileOfBattleObject(battleStat.Owner);
		return base.CanUse(targetTile) && targetTile.HasSameRow(tile);
	}
}