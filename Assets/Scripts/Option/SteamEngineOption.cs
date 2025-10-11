using MessageSystem;

public class SteamEngineOption : IOption
{
	private IBattleObject target;
	public int Level { get; set; }
	private IBuff immuneBuff;
	
	public SteamEngineOption(int level)
	{
		Level = level;
	}

	public void OnAdd(IBattleObject target)
	{
		this.target = target;
		NoticeSystem.Instance.Subscribe<UnitBattleValueChangeNotice>(OnBattleValueChange);
		if (Level >= 3)
		{
			immuneBuff = new BurnImmuneBuff();
			target.UnitCardBattleStat.AddBuff(immuneBuff, this);
		}
	}

	public void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<UnitBattleValueChangeNotice>(OnBattleValueChange);
		if (immuneBuff != null)
		{
			target.UnitCardBattleStat.RemoveBuff<BurnImmuneBuff>(this);
			immuneBuff = null;
		}
	}
	
	private void OnBattleValueChange(UnitBattleValueChangeNotice m)
	{
		if (m.Stat == target.UnitCardBattleStat && m.Type == UnitValueType.Burn && m.Diff > 0)
		{
			UpdatableRoutine.CurrentRoutine.AddInterrupt(ExecuteReinforce, 0.5f);
		}
	}

	private void ExecuteReinforce()
	{
		if (Level <= 2)
		{
			target.UnitCardBattleStat.AddBuff(new ValueAddAttackBuff(Level));
		}
		else if(Level >= 3)
		{
			target.UnitCardBattleStat.AddBuff(new ValueAddAttackBuff(2));
		}
	}
}