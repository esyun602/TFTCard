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
		NoticeSystem.Instance.Subscribe<BattleValueChangeNotice>(OnBattleValueChange);
	}

	public void OnRemove()
	{
		NoticeSystem.Instance.Subscribe<BattleValueChangeNotice>(OnBattleValueChange);
	}
	
	private void OnBattleValueChange(BattleValueChangeNotice m)
	{
		if (m.Stat == target.UnitCardBattleStat && m.Type == BattleValueType.Burn && m.Diff > 0)
		{
			//todo: fix?
			target.UnitCardBattleStat.AddBuff(new ValueAddAttackBuff(Level));
		}
	}

}