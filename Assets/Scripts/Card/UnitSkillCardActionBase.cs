using System;

public abstract class UnitSkillCardActionBase : SkillCardActionBase
{
	protected new UnitSkillCardBattleStat BattleStat => (UnitSkillCardBattleStat)base.BattleStat;

	public override bool CanUse(ITile targetTile)
	{
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
		var tile = map.GetTileOfBattleObject(BattleStat.Owner);
		return base.CanUse(targetTile) && targetTile.HasSameRow(tile);
	}

	public override void SetCardStat(SkillCardStat stat)
	{
		if (stat is not UnitSkillCardStat) throw new ArgumentException();
		base.SetCardStat(stat);
	}

	public override void SetCardBattleStat(SkillCardBattleStat stat)
	{
		if (stat is not UnitSkillCardBattleStat) throw new ArgumentException();
		base.SetCardBattleStat(stat);
	}
}