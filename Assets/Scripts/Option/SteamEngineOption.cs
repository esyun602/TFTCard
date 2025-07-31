using MessageSystem;

public class SteamEngineOption : IOption
{
	private IBattleObject target;
	public int Level { get; set; }
	
	public SteamEngineOption(int level)
	{
		Level = level;
	}

	public void OnAdd(IBattleObject target)
	{
		this.target = target;
		NoticeSystem.Instance.Subscribe<UnitBattleValueChangeNotice>(OnBattleValueChange);
	}

	public void OnRemove()
	{
		NoticeSystem.Instance.Subscribe<UnitBattleValueChangeNotice>(OnBattleValueChange);
	}
	
	private void OnBattleValueChange(UnitBattleValueChangeNotice m)
	{
		if (m.Stat == target.UnitCardBattleStat && m.Type == BattleValueType.Burn && m.Diff > 0)
		{
			//todo: fix?
			target.UnitCardBattleStat.AddBuff(new ValueAddAttackBuff(Level));
		}
	}

}