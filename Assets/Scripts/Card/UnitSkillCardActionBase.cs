public abstract class UnitSkillCardActionBase : SkillCardActionBase
{
	public override bool CanUse(ITile targetTile)
	{
		var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
		var tile = map.GetTileOfBattleObject(stat.Owner);
		return base.CanUse(targetTile) && targetTile.HasSameRow(tile);
	}
}